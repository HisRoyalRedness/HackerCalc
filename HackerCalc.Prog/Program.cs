using System;
using System.IO;
using System.Text;

namespace HisRoyalRedness.com
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = args.Length == 0 ? "1 + !2 * (3 + ~4 & 2) - 4 % 6 << 4 + 10 >> 6" : args[0];
            Console.WriteLine($"Input: {input}");
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                var scanner = new Scanner(ms);
                var parser = new Parser(scanner);
                var result = parser.Parse();
            }
        }
    }
}
