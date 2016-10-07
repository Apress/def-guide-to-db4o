<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Store.aspx.cs" Inherits="Store" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Store Person</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>
                <asp:Label ID="Label1" runat="server" Text="Store Person object"></asp:Label>&nbsp;</h1>
            <p>
                <asp:Label ID="Label2" runat="server" Text="Name: "></asp:Label><asp:TextBox ID="TextBox1"
                    runat="server"></asp:TextBox></p>
            <p>
                <asp:Label ID="Label3" runat="server" Text="Age: "></asp:Label><asp:TextBox ID="TextBox2"
                    runat="server"></asp:TextBox></p>
            <p>
                &nbsp;<input type="button" value="store" onserverclick="Store_Object" id="Button1"
                    runat="server"></p>
            <p>
                <a href="/db2db4oweb_better/Retrieve.aspx">Retrieve stored objects</a></p>
        </div>
    </form>
</body>
</html>
