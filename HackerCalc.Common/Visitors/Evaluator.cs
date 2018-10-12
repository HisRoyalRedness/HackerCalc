using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class Evaluator : ITokenVisitor<IDataType>
    {
        private Evaluator(ICalcEngine calcEngine, IDataMapper dataMapper)
        {
            CalcEngine = calcEngine ?? throw new ArgumentNullException(nameof(calcEngine));
            DataMapper = dataMapper ?? throw new ArgumentNullException(nameof(dataMapper));
        }

        static Evaluator()
        {
            var dataMapper = new DataMapper();
            var calcEngine = new CalcEngine(dataMapper);
            Default = new Evaluator(calcEngine, dataMapper);
        }

        public static IDataType Evaluate(IToken token)
            => token?.Accept(Evaluator.Default);

        public static IDataType Evaluate(IToken token, ICalcEngine calcEngine, IDataMapper dataMapper)
            => token?.Accept(new Evaluator(calcEngine, dataMapper));

        IDataType ITokenVisitor<IDataType>.Visit<TToken>(TToken token)
        {
            switch(token?.Category)
            {
                case null:
                    return null;

                case TokenCategory.LiteralToken:
                    return DataMapper.Map(token as ILiteralToken);

                case TokenCategory.OperatorToken:
                    var opToken = token as IOperatorToken;
                    if (opToken.IsUnary)
                    {
                        return null;
                    }
                    else
                    {
                        var left = opToken.Left?.Accept(this);
                        var right = opToken.Right?.Accept(this);

                        var currentDataTypePair = new DataTypePair(left.DataType, right.DataType);
                        var targetDataTypePair = DataMapper.GetOperandDataTypes(opToken.Operator, currentDataTypePair);
                        if (currentDataTypePair != targetDataTypePair)
                        {
                            // DO cast
                        }
                        return CalcEngine.Calculate(
                            left,
                            right,
                            opToken.Operator);
                    }



                default:
                    throw new UnrecognisedTokenException($"Unrecognised token category {token.Category}");
            }
        }

        public static Evaluator Default { get; private set; }

        public ICalcEngine CalcEngine { get; private set; }
        public IDataMapper DataMapper { get; private set; }
    }

    public static class TokenExtensions
    {
        public static IDataType Evaluate(this IToken token)
            => Evaluator.Evaluate(token);
    }
}
