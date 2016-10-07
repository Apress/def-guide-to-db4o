

package com.db4o.dg2db4o.chapter8;

import com.db4o.*;
import com.db4o.messaging.*;

public class RunListClient implements  Runnable{
    private boolean stop = false; // Use this flag to quit the
    ObjectContainer lClient = null;
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunListClient runner = new RunListClient();
        runner.run();
    }
    
    public void run() {
        synchronized(this){
            try {
                lClient = Db4o.openClient("127.0.0.1", 8732, "user2", "password");
                while (!stop) {
                    listResult(lClient.get(new Person()));
                    Thread.sleep(15000);
                    //this.wait(15000);
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                lClient.close();
            }
        }
    }
    
    public static void listResult(ObjectSet result) {
        System.out.println("LISTCLIENT: Listing...");
        while (result.hasNext()) {
            System.out.println("LISTCLIENT:" + result.next());
        }
    }
    
}
