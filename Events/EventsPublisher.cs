using System.Text;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
namespace NotificationService.Events;

public interface IEventsPublisher
{
    public Task Publish<T>(T obj);

    public Task Publish(string raw);
}

public class EventsPublisher : IEventsPublisher
{
    private readonly IQueueClient _queueClient;

    public EventsPublisher(IQueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task Publish<T>(T obj)
    {
        var objAsText = JsonConvert.SerializeObject(obj);
        var message = new Message(Encoding.UTF8.GetBytes(objAsText));
        await _queueClient.SendAsync(message);
    }

    public async Task Publish(string raw)
    {
        var message = new Message(Encoding.UTF8.GetBytes(raw));
        await _queueClient.SendAsync(message);
    }
}