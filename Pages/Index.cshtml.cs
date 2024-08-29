using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

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

            if (queueClient.Exists())
            {
                string message = "New homepage visit at " + DateTime.Now.ToString();
                string base64Message = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
                queueClient.SendMessage(base64Message);
                _logger.LogInformation("Base64-encoded message sent to queue: {Message}", base64Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to send a message to the queue.");
        } 
    }
}