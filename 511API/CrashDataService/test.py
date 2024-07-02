import geopandas as gpd

# Load the GeoJSON file
shapefile_path = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'

# Path to your shapefile
# shapefile_path = 'path/to/your/shapefile.shp'

# Load the shapefile
gdf = gpd.read_file(shapefile_path)

# Print the attribute table
print(gdf.columns)
