<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Retrieve.aspx.cs" Inherits="Retrieve" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>
                Person objects retrieved</h1>
            <p>
                <asp:GridView ID="GridView1" runat="server">
                </asp:GridView>
            </p>
            <p>
                <a href="/db2db4oweb_better/Store.aspx">Store another object</a></p>
        </div>
    </form>
</body>
</html>
