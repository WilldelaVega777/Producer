using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows;

namespace ProducerAlpha
{
    [Serializable]
    public class Observable : INotifyPropertyChanged
    {
        private bool isDirty;        

        [Browsable(false)]
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            set
            {
                this.isDirty = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        protected void Set<T>(ref T field, T value, string propertyName)
        {
            if (!Object.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            // Marks Object as Dirty
            this.isDirty = true;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
