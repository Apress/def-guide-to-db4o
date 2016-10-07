using System;
using System.Threading;
using com.db4o;
using com.db4o.replication;

namespace com.db4o.dg2db4o.chapter11
{
    class RunReplicator : ServerConfiguration
    {
        private bool stop = false;

        public void Run()
        {
            lock (this)
            {
                Console.WriteLine("Starting replicator...");
                Db4o.Configure().GenerateUUIDs(Int32.MaxValue);
                Db4o.Configure().GenerateVersionNumbers(Int32.MaxValue);
                ObjectContainer remoteServer = null;
                ObjectContainer localProducer = null;
                try
                {
                    remoteServer = Db4o.OpenClient(HOST, PORT, USER, PASS);
                    localProducer = Db4o.OpenFile(LOCALFILE);
                    ReplicationConflictHandler conflictHandler = new MyReplicationConflictHandler();
                    ReplicationProcess replication = localProducer.Ext().ReplicationBegin(remoteServer, conflictHandler);
                    replication.SetDirection(localProducer, remoteServer);

                    while (!stop)
                    { // Your stop condition here...
                        Console.WriteLine("REPLICATOR: Please enter a name: ");
                        String name = Console.ReadLine();
                        Console.WriteLine("REPLICATOR: Please enter an age: ");
                        int age = Convert.ToInt32(Console.ReadLine());
                        Person p = new Person(name, age);
                        localProducer.Set(p);
                        replication.Replicate(p);
                        replication.Commit();
                    }
                }
                catch (ThreadInterruptedException tie)
                {
                    Console.WriteLine("Thread Error!" + tie);
                }
                finally
                {
                    localProducer.Close();
                    remoteServer.Close();

                }
            }
        }
    }
}
