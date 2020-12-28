<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountStatement.aspx.cs" Inherits="ConfigEncryption.AccountStatement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <style>
html , body {
    margin:0;
    padding:0;
 }
body 
{
    background:url(../images/Encryption-bg.jpg);
    background-size:cover;
    font-family: sans-serif;
    font-size: 19px;
    position:relative;
}
body:before 
{
    content: "";
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
    bottom: 0;
    background: #53657a8a;
    z-index: 1;
    width: 100%;
    height: 100vh;
} 
* {
    box-sizing: border-box;
}
.div-ds 
{
    background: #53657a;
    padding:25px;
    width:400px;
    position:fixed;
    top:50%;
    left:50%;
    transform: translate(-50% , -50%);
    min-height:150px;
    z-index:2
}
.div-ds input[type="text"] , .div-ds input[type="password"]
{
    width: 100%;
    padding: 1px 15px;
    line-height: 42px;
    border: 1px solid #ddd;
    margin-bottom: 10px;
   text-align:center;
   font-size:20px;
}
.div-ds input[type="submit"] 
{
    display: block;
    border: 0;
    font-size: 19px;
    background: #f06f5c;
    color: #fff;
    width: 100%;
    line-height: 44px;
    cursor:pointer;
}
.div-ds input[type="submit"].btn-s:first-child {
    margin-bottom:10px;
}
.label-ds 
{
    color:#fff;
    padding-bottom: 10px;
    display: block;
    font-family: sans-serif;
    font-size: 19px;
}
</style>
<head runat="server">
    <title></title>
</head>
<body>
     <form id="form1" runat="server" >
         <asp:Label ID="lbl_Setting" runat="server" Text="Label"></asp:Label>
         <br/>
         <asp:Label ID="lbl_connectionString" runat="server" Text="Label"></asp:Label>
        <div class="div-ds Div_CodeControls" runat="Server" id="Div_CodeControls">
            <asp:Label CssClass="label-ds" ID="Label1" runat="server" Text="Enter Security Code"></asp:Label>
            <asp:TextBox ID="Txt_Code" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Button ID="Btn_Confirm" runat="server" Text="Check!!" onclick="Button1_Click" />
        </div>
        <div class="div-ds Div_EncryptionControl" runat="Server"  id="Div_EncryptionControl">
            <asp:Button CssClass="btn-s" ID="Button1" runat="server" Text="Decrypt Connections"  OnClick="DecryptConnections"/>
            <asp:Button CssClass="btn-s" ID="Button2" runat="server" style="background-color:#68c368" Text="Encrypt Connections Again"  OnClick="EncryptConnections"/>
            <br />
            <asp:Label runat="server" style="color:White; font-size:16px;" ID="Txt_Msg"></asp:Label>
        </div>
    </form>
</body>
</html>
