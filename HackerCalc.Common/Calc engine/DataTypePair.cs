using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{Left}, {Right}")]
    public struct DataTypePair<TDataTypeEnum> : IEquatable<DataTypePair<TDataTypeEnum>>
        where TDataTypeEnum : Enum
    {
        public DataTypePair(TDataTypeEnum left, TDataTypeEnum right)
            : this(left, right, true)
        { }

        public DataTypePair(TDataTypeEnum left, TDataTypeEnum right, bool supported)
        {
            Left = left;
            Right = right;
            OperationSupported = supported;
        }

        public TDataTypeEnum Left { get; private set; }
        public TDataTypeEnum Right { get; private set; }
        public bool OperationSupported { get; private set; }

        public static DataTypePair<TDataTypeEnum> Unsupported { get; } = new DataTypePair<TDataTypeEnum>() { OperationSupported = false };

        #region Equality
        public override bool Equals(object obj)
            => (obj is DataTypePair<TDataTypeEnum> other)
                ? Equals(other)
                : false;
        public bool Equals(DataTypePair<TDataTypeEnum> other) => this == other;
        public override int GetHashCode() => (Left.GetHashCode() ^ 1500450271) + Right.GetHashCode();

        public static bool operator ==(DataTypePair<TDataTypeEnum> a, DataTypePair<TDataTypeEnum> b)
            => a.Left.Equals(b.Left) && a.Right.Equals(b.Right);

        public static bool operator !=(DataTypePair<TDataTypeEnum> a, DataTypePair<TDataTypeEnum> b) => !(a == b);
        #endregion Equality

        //public OperandTypePair TypesFromMap(BinaryOperandTypeMap map)
        //{
        //    var key = new OperandTypePair(Left.DataType, Right.DataType);
        //    return map.ContainsKey(key)
        //        ? map[key]
        //        : OperandTypePair.Unsupported;
        //}

        //public IDataType CastFromMap(BinaryOperandTypeMap map, EvaluatorSettings settings)
        //{
        //    var newPairType = TypesFromMap(map);
        //    if (!newPairType.OperationSupported)
        //        return null;
        //    return new IDataType(Left.CastTo(newPairType.Left), Right.CastTo(newPairType.Right));
        //}
    }

    [DebuggerDisplay("{Left}, {Right}")]
    public struct DataTypeValuePair<TDataTypeEnum> : IEquatable<DataTypeValuePair<TDataTypeEnum>>
        where TDataTypeEnum : Enum
    {
        public DataTypeValuePair(IDataType<TDataTypeEnum> left, IDataType<TDataTypeEnum> right)
        {
            Left = left;
            Right = right;
        }

        public IDataType<TDataTypeEnum> Left { get; private set; }
        public IDataType<TDataTypeEnum> Right { get; private set; }

        #region Equality
        public override bool Equals(object obj)
            => (obj is DataTypeValuePair<TDataTypeEnum> other)
                ? Equals(other)
                : false;
        public bool Equals(DataTypeValuePair<TDataTypeEnum> other) => this == other;
        public override int GetHashCode() => (Left.GetHashCode() ^ 1500450271) + Right.GetHashCode();

        public static bool operator ==(DataTypeValuePair<TDataTypeEnum> a, DataTypeValuePair<TDataTypeEnum> b)
            => a.Left.DataType.Equals(b.Left.DataType) && a.Right.DataType.Equals(b.Right.DataType) && 
                a.Left.Equals(b.Left) && a.Right.Equals(b.Right);

        public static bool operator !=(DataTypeValuePair<TDataTypeEnum> a, DataTypeValuePair<TDataTypeEnum> b) => !(a == b);
        #endregion Equality


        //public OperandTypePair TypesFromMap(BinaryOperandTypeMap map)
        //{
        //    var key = new OperandTypePair(Left.DataType, Right.DataType);
        //    return map.ContainsKey(key)
        //        ? map[key]
        //        : OperandTypePair.Unsupported;
        //}

        //public IDataType CastFromMap(BinaryOperandTypeMap map, EvaluatorSettings settings)
        //{
        //    var newPairType = TypesFromMap(map);
        //    if (!newPairType.OperationSupported)
        //        return null;
        //    return new IDataType(Left.CastTo(newPairType.Left), Right.CastTo(newPairType.Right));
        //}
    }
}
