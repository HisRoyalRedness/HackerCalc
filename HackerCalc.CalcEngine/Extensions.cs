using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    #region EvaluationExtensions
    public static class EvaluationExtensions
    {
        public static IDataType Evaluate(this IToken token, IConfiguration configuration = null)
            => _evaluator.Value.Evaluate(token, configuration);

        static readonly Lazy<Evaluator<DataType>> _evaluator = new Lazy<Evaluator<DataType>>(() => new Evaluator<DataType>(CalcEngine.Instance));
    }
    #endregion EvaluationExtensions
}
