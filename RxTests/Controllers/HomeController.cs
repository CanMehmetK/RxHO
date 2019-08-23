using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
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

        public IActionResult Delay()
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

        public IActionResult Throttle()
        {
            return Ok();
        }

        #region Buffer,Subject bla bla bla
        public IActionResult Buffer()
        {
            //var idealBatchSize = 15;
            //var maxTimeDelay = TimeSpan.FromSeconds
            //    (3);
            //var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10).Concat(Observable.Interval(TimeSpan.FromSeconds(0.01)).Take(100));
            //source.Buffer(maxTimeDelay, idealBatchSize).Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime",buffer));

            // Buffer ile ilgili zımbırtılar

            var source = Observable.Interval(TimeSpan.FromSeconds(1)).Take(10);
            //source.Buffer(3, 1).Subscribe(num => _rxHubContext.Clients.All.SendAsync("SendTime", num));


            // buffer methoduna verilen parametreler, kaç tane değer aldığını ve kaç tanesini skiplediğini gösteriyor. Eğer 3,3 yazsaydık sayılarda bir farklılık görmezdik. Ancak 3,5 gibi bir sayı yazdığımızda bazı değerlerin kaybolduğunu görüyoruz.

            var overlapped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1));
            var standard = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            var skipped = source.Buffer(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)).Subscribe(buffer => _rxHubContext.Clients.All.SendAsync("SendTime", buffer));


            return Ok();
        }
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
                ).Subscribe(num=> {
                    _rxHubContext.Clients.All.SendAsync("SendTime", num);
                })
                ;

            

            return Ok();
        }


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
