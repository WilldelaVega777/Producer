using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ProducerAlpha
{
    [Serializable]
    public class Credits : INotifyPropertyChanged
    {
        //-------------------------------------------------------------------------------------------------
        // Private Fields Section
        //-------------------------------------------------------------------------------------------------
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Producer>     producers;
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Director>     directors;
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Designer>     designers;
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Speaker>      speakers;
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Composer>     composers;
        private System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Developer>    developers;

        //-------------------------------------------------------------------------------------------------
        // Public Properties Section
        //------------------------------------------------------------------------------------------------- 
        public System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Producer> Producers
        {
            get
            {
                return this.producers;
            }
            set
            {
                this.producers = value;
                this.RaisePropertyChanged("Producers");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        public System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Director> Directors
        {
            get
            {
                return this.directors;
            }
            set
            {
                this.directors = value;
                this.RaisePropertyChanged("Directors");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        public System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Designer> Designers
        {
            get
            {
                return this.designers;
            }
            set
            {
                this.designers = value;
                this.RaisePropertyChanged("Designers");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        public System.Collections.ObjectModel.ObservableCollection<ProducerAlpha.Speaker> Speakers
        {
            get
            {
                return this.speakers;
            }
            set
            {
                this.speakers = value;
                this.RaisePropertyChanged("Speakers");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        public System.Collections.ObjectModel.ObservableCollection<Composer> Composers
        {
            get
            {
                return this.composers;
            }
            set
            {
                this.composers = value;
                this.RaisePropertyChanged("Composers");
            }
        }

        //-------------------------------------------------------------------------------------------------        
        public System.Collections.ObjectModel.ObservableCollection<Developer> Developers
        {
            get
            {
                return this.developers;
            }
            set
            {
                this.developers = value;
                this.RaisePropertyChanged("Developers");
            }
        }

        //-------------------------------------------------------------------------------------------------
        // Constructor Method Section
        //-------------------------------------------------------------------------------------------------
        public Credits()
        {
            this.producers = new System.Collections.ObjectModel.ObservableCollection<Producer>();
            this.directors = new System.Collections.ObjectModel.ObservableCollection<Director>();
            this.designers = new System.Collections.ObjectModel.ObservableCollection<Designer>();
            this.speakers = new System.Collections.ObjectModel.ObservableCollection<Speaker>();
            this.composers = new System.Collections.ObjectModel.ObservableCollection<Composer>();
            this.developers = new System.Collections.ObjectModel.ObservableCollection<Developer>();
        }


        //----------------------------------------------------------------------------------------------------------
        // INotifyPropertyChanged Implementation Section
        //----------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        //-------------------------------------------------------------------------------------------------
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                // Events Fire correctly
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
