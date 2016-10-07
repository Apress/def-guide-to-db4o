
package com.db4o.dg2db4o.chapter11;

import com.db4o.*;

public class RunClient implements Runnable, ServerConfiguration {
    private boolean stop = false; // Use this flag to quit
    ObjectContainer db = null;
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunClient runner = new RunClient();
        runner.run();
    }
    
    public void run() {
        synchronized(this){
            try {
                db = Db4o.openClient(HOST, PORT, USER, PASS);
                db.set(new Person("Bob", 35)); // write two demo objects
                db.set(new Person("Alice", 29));
                while (!stop) {
                    listResult(db.get(new Person()));
                    Thread.sleep(15000);
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                db.close();
            }
        }
    }
    
    public static void listResult(ObjectSet result) {
        System.out.println("CLIENT: Listing...");
        while (result.hasNext()) {
            System.out.println("CLIENT:" + result.next());
        }
    }
}
