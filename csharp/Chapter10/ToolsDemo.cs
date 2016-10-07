using System;
using com.db4o;
using com.db4o.tools;   // need reference to db4otools.dll - create by building db4otools project

namespace com.db4o.dg2db4o.chapter10
{
    public class ToolsDemo
    {
        public static void Run()
        {
            Console.WriteLine("\nTOOLS DEMO");

            new Statistics().Run("C:/complete.yap");
            new Defragment().Run("C:/complete.yap", true);

            Console.WriteLine("\nEND TOOLS DEMO");
        }
    }
}
