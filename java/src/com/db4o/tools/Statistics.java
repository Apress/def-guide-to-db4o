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
package com.db4o.tools;

import com.db4o.*;
import com.db4o.ext.*;
import com.db4o.foundation.*;

/**
 * prints statistics about a database file to System.out.
 * <br><br>Pass the database file path as an argument.
 * <br><br><b>This class is not part of db4o.jar!</b><br>
 * It is delivered as sourcecode in the
 * path ../com/db4o/tools/<br><br>
 */
public class Statistics {

	/**
	 * the main method that runs the statistics.
	 * @param String[] a String array of length 1, with the name of the database
	 * file as element 0.
	 */
	public static void main(String[] args) {
		Db4o.configure().messageLevel(-1);
		if (args == null || args.length != 1) {
			System.out.println("Usage: java com.db4o.tools.Statistics <database filename>");
		} else {
			new Statistics().run(args[0]);
		}
	}

	public void run(String filename) {
		if (new java.io.File(filename).exists()) {
			ObjectContainer con = null;
			try {
				con = Db4o.openFile(filename);
				printHeader("STATISTICS");
				System.out.println("File: " + filename);
				printStats(con, filename);
				con.close();
			} catch (Exception e) {
				System.out.println("Statistics failed for file: '" + filename + "'");
				System.out.println(e.getMessage());
				e.printStackTrace();
			}
		} else {
			System.out.println("File not found: '" + filename + "'");
		}
	}

	private void printStats(ObjectContainer con, String filename) {

		Tree unavailable = new TreeString(REMOVE);
		Tree noConstructor = new TreeString(REMOVE);

		final TreeInt[] ids = { new TreeInt(0)};
		// one element too many, substract one in the end

		StoredClass[] internalClasses = con.ext().storedClasses();
		for (int i = 0; i < internalClasses.length; i++) {
			try {
				Class clazz = Class.forName(internalClasses[i].getName());
				try {
					clazz.newInstance();
				} catch (Throwable th) {
					noConstructor =
						noConstructor.add(new TreeString(internalClasses[i].getName()));
				}
			} catch (Throwable t) {
				unavailable = unavailable.add(new TreeString(internalClasses[i].getName()));
			}
		}
		unavailable = unavailable.removeLike(new TreeString(REMOVE));
		noConstructor = noConstructor.removeLike(new TreeString(REMOVE));
		if (unavailable != null) {
			printHeader("UNAVAILABLE");
			unavailable.traverse(new Visitor4() {
				public void visit(Object obj) {
					System.out.println(((TreeString) obj).i_key);
				}
			});
		}
		if (noConstructor != null) {
			printHeader("NO PUBLIC CONSTRUCTOR");
			noConstructor.traverse(new Visitor4() {
				public void visit(Object obj) {
					System.out.println(((TreeString) obj).i_key);
				}
			});
		}

		printHeader("CLASSES");
		System.out.println("Number of objects per class:");

		if (internalClasses.length > 0) {
			Tree all = new TreeStringObject(internalClasses[0].getName(), internalClasses[0]);
			for (int i = 1; i < internalClasses.length; i++) {
				all =
					all.add(
						new TreeStringObject(internalClasses[i].getName(), internalClasses[i]));
			}
			all.traverse(new Visitor4() {
				public void visit(Object obj) {
					TreeStringObject node = (TreeStringObject) obj;
					long[] newIDs = ((StoredClass)node.i_object).getIDs();
					for (int j = 0; j < newIDs.length; j++) {
						if (ids[0].find(new TreeInt((int) newIDs[j])) == null) {
							ids[0] = (TreeInt) ids[0].add(new TreeInt((int) newIDs[j]));
						}
					}
					System.out.println(node.i_key + ": " + newIDs.length);
				}
			});

		}

		printHeader("SUMMARY");
		System.out.println("File: " + filename);
		System.out.println("Stored classes: " + internalClasses.length);
		if (unavailable != null) {
			System.out.println("Unavailable classes: " + unavailable.size());
		}
		if (noConstructor != null) {
			System.out.println("Classes without public constructors: " + noConstructor.size());
		}
		System.out.println("Total number of objects: " + (ids[0].size() - 1));
	}
	
	private void printHeader(String str){
		int stars = (39 - str.length()) / 2;
		System.out.println("\n");
		for (int i = 0; i < stars; i++) {
			System.out.print("*");
		}
		System.out.print(" " + str + " ");
		for (int i = 0; i < stars; i++) {
			System.out.print("*");
		}
		System.out.println();
	}

	private static final String REMOVE = "XXxxREMOVExxXX";

}
