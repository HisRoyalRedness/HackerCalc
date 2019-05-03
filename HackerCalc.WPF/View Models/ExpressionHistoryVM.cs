using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class ExpressionHistoryVM : ViewModelBase
    {
        public void Add(ExpressionVM expressionVM) => Expressions.Insert(0, expressionVM.Model);

        public ObservableCollection<ExpressionM> Expressions { get; } = new ObservableCollection<ExpressionM>();

        public void Clear() => Expressions.Clear();
        public RelayCommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(_ => Clear(), _ => Expressions.Count > 0);
                return _clearCommand;
            }
        }
        RelayCommand _clearCommand;
    }
}
