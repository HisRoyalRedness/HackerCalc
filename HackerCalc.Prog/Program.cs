using System;
using System.IO;
using System.Text;

namespace HisRoyalRedness.com
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args.Length == 0 ? "1 * ~(2 + 3) << 0x5 - 4 & !6 >> 7 | 8 \\ 9" : args[0];
            Console.WriteLine($"Input: {input}");

            foreach (var token in Parser.ParseExpression(input))
                Console.WriteLine(token);
        }
    }
}
