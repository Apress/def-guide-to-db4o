using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using com.db4o;
using com.db4o.messaging;

namespace com.db4o.dg2db4o.chapter8
{
    class RunEmbeddedAddClient 
    {
        private ObjectServer server;
        private ObjectContainer aClient;
        private bool stop = false;

        public RunEmbeddedAddClient(ServerRegistry sr)
        {
            server = sr.GetServer("myserver");
        }

        public void Run()
        {
            lock (this)
            {
                try
                {
                    Console.WriteLine("Starting add client...");
                    aClient = server.OpenClient();
                    while (!stop)
                    {
                        Console.WriteLine("\nADDCLIENT: Please enter a name: ");
                        String name = Console.ReadLine();
                        if (name.Equals("stop"))
                        {
                            stop = true;
                        }
                        else
                        {
                            Console.WriteLine("\nADDCLIENT: Please enter an age: ");
                            int age = Convert.ToInt32(Console.ReadLine());
                            aClient.Set(new Person(name, age));
                            aClient.Commit();
                        }
                    }
                }
                catch (ThreadInterruptedException tie)
                {
                    Console.WriteLine("Thread Error!" + tie);
                }
                finally
                {
                    MessageSender messageSender = aClient.Ext().Configure()
                        .GetMessageSender();
                    messageSender.Send(new StopServer("ADDCLIENT says stop!"));
                    aClient.Close();
                }
            }

        }
    }
}
