
package com.db4o.dg2db4o.chapter9;

import java.io.File;
import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import java.text.NumberFormat;
import com.db4o.*;
import com.db4o.query.*;

public class Main {
    public static final String DBFILE = "C:/transactions.yap";
    public static final int PORT = 8732;
    
    public static void main(String[] args) {
        simpleTransaction();
        isolation();
        semaphores();
    }
    
    private static void resetDatabase() {
        new File(DBFILE).delete();
    }
    
    private static void simpleTransaction(){
        ObjectContainer db;
        Member member1;
        Member member2;
        
        System.out.println("\nSIMPLE TRANSACTION");
        
        resetDatabase();
        
        // Create test objects
        Member m1 = new Member("Gary", "408 123 4567", "gary@example.net");
        Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@example.com");
        m1.getAccount().credit(200.0);
        m2.getAccount().credit(200.0);
        System.out.println("TOTAL: $" + (m1.getAccount().balance() + m2.getAccount().balance()));
        
        // Store Members
        db = Db4o.openFile(DBFILE);
        db.set(m1);
        db.set(m2);
        db.close();
        
        // make sure update and activation depths are sufficient
        Db4o.configure().updateDepth(5);
        Db4o.configure().activationDepth(5);
        
        // Transaction: payment from m1 to m2
        double amount = 300.0;
        db = Db4o.openFile(DBFILE);
        
        member1 = (Member)db.get(new Member("Gary", null, null)).next();
        member2 = (Member)db.get(new Member("Rebecca", null, null)).next();
        try {
            member1.getAccount().credit(amount);
            db.set(member1);
            // db.commit();   //this would cause inconsistency
            member2.getAccount().debit(amount);
            db.set(member2);
        } catch (NegativeBalanceException e) {
            System.out.println(e.toString());
            db.rollback();
        } finally {
            db.commit();
        }
        System.out.println("STATE WITHIN TRANSACTION");
        System.out.println(member1);
        System.out.println(member2);
        
        db.ext().refresh(member1, Integer.MAX_VALUE);
        db.ext().refresh(member2, Integer.MAX_VALUE);
        System.out.println("STATE WITHIN TRANSACTION AFTER REFRESH");
        System.out.println(member1);
        System.out.println(member2);
        db.close();
        
        // start a new transaction to query database
        db = Db4o.openFile(DBFILE);
        member1 = (Member)db.get(new Member("Gary", null, null)).next();
        member2 = (Member)db.get(new Member("Rebecca", null, null)).next();
        System.out.println("STATE AFTER TRANSACTION");
        System.out.println(member1);
        System.out.println(member2);
        double total = member1. getAccount().balance() + member2. getAccount().balance();
        System.out.println("TOTAL: " + NumberFormat.getCurrencyInstance().format(total));
        
        db.close();
        
        System.out.println("\nEND SIMPLE TRANSACTION");
    }
    
    private static void isolation() {
        ObjectContainer db;
        ObjectContainer client1;
        ObjectContainer client2;
        Member member1;
        Member member2;
        Member member1_1;
        Member member2_1;
        Member member1_2;
        Member member2_2;
        double amount = 100.0;
        
        System.out.println("\nISOLATION");
        
        resetDatabase();
        
        Member m1 = new Member("Gary", "408 123 4567", "gary@example.net");
        Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@example.com");
        m1.getAccount().credit(200.00);
        m2.getAccount().credit(200.00);
        
        db = Db4o.openFile(DBFILE);
        db.set(m1);
        db.set(m2);
        db.close();
        
        // Start server
        ObjectServer server = Db4o.openServer(DBFILE, PORT);
        server.grantAccess("user1", "password");
        server.grantAccess("user2", "password");
        
        Db4o.configure().activationDepth(5);
        Db4o.configure().updateDepth(5);
        
        try {
            client1 = Db4o.openClient("localhost", PORT, "user1", "password");
            client2 = Db4o.openClient("localhost", PORT, "user2", "password");
            
            member1_1 = (Member)client1.get(new Member("Gary", null, null)).next();
            member2_1 = (Member)client1.get(new Member("Rebecca", null, null)).next();
            
            member1_1.getAccount().debit(amount);
            client1.set(member1_1);
            member2_1.getAccount().credit(amount);
            client1.set(member2_1);
            
            member1_2 = (Member)client2.get(new Member("Gary", null, null)).next();
            member2_2 = (Member)client2.get(new Member("Rebecca", null, null)).next();
            
            // Collision
            double startBalance = member1_2.getAccount().balance();
            member1_2.getAccount().credit(50);
            client2.set(member1_2);
            member2_2.getAccount().debit(50);
            client2.set(member2_2);
            
            System.out.println("READ BEFORE COMMIT");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            
            client1.commit();
            
            System.out.println("READ AFTER COMMIT");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            
            client1.ext().refresh(member1_1, Integer.MAX_VALUE);
            client1.ext().refresh(member2_1, Integer.MAX_VALUE);
            client2.ext().refresh(member1_2, Integer.MAX_VALUE);
            client2.ext().refresh(member2_2, Integer.MAX_VALUE);
            
            System.out.println("READ AFTER REFRESH");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            double total = member1_1. getAccount().balance() + 
                    member1_2. getAccount().balance();
            System.out.println("TOTAL: " + 
                    NumberFormat.getCurrencyInstance().format(total));
            
            Member persisted = (Member) client2.ext().peekPersisted(member1_2, 10, true);
            if (persisted.getAccount().balance() != startBalance) {
                System.out.println("Object in database has changed during transaction - roll back");
                client2.rollback();
            }
            
            client1.close();
            client2.close();  // OK in 5.0 - in 5.2, hangs here if client2 not rolled back
            server.close();
            
        }catch(Exception e){
            e.printStackTrace();
        }
        
        db = Db4o.openFile(DBFILE);
        member1 = (Member)db.get(new Member("Gary", null, null)).next();
        member2 = (Member)db.get(new Member("Rebecca", null, null)).next();
        System.out.println("STATE AFTER TRANSACTIONS");
        System.out.println(member1);
        System.out.println(member2);
        double total = member1. getAccount().balance() + member2. getAccount().balance();
        System.out.println("TOTAL: " + NumberFormat.getCurrencyInstance().format(total));
        
        db.close();
        
        System.out.println("\nEND ISOLATION");
    }
    
