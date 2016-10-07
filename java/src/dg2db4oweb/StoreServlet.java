/*
 * StoreServlet.java
 *
 * Created on 09 February 2006, 09:10
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
public class StoreServlet extends HttpServlet {
    
    /** Processes requests for both HTTP <code>GET</code> and <code>POST</code> methods.
     * @param request servlet request
     * @param response servlet response
     */
    protected void processRequest(HttpServletRequest request, HttpServletResponse response)
    throws ServletException, IOException {
        response.setContentType("text/html;charset=UTF-8");
        PrintWriter out = response.getWriter();
        
        String name = request.getParameter("name");
        int age= Integer.parseInt(request.getParameter("age"));
        
        ServletContext context = getServletContext();
        ObjectServer server=(ObjectServer)context.getAttribute("db4oServer"); 
        ObjectContainer db = server.openClient();
        
        Person p1= new Person(name,age);
        db.set(p1);
        db.close();
        
        out.println("<html>");
        out.println("<head>");
        out.println("<title>Servlet StoreServlet</title>");
        out.println("</head>");
        out.println("<body>");
        out.println("<h1>Person object stored</h1>");
        out.println("<a href=\"/dg2db4oweb/RetrieveServlet\">Click here to retrieve</a>");
        out.println("</body>");
        out.println("</html>");

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
