using System;
using System.IO;
using com.db4o;
using com.db4o.query;

namespace com.db4o.dg2db4o.chapter9
{
    class Program
    {
        public static string DBFILE = "C:/transactions.yap";
        public static int PORT = 8732;

        static void Main(string[] args)
        {
            SimpleTransaction();
            Isolation();
            Semaphores();
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }

        private static void ResetDatabase()
        {
            File.Delete(DBFILE);
        }

        private static void SimpleTransaction()
        {
            ObjectContainer db;
            Member member1;
            Member member2;
            double total;

            ResetDatabase();

            Console.WriteLine("\nSIMPLE TRANSACTION");

            // Create test objects
            Member m1 = new Member("Gary", "408 123 4567", "gary@example.net");
            Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@example.com");
            m1.Account.Credit(200.00);
            m2.Account.Credit(200.00);
            total = m1.Account.Balance() + m2.Account.Balance();
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}", total));

            // Store Members
            db = Db4o.OpenFile(DBFILE);
            db.Set(m1);
            db.Set(m2);
            db.Close();

            // make sure update and activation depths are sufficient
            Db4o.Configure().UpdateDepth(5);
            Db4o.Configure().ActivationDepth(5);

            // Transaction: payment from m1 to m2
            double amount = 300.0;
            db = Db4o.OpenFile(DBFILE);

            member1 = (Member)db.Get(new Member("Gary", null, null)).Next();
            member2 = (Member)db.Get(new Member("Rebecca", null, null)).Next();
            try
            {
                member1.Account.Credit(amount);
                db.Set(member1);
                //db.Commit();   //this would cause inconsistency
                member2.Account.Debit(amount);
                db.Set(member2);
            }
            catch (NegativeBalanceException e)
            {
                Console.WriteLine(e.ToString());
                db.Rollback();
            }
            finally
            {
                db.Commit();
            }
            Console.WriteLine("STATE WITHIN TRANSACTION");
            Console.WriteLine(member1);
            Console.WriteLine(member2);

            db.Ext().Refresh(member1, int.MaxValue);
            db.Ext().Refresh(member2, int.MaxValue);
            Console.WriteLine("STATE WITHIN TRANSACTION AFTER REFRESH");
            Console.WriteLine(member1);
            Console.WriteLine(member2);
            db.Close();

            // start a new transaction to query database
            db = Db4o.OpenFile(DBFILE);
            member1 = (Member)db.Get(new Member("Gary", null, null)).Next();
            member2 = (Member)db.Get(new Member("Rebecca", null, null)).Next();
            Console.WriteLine("STATE AFTER TRANSACTION");
            Console.WriteLine(member1);
            Console.WriteLine(member2);
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}", (member1.Account.Balance() + member2.Account.Balance())));
            db.Close();

