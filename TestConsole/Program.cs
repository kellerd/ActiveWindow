using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using ActiveWindowLib;
using Microsoft.Win32;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var awtWatch = AutoHotKeyObservable.WatchAwt(TimeSpan.FromMilliseconds(2000));
            var windows = awtWatch.OfType<ProcessInfo>().Select(p => p.ToString()).DistinctUntilChanged();
            var personActive = awtWatch.MouseMoving().PersonActive();

            var all = windows.CombineLatest(personActive,
                                                (w, a) => a == PersonInfo.ActiveStatus.Active ? w : a.ToString()).
                              CombineLatest(SessionObservable.SessionSwitched(),
                                                (w, s) => s == SessionSwitchReason.SessionUnlock ? w : s.ToString()).
                              TimeInterval().HowLongHas("Active");
            using (all.Dump("Windows"))
            {
                Console.ReadKey();
            }
        }

        
    }

}
public static class SampleExtentions
{
   

    public static IDisposable Dump<T>(this IObservable<T> source, string name) 
    {
        return source.Subscribe(
            i => Console.WriteLine("{0}-->{1}", name, (i == null ? String.Empty : i.ToString()).LeftAndRight(60)),
        ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
        () => Console.WriteLine("{0} completed", name));
    }
    public static string LeftAndRight(this string source, int length )
    {
        if (source.Length <= length)
        {
            return source;
        }
        return source.Left(length / 2) + "..." + source.Right(length / 2);
    }

    public static string Right(this string original, int numberCharacters)
    {
        return original.Substring(original.Length - numberCharacters);
    }

    /// <summary>
    /// Returns the source sequence prefixed with the specified value.
    /// </summary>
    /// <typeparam name="TSource">Source sequence element type.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="values">Values to prefix the sequence with.</param>
    /// <returns>Sequence starting with the specified prefix value, followed by the source sequence.</returns>
    public static IEnumerable<TSource> StartWith<TSource>(this IEnumerable<TSource> source, params TSource[] values)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        return source.StartWith_(values);
    }
    private static IEnumerable<TSource> StartWith_<TSource>(this IEnumerable<TSource> source, params TSource[] values)
    {
        foreach (var x in values)
            yield return x;
        foreach (var item in source)
            yield return item;
    }

}
public static class Utils
{
    public static string Left(this string str, int length)
    {
        return str.Substring(0, Math.Min(length, str.Length));
    }
}
