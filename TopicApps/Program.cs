using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace TopicApps
{
    class Program
    {
        static string connectionString = "Endpoint=sb://hp-broker.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iMRvm/aqGV0MGJfBwJegAGG52rMRwdobpp0E3vNzJec=";
        static string path = "hpTopic";
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Name: ");
            var username = Console.ReadLine();
            var manager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!manager.TopicExists(path))
                manager.CreateTopic(path);
            var description = new SubscriptionDescription(path, username);
            manager.CreateSubscription(description);

            var factory = MessagingFactory.CreateFromConnectionString(connectionString);
            var topicClient = factory.CreateTopicClient(path);
            var subscriptionClient = factory.CreateSubscriptionClient(path, username);

            subscriptionClient.OnMessage(msg => ProcessMessage(msg));
            var hellomessage = new BrokeredMessage("Has entered the Room...");
            hellomessage.Label = username;
            topicClient.Send(hellomessage);
            while (true)
            {
                string text = Console.ReadLine();
                if (text.Equals("exit")) break;
                var chatMessage = new BrokeredMessage(text);
                chatMessage.Label = username;
                topicClient.Send(chatMessage);
            }

            var goodbyeMessage = new BrokeredMessage("Has left the building.....");
            goodbyeMessage.Label = username;
            topicClient.Send(goodbyeMessage);
            factory.Close();
        }


        static void ProcessMessage(BrokeredMessage message)
        {
            string user = message.Label;
            string text = message.GetBody<string>();
            Console.WriteLine(user + ">" + text);
        }

    }
}

