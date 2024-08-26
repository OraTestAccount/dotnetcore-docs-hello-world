using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace dotnetcoresample.Pages;

public class IndexModel : PageModel
{

    public string OSVersion { get { return RuntimeInformation.OSDescription; }  }
    
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {      
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Storage connection string is not configured.");
            }

            string queueName = "sample-queue-testing";

            QueueClient queueClient = new QueueClient(connectionString, queueName);

            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                string message = "New homepage visit at " + DateTime.Now.ToString();
                queueClient.SendMessage(message);
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it accordingly
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Optionally, log the error to Application Insights or another logging service
        } 
    }
}
