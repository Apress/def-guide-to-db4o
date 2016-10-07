using System;
using com.db4o;

namespace com.db4o.dg2db4o.chapter10
{
    public class BackUpDemo
    {
        public static void Run()
        {
            Console.WriteLine("\nBACKUP DEMO");
            System.IO.File.Delete("C:/primary.yap");
            System.IO.File.Delete("C:/secondary.yap");
            ObjectContainer db = Db4o.OpenFile("C:/primary.yap");
            db.Set(new Person("Stefan", 39));
            db.Commit();
            try
            {
                db.Ext().Backup("C:/secondary.yap");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                db.Close();
            }
            db.Close();

            Console.WriteLine("\nEND BACKUP DEMO");

        }
    }
}
