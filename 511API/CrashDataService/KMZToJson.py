import fiona
import os
from shapely.geometry import shape, Point

def find_region(point, shapefile):
    with fiona.open(shapefile, 'r') as shp:
        for feature in shp:
            geom = shape(feature['geometry'])
            if geom.contains(point):
                return feature['properties']
    return None

def main():
    # Example points
    points = [
        {"name": "Metro Atlanta", "longitude": -84.3880, "latitude": 33.7890},
        {"name": "Metro Nashville", "longitude": -86.7816, "latitude": 36.1627}
    ]

    # Path to the shapefile
    shapefile = 'C:\\Users\\brandon.hall\\OneDrive - KH\\SigOps Tools\\511API\\CrashDataService\\Polygons.shp'

    # Set the SHAPE_RESTORE_SHX config option to YES
    os.environ['SHAPE_RESTORE_SHX'] = 'YES'

    for point_info in points:
        point = Point(point_info["longitude"], point_info["latitude"])
        region = find_region(point, shapefile)
        if region:
            print(f"The point {point_info['name']} is in the region: {region}")
        else:
            print(f"The point {point_info['name']} is not in any region.")

if __name__ == "__main__":
    main()
