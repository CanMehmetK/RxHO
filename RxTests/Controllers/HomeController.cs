using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;
using RxTests.Models;

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

        #region Buffer,Subject bla bla bla
        public IActionResult Buffer()
        {
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
