using Greet;
using Grpc.Core;
using Sum;
using System;
using System.IO;
using Calculator;
using Sqrt;

namespace server
{
    public class Program
    {
        const int PORT = 50051;

        protected Program()
        {
        }

        static void Main(string[] args)
        {
            Server server = null;
            try
            {
                server = new Server()
                {
                    Services = { GreetingService.BindService(new GreetingServiceImpl()), 
                                 SumService.BindService(new SumServiceImpl()),
                                 calculator.BindService(new CalculatorServiceImpl()),
                                 SqrtService.BindService(new SqrtServiceImpl())},
                    Ports = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("The server is listening on the port: " + PORT);
                Console.ReadKey();
            }
            catch (IOException e){
                Console.WriteLine("Exception: " + e.Message);
                throw;
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}
