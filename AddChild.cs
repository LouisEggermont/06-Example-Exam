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
    public static class AddChild
    {
        [FunctionName("AddChild")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "child")] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");

            CosmosClientOptions options = new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            };

            Child child = JsonConvert.DeserializeObject<Child>(req.ReadAsStringAsync().Result);
            var cosmosClient = new CosmosClient(connectionString,options);
            var container = cosmosClient.GetContainer("MealRegistrationDB", "Children");
            var result = await container.CreateItemAsync<Child>(child, new PartitionKey(child.Class));

            return new OkObjectResult(child);
        }
    }
}
