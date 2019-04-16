using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Reflection;

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
                        Interative(Configuration);
                        break;

                    case "d":
                        if (args.Length > 1)
                            Debug(args[1], Configuration);
                        else
                            Debug("'2018-01-01 01:02:03'", Configuration);
                        break;

                    case "cast":
                        DataMapper.PrintAllTypeCasts();
                        break;

                    case "minmax":
                        PrintMinMax();
                        break;

                    case "opstype":
                        OperationsByType();
                        break;

                    case "opsops":
                        DataMapper.PrintOperandTypes(DataMapper.AllSupportedOperationTypes.Value);
                        break;

                    case "opsopsin":
                        DataMapper.PrintOperandTypes(DataMapper.AllInputOperandTypes.Value);
                        break;
                }
            }
            else
                ShowUsage();
        }

        static void Interative(IConfiguration config)
        {
            config = config ?? new Configuration();
            Console.WriteLine("Enter an expression, or an empty line to quit");
            string input = null;
            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
                Console.WriteLine($" = {_evaluator.Value.Evaluate(Parser.ParseExpression(input, config), config)}");
        }

        static void Debug(string input, IConfiguration config)
        {
            config = config ?? new Configuration();
            Console.WriteLine($"                1         2         3         4         5         6         7         8         9         ");
            Console.WriteLine($"       123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
            Console.WriteLine($"Input: {input}");

            var scannedTokens = new List<Token>();
            var rootToken = DoWithCatch<IToken>(() => Parser.ParseExpression(input, scannedTokens, config), "PARSE");
            Console.WriteLine();

            Console.WriteLine("Expression");
            Console.WriteLine("----------");
            Console.WriteLine($"Postfix: {rootToken?.Print(TokenPrinter.FixType.Postfix)}");
            Console.WriteLine($"Infix:   {rootToken?.Print(TokenPrinter.FixType.Infix)}");
            Console.WriteLine($"Prefix:  {rootToken?.Print(TokenPrinter.FixType.Prefix)}");

            Console.WriteLine();

            Console.WriteLine("Evaluation");
            Console.WriteLine("----------");
            var result = DoWithCatch<IDataType>(() => _evaluator.Value.Evaluate(rootToken, config), "EVALUATE");
            Console.WriteLine(result?.ToString(Verbosity.ValueAndType) ?? "<null>");
            //Console.WriteLine(Engine.State); // Output state of calc

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
                return default;
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

        static void ShowUsage()
        {
            var exeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            Console.WriteLine($"Usage: {exeName}  [ i | d [ <expr> ] | cast | minmax | opstype | opsops | opsopsin ]");
        }

        static void PrintMinMax()
        {
            const int colWidth = -42;

            foreach (var isSigned in new[] { false, true })
            {
                Console.WriteLine();
                Console.WriteLine(isSigned ? "Signed" : "Unsigned");
                Console.WriteLine($"{"Bits",-6}{"Min",colWidth}{"Max",colWidth}{"Mask",colWidth}");
                foreach (var bitWidth in EnumExtensions.GetEnumCollection<IntegerBitWidth>())
                {
                    var signAndBitwidth = new BitWidthAndSignPair(bitWidth, isSigned);
                    var minMax = MinAndMaxMap.Instance[signAndBitwidth];
                    var min = minMax.Min.ToString();
                    var max = minMax.Max.ToString();
                    var mask = minMax.Mask.ToString();
                    Console.WriteLine($"{(int)bitWidth,-6}{min,colWidth}{max,colWidth}{max,colWidth}");
                }
            }
        }

        static void OperationsByType()
        {
            var dict = new Dictionary<DataType, Dictionary<DataType, List<OperatorType>>>();
            foreach(var entry in EnumExtensions.GetEnumCollection<OperatorType>()
                .SelectMany(opType => DataMapper.OperandTypeCastMap[opType].Select(kv =>
                    new Tuple<OperatorType, DataType, DataType>(opType, kv.Value.Left, kv.Value.Right))))
            {
                if (!dict.ContainsKey(entry.Item2))
                    dict.Add(entry.Item2, new Dictionary<DataType, List<OperatorType>>());
                var subDict = dict[entry.Item2];
                if (!subDict.ContainsKey(entry.Item3))
                    subDict.Add(entry.Item3, new List<OperatorType>());
                if (!subDict[entry.Item3].Contains(entry.Item1))
                    subDict[entry.Item3].Add(entry.Item1);
            }
            
            foreach(var leftType in dict.Keys)
            {
                var firstCol = $"{leftType, -20}";
                foreach(var rightType in dict[leftType].Keys)
                {
                    var scndCol = $"{rightType, -20}";
                    var csv = string.Join(", ", dict[leftType][rightType].Select(op => op.GetEnumDescription()));

                    Console.WriteLine($"{firstCol}{scndCol}{csv}");
                    firstCol = $"{"",-20}";
                    scndCol = $"{"",-20}";
                }
                Console.WriteLine();
            }
        }

        internal class SortByTypeComparer : IComparer<Tuple<OperatorType, DataType, DataType>>
        {
            public int Compare(Tuple<OperatorType, DataType, DataType> x, Tuple<OperatorType, DataType, DataType> y)
            {
                var leftCmp = x.Item2.CompareTo(y.Item2);
                if (leftCmp != 0)
                    return leftCmp;
                var rightCmp = x.Item3.CompareTo(y.Item3);
                if (rightCmp != 0)
                    return rightCmp;
                return x.Item1.CompareTo(y.Item1);
            }

            public static SortByTypeComparer Instance => _instance.Value;

            static readonly Lazy<SortByTypeComparer> _instance = new Lazy<SortByTypeComparer>(() => new SortByTypeComparer());
        }

        static CalcEngine Engine { get; } = CalcEngine.Instance;
        static IConfiguration Configuration { get; } = new Configuration()
        {
            AllowMultidayTimes = true
        };

        static Lazy<IEvaluator> _evaluator = new Lazy<IEvaluator>(() => new Evaluator<DataType>(Engine));
    }
}