    private static void semaphores(){
        ObjectContainer db;
        Member member1;
        Member member2;
        Member member1_1;
        Member member2_1;
        Member member1_2;
        Member member2_2;
        double amount = 100.0;
        String SEMAPHORE_NAME = "Semaphore: ";
        
        System.out.println("\nSEMAPHORES");
        
        resetDatabase();
        
        Member m1 = new Member("Gary", "408 123 4567", "gary@example.net");
        Member m2 = new Member("Rebecca", "408 987 6543", "rebecca@example.com");
        m1.getAccount().credit(200.00);
        m2.getAccount().credit(200.00);
        
        db = Db4o.openFile(DBFILE);
        db.set(m1);
        db.set(m2);
        db.close();
        
        Db4o.configure().updateDepth(5);
        
        // Start server
        ObjectServer server = Db4o.openServer(DBFILE, PORT);
        server.grantAccess("user1", "password");
        server.grantAccess("user2", "password");
        try {
            ObjectContainer client1 = Db4o.openClient("localhost", PORT, "user1", "password");
            ObjectContainer client2 = Db4o.openClient("localhost", PORT, "user2", "password");
            
            // Client1 gets objects
            member1_1 = (Member)client1.get(new Member("Gary", null, null)).next();
            member2_1 = (Member)client1.get(new Member("Rebecca", null, null)).next();
            
            // Client1 locks objects
            ObjectLock objectLock = new ObjectLock();
            objectLock.lock(member1_1, client1);
            objectLock.lock(member2_1, client1);
            
            // Client2 gets same objects - now have two running transactions accessing same objects
            member1_2 = (Member)client2.get(new Member("Gary", null, null)).next();
            member2_2 = (Member)client2.get(new Member("Rebecca", null, null)).next();
            
            // Client1 does updates
            member1_1.getAccount().debit(amount);
            client1.set(member1_1);
            member2_1.getAccount().credit(amount);
            client1.set(member2_1);
            
            // Collision - client2 attempts to update an object, tries to get lock
            if (objectLock.lock(member1_2, client2)) {
                member1_2.getAccount().debit(50);
                client2.set(member1_2);
                objectLock.release(member1_2, client2);
            } else{
                System.out.println("Cannot write - object locked by another client");
                client2.rollback();
            }
            
            System.out.println("READ BEFORE COMMIT");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            
            client1.commit();
            // client1 releases locks
            objectLock.release(member1_1, client1);
            objectLock.release(member2_1, client1);
            
            System.out.println("READ AFTER COMMIT");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            
            client1.ext().refresh(member1_1, Integer.MAX_VALUE);
            client1.ext().refresh(member2_1, Integer.MAX_VALUE);
            client2.ext().refresh(member1_2, Integer.MAX_VALUE);
            client2.ext().refresh(member2_2, Integer.MAX_VALUE);
            
            System.out.println("READ AFTER REFRESH");
            System.out.println("CLIENT1:" + member1_1);
            System.out.println("CLIENT1:" + member2_1);
            System.out.println("CLIENT2:" + member1_2);
            System.out.println("CLIENT2:" + member2_2);
            double total = member1_1. getAccount().balance() + member2_1. getAccount().balance();
            System.out.println("TOTAL: " + NumberFormat.getCurrencyInstance().format(total));
            
            client2.close();
            client1.close();
        } catch (Exception e){
        } finally {
            server.close();
        }
        
        db = Db4o.openFile(DBFILE);
        member1 = (Member)db.get(new Member("Gary", null, null)).next();
        member2 = (Member)db.get(new Member("Rebecca", null, null)).next();
        System.out.println("STATE AFTER TRANSACTIONS");
        System.out.println(member1);
        System.out.println(member2);
        double total = member1. getAccount().balance() + member2. getAccount().balance();
        System.out.println("TOTAL: " + NumberFormat.getCurrencyInstance().format(total));
        
        db.close();
        
        System.out.println("\nEND SEMAPHORES");
        
    }
    
}
