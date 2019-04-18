using System;
using System.Linq;

/*
    CalcEngine   

        Implementation of the calculation engine

    Keith Fletcher
    Apr 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

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
                    case OperatorType.Power: errorMsg = Properties.Resources.CALCENGINE_PowerErrorMessage; break;
                    case OperatorType.Root: errorMsg = Properties.Resources.CALCENGINE_RootErrorMessage; break;
                    case OperatorType.Modulo: errorMsg = Properties.Resources.CALCENGINE_ModuloErrorMessage; break;
                    case OperatorType.LeftShift: errorMsg = Properties.Resources.CALCENGINE_LeftShiftErrorMessage; break;
                    case OperatorType.RightShift: errorMsg = Properties.Resources.CALCENGINE_RightShiftErrorMessage; break;
                    case OperatorType.And: errorMsg = Properties.Resources.CALCENGINE_AndErrorMessage; break;
                    case OperatorType.Or: errorMsg = Properties.Resources.CALCENGINE_OrErrorMessage; break;
                    case OperatorType.Xor: errorMsg = Properties.Resources.CALCENGINE_XorErrorMessage; break;
                    default:
                        throw new InvalidCalcOperationException($"Unsupported binary operator '{opType.GetEnumDescription()}'.");
                }

                // The equation hasn't been fully entered yet.
                // Just return the bit that we do have
                if (operands[1] == null && Constants.IsPartialEquationAllowed)
                    return operands[0];

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

/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
*/
