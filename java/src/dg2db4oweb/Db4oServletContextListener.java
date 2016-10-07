
package dg2db4oweb;

import javax.servlet.*;
import javax.servlet.http.*;
import com.db4o.*;
import java.io.File;

public class Db4oServletContextListener
        implements ServletContextListener {
    public static final String KEY_DB4O_FILE_NAME = "db4oFileName";
    public static final String KEY_DB4O_SERVER = "db4oServer";
    private ObjectServer server=null;
    
    public void contextInitialized(ServletContextEvent event) {
        close();
        ServletContext context=event.getServletContext();
        String filePath=context.getRealPath(
                "WEB-INF/db/"+context.getInitParameter(KEY_DB4O_FILE_NAME));
        new File(filePath).delete();
        server=Db4o.openServer(filePath,0);
        context.setAttribute(KEY_DB4O_SERVER,server);
        context.log("db4o startup on "+filePath);
    }
    
    public void contextDestroyed(ServletContextEvent event) {
        ServletContext context = event.getServletContext();
        context.removeAttribute(KEY_DB4O_SERVER);
        close();
        context.log("db4o shutdown");
    }
    
    private void close() {
        if(server!=null) {
            server.close();
        }
        server=null;
    }
}
