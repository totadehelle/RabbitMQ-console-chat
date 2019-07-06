using System;
using System.Collections.Generic;

namespace ChatConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string username;
            
            Console.WriteLine("Welcome to the chat! What's your username?");
            username = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Your username should not be empty or whitespace! What's your username?");
                username = Console.ReadLine();
            }

            Receiver receiver = Receiver.Instance;
            Producer producer = new Producer();
            
            Console.WriteLine("{0}, now you can send messages! If you want to stop the session, send Q.", username);
            
            Message message = new Message();
            message.username = username;
            while (true)
            {
                while(string.IsNullOrWhiteSpace(message.text) || string.IsNullOrEmpty(message.text))
                {message.text = Console.ReadLine();}
                if(message.text == "Q") break;
                message.date = DateTime.Now;
                producer.Produce(message);
                message.text = null;
            }
            
            receiver.Dispose();
            Console.WriteLine("Your session is closed! Press Enter.");
            Console.ReadKey();
        }
    }
}