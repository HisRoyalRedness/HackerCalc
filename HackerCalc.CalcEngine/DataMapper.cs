using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    using DTP = DataTypePair<DataType>;
    using DTVP = DataTypeValuePair<DataType>;

    public static class DataMapper
    {
        static DataMapper()
        {
            BuildOperandTypeCastMap();
            _allTypeCasts = new Lazy<Dictionary<DataType, HashSet<DataType>>>(() => BuildTypeCastSummaryMaps());
        }

        public static DTP GetOperandDataTypes(OperatorType opType, DTP operandTypePair)
            => _operandTypeCastMap[opType][operandTypePair];
        public static DTP GetOperandDataTypes(OperatorType opType, DTVP valuePair)
            => _operandTypeCastMap[opType][new DataTypePair<DataType>(valuePair.Left.DataType, valuePair.Right.DataType)];

        // For unit tests
        public static Dictionary<OperatorType, Dictionary<DTP, DTP>> OperandTypeCastMap => _operandTypeCastMap;

        #region Map - Convert literal token to data type
        public static IDataType<DataType> Map(ILiteralToken token)
        {
            switch (token?.LiteralType)
            {
                case null:
                    return null;
                case LiteralTokenType.Date:
                    return ConvertToTypedDataType<DateTime, DateToken, DateType>(token, typedToken => new DateType(typedToken.TypedValue));
                case LiteralTokenType.Float:
                    return ConvertToTypedDataType<double, FloatToken, FloatType>(token, typedToken => new FloatType(typedToken.TypedValue));
                case LiteralTokenType.LimitedInteger:
                    return ConvertToTypedDataType<BigInteger, LimitedIntegerToken, LimitedIntegerType>(token, typedToken => new LimitedIntegerType(typedToken.TypedValue, ((LimitedIntegerToken)token).SignAndBitWidth));
                case LiteralTokenType.Time:
                    return ConvertToTypedDataType<TimeSpan, TimeToken, TimeType>(token, typedToken => new TimeType(typedToken.TypedValue));
                case LiteralTokenType.Timespan:
                    return ConvertToTypedDataType<TimeSpan, TimespanToken, TimespanType>(token, typedToken => new TimespanType(typedToken.TypedValue));
                case LiteralTokenType.UnlimitedInteger:
                    return ConvertToTypedDataType<BigInteger, UnlimitedIntegerToken, UnlimitedIntegerType>(token, typedToken => new UnlimitedIntegerType(typedToken.TypedValue));
                default:
                    throw new TypeConversionException($"Unhandled literal token type '{token.LiteralType}' while converting to a data type.");
            }
        }

        static IDataType<DataType> ConvertToTypedDataType<TBaseType, TTypedToken, TDataType>(ILiteralToken token, Func<TTypedToken, TDataType> createDataTypeFunc)
            where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
            where TDataType : class, IDataType<DataType>
            => (token is TTypedToken typedToken)
                ? createDataTypeFunc(typedToken)
                : null;
        #endregion Map - Convert literal token to data type

        // Not ideal, but gets the job done for now...
        public static CalcSettings Settings { get; set; }
        public static CalcState State { get; set; }

        static void BuildOperandTypeCastMap()
        {
            // Binary operations
            #region Add
            _operandTypeCastMap.Add(OperatorType.Add, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Time),                 new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Timespan),             new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     new DTP(DataType.Date,                  DataType.Timespan) },

                { new DTP(DataType.Float,               DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 new DTP(DataType.Timespan,              DataType.Time) },
                { new DTP(DataType.Float,               DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 new DTP(DataType.Timespan,              DataType.Time) },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Time,                DataType.Float),                new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.Time),                 new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.Timespan),             new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     new DTP(DataType.Time,                  DataType.Timespan) },

                { new DTP(DataType.Timespan,            DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Timespan,            DataType.Float),                new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.Time),                 new DTP(DataType.Timespan,              DataType.Time) },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     new DTP(DataType.Timespan,              DataType.Timespan) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 new DTP(DataType.Timespan,              DataType.Time) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Add

            #region Subtract
            _operandTypeCastMap.Add(OperatorType.Subtract, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 new DTP(DataType.Date,                  DataType.Date) },
                { new DTP(DataType.Date,                DataType.Float),                new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Time),                 new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.Timespan),             new DTP(DataType.Date,                  DataType.Timespan) },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     new DTP(DataType.Date,                  DataType.Timespan) },

                { new DTP(DataType.Float,               DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Time,                DataType.Float),                new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             new DTP(DataType.Time,                  DataType.Timespan) },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     new DTP(DataType.Time,                  DataType.Timespan) },

                { new DTP(DataType.Timespan,            DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Timespan,            DataType.Float),                new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     new DTP(DataType.Timespan,              DataType.Timespan) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Subtract

            #region Multiply
            _operandTypeCastMap.Add(OperatorType.Multiply, new Dictionary<DTP, DTP>()
            {

                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             new DTP(DataType.Float,                 DataType.Timespan) },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             new DTP(DataType.Float,                 DataType.Timespan) },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     new DTP(DataType.Timespan,              DataType.Float) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             new DTP(DataType.Float,                 DataType.Timespan) },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Multiply

            #region Divide
            _operandTypeCastMap.Add(OperatorType.Divide, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     new DTP(DataType.Timespan,              DataType.Float) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Divide

            #region LeftShift
            _operandTypeCastMap.Add(OperatorType.LeftShift, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion LeftShift

            #region RightShift
            _operandTypeCastMap.Add(OperatorType.RightShift, _operandTypeCastMap[OperatorType.LeftShift]);            
            #endregion RightShift

            #region Power
            _operandTypeCastMap.Add(OperatorType.Power, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.LimitedInteger,        DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.UnlimitedInteger,      DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Power

            #region Root
            _operandTypeCastMap.Add(OperatorType.Root, _operandTypeCastMap[OperatorType.Power]);
            #endregion Root

            #region Modulo
            _operandTypeCastMap.Add(OperatorType.Modulo, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.Float,                 DataType.Float) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.UnlimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.Float) },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     new DTP(DataType.Timespan,              DataType.Float) },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion Modulo

            #region And
            _operandTypeCastMap.Add(OperatorType.And, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Float,               DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.Float,               DataType.LimitedInteger),       new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.Float,               DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },

                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Float),                new DTP(DataType.LimitedInteger,        DataType.UnlimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.UnlimitedInteger),     new DTP(DataType.LimitedInteger,        DataType.UnlimitedInteger) },

                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Float),                DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.UnlimitedInteger),     DTP.Unsupported },

                { new DTP(DataType.UnlimitedInteger,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Float),                new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.LimitedInteger),       new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.UnlimitedInteger,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion And

            #region Or
            _operandTypeCastMap.Add(OperatorType.Or, _operandTypeCastMap[OperatorType.And]);
            #endregion Or

            #region Xor
            _operandTypeCastMap.Add(OperatorType.Xor, _operandTypeCastMap[OperatorType.And]);
            #endregion Xor

            // Unary operations
            #region NumericNegate
            _operandTypeCastMap.Add(OperatorType.NumericNegate, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.Float,                 DataType.Float) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion NumericNegate

            #region BitwiseNegate
            _operandTypeCastMap.Add(OperatorType.BitwiseNegate, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Float,               DataType.Float),                new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
                { new DTP(DataType.UnlimitedInteger,    DataType.UnlimitedInteger),     new DTP(DataType.UnlimitedInteger,      DataType.UnlimitedInteger) },
            });
            #endregion BitwiseNegate
        }

        #region Support and testing
        static Dictionary<DataType, HashSet<DataType>> BuildTypeCastSummaryMaps()
        {
            var allPairCasts = DataMapper.OperandTypeCastMap
                .SelectMany(op => op.Value)
                .Where(pairs => pairs.Value.OperationSupported)
                .ToList();
            var allTypeCasts =
                allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Left, pair.Value.Left)).Concat(
                allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Right, pair.Value.Right)))
                .ToList();
            var allCasts = new Dictionary<DataType, HashSet<DataType>>();
            foreach (var cast in allTypeCasts)
            {
                if (!allCasts.ContainsKey(cast.Key))
                    allCasts.Add(cast.Key, new HashSet<DataType>());
                if (cast.Key != cast.Value)
                    allCasts[cast.Key].Add(cast.Value);
            }
            return allCasts;
        }

        public static void PrintAllTypeCasts()
        {
            foreach (var cast in _allTypeCasts.Value)
            {
                Console.WriteLine($"{cast.Key}:");
                foreach (var val in cast.Value)
                    Console.WriteLine($"   {cast.Key} --> {val}");
                Console.WriteLine();
            }
        }

        public static Lazy<Dictionary<DataType, HashSet<DataType>>> AllTypeCasts => _allTypeCasts;
        static readonly Lazy<Dictionary<DataType, HashSet<DataType>>> _allTypeCasts;
        #endregion Support and testing

        readonly static Dictionary<OperatorType, Dictionary<DTP, DTP>> _operandTypeCastMap = new Dictionary<OperatorType, Dictionary<DTP, DTP>>();
    }
}
