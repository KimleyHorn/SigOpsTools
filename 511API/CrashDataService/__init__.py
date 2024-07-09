import datetime
import logging
import requests
import configparser
import os
import geopandas as gpd
import pandas as pd
from shapely.geometry import Point
import azure.functions as func
import mysql.connector


class incident():
    def __init__(self, id, roadway_name, region, reported, last_updated, description, latitude, longitude, travel_dir, event_type, subtype, lanesAffected, detours):
        self.ID = id
        self.roadway_name = roadway_name
        self.region = region
        self.reported = reported
        self.last_updated = last_updated
        self.description = description
        self.latitude = latitude
        self.longitude = longitude
        self.travel_dir = travel_dir
        self.event_type = event_type
        self.subtype = subtype
        self.lanesAffected = lanesAffected
        self.detours = detours

    def __repr__(self):
        return f"incident({self.ID}, {self.roadway_name}, {self.reported}, {self.last_updated}, {self.description}, {self.latitude}, {self.longitude}, {self.travel_dir}, {self.event_type}, {self.subtype}, {self.lanesAffected}, {self.detours})"

    def __str__(self):
        return f"ID: {self.ID}\nRoadway Name: {self.roadway_name}\nReported: {self.reported}\nLast Updated: {self.last_updated}\nDescription: {self.description}\nLocation: ({self.latitude}, {self.longitude})\nTravel Direction: {self.travel_dir}\nEvent Type: {self.event_type}\nSubtype: {self.subtype}\nLanes Affected: {self.lanesAffected}\nDetours: {self.detours}"

def id_exists(cursor, table_name, record_id):
    cursor.execute(f"SELECT 1 FROM {table_name} WHERE ID = %s", (record_id,))
    return cursor.fetchone() is not None

def updated(cursor, table_name, record_id, new_last_updated):
    cursor.execute(f"SELECT LastUpdated FROM {table_name} WHERE ID = %s", (record_id,))
    result = cursor.fetchone()
    if result:
        existing_last_updated = result[0]
        return new_last_updated > existing_last_updated
    return False


def log_info(inc):
    utc_timestamp = datetime.datetime.utcnow().replace(tzinfo=datetime.timezone.utc).isoformat()
    logging.info(f"  ID: {inc.ID}")
    logging.info(f"  Roadway Name: {inc.roadway_name}")
    if inc.reported is not None:
        logging.info(f"  Reported at: {inc.reported}")
    else:
        logging.info("  Reported at: N/A")
    if inc.last_updated is not None:
        logging.info(f"  Last updated at: {inc.last_updated}")
    else:
        logging.info("  Last updated at: N/A")
    logging.info(f"  Location: ({inc.latitude}, {inc.longitude})")
    logging.info(f"  Travel Direction: {inc.travel_dir}")
    logging.info(f"  Event Type: {inc.event_type}")
    logging.info(f"  Region: {inc.region}")
    logging.info(f"  Description: {inc.description}")
    logging.info(f"  Lanes Affected: {inc.lanesAffected}")
    logging.info(f"  Type: {inc.subtype}")
    # logging.info(f"  Detours: {detours}")
    logging.info('Python timer trigger function ran at %s', utc_timestamp)

def load_config(filename):
    parser = configparser.ConfigParser()
    parser.read(filename)
    return parser

def get_value(row, key, default=None):
    return row.get(key, default)

config_filename = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\config.ini'
config_parsed = load_config(config_filename)

# Check for database configuration
if 'Database' not in config_parsed or any(key not in config_parsed['Database'] for key in ['host', 'user', 'password', 'database']):
    logging.error("Configuration file is missing the 'Database' section or one of the required keys.")
    exit(1)

db_config = {
    'host': config_parsed['Database']['host'],
    'user': config_parsed['Database']['user'],
    'password': config_parsed['Database']['password'],
    'database': config_parsed['Database']['database']
}

