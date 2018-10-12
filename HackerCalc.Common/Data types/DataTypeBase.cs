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
        ValueAndType
    }

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

    public interface IDataType
    {
        object ObjectValue { get; }
        DataType DataType { get; }
        string ToString(Verbosity verbosity);
    }

    public interface IDataType<TBaseType> : IDataType
    {
        TBaseType Value { get; }
    }

    public abstract class DataTypeBase<TBaseType, TDataType> : IDataType<TBaseType>
        where TDataType : class, IDataType
    {
        protected DataTypeBase(TBaseType value, DataType dataType)
        {
            Value = value;
            DataType = dataType;
        }

        public TBaseType Value { get; private set; }
        public object ObjectValue => Value;
        public DataType DataType { get; private set; }

        public override string ToString() => ToString(Verbosity.ValueOnly);
        public string ToString(Verbosity verbosity)
        {
            switch(verbosity)
            {
                case Verbosity.ValueOnly: return $"{Value}";
                case Verbosity.ValueAndType: return $"{Value}: {(GetType().Name)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity));
            }
        }
    }
}
