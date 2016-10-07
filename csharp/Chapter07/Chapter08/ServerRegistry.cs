using System;
using System.Collections;
using System.IO;
using com.db4o;

namespace com.db4o.dg2db4o.chapter8
{
    class ServerRegistry
    {
        private static ServerRegistry theInstance;

        public static ServerRegistry GetInstance()
        {
            if (theInstance == null)
                theInstance = new ServerRegistry();

            return theInstance;
        }

        private Hashtable servers;

        public ServerRegistry()
        {
            servers = new Hashtable();
        }

        public ObjectServer RegisterServer(String filename, String id)
        {
            lock (this)
            {
                File.Delete(filename); // to be omitted if db is new
                ObjectServer server = Db4o.OpenServer(filename, 0);
                servers.Add(id, server);
                return server;
            }
        }

        public ObjectServer GetServer(String id)
        {
            lock (this)
            {
                return (ObjectServer)servers[id];
            }
        }

        public void stopServer(String id)
        {
            lock (this)
            {
                ObjectServer server = (ObjectServer)servers[id];
                if (server != null)
                    server.Close();
            }
        }
    }
}
