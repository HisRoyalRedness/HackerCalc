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
    }

    public interface IDataType<TBaseType, TDataType> : IDataType<DataType>
        where TDataType : class, IDataType<DataType>
    { }

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

        #region Type casting
        public TNewType CastTo<TNewType>()
            where TNewType : class, IDataType<DataType>
        {
            if (typeof(TNewType) == typeof(TDataType))
                return this as TNewType;
            return InternalCastTo<TNewType>()
            ?? throw new TypeConversionException($"Can't convert an object from type {typeof(TDataType).Name} to {typeof(TNewType).Name}.");
        }

        public IDataType<DataType> CastTo(DataType dataType)
        {
            switch(dataType)
            {
                case DataType.Date: return CastTo<DateType>();
                case DataType.Float: return CastTo<FloatType>();
                case DataType.Time: return CastTo<TimeType>();
                case DataType.Timespan: return CastTo<TimespanType>();
                case DataType.LimitedInteger: return CastTo<LimitedIntegerType>();
                case DataType.UnlimitedInteger: return CastTo<UnlimitedIntegerType>();
                default:
                    throw new TypeConversionException($"Unhandled data type {dataType}.");
            }
        }

        protected abstract TNewType InternalCastTo<TNewType>()
            where TNewType : class, IDataType<DataType>;
        #endregion Type casting

        #region IComparable implementation
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDataType<DataType> other)
        {
            throw new NotImplementedException();
        }
        #endregion IComparable implementation

        public override string ToString() => ToString(Verbosity.ValueOnly);
        public virtual string ToString(Verbosity verbosity)
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
