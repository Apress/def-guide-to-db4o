
package com.db4o.dg2db4o.chapter12;

import java.util.*;
import java.io.*;
import com.db4o.*;
import com.db4o.inside.query.*;
import com.db4o.query.*;
import com.db4o.io.crypt.XTeaEncryptionFileAdapter;


public class Main {
    private static final String DBFILE = "C:/advanced.yap";
    private static final String DBFILE2 = "C:/advanced2.yap";
    private static final String ENCRYPTDBFILE = "C:/encrypt.yap";
    private static final int PORT = 8732;
    
    public static void main(String[] args){
        ids();
        callbacks();
        semaphores();
        encryption();
    }
    
    private static void ids(){
        ObjectContainer db;
        long id1;
        Person p1 = new Person("Lincoln", 46);
        Person p2 = new Person("Lincoln", 56);
        
        System.out.println("\nIDS");
        
        new File(DBFILE).delete();
        
        // Store objects and query to list their ids
        db = Db4o.openFile(DBFILE);
        try {
            db.set(p1);
            db.set(p2);
            System.out.println("OBJECTS AND IDS");
            ObjectSet results = db.get(null);
            while (results.hasNext()) {
                Object obj = results.next();
                System.out.println(obj);
                System.out.println(db.ext().getID(obj));
            }
            
            // Get id of p1 before closing transaction
            id1 = db.ext().getID(p1);
            
        } finally {
            db.close();
        }
        
        // Re-attach local object p1 to stored object using bind
        db = Db4o.openFile(DBFILE);
        try {
            System.out.println("ID OF p1 BEFORE BIND");
            long idnow = db.ext().getID(p1);
            System.out.println(idnow);
            db.ext().bind(p1,id1);  // bind p1 to stored object with id1
            p1.setName("Gandhi");   // update and store
            db.set(p1);
        } finally {
            db.close();
        }
        
        // Check that update was successful by listing objects
        db = Db4o.openFile(DBFILE);
        try {
            System.out.println("OBJECTS AND IDS AFTER UPDATE");
            ObjectSet results = db.get(null);
            while (results.hasNext()) {
                Object obj = results.next();
                System.out.println(obj);
                System.out.println(db.ext().getID(obj));
            }
        } finally {
            db.close();
        }
        
        new File(DBFILE).delete();
        new File(DBFILE2).delete();
        
        // Two databases with UUIDs
        Db4o.configure().generateUUIDs(Integer.MAX_VALUE);
        db = Db4o.openFile(DBFILE);
        p1 = new Person("Lincoln", 46);
        p2 = new Person("Lincoln", 56);
        try {
            db.set(p1);
            db.set(p2);
            System.out.println("OBJECTS AND UUIDS - DATABASE 1");
            ObjectSet results = db.get(null);
            while (results.hasNext()) {
                Object obj = results.next();
                System.out.println(obj + ", "  + "UUID:" +
                        db.ext().getObjectInfo(obj).getUUID().getSignaturePart() + "." +
                        db.ext().getObjectInfo(obj).getUUID().getLongPart());
            }
        } finally {
            db.close();
        }
        
        // Second database
        db = Db4o.openFile(DBFILE2);
        p1 = new Person("Lincoln", 46);
        p2 = new Person("Lincoln", 56);
        try {
            db.set(p1);
            db.set(p2);
            System.out.println("OBJECTS AND UUIDS - DATABASE 2");
            ObjectSet results = db.get(null);
            while (results.hasNext()) {
                Object obj = results.next();
                System.out.println(obj + ", "  + "UUID:" +
                        db.ext().getObjectInfo(obj).getUUID().getSignaturePart() + "." +
                        db.ext().getObjectInfo(obj).getUUID().getLongPart());
            }
        } finally {
            db.close();
        }
        
        System.out.println("\nEND IDS");
        
    }
    
