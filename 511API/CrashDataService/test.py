import geopandas as gpd

# Load the GeoJSON file
shapefile_path = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'

# Path to your shapefile
# shapefile_path = 'path/to/your/shapefile.shp'

# Load the shapefile
gdf = gpd.read_file(shapefile_path)

# Print the attribute table
print(gdf.columns)



azurite --location ./AzuriteTeamsBot --blobHost 127.0.0.1 --blobPort 10000 --queuePort 10001 --tablePort 10002
azurite --location ./Azurite511Service --blobHost 127.0.0.2 --blobPort 10000 --queuePort 10001 --tablePort 10002
