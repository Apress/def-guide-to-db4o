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

public partial class Store : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Store_Object(object sender, EventArgs e)
    {
        //ObjectServer server = (ObjectServer)Application["db4oServer"];
        //ObjectContainer db = server.OpenClient();
        ObjectContainer db = Db4oHttpModule.Server.OpenClient();
        //ObjectContainer db = Db4oHttpModule.Client;
        string name = TextBox1.Text;
        int age = Convert.ToInt32(TextBox2.Text);
        Person p1 = new Person(name,age);
        db.Set(p1);
        db.Close();
        Label2.Visible = false;
        Label3.Visible = false;
        TextBox1.Visible = false;
        TextBox2.Visible = false;
        Button1.Visible = false;
        Label1.Text = "Person object stored";
    }

}
