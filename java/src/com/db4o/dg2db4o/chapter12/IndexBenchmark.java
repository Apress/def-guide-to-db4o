package com.db4o.dg2db4o.chapter12;

import java.io.File;
import java.util.Random;

/* 
 * You will need to download this library from
 * http://jakarta.apache.org/commons/
 */
import org.apache.commons.lang.RandomStringUtils;

import com.db4o.Db4o;
import com.db4o.ObjectContainer;
import com.db4o.ObjectSet;

public class IndexBenchmark {
	static Random rand = new Random();
	static String dbpath = "C:/DBS/index01.yap"; // Set your path here
	/**
	 * Run it with 50, 500, 5000, 50000 Objects in each for loop and see what
	 * happens!
	 */
	static int iterations = 5000;
	static boolean useIndex = true; // Try with and without for each iterations

	public static void main(String[] args) {
		new IndexBenchmark().run();
	}

	private void run() {
		new File(dbpath).delete();
		System.out.println("And here we go...");

		if (useIndex)
			Db4o.configure().objectClass(Person.class).objectField("name")
					.indexed(true);

		ObjectContainer db = Db4o.openFile(dbpath);

		for (int i = 0; i < iterations; i++)
			db.set(new Person(RandomStringUtils.randomAlphabetic(10), rand
					.nextInt(100)));
		db.commit();
		db.set(new Person("Einstein", 42));
		System.out.println("----- 50 % written ---- !");
		for (int i = 0; i < iterations; i++)
			db.set(new Person(RandomStringUtils.randomAlphabetic(10), rand
					.nextInt(100)));
		db.commit();
		System.out.println("----- 100% written ---- !");

		Person p1 = new Person();
		p1.setName("Einstein");
		long t1 = System.currentTimeMillis();
		ObjectSet res = db.get(p1);
		long t2 = System.currentTimeMillis();
		System.out.println("Found Einstein in " + (t2 - t1) + " ms!");
		listResult(res);

		db.close();
		System.out.println("Done.");
	}

	public static void listResult(ObjectSet result) {
		while (result.hasNext()) {
			System.out.println(result.next());
		}
	}
}






