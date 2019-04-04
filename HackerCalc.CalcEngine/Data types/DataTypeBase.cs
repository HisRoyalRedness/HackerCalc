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

        #region Operate
        // Called by CalcEngine.Calculate()
        internal static IDataType<DataType> Operate(OperatorType opType, params IDataType<DataType>[] operands)
        {
            if (opType.IsUnaryOperator())
            {
                if (operands.Length != 1)
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is a unary operator, but {operands.Length + 1} operands were provided.");
                return ((InternalDataTypeBase)operands[0]).OperateInternal(opType, operands);
            }
            if (opType.IsBinaryOperator())
            {
                if (operands.Length != 2)
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is a binary operator, but {operands.Length + 1} operands were provided.");
                return ((InternalDataTypeBase)operands[0]).OperateInternal(opType, operands);
            }
            throw new InvalidCalcOperationException("Only unary and binary operations are supported");
        }

        // Called by uniot tests. Minda mirrors what CalcEngine does
        static InternalDataTypeBase UnitTestOperate(OperatorType opType, params IDataType<DataType>[] operands)
        {
            if (operands.Length == 0)
                throw new InvalidCalcOperationException($"No operands were provided.");

            // Unary
            if (operands.Length == 1)
            {
                if (!opType.IsUnaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a unary operator, and a single operand was provided.");
                return (InternalDataTypeBase)((InternalDataTypeBase)operands[0]).OperateInternal(opType, operands);
            }
            // Binary
            else if (operands.Length == 2)
            {
                if (!opType.IsBinaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a binary operator, and two operands were provided.");

                string errorMsg;
                switch (opType)
                {
                    case OperatorType.Add: errorMsg = Properties.Resources.CALCENGINE_AddErrorMessage; break;
                    case OperatorType.Subtract: errorMsg = Properties.Resources.CALCENGINE_SubtractErrorMessage; break;
                    case OperatorType.Multiply:
                    case OperatorType.Divide:
                    case OperatorType.Power:
                    case OperatorType.Root:
                    case OperatorType.Modulo:
                    case OperatorType.LeftShift:
                    case OperatorType.RightShift:
                    case OperatorType.And:
                    case OperatorType.Or:
                    case OperatorType.Xor:
                        throw new NotImplementedException();
                    default:
                        throw new InvalidCalcOperationException($"Unsupported binary operator '{opType.GetEnumDescription()}'.");
                }

                // Make sure the operation is supported for the provided types
                var currentValuePair = CalcEngine.GetDataTypeValuePair(operands[0], operands[1]);
                var targetDataTypePair = DataMapper.GetOperandDataTypes(opType, currentValuePair);
                if (!targetDataTypePair.OperationSupported)
                    throw new InvalidCalcOperationException(
                        string.Format(errorMsg,
                            currentValuePair.Left.DataType,
                            currentValuePair.Left.ObjectValue,
                            currentValuePair.Right.DataType,
                            currentValuePair.Right.ObjectValue));

                // If it is supported, possibly cast the operands into something more useful
                var currentDataTypePair = CalcEngine.GetDataTypePair(operands[0], operands[1]);
                if (currentDataTypePair != targetDataTypePair)
                    operands = new IDataType<DataType>[] { operands[0].CastTo(targetDataTypePair.Left), operands[1].CastTo(targetDataTypePair.Right) };

                // Attempt to perform the operation
                return (InternalDataTypeBase)((InternalDataTypeBase)operands[0]).OperateInternal(opType, operands)
                    ?? throw new InvalidCalcOperationException(
                        string.Format(errorMsg,
                            currentValuePair.Left.DataType,
                            currentValuePair.Left.ObjectValue,
                            currentValuePair.Right.DataType,
                            currentValuePair.Right.ObjectValue));
            }
            else
                throw new InvalidCalcOperationException("Only unary and binary operations are supported");
        }

        protected abstract IDataType<DataType> OperateInternal(OperatorType opType, IDataType<DataType>[] operands);
        #endregion Operate
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

        /// <summary>
        /// Validation for the concrete data type from the static Operate method, just in case
        /// they've added some operate overrides that aren't implemented correctly
        /// </summary>
        protected static void OperateValidate(OperatorType opType, DataType dataType, params IDataType<DataType>[] operands)
        {
            if ((operands?.Length ?? 0) == 0)
                throw new InvalidCalcOperationException("No operands were provided.");
            if (operands[0].DataType != dataType)
                throw new InvalidCalcOperationException(
                    $"Invalid first operand type for {typeof(TBaseType).Name}, attempting a {opType} operation on ({string.Join(", ", operands.Select(o => o.DataType.ToString()))})");
        }

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
