
package com.db4o.dg2db4o.chapter11;

import java.io.*;
import com.db4o.*;

public class RunServer implements Runnable, ServerConfiguration {
    private boolean stop;
    
    // Can run as standalone process, or use Main.java to run as thread
    public static void main(String[] args) {
        RunServer runner = new RunServer();
        runner.run();
    }
    
    public void run() {
        synchronized (this) {
            new File(REMOTEFILE).delete();
            ObjectServer server = Db4o.openServer(REMOTEFILE, PORT);
            server.grantAccess(USER, PASS);
            try {
                while (!stop) {  // Out of band messages here later...
                    System.out.println("SERVER:[" + System.currentTimeMillis()
                    + "] Server's running... ");
                    this.wait(60000);
                }
            } catch (Exception e) {
                e.printStackTrace();
            } finally {
                server.close();
            }
        }
    }
    
}
