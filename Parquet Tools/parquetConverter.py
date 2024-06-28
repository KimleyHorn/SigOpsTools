import pandas as pd

def parquet_to_csv(parquet_file_path: str):
    # Load Parquet file into a Pandas DataFrame
    df = pd.read_parquet(parquet_file_path, engine='pyarrow')
    csv_file_path = parquet_file_path.replace(".parquet", ".csv")
    # Save DataFrame to CSV file
    # Save DataFrame to CSV file
    df.to_csv(csv_file_path, index=False)
    print(f"CSV file saved to {csv_file_path}")


def main():
    print("Please enter the path to the folder of parquet files")
    parquet_file_path = input("Parquet file path: ")
    


    while(not parquet_file_path.endswith(".parquet")):
        print("Invalid file path. Please enter a valid parquet file path")
        parquet_file_path = input("parquet file path: ")
    parquet_to_csv(parquet_file_path)

if __name__ == "__main__":
    main()