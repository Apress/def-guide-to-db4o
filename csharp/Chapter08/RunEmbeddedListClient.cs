using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using com.db4o;

namespace com.db4o.dg2db4o.chapter8
{
    class RunEmbeddedListClient
    {
        private ObjectServer server;
        private ObjectContainer lClient = null;
        private bool stop = false;

        public RunEmbeddedListClient(ServerRegistry sr)
        {
            server = sr.GetServer("myserver");
        }

        public void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting list client...");
                ServerRegistry sr2 = ServerRegistry.GetInstance();
                server = sr2.GetServer("myserver");
                try
                {
                    lClient = server.OpenClient();
                    while (!stop)
                    { // Your stop condition here...
                        ListResult(lClient.Get(new Person()));
                        Monitor.Wait(this, 10000);
                    }
                }
                catch (ThreadInterruptedException tie)
                {
                    Console.WriteLine("Thread Error!" + tie);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Server Error!" + e);
                }
                finally
                {
                    lClient.Close();
                }
            }
        }

        private static void ListResult(ObjectSet result)
        {
            Console.WriteLine("\nLISTCLIENT: Listing...");
            while (result.HasNext())
            {
                Console.WriteLine("LISTCLIENT:" + result.Next());
            }
        }

    }
}
