
package com.db4o.dg2db4o.chapter8;

import com.db4o.*;
import java.util.concurrent.*;

public class Main {
    
    public static void main(String[] args){
        // choose one run scenario, or run network server/clients as separate processes
        //runNetworkServer();
        //runNetworkServerWithSceduledExecutor();
        runEmbeddedServer();
    }
    
    // starts network server and clients in threads localhost)
    private static void runNetworkServer(){
        Thread s = new Thread(new RunServer(),"server");
        Thread a = new Thread(new RunAddClient(), "add client");
        Thread l = new Thread(new RunListClient(), "list client");
        s.setPriority(Thread.MAX_PRIORITY);
        s.start();
        try{  // wait before starting client threads
            Thread.currentThread().sleep(1000);
        }catch(InterruptedException ie){
            ie.printStackTrace();
        }
        l.start();
        a.start();
    }
    
    // starts network server and clients with scheduled executor service (localhost)
    private static void runNetworkServerWithSceduledExecutor(){
        Thread s = new Thread(new RunServer(),"server");
        Thread a = new Thread(new RunAddClient(), "add client");
        Thread l = new Thread(new RunListClient(), "list client");
        ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(3);
        System.out.println("MAIN: Created Scheduler ...");
        ScheduledFuture<?> fs = scheduler.schedule(s,0,TimeUnit.SECONDS);
        ScheduledFuture<?> fa = scheduler.schedule(a,10,TimeUnit.SECONDS);
        ScheduledFuture<?> fl = scheduler.schedule(l,20,TimeUnit.SECONDS);
        while(!fs.isDone()){
            try{
                Thread.currentThread().sleep(1000);
            }catch(InterruptedException ie){
                ie.printStackTrace();
            }
        }
        fl.cancel(true);
        scheduler.shutdown();
        System.out.println("MAIN: Scheduler shut down");
    }
    
    // starts embedded server and clients as threads
    private static void runEmbeddedServer(){
        ServerRegistry sr = new ServerRegistry();
        sr.registerServer("C:/embeddedserver.yap", "myserver");
        Thread s = new Thread(new RunEmbeddedServer(sr),"monitor server");
        s.start();
        Thread a = new Thread(new RunEmbeddedAddClient(sr), "add client");
        Thread l = new Thread(new RunEmbeddedListClient(sr), "list client");
        l.start();
        a.start();
    }
    
}
