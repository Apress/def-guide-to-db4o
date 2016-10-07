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
using j4o.io;
using j4o.lang.reflect;
namespace com.db4o.tools
{
/**
     * Logger class to log and analyse objects in RAM.
     * <br><br>This class is not part of db4o.jar. It is delivered as
     * sourcecode in the path ../com/db4o/tools/<br><br>
     */	public class Logger
	{
		private static int MAXIMUM_OBJECTS = 20;

/**
         * opens a database file and logs the content of a class to
         * standard out.
         * @param [database filename] [fully qualified classname]
         */		public static void Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Console.WriteLine("Usage: java com.db4o.tools.Logger <database filename> <class>");
			}
			else
			{
				if (!new File(args[0]).Exists())
				{
					Console.WriteLine("A database file with the name '" + args[0] + "' does not exist.");
				}
				else
				{
					ExtObjectContainer con1 = null;
					try {
						ObjectContainer c1 = Db4o.OpenFile(args[0]);
						if (c1 == null)
						{
							throw new RuntimeException();
						}
						con1 = c1.Ext();
					}
					catch (Exception e) {
						Console.WriteLine("The database file '" + args[0] + "' could not be opened.");
						return;
					}
					if (args.Length > 1)
					{
						StoredClass sc1 = con1.StoredClass(args[1]);
						if (sc1 == null)
						{
							Console.WriteLine("There is no stored class with the name '" + args[1] + "'.");
						}
						else
						{
							long[] ids1 = sc1.GetIDs();
							for (int i1 = 0; i1 < ids1.Length; i1++) {
								if (i1 > MAXIMUM_OBJECTS)
								{
									break;
								}
								object obj1 = con1.GetByID(ids1[i1]);
								con1.Activate(obj1, Int32.MaxValue);
								Log(con1, obj1);
							}
							MsgCount(ids1.Length);
						}
					}
					else
					{
						ObjectSet set1 = con1.Get(null);
						int i1 = 0;
						while (set1.HasNext()) {
								object obj1 = set1.Next();
								con1.Activate(obj1, Int32.MaxValue);
								Log(con1, obj1);
								if (++i1 > MAXIMUM_OBJECTS)
								{
									break;
								}
							}
						MsgCount(set1.Size());
					}
					con1.Close();
				}
			}
		}

/**
         * logs the structure of an object. @param container the {@link ObjectContainer} to be used, or null to log any object. @param <code>Object</code> the object to be analysed.
         */		public static void Log(ObjectContainer container, object obj)
		{
			if (obj == null)
			{
				Log("[NULL]");
			}
			else
			{
				Log(j4o.lang.Class.GetClassForObject(obj).GetName());
				Log(container, obj, 0, new Collection4());
			}
		}

/**
         * logs the structure of an object. @param <code>Object</code> the object to be analysed.
         */		public static void Log(object obj)
		{
			ObjectSet objectSet = obj as ObjectSet;
			if (objectSet != null)
			{
				while (objectSet.HasNext()) {
						Log(objectSet.Next());
					}
			}
			else
			{
				Log(null, obj);
			}
		}

/**
         * logs all objects in the passed ObjectContainer. @param container the {@link ObjectContainer} to be used.
         */		public static void LogAll(ObjectContainer container)
		{
			ObjectSet set1 = container.Get(null);
			while (set1.HasNext()) {
					Log(container, set1.Next());
				}
		}

