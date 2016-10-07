
package com.db4o.dg2db4o.chapter8;

import com.db4o.*;
import java.io.File;
import java.util.Map;
import java.util.HashMap;

public class ServerRegistry {
    private Map servers;
    
    public ServerRegistry() {
        servers = new HashMap();
    }
    
    public synchronized ObjectServer registerServer(String filename, String id) {
        new File(filename).delete(); // to be omitted if db is new
        ObjectServer server = Db4o.openServer(filename, 0);
        servers.put(id, server);
        return server;
    }
    
    public synchronized ObjectServer getServer(String id) {
        return (ObjectServer)servers.get(id);
    }
    
    public synchronized void stopServer(String id) {
        ObjectServer server = (ObjectServer)servers.get(id);
        if (server!=null)
            server.close();
    }
    
}
