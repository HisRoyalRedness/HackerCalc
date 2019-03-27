using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IEvaluator : ITokenVisitor<IDataType>
    {
        IDataType Evaluate(IToken token);
    }

    public interface IEvaluator<TDataTypeEnum> : IEvaluator, ITokenVisitor<IDataType<TDataTypeEnum>>
        where TDataTypeEnum : Enum
    {
    }
}