            Console.WriteLine("\nEND SIMPLE TRANSACTION");
        }

        private static void Isolation()
        {
            ObjectContainer db;
            ObjectContainer client1;
            ObjectContainer client2;
            Member member1;
            Member member2;
            Member member1_1;
            Member member2_1;
            Member member1_2;
            Member member2_2;
            double amount = 100.0;

            Console.WriteLine("\nISOLATION");

            // Reset data
            ResetDatabase();

            Member m1 = new Member("Gary", "408 123 4567", "gary@example.net");
            Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@example.com");
            m1.Account.Credit(200.0);
            m2.Account.Credit(200.0);
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}", (m1.Account.Balance() + m2.Account.Balance())));
            db = Db4o.OpenFile(DBFILE);
            db.Set(m1);
            db.Set(m2);
            db.Close();

            // Start server
            ObjectServer server = Db4o.OpenServer(DBFILE, PORT);
            server.GrantAccess("user1", "password");
            server.GrantAccess("user2", "password");

            Db4o.Configure().ActivationDepth(5);
            Db4o.Configure().UpdateDepth(5);

            client1 = Db4o.OpenClient("localhost", PORT, "user1", "password");
            client2 = Db4o.OpenClient("localhost", PORT, "user2", "password");

            member1_1 = (Member)client1.Get(new Member("Gary", null, null)).Next();
            member2_1 = (Member)client1.Get(new Member("Rebecca", null, null)).Next();

            member1_1.Account.Debit(amount);
            client1.Set(member1_1);
            member2_1.Account.Credit(amount);
            client1.Set(member2_1);

            member1_2 = (Member)client2.Get(new Member("Gary", null, null)).Next();
            member2_2 = (Member)client2.Get(new Member("Rebecca", null, null)).Next();

            // Collision
            double startBalance = member1_2.Account.Balance();
            member1_2.Account.Credit(50);
            client2.Set(member1_2);
            member2_2.Account.Debit(50);
            client2.Set(member2_2);

            Console.WriteLine("READ BEFORE COMMIT");
            Console.WriteLine("CLIENT1:" + member1_1);
            Console.WriteLine("CLIENT1:" + member2_1);
            Console.WriteLine("CLIENT2:" + member1_2);
            Console.WriteLine("CLIENT2:" + member2_2);

            client1.Commit();

            Console.WriteLine("READ AFTER COMMIT");
            Console.WriteLine("CLIENT1:" + member1_1);
            Console.WriteLine("CLIENT1:" + member2_1);
            Console.WriteLine("CLIENT2:" + member1_2);
            Console.WriteLine("CLIENT2:" + member2_2);

            client1.Ext().Refresh(member1_1, int.MaxValue);
            client1.Ext().Refresh(member2_1, int.MaxValue);
            client2.Ext().Refresh(member1_2, int.MaxValue);
            client2.Ext().Refresh(member2_2, int.MaxValue);

            Console.WriteLine("READ AFTER REFRESH");
            Console.WriteLine("CLIENT1:" + member1_1);
            Console.WriteLine("CLIENT1:" + member2_1);
            Console.WriteLine("CLIENT2:" + member1_2);
            Console.WriteLine("CLIENT2:" + member2_2);
            Console.WriteLine("TOTAL: $" + System.String.Format("{0:C}", (member1_2.Account.Balance() + member2_2.Account.Balance())));

            Member persisted = (Member)client2.Ext().PeekPersisted(member1_2, 10, true);
            if (persisted.Account.Balance() != startBalance)
            {
                Console.WriteLine("Object in database has changed during transaction - roll back");
                client2.Rollback();
            }

            client1.Close();
            client2.Close();
            server.Close();

            db = Db4o.OpenFile(DBFILE);
            member1 = (Member)db.Get(new Member("Gary", null, null)).Next();
            member2 = (Member)db.Get(new Member("Rebecca", null, null)).Next();
            Console.WriteLine("STATE AFTER TRANSACTIONS");
            Console.WriteLine(member1);
            Console.WriteLine(member2);
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}", (member1.Account.Balance() + member2.Account.Balance())));
            db.Close();

            Console.WriteLine("\nEND ISOLATION");
        }

        private static void Semaphores()
        {
            ObjectContainer db;
            Member member1;
            Member member2;
            Member member1_1;
            Member member2_1;
            Member member1_2;
            Member member2_2;
            double amount = 100.0;
            String SEMAPHORE_NAME = "Semaphore: ";

            // Reset data
            ResetDatabase();
            Member m1 = new Member("Gary", "408 123 4567", "gary@someisp.com");
            Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@someisp.com");
            m1.Account.Credit(200.0);
            m2.Account.Credit(200.0);
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}",(m1.Account.Balance() + m2.Account.Balance())));
            db = Db4o.OpenFile(DBFILE);
            db.Set(m1);
            db.Set(m2);
            db.Close();

            Db4o.Configure().ActivationDepth(5);
            Db4o.Configure().UpdateDepth(5);

            // Start server
            ObjectServer server = Db4o.OpenServer(DBFILE, PORT);
            server.GrantAccess("user1", "password");
            server.GrantAccess("user2", "password");
            try
            {
                ObjectContainer client1 = Db4o.OpenClient("localhost", PORT, "user1", "password");
                ObjectContainer client2 = Db4o.OpenClient("localhost", PORT, "user2", "password");
                LockManager lockMan1 = new LockManager(client1);
                LockManager lockMan2 = new LockManager(client2);

                // Client1 gets objects
                member1_1 = (Member)client1.Get(new Member("Gary", null, null)).Next();
                member2_1 = (Member)client1.Get(new Member("Rebecca", null, null)).Next();

                // Client1 locks objects
                ObjectLock objectLock = new ObjectLock();
                objectLock.Lock(member1_1, client1);
                objectLock.Lock(member2_1, client1);

                // Client2 gets same objects - now have two running transactions accessing same objects
                member1_2 = (Member)client2.Get(new Member("Gary", null, null)).Next();
                member2_2 = (Member)client2.Get(new Member("Rebecca", null, null)).Next();

                // Client1 does updates
                member1_1.Account.Debit(amount);
                client1.Set(member1_1);
                member2_1.Account.Credit(amount);
                client1.Set(member2_1);

                // Collision - client2 attempts to update an object, tries to get lock
                if (objectLock.Lock(member1_2, client2))
                {
                    member1_2.Account.Debit(50);
                    client2.Set(member1_2);
                    objectLock.Release(member1_2, client2);
                }
                else
                {
                    Console.WriteLine("Cannot write - object locked by another client");
                    client2.Rollback();
                }

                Console.WriteLine("READ BEFORE COMMIT");
                Console.WriteLine("CLIENT1:" + member1_1);
                Console.WriteLine("CLIENT1:" + member2_1);
                Console.WriteLine("CLIENT2:" + member1_2);
                Console.WriteLine("CLIENT2:" + member2_2);

                client1.Commit();

                // client1 releases locks
                objectLock.Release(member1_1, client1);
                objectLock.Release(member2_1, client1);

                Console.WriteLine("READ AFTER COMMIT");
                Console.WriteLine("CLIENT1:" + member1_1);
                Console.WriteLine("CLIENT1:" + member2_1);
                Console.WriteLine("CLIENT2:" + member1_2);
                Console.WriteLine("CLIENT2:" + member2_2);

                client1.Ext().Refresh(member1_1, int.MaxValue);
                client1.Ext().Refresh(member2_1, int.MaxValue);
                client2.Ext().Refresh(member1_2, int.MaxValue);
                client2.Ext().Refresh(member2_2, int.MaxValue);
                Console.WriteLine("READ AFTER REFRESH");
                Console.WriteLine("CLIENT1:" + member1_1);
                Console.WriteLine("CLIENT1:" + member2_1);
                Console.WriteLine("CLIENT2:" + member1_2);
                Console.WriteLine("CLIENT2:" + member2_2);
                Console.WriteLine("TOTAL: " + System.String.Format("{0:C}",(member1_2.Account.Balance() + member2_2.Account.Balance())));

                client2.Close();
                client1.Close();
            }
            finally
            {
                {
                    server.Close();
                }
            }

            Db4o.Configure().ActivationDepth(10);
            db = Db4o.OpenFile(DBFILE);
            member1 = (Member)db.Get(new Member("Gary", null, null)).Next();
            member2 = (Member)db.Get(new Member("Rebecca", null, null)).Next();
            Console.WriteLine("STATE AFTER TRANSACTIONS");
            Console.WriteLine(member1);
            Console.WriteLine(member2);
            Console.WriteLine("TOTAL: " + System.String.Format("{0:C}",(member1.Account.Balance() + member2.Account.Balance())));
            db.Close();

            Console.WriteLine("\nEND SEMAPHORES");
        }
    }
}
