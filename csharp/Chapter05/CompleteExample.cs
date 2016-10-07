using System;
using System.IO;
using com.db4o;   // remember to add db4o.dll as a reference

namespace com.db4o.dg2db4o.chapter5
{
    public class CompleteExample
    {
        public static void Main(string[] args)
        {
            File.Delete("C:/complete.yap"); // reset database
            Db4o.Configure().MessageLevel(0); // 0=silent, 3 =loud
            ObjectContainer db = Db4o.OpenFile("C:/complete.yap");
            try
            {
                db.Set(new Person("Gandhi", 79));
                db.Set(new Person("Lincoln", 56));
                ObjectSet result = (ObjectSet)db.Get(new Person());
                ListResult(result); // get all

                Person p = new Person();
                p.Name = "Gandhi";
                ObjectSet result2 = (ObjectSet)db.Get(p);
                Person p2 = (Person)result2.Next();
                p2.Age = 90; // Increase Gandhis age 
                db.Set(p2);
                result2.Reset(); // reset the ObjectSet cursor before the first element
                ListResult(result2);

                db.Delete(p2); // Remove the Ghandi dataset
                ObjectSet result3 = (ObjectSet)db.Get(new Person());
                ListResult(result3); // get all

                Console.WriteLine("Press ENTER to end");
                Console.ReadLine();
            }
            finally
            {
                db.Close();
            }
        }

        private static void ListResult(ObjectSet res)
        {
            while (res.HasNext())
            {
                Console.WriteLine(res.Next());
            }
            Console.WriteLine("---------------");
        }
    }
}
