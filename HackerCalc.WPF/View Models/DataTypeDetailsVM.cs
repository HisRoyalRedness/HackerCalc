using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class DataTypeDetailsVM : ViewModelBase<IDataType<DataType>>
    {
        public ObservableCollection<IDataTypeDetailsTabVM> Tabs { get; }
            = new ObservableCollection<IDataTypeDetailsTabVM>(new IDataTypeDetailsTabVM[]
            {
                new NullTabVM(),
                new RationalDetailsTabVM(),
                new LimitedIntegerDetailsTabVM(),
                new DateTimeDetailsTabVM()
            });

        public IDataType<DataType> Evaluation
        {
            get => _evaluation;
            set => SetProperty(ref _evaluation, value, eval => SetEvaluation(eval));
        }
        IDataType<DataType> _evaluation;

        public IDataTypeDetailsTabVM SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }
        IDataTypeDetailsTabVM _selectedTab = null;

        void SetEvaluation(IDataType<DataType> evaluation)
        {
            SelectedTab = Tabs.FirstOrDefault(t => t.CanSelect(evaluation));
            if (SelectedTab != null)
                SelectedTab.Evaluation = evaluation;
        }
    }
}
