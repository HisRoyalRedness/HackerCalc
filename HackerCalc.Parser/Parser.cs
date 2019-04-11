using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
    Static portion of the Parser

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public partial class Parser
    {
        public Parser(Scanner scanner, IConfiguration configuration)
            : this(scanner)
        {
            Configuration = configuration;
        }

        public static IToken ParseExpression(string expression, IConfiguration configuration = null)
            => ParseExpression(expression, null, configuration);

        public static IToken ParseExpression(string expression, List<Token> scannedTokens, IConfiguration configuration)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms, configuration);
                if (scannedTokens != null)
                    scanner.ScannedTokens = tkn => scannedTokens.Add(tkn);
                var parser = new Parser(scanner, configuration);
                return parser.Parse()
                    ? parser.RootToken
                    : null;
            }
        }


        public static IEnumerable<Token> ScanExpression(string expression, IConfiguration configuration = null)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(expression)))
            {
                var scanner = new Scanner(ms, configuration);
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
        public enum TimePortion
        {
            Any,
            Days,
            Hours,
            Minutes,
            Seconds,
            Milliseconds,
            Microseconds,
            Nanoseconds
        }

        bool IsTimespanPortion(TimePortion portion, bool throwSynError = false)
        {
            switch (la.kind)
            {
                case _typed_ts_seconds:
                    return portion == TimePortion.Seconds || portion == TimePortion.Any;

                case _dec_unlimited_int:
                case _true_float:
                    // true_float could be a time portion or just a numeric value.
                    // Return true if it's a time portion. Assume anything else is a numeric
                    var next = scanner.Peek().kind;
                    scanner.ResetPeek();
                    switch (next)
                    {
                        case _ts_days_type:
                            return portion == TimePortion.Days || portion == TimePortion.Any;
                        case _ts_hours_type:
                            return portion == TimePortion.Hours || portion == TimePortion.Any;
                        case _ts_minutes_type:
                            return portion == TimePortion.Minutes || portion == TimePortion.Any;
                        case _ts_seconds_type:
                            return portion == TimePortion.Seconds || portion == TimePortion.Any;
                        case _ts_millisec_type:
                            return portion == TimePortion.Milliseconds || portion == TimePortion.Any;
                        case _ts_microsec_type:
                            return portion == TimePortion.Microseconds || portion == TimePortion.Any;
                        case _ts_nanosec_type:
                            return portion == TimePortion.Nanoseconds || portion == TimePortion.Any;
                        default:
                            return false;
                    }
                // Anything else is possibly a syntax error,
                // but at the very least not a time portion
                default:
                    if (throwSynError)
                        SynErr(la.kind);
                    return false;
            }
        }

        public bool IsTimespanNanoSeconds() => IsTimespanPortion(TimePortion.Nanoseconds);
        public bool IsTimespanMicroSeconds() => IsTimespanPortion(TimePortion.Microseconds);
        public bool IsTimespanMilliSeconds() => IsTimespanPortion(TimePortion.Milliseconds);
        public bool IsTimespanSeconds() => IsTimespanPortion(TimePortion.Seconds);
        public bool IsTimespanMinutes() => IsTimespanPortion(TimePortion.Minutes);
        public bool IsTimespanHours() => IsTimespanPortion(TimePortion.Hours);
        public bool IsTimespanDays() => IsTimespanPortion(TimePortion.Days);
        public bool IsTimespanNumber() => IsTimespanPortion(TimePortion.Any);

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

        public static SourcePosition GetPos(Token token) => new SourcePosition(token.line, token.col);

        public IToken RootToken { get; private set; }

        public IConfiguration Configuration { get; }
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

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
