﻿using FF16Tools.Pack.Crypto;
using FF16Tools.Pack.Packing;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Diagnostics;
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

namespace FF16Tools.Pack.GUI;

/// <summary>
/// The main GUI application logic.
/// </summary>
public partial class MainWindow : Window
{

    //|||||||||||||||||||||||||||||||||| PUBLIC VARIABLES ||||||||||||||||||||||||||||||||||
    public static string Version = "1.0.2";
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

    private enum Game
    {
        FFXVI = 0,
        FFT = 1
    }

    private Dictionary<Game, string> GameNameMap = new()
    {
        [Game.FFXVI] = "Final Fantasy 16",
        [Game.FFT] = "Final Fantasy Tactics - The Ivalice Chronicles"
    };

    private Dictionary<Game, string> GameCodeNameMap = new()
    {
        [Game.FFXVI] = PackKeyStore.FFXVI_CODENAME,
        [Game.FFT] = PackKeyStore.FFT_IVALICE_CODENAME
    };

    private Game game;

    private string CurrentGameCodeName => GameCodeNameMap[game];

    private bool unpackInputPathIsFolder
    {
        get
        {
            return extractionMode == ExtractionMode.MultipleArchives;
        }
    }

    //NOTE: Janky way of displaying work at the moment, it's better to have a progress bar displaying progress but this will suffice.
    private bool isWorking = false;

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

    public async Task Extract()
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
                    using var pack = FF16Pack.Open(pacFilePath, CurrentGameCodeName);
                    string packName = System.IO.Path.GetFileNameWithoutExtension(pacFilePath);
                    string outputDir = System.IO.Path.Combine(unpackOutputPath, packName);
                    Directory.CreateDirectory(outputDir);

                    await pack.ExtractAllAsync(outputDir);
                }

                InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(repackInputPath));
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
                using var pack = FF16Pack.Open(unpackInputPath, CurrentGameCodeName);

                string packName = System.IO.Path.GetFileNameWithoutExtension(unpackInputPath);
                string outputDir = System.IO.Path.Combine(unpackOutputPath, packName);
                Directory.CreateDirectory(outputDir);

                //if its configured to where the user wants to extract a specific game file from an archive, then let them
                if (extractionMode == ExtractionMode.SingleArchiveWithGameFile)
                    await pack.ExtractFileAsync(unpackGameFile, outputDir);
                else //otherwise extract all of the files from the archive...
                    await pack.ExtractAllAsync(outputDir);

                InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
                Process.Start("explorer.exe", outputDir);
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
        using var pack = FF16Pack.Open(path, CurrentGameCodeName, _loggerFactory);
        pack.DumpInfo();

        string inputFileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string outputPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)), $"{inputFileName}_files.txt");
        pack.ListFiles(outputPath, log: true);
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
            if (unpackInputPathIsFolder)
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
        if (folderPath)
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

        foreach (Game gameOption in Enum.GetValues(typeof(Game)))
        {
            ui_game_combobox.Items.Add(GameNameMap[gameOption]);
        }

        ui_game_combobox.SelectedIndex = 0;

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
        tb_TabControl.Visibility = mainElementVisibility;

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
        ui_output_textbox.Text = unpackOutputPath = System.IO.Path.GetDirectoryName(unpackInputPath);
    }

    private void ui_input_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref unpackInputPath, unpackInputPathIsFolder);
        ui_input_textbox.Text = unpackInputPath;
        ui_output_textbox.Text = unpackOutputPath = System.IO.Path.GetDirectoryName(unpackInputPath);
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

        ListFiles();

        BigTextWindow bigTextWindow = new BigTextWindow(textRedirect);
        bigTextWindow.ShowDialog(); //Use showdialog so we can freeze the previous window

        isWorking = false;
        UpdateUI();
    }

    private async void ui_extract_button_Click(object sender, RoutedEventArgs e)
    {
        //NOTE: replace in future with progress bar
        isWorking = true;
        UpdateUI();

        await Extract();

        isWorking = false;
        UpdateUI();
    }


    //================= (REPACK) INPUT SECTION =================

    private void ui_repack_input_textbox_TextChanged(object sender, TextChangedEventArgs e) => repackInputPath = ui_repack_input_textbox.Text;

    private void ui_repack_input_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref repackInputPath, true);
        ui_repack_input_textbox.Text = repackInputPath;
        ui_repack_output_textbox.Text = repackOutputPath = GenerateOutputPathFromInput();
    }

    private void ui_repack_input_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref repackInputPath, true);
        ui_repack_input_textbox.Text = repackInputPath;

        ui_repack_output_textbox.Text = repackOutputPath = GenerateOutputPathFromInput();
    }

    private string GenerateOutputPathFromInput()
    {
        string packName = System.IO.Path.GetFileName(repackInputPath);
        List<string> spl = packName.Split('.').ToList();
        spl.Insert(1, "diff");
        string diffPackName = string.Join('.', spl);

        string diffFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(repackInputPath), diffPackName) + ".pac";
        return diffFullPath;
    }

    //================= (REPACK) OUTPUT SECTION =================

    private void ui_repack_output_textbox_TextChanged(object sender, TextChangedEventArgs e) => repackOutputPath = ui_repack_output_textbox.Text;

    private void ui_repack_output_button_Click(object sender, RoutedEventArgs e)
    {
        GetPath(ref repackOutputPath, false);
        ui_repack_output_textbox.Text = repackOutputPath;
    }

    private void ui_repack_output_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        GetPath(ref repackOutputPath, false);
        ui_repack_output_textbox.Text = repackOutputPath;
    }

    //================= (REPACK) ENCRYPT SECTION =================

    private void ui_repack_encrypt_checkbox_Click(object sender, RoutedEventArgs e) => repackEncrypt = (bool)ui_repack_encrypt_checkbox.IsChecked;

    //================= (REPACK) ACTIONS SECTION =================

    private async void ui_repack_pack_button_Click(object sender, RoutedEventArgs e)
    {
        isWorking = true;
        UpdateUI();

        await Pack();

        isWorking = false;
        UpdateUI();
    }

    public bool CanPack()
    {
        //Make sure the output folder that the user set does exist...
        if (!Directory.Exists(repackInputPath))
        {
            ErrorBox("Extraction Error!", "Error! The input folder path you set does not exist! (or is not a valid folder path)");
            return false;
        }

        return true;
    }

    private async Task Pack()
    {
        if (!CanPack())
            return;

        try
        {
            var packBuilder = new FF16PackBuilder(new PackBuildOptions()
            {
                Encrypt = repackEncrypt,
            });

            packBuilder.InitFromDirectory(repackInputPath);
            await packBuilder.WriteToAsync(repackOutputPath);

            InfoBox("Extraction Complete!", "Finished packing.");

            if (ui_repack_open_on_finish_checkbox.IsChecked == true)
                Process.Start("explorer.exe", $"/select, \"{System.IO.Path.GetFullPath(repackOutputPath)}\"");
        }
        catch (Exception ex)
        {
            ErrorBox("Pack Error!", ex.Message);
        }
    }

    private void ui_game_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        game = (Game)ui_game_combobox.SelectedIndex;
        UpdateUI();
    }
}