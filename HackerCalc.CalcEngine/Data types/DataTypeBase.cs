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

        [DontEnumerate]
        Unknown
    }

    public interface IDataTypeBase<TBaseType, TConcreteDataType> : IDataType<DataType>
        where TConcreteDataType : class, IDataType<DataType>
    { }

    #region InternalDataTypeBase
    public abstract class InternalDataTypeBase : IDataType<DataType>, IDataTypeTesting
    {
        public DataType DataType { get; protected set; }
        public abstract object ObjectValue { get; }

        #region IDataTypeTesting
        string IDataTypeTesting.TypeName => InternalTypeName;
        string IDataTypeTesting.TypeValue => InternalTypeValue;
        #endregion IDataTypeTesting

        public abstract IDataType<DataType> CastTo(DataType dataType);
        public abstract string ToString(Verbosity verbosity);

        public abstract TNewType CastTo<TNewType>()
            where TNewType : class, IDataType<DataType>;

        protected abstract int InternalGetHashCode();
        protected abstract bool InternalEquals(IDataType other);
        protected abstract int InternalCompareTo(IDataType other);
        protected abstract string InternalTypeName { get; }
        protected virtual string InternalTypeValue => ToString(Verbosity.ValueAndBitwidth);

        #region Equality
        public static bool operator ==(InternalDataTypeBase a, InternalDataTypeBase b) => !(a is null) && a.InternalEquals(b);
        public static bool operator !=(InternalDataTypeBase a, InternalDataTypeBase b) => !(a == b);
        public static bool operator ==(InternalDataTypeBase a, IDataType b) => !(a is null) && a.InternalEquals(b);
        public static bool operator !=(InternalDataTypeBase a, IDataType b) => !(a == b);
        public override bool Equals(object obj) => InternalEquals(obj as IDataType);
        public bool Equals(IDataType other) => InternalEquals(other);
        public bool Equals(IDataType<DataType> other) => InternalEquals(other);
        public override int GetHashCode() => InternalGetHashCode();
        #endregion Equality

        #region ICompare
        public int CompareTo(object obj) => InternalCompareTo(obj as IDataType);
        public int CompareTo(IDataType other) => InternalCompareTo(other);
        public int CompareTo(IDataType<DataType> other) => InternalCompareTo(other);
        #endregion ICompare

        #region Operator overloads
        public static InternalDataTypeBase operator +(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Add, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator -(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Subtract, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator *(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Multiply, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator /(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Divide, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator %(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Modulo, a, b) as InternalDataTypeBase;
        //public static InternalDataTypeBase operator <<(InternalDataTypeBase a, int b)
        //    => CalcEngine.Instance.Calculate(OperatorType.LeftShift, a, (IDataType<DataType>)(new UnlimitedIntegerToken(b))) as InternalDataTypeBase;
        //public static InternalDataTypeBase operator >>(InternalDataTypeBase a, int b)
        //    => CalcEngine.Instance.Calculate(OperatorType.Modulo, a, (IDataType<DataType>)(new UnlimitedIntegerToken(b))) as InternalDataTypeBase;
        public static InternalDataTypeBase operator &(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Modulo, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator |(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Modulo, a, b) as InternalDataTypeBase;
        public static InternalDataTypeBase operator ^(InternalDataTypeBase a, InternalDataTypeBase b)
            => CalcEngine.Instance.Calculate(OperatorType.Modulo, a, b) as InternalDataTypeBase;
        #endregion Operator overloads
    }
    #endregion InternalDataTypeBase

    public abstract class DataTypeBase<TBaseType, TConcreteDataType> : InternalDataTypeBase, IDataTypeBase<TBaseType, TConcreteDataType>,
        IEquatable<DataTypeBase<TBaseType, TConcreteDataType>>
        where TConcreteDataType : class, IDataType<DataType>
        where TBaseType : IComparable<TBaseType>
    {
        protected DataTypeBase(TBaseType value, DataType dataType)
        {
            Value = value;
            DataType = dataType;
        }

        public TBaseType Value { get; private set; }
        public override object ObjectValue => Value;

        #region Type casting
        public override TNewType CastTo<TNewType>()
        {
            if (typeof(TNewType) == typeof(TConcreteDataType))
                return this as TNewType;
            return InternalCastTo<TNewType>()
            ?? throw new TypeConversionException($"Can't convert an object from type {typeof(TConcreteDataType).Name} to {typeof(TNewType).Name}.");
        }

        public override IDataType<DataType> CastTo(DataType dataType)
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

        #region Equality
        public bool Equals(DataTypeBase<TBaseType, TConcreteDataType> other) => InternalEquals(other as IDataType);
        public override bool Equals(object obj) => Equals(obj as IDataType);
        public static bool operator ==(DataTypeBase<TBaseType, TConcreteDataType> a, IDataType b) => !(a is null) && a.InternalEquals(b);
        public static bool operator !=(DataTypeBase<TBaseType, TConcreteDataType> a, IDataType b) => !(a == b);
        public override int GetHashCode() => InternalGetHashCode();
        #endregion Equality

        public override string ToString() => ToString(Verbosity.ValueOnly);
        public override string ToString(Verbosity verbosity)
        {
            switch (verbosity)
            {
                case Verbosity.ValueOnly: return $"{Value}";
                case Verbosity.ValueAndBitwidth: return $"{Value}";
                case Verbosity.ValueAndType: return $"{Value}: {(GetType().Name)}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(verbosity));
            }
        }
    }
}
