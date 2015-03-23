using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveWindowLib
{
   public static class ReactiveExtensions
    {

        public static IObservable<TimeInterval<T>> HowLongHas<T>(this IObservable<TimeInterval<T>> source, T startWithValue = null, TimeSpan? time = null) where T : class
        {

            return startWithValue != null ? source.HowLongHas(new TimeInterval<T>(startWithValue, time ?? TimeSpan.Zero)) : source;
        }
        public static IObservable<TimeInterval<T>> HowLongHas<T>(this IObservable<TimeInterval<T>> source, TimeInterval<T>? startWith = null)
        {
            var howLong = source.Zip(source.Skip(1), (prev, curr) => new TimeInterval<T>(prev.Value, curr.Interval));
            return startWith.HasValue ? howLong.StartWith(startWith.Value) : howLong;
        }

    }
}
