using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IDataTypeDetailsTabVM
    {
        string Header { get; }
        bool CanSelect(IDataType<DataType> dataType);
        IDataType<DataType> Evaluation { get; set; }
    }

    #region DataTypeDetailsTabBaseVM
    public abstract class DataTypeDetailsTabBaseVM : ViewModelBase, IDataTypeDetailsTabVM
    {
        public abstract string Header { get; }

        public abstract bool CanSelect(IDataType<DataType> dataType);
        public IDataType<DataType> Evaluation
        {
            get => _evaluation;
            set => SetProperty(ref _evaluation, value, eval => EvaluationChanged(eval));
        }
        IDataType<DataType> _evaluation;

        protected virtual void EvaluationChanged(IDataType<DataType> evaluation) { }
    }
    #endregion DataTypeDetailsTabBaseVM

    #region NullTabVM
    public class NullTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Null";

        public override bool CanSelect(IDataType<DataType> dataType) => false;
    }
    #endregion NullTabVM

    #region RationalDetailsTabVM
    public class RationalDetailsTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Numeric";

        public override bool CanSelect(IDataType<DataType> dataType) => dataType?.DataType == DataType.RationalNumber;

        public string Fraction => FormatRational(rat => rat.Denominator == 1
                ? $"{rat.Numerator}"
                : $"{rat.Numerator} / {rat.Denominator}");

        protected override void EvaluationChanged(IDataType<DataType> evaluation)
            => NotifyPropertyChanged(nameof(Fraction));

        string FormatRational(Func<RationalNumber, string> formatter)
            => Evaluation is RationalNumberType rat && rat != null ? formatter(rat.Value) : string.Empty;
    }
    #endregion RationalDetailsTabVM

    #region LimitedIntegerDetailsTabVM
    public class LimitedIntegerDetailsTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Programmer";

        public override bool CanSelect(IDataType<DataType> dataType) => dataType?.DataType == DataType.LimitedInteger;

        protected override void EvaluationChanged(IDataType<DataType> evaluation)
        { 
            if (Evaluation is LimitedIntegerType lit && lit != null)
            {
                var bits = (int)lit.SignAndBitWidth.BitWidth;
                Value = lit.Value;
                
                if (lit.SignAndBitWidth.BitWidth == IntegerBitWidth.Unlimited)
                {
                    BitwidthText = "Unlimited";

                    BinaryTextSigned = lit.Value.ToBinaryString(true);
                    DecimalTextSigned = lit.Value.ToDecimalString(true);
                    HexadecimalTextSigned = lit.Value.ToHexadecimalString(true);

                    BinaryTextUnsigned = string.Empty;
                    DecimalTextUnsigned = string.Empty;
                    HexadecimalTextUnsigned = string.Empty;
                }
                else
                {
                    BitwidthText = $"{bits} bits, {bits / 8} bytes";

                    BinaryTextSigned = lit.SignedValue.ToBinaryString(bits, true);
                    DecimalTextSigned = lit.SignedValue.ToDecimalString(true, true);
                    HexadecimalTextSigned = lit.SignedValue.ToHexadecimalString(bits / 4, true);

                    BinaryTextUnsigned = lit.UnsignedValue.ToBinaryString(bits, false);
                    DecimalTextUnsigned = lit.UnsignedValue.ToDecimalString(false, true);
                    HexadecimalTextUnsigned = lit.UnsignedValue.ToHexadecimalString(bits / 4);
                }
            }
            else
            {
                Value = BigInteger.Zero;
                BitwidthText = string.Empty;
                BinaryTextSigned = string.Empty;
                BinaryTextUnsigned = string.Empty;
                DecimalTextSigned = string.Empty;
                DecimalTextUnsigned = string.Empty;
                HexadecimalTextSigned = string.Empty;
                HexadecimalTextUnsigned = string.Empty;
            }
        }

        #region Bindings
        public BigInteger Value
        {
            get => _value;
            internal set => SetProperty(ref _value, value);
        }
        BigInteger _value = BigInteger.Zero;
        
        public string BinaryTextSigned
        {
            get => _binText;
            private set => SetProperty(ref _binText, value);
        }
        string _binText;

        public string DecimalTextSigned
        {
            get => _decText;
            private set => SetProperty(ref _decText, value);
        }
        string _decText;

        public string HexadecimalTextSigned
        {
            get => _hexText;
            private set => SetProperty(ref _hexText, value);
        }
        string _hexText;

        public string BinaryTextUnsigned
        {
            get => _binTextNeg;
            private set => SetProperty(ref _binTextNeg, value);
        }
        string _binTextNeg;

        public string DecimalTextUnsigned
        {
            get => _decTextNeg;
            private set => SetProperty(ref _decTextNeg, value);
        }
        string _decTextNeg;

        public string HexadecimalTextUnsigned
        {
            get => _hexTextNeg;
            private set => SetProperty(ref _hexTextNeg, value);
        }
        string _hexTextNeg;

        public string BitwidthText
        {
            get => _bitwidth;
            private set => SetProperty(ref _bitwidth, value);
        }
        string _bitwidth;
        #endregion Bindings
    }
    #endregion LimitedIntegerDetailsTabVM

    #region DateTimeDetailsTabVM
    public class DateTimeDetailsTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Date/Time";

        public override bool CanSelect(IDataType<DataType> dataType) => dataType?.DataType == DataType.Date || dataType?.DataType == DataType.Time || dataType?.DataType == DataType.Timespan;
    }
    #endregion DateTimeDetailsTabVM
}
