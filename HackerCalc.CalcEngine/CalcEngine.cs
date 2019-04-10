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
        private CalcEngine()
        {
        }
        #endregion Constructors

        public static CalcEngine Instance => _instance.Value;
        static Lazy<CalcEngine> _instance = new Lazy<CalcEngine>(() => new CalcEngine());

        #region ICalcEngine implementation
        IDataType ICalcEngine.ConvertToDataType(ILiteralToken token, IConfiguration configuration)
            => DataMapper.Map(token, configuration);
        public IDataType<DataType> ConvertToTypedDataType(ILiteralToken token, IConfiguration configuration)
            => DataMapper.Map(token, configuration);
        IDataType ICalcEngine.Calculate(IConfiguration configuration, OperatorType opType, params IDataType[] operands)
            => Calculate(configuration, opType, operands.Select(o => (IDataType<DataType>)o).ToArray());
        #endregion ICalcEngine implementation

        public IDataType<DataType> Calculate(IConfiguration configuration, OperatorType opType, params IDataType<DataType>[] operands)
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

                string errorMsg;
                switch (opType)
                {
                    case OperatorType.Add: errorMsg = Properties.Resources.CALCENGINE_AddErrorMessage; break;
                    case OperatorType.Subtract: errorMsg = Properties.Resources.CALCENGINE_SubtractErrorMessage; break;
                    case OperatorType.Multiply: errorMsg = Properties.Resources.CALCENGINE_MultiplyErrorMessage; break;
                    case OperatorType.Divide: errorMsg = Properties.Resources.CALCENGINE_DivideErrorMessage; break;
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
                var currentValuePair = GetDataTypeValuePair(operands[0], operands[1]);
                var targetDataTypePair = DataMapper.GetOperandDataTypes(opType, currentValuePair);
                if (!targetDataTypePair.OperationSupported)
                    throw new InvalidCalcOperationException(
                        string.Format(errorMsg, 
                            currentValuePair.Left.DataType, 
                            currentValuePair.Left.ObjectValue, 
                            currentValuePair.Right.DataType, 
                            currentValuePair.Right.ObjectValue));

                // If it is supported, possibly cast the operands into something more useful
                var currentDataTypePair = GetDataTypePair(operands[0], operands[1]);
                if (currentDataTypePair != targetDataTypePair)
                    operands = new IDataType<DataType>[] 
                    {
                        operands[0].CastTo(targetDataTypePair.Left, configuration),
                        operands[1].CastTo(targetDataTypePair.Right, configuration)
                    };

                // Attempt to perform the operation
                return InternalDataTypeBase.Operate(configuration, opType, operands)
                    ?? throw new InvalidCalcOperationException(
                        string.Format(errorMsg,
                            currentValuePair.Left.DataType,
                            currentValuePair.Left.ObjectValue,
                            currentValuePair.Right.DataType,
                            currentValuePair.Right.ObjectValue));
            }
            else
                throw new InvalidCalcOperationException("Only unary and binary operations are supported");
            return null;
        }

        internal static DataTypePair<DataType> GetDataTypePair(IDataType<DataType> left, IDataType<DataType> right)
            => new DataTypePair<DataType>(left.DataType, right.DataType);

        internal static DataTypeValuePair<DataType> GetDataTypeValuePair(IDataType<DataType> left, IDataType<DataType> right)
            => new DataTypeValuePair<DataType>(left, right);
    }
}
