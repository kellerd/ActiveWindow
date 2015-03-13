using System;
using System.Reactive;
using System.Reactive.Linq;
using ActiveWindowLib;
using AutoHotkey.Interop;

public static class AutoHotKeyObservable
{
    private static readonly Lazy<AutoHotkeyEngine> Ahk = new Lazy<AutoHotkeyEngine>(() =>
    {
        var engine = new AutoHotkeyEngine();
        engine.Load("Window.ahk");
        return engine;
    }, true);

    public static AutoHotkeyEngine AutoHotKey { get { return Ahk.Value;  } }

    public static IObservable<IOperation> GetAllAhkVars(this AutoHotkeyEngine ahk)
    {
        var xy = ahk.ExecFunction("GetMyMouse");
        var split = xy.Split('|');

        IOperation p = new ProcessInfo
        {
            WindowTitle = ahk.ExecFunction("GetMyWindow"),
            ProcessId = ahk.ExecFunction("GetPid"),
            ProcessName = ahk.ExecFunction("GetProcessName"),
            ActiveWindowId = ahk.ExecFunction("GetTransparent")
        };

        IOperation m = new MouseInfo { X = Int32.Parse(split[0]), Y = Int32.Parse(split[1]) };

        return Observable.Return(p).
        Concat(
        Observable.Return(m)
        );
    }

    public static IObservable<IOperation> WatchAwt(TimeSpan time)
    {
        var awtWatch = Observable.Interval(time).
            SelectMany(_ => AutoHotKey.GetAllAhkVars());
        return awtWatch;
    }

    public static IObservable<MouseInfo.MouseStatus> MouseMoving(this IObservable<IOperation> awtWatch)
    {
        var mouse = awtWatch.OfType<MouseInfo>();
        var mouseDelta = mouse.Zip(mouse.Skip(1), (prev, curr) => new MouseInfo
        {
            X = curr.X - prev.X,
            Y = curr.Y - prev.Y
        });
        return mouseDelta.Select(p => p.X != 0 && p.Y != 0 ? MouseInfo.MouseStatus.Moving : MouseInfo.MouseStatus.NotMoving);
    }

    public static IObservable<PersonInfo.ActiveStatus> PersonActive(this IObservable<MouseInfo.MouseStatus> mouseMoving)
    {
        var mouseSub =
            mouseMoving.StartWith(MouseInfo.MouseStatus.NotMoving)
                .TimeInterval()
                .Scan(
                    (prev, curr) =>
                        new TimeInterval<MouseInfo.MouseStatus>(curr.Value,
                            curr.Value == prev.Value ? prev.Interval + curr.Interval : curr.Interval));
        var active =
            mouseSub.Select(
                m => m.Value == MouseInfo.MouseStatus.Moving || m.Interval < TimeSpan.FromMinutes(5) ? PersonInfo.ActiveStatus.Active : PersonInfo.ActiveStatus.Inactive)
                .StartWith(PersonInfo.ActiveStatus.Active)
                .DistinctUntilChanged();
        return active;
    }
}