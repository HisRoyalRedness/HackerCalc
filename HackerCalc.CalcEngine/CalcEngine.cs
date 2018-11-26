﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class CalcEngine : ICalcEngine<DataType>
    {
        #region Constructors
        public CalcEngine()
            : this(null)
        { }

        public CalcEngine(CalcSettings settings, CalcState state = null)
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
                Func<DataTypeValuePair<DataType>, string> errorFunc = null;
                switch (opType)
                {
                    case OperatorType.Add: opFunc = Add; errorFunc = AddErrorMessage; break;
                    case OperatorType.Subtract:
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
                    throw new InvalidCalcOperationException(errorFunc(currentValuePair));

                var currentDataTypePair = GetDataTypePair(operands[0], operands[1]);
                if (currentDataTypePair != targetDataTypePair)
                    currentValuePair = GetDataTypeValuePair(operands[0].CastTo(targetDataTypePair.Left), operands[1].CastTo(targetDataTypePair.Right));
                return opFunc(currentValuePair);
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
            //Add (+)             LimitedInteger      LimitedInteger
            //                    Date                Timespan
            //                    Timespan            Date, Time, Timespan
            //                    Float               Float
            //                    Time                Timespan
            //                    UnlimitedInteger    UnlimitedInteger

            switch (pair.Left.DataType)
            {
                case DataType.LimitedInteger:
                    if (pair.Right.DataType == DataType.LimitedInteger)
                        return ((LimitedIntegerType)pair.Left) + ((LimitedIntegerType)pair.Right);
                    break;
                case DataType.Date:
                    if (pair.Right.DataType == DataType.Timespan)
                        return new DateType(((DateType)pair.Left).Value + ((TimespanType)pair.Right).Value);
                    break;
                case DataType.Float:
                    if (pair.Right.DataType == DataType.Float)
                        return ((FloatType)pair.Left) + ((FloatType)pair.Right);
                    break;
                case DataType.Time:
                    if (pair.Right.DataType == DataType.Timespan)
                        return ((TimeType)pair.Left) + ((TimespanType)pair.Right);
                    break;
                case DataType.Timespan:
                    switch (pair.Right.DataType)
                    {
                        case DataType.Date:
                            return ((TimespanType)pair.Right) + ((DateType)pair.Left);
                        case DataType.Time:
                            return ((TimespanType)pair.Left) + ((TimeType)pair.Right);
                        case DataType.Timespan:
                            return ((TimespanType)pair.Left) + ((TimespanType)pair.Right);
                    }
                    break;
                case DataType.UnlimitedInteger:
                    if (pair.Right.DataType == DataType.UnlimitedInteger)
                        return ((UnlimitedIntegerType)pair.Left) + ((UnlimitedIntegerType)pair.Right);
                    break;
                default:
                    throw new InvalidCalcOperationException($"Unhandled data type {pair.Left.DataType}");
            }
            throw new InvalidCalcOperationException($"Unhandled data type {pair.Right.DataType}");
        }
        static string AddErrorMessage(DataTypeValuePair<DataType> pair) => $"Can't add a {pair.Left.DataType} ({pair.Left.ObjectValue}) to a {pair.Right.DataType} ({pair.Right.ObjectValue})";
        #endregion Add
        
    }
}
