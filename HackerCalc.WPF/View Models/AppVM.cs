using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HisRoyalRedness.com
{
    public class AppVM : NotifyBase
    {
        #region Singleton
        public static AppVM Instance => _singleton.Value;
        private AppVM()
        {
            Expression = new ExpressionVM(Configuration.Model);

            if (IsInDesigner)
            {
                //Expression.Input = "1+(2-3*4/5)**6//7";
                //Expression.Input = "1+(2-3*4/5)";
                Expression.Input = "-0x5f695i128";
                ExpressionHistory.Add(new ExpressionVM(Configuration.Model) { Input = "1+2f/3" });
            }
            DataTypeDetails.Evaluation = Expression.Evaluation;
            Expression.PropertyChanged += Expression_PropertyChanged;
        }

        void Expression_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExpressionVM.Evaluation))
                DataTypeDetails.Evaluation = Expression.Evaluation;
        }

        static Lazy<AppVM> _singleton = new Lazy<AppVM>(() => new AppVM());
        #endregion Singleton

        #region Bindable properties
        public ExpressionVM Expression
        {
            get => _expression;
            private set
            {
                var oldExpr = _expression;
                if (SetProperty(ref _expression, value))
                {
                    if (DataTypeDetails != null)
                        DataTypeDetails.Evaluation = value?.Evaluation;

                    if (oldExpr != null)
                        oldExpr.PropertyChanged -= Expression_PropertyChanged;
                    if (value != null)
                        value.PropertyChanged += Expression_PropertyChanged;
                }
            }
        }
        ExpressionVM _expression;
        #endregion Bindable properties

        #region Key entry
        public void AddChar(char chr)
        {
            if (_alphabet.Contains(chr))
                Expression.Input += chr;
        }

        public void Clear() => Expression = new ExpressionVM(Configuration.Model);
        public void Back()
        {
            if (Expression.Input.Length > 0)
                Expression.Input = Expression.Input.Substring(0, Expression.Input.Length - 1);
        }

        public void Enter()
        {
            if (Expression.IsValid)
            {
                ExpressionHistory.Add(Expression);
                Clear();
            }
        }

        // The list of allowable characters
        static readonly HashSet<char> _alphabet =
            ("0123456789" +                 // numerics
             "abcdefx" +                    // hex digits, plus the leading 'x'
             "+-*/\\%!~<>&|^()_,'." +       // operators
             "abcdefghijklmnopqrstuvwxyz" + // alpha lower
             "ABCDEFGHIJKLMNOPQRSTUVWXYZ")  // alpha upper
            .ToHashSet();
        #endregion Key entry

        public void SetExpression(string input)
        {
            Clear();
            Expression.Input = input;
        }

        public ConfigurationVM Configuration { get; } = new ConfigurationVM();
        public ExpressionHistoryVM ExpressionHistory { get; } = new ExpressionHistoryVM();

        public DataTypeDetailsVM DataTypeDetails { get; } = new DataTypeDetailsVM();
    }

    #region DisplayType
    public class DisplayType : IDataType<DataType>
    {
        public DataType DataType => DataType.Unknown;
        public string DisplayValue { get; set; } = "0";

        public object ObjectValue => DisplayValue;

        public IDataType<DataType> CastTo(DataType dataType, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDataType other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDataType<DataType> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDataType other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDataType<DataType> other)
        {
            throw new NotImplementedException();
        }

        public string ToString(Verbosity verbosity) => DisplayValue;

        TNewType IDataType<DataType>.CastTo<TNewType>(IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
    #endregion DisplayType
}
