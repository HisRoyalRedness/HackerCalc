using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public enum Verbosity
    {
        ValueOnly,
        ValueAndBitwidth,
        ValueAndType
    }

    public interface IDataType : IComparable, IComparable<IDataType>, IEquatable<IDataType>
    {
        object ObjectValue { get; }
        string ToString(Verbosity verbosity);
    }

    public interface IDataType<TDataTypeEnum> : IDataType, IComparable<IDataType<TDataTypeEnum>>, IEquatable<IDataType<TDataTypeEnum>>
    {
        TDataTypeEnum DataType { get; }
        TNewType CastTo<TNewType>()
            where TNewType : class, IDataType<TDataTypeEnum>;
        IDataType<TDataTypeEnum> CastTo(TDataTypeEnum dataType);
    }

    public interface IDataTypeTesting
    {
        string TypeName { get; }
        string TypeValue { get; }
    }
}
