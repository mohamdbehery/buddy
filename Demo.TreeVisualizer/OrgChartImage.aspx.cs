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
using System.Drawing.Imaging;

public partial class Homepage_BOM_OrgChartImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Change the response headers to output a JPEG image.
        this.Response.Clear();
        this.Response.ContentType = "image/jpeg";
        System.Drawing.Image OC = null;
        //Build the image 
        string imageName = Request.QueryString["ID"];
        if (Cache[imageName] != null)
        {
            OC = (System.Drawing.Image)Cache[imageName];
            // Write the image to the response stream in JPEG format.
            OC.Save(this.Response.OutputStream, ImageFormat.Jpeg);
            OC.Dispose();
        }
        
        
    }
}
