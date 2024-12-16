using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HumanLibrary;

namespace ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HumanController controller = new HumanController();

            Console.Write("Enter server (1) or client (2): ");
            int result = int.Parse(Console.ReadLine());
            
            try
            {
                if (result == 1)
                {
                    controller.ServerHuman = new Human { Age = 25, FullName = "John Olay", Gender = Gender.Man};
                    Server server = new Server(controller);
                    server.Run(5000).GetAwaiter().GetResult();
                }
                else
                {
                    controller.ClientHuman = new Human { Age = 56, FullName = "Bett Olsen", Gender = Gender.Woman};
                    Client client = new Client(controller);
                    client.Run(1000).GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}