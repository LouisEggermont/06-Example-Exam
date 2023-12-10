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
    public static class AddMeal
    {
        [FunctionName("AddMeal")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "meal")] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");

            CosmosClientOptions options = new CosmosClientOptions()
            {
                ConnectionMode = ConnectionMode.Gateway
            };

            Meal meal = JsonConvert.DeserializeObject<Meal>(req.ReadAsStringAsync().Result);
            var cosmosClient = new CosmosClient(connectionString, options);
            var container = cosmosClient.GetContainer("MealRegistrationDB", "meals");
            var result = await container.CreateItemAsync<Meal>(meal, new PartitionKey(meal.ChildId));

            return new OkObjectResult(meal);
        }
    }
}
