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
    print(f"Incident {index}:")
    print(f"  Location: ({row['Latitude']}, {row['Longitude']})")
    if pd.isna(row['index_right']):
        print("  Region: None (point is not within any region)")
    else:
        # print(f"  Region: {row['region_column_name']}") Replace 'region_column_name' with the actual column name for regions
        print(row)




