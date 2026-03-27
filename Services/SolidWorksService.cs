using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SolidConvert_Pro.Services
{
    public class SolidWorksService : IDisposable
    {
        private SldWorks _swApp;
        private bool _disposed = false;

        public async Task<SldWorks> ConnectAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Try to get an active instance of SolidWorks
                    _swApp = (SldWorks)Marshal.GetActiveObject("sldworks.Application");
                }
                catch (COMException)
                {
                    // If no active instance, create a new instance of SolidWorks
                    _swApp = new SldWorks();
                    _swApp.Visible = true;
                }

                // Set SolidWorks preferences
                _swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfMultiSheetOption, (int)swDxfMultisheet_e.swDxfActiveSheetOnly);
                _swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfVersion, (int)swDxfFormat_e.swDxfFormat_R2000);
                _swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputFonts, 1);
                _swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputLineStyles, 1);

                return _swApp;
            });
        }

        public async Task<ConversionResult> ConvertDrawingAsync(SldWorks swApp, string filePath, string outputFolder, bool exportPdf, bool exportDwg)
        {
            return await Task.Run(() =>
            {
                try
                {
                    ModelDoc2 swDoc = swApp.OpenDoc6(filePath, (int)swDocumentTypes_e.swDocDRAWING,
                        (int)swOpenDocOptions_e.swOpenDocOptions_Silent, string.Empty, 0, 0);

                    if (swDoc == null)
                        return new ConversionResult { Success = false, ErrorMessage = "Failed to open document" };

                    // Get revision
                    CustomPropertyManager swCustProp = swDoc.Extension.CustomPropertyManager[""];
                    string revision = swCustProp.Get("Revision");
                    if (string.IsNullOrEmpty(revision))
                        revision = "R0";

                    string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    bool pdfSuccess = true;
                    bool dwgSuccess = true;
                    string errorMsg = string.Empty;

                    // Export to PDF
                    if (exportPdf)
                    {
                        try
                        {
                            IExportPdfData exportPdfData = (IExportPdfData)swApp.GetExportFileData(1);
                            exportPdfData.ViewPdfAfterSaving = false;
                            string newFileName = System.IO.Path.Combine(outputFolder, $"{fileName}_{revision}.pdf");
                            swDoc.Extension.SaveAs(newFileName, 0, 0, exportPdfData, 0, 0);
                        }
                        catch (Exception ex)
                        {
                            pdfSuccess = false;
                            errorMsg = $"PDF export failed: {ex.Message}";
                        }
                    }

                    // Export to DWG
                    if (exportDwg)
                    {
                        try
                        {
                            DrawingDoc swDraw = (DrawingDoc)swDoc;
                            if (swDraw.GetSheetCount() == 1)
                            {
                                string newFileName = System.IO.Path.Combine(outputFolder, $"{fileName}_{revision}.dwg");
                                swDoc.SaveAs(newFileName);
                            }
                            else if (swDraw.GetSheetCount() > 1)
                            {
                                string[] vSheet = (string[])swDraw.GetSheetNames();
                                for (int i = 0; i < vSheet.Length; i++)
                                {
                                    swDraw.ActivateSheet(vSheet[i]);
                                    string newFileName = System.IO.Path.Combine(outputFolder, $"{fileName}_{revision}-{i + 1}.dwg");
                                    swDoc.SaveAs(newFileName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dwgSuccess = false;
                            if (!string.IsNullOrEmpty(errorMsg))
                                errorMsg += "; ";
                            errorMsg += $"DWG export failed: {ex.Message}";
                        }
                    }

                    swApp.QuitDoc(filePath);

                    bool overallSuccess = (!exportPdf || pdfSuccess) && (!exportDwg || dwgSuccess);
                    return new ConversionResult
                    {
                        Success = overallSuccess,
                        ErrorMessage = overallSuccess ? string.Empty : errorMsg
                    };
                }
                catch (Exception ex)
                {
                    return new ConversionResult { Success = false, ErrorMessage = ex.Message };
                }
            });
        }

        public SldWorks GetApplication()
        {
            return _swApp;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Note: We don't dispose of COM objects here as they may still be in use
                    // The application should handle cleanup when closing
                    _swApp = null;
                }
                _disposed = true;
            }
        }
    }

    public class ConversionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}

