
package com.db4o.dg2db4o.chapter10;

import com.db4o.*;
import com.db4o.types.Blob;
import java.io.File;

public class PicBlobWrapper {
    Blob blob;
    
    public static void main(String[] arguments) throws Exception {
        
        // write a blob file to db
        ObjectContainer con = Db4o.openFile("blobtest.yap");
        PicBlobWrapper picWrapper = new PicBlobWrapper();
        con.set(picWrapper);
        picWrapper.blob.readFrom(new File("test.png"));
        con.close();
        System.out.println("stored...");
        
        // read the blob file
        con = Db4o.openFile("blobtest.yap");
        ObjectSet set = con.get(new PicBlobWrapper());
        PicBlobWrapper picWrapperOut = (PicBlobWrapper) set.next();
        picWrapperOut.blob.writeTo(new File("testOut.png"));
        con.close();
        System.out.println("done...");
    }
    
    
}
