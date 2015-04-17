using ActiveWindowLib;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive;
using System.ComponentModel;
using System.Windows.Input;
using System.Reactive.Subjects;
using ActiveWindow.Models;


namespace ActiveWindow.ViewModels
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
        public ObservableCollection<Rule> Rules
        {
            get { return _Rules; }
            private set
            {
                if (_Rules != value)
                {
                    _Rules = value;
                    OnPropertyChanged("Rules");
                }
            }
        }
        private IConnectableObservable<Timestamped<TimeInterval<string>>> _PublicEvents;
        public IConnectableObservable<Timestamped<TimeInterval<string>>> PublicEvents
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
        private IDisposable PublicEventConnection { get; set; }

        public MainViewModel()
        {
            CreateRule = new RelayCommand<Project>(param =>
            {
                Questions.Clear();
                PublicEventConnection.Dispose();
                Rules.Add(new Rule()
                {
                    What = param.Work.What,
                    Enabled = true,
                    Comparison = RuleType.Equals,
                    From = new TimeSpan(0, 0, 0),
                    To = new TimeSpan(23, 59, 59),
                    Do = Rule.Ignore
                });
                PublicEventConnection = PublicEvents.Connect();
            });
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
                              }).Publish();


            WhatWereYouDoing = PublicEvents.Where(evt =>
            {
               return !Rules.
                    Where(r => r.Enabled).
                    Where(r => evt.Timestamp.TimeOfDay >= r.From).
                    Where(r => evt.Timestamp.TimeOfDay <= r.To).
                    Where(r => r.Comparison == RuleType.Equals).
                    Where(r => r.What == evt.Value.Value).Any();
            }).ObserveOnDispatcher().Subscribe((i) =>
            {
                Work w = new Work() { What = i.Value.Value, When = i.Timestamp.ToLocalTime() };
                if (Questions.ContainsKey(w))
                    Questions[w].Interval += i.Value.Interval;
                else
                    Questions.Add(w, new Project(i.Value.Interval, w));
            });
            PublicEventConnection = PublicEvents.Connect();
        }

        private ObservableDictionary<Work, Project> _Questions;
        private IDisposable _WhatWereYouDoing;

        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Rule> _Rules;
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
