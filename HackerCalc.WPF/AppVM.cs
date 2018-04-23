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
                {
                    try
                    {
                        var token = Parser.ParseExpression(_equation);
                        var eval = token?.Evaluate()?.ToString() ?? "";
                        if (string.IsNullOrEmpty(eval))
                            Errors = "Error";
                        else
                        { 
                            Evaluation = eval;
                            Errors = "";
                        }
                    }
                    catch (ApplicationException aex)
                    {
                        Errors = aex.Message;
                    }
                }
            }
        }
        string _equation = default(string);

        public string Evaluation
        {
            get => _evaluation;
            set { SetProperty(ref _evaluation, value); }
        }
        string _evaluation = default(string);

        public string Errors
        {
            get => _errors;
            set { SetProperty(ref _errors, value); }
        }
        string _errors = default(string);


    }
}
