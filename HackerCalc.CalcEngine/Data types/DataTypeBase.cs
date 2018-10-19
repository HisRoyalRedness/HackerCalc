using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public enum DataType
    {
        [Description("Limited Integer")]
        LimitedInteger,
        [Description("Unlimited Integer")]
        UnlimitedInteger,
        [Description("Float")]
        Float,
        [Description("Date")]
        Date,
        [Description("Time")]
        Time,
        [Description("Timespan")]
        Timespan,

        MAX
    }

    public interface IDataType<TBaseType, TDataType> : IDataType<DataType>
        where TDataType : class, IDataType<DataType>
    {
    }

    public abstract class DataTypeBase<TBaseType, TDataType> : IDataType<TBaseType, TDataType>
        where TDataType : class, IDataType<DataType>
    {
        protected DataTypeBase(TBaseType value, DataType dataType)
        {
            Value = value;
            DataType = dataType;
        }

        public TBaseType Value { get; private set; }
        public object ObjectValue => Value;
        public DataType DataType { get; private set; }

        protected abstract TNewType InternalCastTo<TNewType>()
            where TNewType : class, IDataType<DataType>;

        public override string ToString() => ToString(Verbosity.ValueOnly);
        public string ToString(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.ValueOnly: return $"{Value}";
                case Verbosity.ValueAndType: return $"{Value}: {(GetType().Name)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity));
            }
        }
    }
}
