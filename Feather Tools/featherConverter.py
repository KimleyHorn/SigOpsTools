import pandas as pd


## Converts a Feather file into a CSV file
# To use this function you must have the pandas library installed then add the feather file to this folder
# Next you change the feather file path to the name of the feather file you want to convert
# Then run the script and abra kedabra you have a csv file in the same folder as the feather file
# Example below is the feather file path for the ATSPM_Det_Config_Good (2).feather file
# @param feather_file_path The path to the Feather file
# @return None

#region Helper Methods

def feather_to_csv(feather_file_path: str):

    # Load Feather file into a Pandas DataFrame
    df = pd.read_feather(feather_file_path)
    csv_file_path = feather_file_path.replace(".feather", ".csv")
    # Save DataFrame to CSV file
    df.to_csv(csv_file_path, index=False)
    print(f"Conversion successful. CSV file saved at: {csv_file_path}")
#endregion

def main():
    print("Please enter the path to the Feather file")
    feather_file_path = input("Feather file path: ")
    while(not feather_file_path.endswith(".feather")):
        print("Invalid file path. Please enter a valid Feather file path")
        feather_file_path = input("Feather file path: ")
    feather_to_csv(feather_file_path)

if __name__ == "__main__":
    main()