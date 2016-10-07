package com.db4o.dg2db4o.chapter10;

import com.db4o.*;
import java.io.*;

public class BackupDemo {
    
    public static void main(String[] args){
        // set up some logging to demo this feature also
        Db4o.configure().messageLevel(3);
        Db4o.configure().setOut(System.out);
        //Db4o.configure().setOut(new PrintStream(new File("log.txt"));
        
        new File("C:/primary.yap").delete();
        new File("C:/secondary.yap").delete();
        ObjectContainer db = Db4o.openFile("C:/primary.yap");
        db.set(new Person("Stefan", 39));
        
        db.commit();
        try {
            db.ext().backup("C:/secondary.yap");
        } catch (Exception e) {
            e.printStackTrace();
            db.close();
        }
        db.close();
        System.out.println("Done backup");
    }
    
}
