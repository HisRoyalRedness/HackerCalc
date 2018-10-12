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
                        if (args.Length > 1)
                            Debug(args[1]);
                        else
                            Debug("1+2");
                        break;
                }
            }
        }

        static void Interative()
        {
            Console.WriteLine("Enter an expression, or an empty line to quit");
            string input = null;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
                Console.WriteLine($" = {(Parser.ParseExpression(input)?.Evaluate())}");
        }

        static void Debug(string input)
        {
            Console.WriteLine($"                1         2         3         4         5         6         7         8         9         ");
            Console.WriteLine($"       123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
            Console.WriteLine($"Input: {input}");

            var scannedTokens = new List<Token>();
            var rootToken = DoWithCatch<IToken>(() => Parser.ParseExpression(input, scannedTokens), "PARSE");
            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine("----------");
            Console.WriteLine($"Postfix: {rootToken?.Print(TokenPrinter.FixType.Postfix)}");
            Console.WriteLine($"Infix:   {rootToken?.Print(TokenPrinter.FixType.Infix)}");
            Console.WriteLine($"Prefix:  {rootToken?.Print(TokenPrinter.FixType.Prefix)}");

            Console.WriteLine();

            Console.WriteLine("Evaluation");
            Console.WriteLine("----------");
            var result = DoWithCatch<IDataType>(() => rootToken?.Evaluate(), "EVALUATE");
            Console.WriteLine(result?.ToString(Verbosity.ValueAndType) ?? "<null>");

            Console.WriteLine();

            Console.WriteLine("Scanned token");
            Console.WriteLine("-------------");
            foreach (var tkn in scannedTokens)
                Console.WriteLine($"val: {tkn.val,-15}kind: {tkn.TokenKind.ToString().TrimStart('_')}");

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
            if (token is ILiteralToken literalToken)
            {
                string val = "";
                switch (literalToken.LiteralType)
                {
                    case LiteralTokenType.Float:
                        {
                            var typedToken = literalToken as FloatToken;
                            val = typedToken.TypedValue.ToString("0.000");
                        }
                        break;
                    case LiteralTokenType.Timespan:
                        {
                            var typedToken = literalToken as TimespanToken;
                            val = typedToken.ToLongString();
                        }
                        break;
                    case LiteralTokenType.Time:
                        {
                            var typedToken = literalToken as TimeToken;
                            val = typedToken.TypedValue.ToString();
                        }
                        break;
                    case LiteralTokenType.Date:
                        {
                            var typedToken = literalToken as DateToken;
                            val = typedToken.TypedValue.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        break;
                    case LiteralTokenType.LimitedInteger:
                        {
                            var typedToken = literalToken as LimitedIntegerToken;
                            val = typedToken.TypedValue.ToString();
                            if (includeType)
                                val += $"{(typedToken.IsSigned ? "I" : "U")}{(int)(typedToken.BitWidth)}";
                        }
                        break;
                    case LiteralTokenType.UnlimitedInteger:
                        {
                            var typedToken = literalToken as UnlimitedIntegerToken;
                            val = typedToken.TypedValue.ToString();
                        }
                        break;
                    default:
                        return $"Unrecognised literal token type {literalToken.LiteralType}";
                }

                return includeType
                    ? $"{literalToken.LiteralType,-40}   {val}"
                    : val;
            }

            if (token is OperatorToken opToken)
            {
                return includeType
                    ? $"{opToken.Operator,-40}   {opToken.Operator.GetEnumDescription()}"
                    : opToken.Operator.GetEnumDescription();
            }

            //if (token is OldGroupingToken grpToken)
            //{
            //    return includeType
            //        ? $"{grpToken.GroupingOperator,-40}   {grpToken.GroupingOperator.GetEnumDescription()}"
            //        : grpToken.GroupingOperator.GetEnumDescription();
            //}

            return null;
        }


    }

    
}
