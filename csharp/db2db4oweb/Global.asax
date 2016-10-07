<%@ Application Language="C#" %>
<%@ Import Namespace="com.db4o" %>
<%@ Import Namespace="System.IO" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        Context.Response.Write("APPLICATION STARTUP");
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        Context.Response.Write("APPLICATION END");
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    protected void Db4oHttpModule_OnClientOpened(Object sender, EventArgs e)
    {
        Context.Response.Write("CLIENT OPENED");
    }



    protected void Db4oHttpModule_OnClientClosed(Object sender, EventArgs e)
    {

    }



    protected void Db4oHttpModule_OnServerOpened(Object sender, EventArgs e)
    {
        Context.Response.Write("SERVER OPENED");
    }



    protected void Db4oHttpModule_OnServerClosed(Object sender, EventArgs e)
    {

    }  
       
</script>
