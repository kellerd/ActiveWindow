using ActiveWindowLib;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using ActiveWindow.ViewModels;
using System.Reactive.Linq;
using System.Reactive;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;


namespace ActiveWindow
{
    public class MainViewModel : IDisposable, INotifyPropertyChanged
    {
        public ObservableDictionary<Work, Project> Questions
        {
            get { return _Questions; }
            private set
            {
                if (_Questions != value)
                {
                    _Questions = value;
                    OnPropertyChanged("Questions");
                }
            }
        }
        private IObservable<Timestamped<TimeInterval<string>>> _PublicEvents;
        public IObservable<Timestamped<TimeInterval<string>>> PublicEvents
        {
            get { return _PublicEvents; }
            private set
            {
                if (_PublicEvents != value)
                {
                    _PublicEvents = value;
                    OnPropertyChanged("PublicEvents");
                }
            }
        }
        //var itemAddedObservable = Observable
        //         .FromEventPattern<NotifyCollectionChangedEventArgs>(items, "CollectionChanged")
        //         .Select(change => change.EventArgs.NewItems)
        public MainViewModel()
        {
            CreateRule = new RelayCommand(param => MessageBox.Show(param.ToString()));
        }
        private IDisposable WhatWereYouDoing
        {
            get { return _WhatWereYouDoing; }
            set
            {
                if (_WhatWereYouDoing != value)
                {
                    _WhatWereYouDoing = value;
                    OnPropertyChanged("WhatWereYouDoing");
                }
            }
        }
        public void Dispose()
        {
            if (WhatWereYouDoing != null)
                WhatWereYouDoing.Dispose();
        }

        internal void Load()
        {
            Questions = new ObservableDictionary<Work, Project>();
            var awtWatch = AutoHotKeyObservable.WatchAwt(TimeSpan.FromMilliseconds(2000));
            var windows = awtWatch.OfType<ProcessInfo>().Select(p => p.ToString()).DistinctUntilChanged();
            var personActive = awtWatch.MouseMoving().PersonActive();

            PublicEvents = windows.CombineLatest(personActive,
                                                (w, a) => a == PersonInfo.ActiveStatus.Active ? w : a.ToString()).
                              CombineLatest(SessionObservable.SessionSwitched(),
                                                (w, s) => s == SessionSwitchReason.SessionUnlock ? w : s.ToString()).
                              TimeInterval().HowLongHas("Active").Timestamp().Select(i =>
                              {
                                  return new Timestamped<TimeInterval<string>>(i.Value, i.Timestamp.RoundDown());
                              });
            WhatWereYouDoing = PublicEvents.ObserveOnDispatcher().Subscribe((i) =>
            {
                Work w = new Work() { What = i.Value.Value, When = i.Timestamp.ToLocalTime() };
                if (Questions.ContainsKey(w))
                    Questions[w].Interval += i.Value.Interval;
                else
                    Questions.Add(w, new Project(i.Value.Interval, w));
            });
        }

        private ObservableDictionary<Work, Project> _Questions;
        private IDisposable _WhatWereYouDoing;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
        
        public ICommand CreateRule { get; private set; }
    }



    public static class DateExtensions
    {
        public static DateTimeOffset RoundDown(this DateTimeOffset dateTime)
        {
            return new DateTimeOffset(dateTime.Year, dateTime.Month,
                 dateTime.Day, dateTime.Hour, (dateTime.Minute / 15) * 15, 0, dateTime.Offset);
        }
    }
}
