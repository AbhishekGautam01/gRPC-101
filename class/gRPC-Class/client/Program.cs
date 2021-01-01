using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dummy;

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

            var client = new DummyService.DummyServiceClient(channel);
            channel.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }
}
