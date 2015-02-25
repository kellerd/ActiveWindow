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
        static void Main(string[] args)
        {
            var ahkWindow = new AutoHotkey.Interop.AutoHotkeyEngine();
            ahkWindow.Load("Window.ahk");
            ahkWindow.ExecLabel("Init");
            //execute a specific function (found in functions.ahk), with 2 parameters
            //ahk.ExecFunction("MyFunction", "Hello", "World");

            ////execute a label 
            ////execute a function (in functions.ahk) that adds 5 and return results
            //var add5Results = ahk.Eval("Add5( 5 )");
            //Console.WriteLine("Eval: Result of 5 with Add5 func is {0}", add5Results);

            ////you can also return results with the ExecFunction 
            //add5Results = ahk.ExecFunction("Add5", "5");
            //Console.WriteLine("ExecFunction: Result of 5 with Add5 func is {0}", add5Results);

            try
            {
                //var windowTitleWatch = Observable.Interval(TimeSpan.FromMilliseconds(2000)).
                //Select(_ =>
                //{
                //    return ahkWindow.GetVar("awt");
                //});



                //var mouseWatch =  
                //     

                var awtWatch = Observable.Using(() =>
                {
                    var ahk = new DisposableAutoHotKey();
                    ahk.Load("Window.ahk");
                    ahk.ExecLabel("Init");
                    return ahk;
                }, localahk =>
                {
                    var windowTitleWatch = Observable.Interval(TimeSpan.FromMilliseconds(2000)).
                    Select(_ => localahk.GetVar("awt"));
                    //{
                    //    return String.Format("Window Title {0} on thread: {1}", localahk.GetVar("awt"),Thread.CurrentThread.ManagedThreadId) ;
                    //}
                    //);
                    var mouseWatch = Observable.Interval(TimeSpan.FromMilliseconds(2000)).
                      Select(_ => localahk.GetVar("xpos") + "|" + localahk.GetVar("ypos"));
                      //  {
                      //      return String.Format("Mouse {0} on thread: {1}", localahk.GetVar("xpos") + "|" + localahk.GetVar("ypos"),Thread.CurrentThread.ManagedThreadId);
                      //  }
                      //);
                    var sessionSwitchedWatch = Observable.FromEventPattern<SessionSwitchEventHandler, SessionSwitchEventArgs>(h => Microsoft.Win32.SystemEvents.SessionSwitch += h, h => Microsoft.Win32.SystemEvents.SessionSwitch -= h).
                      Select((f) => f.EventArgs.Reason.ToString()).StartWith(SessionSwitchReason.SessionUnlock.ToString());
                          //String.Format("Session Switch {0} on thread: {1}", f.EventArgs.Reason.ToString(),Thread.CurrentThread.ManagedThreadId));
                    return mouseWatch.DistinctUntilChanged().Merge(sessionSwitchedWatch.DistinctUntilChanged()).Merge(windowTitleWatch.DistinctUntilChanged());
                }
               );


                using (awtWatch.Dump("Main"))
                {
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //var WindowActivated = Observable.FromEventPattern<MsdnMag.GlobalCbtHook.CbtEventHandler, CbtEventArgs>(h => cbtHook.WindowActivated += h, h => cbtHook.WindowActivated -= h);

            Console.ReadLine();
            //var obs = Observable.Interval(TimeSpan.FromMilliseconds(200)).
            //    Select(_ => NativeMethods.GetCurrentProcessInfo().WindowTitle).
            //    Timestamp();

        }
    }

}
public static class SampleExtentions
{
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
