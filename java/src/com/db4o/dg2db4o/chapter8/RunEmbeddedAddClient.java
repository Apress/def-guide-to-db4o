
package com.db4o.dg2db4o.chapter8;

import com.db4o.*;
import com.db4o.messaging.MessageSender;
import java.util.Scanner;

public class RunEmbeddedAddClient implements Runnable {
    ObjectServer server;
    ObjectContainer aClient = null;
    private boolean stop = false;
    Scanner sc = new Scanner(System.in);
    
    public RunEmbeddedAddClient(ServerRegistry sr) {
        server = sr.getServer("myserver");
    }
    
    public void run() {
        synchronized(this){
            try {
                aClient = server.openClient();
                while (!stop) { 
                    System.out.print("\nADDCLIENT:Enter the developer's name and age (e.g. 'Tom 44'): ");
                    String name = sc.next();
                    if(name.equals("stop"))
                        stop = true;
                    else {
                        aClient.set(new Person(name, sc.nextInt()));
                        aClient.commit();
                    }
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                MessageSender messageSender = aClient.ext().configure()
                .getMessageSender();
                messageSender.send(new StopServer("\nADDCLIENT says stop!"));
                aClient.close();
            }
        }
    }
    
    
}
