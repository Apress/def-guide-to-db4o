
package com.db4o.dg2db4o.chapter11;

public class Main {
    
    public static void main(String[] args){
        // network server 
        Thread s = new Thread(new RunServer(),"server");
        Thread c = new Thread(new RunClient(), "client");
        Thread r = new Thread(new RunReplicator(), "replicator");
        s.setPriority(Thread.MAX_PRIORITY);
        s.start();
         try{  // wait before starting client threads
            Thread.currentThread().sleep(1000);
        }catch(InterruptedException ie){
            ie.printStackTrace();
        }
        c.start();
        r.start();
    }
    
}
