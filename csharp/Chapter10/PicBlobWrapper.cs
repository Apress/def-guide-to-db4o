using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using j4o.io;
using com.db4o;
using com.db4o.types;


namespace com.db4o.dg2db4o.chapter10
{
    class PicBlobWrapper
    {
        Blob blob;

        public static void Run()
        {
            Console.WriteLine("\nBLOB DEMO");

            // write the blob file to db
            Console.WriteLine(Directory.GetCurrentDirectory());
            ObjectContainer con = Db4o.OpenFile("blobtest.yap");
            PicBlobWrapper picWrapper = new PicBlobWrapper();
            con.Set(picWrapper);
            picWrapper.blob.ReadFrom(new j4o.io.File("test.png"));  // in bin/debug
            con.Close();
            Console.WriteLine("stored...");

            // read the blob file
            con = Db4o.OpenFile("blobtest.yap");
            ObjectSet set = con.Get(new PicBlobWrapper());
            PicBlobWrapper picWrapperOut = (PicBlobWrapper)set.Next();
            picWrapperOut.blob.WriteTo(new j4o.io.File("test2.png"));
            con.Close();
            Console.WriteLine("done...");

            Console.WriteLine("\nEND BLOB DEMO");
           
        }
    }
}
