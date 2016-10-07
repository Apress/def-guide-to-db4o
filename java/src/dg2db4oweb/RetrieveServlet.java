/*
 * RetrieveServlet.java
 *
 * Created on 09 February 2006, 09:16
 */

package dg2db4oweb;

import java.io.*;
import java.net.*;

import javax.servlet.*;
import javax.servlet.http.*;

import com.db4o.*;

/**
 *
 * @author jpa1
 * @version
 */
public class RetrieveServlet extends HttpServlet {
    
    /** Processes requests for both HTTP <code>GET</code> and <code>POST</code> methods.
     * @param request servlet request
     * @param response servlet response
     */
    protected void processRequest(HttpServletRequest request, HttpServletResponse response)
    throws ServletException, IOException {
        response.setContentType("text/html;charset=UTF-8");
        PrintWriter out = response.getWriter();
        
        ServletContext context = getServletContext();
        ObjectServer server=(ObjectServer)context.getAttribute("db4oServer");
        ObjectContainer db = server.openClient();
        
        ObjectSet results = db.get(new Person());
        
        
        
        out.println("<html>");
        out.println("<head>");
        out.println("<title>Servlet RetrieveServlet</title>");
        out.println("</head>");
        out.println("<body>");
        out.println("<h1>Person objects retrieved</h1>");
        while (results.hasNext()){
            Person person = (Person)results.next();
            String name = person.getName();
            int age = person.getAge();
            out.println("<p>Name: " + name + "<br/>");
            out.println("Age: " + age + "</p>");
        }
        out.println("<a href=\"/dg2db4oweb/index.jsp\">Click here to store another Person</a>");
        out.println("</body>");
        out.println("</html>");
        db.close();
        out.close();
        
    }
    
    // <editor-fold defaultstate="collapsed" desc="HttpServlet methods. Click on the + sign on the left to edit the code.">
    /** Handles the HTTP <code>GET</code> method.
     * @param request servlet request
     * @param response servlet response
     */
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
    throws ServletException, IOException {
        processRequest(request, response);
    }
    
    /** Handles the HTTP <code>POST</code> method.
     * @param request servlet request
     * @param response servlet response
     */
    protected void doPost(HttpServletRequest request, HttpServletResponse response)
    throws ServletException, IOException {
        processRequest(request, response);
    }
    
    /** Returns a short description of the servlet.
     */
    public String getServletInfo() {
        return "Short description";
    }
    // </editor-fold>
}
