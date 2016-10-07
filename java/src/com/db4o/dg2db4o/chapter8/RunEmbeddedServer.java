
package com.db4o.dg2db4o.chapter8;

import com.db4o.*;
import com.db4o.messaging.MessageRecipient;

public class RunEmbeddedServer implements MessageRecipient, Runnable{
    private ObjectServer server;
    private boolean stop = false; // Use this flag to quit
    
    public RunEmbeddedServer(ServerRegistry sr) {
        server = sr.getServer("myserver");
    }
    
    public void run() {
        server.ext().configure().setMessageRecipient(this);
        synchronized (this) {
            try {
                while (!stop) {  
                    System.out.println("\nSERVER:[" + System.currentTimeMillis()
                    + "] Server's running... ");
                    this.wait(60000);
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                System.out.println("\nSERVER:[" + System.currentTimeMillis()
                + "] Server's stopped! ");
                server.close();
            }
        }
    }
    
    public void processMessage(ObjectContainer con, Object message) {
        synchronized (this) {
            if(message instanceof StopServer){
                System.out.println("\nSERVER:" + message);
                stop = true;
                this.notify();
            }
        }
    }
}
