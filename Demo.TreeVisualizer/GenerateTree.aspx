<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenerateTree.aspx.cs" Inherits="Demo.TreeVisualizer.GenerateTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Type in Item:"></asp:Label>&nbsp;
        <asp:TextBox ID="txtItem" runat="server"></asp:TextBox>&nbsp;
        <asp:Button ID="btnShowBOM" runat="server" OnClick="btnShowBOM_Click" Text="Show Tree" />
            <br />
            <br />
            <asp:TreeView ID="treeView1" runat="server" ImageSet="XPFileExplorer" NodeIndent="15">
                <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
                <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px"
                    NodeSpacing="0px" VerticalPadding="2px"></NodeStyle>
                <ParentNodeStyle Font-Bold="False" />
                <SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="0px"
                    VerticalPadding="0px" />
            </asp:TreeView>
        </div>
    </form>
</body>
</html>
