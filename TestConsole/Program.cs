using ActiveWindowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using Microsoft.Win32;
using TransparencyMenu;
using System.Diagnostics;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var hooks = new GlobalHooks(Process.GetCurrentProcess().MainWindowHandle);
            try
            {
                ;
                var WindowActivated = Observable.FromEventPattern<TransparencyMenu.GlobalHooks.WindowEventHandler, WindowEventArgs>(h => {
                    hooks.Shell.WindowActivated += h;
                    hooks.Shell.Start();
                }, h => hooks.Shell.WindowActivated -= h).Finally(
                () => { 
                    hooks.Shell.Stop(); 
                }
                );

            var SessionSwitched = Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(h => Microsoft.Win32.SystemEvents.SessionSwitch += h, h => Microsoft.Win32.SystemEvents.SessionSwitch -= h);
            //var WindowActivated = Observable.FromEventPattern<MsdnMag.GlobalCbtHook.CbtEventHandler, CbtEventArgs>(h => cbtHook.WindowActivated += h, h => cbtHook.WindowActivated -= h);


            var obs = Observable.Interval(TimeSpan.FromMilliseconds(200)).
                Select(_ => NativeMethods.GetCurrentProcessInfo().WindowTitle).
                Timestamp();
            // var sub = obs
            using (var d = WindowActivated.Dump("WindowTitle: "))
            {
                Console.ReadLine();
            }
            }
            finally
            {
                hooks.Shell.Stop();
            }
        }
    }

}
public static class SampleExtentions
{
    public static IDisposable Dump<T>(this IObservable<T> source, string name)
    {
        return source.Subscribe(
        i => Console.WriteLine("{0}-->{1} onthread: {2}", name, i, Thread.CurrentThread.ManagedThreadId),
        ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
        () => Console.WriteLine("{0} completed", name));
    }
}