using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace RxTests.SampleExtensions
{
    public class InjectingSchedulerDependencies
    {
    }
    // Testing için arkadaşlar önemli kavramlar.
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }

        IScheduler Dispatcher { get; }

        IScheduler Immediate { get; }

        IScheduler NewThread { get; }

        IScheduler ThreadPool { get; }

        //IScheduler TaskPool { get; }  // Bunu ekleyip eklememek bana kalmış, targetladığım platforma göre. 
    }

    //public sealed class SchedulerProvider : IScheduler
    //{
    //    public IScheduler CurrentThread
    //    {
    //        get { return Scheduler.CurrentThread; }
    //    }
    //    public IScheduler Dispatcher
    //    {

    //        get { return Dispatcher.Instance; }
    //    }

    //    public IScheduler Immediate
    //    {
    //        get { return Scheduler.Immediate; }
    //    }
    //    public IScheduler NewThread
    //    {
    //        get { return Scheduler.NewThread; }
    //    }
    //    public IScheduler ThreadPool
    //    {
    //        get { return Scheduler.ThreadPool; }
    //    }

    //    //public IScheduler TaskPool { get { return Scheduler.TaskPool; } }
    //}

    public sealed class TestSchedulers /*: ISchedulerProvider*/
    {
        //    private readonly TestScheduler _currentThread = new TestScheduler(‌);
        //    private readonly TestScheduler _dispatcher = new TestScheduler(‌);
        //    private readonly TestScheduler _immediate = new TestScheduler(‌); private readonly TestScheduler _newThread = new TestScheduler(‌); private readonly TestScheduler _threadPool = new TestScheduler(‌);
        //    #region Explicit implementation of ISchedulerService 
        //    IScheduler ISchedulerProvider‌.CurrentThread { get { return _currentThread; } }
        //    IScheduler ISchedulerProvider‌.Dispatcher { get { return _dispatcher; } }
        //    IScheduler ISchedulerProvider‌.Immediate { get { return _immediate; } }
        //    IScheduler ISchedulerProvider‌.NewThread { get { return _newThread; } }
        //    IScheduler ISchedulerProvider‌.ThreadPool { get { return _threadPool; } }
        //    #endregion
        //    public TestScheduler CurrentThread { get { return _currentThread; } }
        //    public TestScheduler Dispatcher { get { return _dispatcher; } }
        //    public TestScheduler Immediate { get { return _immediate; } }

        //    public TestScheduler NewThread { get { return _newThread; } }
        //    public TestScheduler ThreadPool { get { return _threadPool; } }

        //    //ISchedulerProvider explicit olarak implement edildi. 
        //    //Yani Testlerde TestScheduler instancelarına direkt olarak ulaşabiliriz ama test(SUT)'de olan sistemimiz sadece infterface implemantationı görür.    
        //}
    }
}