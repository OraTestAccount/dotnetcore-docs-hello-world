using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.InteropServices;

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
        string connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        string queueName = "sample-queue-testing"; 

        // Instantiate a QueueClient which will be used to create and manipulate the queue
        QueueClient queueClient = new QueueClient(connectionString, queueName);

        // Ensure the queue exists
        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            // Create a message and add it to the queue
            string message = "New homepage visit at " + DateTime.Now.ToString();
            queueClient.SendMessage(message);
        }  
    }
}