    private static void callbacks(){
        
        System.out.println("\nCALLBACKS");
        // callback
        Db4o.configure().objectClass(Fruit.class).cascadeOnUpdate(true);
        ObjectContainer db = Db4o.openFile(DBFILE);
        try {
            Fruit mac = new Fruit("Macintosh Red", 200);
            db.set(mac);
            mac.setAmount(250);
            mac.setAmount(300);
            db.set(mac);
            pause(2000);
            mac.setAmount(400);
            db.set(mac);
            db.commit();
            System.out.println(mac);
            db.ext().refresh(mac,3);
            System.out.println(mac);
            System.out.println("---------------");
        } finally {
            db.close();
            System.out.println("db closed");
        }
        
        System.out.println("\nEND CALLBACKS");
    }
    
    private static void semaphores(){
        
        System.out.println("\nSEMAPHORES");
        
        // Start server
        ObjectServer server = Db4o.openServer(DBFILE, PORT);
        server.grantAccess("user1", "password");
        server.grantAccess("user2", "password");
        server.grantAccess("user3", "password");
        server.grantAccess("user4", "password");
        server.grantAccess("user5", "password");
        
        System.out.println("MAX 4 CLIENTS ALLOWED");
        System.out.println("----------------------");
        System.out.println("FIRST 4 CLIENTS CONNECT");
        
        // Client 1
        LoginManager loginManager1 = new LoginManager("localhost", PORT);
        ObjectContainer client1 = loginManager1.login("user1", "password");
        
        // Client 2
        LoginManager loginManager2 = new LoginManager("localhost", PORT);
        ObjectContainer client2 = loginManager2.login("user2", "password");
        
        // Client 3
        LoginManager loginManager3 = new LoginManager("localhost", PORT);
        ObjectContainer client3 = loginManager3.login("user3", "password");
        
        // Client 4
        LoginManager loginManager4 = new LoginManager("localhost", PORT);
        ObjectContainer client4 = loginManager4.login("user4", "password");
        
        System.out.println("-------------------------");
        System.out.println("CLIENT 5 TRIES TO CONNECT");
        // Client 5
        LoginManager loginManager5 = new LoginManager("localhost", PORT);
        ObjectContainer client5 = loginManager5.login("user5", "password");
        
        System.out.println("--------------------------------------");
        System.out.println("CLIENT 2 CLOSED THEN CLIENT 5 CONNECTS");
        client2.close();
        client5 = loginManager5.login("user5", "password");
        
        System.out.println("--------------------------------------");
        System.out.println("SERVER CLOSED THEN CLIENT 4 CONNECTS");
        client1.close();
        client3.close();
        client4.close();
        client5.close();
        server.close();
        server = Db4o.openServer(DBFILE, PORT);
        client4 = loginManager4.login("user4", "password");
        
        client4.close();
        server.close();
        
        System.out.println("\nEND SEMAPHORES");
    }
    
    private static void encryption(){
        ObjectContainer db=null;
        
        System.out.println("\nENCRYPTION");
        
        new File(ENCRYPTDBFILE).delete();
        
        try {
            //Db4o.configure().encrypt(true);
            //Db4o.configure().password("mypassword");
            Db4o.configure().io(new XTeaEncryptionFileAdapter("mypassword"));
            db = Db4o.openFile(ENCRYPTDBFILE);
            
            db.set(new Person("Lincoln", 56));
        } finally {
            if(db!=null)
                db.close();
        }
        
        try {
            //Db4o.configure().encrypt(true);
            //Db4o.configure().password("wrongpassword");
            Db4o.configure().io(new XTeaEncryptionFileAdapter("mypassword"));  // try setting to correct password
            db = Db4o.openFile(ENCRYPTDBFILE);
            System.out.println("CONTENTS OF ENCRYPTED DATABASE");
            ObjectSet results = db.get(null);
            while (results.hasNext()) {
                Object obj = results.next();
                System.out.println(obj);
            }
        } catch (RuntimeException e) {
            System.out.println("Error opening encrypted database!");
        } finally {
            if(db!=null)
                db.close();
        }
        System.out.println("\nEND ENCRYPTION");
    }
    
    public static void pause(int millis) {
        int start = (int) System.currentTimeMillis();
        while(((int)System.currentTimeMillis()) < start + millis) {
            // empty loop body!
        }
    }
    
}
