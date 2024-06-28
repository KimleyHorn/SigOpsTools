import geopandas as gpd

# Load the GeoJSON file
geojson_path = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'
gdf = gpd.read_file(geojson_path)

# Print the first few rows of the GeoDataFrame to inspect the data
print(gdf.head())

# Print the column names to see what attributes are available
print("Columns in the GeoJSON file:", gdf.columns)

# Print the CRS of the GeoJSON file
print("CRS of the GeoJSON file:", gdf.crs)
