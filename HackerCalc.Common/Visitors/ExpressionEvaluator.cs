using System;

/*
    Evaluator   

        Performs the required operations on a data type tree,
        and returns the resulting data type of the complete
        operation

    Keith Fletcher
    Apr 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

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
