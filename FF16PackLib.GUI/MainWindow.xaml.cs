using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;

using FF16PackLib.GUI;
using Microsoft.Extensions.Logging;

namespace FF16Pack.GUI;

/// <summary>
/// The main GUI application logic.
/// </summary>
public partial class MainWindow : Window
{

    //|||||||||||||||||||||||||||||||||| PUBLIC VARIABLES ||||||||||||||||||||||||||||||||||
    public static string Version = "1.0.1";
    public static string WindowTitle = "FF16 Unpacker by Nenkai (GUI) " + Version;

    //|||||||||||||||||||||||||||||||||| PRIVATE VARIABLES ||||||||||||||||||||||||||||||||||
    private ILoggerFactory _loggerFactory;
    
    private string unpackInputPath = ""; //This is the source file that we read from (or folder path if its configured)
    private string unpackOutputPath = ""; //This is the folder path where we output our data to
    private string unpackGameFile = ""; //This is where the user can specify what file they want to pull from an archive

    private string repackInputPath = ""; 
    private string repackOutputPath = "";
    private string repackPackName = "";
    private bool repackEncrypt = false;

    //int enums that represent each "value" defined in the ExtractionMode combobox.
    //NOTE: these indexes are in the same order as the items in the XAML document.
    private enum ExtractionMode
    {
        SingleArchive = 0,
        MultipleArchives = 1,
        SingleArchiveWithGameFile = 2,
    }

    private ExtractionMode extractionMode;

    private bool unpackInputPathIsFolder
    {
        get
        {
            return extractionMode == ExtractionMode.MultipleArchives;
        }
    }

    //NOTE: Janky way of displaying work at the moment, it's better to have a progress bar displaying progress but this will suffice.
    private bool isWorking = false;

    //create a couple of objects here that allow us to do async work without freezing the main UI thread.
    private BackgroundWorker extractionBackgroundWorker;
    private BackgroundWorker listFilesBackgroundWorker;

    //custom class that will redirect classic Console.Write's to a UI window
    private TextRedirector textRedirect;

    //|||||||||||||||||||||||||||||||||| CONSTRUCTOR ||||||||||||||||||||||||||||||||||
    public MainWindow()
    {
        //Initalize XAML components...
        InitializeComponent();

        //Initalize our own UI
        InitalizeUI();
    }

    //|||||||||||||||||||||||||||||||||| EXTRACTION LOGIC ||||||||||||||||||||||||||||||||||
    //Here we have the main extraction logic for the app

    /// <summary>
    /// Check to make sure that we have the data we need from the user in order to perform an extraction.
    /// </summary>
    /// <returns></returns>
    public bool CanExtract()
    {
        //If the user set their input path use a folder (i.e. they want to bulk convert .pac files)
        if (unpackInputPathIsFolder)
        {
            //Make sure the input folder that the user set does exist...
            if (!Directory.Exists(unpackInputPath))
            {
                ErrorBox("Extraction Error!", "Error! The input folder path you set does not exist! (or is not a valid folder path)");
                return false;
            }
        }
        //If the user set their input path use a single file (i.e. they want to convert a single .pac file)
        else
        {
            //If the file path the user set no longer exists...
            if (!File.Exists(unpackInputPath))
            {
                ErrorBox("Extraction Error!", "Error! The input file path you set does not exist! (or is not a valid file path)");
                return false;
            }
            //If the file path is referencing a file that is not actually a .pac file...
            //NOTE: This only will happen if the user inputs a file manually in the input textbox.
            else if (System.IO.Path.GetExtension(unpackInputPath) != ".pac")
            {
                ErrorBox("Extraction Error!", "Error! The input file path you set is not a .pac file!");
                return false;
            }
        }

        //Make sure the output folder that the user set does exist...
        if (!Directory.Exists(unpackOutputPath))
        {
            ErrorBox("Extraction Error!", "Error! The output folder path you set does not exist! (or is not a valid folder path)");
            return false;
        }

        //We will only hit this if we passed all checks, so yay!
        return true;
    }

