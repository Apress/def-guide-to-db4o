/* Copyright (C) 2004 - 2005  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
using System;
using com.db4o.foundation;
using j4o.lang;
using com.db4o;
using com.db4o.ext;
namespace com.db4o.tools
{
/**
     * prints statistics about a database file to System.out.
     * <br>
     * <br>Pass the database file path as an argument.
     * <br>
     * <br>This class is not part of db4o.dll. It is delivered
     * as sourcecode in the path ../com/db4o/tools/<br><br>
     */	public class Statistics
	{
/**
         * the main method that runs the statistics.
         * @param String[] a String array of length 1, with the name
         * of the database file as element 0.
         */		public static void Main(string[] args)
		{
			if (args == null || args.Length != 1)
			{
				Console.WriteLine("Usage: java com.db4o.tools.Statistics <database filename>");
			}
			else
			{
				new Statistics().Run(args[0]);
			}
		}

		public void Run(string filename)
		{
			if (new j4o.io.File(filename).Exists())
			{
				ObjectContainer con1 = null;
				try {
					con1 = Db4o.OpenFile(filename);
					PrintHeader("STATISTICS");
					Console.WriteLine("File: " + filename);
					PrintStats(con1, filename);
					con1.Close();
				}
				catch (Exception e) {
					Console.WriteLine("Statistics failed for file: '" + filename + "'");
					Console.WriteLine(e.Message);
					j4o.lang.JavaSystem.PrintStackTrace(e);
				}
			}
			else
			{
				Console.WriteLine("File not found: '" + filename + "'");
			}
		}

		private void PrintStats(ObjectContainer con, string filename)
		{
			Tree unavailable = new TreeString(REMOVE);
			Tree noConstructor = new TreeString(REMOVE);
			StoredClass[] internalClasses = con.Ext().StoredClasses();
			for (int i1 = 0; i1 < internalClasses.Length; i1++) {
				try {
					Class clazz1 = Class.ForName(internalClasses[i1].GetName());
					try {
						clazz1.NewInstance();
					}
					catch (Exception th) {
						noConstructor = noConstructor.Add(new TreeString(internalClasses[i1].GetName()));
					}
				}
				catch (Exception t) {
					unavailable = unavailable.Add(new TreeString(internalClasses[i1].GetName()));
				}
			}
			unavailable = unavailable.RemoveLike(new TreeString(REMOVE));
			noConstructor = noConstructor.RemoveLike(new TreeString(REMOVE));
			if (unavailable != null)
			{
				PrintHeader("UNAVAILABLE");
				unavailable.Traverse(new StatisticsPrintKey());
			}
			if (noConstructor != null)
			{
				PrintHeader("NO PUBLIC CONSTRUCTOR");
				noConstructor.Traverse(new StatisticsPrintKey());
			}
			PrintHeader("CLASSES");
			Console.WriteLine("Number of objects per class:");
			if (internalClasses.Length > 0)
			{
				Tree all1 = new TreeStringObject(internalClasses[0].GetName(), internalClasses[0]);
				for (int i1 = 1; i1 < internalClasses.Length; i1++) {
					all1 = all1.Add(new TreeStringObject(internalClasses[i1].GetName(), internalClasses[i1]));
				}
				all1.Traverse(new StatisticsPrintNodes());
			}
			PrintHeader("SUMMARY");
			Console.WriteLine("File: " + filename);
			Console.WriteLine("Stored classes: " + internalClasses.Length);
			if (unavailable != null)
			{
				Console.WriteLine("Unavailable classes: " + unavailable.Size());
			}
			if (noConstructor != null)
			{
				Console.WriteLine("Classes without public constructors: " + noConstructor.Size());
			}
			Console.WriteLine("Total number of objects: " + (ids.Size() - 1));
		}

		private void PrintHeader(string str)
		{
			int starcount = (39 - str.Length) / 2;
			string stars = "";
			for (int i1 = 0; i1 < starcount; i1++) {
				stars += "*";
			}
			Console.WriteLine("\n\n" + stars + " " + str + " " + stars);
		}

		static internal TreeInt ids = new TreeInt(0);

		private static string REMOVE = "XXxxREMOVExxXX";

	}
	internal class StatisticsPrintKey : Visitor4
	{
		public void Visit(object obj)
		{
			Console.WriteLine(((TreeString)obj).i_key);
		}

	}
	internal class StatisticsPrintNodes : Visitor4
	{
		public void Visit(object obj)
		{
			TreeStringObject node = (TreeStringObject)obj;
			long[] newIDs = ((StoredClass)node.i_object).GetIDs();
			for (int j = 0; j < newIDs.Length; j++) {
				if (Statistics.ids.Find(new TreeInt((int)newIDs[j])) == null)
				{
					Statistics.ids = (TreeInt)Statistics.ids.Add(new TreeInt((int)newIDs[j]));
				}
			}
			Console.WriteLine(node.i_key + ": " + newIDs.Length);
		}

	}
}
