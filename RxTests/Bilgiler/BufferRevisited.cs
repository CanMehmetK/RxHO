using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxTests.Bilgiler
{
    public class BufferRevisited
    {
        /*
                Buffer is not a new operator to us; however, it can now be conceptually grouped with the window operators. Each of these
            windowing operators act on a sequence and a window of time. Each operator will open a window when the source sequence produces
            a value. The way the window is closed, and which values are exposed, are the main differences between each of the operators.
            Let us just quickly recap the internal working of the Buffer operator and see how this maps to the concept of "windows of time".

                Buffer will create a window when the first value is produced. It will then put that value into an internal cache. The window
            will stay open until the count of values has been reached; each of these values will have been cached. When the count has been
            reached, the window will close and the cache will be published to the result sequence as an IList<T>. When the next value is 
            produced from the source, the cache is cleared and we start again. This means that Buffer will take an IObservable<T> and return
            an IObservable<IList<T>>.
            
        source|-0-1-2-3-4-5-6-7-8-9|

        result|-----0-----3-----6-9|             
                    1     4     7             
                    2     5     8

                Understanding buffer with time is only a small step away from understanding buffer with count; instead of passing a count, we
            pass a TimeSpan. The closing of the window(and therefore the buffer's cache) is now dictated by time instead of the number of values.
            This is now more complicated as we have introduced some sort of scheduling. To produce the IList<T> at the correct point in time,
            we need a scheduler assigned to perform the timing. Incidentally, this makes testing a lot easier.

        source|-0-1-2-3-4-5-6-7-8-9-|

        result|----0----2----5----7-|                               
                   1    3    6    8                                         
                        4         9


         */
    }
}
