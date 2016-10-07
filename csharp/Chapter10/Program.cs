using System;
using System.IO;
using com.db4o;

namespace com.db4o.dg2db4o.chapter10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //logging
            Db4o.Configure().MessageLevel(3);
            //Db4o.Configure().SetOut(new j4o.io.PrintStream(new StreamWriter("log.txt")));
            Db4o.Configure().SetOut(new j4o.io.PrintStream(System.Console.Out));

            PicBlobWrapper.Run();
            BackUpDemo.Run();
            ToolsDemo.Run();

            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();
        }
    }
}
