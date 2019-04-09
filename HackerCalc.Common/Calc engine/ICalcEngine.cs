using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface ICalcEngine
    {
        IDataType ConvertToDataType(ILiteralToken token, IConfiguration configuration);
        IDataType Calculate(IConfiguration configuration, OperatorType opType, params IDataType[] operands);
    }

    public interface ICalcEngine<TDataTypeEnum> : ICalcEngine
        where TDataTypeEnum : Enum
    {
        IDataType<TDataTypeEnum> ConvertToTypedDataType(ILiteralToken token, IConfiguration configuration);
        IDataType<TDataTypeEnum> Calculate(IConfiguration configuration, OperatorType opType, params IDataType<TDataTypeEnum>[] operands);
    }
}
