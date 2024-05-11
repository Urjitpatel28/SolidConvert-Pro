using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SolidConvert_Pro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int n;
        SldWorks swApp;
        ModelDoc2 swDoc;
        DrawingDoc swDraw;
        Sheet Sheet;
        string sNewFileName;
        string folderPath;
        string rev;
        ExportPdfData exportPdfData;

        private void btnAddfiles_Click(object sender, EventArgs e)
        {
            //File selection dialogbox
            OpenFileDialog FB = new OpenFileDialog();
            FB.Title = "Please select files";
            FB.Multiselect = true;
            FB.InitialDirectory = @"Desktop";
            FB.Filter = "SolidWorks Drawing | *slddrw";
            FB.ShowDialog();

            //Add selected file to list
            string[] sFilenames = FB.FileNames;
            n = sFilenames.Length;

            string sFilename;

            for (int i = 0; i < n; i = i + 1)
            {
                sFilename = sFilenames[i];
                lstSelectedFiles.Items.Add(sFilename);
            }

            n = lstSelectedFiles.Items.Count;
            lblinfoSelected.Text = n + " files selected.";
            txtboxStatus.Text = $"0 / {n}";
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            //remove selected files from list
            var selectedItems = lstSelectedFiles.SelectedItems.Cast<object>().ToList();

            foreach (var selectedItem in selectedItems)
            {
                lstSelectedFiles.Items.Remove(selectedItem);
            }

            n = lstSelectedFiles.Items.Count;
            lblinfoSelected.Text = n + " files selected.";
            txtboxStatus.Text = $"0 / {n}";
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            //clear list
            lstSelectedFiles.Items.Clear();
            n = lstSelectedFiles.Items.Count;
            lblinfoSelected.Text = n + " files selected.";
            txtboxStatus.Text = $"0 / {n}";
            lblStatus.Text = string.Empty;
            lblUpdate.Text = string.Empty;
        }

        private void btnSelectOutFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Please select the desired folder";
            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                txtboxOutFolder.Text = folderPath;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Path validation
            if (txtboxOutFolder.Text.Length == 0)
            {
                lblStatus.Text = "Please select output folder.";
                lblStatus.ForeColor = Color.Red;
            }

            if (txtboxOutFolder.Text.Length > 0)
            {
                folderPath = txtboxOutFolder.Text;

                if (IsValidFolderPath(folderPath))
                {
                    if (lstSelectedFiles.Items.Count == 0)
                    {
                        lblStatus.Text = "Please select file to convert.";
                        lblStatus.ForeColor = Color.Red;
                    }
                    else
                    {
                        txtboxOutFolder.Text = folderPath;
                        if (checkBoxPDF.Checked || checkBoxDWG.Checked)
                        {
                            lblStatus.Text = "Starting Conversation...";
                            lblStatus.ForeColor = Color.Blue;

                            ConnectToSolidworks();

                            int i = 1;
                            foreach (var item in lstSelectedFiles.Items)
                            {

                                txtboxStatus.Text = $"{i} / {n}";
                                lblStatus.Text = "Converting...";
                                lblStatus.ForeColor = Color.Blue;
                                lblUpdate.Text = System.IO.Path.GetFileNameWithoutExtension(item.ToString());
                                lblUpdate.ForeColor = Color.BlueViolet;

                                OpenDrawing(Convert.ToString(item));

                                if (checkBoxPDF.Checked)
                                {
                                    PDF(Convert.ToString(item));
                                }
                                if (checkBoxDWG.Checked)
                                {
                                    DWG(Convert.ToString(item));
                                }
                                CloseDrawing(Convert.ToString(item));

                                i++;
                            }

                            lblStatus.Text = "Completed";
                            lblStatus.ForeColor = Color.Green;
                            lblUpdate.Text = string.Empty;
                        }
                        else
                        {
                            lblStatus.Text = "Please select export file type.";
                            lblStatus.ForeColor = Color.Red;
                        }
                    }
                }
                else
                {
                    lblStatus.Text = "The folder path is not valid.";
                    lblStatus.ForeColor = Color.Red;
                }
            }
        }

        static bool IsValidFolderPath(string folderPath)
        {
            bool isValid = false;

            try
            {
                // Check if the folder path exists
                if (Directory.Exists(folderPath))
                {
                    isValid = true;
                }
            }
            catch (System.Exception)
            {
                // Handle exception if needed
            }
            return isValid;
        }

        private void ConnectToSolidworks()
        {
            swApp = null;

            try
            {
                // Try to get an active instance of SolidWorks
                swApp = (SldWorks)Marshal.GetActiveObject("sldworks.Application");
            }
            catch (Exception)
            {
                // SolidWorks is not running, create a new instance
                lblStatus.Text = "Opening SolidWorks...";
                lblStatus.ForeColor = Color.Blue;
                swApp = new SldWorks();
                swApp.Visible = true;
            }

            //set solidworks
            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfMultiSheetOption, (int)swDxfMultisheet_e.swDxfActiveSheetOnly);
            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfVersion, (int)swDxfFormat_e.swDxfFormat_R2000);
            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputFonts, 1);
            swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputLineStyles, 1);
        }

        private void OpenDrawing(String Filename)
        {
            swDoc = null;
            swDoc = swApp.OpenDoc6(Filename, (int)swDocumentTypes_e.swDocDRAWING, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, string.Empty, 0, 0);

            //get revision
            CustomPropertyManager swCustProp = swDoc.Extension.CustomPropertyManager[""];
            rev = swCustProp.Get("Revision");

            //pdf view after saving elimination
            exportPdfData = swApp.GetExportFileData(1);
            exportPdfData.ViewPdfAfterSaving = false;
        }

        private void PDF(String Filename)
        {
            sNewFileName = folderPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(Filename) + "_" + rev + ".pdf";
            swDoc.Extension.SaveAs(sNewFileName,0,0, exportPdfData,0,0);
        }
        private void DWG(String Filename)
        {
            swDraw = (DrawingDoc)swDoc;
            if (swDraw.GetSheetCount() == 1)
            {
                sNewFileName = folderPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(Filename) + "_" + rev + ".dwg";
                swDoc.SaveAs(sNewFileName);
            }

            if (swDraw.GetSheetCount() > 1)
            {
                int i = 1;
                string[] vSheet = (string[])swDraw.GetSheetNames();
                foreach (string v in vSheet)
                {
                    swDraw.ActivateSheet(v);
                    sNewFileName = folderPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(Filename) + "_" + rev + "-" + i + ".dwg";
                    swDoc.SaveAs(sNewFileName);
                    i++;
                }
            }
        }

        private void CloseDrawing(String Filename)
        {
            //swApp.CloseDoc(Filename);
            swApp.QuitDoc(Filename);
        }
    }
}
