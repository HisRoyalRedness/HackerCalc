using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class ExpressionVM : ViewModelBase
    {
        public ExpressionVM()
            : this(new Configuration())
        {
            Input = IsInDesigner ? "1+(2-3*4/5)//2" : "";
        }

        public ExpressionVM(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Bindable properties
        public string Input
        {
            get => _input;
            set { SetProperty(ref _input, value, _ => Evaluate()); }
        }
        string _input = default(string);

        public bool IsValid
        {
            get => _isValid;
            private set { SetProperty(ref _isValid, value); }
        }
        bool _isValid = false;

        public string ParseError
        {
            get => _parseError;
            private set { SetProperty(ref _parseError, value); }
        }
        string _parseError = string.Empty;

        public IToken ParsedExpression
        {
            get => _parsedExpression;
            private set { SetProperty(ref _parsedExpression, value); }
        }
        IToken _parsedExpression = null;

        public IDataType<DataType> Evaluation
        {
            get => _evaluation;
            private set { SetProperty(ref _evaluation, value); }
        }
        IDataType<DataType> _evaluation = null;
        #endregion Bindable properties

        public IConfiguration Configuration { get; }

        void Evaluate()
        {
            ParseError = string.Empty;
            Evaluation = null;
            try
            {
                var errors = new string[] { };
                ParsedExpression = Parser.ParseExpression(Input, Configuration, ref errors);
                Evaluation = (IDataType<DataType>)_evaluator.Evaluate(ParsedExpression, Configuration);
                ParseError = string.Join("\r\n", errors);
            }
            catch (Exception ex)
            {
                ParsedExpression = null;
                ParseError = ex.Message;
            }

            IsValid = Evaluation != null;
        }

        protected override string ValidateProperty(string propertyName)
        {
            switch(propertyName)
            {
                case nameof(Input):
                    return ParseError;
            }
            return null;
        }

        public override string ToString()
        {
            return IsValid
                ? $"{Input}={Evaluation}"
                : Input;
        }

        Evaluator<DataType> _evaluator = new Evaluator<DataType>(CalcEngine.Instance);
    }
}
