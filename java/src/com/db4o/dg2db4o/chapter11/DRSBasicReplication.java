
package com.db4o.dg2db4o.chapter11;

import java.io.File;
import com.db4o.*;
import com.db4o.replication.ReplicationSession;
import com.db4o.replication.Replication;
import com.db4o.replication.ConflictResolver;
import org.hibernate.cfg.Configuration;

public class DRSBasicReplication {
    private final String LOCAL_DB = "C:/local01.yap"; 
    private final String REMOTE_DB = "C:/remote01.yap"; 
    
    public static void main(String[] args) {
        DRSBasicReplication basicRep = new DRSBasicReplication();
        basicRep.runReplication();
    }
    
    public void runReplication(){
        Db4o.configure().generateUUIDs(Integer.MAX_VALUE);
        Db4o.configure().generateVersionNumbers(Integer.MAX_VALUE);
        
        new File(LOCAL_DB).delete();
        ObjectContainer handheld = Db4o.openFile(LOCAL_DB);
        handheld.set(new Person("Gandhi", 79));
        handheld.set(new Person("Lincoln", 56));
        handheld.commit();
        
        new File(REMOTE_DB).delete();
        ObjectContainer desktop = Db4o.openFile(REMOTE_DB);
        
        
        //ReplicationSession replication = Replication.begin(handheld, desktop);
        ReplicationSession replication = Replication.begin( handheld, desktop,
                new ConflictResolver() {
            public Object resolveConflict(ReplicationSession session,
                    Object a, Object b) {
                return a;
            }
        });
        
        ObjectSet changed = replication.providerA().objectsChangedSinceLastReplication();
        while (changed.hasNext())
            replication.replicate(changed.next());
//        while (changed.hasNext()) {
//            Person p = (Person) changed.next();
//            if(p.getAge()%2 == 0)
//                replication.replicate(p);
//        }
        
        replication.commit();
        replication.close();
        
        desktop.set(new Person("Teresa", 87));
        desktop.set(new Person("Mandela", 86));
        Person update = (Person)desktop.get(new Person("Gandhi",0)).next();
        update.setAge(80);
        desktop.set(update);
        desktop.commit();

        replication = Replication.begin( handheld, desktop,
                new ConflictResolver() {
            public Object resolveConflict(ReplicationSession session,
                    Object a, Object b) {
                return a;
            }
        });

        ObjectSet changed2 = replication.providerB().objectsChangedSinceLastReplication();
        while (changed2.hasNext())
            replication.replicate(changed2.next());
        replication.commit();
        
        try {
            Thread.sleep(5000); // Wait 5 seconds...
        } catch (Exception e) {
            e.printStackTrace();
        }
        
        ObjectSet localdata = handheld.get(new Person()); // using the local handheld container
        while (localdata.hasNext()) {
            Person p = (Person) localdata.next();
            //System.out.println("Local:" + localdata.next());
            System.out.println("Local:" + p + ", "  + "UUID:" + handheld.ext().getObjectInfo(p).getUUID().getSignaturePart() +
                    "." + handheld.ext().getObjectInfo(p).getUUID().getLongPart() +
                    ", version:" + handheld.ext().getObjectInfo(p).getVersion());
        }
        ObjectSet remotedata = desktop.get(new Person()); // using the remote desktop container
        while (remotedata.hasNext()) {
            //System.out.println("Remote:" + remotedata.next());
            Person p = (Person) remotedata.next();
            System.out.println("Remote:" + p + ", "  + "UUID:" + desktop.ext().getObjectInfo(p).getUUID().getSignaturePart() +
                    "." + desktop.ext().getObjectInfo(p).getUUID().getLongPart() +
                    ", version:" + desktop.ext().getObjectInfo(p).getVersion());
        }
        handheld.close();
        desktop.close();
    }
    
}
