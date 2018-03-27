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
            if (args.Length > 0)
            {
                switch(args[0].ToLower())
                {
                    case "i":
                        Interative();
                        break;

                    case "d":
                        Debug("(i32)1+2*3-4/5");
                        break;
                }
            }
        }

        static void Interative()
        {
            Console.WriteLine("Enter an expression, or an empty line to quit");
            string input = null;
            while(!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
                Console.WriteLine($" = {PrintToken(Parser.ParseExpression(input)?.Evaluate(), true)}");
        }

        static void Debug(string input)
        {
            Console.WriteLine($"                1         2         3         4         5         6         7         8         9         ");
            Console.WriteLine($"       123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
            Console.WriteLine($"Input: {input}");

            var rootToken = Parser.ParseExpression(input);

            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine("----------");
            var expr = rootToken?.Print(TokenPrinter.FixType.Postfix);
            Console.WriteLine(expr);

            Console.WriteLine();

            Console.WriteLine("Evaluation");
            Console.WriteLine("----------");
            var eval = PrintToken(rootToken?.Evaluate(), true);
            Console.WriteLine(eval);

            Console.WriteLine();

            Console.WriteLine("Scanner");
            Console.WriteLine("-------");
            foreach (var tkn in Parser.ScanExpression(input))
                Console.WriteLine($"val: {tkn.val,-15}kind: {tkn.TokenKind.ToString().TrimStart('_')}");

            Console.WriteLine();
        }

        static string PrintToken(IToken token, bool includeType = true)
        {
            var literalToken = token as ILiteralToken;
            if (literalToken != null)
            {
                string val = "";
                string dataType = "";

                switch(literalToken.DataType)
                {
                    case TokenDataType.Integer:
                        {
                            var typedToken = literalToken as IntegerToken;
                            val = typedToken.TypedValue.ToString();
                            dataType = includeType
                                ? (typedToken.BitWidth == IntegerToken.IntegerBitWidth.Unbound
                                    ? "Unbound integer"
                                    : $"{(typedToken.IsSigned ? "I" : "U")}{(int)(typedToken.BitWidth)}")
                                : null;
                        }
                        break;
                    case TokenDataType.Float:
                        {
                            var typedToken = literalToken as FloatToken;
                            val = typedToken.TypedValue.ToString("0.000");
                            dataType = includeType ? "Float" : null;
                        }
                        break;
                    case TokenDataType.Timespan:
                        {
                            var typedToken = literalToken as TimespanToken;
                            val = typedToken.ToLongString();
                            dataType = includeType ? "Timespan" : null;
                        }
                        break;
                    case TokenDataType.Time:
                        {
                            var typedToken = literalToken as TimeToken;
                            val = typedToken.TypedValue.ToString();
                            dataType = includeType ? "Time" : null;
                        }
                        break;
                    case TokenDataType.Date:
                        {
                            var typedToken = literalToken as DateToken;
                            val = typedToken.TypedValue.ToString("yyyy-MM-dd HH:mm:ss");
                            dataType = includeType ? "Date" : null;
                        }
                        break;
                    default:
                        return $"Unrecognised data type {literalToken.DataType}";
                }

                return string.IsNullOrWhiteSpace(dataType)
                    ? val
                    : $"{val}   -   {dataType}";
            }

            var opToken = token as OperatorToken;
            if (opToken != null)
            {
                return opToken.Operator.ToString();
            }

            return null;
        }


    }

    
}
