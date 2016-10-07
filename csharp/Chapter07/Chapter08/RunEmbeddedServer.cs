using System;
using System.Collections.Generic;
using System.Threading;
using com.db4o;
using com.db4o.messaging;

namespace com.db4o.dg2db4o.chapter8
{
    class RunEmbeddedServer : MessageRecipient
    {
        private ObjectServer server;
        private bool stop = false;

        // To run in embedded mode uncomment Main method here and comment out Main method in RunServer
        public static void Main(string[] args)
        {
            //ServerRegistry sr = new ServerRegistry();
            ServerRegistry sr = ServerRegistry.GetInstance();
            sr.RegisterServer("C:/embeddedserver.yap", "myserver");
            RunEmbeddedServer s = new RunEmbeddedServer(sr);
            Thread serverThread = new Thread(new ThreadStart(s.Run));
            RunEmbeddedAddClient a = new RunEmbeddedAddClient(sr);
            RunEmbeddedListClient l = new RunEmbeddedListClient(sr);
            Thread addThread = new Thread(new ThreadStart(a.Run));
            Thread listThread = new Thread(new ThreadStart(l.Run));
            serverThread.Start();
            addThread.Start();
            listThread.Start();
        }

        public RunEmbeddedServer(ServerRegistry sr)
        {
            server = sr.GetServer("myserver");
        }

        private void Run()
        {
            Console.WriteLine("Starting server monitor...");
            server.Ext().Configure().SetMessageRecipient(this);
            lock (this)
            {
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

        public void ProcessMessage(ObjectContainer con, Object message)
        {
            lock (this)
            {
                if (message is StopServer)
                {
                    Console.WriteLine("\nSERVER:" + message);
                    stop = true;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
