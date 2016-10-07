using System;
using System.Collections.Generic;
using System.IO;
using com.db4o;
using com.db4o.query;
using com.db4o.inside.query;

namespace com.db4o.dg2db4o.chapter6
{

    public class QueryExamples
    {
        private static string DBFILE = "C:/queries.yap";

        public static void Main(string[] args)
        {
            StoreData();
            Qbe();
            NativeQueries();
            Soda();
            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }

        public static void StoreData()
        {
            File.Delete(DBFILE); // reset database
            ObjectContainer db = Db4o.OpenFile(DBFILE);
            db.Set(new Person("Gandhi", 79));
            db.Set(new Person("Lincoln", 56));
            db.Set(new Person("Teresa", 86));
            db.Set(new Person("Mandela", 86));
            db.Set(new Person("John", 42));
            db.Set(new Person("Ben", 82));
            db.Close();
        }

        public static void Qbe()
        {
            ObjectSet res;
            Person template;

            Console.WriteLine("\nQBE EXAMPLES");

            ObjectContainer db = Db4o.OpenFile(DBFILE);

            // Get all Person objects
            Console.WriteLine("ALL PERSON OBJECTS");
            res = db.Get(new Person());
            ListResult(res);

            // Get by name
            Console.WriteLine("FIND BY NAME");
            template = new Person();
            template.Name = "Ben";
            res = db.Get(template);
            ListResult(res);

            // Get by name and age
            Console.WriteLine("FIND BY NAME AND AGE");
            template = new Person();
            template.Name = "Ben";
            template.Age = 42;
            res = db.Get(template);
            ListResult(res);

            // Get by age (name is null)
            Console.WriteLine("FIND BY AGE");
            template = new Person(null, 82);
            res = db.Get(template);
            ListResult(res);

            // Get all Person objects using class name
            Console.WriteLine("ALL PERSON OBJECTS (CLASS NAME)");
            res = db.Get(typeof(Person));
            ListResult(res);

            // Get all objects using null
            Console.WriteLine("ALL OBJECTS");
            res = db.Get(null);
            ListResult(res);

            db.Close();
            Console.WriteLine("END OF QBE EXAMPLES");
        }

        public static void NativeQueries()
        {
            IList<Person> persons;

            Console.WriteLine("\nNQ EXAMPLES");

            ObjectContainer db = Db4o.OpenFile(DBFILE);

            // Add query event handler to check optimization
            PersonQueryExecutionHandler handler = new PersonQueryExecutionHandler();
            ((YapStream)db).GetNativeQueryHandler().QueryExecution +=
                new QueryExecutionHandler(handler.NotifyQueryExecuted);

            // Add a query execution listener - pre 5.2
            // ((YapStream)db).GetNativeQueryHandler().AddListener(new PersonQueryExecutionListener());

            // Simple query
            Console.WriteLine("SIMPLE NQ - AGE > 60");
            persons = db.Query<Person>(delegate(Person person)
            {
                return person.Age > 60;
            });
            foreach (Person person in persons)
                Console.WriteLine(person);

            // Range query
            Console.WriteLine("RANGE NQ - AGE BETWEEN 60 AND 80");
            persons = db.Query<Person>(delegate(Person person)
            {
                return person.Age < 60 || person.Age > 80;
            });
            foreach (Person person in persons)
                Console.WriteLine(person);

            // Compound query
            Console.WriteLine("COMPOUND NQ - AGE >80 AND NAME='MANDELA'");
            persons = db.Query<Person>(delegate(Person person)
            {
                return person.Age > 80 && person.Name.Equals("Mandela");
            });
            foreach (Person person in persons)
                Console.WriteLine(person);

            // Sorted query using 5.2 sorted NQ
            Console.WriteLine("SORTED NQ - ALL PERSONS");
            // Set up Comparison
            Comparison<Person> personCmp = new Comparison<Person>(delegate(Person p1, Person p2)
            {
                return p2.Name.CompareTo(p1.Name);
            });
            // Query with Comparison
            persons = db.Query<Person>(delegate(Person person)
                {
                    return true;
                }, personCmp);

            foreach (Person person in persons)
                Console.WriteLine(person);
            Console.WriteLine("END SORTED SIMPLE NQ V2");

            // Sorted query pre 5.2 using array copy
            Console.WriteLine("SORTED NQ - USING ARRAY COPY");
            persons = db.Query<Person>(delegate(Person person)
                {
                    return true;
                });
            Console.WriteLine("Before sort");
            foreach (Person person in persons)
                Console.WriteLine(person);

            Person[] personsArray = new Person[persons.Count];
            persons.CopyTo(personsArray, 0);
            System.Array.Sort<Person>(personsArray);
            Console.WriteLine("After sort");
            foreach (Person person in personsArray)
                Console.WriteLine(person);
            Console.WriteLine("END SORTED SIMPLE NQ");

            db.Close();
            Console.WriteLine("END OF NQ EXAMPLES");
        }

