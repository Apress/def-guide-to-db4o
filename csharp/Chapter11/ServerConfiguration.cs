using System;
using System.Collections.Generic;
using System.Text;

namespace com.db4o.dg2db4o.chapter11
{
    public abstract class ServerConfiguration
    { 
        public string HOST = "127.0.0.1";
        public string REMOTEFILE = "C:/remote.yap"; // gets changes
        public string LOCALFILE = "C:/local.yap"; // produces changes
        public int PORT = 8732;
        public string USER = "myName";
        public string PASS = "myPwd";
    }

}
