﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace MultiClient
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 12345;

        static void Main()
        {
            Console.Title = "Database Client";
            Connect();
            MainLoop();
            Quit();
        }


        /// <summary>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        private static void Connect()
        {
            int attempts = 1;

            while (!ClientSocket.Connected)
            {
                try
                {
                    Console.WriteLine("Connection attempt " + attempts);
                    attempts++;
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }

        private static void MainLoop()
        {
            Console.WriteLine(@"Write your command (insert, update or find) followed by first name, last name and date of birth (YYYY/MM/DD).\n" +
                "Please separate each operation with a single space. Type 'quit' to quit the program.\n" +
                "E.g.: To insert an entry:\"insert John Doe 1956/01/22\"\n" +
                "to update an entry insert the ID before the name: \"update 34 John Doe 1966/01/22\"");

            while (true)
            {
                SendRequest();
                ReceiveResponse();
            }
        }

        /// <summary>
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        private static void Quit()
        {
            SendString("quit"); // Send quit command to server
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            Console.Write("Send a request: ");
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "quit")
            {
                Quit();
            }
        }

        /// <summary>
        /// Sends a string to the server with ASCII encoding.
        /// </summary>
        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private static void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);
        }
    }
}