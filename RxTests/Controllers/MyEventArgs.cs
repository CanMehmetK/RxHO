using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxTests.Controllers
{
    public class MyEventArgs : EventArgs
    {
        private readonly long _value;
        public MyEventArgs(long value)
        {
            _value = value;
        }
        public long Value
        {
            get { return _value; }
        }
    }
}
