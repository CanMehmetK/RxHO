using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxTests.Bilgiler
{
    public class Window
    {
        /*
                    The Window operators are very similat to the Buffer operators; they only really differ by their return type.
                Where Buffer would take an IObservable<T> and return an IObservable<IList<T>>, the Window operators return an
                IObservable<IObservable<T>>. It is also worth noting that the Buffer operators will not yield their buffers
                until the window closes.

        This is an example of Window with count of 3 as a marble diagram:
                
                source |-0-1-2-3-4-5-6-7-8-9| 
                window0|-0-1-2| 
                window1        3-4-5| 
                window2              6-7-8| 
                window3                    9|

       
                source |-0-1-2-3-4-5-6-7-8-9| 
                window0|-0-1-| 
                window1      2-3-4| 
                window2           -5-6-| 
                window3                7-8-9|


                The major difference we see here is that the Window operators can notify you of values from the source as soon as they
            are produced.
                The Buffer operators, on the other hand, must wait until the window closes before the values can be notified as an
            entire list.


         
         */





    }


}
