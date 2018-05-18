using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace HisRoyalRedness.com
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "i":
                        Interative();
                        break;

                    case "d":
                        Debug("1u8 + 1234");
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

            var scannedTokens = new List<Token>();
            var parsedTokens = new List<IToken>();
            var rootToken = DoWithCatch<IToken>(() => Parser.ParseExpression(input, scannedTokens, parsedTokens), "PARSE");
            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine("----------");
            var expr = rootToken?.Print(TokenPrinter.FixType.Postfix);
            Console.WriteLine($"Postfix: {expr}");
            Console.WriteLine($"Infix:   {rootToken?.Print(TokenPrinter.FixType.Infix)}");
            Console.WriteLine($"Prefix:  {rootToken?.Print(TokenPrinter.FixType.Prefix)}");

            Console.WriteLine();

            Console.WriteLine("Evaluation");
            Console.WriteLine("----------");
            var evalToken = DoWithCatch<IToken>(() => rootToken?.Evaluate(), "EVALUATE");
            if (evalToken != null)
                Console.WriteLine(PrintToken(evalToken, true));

            Console.WriteLine();

            Console.WriteLine("Scanned token");
            Console.WriteLine("-------------");
            foreach (var tkn in scannedTokens)
                Console.WriteLine($"val: {tkn.val,-15}kind: {tkn.TokenKind.ToString().TrimStart('_')}");

            Console.WriteLine();

            Console.WriteLine("Parsed tokens");
            Console.WriteLine("-------------");
            foreach (var tkn in parsedTokens)
                Console.WriteLine(PrintToken(tkn, true));

            Console.WriteLine();

        }

        static T DoWithCatch<T>(Func<T> func, string errorType)
        {
            try
            {
                return func();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{errorType.ToUpper()} ERROR: {ex.Message}");
                return default(T);
            }
        }

        static string PrintToken(IToken token, bool includeType = true)
        {
            var literalToken = token as ILiteralToken;
            if (literalToken != null)
            {
                string val = "";
                switch(literalToken.DataType)
                {
                    case TokenDataType.Float:
                        {
                            var typedToken = literalToken as FloatToken;
                            val = typedToken.TypedValue.ToString("0.000");
                        }
                        break;
                    case TokenDataType.Timespan:
                        {
                            var typedToken = literalToken as TimespanToken;
                            val = typedToken.ToLongString();
                        }
                        break;
                    case TokenDataType.Time:
                        {
                            var typedToken = literalToken as TimeToken;
                            val = typedToken.TypedValue.ToString();
                        }
                        break;
                    case TokenDataType.Date:
                        {
                            var typedToken = literalToken as DateToken;
                            val = typedToken.TypedValue.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        break;
                    case TokenDataType.LimitedInteger:
                        {
                            var typedToken = literalToken as LimitedIntegerToken;
                            val = typedToken.TypedValue.ToString();
                            if (includeType)
                                val += $"{(typedToken.IsSigned ? "I" : "U")}{(int)(typedToken.BitWidth)}";
                        }
                        break;
                    case TokenDataType.UnlimitedInteger:
                        {
                            var typedToken = literalToken as UnlimitedIntegerToken;
                            val = typedToken.TypedValue.ToString();
                        }
                        break;
                    default:
                        return $"Unrecognised data type {literalToken.DataType}";
                }

                return includeType
                    ? $"{literalToken.DataType,-40}   {val}"
                    : val;
            }

            var opToken = token as OperatorToken;
            if (opToken != null)
            {
                return includeType
                    ? $"{opToken.Operator,-40}   {opToken.Operator.GetEnumDescription()}"
                    : opToken.Operator.GetEnumDescription();
            }

            var grpToken = token as GroupingToken;
            if (grpToken != null)
            {
                return includeType
                    ? $"{grpToken.GroupingOperator,-40}   {grpToken.GroupingOperator.GetEnumDescription()}"
                    : grpToken.GroupingOperator.GetEnumDescription();
            }

            return null;
        }


    }

    
}
