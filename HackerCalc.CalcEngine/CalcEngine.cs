using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class CalcEngine : ICalcEngine<DataType>
    {
        #region Constructor
        private CalcEngine(CalcSettings settings, CalcState state = null)
        {
            Settings = settings ?? new CalcSettings();
            State = State ?? new CalcState();

            // For now, I don't anticipate having multiple CalcEngine instances at one time.
            // If that changes, I'll need to think of some other way to get the settings
            // passed on to each data type of a given calc instance.
            DataMapper.Settings = Settings;
            DataMapper.State = State;
        }
        #endregion Constructors

        public static CalcEngine Instance => _instance.Value;
        static Lazy<CalcEngine> _instance = new Lazy<CalcEngine>(() => new CalcEngine(null));

        #region ICalcEngine implementation
        IDataType ICalcEngine.ConvertToDataType(ILiteralToken token)
            => DataMapper.Map(token);
        public IDataType<DataType> ConvertToTypedDataType(ILiteralToken token)
            => DataMapper.Map(token);
        IDataType ICalcEngine.Calculate(OperatorType opType, params IDataType[] operands)
            => Calculate(opType, operands.Select(o => (IDataType<DataType>)o).ToArray());
        ICalcSettings ICalcEngine.Settings => Settings;
        ICalcState ICalcEngine.State => State;

        public CalcSettings Settings { get; private set; }
        public CalcState State { get; private set; }
        #endregion ICalcEngine implementation

        public IDataType<DataType> Calculate(OperatorType opType, params IDataType<DataType>[] operands)
        {
            if (operands.Length == 0)
                    throw new InvalidCalcOperationException($"No operands were provided.");

            // Unary
            if (operands.Length == 1)
            {
                if (!opType.IsUnaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a unary operator, and a single operand was provided.");

            }
            // Binary
            else if (operands.Length == 2)
            {
                if(!opType.IsBinaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a binary operator, and two operands were provided.");

                Func<DataTypeValuePair<DataType>, IDataType<DataType>> opFunc = null;
                string errorMsg;
                switch (opType)
                {
                    case OperatorType.Add: opFunc = Add; errorMsg = Properties.Resources.CALCENGINE_AddErrorMessage;  break;
                    case OperatorType.Subtract: opFunc = Subtract; errorMsg = Properties.Resources.CALCENGINE_SubtractErrorMessage; break;
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

                var currentValuePair = GetDataTypeValuePair(operands[0], operands[1]);
                var targetDataTypePair = DataMapper.GetOperandDataTypes(opType, currentValuePair);
                if (!targetDataTypePair.OperationSupported)
                    throw new InvalidCalcOperationException(
                        string.Format(errorMsg, 
                            currentValuePair.Left.DataType, 
                            currentValuePair.Left.ObjectValue, 
                            currentValuePair.Right.DataType, 
                            currentValuePair.Right.ObjectValue));

                var currentDataTypePair = GetDataTypePair(operands[0], operands[1]);
                if (currentDataTypePair != targetDataTypePair)
                    currentValuePair = GetDataTypeValuePair(operands[0].CastTo(targetDataTypePair.Left), operands[1].CastTo(targetDataTypePair.Right));
                var opResult = opFunc(currentValuePair);
                if (opResult == null)
                    throw new InvalidCalcOperationException(
                        string.Format(errorMsg,
                            currentValuePair.Left.DataType,
                            currentValuePair.Left.ObjectValue,
                            currentValuePair.Right.DataType,
                            currentValuePair.Right.ObjectValue));
                return opResult;
            }
            else
                throw new InvalidCalcOperationException("Only unary and binary operations are supported");
            return null;
        }

        static DataTypePair<DataType> GetDataTypePair(IDataType<DataType> left, IDataType<DataType> right)
            => new DataTypePair<DataType>(left.DataType, right.DataType);

        static DataTypeValuePair<DataType> GetDataTypeValuePair(IDataType<DataType> left, IDataType<DataType> right)
            => new DataTypeValuePair<DataType>(left, right);

        #region Add
        static IDataType<DataType> Add(DataTypeValuePair<DataType> pair)
        {
            /*
            Add (+)             LimitedInteger      LimitedInteger
                                UnlimitedInteger    UnlimitedInteger
                                Float               Float
                                Date                Timespan
                                Time                Timespan
                                Timespan            Date, Time, Timespan
            */

            switch (pair.Left.DataType)
            {
                case DataType.LimitedInteger:
                    if (pair.Right.DataType == DataType.LimitedInteger)
                        return ((LimitedIntegerType)pair.Left) + ((LimitedIntegerType)pair.Right);
                    break;
                case DataType.UnlimitedInteger:
                    if (pair.Right.DataType == DataType.UnlimitedInteger)
                        return ((UnlimitedIntegerType)pair.Left) + ((UnlimitedIntegerType)pair.Right);
                    break;
                case DataType.Float:
                    if (pair.Right.DataType == DataType.Float)
                        return ((FloatType)pair.Left) + ((FloatType)pair.Right);
                    break;
                case DataType.Date:
                    if (pair.Right.DataType == DataType.Timespan)
                        return new DateType(((DateType)pair.Left).Value + ((TimespanType)pair.Right).Value);
                    break;
                case DataType.Time:
                    if (pair.Right.DataType == DataType.Timespan)
                        return ((TimeType)pair.Left) + ((TimespanType)pair.Right);
                    break;
                case DataType.Timespan:
                    switch (pair.Right.DataType)
                    {
                        case DataType.Date:
                            return ((TimespanType)pair.Left) + ((DateType)pair.Right);
                        case DataType.Time:
                            return ((TimespanType)pair.Left) + ((TimeType)pair.Right);
                        case DataType.Timespan:
                            return ((TimespanType)pair.Left) + ((TimespanType)pair.Right);
                    }
                    break;
                default:
                    throw new InvalidCalcOperationException($"Unhandled data type {pair.Left.DataType}");
            }
            return null;
        }
        #endregion Add

        #region Subtract
        static IDataType<DataType> Subtract(DataTypeValuePair<DataType> pair)
        {
            /*
            Subtract (-)        LimitedInteger      LimitedInteger
                                UnlimitedInteger    UnlimitedInteger
                                Float               Float
                                Date                Date, Timespan
                                Time                Time, Timespan
                                Timespan            Timespan
            */

            switch (pair.Left.DataType)
            {
                case DataType.LimitedInteger:
                    if (pair.Right.DataType == DataType.LimitedInteger)
                        return ((LimitedIntegerType)pair.Left) - ((LimitedIntegerType)pair.Right);
                    break;
                case DataType.UnlimitedInteger:
                    if (pair.Right.DataType == DataType.UnlimitedInteger)
                        return ((UnlimitedIntegerType)pair.Left) - ((UnlimitedIntegerType)pair.Right);
                    break;
                case DataType.Float:
                    if (pair.Right.DataType == DataType.Float)
                        return ((FloatType)pair.Left) - ((FloatType)pair.Right);
                    break;
                case DataType.Date:
                    switch (pair.Right.DataType)
                    {
                        case DataType.Date:
                            return ((DateType)pair.Left) - ((DateType)pair.Right);
                        case DataType.Timespan:
                            return ((DateType)pair.Left) - ((TimespanType)pair.Right);
                    }
                    break;
                case DataType.Time:
                    switch(pair.Right.DataType)
                    {
                        case DataType.Time:
                            return ((TimeType)pair.Left) - ((TimeType)pair.Right);
                        case DataType.Timespan:
                            return ((TimeType)pair.Left) - ((TimespanType)pair.Right);
                    }
                    break;
                case DataType.Timespan:
                    if (pair.Right.DataType == DataType.Timespan)
                        return ((TimespanType)pair.Left) - ((TimespanType)pair.Right);
                    break;
                default:
                    throw new InvalidCalcOperationException($"Unhandled data type {pair.Left.DataType}");
            }
            return null;
        }
        #endregion Subtract

    }
}
