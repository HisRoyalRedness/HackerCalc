using System;
using System.Collections.Generic;
using System.Linq;
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
            set { SetProperty(ref _evaluationToken, value); }
        }
        IToken _evaluationToken = null;

        public string Errors
        {
            get => _errors;
            set { SetProperty(ref _errors, value); }
        }
        string _errors = default(string);


        void Evaluate(string expression)
        {
            try
            {
                Evaluation = Parser.ParseExpression(expression)?.Evaluate();
                Errors = Evaluation == null ? "Error" : string.Empty;
            }
            catch (ApplicationException aex)
            {
                Errors = aex.Message;
            }
        }
    }
}
