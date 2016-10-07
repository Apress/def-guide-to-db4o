using System;
using System.Threading;
using System.IO;
using com.db4o;
using com.db4o.ext;
using com.db4o.io.crypt;

namespace com.db4o.dg2db4o.chapter12
{
    public class Program
    {
         private static string DBFILE = "C:/advanced.yap";
    private static string DBFILE2 = "C:/advanced2.yap";
    private static string ENCRYPTDBFILE = "C:/encrypt.yap";
    private static int PORT = 8732;
    
    public static void Main(String[] args){
        Ids();
        Callbacks();
        Semaphores();
        Encryption();

        Console.WriteLine("Press ENTER to end");
        Console.ReadLine();
    }
    
    private static void Ids(){
        ObjectContainer db;
        long id1;
        Person p1 = new Person("Lincoln", 46);
        Person p2 = new Person("Lincoln", 56);
        
        Console.WriteLine("\nIDS");
        
        File.Delete(DBFILE);
        
        // Store objects and query to list their ids
        db = Db4o.OpenFile(DBFILE);
        try {
            db.Set(p1);
            db.Set(p2);
            Console.WriteLine("OBJECTS AND IDS");
            ObjectSet results = db.Get(null);
            while (results.HasNext()) {
                Object obj = results.Next();
                Console.WriteLine(obj);
                Console.WriteLine(db.Ext().GetID(obj));
            }
            
            // Get id of p1 before closing transaction
            id1 = db.Ext().GetID(p1);
            
        } finally {
            db.Close();
        }
        
        // Re-attach local object p1 to stored object using bind
        db = Db4o.OpenFile(DBFILE);
        try {
            Console.WriteLine("ID OF p1 BEFORE BIND");
            long idnow = db.Ext().GetID(p1);
            Console.WriteLine(idnow);
            db.Ext().Bind(p1,id1);  // bind p1 to stored object with id1
            p1.Name = "Gandhi";   // update and store
            db.Set(p1);
        } finally {
            db.Close();
        }
        
        // Check that update was successful by listing objects
        db = Db4o.OpenFile(DBFILE);
        try {
            Console.WriteLine("OBJECTS AND IDS AFTER UPDATE");
            ObjectSet results = db.Get(null);
            while (results.HasNext()) {
                Object obj = results.Next();
                Console.WriteLine(obj);
                Console.WriteLine(db.Ext().GetID(obj));
            }
        } finally {
            db.Close();
        }
        
        File.Delete(DBFILE);
       File.Delete(DBFILE2);
        
        // Two databases with UUIDs
        Db4o.Configure().GenerateUUIDs(Int32.MaxValue);
        db = Db4o.OpenFile(DBFILE);
        p1 = new Person("Lincoln", 46);
        p2 = new Person("Lincoln", 56);
        try {
            db.Set(p1);
            db.Set(p2);
            Console.WriteLine("OBJECTS AND UUIDS - DATABASE 1");
            ObjectSet results = db.Get(null);
            while (results.HasNext()) {
                Object obj = results.Next();
                Console.WriteLine(obj + ", "  + "UUID:" +
                        db.Ext().GetObjectInfo(obj).GetUUID().GetSignaturePart() + "." +
                        db.Ext().GetObjectInfo(obj).GetUUID().GetLongPart());
            }
        } finally {
            db.Close();
        }
        
        // Second database
        db = Db4o.OpenFile(DBFILE2);
        p1 = new Person("Lincoln", 46);
        p2 = new Person("Lincoln", 56);
        try {
            db.Set(p1);
            db.Set(p2);
            Console.WriteLine("OBJECTS AND UUIDS - DATABASE 2");
            ObjectSet results = db.Get(null);
            while (results.HasNext()) {
                Object obj = results.Next();
                Console.WriteLine(obj + ", "  + "UUID:" +
                        db.Ext().GetObjectInfo(obj).GetUUID().GetSignaturePart() + "." +
                        db.Ext().GetObjectInfo(obj).GetUUID().GetLongPart());
            }
        } finally {
            db.Close();
        }
        
        Console.WriteLine("\nEND IDS");
        
    }
    
    private static void Callbacks(){
        
        Console.WriteLine("\nCALLBACKS");
        // callback
        Db4o.Configure().ObjectClass(typeof(Fruit)).CascadeOnUpdate(true);
        ObjectContainer db = Db4o.OpenFile(DBFILE);
        try {
            Fruit mac = new Fruit("Macintosh Red", 200);
            db.Set(mac);
            mac.Amount = 250;
            mac.Amount = 300;
            db.Set(mac);
            Thread.Sleep(2000);
            mac.Amount = 400;
            db.Set(mac);
            db.Commit();
            Console.WriteLine(mac);
            db.Ext().Refresh(mac,3);
            Console.WriteLine(mac);
            Console.WriteLine("---------------");
        } finally {
            db.Close();
            Console.WriteLine("db closed");
        }
        
        Console.WriteLine("\nEND CALLBACKS");
    }
    
