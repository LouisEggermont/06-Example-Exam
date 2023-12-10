using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Howest.MCT
{
    public static class GetAllChildren
    {
        [FunctionName("GetAllChildren")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "child")] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");

            CosmosClientOptions options = new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            };
            List<Child> children = new List<Child>();
            var cosmosClient = new CosmosClient(connectionString,options);
            var container = cosmosClient.GetContainer("MealRegistrationDB", "Children");
            string sql = "SELECT * from c";
            var iterator = container.GetItemQueryIterator<Child>(sql);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    children.Add(item);
                }
            }

            return new OkObjectResult(children);
        }
    }
}
