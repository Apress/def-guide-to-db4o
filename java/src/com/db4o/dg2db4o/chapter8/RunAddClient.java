
package com.db4o.dg2db4o.chapter8;

import java.util.Scanner;
import com.db4o.*;
import com.db4o.messaging.*;

public class RunAddClient implements Runnable {
    private boolean stop = false; // Use this flag to quit
    ObjectContainer aClient = null;
    Scanner sc = new Scanner(System.in);
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunAddClient runner = new RunAddClient();
        runner.run();
    }
    
    public void run() {
        synchronized(this){
            try {
                aClient = Db4o.openClient("localhost", 8732, "user1", "password");
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
                messageSender.send(new StopServer("ADDCLIENT says stop!"));
                aClient.close();
            }
        }
    }
}

