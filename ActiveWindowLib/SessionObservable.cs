using System;
using System.Reactive.Linq;
using Microsoft.Win32;

namespace ActiveWindowLib
{
    public class SessionObservable
    {
        public static IObservable<SessionSwitchReason> SessionSwitched()
        {
            var sessionSwitchedWatch =
                Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(
                    h => SystemEvents.SessionSwitch += h, h => SystemEvents.SessionSwitch -= h);
            var sessionSwitchedSub =
                sessionSwitchedWatch.Select(f => f.EventArgs.Reason)
                    .StartWith(SessionSwitchReason.SessionUnlock);
            return sessionSwitchedSub;
        }
    }
}