using Buddy.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static Buddy.Utilities.Enums.HelperEnums;

namespace Buddy.Utilities.Models
{
    public class ExecResult
    {
        public ExecResult()
        {
            ErrorCode = Constants.DefaultErrorCode;
            Success = true;
        }
        public bool Success { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorException { get; set; }
        public string ExecutionMessages { get; set; }
        public DataSet ResultSet { get; set; }
        public string ResultField { get; set; }
        public int AffectedRowsCount { get; set; }

        public XmlDocument SerializeToXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xsData = new XmlSerializer(typeof(ExecResult));
            var returnedXML = "";

            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
                {
                    xsData.Serialize(xmlWriter, this);
                    returnedXML = stringWriter.ToString(); // Your XML
                }
            }
            xmlDoc.LoadXml(returnedXML);
            return xmlDoc;
        }
    }
}
