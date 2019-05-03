using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public abstract class ViewModelBase : NotifyBase, IDataErrorInfo
    {
        protected ViewModelBase()
        { }

        #region IDataErrorInfo implementation
        public string this[string columnName] => ValidateProperty(columnName) ?? String.Empty;

        protected virtual string ValidateProperty(string propertyName) => null;

        public string Error => null; //throw new NotImplementedException();
        #endregion IDataErrorInfo implementation
    }

    public abstract class ViewModelBase<TModel> : ViewModelBase
    {
        protected ViewModelBase()
            : base()
        { }

        protected ViewModelBase(TModel model)
            : base()
        {
            Model = model;
        }

        public TModel Model { get; }
    }
}
