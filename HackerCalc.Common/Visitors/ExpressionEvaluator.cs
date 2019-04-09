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
        public Evaluator(ICalcEngine<TDataEnum> calcEngine)
        {
            CalcEngine = calcEngine;
        }

        #region IEvaluator Evaluate implementation
        public IDataType Evaluate(IToken token, IConfiguration configuration)
        {
            configuration?.State?.Reset();
            var evaluation = new Evaluation(CalcEngine, configuration);
            return token?.Accept<IDataType<TDataEnum>>(evaluation);
        }

        #region Evaluation
        class Evaluation : ITokenVisitor<IDataType<TDataEnum>>, ITokenVisitor<IDataType>
        {
            public Evaluation(ICalcEngine<TDataEnum> calcEngine, IConfiguration configuration)
            {
                CalcEngine = calcEngine;
                Configuration = configuration;
            }

            IDataType<TDataEnum> ITokenVisitor<IDataType<TDataEnum>>.Visit<TToken>(TToken token)
                => VisitInternal(token);

            IDataType ITokenVisitor<IDataType>.Visit<TToken>(TToken token)
                => VisitInternal(token);

            IDataType<TDataEnum> VisitInternal(IToken token)
            {
                switch (token?.Category)
                {
                    case null:
                        return null;

                    case TokenCategory.LiteralToken:
                        return CalcEngine.ConvertToTypedDataType(token as ILiteralToken, Configuration);

                    case TokenCategory.OperatorToken:
                        var opToken = token as IOperatorToken;
                        var left = opToken.Left?.Accept((ITokenVisitor<IDataType<TDataEnum>>)this);
                        if (opToken.IsUnary)
                            return CalcEngine.Calculate(Configuration, opToken.Operator, left);
                        else
                        {
                            var right = opToken.Right?.Accept((ITokenVisitor<IDataType<TDataEnum>>)this);
                            return CalcEngine.Calculate(Configuration, opToken.Operator, left, right);
                        }


                    default:
                        throw new UnrecognisedTokenException($"Unrecognised token category {token.Category}");
                }
            }

            public ICalcEngine<TDataEnum> CalcEngine { get; }
            public IConfiguration Configuration { get; }
        }
        #endregion Evaluation
        #endregion IEvaluator Evaluate implementation

        public ICalcEngine<TDataEnum> CalcEngine { get; }
    }
}