    private static void Semaphores(){
        
        Console.WriteLine("\nSEMAPHORES");
        
        // Start server
        ObjectServer server = Db4o.OpenServer(DBFILE, PORT);
        server.GrantAccess("user1", "password");
        server.GrantAccess("user2", "password");
        server.GrantAccess("user3", "password");
        server.GrantAccess("user4", "password");
        server.GrantAccess("user5", "password");
        
        Console.WriteLine("MAX 4 CLIENTS ALLOWED");
        Console.WriteLine("----------------------");
        Console.WriteLine("FIRST 4 CLIENTS CONNECT");
        
        // Client 1
        LoginManager loginManager1 = new LoginManager("localhost", PORT);
        ObjectContainer client1 = loginManager1.Login("user1", "password");
        
        // Client 2
        LoginManager loginManager2 = new LoginManager("localhost", PORT);
        ObjectContainer client2 = loginManager2.Login("user2", "password");
        
        // Client 3
        LoginManager loginManager3 = new LoginManager("localhost", PORT);
        ObjectContainer client3 = loginManager3.Login("user3", "password");
        
        // Client 4
        LoginManager loginManager4 = new LoginManager("localhost", PORT);
        ObjectContainer client4 = loginManager4.Login("user4", "password");
        
        Console.WriteLine("-------------------------");
        Console.WriteLine("CLIENT 5 TRIES TO CONNECT");
        // Client 5
        LoginManager loginManager5 = new LoginManager("localhost", PORT);
        ObjectContainer client5 = loginManager5.Login("user5", "password");
        
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("CLIENT 2 CLOSED THEN CLIENT 5 CONNECTS");
        client2.Close();
        client5 = loginManager5.Login("user5", "password");
        
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("SERVER CLOSED THEN CLIENT 4 CONNECTS");
        client1.Close();
        client3.Close();
        client4.Close();
        client5.Close();
        server.Close();
        server = Db4o.OpenServer(DBFILE, PORT);
        client4 = loginManager4.Login("user4", "password");
        
        client4.Close();
        server.Close();
        
        Console.WriteLine("\nEND SEMAPHORES");
    }
    
    private static void Encryption(){
        ObjectContainer db=null;
        
        Console.WriteLine("\nENCRYPTION");
        
        File.Delete(ENCRYPTDBFILE);
        
        try {
            //Db4o.Configure().Encrypt(true);
            //Db4o.Configure().Password("mypassword");
            Db4o.Configure().Io(new XTeaEncryptionFileAdapter("mypassword"));
            db = Db4o.OpenFile(ENCRYPTDBFILE);
            
            db.Set(new Person("Lincoln", 56));
        } finally {
            if(db!=null)
                db.Close();
        }
        
        try {
            //Db4o.Configure().Encrypt(true);
            //Db4o.Configure().Password("wrongpassword");
            Db4o.Configure().Io(new XTeaEncryptionFileAdapter("mypassword"));  // try setting to correct password
            db = Db4o.OpenFile(ENCRYPTDBFILE);
            Console.WriteLine("CONTENTS OF ENCRYPTED DATABASE");
            ObjectSet results = db.Get(null);
            while (results.HasNext()) {
                Object obj = results.Next();
                Console.WriteLine(obj);
            }
        } catch (Exception e) {
            Console.WriteLine("Error opening encrypted database!");
        } finally {
            if(db!=null)
                db.Close();
        }
        Console.WriteLine("\nEND ENCRYPTION");
    }
    

        //public static void Main(string[] args)
        //{
        //    convertClassNames("com.db4o.dg2db4o.chapter12", "dg2db4o");
        //    convertClassNames("com.db4o", "db4o");
        //    //readAll();
        //    //addData();
        //    //readAll();
        //    storeList();
        //    readList();
        //    //   File.Delete("C:/nq.yap"); // reset database
        //    //   Db4o.Configure().MessageLevel(1); // 0=silent, 4=loud
        //    //   Db4o.Configure().ObjectClass(typeof(Person)).ObjectField("_name").Indexed(true);
        //    //   ObjectContainer db = Db4o.OpenFile("C:/nq.yap");
        //    //   try
        //    //   {
        //    //       db.Set(new Person("Gandhi", 79));
        //    //       db.Set(new Person("Lincoln", 56));
        //    //       db.Set(new Person("Teresa", 86));
        //    //       db.Set(new Person("Mandela", 86));

