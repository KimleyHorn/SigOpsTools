import boto3
import configparser
import time
import pandas as pd
class AthenaQueryManager:
    def __init__(self):
        # Load configuration
        self.__config = configparser.ConfigParser()
        self.__config.read('config.ini')
        AWS_ACCESS_KEY = self.__config['aws']['aws_access_key_id']
        AWS_SECRET_KEY = self.__config['aws']['aws_secret_access_key']
        AWS_REGION = self.__config['aws']['region']
        S3_BUCKET_NAME = self.__config['aws']['aws_bucket_name']
        S3_OUTPUT_DIRECTORY = self.__config['athena']['cycle_time_output_location']
        SCHEMA_NAME = self.__config['athena']['table']
        temp_file_location: str = "athena_query_results.csv"
        athena_client = boto3.client(
            "athena",
            aws_access_key_id=AWS_ACCESS_KEY,
            aws_secret_access_key=AWS_SECRET_KEY,
            region_name=AWS_REGION,
        )
        query_response = athena_client.start_query_execution(
        QueryString="SELECT * FROM table",
        QueryExecutionContext={"Database": SCHEMA_NAME},
        ResultConfiguration={
            "OutputLocation": S3_OUTPUT_DIRECTORY,
            "EncryptionConfiguration": {"EncryptionOption": "SSE_S3"},
        },
        )
        while True:
            try:
                # This function only loads the first 1000 rows
                athena_client.get_query_results(
                    QueryExecutionId=query_response["QueryExecutionId"]
                )
                break
            except Exception as err:
                if "not yet finished" in str(err):
                    time.sleep(0.001)
                else:
                    raise err



                s3_client = boto3.client(
                    "s3",
                    aws_access_key_id=AWS_ACCESS_KEY,
                    aws_secret_access_key=AWS_SECRET_KEY,
                    region_name=AWS_REGION,
                )
                s3_client.download_file(
                    S3_BUCKET_NAME,
                    f"{S3_OUTPUT_DIRECTORY}/{query_response['QueryExecutionId']}.csv",
                    temp_file_location,
                )
                df = pd.read_csv(temp_file_location)


if __name__ == "__main__":
    athena_manager = AthenaQueryManager()
