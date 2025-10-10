using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolidConvert_Pro
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			// Handle drag and drop events
			this.DragEnter += MainForm_DragEnter;
			this.DragDrop += MainForm_DragDrop;
		}



		// When files are dragged into the form area
		private void MainForm_DragEnter(object sender, DragEventArgs e)
		{
			// Check if the data being dragged is a file
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy; // Show the copy icon
			}
			else
			{
				e.Effect = DragDropEffects.None; // Show the 'not allowed' icon
			}
		}

		// When files are dropped onto the form
		private void MainForm_DragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the file paths
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			foreach (string file in files)
			{
				// Check if the file has the .slddrw extension
				if (System.IO.Path.GetExtension(file).Equals(".slddrw", StringComparison.OrdinalIgnoreCase))
				{
					// Add the file to the ListBox
					lstSelectedFiles.Items.Add(file);
				}
			}
		}


		int n;
		SldWorks swApp;
		ModelDoc2 swDoc;
		DrawingDoc swDraw;
		Sheet Sheet;
		string sNewFileName;
		string folderPath;
		string rev;
		IExportPdfData exportPdfData;

		private void BtnAddfiles_Click(object sender, EventArgs e)
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

			for (int i = 0; i < n; i++)
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

		private async void btnExport_Click(object sender, EventArgs e)
		{
			await Main();
		}

		public async System.Threading.Tasks.Task Main()
		{

			// Path validation
			if (txtboxOutFolder.Text.Length == 0)
			{
				lblStatus.Text = "Please select output folder.";
				lblStatus.ForeColor = Color.Red;
				return; // Early exit if output folder is not selected
			}

			folderPath = txtboxOutFolder.Text;

			if (!IsValidFolderPath(folderPath))
			{
				lblStatus.Text = "The folder path is not valid.";
				lblStatus.ForeColor = Color.Red;
				return; // Early exit if folder path is invalid
			}

			if (lstSelectedFiles.Items.Count == 0)
			{
				lblStatus.Text = "Please select file to convert.";
				lblStatus.ForeColor = Color.Red;
				return; // Early exit if no files are selected
			}

			if (!checkBoxPDF.Checked && !checkBoxDWG.Checked)
			{
				lblStatus.Text = "Please select export file type.";
				lblStatus.ForeColor = Color.Red;
				return; // Early exit if no export type is selected
			}

			lblStatus.Text = "Starting Conversion...";
			lblStatus.ForeColor = Color.Blue;

			MakeFormReadOnly(this);

			await ConnectToSolidworksAsync();

			int i = 1;
			await Task.Run(() =>
			{
				foreach (var item in lstSelectedFiles.Items)
				{
					this.Invoke(new Action(() =>
					{
						// Update the UI from the UI thread
						txtboxStatus.Text = $"{i} / {n}";
						lblStatus.Text = "Converting...";
						lblStatus.ForeColor = Color.Blue;
						lblUpdate.Text = Path.GetFileNameWithoutExtension(item.ToString());
						lblUpdate.ForeColor = Color.BlueViolet;
					}));

					OpenDrawing(item.ToString());

					if (checkBoxPDF.Checked)
					{
						PDF(item.ToString());
					}

					if (checkBoxDWG.Checked)
					{
						DWG(item.ToString());
					}

					CloseDrawing(item.ToString());
					i++;
				}
			});

			lblStatus.Text = "Completed";
			lblStatus.ForeColor = Color.Green;
			lblUpdate.Text = string.Empty;
			Clearup();
			MakeFormRead(this);
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
				// no exception
			}
			return isValid;
		}

		private async Task ConnectToSolidworksAsync()
		{
			swApp = null;

			try
			{
				// Update UI to indicate SolidWorks is opening (since it's on a separate thread now)
				this.Invoke(new Action(() =>
				{
					lblStatus.Text = "Connecting to SolidWorks...";
					lblStatus.ForeColor = Color.Blue;
				}));

				// Try to get an active instance of SolidWorks in a background thread
				await Task.Run(() =>
				{
					try
					{
						swApp = (SldWorks)Marshal.GetActiveObject("sldworks.Application");
					}
					catch (COMException)
					{
						// If no active instance, create a new instance of SolidWorks
						swApp = new SldWorks();
						swApp.Visible = true;
					}
				});

				// After connection, set SolidWorks preferences
				swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfMultiSheetOption, (int)swDxfMultisheet_e.swDxfActiveSheetOnly);
				swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfVersion, (int)swDxfFormat_e.swDxfFormat_R2000);
				swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputFonts, 1);
				swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputLineStyles, 1);

				// Update the UI to show successful connection
				this.Invoke(new Action(() =>
				{
					lblStatus.Text = "Connected to SolidWorks";
					lblStatus.ForeColor = Color.Green;
				}));
			}
			catch (Exception ex)
			{
				// Handle exceptions related to connecting to SolidWorks
				this.Invoke(new Action(() =>
				{
					lblStatus.Text = $"Error connecting to SolidWorks: {ex.Message}";
					lblStatus.ForeColor = Color.Red;
				}));
			}
		}

		private void OpenDrawing(String Filename)
		{
			swDoc = null;
			swDoc = swApp.OpenDoc6(Filename, (int)swDocumentTypes_e.swDocDRAWING, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, string.Empty, 0, 0);

			//get revision
			CustomPropertyManager swCustProp = swDoc.Extension.CustomPropertyManager[""];
			rev = swCustProp.Get("Revision");

			//pdf view after saving elimination
			exportPdfData = (IExportPdfData)swApp.GetExportFileData(1);
			exportPdfData.ViewPdfAfterSaving = false;

		}

		private void PDF(String Filename)
		{
			sNewFileName = folderPath + "\\" + System.IO.Path.GetFileNameWithoutExtension(Filename) + "_" + rev + ".pdf";
			swDoc.Extension.SaveAs(sNewFileName, 0, 0, exportPdfData, 0, 0);
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

		private void Clearup()
		{
			swDoc = null;
			swApp = null;
		}

		private void MakeFormReadOnly(Control parentControl)
		{
			foreach (Control control in parentControl.Controls)
			{
				// For input controls, disable them or make read-only based on their type
				if (control is TextBox)
				{
					// Make TextBox read-only
					((TextBox)control).ReadOnly = true;
				}
				else if (control is Button || control is CheckBox || control is RadioButton || control is ComboBox || control is ListBox)
				{
					// Disable other interactive controls
					control.Enabled = false;
				}

				// Recursively apply to child controls (in case of panels, group boxes, etc.)
				if (control.HasChildren)
				{
					MakeFormReadOnly(control);
				}
			}
		}
		private void MakeFormRead(Control parentControl)
		{
			foreach (Control control in parentControl.Controls)
			{
				// For input controls, disable them or make read-only based on their type
				if (control is TextBox)
				{
					// Make TextBox read-only
					((TextBox)control).ReadOnly = false;
				}
				else if (control is Button || control is CheckBox || control is RadioButton || control is ComboBox || control is ListBox)
				{
					// Disable other interactive controls
					control.Enabled = true;
				}

				// Recursively apply to child controls (in case of panels, group boxes, etc.)
				if (control.HasChildren)
				{
					MakeFormReadOnly(control);
				}
			}
		}

	}

}
