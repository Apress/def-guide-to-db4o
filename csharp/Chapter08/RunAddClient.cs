using System;
using com.db4o;

namespace com.db4o.dg2db4o.chapter8
{
    class RunAddClient
    {
        private ObjectContainer aClient;
        private bool stop = false;

        public void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting add client...");
                aClient = Db4o.OpenClient("localhost", 8732, "user1", "password");
                while (!stop)
                {
                    Console.WriteLine("ADDCLIENT: Please enter a name: ");
                    String name = Console.ReadLine();
                    Console.WriteLine("ADDCLIENT: Please enter an age: ");
                    int age = Convert.ToInt32(Console.ReadLine());
                    aClient.Set(new Person(name, age));
                    aClient.Commit();
                }
                aClient.Close();
            }
        }

    }
}
