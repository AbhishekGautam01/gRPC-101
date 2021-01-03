using Grpc.Core;
using System;
using System.Threading.Tasks;
using Greet;
using Sum;
using Calculator;
using System.Linq;

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
            var greetingRequest = new Greeting()
            {
                FirstName = "Abhishek",
                LastName = "Gautam"
            };
            var request = new GreetingRequest()
            {
                Greeting = greetingRequest
            };
            var unaryGreetingResponse = greetingClient.Greet(request);
            Console.WriteLine(unaryGreetingResponse.Result);
            #endregion
            #region SUM-Unary
            var sumClient = new SumService.SumServiceClient(channel);
            var additionRequest = new Addition()
            {
                First = 10,
                Second = 3
            };
            var sumRequest = new SumRequest()
            {
                Sum = additionRequest
            };
            var sumResponse = sumClient.Sum(sumRequest);
            Console.WriteLine(sumResponse);
            #endregion
            #region ServerStreaming 
            var serverStreamRequest = new GreetingManyTimesRequest() { Greeting = greetingRequest };
            var serverStreamResponse = greetingClient.GreetManyTimes(serverStreamRequest);
            while(await serverStreamResponse.ResponseStream.MoveNext())
            {
                Console.WriteLine(serverStreamResponse.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            #endregion
            #region CalculatorService-Streaming
            var calculatorClient = new calculator.calculatorClient(channel);
            var decomposePrimeRequest = new PrimeDecompositionRequest()
            {
                Input = 120
            };
            var decomposePrimeResponse = calculatorClient.PrimeDecomposition(decomposePrimeRequest);
            while(await decomposePrimeResponse.ResponseStream.MoveNext())
            {
                Console.WriteLine(decomposePrimeResponse.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
            #endregion
            #region Client Streaming - Long Greet 
            var longGreetRequest = new LongGreetRequest() { Greeting = greetingRequest };
            var longGreetStream = greetingClient.LongGreet();
            foreach (int i in Enumerable.Range(1, 10))
            {
                await longGreetStream.RequestStream.WriteAsync(longGreetRequest);
            }
            await longGreetStream.RequestStream.CompleteAsync();
            var longGreetResponse = await longGreetStream.ResponseAsync;
            Console.WriteLine(longGreetResponse);
            #endregion
            #region Client Streaming - Average
            var computeAverageStream = calculatorClient.ComputeAverage();
            foreach (int i in Enumerable.Range(1, 4))
            {
                await computeAverageStream.RequestStream.WriteAsync(new ComputeAverageRequest() { Input = i });
            }
            await computeAverageStream.RequestStream.CompleteAsync();
            var average = await computeAverageStream.ResponseAsync;
            Console.WriteLine($"Average: {average}");
            #endregion
            #region Bi-Di Greeting
            await DoGreetEveryone(greetingClient);
            #endregion
            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }

        public static async Task DoGreetEveryone(GreetingService.GreetingServiceClient client)
        {
            var stream = client.GreetEveryone();
            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Recieved: " + stream.ResponseStream.Current.Result);
                };
            });
            Greeting[] greetings =
            {
                new Greeting(){FirstName="Abhishek", LastName="Gautam"},
                new Greeting(){FirstName="Bob", LastName="Builder"}
            };
            foreach (var greeting in greetings)
            {
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest()
                {
                    Greeting = greeting
                });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }
    }
}
