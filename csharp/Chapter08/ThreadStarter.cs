using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace com.db4o.dg2db4o.chapter8
{
    class ThreadStarter
    {

        // This method is called by the timer delegate.
        public void StartClients(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            RunAddClient a = new RunAddClient();
            RunListClient l = new RunListClient();
            Thread addThread = new Thread(new ThreadStart(a.Run));
            Thread listThread = new Thread(new ThreadStart(l.Run));
            addThread.Start();
            listThread.Start();
            autoEvent.Set();
        }
    }
}
