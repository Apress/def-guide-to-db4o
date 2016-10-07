
package com.db4o.dg2db4o.chapter5;

import java.io.File;
import com.db4o.Db4o;
import com.db4o.ObjectContainer;
import com.db4o.ObjectSet;

public class CompleteExample {
    public static void main(String[] args) {
        new File("C:/complete.yap").delete();
        Db4o.configure().messageLevel(0); // 0=silent, 4=loud
        ObjectContainer db = Db4o.openFile("C:/complete.yap");
        try {
            db.set(new Person("Gandhi", 79));
            db.set(new Person("Lincoln", 56));
            ObjectSet result = (ObjectSet) db.get(new Person());
            listResult(result); // get all
            
            Person p = new Person();
            p.setName("Gandhi");
            ObjectSet result2 = (ObjectSet) db.get(p);
            Person p2 = (Person) result2.next();
            p2.setAge(90); // Increase Gandhi's age 
            db.set(p2);
            result2.reset(); // reset the ObjectSet cursor before the first element
            listResult(result2);
            
            db.delete(p2); // Remove the Ghandi object
            ObjectSet result3 = (ObjectSet) db.get(new Person());
            listResult(result3); // get all
        } finally {
            db.close();
        }
    }
    
    public static void listResult(ObjectSet result) {
        while (result.hasNext()) {
            System.out.println(result.next());
        }
        System.out.println("---------------");
    }
}
