using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(params string[] properties)
        {
            foreach (var property in properties)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected TValue GetProperty<TValue>(ref TValue propertyMember) => propertyMember;

        protected bool SetProperty<TValue>(ref TValue propertyMember, TValue newValue, Action<TValue> actionIfChanged = null, [CallerMemberName]string propertyName = "")
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            if (HasChanged(ref propertyMember, newValue))
            {
                propertyMember = newValue;
                NotifyPropertyChanged(propertyName);
                actionIfChanged?.Invoke(newValue);
                return true;
            }

            return false;
        }

        bool HasChanged<TValue>(ref TValue propertyMember, TValue newValue)
            => !(((propertyMember == null && newValue == null) ||
                (propertyMember != null && propertyMember.Equals(newValue))));
        #endregion INotifyPropertyChanged implementation
    }

}
