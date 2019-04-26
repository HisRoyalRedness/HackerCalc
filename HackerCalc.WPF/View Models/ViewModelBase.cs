using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public abstract class ViewModelBase : ViewModelBase<object>
    {

    }

    public abstract class ViewModelBase<TModel> : NotifyBase, IDataErrorInfo
    {
        protected ViewModelBase()
            : this(default(TModel))
        { }

        protected ViewModelBase(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }

        #region IDataErrorInfo implementation
        public string this[string columnName] => ValidateProperty(columnName) ?? String.Empty;

        protected virtual string ValidateProperty(string propertyName) => null;

        public string Error => throw new NotImplementedException();
        #endregion IDataErrorInfo implementation
    }
}
