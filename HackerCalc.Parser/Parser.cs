using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
        public static IToken ParseExpression(string expression)
            => ParseExpression(expression, null);

        public static IToken ParseExpression(string expression, List<Token> scannedTokens)
        {
            IToken rootToken = null;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                if (scannedTokens != null)
                    scanner.ScannedTokens = tkn => scannedTokens.Add(tkn);
                var parser = new Parser(scanner);
                if (parser.Parse())
                    rootToken = parser.RootToken;
            }
            return rootToken;
        }


        public static IEnumerable<Token> ScanExpression(string expression)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms);
                while (true)
                {
                    var token = scanner.Scan();
                    if (token.kind == 0)
                        yield break;
                    yield return token;
                }
            }
        }

        #region Resolvers
        public bool IsTimespanSeconds()
        {
            switch(la.kind)
            {
                // Definately seconds
                case _typed_ts_seconds:
                    return true;

                // Maybe seconds. Need to check further
                case _dec_unlimited_int:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_seconds_type:
                            return true;
                        default:
                            return false;
                    }
                // Anything else is a syntax error
                default:
                    SynErr(la.kind);
                    return false;
            }
        }

        public bool IsTimespanMinutes()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_unlimited_int:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_minutes_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanHours()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_unlimited_int:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_hours_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanDays()
        {
            switch (la.kind)
            {
                // Maybe minutes. Need to check further
                case _dec_unlimited_int:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_days_type:
                            return true;
                        default:
                            return false;
                    }
                default:
                    return false;
            }
        }

        public bool IsTimespanNumber()
        {
            switch(la.kind)
            {
                // Typed timespan
                case _typed_ts_seconds:
                    return true;

                // Stuff that could be either an integer, float or timespan
                case _true_float:
                case _dec_unlimited_int:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_seconds_type:
                        case _ts_minutes_type:
                        case _ts_hours_type:
                        case _ts_days_type:
                            return true;
                        default:
                            return false;
                    }

                // Assume anything else is not a TimeSpan
                default:
                    return false;
            }
        }

        public bool IsDateTime()
        {
            if (la.kind == _date || la.kind == _date_rev)
            {
                var next = scanner.Peek().kind;
                scanner.ResetPeek();
                return next == _time;
            }
            else
                return false;
        }

        public bool IsBracket()
        {
            // Check if it's a negated bracket
            if (la.kind == _subToken)
            {
                var next = scanner.Peek().kind;
                scanner.ResetPeek();
                return next == _openBracket;
            }
            // else just check for an opening bracket
            else
                return la.kind == _openBracket;
        }

        // Checks if partial equations are enabled, and if is, if the next token is EOF
        public bool IsPartialEquation() => Constants.IsPartialEquationAllowed && la.kind == 0;
        #endregion Resolvers

        void AddToken(IToken token)
        { }

        public IToken RootToken { get; private set; }
    }

    #region Errors
    public partial class Errors
    {
        public virtual void SemErr(int line, int col, string s)
        {
            WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            WriteLine(s);
        }

        protected virtual void WriteLine(string format, params object[] args)
        {
            var currentForeColour = Console.ForegroundColor;
            var currentBackColour = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ForegroundColor = currentForeColour;
            Console.BackgroundColor = currentBackColour;
        }
    }
    #endregion Errors
}
