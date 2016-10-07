using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using com.db4o;

namespace com.db4o.dg2db4o.chapter8
{
    class RunListClient
    {
        ObjectContainer lClient = null;
        private bool stop = false;

        public void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting list client...");
                try
                {
                    lClient = Db4o.OpenClient("localhost", 8732, "user2", "password");
                    lClient.Set(new Person("Alice", 22));
                    lClient.Set(new Person("Bob", 26));
                    lClient.Commit();
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
