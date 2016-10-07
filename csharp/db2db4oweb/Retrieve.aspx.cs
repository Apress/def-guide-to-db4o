using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using com.db4o;

public partial class Retrieve : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //ObjectServer server = (ObjectServer)Application["db4oServer"];
        //ObjectContainer db = server.OpenClient();
        ObjectContainer db = Db4oHttpModule.Client;

        ObjectSet results = db.Get(new Person());

        GridView1.DataSource = results;
        GridView1.DataBind();
        
        db.Close();

    }
}
