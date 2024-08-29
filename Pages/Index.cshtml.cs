using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace dotnetcoresample.Pages;

public class IndexModel : PageModel
{
    public string OSVersion { get { return RuntimeInformation.OSDescription; } }
    
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {      
        try
        {
            _logger.LogInformation("OnGet method called.");

            string connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Storage connection string is not configured.");
            }

            string queueName = "sample-queue-testing";

            QueueClient queueClient = new QueueClient(connectionString, queueName);

            queueClient.CreateIfNotExists();

            bool queueExists = queueClient.Exists();
            _logger.LogInformation("Queue client exists: {Exists}", queueExists);

            if (queueExists)
            {
                string message = "New homepage visit at " + DateTime.Now.ToString();
                queueClient.SendMessage(message);
                _logger.LogInformation("Message sent to queue: {Message}", message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to send a message to the queue.");
        } 
    }
}
