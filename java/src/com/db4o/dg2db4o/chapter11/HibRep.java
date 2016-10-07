
package com.db4o.dg2db4o.chapter11;

import com.db4o.Db4o;
import com.db4o.ObjectContainer;
import com.db4o.ObjectSet;
import com.db4o.ext.ExtDb4o;
import com.db4o.replication.Replication;
import com.db4o.replication.ReplicationSession;
import org.hibernate.cfg.Configuration;

import java.io.File;
import java.io.IOException;

import java.sql.*;

public class HibRep
{
    private static final String handheld_DB = "c:/local_db.yap ";

    public static void main(String[] args)
    {
       new HibRep().run();
    }

    private void run(){
        Db4o.configure().generateUUIDs(Integer.MAX_VALUE);
        Db4o.configure().generateVersionNumbers(Integer.MAX_VALUE);
        new File(handheld_DB).delete();
        ObjectContainer handheld = Db4o.openFile(handheld_DB);
        handheld.set(new Person("Ghandi", 79));
        handheld.set(new Person("Lincoln", 56));
        handheld.commit();

        Configuration hibernate = new Configuration().configure("com/db4o/dg2db4o/chapter11/hibernate.cfg.xml");
        ReplicationSession replication = Replication.begin(handheld, hibernate);

        //Query for objects changed from db4o
        ObjectSet changed =
           replication.providerA().objectsChangedSinceLastReplication();
        while (changed.hasNext())
        {
            Person p = (Person) changed.next();
            replication.replicate(p);
        }
        replication.commit();
        replication.close();
        handheld.close();
   }
}
