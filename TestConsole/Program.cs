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
using System.Diagnostics;

namespace TestConsole
{
    class Program
    {
        static DisposableAutoHotKey ahk = new DisposableAutoHotKey();
        static void Main(string[] args)
        {
            ahk.Load("Window.ahk");
            ahk.ExecLabel("Init");

            var sessionSwitchedWatch = Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(h => Microsoft.Win32.SystemEvents.SessionSwitch += h, h => Microsoft.Win32.SystemEvents.SessionSwitch -= h);

            var awtWatch = Observable.Interval(TimeSpan.FromMilliseconds(2000)).
                SelectMany(_ => ahk.GetAllAHKVars());

            var mouse = awtWatch.OfType<MouseInfo>();
            var skipMouse = mouse.Skip(1);
            var mouseDelta = mouse.Zip(skipMouse, (prev, curr) => new MouseInfo
                                                        {
                                                            X = curr.X - prev.X,
                                                            Y = curr.Y - prev.Y
                                                        });


            var windows = awtWatch.OfType<ProcessInfo>();

            var mouseSub = mouseDelta.Where(p => p.X != 0 && p.Y != 0).Select(p => p.ToString()).DistinctUntilChanged().TimeInterval();
            var windowsSub = windows.Select(p => p.ToString()).DistinctUntilChanged().TimeInterval();
            var sessionSwitchedSub = sessionSwitchedWatch.Select((f) => f.EventArgs.Reason.ToString()).StartWith(SessionSwitchReason.SessionUnlock.ToString()).TimeInterval();



            using (mouseSub.Merge(windowsSub).Merge(sessionSwitchedSub).Dump("Main"))
            {
                Console.ReadLine();
            }

        }
    }

}
public static class SampleExtentions
{
    public static IObservable<IOperation> GetAllAHKVars(this AutoHotkey.Interop.AutoHotkeyEngine ahk)
    {
        var xy = ahk.ExecFunction("GetMyMouse", string.Empty, string.Empty);
        var split = xy.Split('|');
        return Observable.Return<IOperation>(new ProcessInfo() { WindowTitle = ahk.ExecFunction("GetMyWindow", string.Empty) }).Concat
        (Observable.Return<IOperation>(new MouseInfo { X = Int32.Parse(split[0]), Y = Int32.Parse(split[1]) })
            //(Observable.Return<IOperation>(new MouseInfo { X = Int32.Parse(ahk.GetVar("xpos")), Y = Int32.Parse(ahk.GetVar("ypos")) })
        );
    }

    public static IDisposable Dump<T>(this IObservable<T> source, string name)
    {
        return source.Subscribe(
        i => Console.WriteLine("{0}-->{1} onthread: {2}", name, i.ToString().Left(40), Thread.CurrentThread.ManagedThreadId),
        ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
        () => Console.WriteLine("{0} completed", name));
    }
}
public static class Utils
{
    public static string Left(this string str, int length)
    {
        return str.Substring(0, Math.Min(length, str.Length));
    }
}
public class DisposableAutoHotKey : AutoHotkey.Interop.AutoHotkeyEngine, IDisposable
{
    public void Dispose()
    {
    }
}
