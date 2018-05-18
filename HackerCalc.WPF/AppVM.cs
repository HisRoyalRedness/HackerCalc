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
        public OtherBasesVM OtherBases { get; } = new OtherBasesVM();

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

        public ILiteralToken Evaluation
        {
            get => _evaluationToken;
            private set { SetProperty(ref _evaluationToken, value); }
        }
        ILiteralToken _evaluationToken = null;

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
                OtherBases.Token = Evaluation;
            }
            catch (ApplicationException aex)
            {
                Errors = aex.Message;
            }
        }
    }
}
