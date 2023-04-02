# Sample C# Azure Function
Welcome to the sample c# Azure functions. This contains two sample projects written in Visual Studio 2019:
- ExtractyApp.csproj. A sample Azure function which reads a CSV using the DataLakeClient API and writes to a SQL Server using SqlBulkCopy API
- TestProject1.csproj. A sample C# unit test project that shows how make a unit test for an azure function using a mock HTTP request object.

## Key concepts to demo ?
- A template for writing c# Azure function that use Datalakes and SQL
- Example of using the latest Azure.Identity for both Managed Identity when run from Azure and Visual Studio Azure Identity when run from Visual Studio.
- Example use of the DataLakeServiceClient instead of the more generic BlobServiceClient.
- Example use of SqlBulkCopy API to write a Dataset from Dot.Ney to SQL with a Bulk API.

## Contents

