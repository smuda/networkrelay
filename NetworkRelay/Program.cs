// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Allberg Konsult AB">
//   Allberg Konsult AB
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Allberg.NetworkRelay
{
    using System;
    using System.Linq;
    using System.Threading;

    using global::NetworkRelay;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("NetworkRelay (C) 2015 John Allberg");
            Console.WriteLine();

            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            Arguments arguments;
            try
            {
                arguments = new Arguments(args);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return;
            }

            var listeners = arguments.Ports.Select(port => new Listener(arguments.Hostname, port)).ToList();

            var threads = listeners.Select(listener => new Thread(listener.Listen) { IsBackground = true });

            foreach (var thread in threads)
            {
                thread.Start();
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// The print usage.
        /// </summary>
        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("nr.exe <hostname> <port> <port> <port>");
        }
    }
}
