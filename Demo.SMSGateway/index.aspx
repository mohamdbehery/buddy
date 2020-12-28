<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SMSGateway.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divData" runat="server"></div>
        <br />
        <asp:TextBox ID="txtData" runat="server" Width="1200"></asp:TextBox>
        <br />
        <asp:TextBox ID="txtKey" runat="server" Width="300"></asp:TextBox>
        <br />
        <asp:Button ID="btnSendRequest" runat="server" Text="Request!" OnClick="btnSendRequest_Click" />
    </form>
</body>
</html>
