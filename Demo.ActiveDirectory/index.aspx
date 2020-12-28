<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="ActiveDirectory.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblStatus" runat="server"></asp:Label>
        <br />
        <br />
        <asp:TextBox ID="txtUN" runat="server"></asp:TextBox>
        <br />
        <asp:TextBox ID="txtPass" TextMode="Password" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnLogin" runat="server" Text="LOG IN" OnClick="btnLogin_Click" />
    </div>
    </form>
</body>
</html>