    public void Extract()
    {
        //Make sure we can extract...
        if (!CanExtract())
            return;

        //|||||||||||||||||||||||||||||||||| INPUT FOLDER (MULTIPLE .PAC) ||||||||||||||||||||||||||||||||||
        //Here the user input a folder path that contains multiple .pac files, so they want to bulk unpack whatever is in them.
        if (unpackInputPathIsFolder)
        {
            //declare an array that will store the pac files that we find
            List<string> pacFilePaths = [];

            //iterate through all of the files in the input folder path to find the .pac files
            foreach (string filePath in Directory.GetFiles(unpackInputPath))
            {
                if (System.IO.Path.GetExtension(filePath) == ".pac")
                    pacFilePaths.Add(filePath);
            }

            //if for whatever reason there are no .pac files in the input folder path... then we can't continue because we dont have anything to unpack!
            if (pacFilePaths.Count <= 0)
            {
                ErrorBox("Extraction Error!", "Error! The input folder path has no .pac files!");
                return;
            }

            //perform an extraction on all of the pac files we found...
            try
            {
                foreach (string pacFilePath in pacFilePaths)
                {
                    using var pack = FF16PackLib.FF16Pack.Open(pacFilePath);
                    string packName = System.IO.Path.GetFileNameWithoutExtension(pacFilePath);
                    string outputDir = System.IO.Path.Combine(unpackOutputPath, $"{packName}_extracted");
                    Directory.CreateDirectory(outputDir);

                    pack.ExtractAll(outputDir);
                }

                InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
            }
            catch (Exception ex)
            {
                ErrorBox("Extraction Error!", string.Format("Error! {0}", ex));
            }
        }

        //|||||||||||||||||||||||||||||||||| INPUT FILE (SINGLE .PAC) ||||||||||||||||||||||||||||||||||
        //Here the user input a single pac file path...
        else
        {
            //perform extraction on the single pac file that the user wants to use...
            try
            {
                using var pack = FF16PackLib.FF16Pack.Open(unpackInputPath);

                string packName = System.IO.Path.GetFileNameWithoutExtension(unpackInputPath);
                string outputDir = System.IO.Path.Combine(unpackOutputPath, $"{packName}_extracted");
                Directory.CreateDirectory(outputDir);

                //if its configured to where the user wants to extract a specific game file from an archive, then let them
                if(extractionMode == ExtractionMode.SingleArchiveWithGameFile)
                    pack.ExtractFile(unpackGameFile, outputDir);
                else //otherwise extract all of the files from the archive...
                    pack.ExtractAll(outputDir);

                InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
            }
            catch (Exception ex)
            {
                ErrorBox("Extraction Error!", string.Format("Error! {0}", ex));
            }
        }
    }

    //|||||||||||||||||||||||||||||||||| LIST FILE LOGIC ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| LIST FILE LOGIC ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| LIST FILE LOGIC ||||||||||||||||||||||||||||||||||
    //Here we have the main logic of the app where we can list files and parse other information about an arhcive.

