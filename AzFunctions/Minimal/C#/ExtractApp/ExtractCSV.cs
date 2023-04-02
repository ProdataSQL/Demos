using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Configuration;

namespace ExtractApp
{
    public static class ExtractCSV
    {

        [FunctionName("ExtractCSV")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            /* Set Parameters */
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string storageAccount  = data?.storageAccount;
            string container = data?.container;
            string directory = data?.directory;   
            string fileName = data?.fileName;
            string sqlServer = data?.sqlServer;
            string sqlDatabase = data?.sqlDatabase;
            string sqlTable = data?.sqlTable;
            var dt = new DataTable();
            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"); //Regex to identify and Split Double Quote

            // read config file if present. we use this for toggle for ExcludeManagedIdentityCredential
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json",optional:true, reloadOnChange:true)
                .AddEnvironmentVariables()
                .Build();


            /* Authenticate to ASDL.  Note that we have a setting to Exclude MSI or not*/
            var sw = new Stopwatch();
            sw.Start();
            var defaultAzureCredentialOptions = new DefaultAzureCredentialOptions()
            {
                ExcludeAzureCliCredential = true,
                ExcludeManagedIdentityCredential =
                    Convert.ToBoolean(config["ExcludeManagedIdentityCredential"] ?? "false"),
                ExcludeSharedTokenCacheCredential = true,
                ExcludeVisualStudioCredential = false,
                ExcludeAzurePowerShellCredential = true,
                ExcludeEnvironmentCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeInteractiveBrowserCredential = true
            };
            var resource = $"https://{storageAccount}.blob.core.windows.net";
            var dataLakeServiceClient = new DataLakeServiceClient(new Uri(resource), new DefaultAzureCredential(defaultAzureCredentialOptions));
            var fileSystemClient = dataLakeServiceClient.GetFileSystemClient(container);
            var directoryClient = fileSystemClient.GetDirectoryClient(directory);
            var fileClient = directoryClient.GetFileClient(fileName);

            Console.WriteLine("ExtractCSV - Sample AzFunction to Extract CSV and write to SQL");
            Console.WriteLine("Source: {0}", System.IO.Path.Combine(storageAccount!,container!, directory!, fileName!));
            Console.WriteLine("Target: {0}.{1}.{2}", sqlServer,sqlDatabase, sqlTable);

            Console.WriteLine("Connected to ASDL: {0} ms", sw.ElapsedMilliseconds);

            /* Authenticate to SQL. */
            var con = new SqlConnection()
            {
                ConnectionString = $"Server={sqlServer};Database={sqlDatabase}",
                AccessToken =  (await new DefaultAzureCredential(defaultAzureCredentialOptions).GetTokenAsync(new TokenRequestContext(new string[] { "https://database.windows.net//.default" }))).Token
            };
            await con.OpenAsync();
            Console.WriteLine("Connected to SQL: {0} ms", sw.ElapsedMilliseconds);
            sw.Restart();

            // Read CSV into DataTable
            var streamReader = new StreamReader(await fileClient.OpenReadAsync());
            string[] headers = (await streamReader.ReadLineAsync())!.Split(',');
            foreach (string header in headers)
            {
                dt.Columns.Add(header == "" ? "id" : header);
            }

            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                object[] columnArray = csvParser.Split(line!).ToArray<object>();
                var nr = dt.NewRow();

                //Todo: Add Logic for specialist Transform
                nr.ItemArray = columnArray;
                dt.Rows.Add(nr);
            }
            Console.WriteLine("Read CSV: {0} Rows in {1} ms", dt.Rows.Count, sw.ElapsedMilliseconds);
            sw.Restart();


            using (var sqlBulkCopy = new SqlBulkCopy(con))
            {
                sqlBulkCopy.DestinationTableName = sqlTable;
                await sqlBulkCopy.WriteToServerAsync(dt);
            }
            con.Close();
            con.Dispose();
            Console.WriteLine("Write SQL: {0} Rows in {1} ms", dt.Rows.Count, sw.ElapsedMilliseconds);
            sw.Restart();


            string responseMessage = "OK";
            return new OkObjectResult(responseMessage);
        }
    }
}