        //    ////       ((YapStream)db).GetNativeQueryHandler().AddListener(new PersonQueryExecutionListener());

        //    //       // Simple query
        //    //       IList<Person> persons = db.Query<Person>(delegate(Person person)
        //    //       {
        //    //           //Console.WriteLine("in predicate");
        //    //           return person.Age > 60;
        //    //       });
        //    //       foreach(Person person in persons)
        //    //           Console.WriteLine(person);

        //    //       // Range query
        //    //       persons = db.Query<Person>(delegate(Person person)
        //    //       {
        //    //           //Console.WriteLine("in predicate");
        //    //           return person.Age < 60 || person.Age > 80;
        //    //       });
        //    //       foreach (Person person in persons)
        //    //           Console.WriteLine(person);

        //    //       // Compound query
        //    //       persons = db.Query<Person>(delegate(Person person)
        //    //       {
        //    //           //Console.WriteLine("in predicate");
        //    //           return person.Age > 80 && person.Name.Equals("Mandela");
        //    //           //return person.Age > 80 && person.Name == "Mandela";
        //    //       });
        //    //       foreach (Person person in persons)
        //    //           Console.WriteLine(person);

        //    //       Console.ReadLine();
        //    //   }
        //    //   finally
        //    //   {
        //    //       db.Close();
        //    //   }
        //    //File.Delete("C:/ch12.yap"); // reset database
        //    //Db4o.Configure().ObjectClass(typeof(Fruit)).CascadeOnUpdate(true);
        //    //ObjectContainer db = Db4o.OpenFile("C:/ch12.yap");
        //    //try
        //    //{
        //    //    Fruit mac = new Fruit("Macintosh Red", 200);
        //    //    db.Set(mac);
        //    //    mac.Amount = 300;
        //    //    db.Set(mac);
        //    //    Thread.Sleep(2000);
        //    //    mac.Amount = 400;
        //    //    db.Set(mac);
        //    //    db.Commit();
        //    //    Console.WriteLine(mac);
        //    //    db.Ext().Refresh(mac, 10);
        //    //    Console.WriteLine(mac);
        //    //    Console.WriteLine("---------------");
        //    //    Console.ReadLine();
        //    //}
        //    //finally
        //    //{
        //    //    db.Close();
        //    //}

        //    // Start server
        //    ObjectServer server = Db4o.OpenServer("c:/ch12.yap", 8600);
        //    server.GrantAccess("user1", "password");
        //    server.GrantAccess("user2", "password");
        //    server.GrantAccess("user3", "password");
        //    server.GrantAccess("user4", "password");
        //    server.GrantAccess("user5", "password");

        //    // Concurrent clients
        //    Console.WriteLine("MAX 4 CLIENTS ALLOWED");
        //    Console.WriteLine("----------------------");
        //    Console.WriteLine("FIRST 4 CLIENTS CONNECT");

        //    // Client 1
        //    LoginManager loginManager1 = new LoginManager("localhost", 8600);
        //    ObjectContainer client1 = loginManager1.Login("user1", "password");

        //    // Client 2
        //    LoginManager loginManager2 = new LoginManager("localhost", 8600);
        //    ObjectContainer client2 = loginManager2.Login("user2", "password");

        //    // Client 3
        //    LoginManager loginManager3 = new LoginManager("localhost", 8600);
        //    ObjectContainer client3 = loginManager3.Login("user3", "password");

        //    // Client 4
        //    LoginManager loginManager4 = new LoginManager("localhost", 8600);
        //    ObjectContainer client4 = loginManager4.Login("user4", "password");

        //    Console.WriteLine("-------------------------");
        //    Console.WriteLine("CLIENT 5 TRIES TO CONNECT");
        //    // Client 5
        //    LoginManager loginManager5 = new LoginManager("localhost", 8600);
        //    ObjectContainer client5 = loginManager5.Login("user5", "password");

        //    Console.WriteLine("--------------------------------------");
        //    Console.WriteLine("CLIENT 2 CLOSED THEN CLIENT 5 CONNECTS");
        //    client2.Close();
        //    client5 = loginManager5.Login("user5", "password");

        //    Console.WriteLine("--------------------------------------");
        //    Console.WriteLine("SERVER CLOSED THEN CLIENT 4 CONNECTS");
        //    server.Close();
        //    server = Db4o.OpenServer("c:/ch12.yap", 8600);
        //    client4 = loginManager4.Login("user4", "password");

        //    Console.ReadLine();
        //    //server.Close();
        //}

        

    }
}


