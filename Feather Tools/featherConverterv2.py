import os
import pandas as pd

def feather_to_csv(feather_file_path: str, output_dir: str):
    # Load Feather file into a Pandas DataFrame
    df = pd.read_feather(feather_file_path)
    csv_file_path = os.path.join(output_dir, os.path.basename(feather_file_path).replace(".feather", ".csv"))
    # Save DataFrame to CSV file
    df.to_csv(csv_file_path, index=False)
    print(f"Conversion successful. CSV file saved at: {csv_file_path}")

def main():
    print("Please enter the directory path containing Feather files")
    directory_path = input("Directory path: ")
    
    if not os.path.isdir(directory_path):
        print("Invalid directory path.")
        return
    
    output_dir = os.path.join(directory_path, "ATSPM_Good_Detectors")
    os.makedirs(output_dir, exist_ok=True)
    
    for file_name in os.listdir(directory_path):
        if file_name.endswith(".feather"):
            feather_file_path = os.path.join(directory_path, file_name)
            feather_to_csv(feather_file_path, output_dir)

if __name__ == "__main__":
    main()