        public static void Soda()
        {
            ObjectSet res;

            Console.WriteLine("\nQBE EXAMPLES");

            ObjectContainer db = Db4o.OpenFile(DBFILE);


            // Get all Persons
            Console.WriteLine("SODA - ALL PERSONS");
            Query query = db.Query();
            query.Constrain(typeof(Person));
            res = query.Execute();
            ListResult(res);

            // Get by name
            Console.WriteLine("SODA - GET BY NAME");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_name").Constrain("Lincoln"); // search a name
            //query.Descend("_placeOfBirth").Constrain("Hardin County"); // uncomment to try query by invalid field
            res = query.Execute();
            ListResult(res);

            // Not query
            Console.WriteLine("SODA - AGE NOT 56");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_age").Constrain(56).Not(); // not 56
            res = query.Execute();
            ListResult(res);

            // Compound query
            Console.WriteLine("SODA - AGE= 86 AND NAME = 'MANDELA'");
            query = db.Query();
            Constraint firstConstr = query.Descend("_age").Constrain(86); // first constraint
            query.Descend("_name").Constrain("Mandela").And(firstConstr); // second using and
            res = query.Execute();
            ListResult(res);

            // Compound query - alternative version
            Console.WriteLine("SODA - AGE= 86 AND NAME = 'MANDELA'");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_age").Constrain(86); // first constraint
            query.Descend("_name").Constrain("Mandela");
            res = query.Execute();
            ListResult(res);

            // Or query
            Console.WriteLine("SODA - AGE= 86 OR NAME = 'LINCOLN'");
            query = db.Query();
            firstConstr = query.Descend("_age").Constrain(86); // first constraint
            query.Descend("_name").Constrain("Lincoln").Or(firstConstr); // second using and
            res = query.Execute();
            ListResult(res);

            // Range query
            Console.WriteLine("SODA - AGE BETWEEN 60 AND 80");
            query = db.Query();
            query.Constrain(typeof(Person));
            firstConstr = query.Descend("_age").Constrain(60).Greater(); // first constraint
            query.Descend("_age").Constrain(80).Smaller().And(firstConstr); // second using and
            res = query.Execute();
            ListResult(res);

            // "Greater" query
            Console.WriteLine("SODA - AGE > 80");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_age").Constrain(80).Greater();
            res = query.Execute();
            ListResult(res);

            // "Like" query
            Console.WriteLine("SODA - LIKE 'Ma'");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_name").Constrain("Ma").Like(); // also works with "ma"
            res = query.Execute();
            ListResult(res);

            // Sorted query
            Console.WriteLine("SORTED SODA - ALL PERSONS");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_name").OrderAscending(); // the list should start with the lowest
            res = query.Execute();
            ListResult(res);

            // Null/zero value query
            Person p = new Person();
            p.Name = "Fred";
            db.Set(p);
            db.Commit();
            Console.WriteLine("SODA - NULL/ZERO VALUE");
            query = db.Query();
            query.Constrain(typeof(Person));
            query.Descend("_age").Constrain(0); // field has been set
            res = query.Execute();
            ListResult(res);

            db.Close();

            Console.WriteLine("END OF SODA EXAMPLES");
        }

        private static void ListResult(ObjectSet res)
        {
            while (res.HasNext())
                Console.WriteLine(res.Next());
        }
    }
}
