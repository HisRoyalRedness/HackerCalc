using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class Evaluator<TDataEnum> : IEvaluator<TDataEnum>
        where TDataEnum : Enum
    {
        public Evaluator(ICalcEngine<TDataEnum> calcEngine, ICalcSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings)); ;
            CalcEngine = calcEngine ?? throw new ArgumentNullException(nameof(calcEngine));
        }

        #region IEvaluator Evaluate implementation
        IDataType IEvaluator.Evaluate(IToken token)
        { 
            CalcEngine.State.Reset();
            return token.Accept<IDataType<TDataEnum>>(this);
        }

        IDataType ITokenVisitor<IDataType>.Visit<TToken>(TToken token)
            => VisitInternal(token);

        IDataType<TDataEnum> ITokenVisitor<IDataType<TDataEnum>>.Visit<TToken>(TToken token)
            => VisitInternal(token);
        #endregion IEvaluator Evaluate implementation

        IDataType<TDataEnum> VisitInternal(IToken token)
        {
            switch (token?.Category)
            {
                case null:
                    return null;

                case TokenCategory.LiteralToken:
                    return CalcEngine.ConvertToTypedDataType(token as ILiteralToken);

                case TokenCategory.OperatorToken:
                    var opToken = token as IOperatorToken;
                    var left = opToken.Left?.Accept((ITokenVisitor<IDataType<TDataEnum>>)this);
                    if (opToken.IsUnary)
                        return CalcEngine.Calculate(opToken.Operator, left);
                    else
                    {
                        var right = opToken.Right?.Accept((ITokenVisitor<IDataType<TDataEnum>>)this);
                        return CalcEngine.Calculate(opToken.Operator, left, right);
                    }


                default:
                    throw new UnrecognisedTokenException($"Unrecognised token category {token.Category}");
            }
        }

        public ICalcEngine<TDataEnum> CalcEngine { get; private set; }
        public ICalcSettings Settings { get; private set; }
        public ICalcState State { get; private set; }

    }
}
