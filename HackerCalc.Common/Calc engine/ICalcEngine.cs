using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface ICalcEngine
    {
        IDataType ConvertToDataType(ILiteralToken token);
        IDataType Calculate(OperatorType opType, params IDataType[] operands);
    }

    public interface ICalcEngine<TDataTypeEnum> : ICalcEngine
        where TDataTypeEnum : Enum
    {
        IDataType<TDataTypeEnum> ConvertToTypedDataType(ILiteralToken token);
        IDataType<TDataTypeEnum> Calculate(OperatorType opType, params IDataType<TDataTypeEnum>[] operands);
    }

    public interface IEvaluator : ITokenVisitor<IDataType>
    {
        IDataType Evaluate(IToken token);
    }

    public interface IEvaluator<TDataTypeEnum> : IEvaluator, ITokenVisitor<IDataType<TDataTypeEnum>>
        where TDataTypeEnum : Enum
    { }
}
