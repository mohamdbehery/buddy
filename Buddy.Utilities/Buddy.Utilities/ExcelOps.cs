using System;
using System.Linq;
using System.Data;
using System.IO;

namespace Buddy.Utilities
{
    //public sealed class AdvDataTable : DataTable
    //{
    //    public AdvDataTable()
    //    {
    //    }

    //    public AdvDataTable(string tableName) : base(tableName)
    //    {
    //    }
    //}

    //public class ExcelHelper
    //{


    //    public AdvDataTable PopulateDataTableHeader(AdvDataTable tableToPopulate, DataTable headerRow)
    //    {

    //        for (var i = 0; i < headerRow.Rows.Count; i++)
    //        {
    //            //ignore blank columns
    //            var columnname = headerRow.Rows[i]["COLUMN_NAME"].ToString();
    //            var ordinal = headerRow.Rows[i]["ORDINAL_POSITION"].ToString();

    //            if (columnname != "F" + ordinal)
    //            {

    //                tableToPopulate.Columns.Add(columnname);
    //            }

    //        }
    //        return tableToPopulate;

    //    }


    //    /// <summary>
    //    /// Generate Excel File From Datatable and Return Saved File Path
    //    /// </summary>
    //    /// <param name="dtRecords"></param>
    //    /// <param name="excelFilePath"></param>
    //    /// <returns></returns>
    //    public string GenerateExcelFileFromDataTable(DataTable dtRecords, string excelFilePath)
    //    {
    //        System.Web.UI.WebControls.DataGrid dgd = new System.Web.UI.WebControls.DataGrid();
    //        StringWriter oStringWriter = new StringWriter();
    //        HtmlTextWriter oHtmlTextWriter = new HtmlTextWriter(oStringWriter);
    //        StreamWriter objStreamWriter = File.AppendText(excelFilePath);

    //        for (int report = 0; report < 1; report++)
    //        {
    //            dgd.DataSource = dtRecords;
    //            dgd.DataBind();

    //            if (report != 0)
    //            {
    //                oHtmlTextWriter.WriteBreak();
    //            }
    //            dgd.RenderControl(oHtmlTextWriter);
    //        }

    //        objStreamWriter.WriteLine(oStringWriter.ToString());
    //        objStreamWriter.Close();
    //        oHtmlTextWriter.Close();
    //        oStringWriter.Close();
    //        return excelFilePath;

    //    }


    //}

    //public class ExcelOps
    //{
    //    private string _serverFileName;
    //    private OleDbConnection _connection;
    //    int _tableIndex;
    //    ExcelHelper _excelHelper = new ExcelHelper();

    //    public ExcelOps()
    //    {
    //    }

    //    protected OleDbConnection Connection
    //    {
    //        get
    //        {
    //            if (_connection == null)
    //            {
    //                if (_serverFileName == null)
    //                    throw new Exception("A file must be uploaded before a connection can be made.  ServerFileName is null.");
    //                _connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + _serverFileName + ";Extended Properties=\"Excel 12.0;IMEX=1;\"");
    //            }
    //            return (_connection);
    //        }
    //    }

    //    private DataTable _schema;

    //    public DataTable Schema
    //    {
    //        get
    //        {
    //            if (_schema == null)
    //            {
    //                try
    //                {
    //                    Connection.Open();
    //                    _schema = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
    //                    Connection.Close();
    //                }
    //                catch (OleDbException err)
    //                {
    //                    throw new Exception("File '" + _serverFileName + "' does not appear to be a valid Excel spreadsheet.", err);
    //                }
    //            }
    //            return (_schema);
    //        }
    //    }

    //    private DataTable _columnSchema;

    //    protected DataTable ColumnSchema
    //    {
    //        get
    //        {
    //            if (_columnSchema == null)
    //            {
    //                try
    //                {

    //                    var restrictions = new[] { null, null, SheetName, null };
    //                    Connection.Open();
    //                    _columnSchema = Connection.GetSchema("Columns", restrictions);
    //                    Connection.Close();
    //                }
    //                catch (OleDbException err)
    //                {
    //                    throw new Exception("File '" + _serverFileName + "' does not appear to be a valid Excel spreadsheet.", err);
    //                }
    //            }
    //            return (_columnSchema);
    //        }
    //    }

    //    private string _sheetName;
    //    public string SheetName
    //    {
    //        get
    //        {
    //            if (_sheetName == null)
    //            {
    //                var sheet = from i in Schema.AsEnumerable()
    //                                //where (!i.Field<string>("TABLE_NAME").EndsWith("FilterDatabase"))
    //                            where i.Field<string>("TABLE_NAME").EndsWith("$'") || i.Field<string>("TABLE_NAME").EndsWith("$")
    //                            select i.Field<string>("TABLE_NAME");
    //                string[] strSheets = sheet.ToArray();
    //                _sheetName = strSheets[_tableIndex];
    //            }
    //            return _sheetName;
    //        }
    //    }

    //    //TO GET THE COUNT OF SHEETS IN THE INPUT FILE
    //    public string[] GetCountOfSheets(string filePath)
    //    {
    //        _serverFileName = filePath;
    //        var sheet = from i in Schema.AsEnumerable()
    //                        //where (!i.Field<string>("TABLE_NAME").EndsWith("FilterDatabase"))
    //                    where i.Field<string>("TABLE_NAME").EndsWith("$'") || i.Field<string>("TABLE_NAME").EndsWith("$")
    //                    select i.Field<string>("TABLE_NAME");
    //        return sheet.ToArray();
    //    }

