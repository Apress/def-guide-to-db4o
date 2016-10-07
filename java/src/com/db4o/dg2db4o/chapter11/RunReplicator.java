
package com.db4o.dg2db4o.chapter11;

import com.db4o.*;
import com.db4o.replication.*;
import java.util.Scanner;

public class RunReplicator implements Runnable, ServerConfiguration {
    private boolean stop = false; // Use this flag to quit
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunReplicator runner = new RunReplicator();
        runner.run();
    }
    
    public void run() {
        Db4o.configure().generateUUIDs(Integer.MAX_VALUE);
        Db4o.configure().generateVersionNumbers(Integer.MAX_VALUE);
        ObjectContainer remoteServer = null;
        ObjectContainer localProducer = null;
        try {
            remoteServer = Db4o.openClient(HOST, PORT, USER, PASS);
            localProducer = Db4o.openFile(LOCALFILE);
            
            ReplicationProcess replication = localProducer.ext().replicationBegin(
                    remoteServer, new ReplicationConflictHandler() {
                public Object resolveConflict(
                        ReplicationProcess replicationProcess, Object a, Object b) {
                    return a;
                }
            });
            replication.setDirection(localProducer, remoteServer);
            
            Scanner sc = new Scanner(System.in);
            while(!stop) {
                System.out.print("Enter next surname and age (e.g. 'Tom 44'): ");
                Person p = new Person(sc.next(), sc.nextInt());
                localProducer.set(p);
                replication.replicate(p);
                replication.commit();
            }
            sc.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
        localProducer.close();
        remoteServer.close();
        System.out.println("Replicator shut down!");
    }
    
}
