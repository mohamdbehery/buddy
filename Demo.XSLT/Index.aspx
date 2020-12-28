<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="XSLT.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function loadXMLDoc(filename) {
            var xhttp;
            if (window.ActiveXObject) {
                xhttp = new ActiveXObject("Msxml2.XMLHTTP");
            }
            else {
                xhttp = new XMLHttpRequest();
            }
            xhttp.open("GET", filename, false);
            try { xhttp.responseType = "msxml-document" } catch (err) { } // Helping IE11
            xhttp.send("");
            return xhttp.responseXML;
        }

        function displayResult() {
            var xml = loadXMLDoc("CatalogData.xml");
            var xsl = loadXMLDoc("CatalogTemplate.xslt");
            // code for IE
            if (window.ActiveXObject || xml.responseType == "msxml-document") {
                var ex = xml.transformNode(xsl);
                document.getElementById("container").innerHTML = ex;
            }
            // code for Chrome, Firefox, Opera, etc.
            else if (document.implementation && document.implementation.createDocument) {
                var xsltProcessor = new XSLTProcessor();
                xsltProcessor.importStylesheet(xsl);
                var resultDocument = xsltProcessor.transformToFragment(xml, document);
                document.getElementById("container").appendChild(resultDocument);
            }
        }
    </script>
</head>
<body onload="displayResult()">
    <form id="form1" runat="server">
        <div id="container"></div>
    </form>
</body>
</html>
