using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RxTests.Bilgiler
{
    public class Join
    {
        /*
                    The Join operator allows you to logically join two sequences. Whereas the Zip operator would
                pair values from the two sequences together by index, the Join operator allows you join sequences
                by intersecting windows. Like the Window overload we just looked at, you can specify when a window
                should close via an observable sequence; this sequence is returned from a function that takes an
                opening value. The Join operator has two such functions, one for the first source sequence and
                one for the second source sequence. Like the Zip operator, we also need to provide a selector function
                to produce the result item from the pair of values.

            IObservable<TLeft> left is the source sequence that defines when a window starts. This is just like the Buffer and Window
        operators, except that every value published from this source opens a new window. In Buffer and Window, by contrast,
        some values just fell into an existing window.

            I like to think of IObservable<TRight> right as the window value sequence. While the left sequence controls opening
        the windows, the right sequence will try to pair up with a value from the left sequence.

            Let us imagine that our left sequence produces a value, which creates a new window. If the right sequence produces a value
        while the window is open, then the resultSelector function is called with the two values. This is the crux of join,
        pairing two values from a sequence that occur within the same window. This then leads us to our next question;
        when does the window close? The answer illustrates both the power and the complexity of the Join operator.

            When left produces a value, a window is opened. That value is also passed, at that time, to the leftDurationSelector
        function, which returns an IObservable<TLeftDuration>. When that sequence produces a value or completes, the window
        for that value is closed. Note that it is irrelevant what the type of TLeftDuration is. This initially left me with 
        the felling that IObservable<TLeftDuration> was a bit excessive as you effectively just need some sort of event to 
        say 'Closed'. However, by being allowed to use IObservable<T>, you can do some clever manipulation as we will see later.

            Let us now imagine a scenario where the left sequence produces values twice as fast as the right sequence.
        Imagine that in addition we never close the windows; we could do this by always returning Observable.Never<Unit>()
        from the leftDurationSelector function. This would result in the following pairs being produce.

        Left sequence

        L 0-1-2-3-4-5-

        R --A---B---C- 
        0, A 
        1, A 
        0, B 
        1, B 
        2, B 
        3, B 
        0, C 
        1, C 
        2, C 
        3, C 
        4, C 
        5, C

            As you can see, the left values are cached and replayed each time the right produces a value.

            Now it seems fairly obvious that, if I immediately closed the window by returning Observable.Empty<Unit>, or perhaps
        Observable.Return(o), windows would never be opened thus no pairs would ever get produced. However, what could I do to make
        sure that these windows did not overlap- so that, once a second value was produced I would no longer see the first value?
        Well, if we returned the left sequence from the leftDurationSelector, that could do the trick. But wait, when we return the
        sequence left from the leftDurationSelector, it would try to create another subscription and that may introduce side effects.
        The quick answer to that is to Publish and RefCount the left sequence. If we do that, the results look more like this.

        left  |-0-1-2-3-4-5| 
        right |---A---B---C| 
        result|---1---3---5           
                  A   B   C

            The last example is very similar to CombineLatest, except that it is only producing a pait when the right sequence changes.
        We could use Join to produce our own version of CombineLatest. If the values from the left sequence expire when the next value
        from left was notified, then I would be well on my way to implementing my version of CombineLatest. However I need the same thing
        to happen for the right. Luckily the Join operator provides a rightDurationSelector that works just like the leftDurationSelector.
        This is simple to implement; all I need to do is return a reference to the same left sequence when a left value is produced, and
        do the same for the right.

        
         
         
         
         */
    }
    //public static IObservable<TResult> MyCombineLatest<TLeft,TRight, TResult>
    //    (
    //    IObservable<TLeft> left, 
    //    IObservable<TRight> right, 
    //    Func<TLeft,TRight,TResult> resultSelector
    //    )
    //{
    //    var refcountedLeft = left.Publish().RefCount();
    //    var refcountedRight = right.Publish().RefCount();

    //    return Observable.Join(
    //        refcountedLeft,
    //        refcountedRight,
    //        value => refcountedLeft,
    //        value => refcountedRight,
    //        resultSelector);
    //}

    /*
            While the code above is not production quality(it would need to have some gates in place to mitigate race conditions), it shows how powerful Join is,
        we can actually use it to create other operators!
     */




    /*
                GroupJoin
            When the Join operator pairs up values that coincide within a window, it will pass the scalar values left and right to the
        resultSelector. The GroupJoin operator takes this one step further by passing the left(scalar) value immediately to the resultSelector
        with the right(sequence) value. The right parameter represents all of the values from the right sequences that occur within the window.
        Its signature is very similar to Join, but note the difference int he resultSelector parameter.


     
     
     
     
     
     
     
     
     */

    //public static IObservable<TResult> GroupJoin<TLeft,TRight,TLeftDuration,TRightDuration,TResult>(
    //    this IObservable<TLeft> left,
    //    IObservable<TRight> right,
    //    Func<TLeft, IObservable<TLeftDuration>> leftDurationSelector,
    //    Func<TRight, IObservable<TRightDuration>> rightDurationSelector,
    //    Func<TLeft, IObservable<TRight>, TResult resultSelector>
    //    )
    //{ }

    /*
                If we went back to our first Join example where we had;
            ~the left producing values twice as fast as the right,
            ~the left never expiring,
            ~the right immediately expiring

            this is what the result may look like

            left              |-0-1-2-3-4-5| 
            right             |---A---B---C| 
            0th window values   --A---B---C| 
            1st window values     A---B---C| 
            2nd window values       --B---C| 
            3rd window values         B---C| 
            4th window values           --C| 
            5th window values             C|

    We could switch it around an have the left expired immediately and the right never expire. The result would then look like this:

            left              |-0-1-2-3-4-5| 
            right             |---A---B---C| 
            0th window values   | 
            1st window values     A|
            2nd window values       A| 
            3rd window values         AB| 
            4th window values           AB| 
            5th window values             ABC|

                This starts to make things interesting. Perceptive readers may have noticed that with GroupJoin you could
            effectively re-create your own Join method by doing something like this:

    public IObservable<TResult> MyJoin<TLeft, TRight, TLeftDuration, TRightDuration, TResult>
    (‌ 
    IObservable<TLeft> left, 
    IObservable<TRight> right, 
    Func<TLeft, IObservable<TLeftDuration>> leftDurationSelector, 
    Func<TRight, IObservable<TRightDuration>> rightDurationSelector, 
    Func<TLeft, TRight, TResult> resultSelector
    ) 
    { return Observable‌.GroupJoin 
    (‌ 
    left, 
    right, 
    leftDurationSelector,
    rightDurationSelector, 
    (‌leftValue, rightValues)=> 
    rightValues‌.Select(‌rightValue=>resultSelector(
    ‌leftValue, rightValue)) 
    ) ‌
    .Merge(‌); 
    }

            You could even create a crude version of Window with this code:

            public IObservable<IObservable<T>> MyWindow<T>(‌ 
            IObservable<T> source,  
            TimeSpan windowPeriod) 
            { return Observable‌.Create<IObservable<T>>(‌o  
            =>
            { 
            var sharedSource = source ‌
            .Publish(‌) 
            ‌.RefCount(‌); 
            var intervals = Observable‌.Return(‌0L) ‌
            .Concat(‌Observable‌.Interval(‌windowPeriod)) ‌
            .TakeUntil(‌sharedSource‌.TakeLast(‌1)) ‌
            .Publish(‌) ‌
            .RefCount(‌);
            return intervals‌.GroupJoin(‌ 
            sharedSource, 
            _ => intervals,  
            _ => Observable‌.Empty<Unit>(‌),  
            (‌left, sourceValues) => sourceValues) ‌
            .Subscribe(‌o); 
            }); 
            }

                        GroupJoin and other window operators reduce the need for low-level plumbing of state an concurrency.
                    By exposing a high-level API, code that would be otherwise difficult to write, becomes a cinch to put
                    together. For example, those in the finance industry could use GroupJoin to easily produce real-time
                    Volume or TimeWeighted Average Prices(VWAP/TWAP).

               Rx delivers yet another way to query data in motion by allowing you to interrogate sequences of coincidence.
            This enables you to solve the intrinsically complex problem of managing state and concurrency while performing
            matching from multiple sources. Bu encapsulating these low level operations, you are able to leverage Rx to
            design your software in an expressive and testable fashion. Using the Rx operators as building blocks, your
            code effectively becomes a composition of many simple operators. This allows the complexity of the domain code
            to be the focus, not the otherwise incidental supporting code.







     */


}