/**
         * limits logging to a maximum depth. @param int the maximum depth.
         */		public static void SetMaximumDepth(int depth)
		{
			maximumDepth = depth;
		}

		private static void MsgCount(int count)
		{
			Console.WriteLine("\n\nLog complete.\nObjects: " + count);
			if (count > MAXIMUM_OBJECTS)
			{
				Console.WriteLine("Displayed due to setting of " + Class.GetClassForType(typeof(Logger)).GetName() + "#MAXIMUM_OBJECTS: " + MAXIMUM_OBJECTS);
			}
		}

		private static void Log(ObjectContainer a_container, object a_object, int a_depth, Collection4 a_list)
		{
			if (a_list.Contains(a_object) || a_depth > maximumDepth)
			{
				return;
			}
			Class clazz1 = j4o.lang.Class.GetClassForObject(a_object);
			for (int i1 = 0; i1 < IGNORE.Length; i1++) {
				if (clazz1.IsAssignableFrom(IGNORE[i1]))
				{
					return;
				}
			}
			if (Platform4.IsSimple(clazz1))
			{
				Log(a_depth + 1, j4o.lang.Class.GetClassForObject(a_object).GetName(), a_object.ToString());
				return;
			}
			a_list.Add(a_object);
			Class[] classes1 = GetClassHierarchy(a_object);
			string spaces1 = "";
			for (int i1 = classes1.Length - 1; i1 >= 0; i1--) {
				spaces1 = spaces1 + sp;
				string className1 = spaces1;
				int pos1 = classes1[i1].GetName().LastIndexOf(".");
				if (pos1 > 0)
				{
					className1 += classes1[i1].GetName().Substring(pos1);
				}
				else
				{
					className1 += classes1[i1].GetName();
				}
				if (classes1[i1] == Class.GetClassForType(typeof(j4o.util.Date)))
				{
					string fieldName1 = className1 + ".getTime";
					object obj1 = System.Convert.ToInt64(((j4o.util.Date)a_object).GetTime());
					Log(a_container, obj1, Class.GetClassForType(typeof(Int64)), fieldName1, a_depth + 1, -1, a_list);
				}
				else
				{
					Field[] fields1 = classes1[i1].GetDeclaredFields();
					for (int j1 = 0; j1 < fields1.Length; j1++) {
						Platform4.SetAccessible(fields1[j1]);
						string fieldName1 = className1 + "." + fields1[j1].GetName();
						try {
							object obj1 = fields1[j1].Get(a_object);
							if (j4o.lang.Class.GetClassForObject(obj1).IsArray())
							{
								obj1 = NormalizeNArray(obj1);
								int len1 = j4o.lang.reflect.JavaArray.GetLength(obj1);
								for (int k1 = 0; k1 < len1; k1++) {
									object element1 = j4o.lang.reflect.JavaArray.Get(obj1, k1);
									Class arrClass1 = element1 == null ? null : j4o.lang.Class.GetClassForObject(element1);
									Log(a_container, element1, arrClass1, fieldName1, a_depth + 1, k1, a_list);
								}
							}
							else
							{
								Log(a_container, obj1, fields1[j1].GetType(), fieldName1, a_depth + 1, -1, a_list);
							}
						}
						catch (Exception e) {
						}
					}
				}
			}
		}

		private static void Log(ObjectContainer a_container, object a_object, Class a_Class, string a_fieldName, int a_depth, int a_arrayElement, Collection4 a_list)
		{
			if (a_depth > maximumDepth)
			{
				return;
			}
			string fieldName1 = a_arrayElement > -1 ? a_fieldName + sp + sp + a_arrayElement : a_fieldName;
			if (a_object != null)
			{
				if (a_container == null || a_container.Ext().IsStored(a_object))
				{
					if (a_container == null || a_container.Ext().IsActive(a_object))
					{
						Log(a_depth, fieldName1, "");
						Class clazz1 = j4o.lang.Class.GetClassForObject(a_object);
						bool found1 = false;
						if (Platform4.IsSimple(clazz1))
						{
							Log(a_depth + 1, j4o.lang.Class.GetClassForObject(a_object).GetName(), a_object.ToString());
							found1 = true;
						}
						if (!found1)
						{
							Log(a_container, a_object, a_depth, a_list);
						}
					}
					else
					{
						Log(a_depth, fieldName1, "DEACTIVATED " + j4o.lang.Class.GetClassForObject(a_object).GetName());
					}
					return;
				}
				else
				{
					Log(a_depth, fieldName1, a_object.ToString());
				}
			}
			else
			{
				Log(a_depth, fieldName1, "[NULL]");
			}
		}

		private static void Log(string a_msg)
		{
			if (!silent)
			{
				Console.WriteLine(a_msg);
			}
		}

		private static void Log(int indent, string a_property, string a_value)
		{
			for (int i1 = 0; i1 < indent; i1++) {
				a_property = sp + sp + a_property;
			}
			Log(a_property, a_value);
		}

		private static void Log(string a_property, string a_value)
		{
			if (a_value == null) a_value = "[NULL]"; 
			Log(a_property + ": " + a_value);
		}

		private static void Log(bool a_true)
		{
			if (a_true)
			{
				Log("true");
			}
			else
			{
				Log("false");
			}
		}

		private static void Log(Exception e, object obj, string msg)
		{
			string l_msg1;
			if (e != null)
			{
				l_msg1 = "!!! " + j4o.lang.Class.GetClassForObject(e).GetName();
				string l_exMsg1 = e.Message;
				if (l_exMsg1 != null)
				{
					l_msg1 += sp + l_exMsg1;
				}
			}
			else
			{
				l_msg1 = "!!!Exception log";
			}
			if (obj != null)
			{
				l_msg1 += " in " + j4o.lang.Class.GetClassForObject(obj).GetName();
			}
			if (msg != null)
			{
				l_msg1 += sp + msg;
			}
			Log(l_msg1);
		}

		private static Class[] GetClassHierarchy(object a_object)
		{
			Class[] classes1 = new Class[] {j4o.lang.Class.GetClassForObject(a_object)};
			return GetClassHierarchy(classes1);
		}

		private static Class[] GetClassHierarchy(Class[] a_classes)
		{
			Class clazz1 = a_classes[a_classes.Length - 1].GetSuperclass();
			if (clazz1.Equals(Class.GetClassForType(typeof(object))))
			{
				return a_classes;
			}
			Class[] classes1 = new Class[a_classes.Length + 1];
			j4o.lang.JavaSystem.Arraycopy(a_classes, 0, classes1, 0, a_classes.Length);
			classes1[a_classes.Length] = clazz1;
			return GetClassHierarchy(classes1);
		}

		private static object NormalizeNArray(object a_object)
		{
			if (j4o.lang.reflect.JavaArray.GetLength(a_object) > 0)
			{
				object first1 = j4o.lang.reflect.JavaArray.Get(a_object, 0);
				if (first1 != null && j4o.lang.Class.GetClassForObject(first1).IsArray())
				{
					int[] dim1 = ArrayDimensions(a_object);
					object all1 = (object)new object[ArrayElementCount(dim1)];
					NormalizeNArray1(a_object, all1, 0, dim1, 0);
					return all1;
				}
			}
			return a_object;
		}

		private static int NormalizeNArray1(object a_object, object a_all, int a_next, int[] a_dim, int a_index)
		{
			if (a_index == a_dim.Length - 1)
			{
				for (int i1 = 0; i1 < a_dim[a_index]; i1++) {
					j4o.lang.reflect.JavaArray.Set(a_all, a_next++, j4o.lang.reflect.JavaArray.Get(a_object, i1));
				}
			}
			else
			{
				for (int i1 = 0; i1 < a_dim[a_index]; i1++) {
					a_next = NormalizeNArray1(j4o.lang.reflect.JavaArray.Get(a_object, i1), a_all, a_next, a_dim, a_index + 1);
				}
			}
			return a_next;
		}

		private static int[] ArrayDimensions(object a_object)
		{
			int count1 = 0;
			for (Class clazz1 = j4o.lang.Class.GetClassForObject(a_object); clazz1.IsArray(); clazz1 = clazz1.GetComponentType()) {
				count1++;
			}
			int[] dim1 = new int[count1];
			for (int i1 = 0; i1 < count1; i1++) {
				dim1[i1] = j4o.lang.reflect.JavaArray.GetLength(a_object);
				a_object = j4o.lang.reflect.JavaArray.Get(a_object, 0);
			}
			return dim1;
		}

		private static int ArrayElementCount(int[] a_dim)
		{
			int elements1 = a_dim[0];
			for (int i1 = 1; i1 < a_dim.Length; i1++) {
				elements1 *= a_dim[i1];
			}
			return elements1;
		}

		private static Class[] IGNORE = {Class.GetClassForType(typeof(Class))};

		private static int maximumDepth = Int32.MaxValue;

		private static string sp = " ";

		private static bool silent;

	}
}
