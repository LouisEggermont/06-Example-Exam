using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Howes.MCT
{
    public static class GetMealClass
    {
        [FunctionName("GetMealClass")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mealbyclass/{classNr}")] HttpRequest req,
            ILogger log, string classNr)
        {
           var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");

            CosmosClientOptions options = new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            };

            var cosmosClient = new CosmosClient(connectionString,options);

            List<Meal> meals = new List<Meal>();
            var containerMeal = cosmosClient.GetContainer("MealRegistrationDB", "meals");

            List<Child> children = new List<Child>();
            var containerChild = cosmosClient.GetContainer("MealRegistrationDB", "Children");
            string sqlChild = $"SELECT * from c where c.class = '{classNr}'";
            var iteratorChild = containerChild.GetItemQueryIterator<Child>(sqlChild);
            while (iteratorChild.HasMoreResults)
            {
                var response = await iteratorChild.ReadNextAsync();
                foreach (var item in response)
                {
                    children.Add(item);
                    Console.WriteLine(item.Id);
                    string sqlMeal = $"SELECT * from c where c.childId = '{item.Id}'";
                    var iteratorMeal = containerMeal.GetItemQueryIterator<Meal>(sqlMeal);
                    while (iteratorMeal.HasMoreResults)
                    {
                        var responseMeal = await iteratorMeal.ReadNextAsync();
                        foreach (var itemMeal in responseMeal)
                        {
                            meals.Add(itemMeal);
                        }
                    }
                }
            }

            // List<Meal> meals = new List<Meal>();
            // var containerMeal = cosmosClient.GetContainer("MealRegistrationDB", "meals");
            // string sqlMeal = $"SELECT * from c where c.childId = '{childId}'";
            // var iteratorMeal = containerMeal.GetItemQueryIterator<Meal>(sql);
            // while (iteratorMeal.HasMoreResults)
            // {
            //     var response = await iteratorMeal.ReadNextAsync();
            //     foreach (var item in response)
            //     {
            //         meals.Add(item);
            //     }
            // }

            return new OkObjectResult(meals);
        }
    }
}
