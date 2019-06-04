using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

/*
    DataMapper

        Implemention of the DataMapper. This class is responsible
        for the conversion between different data types, as well
        as which operand types are accepted for a given operation

    Keith Fletcher
    Apr 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    using DTP = DataTypePair<DataType>;
    using DTVP = DataTypeValuePair<DataType>;

    public static class DataMapper
    {
        static DataMapper()
        {
            BuildOperandTypeCastMap();
            AllTypeCasts = new Lazy<Dictionary<DataType, HashSet<DataType>>>(() => BuildTypeCastSummaryMaps());
            AllInputOperandTypes = new Lazy<Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>>>(() => BuildOperandTypeMaps(true));
            AllSupportedOperationTypes = new Lazy<Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>>>(() => BuildOperandTypeMaps(false));
        }

        public static DTP GetOperandDataTypes(OperatorType opType, DTP operandTypePair)
            => OperandTypeCastMap[opType][operandTypePair];
        public static DTP GetOperandDataTypes(OperatorType opType, DTVP valuePair)
            => OperandTypeCastMap[opType][new DataTypePair<DataType>(valuePair.Left.DataType, valuePair.Right.DataType)];

        // For unit tests
        public static Dictionary<OperatorType, Dictionary<DTP, DTP>> OperandTypeCastMap { get; } = new Dictionary<OperatorType, Dictionary<DTP, DTP>>();

        #region Map - Convert literal token to data type
        public static IDataType<DataType> Map(ILiteralToken token, IConfiguration configuration)
        {
            switch (token?.LiteralType)
            {
                case null:
                    return null;
                case LiteralTokenType.LimitedInteger:
                    return ConvertToTypedDataType<BigInteger, LimitedIntegerToken, LimitedIntegerType>(token, typedToken => new LimitedIntegerType(typedToken.TypedValue, ((LimitedIntegerToken)token).SignAndBitWidth, configuration));
                case LiteralTokenType.Rational:
                    return ConvertToTypedDataType<RationalNumber, RationalNumberToken, RationalNumberType>(token, typedToken => new RationalNumberType(typedToken.TypedValue));
                case LiteralTokenType.Date:
                    return ConvertToTypedDataType<DateTime, DateToken, DateType>(token, typedToken => new DateType(typedToken.TypedValue));
                case LiteralTokenType.Time:
                    return ConvertToTypedDataType<TimeSpan, TimeToken, TimeType>(token, typedToken => new TimeType(typedToken.TypedValue));
                case LiteralTokenType.Timespan:
                    return ConvertToTypedDataType<TimeSpan, TimespanToken, TimespanType>(token, typedToken => new TimespanType(typedToken.TypedValue));

                default:
                    throw new TypeConversionException($"Unhandled literal token type '{token.LiteralType}' while converting to a data type.");
            }
        }

        static IDataType<DataType> ConvertToTypedDataType<TBaseType, TTypedToken, TDataType>(ILiteralToken token, Func<TTypedToken, TDataType> createDataTypeFunc)
            where TTypedToken : class, ILiteralToken, ILiteralToken<TBaseType, TTypedToken>
            where TDataType : class, IDataType<DataType>
            where TBaseType : IComparable<TBaseType>, IComparable

            => (token is TTypedToken typedToken)
                ? createDataTypeFunc(typedToken)
                : null;
        #endregion Map - Convert literal token to data type

        static void BuildOperandTypeCastMap()
        {
            // Binary operations
            #region Add
            OperandTypeCastMap.Add(OperatorType.Add, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             new DTP(DataType.Date,                  DataType.Timespan) },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             new DTP(DataType.Time,                  DataType.Timespan) },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 new DTP(DataType.Timespan,              DataType.Date) },
                { new DTP(DataType.Timespan,            DataType.Time),                 new DTP(DataType.Timespan,              DataType.Time) },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },

            });
            #endregion Add

            #region Subtract
            OperandTypeCastMap.Add(OperatorType.Subtract, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 new DTP(DataType.Date,                  DataType.Date) },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             new DTP(DataType.Date,                  DataType.Timespan) },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 new DTP(DataType.Time,                  DataType.Time) },
                { new DTP(DataType.Time,                DataType.Timespan),             new DTP(DataType.Time,                  DataType.Timespan) },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
            });
            #endregion Subtract

            #region Multiply
            OperandTypeCastMap.Add(OperatorType.Multiply, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.LimitedInteger) },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       new DTP(DataType.Timespan,              DataType.RationalNumber) },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     new DTP(DataType.Timespan,              DataType.IrrationalNumber) },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
            });
            #endregion Multiply

            #region Divide
            OperandTypeCastMap.Add(OperatorType.Divide, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       new DTP(DataType.Timespan,              DataType.LimitedInteger) },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       new DTP(DataType.Timespan,              DataType.RationalNumber) },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     new DTP(DataType.Timespan,              DataType.IrrationalNumber) },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },
            });
            #endregion Divide

            #region LeftShift
            OperandTypeCastMap.Add(OperatorType.LeftShift, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.RationalNumber,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.RationalNumber,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
            });
            #endregion LeftShift

            #region RightShift
            OperandTypeCastMap.Add(OperatorType.RightShift, OperandTypeCastMap[OperatorType.LeftShift]);
            #endregion RightShift

            #region Power
            OperandTypeCastMap.Add(OperatorType.Power, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported},

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
            });
            #endregion Power

            #region Root
            OperandTypeCastMap.Add(OperatorType.Root, OperandTypeCastMap[OperatorType.Power]);
            #endregion Root

            #region Modulo
            OperandTypeCastMap.Add(OperatorType.Modulo, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
            });
            #endregion Modulo

            #region And
            OperandTypeCastMap.Add(OperatorType.And, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.IrrationalNumber),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.LimitedInteger,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.LimitedInteger,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.RationalNumber,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.IrrationalNumber),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.RationalNumber,      DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.IrrationalNumber,    DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.IrrationalNumber,    DataType.RationalNumber),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.IrrationalNumber,    DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Date,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Time,                DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Timespan),             DTP.Unsupported },

                { new DTP(DataType.Timespan,            DataType.LimitedInteger),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },


            });
            #endregion And

            #region Or
            OperandTypeCastMap.Add(OperatorType.Or, OperandTypeCastMap[OperatorType.And]);
            #endregion Or

            #region Xor
            OperandTypeCastMap.Add(OperatorType.Xor, OperandTypeCastMap[OperatorType.And]);
            #endregion Xor

            // Unary operations
            #region NumericNegate
            OperandTypeCastMap.Add(OperatorType.NumericNegate, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       new DTP(DataType.RationalNumber,        DataType.RationalNumber) },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     new DTP(DataType.IrrationalNumber,      DataType.IrrationalNumber) },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             new DTP(DataType.Timespan,              DataType.Timespan) },

            });
            #endregion NumericNegate

            #region BitwiseNegate
            OperandTypeCastMap.Add(OperatorType.BitwiseNegate, new Dictionary<DTP, DTP>()
            {
                { new DTP(DataType.LimitedInteger,      DataType.LimitedInteger),       new DTP(DataType.LimitedInteger,        DataType.LimitedInteger) },
                { new DTP(DataType.RationalNumber,      DataType.RationalNumber),       DTP.Unsupported },
                { new DTP(DataType.IrrationalNumber,    DataType.IrrationalNumber),     DTP.Unsupported },
                { new DTP(DataType.Date,                DataType.Date),                 DTP.Unsupported },
                { new DTP(DataType.Time,                DataType.Time),                 DTP.Unsupported },
                { new DTP(DataType.Timespan,            DataType.Timespan),             DTP.Unsupported },
            });
            #endregion BitwiseNegate
        }

        #region Support and testing
        static Dictionary<DataType, HashSet<DataType>> BuildTypeCastSummaryMaps()
        {
            var allPairCasts = DataMapper.OperandTypeCastMap
                .SelectMany(op => op.Value)
                .Where(pairs => pairs.Value.OperationSupported);
            var allTypeCasts =
                allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Left, pair.Value.Left)).Concat(
                allPairCasts.Select(pair => new KeyValuePair<DataType, DataType>(pair.Key.Right, pair.Value.Right)));
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

        // List of operand types, grouped by operations, that are supported as input operand types
        static Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>> BuildOperandTypeMaps(bool inputTypes = false)
        {
            var dict = new Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>>();
            foreach (var opType in EnumExtensions.GetEnumCollection<OperatorType>())
            {
                if (!dict.ContainsKey(opType))
                    dict.Add(opType, new Dictionary<DataType, HashSet<DataType>>());
                var subDict = dict[opType];
                var opTypesByOperator = DataMapper.OperandTypeCastMap[opType]
                    .Where(kv => kv.Value.OperationSupported)
                    .Select(kv => new Tuple<OperatorType, DataType, DataType>(
                        opType,
                        inputTypes ? kv.Key.Left : kv.Value.Left,
                        inputTypes ? kv.Key.Right : kv.Value.Right))
                    .Distinct();
                foreach (var entry in opTypesByOperator)
                {
                    if (!subDict.ContainsKey(entry.Item2))
                        subDict.Add(entry.Item2, new HashSet<DataType>());
                    if (!subDict[entry.Item2].Contains(entry.Item3))
                        subDict[entry.Item2].Add(entry.Item3);
                }
            }
            return dict;
        }

        public static void PrintAllTypeCasts()
        {
            foreach (var cast in AllTypeCasts.Value.OrderBy(k => k))
            {
                Console.WriteLine($"{cast.Key}:");
                foreach (var val in cast.Value.OrderBy(k => k))
                    Console.WriteLine($"   {cast.Key} --> {val}");
                Console.WriteLine();
            }
        }

        public static void PrintOperandTypes(Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>> dict)
        {
            foreach (var opType in dict.Keys.OrderBy(k => k))
            {
                var opStr = $"{opType} ({opType.GetEnumDescription()})";
                var firstCol = $"{opStr,-20}";
                foreach (var rightType in dict[opType].Keys.OrderBy(k => k))
                {
                    var scndCol = $"{rightType,-20}";
                    var csv = string.Join(", ", dict[opType][rightType].OrderBy(k => k).Select(op => op.ToString()));

                    Console.WriteLine($"{firstCol}{scndCol}{csv}");
                    firstCol = $"{"",-20}";
                    scndCol = $"{"",-20}";
                }
                Console.WriteLine();
            }
        }

        public static Lazy<Dictionary<DataType, HashSet<DataType>>> AllTypeCasts { get; private set; }
        public static Lazy<Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>>> AllInputOperandTypes { get; private set; }
        public static Lazy<Dictionary<OperatorType, Dictionary<DataType, HashSet<DataType>>>> AllSupportedOperationTypes { get; private set; }

        #endregion Support and testing
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
