import requests
import configparser
import fiona
import os
import geopandas as gpd
import pandas as pd
from shapely.geometry import shape, Point
from datetime import datetime, timedelta


# Path to the shapefile
shapefile = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'
gdf = gpd.read_file(shapefile)

# Check if the CRS is defined
if gdf.crs is None:
    print("CRS is not defined. Setting CRS...")
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
    print(f"Failed to retrieve data. HTTP Status code: {response.status_code}")

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
    
    print(f"Incident {index}:")
    
    latitude = row.get('Latitude', 'N/A')
    longitude = row.get('Longitude', 'N/A')
    description = row.get('Description', 'N/A')
    reported = row.get('Reported', None)
    type = row.get('Subtype', None)
    last_updated = row.get('LastUpdated', None)

    print(f"  Location: ({latitude}, {longitude})")
    print(f"  Region: {region}")
    print(f"  Description: {description}")
    print(f"  Type: {type}")
    
    if reported is not None:
        print(f"  Reported at: {datetime.fromtimestamp(reported)}")
    else:
        print("  Reported at: N/A")
    
    if last_updated is not None:
        print(f"  Last updated at: {datetime.fromtimestamp(last_updated)}")
    else:
        print("  Last updated at: N/A")