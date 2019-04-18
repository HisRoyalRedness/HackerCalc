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
                Expression.AddChar(chr);
        }

        public void Clear() => Expression.Clear();
        public void Back() => Expression.Back();


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
        //public string Expression
        //{
        //    get => _expression;
        //    set
        //    {
        //        if (SetProperty(ref _expression, value))
        //            Evaluate(_expression);
        //    }
        //}
        //string _expression = default(string);

        //public IDataType<DataType> Evaluation
        //{
        //    get => _evaluationToken;
        //    private set { SetProperty(ref _evaluationToken, value); }
        //}
        //IDataType<DataType> _evaluationToken = null;

        //public IDataType<DataType> CalcDisplay
        //{
        //    get => _calcDisplay;
        //    private set { SetProperty(ref _calcDisplay, value); }
        //}
        //IDataType<DataType> _calcDisplay = new DisplayType();
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

    public class ExpressionVM : NotifyBase
    {
        public ExpressionVM()
            : this(new Configuration())
        {
            Expression = "1+(2-3*4/5)";
            Evaluate();
        }

        public ExpressionVM(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string Expression { get; private set; } = "";
        public bool IsValid { get; private set; } = false;
        public IConfiguration Configuration { get; }
        public string Error { get; private set; } = null;
        public IDataType<DataType> Evaluation { get; private set; }
        public IToken ParsedExpression { get; private set; }

        public void AddChar(char chr)
        {
            Expression += chr;
            Evaluate();
        }

        public void Clear()
        {
            Expression = "";
            Evaluate();
        }

        public void Back()
        {
            if (Expression.Length > 0)
                Expression = Expression.Substring(0, Expression.Length - 1);
            Evaluate();
        }

        void Evaluate()
        {
            Evaluation = null;
            try
            {
                ParsedExpression = Parser.ParseExpression(Expression, Configuration);
                Evaluation = (IDataType<DataType>)_evaluator.Evaluate(ParsedExpression, Configuration);
            }
            catch(Exception ex)
            {
                Error = ex.Message;
            }

            IsValid = Evaluation != null;
            NotifyPropertyChanged(nameof(Error), nameof(IsValid), nameof(Evaluation), nameof(Expression), nameof(ParsedExpression));
        }

        public override string ToString()
        {
            return IsValid
                ? $"{Expression}={Evaluation}"
                : Expression;
        }

        Evaluator<DataType> _evaluator = new Evaluator<DataType>(CalcEngine.Instance);
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
