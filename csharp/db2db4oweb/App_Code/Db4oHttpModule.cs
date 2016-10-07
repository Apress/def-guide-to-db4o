using System;
using System.Configuration;
using System.Web;
using com.db4o;

public class Db4oHttpModule : IHttpModule
{
    private static readonly string KEY_DB4O_FILE_NAME = "db4oFileName";
    private static readonly string KEY_DB4O_CLIENT = "db4oClient";
    private static ObjectServer objectServer = null;

    public void Init(HttpApplication application)
    {
        application.EndRequest += new EventHandler(Application_EndRequest);
    }

    public static ObjectContainer Client
    {
        get
        {
            HttpContext context = HttpContext.Current;
            ObjectContainer objectClient = context.Items[KEY_DB4O_CLIENT] as ObjectContainer;
            if (objectClient == null)
            {
                objectClient = Server.OpenClient();
                context.Items[KEY_DB4O_CLIENT] = objectClient;
                OnClientOpened();
            }
            return objectClient;
        }
    }

    public static ObjectServer Server
    {
        get
        {
            HttpContext context = HttpContext.Current;
            if (objectServer == null)
            {
                string yapFilePath = context.Server.MapPath(ConfigurationSettings.AppSettings[KEY_DB4O_FILE_NAME]);
                objectServer = Db4o.OpenServer(yapFilePath, 8732);
                OnServerOpened();
            }
            return objectServer;
        }
    }


    private void Application_EndRequest(object sender, EventArgs e)
    {
        HttpApplication application = (HttpApplication)sender;
        HttpContext context = application.Context;

        ObjectContainer objectClient = context.Items[KEY_DB4O_CLIENT] as ObjectContainer;
        if (objectClient != null)
        {
            objectClient.Close();
            objectClient = null;
            context.Items[KEY_DB4O_CLIENT] = null;
            OnClientClosed();
        }
    }

    public void Dispose()
    {
        if (objectServer != null)
        {
            objectServer.Close();
            objectServer = null;
            OnServerClosed();
        }
    }

    public static string HashCodes
    {
        get { return "Server HashCode: " + Server.GetHashCode() + " Client HashCode: " + Client.GetHashCode(); }
    }

    #region Events
    public delegate void ClientOpenedEventHandler(object sender, EventArgs e);
    private static ClientOpenedEventHandler _clientOpenedEventHandler = null;

    public event ClientOpenedEventHandler ClientOpened
    {
        add { _clientOpenedEventHandler += value; }
        remove { _clientOpenedEventHandler -= value; }
    }

    public delegate void ServerOpenedEventHandler(object sender, EventArgs e);
    private static ServerOpenedEventHandler _serverOpenedEventHandler = null;

    public event ServerOpenedEventHandler ServerOpened
    {
        add { _serverOpenedEventHandler += value; }
        remove { _serverOpenedEventHandler -= value; }
    }

    public delegate void ClientClosedEventHandler(object sender, EventArgs e);
    private static ClientClosedEventHandler _clientClosedEventHandler = null;

    public event ClientClosedEventHandler ClientClosed
    {
        add { _clientClosedEventHandler += value; }
        remove { _clientClosedEventHandler -= value; }
    }

    public delegate void ServerClosedEventHandler(object sender, EventArgs e);
    private static ServerClosedEventHandler _serverClosedEventHandler = null;

    public event ServerClosedEventHandler ServerClosed
    {
        add { _serverClosedEventHandler += value; }
        remove { _serverClosedEventHandler -= value; }
    }
    #endregion

    #region Event raising
    protected static void OnClientClosed()
    {
        if (_clientClosedEventHandler != null)
            _clientClosedEventHandler(typeof(Db4oHttpModule), null);
    }

    protected static void OnClientOpened()
    {
        if (_clientOpenedEventHandler != null)
            _clientOpenedEventHandler(typeof(Db4oHttpModule), null);
    }

    protected static void OnServerOpened()
    {
        if (_serverOpenedEventHandler != null)
            _serverOpenedEventHandler(typeof(Db4oHttpModule), null);
    }

    protected static void OnServerClosed()
    {
        if (_serverClosedEventHandler != null)
            _serverClosedEventHandler(typeof(Db4oHttpModule), null);
    }
    #endregion
}
