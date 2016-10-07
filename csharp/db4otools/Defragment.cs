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
using com.db4o;
using com.db4o.ext;
using com.db4o.types;
using j4o.io;
using j4o.lang;
using j4o.lang.reflect;
namespace com.db4o.tools
{
/**
    * defragments a database file. 
    * <br><br>This class is not part of db4o.jar. It is delivered as
    * sourcecode in the path ../com/db4o/tools/<br><br>
    * <b>Prerequites:</b><br>
    * - The database file may not be in use.<br>
    * - All stored classes need to be available.<br>
    * - If you use yor own special Db4o translators, they need to be
    * installed before starting Defragment.
    * <br><br>
    * <b>Performed tasks:</b><br>
    * - Free filespace is removed.<br>
    * - Deleted IDs are removed.<br>
    * - Unavailable classes are removed.<br>
    * - Unavailable class members are removed.<br>
    * - Class indices are restored.<br>
    * - Previous rename tasks are removed.<br>
    * <br>
    * <b>Backup:</b><br>
    * Defragment creates a backup file with the name [filename].bak.
    * If a file with this name is already present, Defragment will not run
    * for safety reasons.<br>
    * <br>
    * <b>Recommendations:</b><br>
    * - Keep the backup copy of your database file.<br>
    * - <b>Always</b> back up your class files with your database files also.<br>
    * You will need them to restore the full data of all objects from old
    * database file versions.<br>
    * - Scan the output log for "Class not available" messages.<br>
    * <br>
    * You may also run this task programmatically on a scheduled basis. In this
    * case note that <code>Defragment</code> modifies db4o configuration
    * parameters. You may have to restore them for your application. See the
    * private methods Defragment#configureDb4o() and Db4o#restoreConfiguration()
    * in the sourcecode of com.db4o.tools.Defragment.cs for the exact changed
    * parameters that may need to be restored.
    */	public class Defragment
	{
		static readonly j4o.lang.Class ObjectClass = j4o.lang.Class.GetClassForType(typeof(object));

/**
       * the main method is the only entry point
       */		public Defragment() : base()
		{
		}

/**
       * the main method that runs Defragment. 
       * @param String[] a String array of length 1, with the name of
       * the database file as element 0.
       */		public static void Main(string[] args)
		{
			if (args != null && args.Length > 0)
			{
				bool forceBackupDelete1 = args.Length > 1 && "!".Equals(args[1]);
				new Defragment().Run(args[0], forceBackupDelete1);
			}
			else
			{
				Console.WriteLine("Usage: java com.db4o.tools.Defragment <database filename>");
			}
		}

/**
       * programmatic interface to run Defragment with a forced delete of
       * a possible old Defragment backup. <br>
       * This method is supplied for regression tests only. It is not
       * recommended to be used by application programmers.
       * @param filename the database file.
       * @param forceBackupDelete forces deleting an old backup.
       * <b>Not recommended.</b>
       */		public void Run(string filename, bool forceBackupDelete)
		{
			File file = new File(filename);
			if (file.Exists())
			{
				bool canRun = true;
				ExtFile backupTest = new ExtFile(file.GetAbsolutePath() + ".bak");
				if (backupTest.Exists())
				{
					if (forceBackupDelete)
					{
						backupTest.Delete();
					}
					else
					{
						canRun = false;
						Console.WriteLine("A backup file with the name ");
						Console.WriteLine("'" + backupTest.GetAbsolutePath() + "'");
						Console.WriteLine("already exists.");
						Console.WriteLine("Remove this file before calling 'Defragment'.");
					}
				}
				if (canRun)
				{
					file.RenameTo(backupTest);
					try {
						ConfigureDb4o();
						ObjectContainer readFrom = Db4o.OpenFile(backupTest.GetAbsolutePath());
						ObjectContainer writeTo = Db4o.OpenFile(file.GetAbsolutePath());
						writeTo.Ext().MigrateFrom(readFrom);
						Migrate(readFrom, writeTo);
						readFrom.Close();
						writeTo.Close();
						Console.WriteLine("Defragment operation completed successfully.");
					}
					catch (Exception e) {
						Console.WriteLine("Defragment operation failed.");
						j4o.lang.JavaSystem.PrintStackTrace(e);
						try {
							new File(filename).Delete();
							backupTest.Copy(filename);
						}
						catch (Exception ex) {
							Console.WriteLine("Restore failed.");
							Console.WriteLine("Please use the backup file:");
							Console.WriteLine("'" + backupTest.GetAbsolutePath() + "'");
							return;
						}
						Console.WriteLine("The original file was restored.");
						try {
							new File(backupTest.GetAbsolutePath()).Delete();
						}
						catch (Exception ignored) {
						}
					}
					finally {
						RestoreConfiguration();
					}
				}
			}
			else
			{
				Console.WriteLine("File '" + file.GetAbsolutePath() + "' does not exist.");
			}
		}

		private void ConfigureDb4o()
		{
			Db4o.Configure().ActivationDepth(0);
			Db4o.Configure().Callbacks(false);
			Db4o.Configure().ClassActivationDepthConfigurable(false);
			Db4o.Configure().WeakReferences(false);
		}

		private void RestoreConfiguration()
		{
			Db4o.Configure().ActivationDepth(5);
			Db4o.Configure().Callbacks(true);
			Db4o.Configure().ClassActivationDepthConfigurable(true);
			Db4o.Configure().WeakReferences(true);
		}

		private void Migrate(ObjectContainer origin, ObjectContainer destination)
		{
			StoredClass[] classes = origin.Ext().StoredClasses();
			FilterAbstractSecondAndNotFoundClasses(classes);
			FilterSubclasses(classes);
			MigrateClasses(origin, destination, classes);
		}

		private static void MigrateClasses(ObjectContainer origin, ObjectContainer destination, StoredClass[] classes)
		{
			for (int i = 0; i < classes.Length; i++) {
				if (classes[i] != null)
				{
					long[] ids = classes[i].GetIDs();
					origin.Ext().Purge();
					destination.Commit();
					destination.Ext().Purge();
					for (int j = 0; j < ids.Length; j++) {
						object obj = origin.Ext().GetByID(ids[j]);
						origin.Activate(obj, 1);
						origin.Deactivate(obj, 2);
						origin.Activate(obj, 3);
						destination.Set(obj);
						origin.Deactivate(obj, 1);
						destination.Deactivate(obj, 1);
					}
				}
			}
		}

		/// <summary>
		/// Remove subclasses from the list since objects from subclasses will be
		/// returned by superclass.getIds()
		/// </summary>
		/// <param name="classes"></param>
		private static void FilterSubclasses(StoredClass[] classes)
		{
			for (int i = 0; i < classes.Length; i++) {
				if (classes[i] == null)
				{
					continue;
				}
				Class javaClass = Class.ForName(classes[i].GetName());
				if (IsSubclass(classes, javaClass))
				{
					classes[i] = null;
				}
			}
		}

		private static bool IsSubclass(StoredClass[] classes, Class candidate)
		{
			for (int j = 0; j < classes.Length; j++) {
				if (classes[j] != null)
				{
					Class superClass1 = Class.ForName(classes[j].GetName());
					if (candidate != superClass1 && superClass1.IsAssignableFrom(candidate))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void FilterAbstractSecondAndNotFoundClasses(StoredClass[] classes)
		{
			for (int i = 0; i < classes.Length; i++) {
				try {
					Class javaClass = Class.ForName(classes[i].GetName());
					if (javaClass == null || javaClass == ObjectClass || IsSecondClass(javaClass) || Modifier.IsAbstract(javaClass.GetModifiers()))
					{
						classes[i] = null;
					}
				}
				catch (ClassNotFoundException e) {
					classes[i] = null;
				}
			}
		}

		private static bool IsSecondClass(Class javaClass)
		{
			return Class.GetClassForType(typeof(SecondClass)).IsAssignableFrom(javaClass);
		}

		private class ExtFile : File
		{
			public ExtFile(string path) : base(path)
			{
			}

			public ExtFile Copy(string toPath)
			{
				try {
					
					{
						new ExtFile(toPath).Mkdirs();
						new ExtFile(toPath).Delete();
						int bufferSize1 = 64000;
						RandomAccessFile rafIn = new RandomAccessFile(GetAbsolutePath(), "r");
						RandomAccessFile rafOut = new RandomAccessFile(toPath, "rw");
						long len = j4o.lang.JavaSystem.GetLengthOf(rafIn);
						byte[] bytes = new byte[bufferSize1];
						while (len > 0) {
								len -= bufferSize1;
								if (len < 0)
								{
									bytes = new byte[(int)(len + bufferSize1)];
								}
								rafIn.Read(bytes);
								rafOut.Write(bytes);
							}
						rafIn.Close();
						rafOut.Close();
						return new ExtFile(toPath);
					}
				}
				catch (Exception e) {
					
					{
						j4o.lang.JavaSystem.PrintStackTrace(e);
						throw e;
					}
				}
			}

		}
	}
}
