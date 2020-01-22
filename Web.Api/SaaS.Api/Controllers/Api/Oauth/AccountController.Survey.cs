using System.Collections.Generic;
using Newtonsoft.Json;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Api.Oauth;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using RazorEngine;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using SaaS.Data.Entities.Accounts;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        private static readonly string XlsxMediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        [HttpPost, Route("survey"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> Survey(SurveyViewModel model)
        {
            var compressedData = Compress(System.Text.Encoding.UTF8.GetBytes(model.Data.ToString(Formatting.None)));
            await _auth.SurveyAsync(AccountId, compressedData, model.Lang);
            return Ok();
        }

        [HttpGet, Route("survey/getall"), SaaSAuthorize(Roles = "admin")]
        public async Task<IHttpActionResult> GetAllSurvey()
        {
            try
            {
                return Ok(await _auth.GetAllSurveyAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

#if DEBUG

        [HttpPost, Route("survey/report")]
        public async Task<IHttpActionResult> SurveyReport(List<AccountSurvey> surveys)
        {
            try
            {
                //only for Debug lock service on stage
                if (HttpContext.Current.Request.Headers.GetValues("Accept").Any(headerValue
                    => !string.IsNullOrWhiteSpace(headerValue) &&
                    headerValue.Split(',').Any(segment => MediaTypeHeaderValue.Parse(segment).MediaType == XlsxMediaType)))
                    return ExportToExcel(surveys);
                else
                    return ExportToJson(surveys);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
#endif

        private IHttpActionResult ExportToJson(IEnumerable<AccountSurvey> data)
        {
            var decompressedSurveys = new List<string>();
            foreach (var survey in data)
            {
                var decompressedData = Decompress(survey.CollectedData);
                decompressedSurveys.Add(System.Text.Encoding.UTF8.GetString(decompressedData));
            }
            return Ok(decompressedSurveys);
        }

        private IHttpActionResult ExportToExcel(IEnumerable<AccountSurvey> data)
        {
            var sectionsTable = new List<SectionTableViewModel>();
            List<SectionTableRowViewModel> rows = new List<SectionTableRowViewModel>();

            foreach (var survey in data)
            {
                var row = new SectionTableRowViewModel(survey.AccountId, survey.CreateDate);
                try
                {
                    var decompressedData = Decompress(survey.CollectedData);
                    var surveySections = JsonConvert.DeserializeObject<IEnumerable<SurveySection>>(System.Text.Encoding.UTF8.GetString(decompressedData));

                    foreach (var surveySection in surveySections)
                    {
                        SectionTableViewModel sectionTableVM = sectionsTable.FirstOrDefault(item => item.Id == surveySection.Id);
                        if (sectionTableVM == null)
                        {
                            sectionTableVM = new SectionTableViewModel(surveySection.Id);
                            sectionsTable.Add(sectionTableVM);
                        }

                        foreach (var surveySectionGroup in surveySection.Groups)
                        {
                            SectionGroupTableViewModel sectionGroupTableVM = sectionTableVM.Groups.FirstOrDefault(g => g.Id == surveySectionGroup.Id);
                            if (sectionGroupTableVM == null)
                            {
                                sectionGroupTableVM = new SectionGroupTableViewModel(surveySectionGroup.Id);
                                sectionTableVM.Groups.Add(sectionGroupTableVM);
                            }

                            foreach (var surveySectionGroupQa in surveySectionGroup.Qa)
                            {
                                var qId = string.Format("{0}_{1}_{2}", surveySection.Id, surveySectionGroup.Id, surveySectionGroupQa.Q);
                                if (!sectionGroupTableVM.QAs.Contains(qId))
                                {
                                    sectionGroupTableVM.QAs.Add(qId);
                                }
                                row.QAs.Add(new SurveyQATable(qId, surveySectionGroupQa.Q, surveySectionGroupQa.A));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
                rows.Add(row);
            }

            using (var table = new System.Data.DataTable())
            {
                var allQas = sectionsTable.SelectMany(s => s.Groups.SelectMany(g => g.QAs));
                table.Columns.Add("DateTime UTC", typeof(string));
                table.Columns.Add("AccountId", typeof(string));
                foreach (var qaColumn in allQas)
                {
                    table.Columns.Add(qaColumn, typeof(string));
                }

                foreach (var row in rows)
                {
                    var dataRow = table.NewRow();
                    dataRow["DateTime UTC"] = row.CreateDate.ToString();
                    dataRow["AccountId"] = row.AccountId.ToString();
                    for (int i = 2; i < dataRow.Table.Columns.Count; i++)
                    {
                        DataColumn column = dataRow.Table.Columns[i];
                        var existed = row.QAs.FirstOrDefault(item => item.QId == column.ColumnName);
                        if (existed != null)
                            dataRow[column] = existed.A;
                    }

                    table.Rows.Add(dataRow);
                }
                return DataTableToExcel(table, sectionsTable);
            }
        }

        private IHttpActionResult DataTableToExcel(System.Data.DataTable dataTable, List<SectionTableViewModel> sectionsTable)
        {
            //var fileName = Path.GetTempFileName();
            string fileName = HttpContext.Current.Server.MapPath($"temp/{Guid.NewGuid()}.xlsx");
            string tempDir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            var excel = new Microsoft.Office.Interop.Excel.Application();
            var worKbooK = excel.Workbooks.Add(Type.Missing);
            try
            {
                excel.Visible = false;
                excel.DisplayAlerts = false;
                var worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;                
                worKsheeT.Name = "SurveyReport";

                int rowDataPadding = 1;
                int skipCells = 2;
                for (int i = 0; i < sectionsTable.Count; i++)
                {
                    var from = skipCells;
                    skipCells += sectionsTable[i].QaCount;
                    worKsheeT.Cells[rowDataPadding, from + 1] = $"Section {sectionsTable[i].Id}";
                    var cell = worKsheeT.Cells[rowDataPadding, from + 1];
                    cell.Font.Bold = true;
                    cell.Font.Size += 2;
                    cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    worKsheeT.Range[worKsheeT.Cells[rowDataPadding, from + 1], worKsheeT.Cells[rowDataPadding, skipCells]].Merge();
                }
                rowDataPadding++;
                skipCells = 2;

                var groups = sectionsTable.SelectMany(s => s.Groups).ToList();
                for (int i = 0; i < groups.Count; i++)
                { 
                    var from = skipCells;
                    skipCells += groups[i].QAs.Count;
                    worKsheeT.Cells[rowDataPadding, from + 1] = $"Grop {groups[i].Id}";
                    var cell = worKsheeT.Cells[rowDataPadding, from + 1];
                    cell.Font.Bold = true;
                    cell.Font.Size += 1;
                    cell.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    worKsheeT.Range[worKsheeT.Cells[rowDataPadding, from + 1], worKsheeT.Cells[rowDataPadding, skipCells]].Merge();
                }
                rowDataPadding++;
                skipCells = 0;

                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var name = dataTable.Columns[i].ColumnName;
                    var cheat = name.LastIndexOf("_");
                    if (cheat >= 0)
                    {
                        name = name.Substring(cheat+1);
                    }
                    worKsheeT.Cells[rowDataPadding, i + 1] = name;
                    var cell = worKsheeT.Cells[rowDataPadding, i + 1];
                    cell.Font.Bold = true;
                }
                rowDataPadding++;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var dataRow = dataTable.Rows[i];
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        var value = dataRow[j] as string;
                        if (value != null)
                        {
                            worKsheeT.Cells[i + 1 + rowDataPadding, j + 1] = value;
                        }
                    }
                }

                var celLrangE = worKsheeT.Range[worKsheeT.Cells[3, 1], worKsheeT.Cells[3, dataTable.Columns.Count]];
                celLrangE.EntireColumn.AutoFit();

                worKbooK.SaveAs(fileName);
            }
            finally
            {
                worKbooK.Close();
                excel.Quit();
            }

            if (File.Exists(fileName))
            {
                try
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(fileName)))
                    {
                        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(ms.ToArray());
                        result.Content.Headers.ContentLength = ms.Length;
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "survey_report.xlsx"
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue(XlsxMediaType);
                        return ResponseMessage(result);
                    }
                }
                finally
                {
                    File.Delete(fileName);
                }
            }

            return BadRequest();
        }

        private static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            {
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}