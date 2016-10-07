/*
 * Mixed.java
 *
 * Created on 02 April 2006, 20:29
 *
 * To change this template, choose Tools | Template Manager
 * and open the template in the editor.
 */

package com.db4o.dg2db4o.chapter12;

import com.db4o.*;
import com.db4o.ext.*;
import java.util.Scanner;

/**
 *
 * @author jpa1
 */
public class MixedExample {
    private final String DBFILENAME = "c:/mixed.yap";
    
    public static void main(String[] args){
        MixedExample mixed = new MixedExample();
        mixed.convertClassNames();
        mixed.readAll();
        mixed.addData();
        mixed.readAll();
    }
    
    public void readAll() {
        System.out.println("Reading database in Java");
        ObjectContainer db = Db4o.openFile(DBFILENAME);
        ObjectSet results = db.get(new Person());
        while (results.hasNext()) {
            System.out.println(results.next());
        }
        db.close();
    }
    
    public void addData() {
        boolean stop = false;
        System.out.println("Adding new data in Java");
        ObjectContainer db = Db4o.openFile(DBFILENAME);
        Scanner sc = new Scanner(System.in);
        while (!stop) {
            System.out.print("\nEnter the developer's name and age (e.g. 'Tom 44'): ");
            String name = sc.next();
            if(name.equals("stop"))
                stop = true;
            else {
                db.set(new Person(name, sc.nextInt()));
                db.commit();
            };
        }
        db.close();
    }
    
    public void convertClassNames() {
        System.out.println("Checking class names in Java");
        ObjectContainer db = Db4o.openFile(DBFILENAME);
        StoredClass[] classes = db.ext().storedClasses();
        for (int i = 0; i < classes.length; i++) {
            StoredClass storedClass = classes[i];
            String name = storedClass.getName();
            //System.out.println(name);
            String newName = null;
            int pos = name.indexOf(",");
            if(pos > 0){
                newName = name.substring(0, pos);
                storedClass.rename(newName);
                System.out.println("Renaming " + name + " to " + newName);
            }
        }
        db.close();
    }
    
}
