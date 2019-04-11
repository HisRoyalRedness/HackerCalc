using System.Collections.Generic;
using System.Linq;

/*
    Base type for all functions

    Keith Fletcher
    Oct 2018

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    public interface IFunctionToken : IToken
    {
        string Name { get; }
        List<IToken> Parameters { get; }
    }

    public class FunctionToken : TokenBase<FunctionToken>, IFunctionToken
    {
        #region Constructors
        private FunctionToken(string name, string rawToken, SourcePosition position)
            : base(rawToken, position)
        {
            Name = name;
        }
        #endregion Constructors

        #region Parsing
        public static FunctionToken Parse(string name, List<IToken> parameters, SourcePosition position)
        {
            var rawToken = $"{name}({(string.Join(", ", parameters.Select(p => p.RawToken)))})";
            return new FunctionToken(name, rawToken, position).Tap(ft => ft.Parameters.AddRange(parameters));
        }
        #endregion Parsing

        public string Name { get; private set; }
        public List<IToken> Parameters { get; } = new List<IToken>();
        public override TokenCategory Category => TokenCategory.FunctionToken;

        #region ToString
        public override string ToString() => this.Accept(new TokenPrinter(TokenPrinter.FixType.Infix));
        #endregion ToString
    }
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
