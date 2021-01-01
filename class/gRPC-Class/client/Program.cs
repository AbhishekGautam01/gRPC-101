using Grpc.Core;
using System;
using System.Threading.Tasks;
using Greet;
using Sum;

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

            var greetingClient = new GreetingService.GreetingServiceClient(channel);
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
            var greetingResponse = greetingClient.Greet(request);
            Console.WriteLine(greetingResponse.Result);

            var sumClient = new SumService.SumServiceClient(channel);
            var add = new Addition()
            {
                First = 10,
                Second = 3
            };
            var sumRequest = new SumRequest()
            {
                Sum = add
            };
            var sumResponse = sumClient.Sum(sumRequest);
            Console.WriteLine(sumResponse);

            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }
}
