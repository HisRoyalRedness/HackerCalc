using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class AppVM : NotifyBase
    {
        public string Equation
        {
            get => _equation;
            set
            {
                if (SetProperty(ref _equation, value))
                    Evaluate(_equation);
            }
        }
        string _equation = default(string);

        public IToken Evaluation
        {
            get => _evaluationToken;
            private set { SetProperty(ref _evaluationToken, value); }
        }
        IToken _evaluationToken = null;

        public string HexValue
        {
            get => _hexValue;
            private set { SetProperty(ref _hexValue, value); }
        }
        string _hexValue = "";

        public string DecValue
        {
            get => _decValue;
            private set { SetProperty(ref _decValue, value); }
        }
        string _decValue = "";

        public string OctValue
        {
            get => _octValue;
            private set { SetProperty(ref _octValue, value); }
        }
        string _octValue = "";

        public string Errors
        {
            get => _errors;
            private set { SetProperty(ref _errors, value); }
        }
        string _errors = default(string);


        void Evaluate(string expression)
        {
            try
            {
                Evaluation = Parser.ParseExpression(expression)?.Evaluate();
                Errors = Evaluation == null ? "Error" : string.Empty;
                switch (Evaluation?.GetType()?.Name)
                {
                    case nameof(LimitedIntegerToken):
                        SetBaseNumbers(((LimitedIntegerToken)Evaluation).TypedValue);
                        break;

                    case nameof(UnlimitedIntegerToken):
                        SetBaseNumbers(((UnlimitedIntegerToken)Evaluation).TypedValue);
                        break;

                    default:
                        SetBaseNumbers(null);
                        break;
                }
            }
            catch (ApplicationException aex)
            {
                Errors = aex.Message;
            }
        }

        void SetBaseNumbers(BigInteger? value)
        {
            if (value == null)
            {
                DecValue = "";
            }
            else
            {
                DecValue = value.Value.ToString();
            }
        }
    }
}
