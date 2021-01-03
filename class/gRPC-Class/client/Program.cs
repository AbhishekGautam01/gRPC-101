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
        static async Task Main(string[] args)
        {
            Channel channel = new Channel(TARGET, ChannelCredentials.Insecure);
            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The client connected successfully");
            });
            #region Greeting-Unary
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
            #endregion
            #region ServerStreaming 
            var streamRequest = new GreetingManyTimesRequest() { Greeting = greeting };
            var streamResponse = greetingClient.GreetManyTimes(streamRequest);
            while(await streamResponse.ResponseStream.MoveNext())
            {
                Console.WriteLine(streamResponse.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            #endregion
            #region SUM-Unary
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
            #endregion
            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }
}
