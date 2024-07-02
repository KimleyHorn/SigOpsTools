import datetime
import logging
import requests
import configparser
import os
import geopandas as gpd
import pandas as pd
from shapely.geometry import Point
import azure.functions as func

def main(mytimer: func.TimerRequest) -> None:
    utc_timestamp = datetime.datetime.utcnow().replace(
        tzinfo=datetime.timezone.utc).isoformat()

    if mytimer.past_due:
        logging.info('The timer is past due!')

    # Path to the shapefile
    shapefile = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'
    gdf = gpd.read_file(shapefile)

    # Check if the CRS is defined
    if gdf.crs is None:
        logging.info("CRS is not defined. Setting CRS...")
        # Set the CRS (replace 'EPSG:4326' with the appropriate EPSG code for your data)
        gdf = gdf.set_crs('EPSG:4326')

    def add_names():
        gdf['name'] = ''
        for index, row in gdf.iterrows():
            gdf.at[index, 'name'] = f"Region {index + 1}"

    add_names()
    # Set the SHAPE_RESTORE_SHX config option to YES
    os.environ['SHAPE_RESTORE_SHX'] = 'YES'

    # Read the configuration file
    config = configparser.ConfigParser()
    config.read('config.ini')

    # API endpoint
    base_url = "https://511ga.org/api/v2/get/event"
    params = {
        "key": config['FiveOneOne']['api_key'],
        "format": "json"
    }

    # Make the GET request with parameters
    response = requests.get(base_url, params=params)

    # Check if the request was successful
    if response.status_code == 200:
        # Parse the JSON response
        data = response.json()
        # Print the JSON data
        # JSON data should be accessed like data[index]["key"]
    else:
        logging.error(f"Failed to retrieve data. HTTP Status code: {response.status_code}")
        return

    points = gpd.GeoDataFrame(data,
        geometry=[Point(points['Longitude'], points['Latitude']) for points in data],
        crs="EPSG:4326")  # Ensure the CRS matches your shapefile

    # Perform spatial join to check if points are within regions
    points_within_regions = gpd.sjoin(points, gdf, how="left")

    for index, row in points_within_regions.iterrows():
        # Check if 'Subtype' exists and filter out non-crash subtypes
        if 'Subtype' in row and pd.notna(row['Subtype']) and row['Subtype'] != "crash":
            continue

        # Ensure 'index_right' exists and handle missing values
        if 'index_right' in row and pd.isna(row['index_right']):
            region = 'None (point is not within any region)'
        else:
            region = row.get('name', 'N/A')  # Replace 'name' with the actual column name for regions

        logging.info(f"Incident {index}:")

        latitude = row.get('Latitude', 'N/A')
        longitude = row.get('Longitude', 'N/A')
        description = row.get('Description', 'N/A')
        reported = row.get('Reported', None)
        type = row.get('Subtype', None)
        lanesAffected = row.get('LanesAffected', None)
        last_updated = row.get('LastUpdated', None)

        logging.info(f"  Location: ({latitude}, {longitude})")
        logging.info(f"  Region: {region}")
        logging.info(f"  Description: {description}")
        logging.info(f"  Lanes Affected: {lanesAffected}")
        logging.info(f"  Type: {type}")

        if reported is not None:
            logging.info(f"  Reported at: {datetime.datetime.fromtimestamp(reported)}")
        else:
            logging.info("  Reported at: N/A")

        if last_updated is not None:
            logging.info(f"  Last updated at: {datetime.datetime.fromtimestamp(last_updated)}")
        else:
            logging.info("  Last updated at: N/A")

    logging.info('Python timer trigger function ran at %s', utc_timestamp)
