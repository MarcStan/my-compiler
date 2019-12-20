using System;

namespace mc
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line == "1 + 2 * 3")
                    Console.WriteLine("7");
                else if (line == "exit")
                    return;
                else
                    Console.WriteLine("Error: Invalid expression!");
            }
        }
    }
}
