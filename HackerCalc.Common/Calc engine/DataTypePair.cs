using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{Left}, {Right}")]
    public struct DataTypePair<TDataTypeEnum> : IEquatable<DataTypePair<TDataTypeEnum>>, IComparable<DataTypePair<TDataTypeEnum>>, IComparable
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

        #region IComparable implementation
        public int CompareTo(DataTypePair<TDataTypeEnum> other)
        {
            if (other == null)
                return 1;
            var leftCmp = Left.CompareTo(other.Left);
            return leftCmp == 0
                ? Right.CompareTo(other.Right)
                : leftCmp;
        }

        public int CompareTo(object obj) => CompareTo(obj as IComparable<DataTypePair<TDataTypeEnum>>);
        #endregion IComparable implementation
    }

    [DebuggerDisplay("{Left}, {Right}")]
    public struct DataTypeValuePair<TDataTypeEnum> : IEquatable<DataTypeValuePair<TDataTypeEnum>>, IComparable<DataTypeValuePair<TDataTypeEnum>>, IComparable
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

        #region IComparable implementation
        public int CompareTo(DataTypeValuePair<TDataTypeEnum> other)
        {
            if (other == null)
                return 1;
            var leftTypeCmp = Left.DataType.CompareTo(other.Left.DataType);
            if (leftTypeCmp != 0)
                return leftTypeCmp;
            var rightTypeCmp = Right.DataType.CompareTo(other.Right.DataType);
            if (rightTypeCmp != 0)
                return rightTypeCmp;
            var leftValueCmp = Left.CompareTo(other.Left);
            if (leftValueCmp != 0)
                return leftValueCmp;
            return Right.CompareTo(other.Right);
        }

        public int CompareTo(object obj) => CompareTo(obj as IComparable<DataTypePair<TDataTypeEnum>>);
        #endregion IComparable implementation
    }
}
