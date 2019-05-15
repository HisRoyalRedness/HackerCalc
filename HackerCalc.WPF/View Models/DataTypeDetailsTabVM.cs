using System;
using System.Collections.Generic;
using System.Linq;
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

    public class NullTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Null";

        public override bool CanSelect(IDataType<DataType> dataType) => false;
    }

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

    public class LimitedIntegerDetailsTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Programmer";

        public override bool CanSelect(IDataType<DataType> dataType) => dataType?.DataType == DataType.LimitedInteger;

        public string Bitwidth => FormatLimitedInteger(lit =>
            lit.SignAndBitWidth.BitWidth == IntegerBitWidth.Unlimited
                ? "Unlimited"
                : $"{lit.SignAndBitWidth.BitWidth.GetEnumDescription()} bits, {((int)lit.SignAndBitWidth.BitWidth)/8} bytes");
        
        

        protected override void EvaluationChanged(IDataType<DataType> evaluation)
            => NotifyPropertyChanged(nameof(Bitwidth));

        string FormatLimitedInteger(Func<LimitedIntegerType, string> formatter)
            => Evaluation is LimitedIntegerType lit && lit != null ? formatter(lit) : string.Empty;
    }

    public class DateTimeDetailsTabVM : DataTypeDetailsTabBaseVM
    {
        public override string Header => "Date/Time";

        public override bool CanSelect(IDataType<DataType> dataType) => dataType?.DataType == DataType.Date || dataType?.DataType == DataType.Time || dataType?.DataType == DataType.Timespan;
    }
}
