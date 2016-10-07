
package com.db4o.dg2db4o.chapter8;

import com.db4o.*;

public class RunEmbeddedListClient implements Runnable {
    ObjectServer server;
    ObjectContainer rClient = null;
    private boolean stop = false;
    
    public RunEmbeddedListClient(ServerRegistry sr) {
        server = sr.getServer("myserver");
    }
    
    public void run() {
        synchronized(this){
            try {
                rClient = server.openClient();
                while (!stop) { 
                    listResult(rClient.get(new Person()));
                    Thread.sleep(15000);
                }
            } catch (Exception e) {
                e.printStackTrace();
                System.out.println("\nLISTCLIENT: server not available");
            } finally {
                rClient.close();
            }
        }
    }
    
    public static void listResult(ObjectSet result) {
        System.out.println("\nLISTCLIENT: Listing...");
        while (result.hasNext()) {
            System.out.println("LISTCLIENT:" + result.next());
        }
    }
}
