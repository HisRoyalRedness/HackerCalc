using System;
using System.Diagnostics;

/*
    DataType pairs for binary operations (i.e. left and right)

    Keith Fletcher
    Apr 2019

    This file is Unlicensed.
    See the foot of the file, or refer to <http://unlicense.org>
*/

namespace HisRoyalRedness.com
{
    /// <summary>
    /// Represents an operand pair (for binary operations), that contain
    /// the operand types only
    /// </summary>
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

    /// <summary>
    /// Represents an operand pair (for binary operations), that contain
    /// the operand type and value
    /// </summary>
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

        public DataTypePair<TDataTypeEnum> ToDataTypePair()
            => new DataTypePair<TDataTypeEnum>(Left.DataType, Right.DataType);

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
