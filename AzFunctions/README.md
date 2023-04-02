# Sample C# Azure Function
Welcome to the sample c# Azure functions. This contains two sample projects written in Visual Studio 2019:
- ExtractyApp.csproj. A sample Azure function which reads a CSV using the DataLakeClient API and writes to a SQL Server using SqlBulkCopy API
- TestProject1.csproj. A sample C# unit test project that shows how make a unit test for an azure function using a mock HTTP request object.

## Key concepts to demo
- A template for writing c# Azure function that use Datalakes and SQL
- Example of using the latest Azure.Identity for both Managed Identity when run from Azure and Visual Studio Azure Identity when run from Visual Studio.
- Example use of the DataLakeServiceClient instead of the more generic BlobServiceClient.
- Example use of SqlBulkCopy API to write a Dataset from Dot.Ney to SQL with a Bulk API.
- Sample Dot.Net Unit Test project showing how to write unit tests to develop and test functions locally so you dont have to mess with debugging as much in Azure.
- Use of a local.settings.json file and Envioronment variables to have different settings in VS Studio versus Azure. Specificaaly the ExcludeManagedIdentityCredential boolean flag.

## Why use an Azure Function 
Theres a few scenarios where Azure Functions can be useful with data engineering, even if you are using ADF.
- When the source file needs some high code solution to parse.
- C# does allow for more streamlined file procesing. At the extepse of development time. This can lead to better performance. More than 2x in this case of a 12MN file
- When the low code ADF envionment is not flexible enough.


## Performanc Testing ADF Copy v c# Azure function
As a performnance test we took a sample 12 MB CSV File which you can download from kaggle here
https://www.kaggle.com/datasets/utsh0dey/25k-movie-dataset

We then read the file and loaded it into SQL using two scenarios
- The Standard Copy Activity using the Sqlpool COPY option.
- This sample c# azure function.

Avergage ruin times are show below. Potentially the c# azure funciton was more than 2x faster. We should not that we awere also using the most basic tier of azure funcitions. The "consumption" plan is abotu 1 core and 1.5GB ram so really limited in resources.
image.png
