using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ActiveWindow.ViewModels
{
    public class Project : INotifyPropertyChanged

    {
        private Work _Work;

        public Work Work
        {
            get { return _Work; }
            set { _Work = value; OnPropertyChanged("Work"); }
        }
        private TimeSpan _Interval;

        public TimeSpan Interval
        {
            get { return _Interval; }
            set { _Interval = value; OnPropertyChanged("Interval"); }
        }
        
        public Project(TimeSpan timeSpan, Work w)
        {
            // TODO: Complete member initialization
            this.Interval = timeSpan;
            this.Work = w;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
