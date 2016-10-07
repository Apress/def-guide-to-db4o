
package com.db4o.dg2db4o.chapter11;

import org.hsqldb.Server;

public class StartHsqlServer
{
    public static void main(String[] args)
    {
        String serverProps;
        String url;
        String user = "sa";
        String password = "";
        Server server;
        try
        {
            serverProps = "database.0=file:c:/HIB/replicate";
            url = "jdbc:hsqldb:hsql://localhost";
            server = new Server();
            server.putPropertiesFromString(serverProps);
            server.setLogWriter(null);
            server.setErrWriter(null);
            server.start();
        }
        catch (Exception e)
        {
            System.out.println("Error starting server: " +
                    e.toString());
        }
    }
}
