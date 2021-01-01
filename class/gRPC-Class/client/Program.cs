using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dummy;
using Greet;

namespace client
{
    class Program
    {
        const string TARGET = "localhost:50051";
        static void Main(string[] args)
        {
            Channel channel = new Channel(TARGET, ChannelCredentials.Insecure);
            channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });

            var client = new GreetingService.GreetingServiceClient(channel);
            //It looks like we are directly calling the function but this call will happen over http/2 and will goto server. 
            var greeting = new Greeting()
            {
                FirstName = "Abhishek",
                LastName = "Gautam"
            };
            var request = new GreetingRequest()
            {
                Greeting = greeting
            };
            var response = client.Greet(request);
            Console.WriteLine(response.Result);
            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }
}
