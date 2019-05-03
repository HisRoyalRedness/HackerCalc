using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    #region ExpressionVM
    public class ExpressionVM : ViewModelBase<ExpressionM>
    {
        public ExpressionVM(IConfiguration config)
            : base(new ExpressionM())
        {
            Configuration = config;
            Evaluate();
        }

        #region Bindable properties
        public string Input
        {
            get => Model.Input;
            set { SetProperty(() => Input, nv => Model.Input = nv, value, _ => Evaluate()); }
        }

        public bool IsValid
        {
            get => Model.IsValid;
            private set { SetProperty(() => IsValid, nv => Model.IsValid = nv, value); }
        }

        public string ParseError
        {
            get => Model.ParseError;
            private set { SetProperty(() => ParseError, nv => Model.ParseError = nv, value); }
        }

        public IToken ParsedExpression
        {
            get => Model.ParsedExpression;
            private set { SetProperty(() => ParsedExpression, nv => Model.ParsedExpression = nv, value); }
        }

        public IDataType<DataType> Evaluation
        {
            get => Model.Evaluation;
            private set { SetProperty(() => Evaluation, nv => Model.Evaluation = nv, value); }
        }

        public bool IsInitial
        {
            get => _isInitial;
            set { SetProperty(ref _isInitial, value); }
        }
        bool _isInitial = true;
        #endregion Bindable properties

        IConfiguration Configuration { get; }

        void Evaluate()
        {
            ParseError = string.Empty;
            if (string.IsNullOrWhiteSpace(Input))
            {
                ParsedExpression = null;
                Evaluation = new UnlimitedIntegerType(0);
                IsValid = false;
            }
            else
            {
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
    #endregion ExpressionVM
}
