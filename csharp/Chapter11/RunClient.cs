using System;
using System.Threading;
using com.db4o;

namespace com.db4o.dg2db4o.chapter11
{
    class RunClient : ServerConfiguration
    {
        private ObjectContainer db;
        private bool stop = false;

        public void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting client...");
                try
                {
                    db = Db4o.OpenClient(HOST, PORT, USER, PASS);
                    db.Set(new Person("Bob", 35));
                    db.Set(new Person("Alice", 29));
                    db.Commit();
                    while (!stop)
                    { // Your stop condition here...
                        ListResult(db.Get(new Person()));
                        Monitor.Wait(this, 10000);
                    }
                }
                catch (ThreadInterruptedException tie)
                {
                    Console.WriteLine("Thread Error!" + tie);
                }
                finally
                {
                    db.Close();
                }
            }
        }

        private static void ListResult(ObjectSet result)
        {
            while (result.HasNext())
            {
                Console.WriteLine("CLIENT:" + result.Next());
            }
        }

    }
}
