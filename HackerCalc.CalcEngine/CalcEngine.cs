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
        #region ICalcEngine implementation
        IDataType ICalcEngine.ConvertToDataType(ILiteralToken token)
            => DataMapper.Map(token);
        public IDataType<DataType> ConvertToTypedDataType(ILiteralToken token)
            => DataMapper.Map(token);
        IDataType ICalcEngine.Calculate(OperatorType opType, params IDataType[] operands)
            => Calculate(opType, operands.Select(o => (IDataType<DataType>)o).ToArray());
        #endregion ICalcEngine implementation

        public IDataType<DataType> Calculate(OperatorType opType, params IDataType<DataType>[] operands)
        {
            // Unary
            if (operands.Length == 1)
            {
                if (!opType.IsBinaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a unary operator and expects two operands.");

            }
            // Binary
            else if (operands.Length == 2)
            {
                if(!opType.IsBinaryOperator())
                    throw new InvalidCalcOperationException($"Operator '{opType.GetEnumDescription()}' is not a binary operator and expects a single operand.");
                
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
                {
                    //currentValuePair = GetDataTypePair(operands[0].c)
                    // DO cast
                }
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
            switch (pair.Left.DataType)
            {
                //case TokenDataType.Date:
                //    if (pair.Right.DataType == TokenDataType.Timespan)
                //        return ((OldDateToken)pair.Left) + ((OldTimespanToken)pair.Right);
                //    break;
                //case TokenDataType.Float:
                //    if (pair.Right.DataType == TokenDataType.Float)
                //        return ((OldFloatToken)pair.Left) + ((OldFloatToken)pair.Right);
                //    break;
                //case TokenDataType.LimitedInteger:
                //    if (pair.Right.DataType == TokenDataType.LimitedInteger)
                //        return ((OldLimitedIntegerToken)pair.Left) + ((OldLimitedIntegerToken)pair.Right);
                //    break;
                //case TokenDataType.Time:
                //    if (pair.Right.DataType == TokenDataType.Timespan)
                //        return ((OldTimeToken)pair.Left) + ((OldTimespanToken)pair.Right);
                //    break;
                //case TokenDataType.Timespan:
                //    switch (pair.Right.DataType)
                //    {
                //        case TokenDataType.Date:
                //            return ((OldDateToken)pair.Right) + ((OldTimespanToken)pair.Left);
                //        case TokenDataType.Time:
                //            return ((OldTimespanToken)pair.Left) + ((OldTimeToken)pair.Right);
                //        case TokenDataType.Timespan:
                //            return ((OldTimespanToken)pair.Left) + ((OldTimespanToken)pair.Right);
                //    }
                //    break;
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
