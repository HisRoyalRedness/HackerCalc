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
        public AppVM()
        {
            //Expression = "1+2+3+4+5u8";
        }

        #region Key entry
        public void AddChar(char chr)
        {
            if (_alphabet.Contains(chr))
                Expression.Input += chr;
        }

        public void Clear() => Expression.Input = "";
        public void Back()
        {
            if (Expression.Input.Length > 0)
                Expression.Input = Expression.Input.Substring(0, Expression.Input.Length - 1);
        }

        public void Enter()
        {
            if (Expression.IsValid)
            {
                ExpressionHistory.Add(Expression.ToString());
                Clear();
            }
        }
        #endregion Key entry

        #region Bindable properties

        #endregion Bindable properties

        public string Errors
        {
            get => _errors;
            private set { SetProperty(ref _errors, value); }
        }
        string _errors = default(string);

        public IConfiguration Configuration { get; } = new Configuration();

        //void Evaluate(string expression)
        //{
        //    try
        //    {
        //        var token = Parser.ParseExpression(expression, Configuration);
        //        Evaluation = (IDataType<DataType>)_evaluator.Evaluate(token, Configuration);
        //        Debug.WriteLine(Evaluation);
        //        Errors = Evaluation == null ? "Error" : string.Empty;
        //    }
        //    catch (ApplicationException aex)
        //    {
        //        Errors = aex.Message;
        //    }
        //}

        public ExpressionVM Expression { get; set; } = new ExpressionVM();
        public ObservableCollection<string> ExpressionHistory { get; } = new ObservableCollection<string>();

        Evaluator<DataType> _evaluator = new Evaluator<DataType>(CalcEngine.Instance);

        // The list of allowable characters
        static readonly HashSet<char> _alphabet =
            ("0123456789" +                 // numerics
             "abcdefx" +                    // hex digits, plus the leading 'x'
             "+-*/\\%!~<>&|^()_,'." +       // operators
             "abcdefghijklmnopqrstuvwxyz" + // alpha lower
             "ABCDEFGHIJKLMNOPQRSTUVWXYZ")  // alpha upper
            .ToHashSet();
    }

    

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
}
