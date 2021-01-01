using Greet;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        const int PORT = 50051;
        static void Main(string[] args)
        {
            Server server = null;
            try
            {
                server = new Server()
                {
                    Services = { GreetingService.BindService(new GreetingServiceImpl())}, 
                    Ports = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("The server is listening on the port: " + PORT);
                Console.ReadKey();
            }
            catch (IOException e)
            {
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
