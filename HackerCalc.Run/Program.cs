using System;
using System.IO;
using System.Text;

namespace HisRoyalRedness.com
{
    class Program
    {
        static void Main(string[] args)
        {
            //var input = args.Length == 0 ? "1I * ~(2U + 0.3) << 0X50 - .4 & !6 >> 7F | 8U64 \\ 9 ^ 10.1F - 11t + 12:33.123 * 23:44:12 / 1:23:45:36.32" : args[0];
            //var input = args.Length == 0 ? "1 + (2 _64 - 3u) * 4I_16 / 5U_4 + 6.0 - 7f + 0x88 * 92.345t" : args[0];
            var input = args.Length == 0 ? "1U + 0Xffi" : args[0];


            Console.WriteLine($"                1         2         3         4         5         6         7         8         9         ");
            Console.WriteLine($"       123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
            Console.WriteLine($"Input: {input}");

            Console.WriteLine("Parser");
            Console.WriteLine("-------");
            foreach (var token in Parser.ParseExpression(input))
                Console.WriteLine(token);

            Console.WriteLine();

            Console.WriteLine("Scanner");
            Console.WriteLine("-------");
            foreach (var token in Parser.ScanExpression(input))
                Console.WriteLine($"val: {token.val,-15}kind: {token.TokenKind.ToString().TrimStart('_')}");

        }
    }
}