    //    public AdvDataTable SourceToDataTable(string filePath, bool addCustomColumns)
    //    {
    //        _serverFileName = filePath;
    //        string tableName = SheetName;
    //        var dtFileData = new AdvDataTable(tableName);
    //        try
    //        {
    //            if (!addCustomColumns)
    //            {
    //                if (!tableName.Contains("$"))
    //                    tableName += "$";
    //                var adapter = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", Connection);
    //                adapter.Fill(dtFileData);
    //            }
    //        }

    //        catch (Exception err)
    //        {
    //            throw new Exception("Error while conversion {0}" + err.Message);
    //        }
    //        finally
    //        {
    //            if (Connection.State == ConnectionState.Open)
    //                Connection.Close();
    //        }
    //        return dtFileData;

    //    }

    //    public AdvDataTable SourceToDataTable(string filePath, bool addCustomColumns, string tableName)
    //    {
    //        _serverFileName = filePath;

    //        var dtFileData = new AdvDataTable(tableName);
    //        try
    //        {
    //            if (!addCustomColumns)
    //            {
    //                if (!tableName.Contains("$"))
    //                    tableName += "$";
    //                var adapter = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", Connection);
    //                adapter.Fill(dtFileData);
    //            }
    //        }

    //        catch (Exception err)
    //        {
    //            throw new Exception("Error while conversion {0}" + err.Message);
    //        }
    //        finally
    //        {
    //            if (Connection.State == ConnectionState.Open)
    //                Connection.Close();
    //        }
    //        return dtFileData;

    //    }

    //    /// <summary>
    //    /// This method reads Data from Excel and Convert to Datable using  ExcelDataReader
    //    /// Lightweight and fast library written in C# for reading Microsoft Excel files (2.0-2007).
    //    /// </summary>
    //    /// <param name="filePath"></param>
    //    /// <param name="addCustomColumns"></param>
    //    /// <returns></returns>
    //    public DataTable ExcelToDatatableUsingExcelDataReader(string filePath, bool addCustomColumns)
    //    {

    //        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
    //        IExcelDataReader excelReader;

    //        //1. Reading Excel file
    //        if (Path.GetExtension(filePath).ToUpper() == ".XLS")
    //        {
    //            //1.1 Reading from a binary Excel file ('97-2003 format; *.xls)
    //            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
    //        }
    //        else
    //        {
    //            //1.2 Reading from a OpenXml Excel file (2007 format; *.xlsx)
    //            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    //        }

    //        //3. DataSet - Create column names from first row            

    //        var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
    //        {
    //            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
    //            {
    //                UseHeaderRow = true
    //            }
    //        });


    //        ////5. Data Reader methods
    //        //while (excelReader.Read())
    //        //{
    //        ////excelReader.GetInt32(0);
    //        //}

    //        //6. Free resources (IExcelDataReader is IDisposable)
    //        excelReader.Close();
    //        if (result.Tables.Count > 0)
    //            return result.Tables[0];
    //        else
    //            return new DataTable();

    //    }


    //    public AdvDataTable SourceToDataTable(string filePath, bool addCustomColumns, int startPosition, int maxrecords, int tableIndex)
    //    {
    //        _sheetName = null;
    //        return SourceToDataTable(filePath, true, addCustomColumns, startPosition, maxrecords, tableIndex);
    //    }



    //    public AdvDataTable SourceToDataTable(string filePath, bool fileIncludesHeader, bool addCustomColumns)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    //TODO :reade data row in batches
    //    public AdvDataTable SourceToDataTable(string filePath, bool fileIncludesHeader, bool addCustomColumns, int startPosition, int maxrecords, int tableindex)
    //    {
    //        _tableIndex = tableindex;
    //        _serverFileName = filePath;
    //        var excelDataTable = new AdvDataTable("ExcelData");
    //        if (fileIncludesHeader && startPosition == 1 && maxrecords == 1)
    //            excelDataTable = _excelHelper.PopulateDataTableHeader(excelDataTable, ColumnSchema);
    //        else
    //            excelDataTable = ReadDataRow(tableindex, startPosition, maxrecords);

    //        return excelDataTable;

    //    }


    //    public AdvDataTable ReadDataRow(int tableIndex, int startPosition, int maxRecords)
    //    {
    //        string tableName = SheetName;
    //        if (!tableName.Contains("$"))
    //            tableName += "$";

    //        return ReadDataRow("ImportItem", "SELECT * FROM [" + tableName + "]", startPosition, maxRecords);

    //    }

    //    private AdvDataTable ReadDataRow(string tableName, string statement, int startPosition, int maxRecords)
    //    {
    //        var dtFileData = new AdvDataTable(tableName);
    //        var adapter = new OleDbDataAdapter(statement, Connection);
    //        adapter.Fill(startPosition, maxRecords, dtFileData);
    //        return dtFileData;

    //    }

    //    public void CleanUp()
    //    {


    //        _columnSchema = null;
    //        _sheetName = null;
    //        _schema = null;
    //    }
    //}
}
