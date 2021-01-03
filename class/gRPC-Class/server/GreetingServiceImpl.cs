using Greet;
using Grpc.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Greet.GreetingService;

namespace server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);
            return Task.FromResult(new GreetingResponse() { Result = result });
        }

        public override async Task GreetManyTimes(GreetingManyTimesRequest request, IServerStreamWriter<GreetingManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server recieved the request: ");
            Console.WriteLine(request.ToString());

            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);
            foreach (int i in Enumerable.Range(1, 10))
            {
                await responseStream.WriteAsync(new GreetingManyTimesResponse() { Result = result });
            }
        }

        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            var result = new StringBuilder();
            while (await requestStream.MoveNext())
            {
                result.AppendLine($"Hello {requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}");

            }
            return new LongGreetResponse() { Result = result.ToString() };
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetingEveryoneResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var result = $"Hello {requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName} {Environment.NewLine}";
                await responseStream.WriteAsync(new GreetingEveryoneResponse() { Result = result });
            }

        }

        public override async Task<GreetingResponse> Greet_With_Deadline(GreetingRequest request, ServerCallContext context)
        {
            await Task.Delay(300);
            return new GreetingResponse() { Result = $"hello {request.Greeting.FirstName}" };
        }
    }
}
