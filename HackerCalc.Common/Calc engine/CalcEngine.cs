using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface ICalcEngine
    {
        IDataType Calculate(IDataType left, IDataType right, OperatorType opType);
        IDataMapper DataMapper { get; }
    }

    public class CalcEngine : ICalcEngine
    {
        public CalcEngine(IDataMapper dataMapper)
        {
            DataMapper = dataMapper;
        }

        public IDataType Calculate(IDataType left, IDataType right, OperatorType opType)
        {
            var currentDataTypePair = GetDataTypePair(left, right);
            var targetDataTypePair = DataMapper.GetOperandDataTypes(opType, currentDataTypePair);
            return null;
        }

        static DataTypePair GetDataTypePair(IDataType left, IDataType right)
            => new DataTypePair(left.DataType, right.DataType);

        #region Add
        static IDataType Add(DataTypeValuePair pair)
        {
            return null;
            //switch (pair.Left.DataType)
            //{
            //    //case TokenDataType.Date:
            //    //    if (pair.Right.DataType == TokenDataType.Timespan)
            //    //        return ((OldDateToken)pair.Left) + ((OldTimespanToken)pair.Right);
            //    //    break;
            //    //case TokenDataType.Float:
            //    //    if (pair.Right.DataType == TokenDataType.Float)
            //    //        return ((OldFloatToken)pair.Left) + ((OldFloatToken)pair.Right);
            //    //    break;
            //    //case TokenDataType.LimitedInteger:
            //    //    if (pair.Right.DataType == TokenDataType.LimitedInteger)
            //    //        return ((OldLimitedIntegerToken)pair.Left) + ((OldLimitedIntegerToken)pair.Right);
            //    //    break;
            //    //case TokenDataType.Time:
            //    //    if (pair.Right.DataType == TokenDataType.Timespan)
            //    //        return ((OldTimeToken)pair.Left) + ((OldTimespanToken)pair.Right);
            //    //    break;
            //    //case TokenDataType.Timespan:
            //    //    switch (pair.Right.DataType)
            //    //    {
            //    //        case TokenDataType.Date:
            //    //            return ((OldDateToken)pair.Right) + ((OldTimespanToken)pair.Left);
            //    //        case TokenDataType.Time:
            //    //            return ((OldTimespanToken)pair.Left) + ((OldTimeToken)pair.Right);
            //    //        case TokenDataType.Timespan:
            //    //            return ((OldTimespanToken)pair.Left) + ((OldTimespanToken)pair.Right);
            //    //    }
            //    //    break;
            //    case DataType.UnlimitedInteger:
            //        if (pair.Right.DataType == DataType.UnlimitedInteger)
            //            return ((UnlimitedIntegerType)pair.Left) + ((UnlimitedIntegerType)pair.Right);
            //        break;
            //    default:
            //        throw new UnrecognisedTokenException($"Unhandled data type {pair.Left.DataType}");
            //}
            //throw new UnrecognisedTokenException($"Unhandled data type {pair.Right.DataType}");
        }

        //static string AddErrorMessage(OperatorProperties opProp, TokenPair pair) => $"Can't add a {pair.Left.DataType} ({pair.Left.ObjectValue}) to a {pair.Right.DataType} ({pair.Right.ObjectValue})";

        //static readonly BinaryOperandTypeMap _addMapping = new BinaryOperandTypeMap()
        //        {
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Date),                              OperandTypePair.Unsupported },
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan) },

        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Date),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.LimitedInteger),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Time),                             new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Timespan),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.UnlimitedInteger),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },

        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Date),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Float),                   new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Time),                    new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.Timespan),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.UnlimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },

        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Date),                              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Float),                             new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.LimitedInteger),                    new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Time),                              new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.UnlimitedInteger),                  new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan) },

        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Float),                         new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.LimitedInteger),                new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.UnlimitedInteger),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },

        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Date),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date) },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Float),                 new OperandTypePair(TokenDataType.Float, TokenDataType.Float) },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.LimitedInteger),        new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger) },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Time),                  new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time) },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.Timespan),              new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan) },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger) },
        //        };

        //// Used for unit testing, to validate the return type after an operation on two input types
        //static readonly BinaryOperandResultTypeMap _addResultMapping = new BinaryOperandResultTypeMap()
        //        {
        //            { new OperandTypePair(TokenDataType.Date, TokenDataType.Timespan),                          TokenDataType.Date },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Date),                          TokenDataType.Date },
        //            { new OperandTypePair(TokenDataType.Float, TokenDataType.Float),                            TokenDataType.Float },
        //            { new OperandTypePair(TokenDataType.LimitedInteger, TokenDataType.LimitedInteger),          TokenDataType.LimitedInteger },
        //            { new OperandTypePair(TokenDataType.Time, TokenDataType.Timespan),                          TokenDataType.Time },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Time),                          TokenDataType.Time },
        //            { new OperandTypePair(TokenDataType.Timespan, TokenDataType.Timespan),                      TokenDataType.Timespan },
        //            { new OperandTypePair(TokenDataType.UnlimitedInteger, TokenDataType.UnlimitedInteger),      TokenDataType.UnlimitedInteger },
        //        };
        #endregion Add

        public IDataMapper DataMapper { get; private set; }

    }
}
