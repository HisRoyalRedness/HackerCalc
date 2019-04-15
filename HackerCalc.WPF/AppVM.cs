using System;
using System.Collections.Generic;
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
            Expression = "1+2+3+4+5u8";
        }
        //public OtherBasesVM OtherBases { get; } = new OtherBasesVM();

        public void Keydown(KeyEventArgs keArgs)
        {
            Debug.WriteLine(keArgs.Key);
        }

        public string Expression
        {
            get => _expression;
            set
            {
                if (SetProperty(ref _expression, value))
                    Evaluate(_expression);
            }
        }
        string _expression = default(string);

        public IDataType<DataType> Evaluation
        {
            get => _evaluationToken;
            private set { SetProperty(ref _evaluationToken, value); }
        }
        IDataType<DataType> _evaluationToken = null;

        public IDataType<DataType> CalcDisplay
        {
            get => _calcDisplay;
            private set { SetProperty(ref _calcDisplay, value); }
        }
        IDataType<DataType> _calcDisplay = new DisplayType();

        public string Errors
        {
            get => _errors;
            private set { SetProperty(ref _errors, value); }
        }
        string _errors = default(string);

        public IConfiguration Configuration { get; } = new Configuration();

        void Evaluate(string expression)
        {
            try
            {
                var token = Parser.ParseExpression(expression, Configuration);
                Evaluation = (IDataType<DataType>)_evaluator.Evaluate(token, Configuration);
                Debug.WriteLine(Evaluation);
                Errors = Evaluation == null ? "Error" : string.Empty;
            }
            catch (ApplicationException aex)
            {
                Errors = aex.Message;
            }
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
