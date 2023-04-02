using System.Linq.Expressions;
using System.Text;
using ExtractApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Discovery;
using Newtonsoft.Json;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        

        [TestInitialize] 
        public void Initialize()
        {

        }

        [TestMethod]
        [TestCategory("CSV")]
        [TestCategory("azFunctions")]
        [Description("Extract a CSV to SQL ASDL API => SqlBilkCopy")]
        public async Task ExtractCsv()
        {
            var bodyDictionary= new Dictionary<string, string>()
            {
                {"storageAccount", "bobawdatalakedev"},
                {"container", "raw"},
                {"directory", ""},
                {"fileName", "25k IMDb movie Dataset.csv"},
                {"sqlServer", "swat-dev.database.windows.net"},
                {"sqlDatabase", "SwatDW"},
                {"sqlTable", "stg.IMDB"}
            };

         

            var json = JsonConvert.SerializeObject(bodyDictionary);

            var body = new MemoryStream(Encoding.ASCII.GetBytes(json));

            var req = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = body

            };

            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            var response = await ExtractCSV.Run(req, logger);
            var result = (OkObjectResult)response;

            Console.WriteLine(result.Value );

        }
    }
}