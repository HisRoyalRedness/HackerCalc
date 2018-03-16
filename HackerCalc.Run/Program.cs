using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
            var input = args.Length == 0 ? " 1 + 2" : args[0];


            Console.WriteLine($"                1         2         3         4         5         6         7         8         9         ");
            Console.WriteLine($"       123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
            Console.WriteLine($"Input: {input}");

            Console.WriteLine();

            Console.WriteLine("Parser");
            Console.WriteLine("-------");
            var rootToken = Parser.ParseExpression(input);
            Console.WriteLine(rootToken);

            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine("----------");
            var expr = rootToken?.Aggregate(new TokenPrinter(TokenPrinter.FixType.Postfix)) ?? "<no token>";
            Console.WriteLine(expr);

            Console.WriteLine();

            Console.WriteLine("Scanner");
            Console.WriteLine("-------");
            foreach (var tkn in Parser.ScanExpression(input))
                Console.WriteLine($"val: {tkn.val,-15}kind: {tkn.TokenKind.ToString().TrimStart('_')}");

            Console.WriteLine();
        }
    }
}
