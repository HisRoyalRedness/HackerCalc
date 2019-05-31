using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HisRoyalRedness.com
{
    /// <summary>
    /// Interaction logic for GroupedDigitControl.xaml
    /// </summary>
    public partial class GroupedDigitControl : UserControl
    {
        public GroupedDigitControl()
        {
            InitializeComponent();
        }

        public string Number
        {
            get { return (string)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
            nameof(Number), 
            typeof(string), 
            typeof(GroupedDigitControl),
            new PropertyMetadata("", new PropertyChangedCallback((o, e) => CreateGroups(o, (string)e.NewValue))));

        public uint GroupSize
        {
            get { return (uint)GetValue(GroupSizeProperty); }
            set { SetValue(GroupSizeProperty, value); }
        }

        public static readonly DependencyProperty GroupSizeProperty = DependencyProperty.Register(
            nameof(GroupSize),
            typeof(uint),
            typeof(GroupedDigitControl),
            new PropertyMetadata((uint)0, new PropertyChangedCallback((o, e) => CreateGroups(o, (uint)e.NewValue))));

        public string Sign
        {
            get { return (string)GetValue(SignProperty); }
            private set { SetValue(SignProperty, value); }
        }

        public static readonly DependencyProperty SignProperty = DependencyProperty.Register(
            nameof(Sign), 
            typeof(string), 
            typeof(GroupedDigitControl), 
            new PropertyMetadata(""));

        static void CreateGroups(DependencyObject o, uint groupSize)
        {
            if (o is GroupedDigitControl gdc)
                gdc.CreateGroups(gdc.Number, groupSize);
        }

        static void CreateGroups(DependencyObject o, string number)
        {
            if (o is GroupedDigitControl gdc)
                gdc.CreateGroups(number, gdc.GroupSize);
        }

        void CreateGroups(string number, uint groupSize)
        {
            Groups.Clear();

            var num = number?.Trim() ?? string.Empty;
            Sign = num.StartsWith("-") ? "-" : string.Empty;
            num = num.TrimStart(new[] { '-', ' ' });

            var leadingChar = num.Length > 0 ? num[0] : '\0';

            if (groupSize <= 1)
            {
                Groups.Add(new NumberGroup(num, leadingChar));
                return;
            }

            var pad = (int)(num.Length % GroupSize);
            if (pad > 0)
                num = num.PadLeft(num.Length + ((int)GroupSize - pad), '0');

            foreach (var grp in num.Batch((int)GroupSize))
            {
                var numGroup = new NumberGroup(new string(grp.ToArray()), leadingChar);
                if (leadingChar != '\0' && !numGroup.AllLeadingsChars)
                    leadingChar = '\0';
                Groups.Add(numGroup);
            }
        }

        public ObservableCollection<NumberGroup> Groups { get; } = new ObservableCollection<NumberGroup>();

        #region NumberGroup
        public class NumberGroup : ViewModelBase
        {
            public NumberGroup(string value)
                : this(value, '\0')
            { }

            public NumberGroup(string value, char leadingChar)
            {
                Value = value;
                AllLeadingsChars = leadingChar != '\0' && Value.All(c => c == leadingChar);
                var lead = leadingChar == '\0' ? "" : new string(Value.TakeWhile(c => c == leadingChar).ToArray());

                if (leadingChar == '\0' || lead.Length == 0)
                {
                    LeadingValue = string.Empty;
                    TrailingValue = Value;
                    return;
                }

                LeadingValue = lead;

                if (lead.Length < Value.Length)
                    TrailingValue = Value.Substring(lead.Length);
            }

            public string LeadingValue { get; }
            public string TrailingValue { get; }

            internal bool AllLeadingsChars { get; }

            public string Value { get; }
        }
        #endregion NumberGroup
    }
}
