using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter8
{
    class StopServer
    {
        private string _info;

        public StopServer(string info)
        {
            _info = info;
        }

        public override string ToString()
        {
            return _info;
        }
    }
}
