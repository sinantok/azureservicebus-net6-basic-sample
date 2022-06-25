using CoreLibrary;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
// See https://aka.ms/new-console-template for more information



Console.WriteLine("Hello, World!");


ConsumeSub<ContentCreatedEvent>(Constants.ContentTopic, Constants.ContentCreatedSubName, i =>
{
    Console.WriteLine($"ContentCreatedEvent ReceivedMessage with id: {i.Id}, ContentType: {i.ContentType}");
}).Wait();


ConsumeSub<ContentDeletedEvent>(Constants.ContentTopic, Constants.ContentDeletedSubName, i =>
{
    Console.WriteLine($"ContentDeletedEvent ReceivedMessage with id: {i.Id}");
}).Wait();

Console.WriteLine("\n\n\n");

Console.ReadLine();



static async Task ConsumeSub<T>(string topicName, string subName, Action<T> receivedAction)
{
    ISubscriptionClient client = new SubscriptionClient(Constants.ConnectionString, topicName, subName);

    client.RegisterMessageHandler(async (message, ct) =>
    {
        var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));

        receivedAction(model);

        await Task.CompletedTask;
    },
    new MessageHandlerOptions(i => Task.CompletedTask));
    
    Console.WriteLine($"{typeof(T).Name} is listening....");
}
