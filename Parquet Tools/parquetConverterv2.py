import os
import pandas as pd

def process_parquet_files(directory_path: str, aggregate_output_csv_path: str = None):
    dataframes = []

    for filename in os.listdir(directory_path):
        if filename.endswith(".parquet"):
            parquet_file_path = os.path.join(directory_path, filename)
            csv_file_path = parquet_file_path.replace(".parquet", ".csv")

            # Load Parquet file into a Pandas DataFrame using Apache Arrow
            df = pd.read_parquet(parquet_file_path, engine='pyarrow')
            # Save individual DataFrame to a CSV file
            df.to_csv(csv_file_path, index=False)
            print(f"Converted {parquet_file_path} to {csv_file_path}")

            # Append the DataFrame to the list for aggregation
            dataframes.append(df)

    if aggregate_output_csv_path and dataframes:
        # Concatenate all DataFrames in the list
        aggregated_df = pd.concat(dataframes, ignore_index=True)
        # Save the aggregated DataFrame to a CSV file
        aggregated_df.to_csv(aggregate_output_csv_path, index=False)
        print(f"Aggregated CSV saved to {aggregate_output_csv_path}")

def main():
    print("Please enter the path to the folder containing Parquet files:")
    directory_path = input("Directory path: ")

    print("If you want to aggregate all Parquet files into a single CSV, please enter the path for the output CSV file (or press Enter to skip):")
    aggregate_output_csv_path = input("Output CSV path: ")

    if not aggregate_output_csv_path:
        aggregate_output_csv_path = None

    process_parquet_files(directory_path, aggregate_output_csv_path)

if __name__ == "__main__":
    main()
