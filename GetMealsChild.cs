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
    public static class GetMealsChild
    {
        [FunctionName("GetMealsChild")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mealbychild/{childId}")] HttpRequest req,
            ILogger log, string childId)
        {
            var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");

            CosmosClientOptions options = new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            };
            List<Meal> meals = new List<Meal>();
            var cosmosClient = new CosmosClient(connectionString,options);
            var container = cosmosClient.GetContainer("MealRegistrationDB", "meals");
            string sql = $"SELECT * from c where c.childId = '{childId}'";
            var iterator = container.GetItemQueryIterator<Meal>(sql);
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    meals.Add(item);
                }
            }

            return new OkObjectResult(meals);
        }
    }
}
