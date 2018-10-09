using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IFunctionToken : IToken
    {
        string Name { get; }
        List<IToken> Parameters { get; }
    }

    public class FunctionToken : TokenBase<OperatorToken>, IFunctionToken
    {
        public FunctionToken(string name, string rawToken)
            : base(rawToken)
        {
            Name = name;
        }

        public static FunctionToken Parse(string name, List<IToken> parameters)
        {
            var rawToken = $"{name}({(string.Join(", ", parameters.Select(p => p.RawToken)))})";
            return new FunctionToken(name, rawToken).Tap(ft => ft.Parameters.AddRange(parameters));
        }

        public string Name { get; private set; }
        public List<IToken> Parameters { get; } = new List<IToken>();
    }
}