# Function to insert records into the database
def insert_records_to_db(records, db_config):
    try:
        # Establish database connection
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor()
        table_name = "incidents"

        # Insert each record
        for record in records:
            if id_exists(cursor, table_name, record.ID):
                if updated(cursor, table_name, record.ID, record.last_updated):
                    try:
                        cursor.execute(f"""
                            UPDATE {table_name}
                            SET LastUpdated = %s, Description = %s, LanesAffected = %s, Detours = %s
                            WHERE ID = %s
                        """, (record.last_updated, record.description, record.lanesAffected, record.detours, record.ID))
                        logging.info(f"Record {record.ID} updated successfully.")
                    except mysql.connector.Error as update_err:
                        logging.error(f"Error updating record {record.ID}: {update_err}")
                else:
                    logging.info(f"ID {record.ID} already exists and is up-to-date. Skipping update.")
                continue

            try:
                cursor.execute(f"""
                    INSERT INTO {table_name} (ID, RoadwayName, Region, DateReported, LastUpdated, Description, Latitude, Longitude, DirectionOfTravel, EventType, Subtype, LanesAffected, Detours)
                    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
                """, (record.ID, record.roadway_name, record.region, record.reported, record.last_updated, record.description,
                record.latitude, record.longitude, record.travel_dir, record.event_type, record.subtype, record.lanesAffected, record.detours))
                logging.info(f"Record {record.ID} inserted successfully.")
            except mysql.connector.Error as insert_err:
                logging.error(f"Error inserting record {record.ID}: {insert_err}")

        # Commit the transaction
        conn.commit()
        logging.info("Records processed successfully.")

    except mysql.connector.Error as err:
        logging.error(f"Error processing records in database: {err}")
    finally:
        # Close the connection
        cursor.close()
        conn.close()

def main(mytimer: func.TimerRequest) -> None:
    if mytimer.past_due:
        logging.info('The timer is past due!')

    # Check for API key configuration
    if 'FiveOneOne' not in config_parsed or 'api_key' not in config_parsed['FiveOneOne']:
        logging.error("Configuration file is missing the 'FiveOneOne' section or 'api_key'.")
        return

    # Path to the shapefile
    shapefile_relative = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'
    gdf = gpd.read_file(shapefile_relative)

    # Check if the CRS is defined
    if gdf.crs is None:
        logging.info("CRS is not defined. Setting CRS...")
        gdf = gdf.set_crs('EPSG:4326')

    def add_names():
        gdf['name'] = ''
        for index, row in gdf.iterrows():
            gdf.at[index, 'name'] = f"Region {index + 1}"

    add_names()
    os.environ['SHAPE_RESTORE_SHX'] = 'YES'

    # API endpoint
    base_url = "https://511ga.org/api/v2/get/event"
    params = {
        "key": config_parsed['FiveOneOne']['api_key'],
        "format": "json"
    }

    # Make the GET request with parameters
    response = requests.get(base_url, params=params)

    # Check if the request was successful
    if response.status_code == 200:
        data = response.json()
    else:
        logging.error(f"Failed to retrieve data. HTTP Status code: {response.status_code}")
        return

    points = gpd.GeoDataFrame(data,
        geometry=[Point(point['Longitude'], point['Latitude']) for point in data],
        crs="EPSG:4326")

    # Perform spatial join to check if points are within regions
    points_within_regions = gpd.sjoin(points, gdf, how="left")
    incident_log = []
    for index, row in points_within_regions.iterrows():
        # Uncomment this if you need to filter by 'Subtype'
        # if 'Subtype' in row and pd.notna(row['Subtype']) and row['Subtype'] != "crash":
        #     continue

        if 'index_right' in row and pd.isna(row['index_right']):
            region = 'None'
        else:
            region = row.get('name', 'N/A')

        logging.info(f"Incident {index}:")

        ID = row.get('ID', 'N/A')
        roadway_name = row.get('RoadwayName', 'N/A')
        reported = datetime.datetime.fromtimestamp(row['Reported']) if 'Reported' in row and pd.notna(row['Reported']) else None
        last_updated = datetime.datetime.fromtimestamp(row['LastUpdated']) if 'LastUpdated' in row and pd.notna(row['LastUpdated']) else None
        description = row.get('Description', 'N/A')
        latitude = row.get('Latitude', 'N/A')
        longitude = row.get('Longitude', 'N/A')
        travel_dir = row.get('DirectionOfTravel', 'N/A')
        event_type = row.get('EventType', 'N/A')
        subtype = row.get('Subtype', None)
        lanesAffected = row.get('LanesAffected', None)
        detours = row.get('DetourInstructions', 'N/A')
        # TODO Calculate duration in database after figuring out how events are updated
        # duration = row.get('Duration', 'N/A') 
        # use row.get to create a new incident object
        inc = incident(ID, roadway_name, region, reported, last_updated, description, latitude, longitude, travel_dir, event_type, subtype, lanesAffected, detours)
        incident_log.append(inc)
        log_info(inc)
    insert_records_to_db(incident_log, db_config)

if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    class TimerRequest(func.TimerRequest):
        def __init__(self, past_due=False):
            self.past_due = past_due
            self.schedule_status = func._timer.ScheduleStatus()
            self.schedule_status.last = datetime.datetime.utcnow()
            self.schedule_status.next = datetime.datetime.utcnow() + datetime.timedelta(hours=1)

    mytimer = TimerRequest(past_due=False)
    main(mytimer)
