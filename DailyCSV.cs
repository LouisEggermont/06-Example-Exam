using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


public static class DailyCSVFunction
{
    [FunctionName("DailyCSVFunction")]
    public static async Task RunAsync([TimerTrigger("0 1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        DateTime currentDate = DateTime.UtcNow;
        string containerName = $"m{currentDate.ToString("yyyyMMdd")}";
        string cosmosDate = currentDate.ToString("yyyy-MM-dd");

        // Your existing code for retrieving classes, writing CSV, and uploading to Blob Storage
        List<MyClass> classes = await RetrieveClassesAndMealsAsync(cosmosDate);
        string csvContent = WriteCsv(classes);
        await Upload(csvContent, containerName);
    }

    static async Task<List<MyClass>> RetrieveClassesAndMealsAsync(string date)
    {
            var connectionString = Environment.GetEnvironmentVariable("CosmosConectionString");
            CosmosClientOptions options = new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            };
            var cosmosClient = new CosmosClient(connectionString,options);

            var containerMeal = cosmosClient.GetContainer("MealRegistrationDB", "meals");
            var containerChild = cosmosClient.GetContainer("MealRegistrationDB", "Children");

            List<MyClass> classes = new List<MyClass>();

            string sqlClass = "SELECT DISTINCT c.class FROM c";
            var iteratorClass = containerChild.GetItemQueryIterator<MyClass>(sqlClass);
            while (iteratorClass.HasMoreResults)
            {
                var response = await iteratorClass.ReadNextAsync();
                foreach (var item in response)
                {
                    classes.Add(item);

                    string sqlChild = $"SELECT * from c where c.class = '{item.ClassName}'";
                    var iteratorChild = containerChild.GetItemQueryIterator<Child>(sqlChild);
                    while (iteratorChild.HasMoreResults)
                    {
                        var responseChild = await iteratorChild.ReadNextAsync();
                        foreach (var child in responseChild)
                        {
                            string sqlMeal = $"SELECT * from c where c.childId = '{child.Id}' and STARTSWITH(c.date, '{date}')";
                            var iteratorMeal = containerMeal.GetItemQueryIterator<Meal>(sqlMeal);
                            while (iteratorMeal.HasMoreResults)
                            {
                                var responseMeal = await iteratorMeal.ReadNextAsync();
                                foreach (var itemMeal in responseMeal)
                                {
                                    item.Meals.Add(itemMeal);
                                }
                            }
                        }
                    }
                }
            }
            return classes;
    }

    static string WriteCsv(List<MyClass> classes)
    {
                     // Define the CSV header
                string header = "Class,MealId,ChildId,Choice,Date";

                string csv = header + Environment.NewLine;


                // Iterate over each class
                foreach (var myClass in classes)
                {
                    // Iterate over meals in the class
                    foreach (var meal in myClass.Meals)
                    {
                        // Append CSV row for each meal
                        csv += $"{myClass.ClassName},{meal.Id},{meal.ChildId},{meal.Choice},{meal.Date}{Environment.NewLine}";
                    }
                }
                Console.WriteLine(csv);
                // Generate a unique file name based on the current timestamp
                string fileName = $"meals-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";

                // Write the CSV content to a file
                File.WriteAllText(fileName, csv);

                // Return the generated file name
                return fileName;   
    }

    static async Task Upload(string fileName, string containerName)
    {
                    string connectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    BlobContainerClient bc = blobServiceClient.GetBlobContainerClient(containerName);
                    bc.CreateIfNotExists(); 

                    BlobClient bb = bc.GetBlobClient(fileName);
                    bb.Upload(fileName);
    }
}
