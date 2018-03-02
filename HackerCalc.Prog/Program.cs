using System;
using System.IO;
using System.Text;

namespace HisRoyalRedness.com
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args.Length == 0 ? "1 * ~(2U + 0.3) << 0x5 - .4 & !6 >> 7F | 8I \\ 9 ^ 10.1" : args[0];
            Console.WriteLine($"Input: {input}");

            foreach (var token in Parser.ParseExpression(input))
                Console.WriteLine(token);
        }
    }
}
