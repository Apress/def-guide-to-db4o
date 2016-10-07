
package com.db4o.dg2db4o.chapter10;

import com.db4o.tools.*;

public class ToolsDemo {
    
    public static void main(String[] args){
        new Statistics().run("C:/complete.yap");
        new Defragment().run("C:/complete.yap", true);
    }
    
}
