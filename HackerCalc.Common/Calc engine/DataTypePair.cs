using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    [DebuggerDisplay("{Left}, {Right}")]
    public struct DataTypeValuePair
    {
        public DataTypeValuePair(IDataType left, IDataType right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
#if INCOMPLETE_EQ
#else
            Right = right ?? throw new ArgumentNullException(nameof(right));
#endif
        }

        public IDataType Left { get; private set; }
        public IDataType Right { get; private set; }


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
    public struct DataTypePair : IEquatable<DataTypePair>
    {
        public DataTypePair(DataType left, DataType right)
            : this(left, right, true)
        { }

        public DataTypePair(DataType left, DataType right, bool supported)
        {
            Left = left;
            Right = right;
            OperationSupported = supported;
        }

        public DataType Left { get; private set; }
        public DataType Right { get; private set; }
        public bool OperationSupported { get; private set; }

        public static DataTypePair Unsupported { get; } = new DataTypePair() { OperationSupported = false };

        #region Equality
        public override bool Equals(object obj)
        {
            var other = obj as DataTypePair?;
            return other == null
                ? false
                : Equals(other.Value);
        }
        public bool Equals(DataTypePair other) => this == other;
        public override int GetHashCode() => (((int)Left + 1) * (int)DataType.MAX + (int)Right).GetHashCode();

        public static bool operator ==(DataTypePair a, DataTypePair b)
            => a.Left == b.Left && a.Right == b.Right;
        public static bool operator !=(DataTypePair a, DataTypePair b) => !(a == b);
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
