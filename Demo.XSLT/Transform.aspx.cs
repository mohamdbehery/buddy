using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XSLT
{
    public partial class Transform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XPathDocument myXPathDoc = new XPathDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Report.xml"));
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Report.xslt"));
            XmlTextWriter myWriter = new XmlTextWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"NewReportOutput.xml"), null);
            myXslTrans.Transform(myXPathDoc, null, myWriter);
            myWriter.Flush();
            myWriter.Close();
        }
    }
}