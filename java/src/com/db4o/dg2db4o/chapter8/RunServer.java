
package com.db4o.dg2db4o.chapter8;

import java.io.*;
import com.db4o.*;
import com.db4o.messaging.*;


public class RunServer implements MessageRecipient, Runnable{
    private ObjectServer server;
    private boolean stop = false; // Use this flag to quit 
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunServer runner = new RunServer();
        runner.run();
    }
    
    public void run() {
        synchronized (this) {
            new File("c:/netserver.yap").delete(); 
            ObjectServer server = Db4o.openServer("c:/netserver.yap", 8732);
            server.grantAccess("user1", "password");
            server.grantAccess("user2", "password");
            server.ext().configure().setMessageRecipient(this);
            try {
                while (!stop) {  // Out of band messages here later...
                    System.out.println("SERVER:[" + System.currentTimeMillis()
                    + "] Server's running... ");
                    this.wait(60000);
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                MessageSender messageSender = server.ext().configure().getMessageSender();
                messageSender.send(new StopServer("SERVER is stopping!"));
                server.close();
            }
        }
    }
    
       public void processMessage(ObjectContainer con, Object message) {
        synchronized (this) {
            if(message instanceof StopServer){
                System.out.println("SERVER:" + message);
                stop = true;
                this.notify();
            }
        }
    }
    
    
}



