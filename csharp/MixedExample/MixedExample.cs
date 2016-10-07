using System;
using com.db4o;
using com.db4o.ext;

namespace com.db4o.dg2db4o.chapter12
{
    class MixedExample
    {
        private string DBFILENAME = "c:/mixed.yap";

        public static void Main(String[] args)
        {
            MixedExample mixed = new MixedExample();
            mixed.ConvertClassNames("com.db4o.dg2db4o.chapter12", "MixedExample");  // use assembly name created in this project, was dg2db4o in book
            mixed.ConvertClassNames("com.db4o", "db4o");
            mixed.ReadAll();
            mixed.AddData();
            mixed.ReadAll();

            Console.WriteLine("--------------");
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();

        }

        public void ReadAll()
        {
            Console.WriteLine("Reading database in .NET");
            ObjectContainer db = Db4o.OpenFile(DBFILENAME);
            ObjectSet results = db.Get(new Person());
            while (results.HasNext())
            {
                Console.WriteLine(results.Next());
            }
            db.Close();
        }

        public void AddData()
        {
            bool stop = false;
            Console.WriteLine("Adding new data in .NET");
            ObjectContainer db = Db4o.OpenFile(DBFILENAME);

            while (!stop)
            { // Your stop condition here...
                Console.WriteLine("Please enter a name: ");
                String name = Console.ReadLine();
                if (name.Equals("stop"))
                    break;
                Console.WriteLine("Please enter an age: ");
                int age = Convert.ToInt32(Console.ReadLine());
                Person p = new Person(name, age);
                db.Set(p);
            }
            db.Close();
        }

        public void ConvertClassNames(string classNamespace, string assembly)
        {
            Console.WriteLine("Checking class names in .NET");
            ObjectContainer db = Db4o.OpenFile(DBFILENAME);
            StoredClass[] classes = db.Ext().StoredClasses();
            for (int i = 0; i < classes.Length; i++)
            {
                StoredClass storedClass = classes[i];
                String name = storedClass.GetName();
                //Console.WriteLine(name);
                String newName = null;
                int pos = name.IndexOf(",");
                if (pos == -1)
                {
                    pos = name.IndexOf(classNamespace);
                    if (pos == 0)
                    {
                        newName = name + ", " + assembly;
                        storedClass.Rename(newName);
                        Console.WriteLine("Renaming " + name + " to " + newName);
                    }
                }
            }
            db.Close();
        }
    }
}
