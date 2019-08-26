using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;
using RxTests.Models;
using RxTests.SampleExtensions;

namespace RxTests.Controllers
{
    /*
     1- Problem
        Rx Nasıl daha şey olabilir
        Maintınıbilitisi daha güzel kodla birlikte uygulamamızı
        Rahatlamaya yönelik bir şey ya
        Bişeyler yapıyoruz işde...


     2- Çözüm
        
         */
    public class HomeController : Controller
    {
        private readonly IHubContext<RxHub> _rxHubContext;
        public HomeController(IHubContext<RxHub> rxHubContext)
        {
            _rxHubContext = rxHubContext;
        }

        public IActionResult Pong()
        {
            _rxHubContext.Clients.All.SendAsync("SendTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm:ss"));
            return Ok();
        }
        public async Task<List<il>> Dapper()
        {
            IDbConnection Connection = new MySqlConnection("server=10.225.4.23;userid=root;password=y123;database=cit_2018;charset=utf8;Allow User Variables=true");
            using (IDbConnection conn = Connection)
            {
                // select * from sayimtipi;

                conn.Open();
                {
                    var qResult = (await conn.QueryAsync<il>("SELECT * FROM cit_2018.il;")).ToList();
                    return qResult;
                }
            }
        }

        #region Data can be buffered, throttled, sampled or delay to meet your needs.
        public IActionResult Buffer()
        {
            //var idealBatchSize = 15;
            //var maxTimeDelay = TimeSpan.FromSeconds
            //    (3);
            //var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(0.01)).Take(100));
            //source.Buffer(maxTimeDelay, idealBatchSize).Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer.Count));

            // Buffer ile ilgili zımbırtılar

            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            //source.Buffer(3, 1).Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));


            // buffer methoduna verilen parametreler, kaç tane değer aldığını ve kaç tanesini skiplediğini gösteriyor. Eğer 3,3 yazsaydık sayılarda bir farklılık görmezdik. Ancak 3,5 gibi bir sayı yazdığımızda bazı değerlerin kaybolduğunu görüyoruz.

            var overlapped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));
            var standard = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            var skipped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)).Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer));


            return Ok();
        }
        public IActionResult Delay() // Bütün sequence zaman olarak yer değiştirebilir.
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Timestamp();

            var delay = source.Delay(TimeSpan.FromSeconds(2));
            source.Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer));

            delay.Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer));

            // Delay OnError bildirimlerini ertelemez.

            return Ok();
        }

        public IActionResult Sample()
        {
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(150));
            interval.Sample(TimeSpan.FromSeconds(1)).Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer));

            // timespandeki her spesifik alanın son değerini alır.This is great for getting timely data from a sequence that produces too much information for your requirements.

            return Ok();
        }

        public IActionResult Throttle() // Açılır kapanır bir pencere gibi, ne zaman bir değer getirirse pencere kapanır.Sample gibi bir zaman aralığındaki son değeri alır. Throttle method is only useful for sequences that produce values at a variable rate.
        {
            //pdf olarak bir tane örneği var.
            return Ok();
        }
        #endregion
        public IActionResult Timeout() // belirli bir zaman aralığında bir değer almazsak timeouta düşeriz., timeliness of data can be asserted.
        {
            //var source = Observable.Interval(TimeSpan.FromMilliseconds(100)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(2)));

            //var timeout = source.Timeout(TimeSpan.FromSeconds(1));
            //timeout.Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));

            // ---------------------

            var dueDate = DateTimeOffset.UtcNow.AddSeconds(4);
            var source = Observable.Interval(TimeSpan.FromSeconds(1));
            var timeout = source.Timeout(dueDate).Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));

            return Ok();

        }


        #region blah blah

        public IActionResult Subject()
        {
            return Ok();
        }

        public void veriIsle(object veri)
        {
            // Veri işle
        }
        #endregion


        public async Task<IActionResult> DurArtikBe()
        {
            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Select(i => new EventPattern<MyEventArgs>(this, new MyEventArgs(i)));
            var result = source.ToEventPattern();


            result.OnNext += (sender, EventArgs) =>

                _rxHubContext.Clients.All.SendAsync("SendTime", EventArgs.Value);

            //await _rxHubContext.Clients.All.SendAsync("SendTime", DateTime.Now.ToString("Strings from numbers:"));

            //source.Subscribe(num =>
            //{
            //    _rxHubContext.Clients.All.SendAsync("SendTime", num);

            //});
            return Ok();

        }


        #region 1'den fazla sequenceları birleştirip gelen dataları nasıl görmek istiyorsak zip, and, then, when
        public IActionResult ZipZip() // Aslında fermuar, 2 sequencetaki değerleri pair olarak bizlere gösterir, ilk sequence bittiğinde durur. 2sinden biri hata verirse en fresh değeri gösterir.
        {
            var nums = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(3);
            var chars = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(6).Select(i => Char.ConvertFromUtf32((int)i + 97));

            nums.Zip(chars, (lhs, rhs) => new { Left = lhs, Right = rhs }).Subscribe(num => { _rxHubContext.Clients.All.SendAsync("SendTime", num); });

            return Ok();




        }


        public IActionResult MuthisSeylerWithZipAndThenWhen()
        {

            var one = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);
            var two = Observable.Interval(TimeSpan.FromMilliseconds(250)).Take(10);
            var three = Observable.Interval(TimeSpan.FromMilliseconds(150)).Take(14);

            //var zippedSequence = one.Zip(two, (lhs, rhs) 
            //    => new { One = lhs, Two = rhs })
            //    .Zip(three, (lhs, rhs) 
            //    => new { One = lhs.One, Two = lhs.Two, Three = rhs });

            //zippedSequence.Subscribe(num => { _rxHubContext.Clients.All.SendAsync("SendTime", num); });





            //---------- BUNUN YERINE AŞAĞIDAKİ --------------//

            //var pattern = one.And(two).And(three);
            //var plan = pattern.Then((first, second, third) => new { One = first, Two = second, Three = third });

            //var zippedSequence = Observable.When(plan);
            //zippedSequence.Subscribe(num =>
            //{
            //    _rxHubContext.Clients.All.SendAsync("SendTime", num);
            //});

            // ------------ YAZIM DAHA DA AZALTILABİLİR ---------- //

            var zippedSequence = Observable.When(
                one.And(two)
                .And(three)
                .Then((birinci, ikinci, ucuncu) =>
                new
                {
                    Bir = birinci,
                    Iki = ikinci,
                    Uc = ucuncu
                })
                ).Subscribe(num =>
                {
                    _rxHubContext.Clients.All.SendAsync("SendTime", num);
                })
                ;



            return Ok();
        }
        #endregion


        #region hataya düşürdüm.
        public async Task<IActionResult> Deniyoruz() // Catch bütün hataları bulur ve YUTAR. "try{DoSomeWork();}catch{}" gibi. TimeoutException diye bir şey de var.
        {
            var source = new Subject<int>();
            var result = source.Catch(Observable.Empty<int>());
            source.OnNext(1);
            source.OnNext(2);
            source.OnError(new Exception("Fail!"));
            return Ok();
        }

        public IActionResult CatchDenemeleri() // gördüm.
        {
            var source = new Subject<int>();
            var result = source.Catch<int, TimeoutException>(tx => Observable.Return(-1));
            source.Subscribe(num =>
            {
                source.OnNext(1);
                source.OnNext(2);
            }
            );
            source.OnError(new TimeoutException());

            return Ok();
        }
        #endregion

        //public IActionResult UsingDenemeleri() // kendi methodlarımı eklemek gerekebilir.
        //{
        //    var source = Observable.Interval(TimeSpan.FromSeconds(1));
        //    var result = Observable.Using(
        //        () => new timeIt("Subscription Timer"), timeIt => source
        //        );
        //    result.Take(5);
        //    return Ok();
        //}



        public IActionResult ConcatDeniyorum() // 2 sequence birleştirme.
        {
            var s1 = Observable.Range(0, 3);
            var s2 = Observable.Range(5, 5);
            s1.Concat(s2).Subscribe(num =>
            {
                _rxHubContext.Clients.All.SendAsync("SendTime", num);

            });

            return Ok();

        }

        public IActionResult Wizard() // sadece observable instanceları değil Gerçek data değerlerini paylaşabilmek istiyorsak Publish(). Bu bize IConnectableObservable<T> dönüyor, bunun için ise Connect() kullanıp, bu sharing functionality'e ulaşabiliriz.
        {
            //var period = TimeSpan.FromSeconds(1);
            //var observable = Observable.Interval(period).Publish();
            //observable.Connect();
            //observable.Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));
            //Thread.Sleep(period);
            //observable.Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));

            // Sleep ile bile 2 subscription da yapılmadan gerçek anlamda subscribe olamıyoruz.

            //------------- Bu yazım aslında bir uygulama data sequence'ları paylaşma gereksinimi duyduğunda gayet kullanışlı.

            var period = TimeSpan.FromSeconds(1);
            var observable = Observable.Interval(period).Publish();
            observable.Subscribe(i => _rxHubContext.Clients.All.SendAsync("SendTime", i));
            Thread.Sleep(period);
            observable.Subscribe(i => _rxHubContext.Clients.All.SendAsync("SendTime", i));

            observable.Connect();

            return Ok();
        }
        #region Bu 2 arkadaş UI application yazarken çok yararlıymış, çünkü; UI threadini blocklamak istemeyiz ama UI thread'indeki UI objectlerini updatelememiz lazım.
        //
        public IActionResult SubscribeOn() // we should use this to describe how we want any warm-up and background processing code to be scheduled. Bütün actionlar aynı thread içinde oluyor, ve her şey sequential. Subscription oldugunda Create çağırılmış oluyor,Create bitene kadar synchronous kalıyor.
                                           // //‌.SubscribeOn(‌Scheduler‌.ThreadPool)'ı eklediğimizde executionın sırası değişiyor. Bunu ekledikten sonra subscribe artık non-blocking.


        {
            return Ok();
        }

        public IActionResult ObserveOn()
        //is used to declare where you want your notifications to be scheduled to. most useful when working with STA systems, most commonly UI apps
        {
            return Ok();
        }
        #endregion

        public IActionResult BuNe()
        {
            // Here we pass myName as the state. We also pass a delegate that will take the state and return a disposable. The disposable is used for cancellation.
            // The delegate also takes an IScheduler parameter, which we name "_".This is the convention to indicate we are ignoring the argument. 
            // When we pass myName as the state, a reference to the state is kept internally. So when we update the myName variable to "John", the reference to "Lee" is still maintaine by the scheduler's internal workings.
            // ÇIKMAZLARI ÖNLEMEK İÇİN SCHEDULE METHODLARINI KULLANABİLİRİZ
            // With any concurrent software, you should avoid modifying shared state.
            // IScheduler type'i, adından da belli olduğu gibi bir actionı ileri bir tarihte execute edebileceğimiz anlamına gelir

            //var myName = "Lee";
            //Scheduler.Schedule(myName,
            //    (_,state) =>
            //    {
            //        _rxHubContext.Clients.All.SendAsync("SendTime", state);
            //        return Disposable.Empty;
            //    }

            //    )
            //    ;
            //myName = "John";



            //Schedule'ı nasıl kullanacağımı anlayamadım. 

            //var delay = TimeSpan.FromSeconds(1);

            //_rxHubContext.Clients.All.SendAsync("Before school", DateTime.Now);

            //Scheduler.Schedule(delay,
            //    ()=> _rxHubContext.Clients.All.SendAsync("Inside school", DateTime.Now));

            //_rxHubContext.Clients.All.SendAsync("After school", DateTime.Now);

            /*  !!!!!!!!!----------!!!!!!!
             *      Diyelim ki;çalışmakta olan bir işi iptal etmek istiyorum, ve IDisposable'dan dispose yapmam gerekiyor, ama işi hala yapıyorsam disposable'a nasıl geri döneceğim?
             *  Başka bir thread açıp iş concurrent olarak çalışabilir ama thread yaratmaktan kaçıyoruz zaten.    
            */

            // Rx takes our recursive method and transforms it to a lopp structure instead. Brilliant!

            return Ok();
        }

        public IActionResult EventLoopScheduler()
        // Allows us to designate a specific thread to a scheduler.CurrentThreadScheduler nasıl içiçe scheduled actionlar için trambolin görevi görüyorsa,
        // bu arkadaş da aynı mekanizmayı sağlar. Arasındaki fark ise EventLoopScheduler'da schedule etmek için bizim istediğimiz thread kullanılır. EventLoopScheduler can be created with an empty constructor, or you can oass it a thread factory delegate.
        // diyelim ki thread adını koyduk, prioritysini ve cultureını belirledik ve en önemlisi bu thread bir background threadi mi değil mi onu belirledik.
        // unutmayalım eğer thread's propertysi olan IsBackground'u  false yapmazsak, thread terminate olana kadar application wont terminate.
        // EventLoopScheduler IDisposable implement edip Dispose çağırdığı için, threadi terminate etmemize izin verir.
        {
            return Ok();
        }

        public IActionResult NewThreadScheduler()
            //threadin veya EventLoopScheduler'ın resourcelari ile uğraşmak istemiyorsak;
            //Kendi NewThreadScheduler instanceımızı yaratabilir veya Scheduler.NewThread propertysinin statik instanceına erişip kullanabiliriz.
            //contstructor, Kendi factorymizi sağlıyorsak, IsBackground'ı uygun bir şekilde set etmeliyiz.
            //Eğer Schedule çağırıyorsak, aslında yapmış olduğumuz iş EventLoopScheduler yaratmak.
            //Bu yol ile herhangi içiçe olan schedule'lar aynı thread içinde olacak. Subsequent'ler(non-nested) ise Schedule'ı yeni EventLoopScheduler çağırmak ve thread factory function'ı yeni bir thread çağırmak için kullanır.
            
        {
            return Ok();
        }

        public IActionResult ThreadPoolScheduler()
            //bu arkadaş basitçe ThreadPool'a tunnel request oluyor. 
            //For requests that are scheduled ASAP, the action is just sent to ThreadPool.QueueUserWorkItem. 
            //Daha sonrası için schedulelanan requestler için ise, System.Threading.Timer
            //Bir önceki Schedulelar gibi bundaki nested'lar seriler şeklinde gelmeyebilir.
        {
            //_rxHubContext.Clients.All.SendAsync("SendTime", Thread.CurrentThread.ManagedThreadId);
            //Scheduler.ThreadPool.Schedule("A", OuterAction);
            //Scheduler.ThreadPool.Schedule("B", OuterAction);

            return Ok();
        }

        public IActionResult TaskPool()
        {
            return Ok();
        }

        public IActionResult Files()
        {
            var source = new FileStream(@"C:\Somefile.txt", FileMode.Open, FileAccess.Read);

            var factory = Observable.FromAsyncPattern<byte[], int, int, int>(source.BeginRead, source.EndRead);
            var buffer = new byte[source.Length];
            IObservable<int> reader = factory(buffer, 0, (int)source.Length);
            reader.Subscribe(bytesRead => _rxHubContext.Clients.All.SendAsync("SendTime", bytesRead));
            return Ok();

            



            /* Her şey OK ama if we want to read CHUNKS OF DATA at a time, this is not good. Buffer size'ını spesifik bir değer olarak belirlememiz gerekli, mesela 4KB(4096 bytes)
        İşe yarar ama yalnızca 4kb'lık bir alanı okur,As the position of the FileStream will have advanced to the point it stopped reading, we can reuse the factory
        to reload the buffer.
        Next, we want to start pushing these bytes into an observable sequence.
             
             */
        }

        public IActionResult ObservableInterval()
        {

            var oneNumberPerSecond = Observable.Interval(TimeSpan.FromSeconds(1)); // Bir Saniye aralıklarla tetiklenen bir subcriber (Üye/Abone/Bağlanma noktası) üret

            var stringsFromNumbers = from n in oneNumberPerSecond
                                     select new string('*', (int)n);  // Her bir tetikleme için artan sayı kadar tetiklemenin sonucunda yızdız basar.
            
            _rxHubContext.Clients.All.SendAsync("SendTime", DateTime.Now.ToString("Strings from numbers:")); //

            stringsFromNumbers.Subscribe(num =>
            {

                _rxHubContext.Clients.All.SendAsync("SendTime", num);

            });




            //using (stringsFromNumbers.Subscribe(num => { _rxHubContext.Clients.All.SendAsync("SendTime", num); }))
            //{
            //    _rxHubContext.Clients.All.SendAsync("SendTime", "içerde");
            //    while (1 == 1) { }
            //}


            //    using (stringsFromNumbers.Subscribe(num =>
            //     {

            //         _rxHubContext.Clients.All.SendAsync("SendTime", num);


            //     });
            //}

            return Ok();
        }

        //public static IObserver<int> ObserveDates(int hebele)
        //{
        //    Observable.Create<int>(observer =>
        //   {
        //       observer.OnNext(1);
        //       observer.OnNext(2);
        //       observer.OnNext(3);
        //       observer.OnCompleted();

        //       return Disposable.Empty;
        //   });
        //    return "";
        //}



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