    public void ListFile(string path)
    {
        using var pack = FF16PackLib.FF16Pack.Open(path, _loggerFactory);
        pack.DumpInfo();

        string inputFileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string outputPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)), $"{inputFileName}_files.txt");
        pack.ListFiles(outputPath);
        Console.WriteLine($"Done. ({outputPath})");
    }

    public void ListFiles()
    {
        //Reuse our checks from Extraction to make sure we can do what we need to.
        if (!CanExtract())
            return;

        //Clear the text on our window because we will be writing new data to it
        textRedirect.Clear();

        try
        {
            if(unpackInputPathIsFolder)
            {
                List<string> pacFilePaths = new List<string>();

                foreach (string filePath in Directory.GetFiles(unpackInputPath))
                {
                    if (System.IO.Path.GetExtension(filePath) == ".pac")
                        pacFilePaths.Add(filePath);
                }

                if (pacFilePaths.Count <= 0)
                {
                    ErrorBox("Extraction Error!", "Error! The input folder path has no .pac files!");
                    return;
                }

                for (int i = 0; i < pacFilePaths.Count; i++)
                    ListFile(pacFilePaths[i]);
            }
            else
                ListFile(unpackInputPath);
        }
        catch (Exception ex)
        {
            ErrorBox("Archive Parsing Error!", string.Format("Error! {0}", ex));
        }
    }

    //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
    //This fancy stuff allows us to do work in async! (Won't freeze the Main UI thread)

    private void BackgroundWorker_DoWork_Extract(object? sender, DoWorkEventArgs e) => Extract();

    private void BackgroundWorker_RunWorkerCompleted_Extract(object? sender, RunWorkerCompletedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        isWorking = false;
        UpdateUI();
    }

    private void BackgroundWorker_DoWork_ListFiles(object? sender, DoWorkEventArgs e) => ListFiles();

    private void BackgroundWorker_RunWorkerCompleted_ListFiles(object? sender, RunWorkerCompletedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        isWorking = false;
        UpdateUI();

        BigTextWindow bigTextWindow = new BigTextWindow(textRedirect);
        bigTextWindow.ShowDialog(); //Use showdialog so we can freeze the previous window
    }

    //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||
    //Some utility functions that get used quite a bit...

    /// <summary>
    /// Pops a file/folder dialog.
    /// <para>NOTE: When using a file dialog, there is a pre-applied .pac extension/filter. </para>
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    public static string GetPath(bool folderPath)
    {
        if(folderPath)
        {
            OpenFolderDialog folderDialog = new();

            if (folderDialog.ShowDialog() == true)
                return folderDialog.FolderName;
        }
        else
        {
            OpenFileDialog fileDialog = new();
            fileDialog.DefaultExt = ".pac";
            fileDialog.Filter = ".pac Archives (.pac)|*.pac";

            if (fileDialog.ShowDialog() == true)
                return fileDialog.FileName;
        }

        return "";
    }

    public static void GetPath(ref string path, bool folderPath)
    {
        string newPath = GetPath(folderPath);

        if (newPath != null && !string.IsNullOrEmpty(newPath))
            path = newPath;
    }

    //Condensed wrappers for message boxes...
    public static void ErrorBox(string title, string contents) => MessageBox.Show(contents, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public static void InfoBox(string title, string contents) => MessageBox.Show(contents, title, MessageBoxButton.OK, MessageBoxImage.Information);

    //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||

    public void InitalizeUI()
    {
        this.Title = WindowTitle;
        ui_extractiomode_combobox.SelectedIndex = 0; //Select the first index of extraction mode by default "SingleArchive"

        //setup background workers so we can do async work
        extractionBackgroundWorker = new BackgroundWorker();
        extractionBackgroundWorker.DoWork += BackgroundWorker_DoWork_Extract;
        extractionBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted_Extract;

        listFilesBackgroundWorker = new BackgroundWorker();
        listFilesBackgroundWorker.DoWork += BackgroundWorker_DoWork_ListFiles;
        listFilesBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted_ListFiles;

        //setup our log provider so we can output everything to our BigTextWindow later.
        textRedirect = new TextRedirector();
        _loggerFactory = LoggerFactory.Create(e => e.AddProvider(new TextRedirectLoggerProvider(textRedirect)));

        //update the UI to reflect the state of the app
        UpdateUI();
    }

    private void UpdateUI()
    {
        //NOTE: Bit of a janky way UI wise to hide all of the UI and show a little text that says progress is happening...
        //We do have the infrastructure to build a progress bar but for now this should suffice.

        Visibility mainElementVisibility = isWorking ? Visibility.Hidden : Visibility.Visible;

        ui_working_label.Visibility = isWorking ? Visibility.Visible : Visibility.Hidden;

        ui_seperatorTop.Visibility = mainElementVisibility;
        ui_extractiomode_combobox.Visibility = mainElementVisibility;
        ui_extractiomode_label.Visibility = mainElementVisibility;
        ui_input_label.Visibility = mainElementVisibility;
        ui_input_textbox.Visibility = mainElementVisibility;
        ui_input_button.Visibility = mainElementVisibility;
        ui_gamefile_label.Visibility = mainElementVisibility;
        ui_gamefile_textbox.Visibility = mainElementVisibility;
        ui_output_label.Visibility = mainElementVisibility;
        ui_output_textbox.Visibility = mainElementVisibility;
        ui_output_button.Visibility = mainElementVisibility;
        ui_seperatorBottom.Visibility = mainElementVisibility;
        ui_listfiles_button.Visibility = mainElementVisibility;
        ui_extract_button.Visibility = mainElementVisibility;

        //enable this field if the user wants to extract a specific file from the .pac archive, otherwise disable it because we'd confuse the user
        ui_gamefile_label.IsEnabled = extractionMode == ExtractionMode.SingleArchiveWithGameFile;
        ui_gamefile_textbox.IsEnabled = extractionMode == ExtractionMode.SingleArchiveWithGameFile;

        //Disable the list files button because with this mode the user already knows what file they want (supposedly)
        ui_listfiles_button.IsEnabled = extractionMode != ExtractionMode.SingleArchiveWithGameFile;
    }

    //|||||||||||||||||||||||||||||||||| XAML EVENTS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| XAML EVENTS ||||||||||||||||||||||||||||||||||
    //|||||||||||||||||||||||||||||||||| XAML EVENTS ||||||||||||||||||||||||||||||||||
    //Here we hook up our app logic to the UI element events.

    //================= (UNPACK) EXTRACTION MODE SECTION =================

    private void ui_extractiomode_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        extractionMode = (ExtractionMode)ui_extractiomode_combobox.SelectedIndex;
        UpdateUI();
    }

    //================= (UNPACK) INPUT SECTION =================
    private void ui_input_textbox_TextChanged(object sender, TextChangedEventArgs e) => unpackInputPath = ui_input_textbox.Text;

    private void ui_input_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref unpackInputPath, unpackInputPathIsFolder);
        ui_input_textbox.Text = unpackInputPath;
    }

    private void ui_input_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref unpackInputPath, unpackInputPathIsFolder);
        ui_input_textbox.Text = unpackInputPath;
    }

    private void ui_input_textbox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] dataString = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = dataString[0];
            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path) && !Directory.Exists(path))
                return;

            ui_input_textbox.Text = path;
        }
    }

    private void ui_input_textbox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }

    //================= (UNPACK) GAME FILE SECTION =================

    private void ui_gamefile_textbox_TextChanged(object sender, TextChangedEventArgs e) => unpackGameFile = ui_gamefile_textbox.Text;

    //================= (UNPACK) OUTPUT SECTION =================

    private void ui_output_textbox_TextChanged(object sender, TextChangedEventArgs e) => unpackOutputPath = ui_output_textbox.Text;

    private void ui_output_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref unpackOutputPath, true);
        ui_output_textbox.Text = unpackOutputPath;
    }

    private void ui_output_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref unpackOutputPath, true);
        ui_output_textbox.Text = unpackOutputPath;
    }

    private void ui_output_textbox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }

    private void ui_output_textbox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] dataString = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = dataString[0];
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
                return;

            ui_output_textbox.Text = path;
        }
    }

    //================= (UNPACK) ACTIONS SECTION =================

    private void ui_listfiles_button_Click(object sender, RoutedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        isWorking = true;
        UpdateUI();

        listFilesBackgroundWorker.RunWorkerAsync();
    }

    private void ui_extract_button_Click(object sender, RoutedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        isWorking = true;
        UpdateUI();

        extractionBackgroundWorker.RunWorkerAsync();
    }


    //================= (REPACK) INPUT SECTION =================

    private void ui_repack_input_textbox_TextChanged(object sender, TextChangedEventArgs e) => repackInputPath = ui_repack_input_textbox.Text;

    private void ui_repack_input_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref repackInputPath, true);
        ui_repack_input_textbox.Text = repackInputPath;
    }

    private void ui_repack_input_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref repackInputPath, true);
        ui_repack_input_textbox.Text = repackInputPath;
    }

    //================= (REPACK) OUTPUT SECTION =================

    private void ui_repack_output_textbox_TextChanged(object sender, TextChangedEventArgs e) => repackOutputPath = ui_repack_output_textbox.Text;

    private void ui_repack_output_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref repackOutputPath, true);
        ui_repack_output_textbox.Text = repackOutputPath;
    }

    private void ui_repack_output_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref repackOutputPath, true);
        ui_repack_output_textbox.Text = repackOutputPath;
    }

    //================= (REPACK) PACK NAME SECTION =================

    private void ui_repack_packName_textbox_TextChanged(object sender, TextChangedEventArgs e) => repackPackName = ui_repack_packName_textbox.Text;

    //================= (REPACK) ENCRYPT SECTION =================

    private void ui_repack_encrypt_checkbox_Click(object sender, RoutedEventArgs e) => repackEncrypt = (bool)ui_repack_encrypt_checkbox.IsChecked;

    //================= (REPACK) ACTIONS SECTION =================

    private void ui_repack_pack_button_Click(object sender, RoutedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        //isWorking = true;
        //UpdateUI();
    }
}