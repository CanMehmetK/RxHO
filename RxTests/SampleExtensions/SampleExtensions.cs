using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxTests.SampleExtensions
{
    public static class SampleExtentions // Console kadar kafana taş düşsün
    {
        public static void Dump<T>(this IObservable<T> source, string name)
        {
            source.Subscribe(
               i => Console.WriteLine("{0}-->{1}", name, i),
               ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
               () => Console.WriteLine("{0} completed", name));
        }


    }
}
