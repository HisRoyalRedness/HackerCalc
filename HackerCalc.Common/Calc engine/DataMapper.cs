using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IDataMapper
    {
        IDataType Map(ILiteralToken token);
        DataTypePair GetOperandDataTypes(OperatorType opType, DataTypePair operandTypePair);
    }

    public class DataMapper : IDataMapper
    {
        public IDataType Map(ILiteralToken token)
        {
            switch(token.LiteralType)
            {
                case LiteralTokenType.UnlimitedInteger:
                    return new UnlimitedIntegerType((token as UnlimitedIntegerToken).TypedValue);
            }
            return null;
        }

        static DataMapper()
        {
            BuildOperandTypeCastMap();
        }

        public DataTypePair GetOperandDataTypes(OperatorType opType, DataTypePair operandTypePair)
            => _operandTypeCastMap[opType][operandTypePair];

        static void BuildOperandTypeCastMap()
        {
            _operandTypeCastMap.Add(OperatorType.Add, new Dictionary<DataTypePair, DataTypePair>()
            {
                { new DataTypePair(DataType.Date, DataType.Date),                              DataTypePair.Unsupported },
                { new DataTypePair(DataType.Date, DataType.Float),                             new DataTypePair(DataType.Date, DataType.Timespan) },
                { new DataTypePair(DataType.Date, DataType.LimitedInteger),                    new DataTypePair(DataType.Date, DataType.Timespan) },
                { new DataTypePair(DataType.Date, DataType.Time),                              new DataTypePair(DataType.Date, DataType.Timespan) },
                { new DataTypePair(DataType.Date, DataType.Timespan),                          new DataTypePair(DataType.Date, DataType.Timespan) },
                { new DataTypePair(DataType.Date, DataType.UnlimitedInteger),                  new DataTypePair(DataType.Date, DataType.Timespan) },

                { new DataTypePair(DataType.Float, DataType.Date),                             new DataTypePair(DataType.Timespan, DataType.Date) },
                { new DataTypePair(DataType.Float, DataType.Float),                            new DataTypePair(DataType.Float, DataType.Float) },
                { new DataTypePair(DataType.Float, DataType.LimitedInteger),                   new DataTypePair(DataType.Float, DataType.Float) },
                { new DataTypePair(DataType.Float, DataType.Time),                             new DataTypePair(DataType.Timespan, DataType.Time) },
                { new DataTypePair(DataType.Float, DataType.Timespan),                         new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.Float, DataType.UnlimitedInteger),                 new DataTypePair(DataType.Float, DataType.Float) },

                { new DataTypePair(DataType.LimitedInteger, DataType.Date),                    new DataTypePair(DataType.Timespan, DataType.Date) },
                { new DataTypePair(DataType.LimitedInteger, DataType.Float),                   new DataTypePair(DataType.Float, DataType.Float) },
                { new DataTypePair(DataType.LimitedInteger, DataType.LimitedInteger),          new DataTypePair(DataType.LimitedInteger, DataType.LimitedInteger) },
                { new DataTypePair(DataType.LimitedInteger, DataType.Time),                    new DataTypePair(DataType.Timespan, DataType.Time) },
                { new DataTypePair(DataType.LimitedInteger, DataType.Timespan),                new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.LimitedInteger, DataType.UnlimitedInteger),        new DataTypePair(DataType.LimitedInteger, DataType.LimitedInteger) },

                { new DataTypePair(DataType.Time, DataType.Date),                              new DataTypePair(DataType.Timespan, DataType.Date) },
                { new DataTypePair(DataType.Time, DataType.Float),                             new DataTypePair(DataType.Time, DataType.Timespan) },
                { new DataTypePair(DataType.Time, DataType.LimitedInteger),                    new DataTypePair(DataType.Time, DataType.Timespan) },
                { new DataTypePair(DataType.Time, DataType.Time),                              new DataTypePair(DataType.Time, DataType.Timespan) },
                { new DataTypePair(DataType.Time, DataType.Timespan),                          new DataTypePair(DataType.Time, DataType.Timespan) },
                { new DataTypePair(DataType.Time, DataType.UnlimitedInteger),                  new DataTypePair(DataType.Time, DataType.Timespan) },

                { new DataTypePair(DataType.Timespan, DataType.Date),                          new DataTypePair(DataType.Timespan, DataType.Date) },
                { new DataTypePair(DataType.Timespan, DataType.Float),                         new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.Timespan, DataType.LimitedInteger),                new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.Timespan, DataType.Time),                          new DataTypePair(DataType.Timespan, DataType.Time) },
                { new DataTypePair(DataType.Timespan, DataType.Timespan),                      new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.Timespan, DataType.UnlimitedInteger),              new DataTypePair(DataType.Timespan, DataType.Timespan) },

                { new DataTypePair(DataType.UnlimitedInteger, DataType.Date),                  new DataTypePair(DataType.Timespan, DataType.Date) },
                { new DataTypePair(DataType.UnlimitedInteger, DataType.Float),                 new DataTypePair(DataType.Float, DataType.Float) },
                { new DataTypePair(DataType.UnlimitedInteger, DataType.LimitedInteger),        new DataTypePair(DataType.LimitedInteger, DataType.LimitedInteger) },
                { new DataTypePair(DataType.UnlimitedInteger, DataType.Time),                  new DataTypePair(DataType.Timespan, DataType.Time) },
                { new DataTypePair(DataType.UnlimitedInteger, DataType.Timespan),              new DataTypePair(DataType.Timespan, DataType.Timespan) },
                { new DataTypePair(DataType.UnlimitedInteger, DataType.UnlimitedInteger),      new DataTypePair(DataType.UnlimitedInteger, DataType.UnlimitedInteger) },

            });


            //        static readonly BinaryOperandTypeMap _addMapping = new BinaryOperandTypeMap()
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



            //var bw in EnumExtensions.GetEnumCollection<OldLimitedIntegerToken.IntegerBitWidth>()
            //foreach (var tp in EnumExtensions.GetEnumCollection<OperatorType>(e => e.IsBinaryOperator()))
            //{

            //}
        }


        readonly static Dictionary<OperatorType, Dictionary<DataTypePair, DataTypePair>> _operandTypeCastMap = new Dictionary<OperatorType, Dictionary<DataTypePair, DataTypePair>>();
        //readonly static Dictionary<DataTypePair, DataTypePair> _operandTypeCastMap = new Dictionary<DataTypePair, DataTypePair>();
    }
}
