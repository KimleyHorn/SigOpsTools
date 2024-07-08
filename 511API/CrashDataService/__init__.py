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

def load_config(filename):
    parser = configparser.ConfigParser()
    parser.read(filename)
    return parser

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

def insert_records_to_db(records):
    try:
        # Establish database connection
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor()

        # Insert each record
        for record in records:
            cursor.execute("""
                INSERT INTO YourTableName (Column1, Column2, Column3)
                VALUES (%s, %s, %s)
            """, (record['Column1'], record['Column2'], record['Column3']))

        # Commit the transaction
        conn.commit()
        logging.info("Records inserted successfully.")

    except mysql.connector.Error as err:
        logging.error(f"Error inserting records into database: {err}")
    finally:
        # Close the connection
        cursor.close()
        conn.close()

def main(mytimer: func.TimerRequest) -> None:
    utc_timestamp = datetime.datetime.utcnow().replace(
        tzinfo=datetime.timezone.utc).isoformat()

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

    for index, row in points_within_regions.iterrows():
        # Uncomment this if you need to filter by 'Subtype'
        # if 'Subtype' in row and pd.notna(row['Subtype']) and row['Subtype'] != "crash":
        #     continue

        if 'index_right' in row and pd.isna(row['index_right']):
            region = 'None (point is not within any region)'
        else:
            region = row.get('name', 'N/A')

        logging.info(f"Incident {index}:")

        id = row.get('ID', 'N/A')
        roadway_name = row.get('RoadwayName', 'N/A')
        reported = row.get('Reported', None)
        last_updated = row.get('LastUpdated', None)
        description = row.get('Description', 'N/A')
        latitude = row.get('Latitude', 'N/A')
        longitude = row.get('Longitude', 'N/A')
        travel_dir = row.get('TravelDirection', 'N/A')
        event_type = row.get('EventType', 'N/A')
        subtype = row.get('Subtype', None)
        lanesAffected = row.get('LanesAffected', None)
        detours = row.get('Detours', 'N/A')
        duration = row.get('Duration', 'N/A')

        logging.info(f"  Location: ({latitude}, {longitude})")
        logging.info(f"  Region: {region}")
        logging.info(f"  Description: {description}")
        logging.info(f"  Lanes Affected: {lanesAffected}")
        logging.info(f"  Type: {subtype}")

        if reported is not None:
            logging.info(f"  Reported at: {datetime.datetime.fromtimestamp(reported)}")
        else:
            logging.info("  Reported at: N/A")

        if last_updated is not None:
            logging.info(f"  Last updated at: {datetime.datetime.fromtimestamp(last_updated)}")
        else:
            logging.info("  Last updated at: N/A")

    logging.info('Python timer trigger function ran at %s', utc_timestamp)

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
