using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace SMSGateway
{
    public partial class index : System.Web.UI.Page
    {
        string hashStr;
        string SecureHashSecretKey;
        Dictionary<string, string> dictRequest;
        Dictionary<string, string> dictResponse;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string hashMethod(string Data, string Key)
        {
            string DataHashHex;
            var KeyBytes = Encoding.UTF8.GetBytes(Key);
            using (var SHA256 = new HMACSHA256(KeyBytes))
            {
                var DataBytes = Encoding.UTF8.GetBytes(Data);
                var DataHashBytes = SHA256.ComputeHash(DataBytes);
                DataHashHex = string.Concat(Array.ConvertAll(DataHashBytes, b => b.ToString("X2")));
            }
            return DataHashHex;
        }

        protected void btnSendRequest_Click(object sender, EventArgs e)
        {
            hashStr = txtData.Text;
            SecureHashSecretKey = txtKey.Text;
            string[] items = hashStr.Split(new[] { '&' });
            dictRequest = items.Select(item => item.Split(new[] { '=' })).ToDictionary(pair => pair[0], pair => pair[1]);
            string GeneratedHashKey = hashMethod(hashStr, SecureHashSecretKey);
            //string GeneratedHashKey = "13FA8D3CFE5E31FE0EE1614110AE75E27A29EA7B70BA7DD48350D6BCA63D7C92";
            divData.InnerHtml = GeneratedHashKey;
            string RequestData = PrepareRequest(GeneratedHashKey);
            SendRequest(RequestData);
            if (dictResponse.ContainsKey("StatusCode"))
                divData.InnerHtml += "<br /> StatusCode: " + dictResponse["StatusCode"] + "<br />";
            else
                divData.InnerHtml += "<br /> StatusCode: None";

            if (dictResponse.ContainsKey("Response"))
                divData.InnerHtml += "Response : " + dictResponse["Response"] + "<br />";
            else
                divData.InnerHtml += "<br /> Response: None";
        }

        private string PrepareRequest(string GenratedHash)
        {
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(Server.MapPath("~/SMSRequest.xml"));
            XmlNode AccountId  = XMLDoc.ChildNodes[0].ChildNodes[0];
            AccountId.InnerText = dictRequest["AccountId"];
            XmlNode Password = XMLDoc.ChildNodes[0].ChildNodes[1];
            Password.InnerText = dictRequest["Password"];
            XmlNode SecureHash = XMLDoc.ChildNodes[0].ChildNodes[2];
            SecureHash.InnerText = GenratedHash;
            XmlNode SenderName = XMLDoc.ChildNodes[0].ChildNodes[3].ChildNodes[0];
            SenderName.InnerText = dictRequest["SenderName"];
            XmlNode ReceiverMSISDN = XMLDoc.ChildNodes[0].ChildNodes[3].ChildNodes[1];
            ReceiverMSISDN.InnerText = dictRequest["ReceiverMSISDN"];
            XmlNode SMSText = XMLDoc.ChildNodes[0].ChildNodes[3].ChildNodes[2];
            SMSText.InnerText = dictRequest["SMSText"];

            return XMLDoc.OuterXml;
        }
        
        public string SendRequest(string Data)
        {
            try
            {
                HttpWebResponse RSP;
                byte[] bytes = Encoding.UTF8.GetBytes(Data);
                HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(new Uri("https://e3len.vodafone.com.eg/web2sms/sms/submit/", UriKind.Absolute));
                Req.ContentType = "application/xml";
                Req.Host = "e3len.vodafone.com.eg";
                Req.UserAgent = "Apache-HttpClient/4.1.1 (java 1.5)";
                Req.Method = "POST";
                //Req.Accept = "gzip,deflate";
                //Req.ContentLength = bytes.Length;
                //Req.KeepAlive = true;
                using (Stream ReqStream = Req.GetRequestStream())
                {
                    ReqStream.Write(bytes, 0, bytes.Length);
                    ReqStream.Close();
                }

                RSP = (HttpWebResponse)Req.GetResponse();
                dictResponse = new Dictionary<string, string>()
                {
                        { "StatusCode", RSP.StatusCode.ToString() },
                        { "Response", new StreamReader(RSP.GetResponseStream()).ReadToEnd() },
                };

                return new StreamReader(RSP.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message + "<br />" + ex.InnerException;
            }
        }
    }
}