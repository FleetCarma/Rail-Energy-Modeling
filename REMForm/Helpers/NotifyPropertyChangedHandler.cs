using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace REMForm.Helpers
{
    /// <summary>
    /// Helper class inherited by almost all UI models.
    /// This is used in databinding to notify the system that a property has been updated,
    /// and that all UI elements bound to the property need to be updated
    /// </summary>
    public abstract class NotifyPropertyChangedHandler : INotifyPropertyChanged
    {
        public void ForceUpdate()
        {
            NotifyPropertyChanged(string.Empty);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract void NotifyAllPropertiesChanged();
        #endregion
    }
}
