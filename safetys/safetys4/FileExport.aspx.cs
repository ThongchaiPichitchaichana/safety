using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class FileExport : System.Web.UI.Page
    {
        string id = "";
        string ReportType = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            id = Request.QueryString["id"];


            if (!IsPostBack)
            {
               ReportDocument cryRpt;
               cryRpt = new ReportDocument();
               cryRpt.Load(Server.MapPath("~/IncidentExport.rpt"));
               CrystalReportViewer1.ReportSource = cryRpt;
               TextObject to = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["ReportName"]);
                
               
               to.Text = "Hello World!!!";
 
                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                string path_write = string.Format("{0}" + "\\safetys4\\safetys4\\Export\\IncidentReport.pdf", Server.MapPath(@"\"));

                CrDiskFileDestinationOptions.DiskFileName = path_write;
                CrExportOptions = cryRpt.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }
                cryRpt.Export();


                string path_zip = string.Format("{0}" + "\\safetys4\\safetys4\\Export\\Incident.zip", Server.MapPath(@"\"));

                using (FileStream zipFileToOpen = new FileStream(path_zip, FileMode.OpenOrCreate))
                {

                    using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))
                    {
                        archive.CreateEntryFromFile(path_write, "IncidentReport.pdf");
                       // archive.CreateEntryFromFile(@"D:\Example\file2.pdf", "file2.pdf");
                    }
                }

               
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "filename=" + "filedownload.zip");
                Response.TransmitFile(path_zip);
            
            }

        }
    }
}