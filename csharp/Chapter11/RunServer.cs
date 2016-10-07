using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using com.db4o;

namespace com.db4o.dg2db4o.chapter11
{
    class RunServer : ServerConfiguration
    {
        public static void Main(string[] args)
        {
            RunServer s = new RunServer();
            Thread serverThread = new Thread(new ThreadStart(s.Run));
            serverThread.Priority = ThreadPriority.Highest;
            serverThread.Start();

            //AutoResetEvent autoEvent = new AutoResetEvent(false);
            //ThreadStarter threadStart = new ThreadStarter();
            //TimerCallback timerDelegate = new TimerCallback(threadStart.StartClients);
            //Timer delay = new Timer(timerDelegate, autoEvent, 5000, 0);
            //autoEvent.WaitOne(10000, false);
            //delay.Dispose();
            //Console.WriteLine("MAIN: Destroying timer, client threads should start...");

            RunClient c = new RunClient();
            RunReplicator r = new RunReplicator();
            Thread clientThread = new Thread(new ThreadStart(c.Run));
            Thread replicateThread = new Thread(new ThreadStart(r.Run));
            clientThread.Start();
            replicateThread.Start();
        }

        private bool stop = false;

        private void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting server...");
                File.Delete(REMOTEFILE);
                ObjectServer server = Db4o.OpenServer(REMOTEFILE, PORT);
                server.GrantAccess(USER, PASS);
                try
                {
                    while (!stop)
                    {
                        Monitor.Wait(this, 60000);
                        Console.WriteLine("SERVER: Server is listening ...");
                    }
                }
                catch (ThreadInterruptedException tie)
                {
                    Console.WriteLine("Thread Error!" + tie);
                }
                finally
                {
                    server.Close();
                }
            }
            Console.WriteLine("Server ends...");
        }

    }
}
