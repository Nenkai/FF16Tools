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
using Microsoft.Win32;

using FF16PackLib.GUI;
using System.ComponentModel;

namespace FF16Pack.GUI
{
    public partial class MainWindow : Window
    {
        //|||||||||||||||||||||||||||||||||| PUBLIC VARIABLES ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| PUBLIC VARIABLES ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| PUBLIC VARIABLES ||||||||||||||||||||||||||||||||||

        public static string Version = "1.0.0";
        public static string WindowTitle = "FF16 Unpacker by Nenkai (GUI) " + Version;

        //|||||||||||||||||||||||||||||||||| PRIVATE VARIABLES ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| PRIVATE VARIABLES ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| PRIVATE VARIABLES ||||||||||||||||||||||||||||||||||

        private string inputPath = "";
        private string outputPath = "";
        private string gameFile = "";

        private enum ExtractionMode
        {
            SingleArchive = 0,
            MultipleArchives = 1,
            SingleArchiveWithGameFile = 2,
        }

        private ExtractionMode extractionMode;

        private bool isWorking = false; //NOTE: Janky way of displaying work at the moment, it's better to have a progress bar displaying progress but this will suffice.

        //create a couple of objects here that allow us to do async work
        private BackgroundWorker extractionBackgroundWorker;
        private BackgroundWorker listFilesBackgroundWorker;

        private bool inputPathIsFolder
        {
            get
            {
                return extractionMode == ExtractionMode.MultipleArchives;
            }
        }

        //custom class that will redirect classic Console.Write's to a UI window
        private ConsoleTextRedirect consoleTextRedirect;

        //|||||||||||||||||||||||||||||||||| CONSTRUCTOR ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| CONSTRUCTOR ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| CONSTRUCTOR ||||||||||||||||||||||||||||||||||

        public MainWindow()
        {
            //Initalize XAML components...
            InitializeComponent();

            //Initalize our own UI
            InitalizeUI();
        }

        //|||||||||||||||||||||||||||||||||| MAIN LOGIC ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| MAIN LOGIC ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| MAIN LOGIC ||||||||||||||||||||||||||||||||||

        public bool CanExtract()
        {
            if (inputPathIsFolder)
            {
                if (!Directory.Exists(inputPath))
                {
                    ErrorBox("Extraction Error!", "Error! The input folder path you set does not exist! (or is not a valid folder path)");
                    return false;
                }
            }
            else
            {
                if (!File.Exists(inputPath))
                {
                    ErrorBox("Extraction Error!", "Error! The input file path you set does not exist! (or is not a valid file path)");
                    return false;
                }
                else if (System.IO.Path.GetExtension(inputPath) != ".pac") //NOTE: This shouldn't happen, but users can input file paths manually if they want to... gotta be safe
                {
                    ErrorBox("Extraction Error!", "Error! The input file path you set is not a .pac file!");
                    return false;
                }
            }

            if (!Directory.Exists(outputPath))
            {
                ErrorBox("Extraction Error!", "Error! The output folder path you set does not exist! (or is not a valid folder path)");
                return false;
            }

            return true;
        }

        public bool CanGetInfo()
        {
            if (inputPathIsFolder)
            {
                if (!Directory.Exists(inputPath))
                {
                    ErrorBox("Extraction Error!", "Error! The input folder path you set does not exist! (or is not a valid folder path)");
                    return false;
                }
            }
            else
            {
                if (!File.Exists(inputPath))
                {
                    ErrorBox("Extraction Error!", "Error! The input file path you set does not exist! (or is not a valid file path)");
                    return false;
                }
                else if (System.IO.Path.GetExtension(inputPath) != ".pac") //NOTE: This shouldn't happen, but users can input file paths manually if they want to... gotta be safe
                {
                    ErrorBox("Extraction Error!", "Error! The input file path you set is not a .pac file!");
                    return false;
                }
            }

            return true;
        }

        public void Extract()
        {
            if (!CanExtract())
                return;

            //|||||||||||||||||||||||||||||||||| INPUT FOLDER (MULTIPLE .PAC) ||||||||||||||||||||||||||||||||||
            //|||||||||||||||||||||||||||||||||| INPUT FOLDER (MULTIPLE .PAC) ||||||||||||||||||||||||||||||||||
            //|||||||||||||||||||||||||||||||||| INPUT FOLDER (MULTIPLE .PAC) ||||||||||||||||||||||||||||||||||
            if (inputPathIsFolder)
            {
                List<string> pacFilePaths = new List<string>();

                foreach (string filePath in Directory.GetFiles(inputPath))
                {
                    if (System.IO.Path.GetExtension(filePath) == ".pac")
                        pacFilePaths.Add(filePath);
                }

                if (pacFilePaths.Count <= 0)
                {
                    ErrorBox("Extraction Error!", "Error! The input folder path has no .pac files!");
                    return;
                }

                try
                {
                    foreach(string pacFilePath in pacFilePaths)
                    {
                        using var pack = FF16PackLib.FF16Pack.Open(pacFilePath);
                        pack.ExtractAll(outputPath);
                    }

                    InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
                }
                catch (Exception ex)
                {
                    ErrorBox("Extraction Error!", string.Format("Error! {0}", ex));
                }
            }
            //|||||||||||||||||||||||||||||||||| INPUT FILE (SINGLE .PAC) ||||||||||||||||||||||||||||||||||
            //|||||||||||||||||||||||||||||||||| INPUT FILE (SINGLE .PAC) ||||||||||||||||||||||||||||||||||
            //|||||||||||||||||||||||||||||||||| INPUT FILE (SINGLE .PAC) ||||||||||||||||||||||||||||||||||
            else
            {
                try
                {
                    using var pack = FF16PackLib.FF16Pack.Open(inputPath);

                    if(extractionMode == ExtractionMode.SingleArchiveWithGameFile)
                        pack.ExtractFile(gameFile, outputPath);
                    else
                        pack.ExtractAll(outputPath);

                    InfoBox("Extraction Complete!", "Finished Extracting .pac files to output folder.");
                }
                catch (Exception ex)
                {
                    ErrorBox("Extraction Error!", string.Format("Error! {0}", ex));
                }
            }
        }

        public void ListFiles()
        {
            if (!CanGetInfo())
                return;

            consoleTextRedirect.Clear();

            try
            {
                if(inputPathIsFolder)
                {
                    List<string> pacFilePaths = new List<string>();

                    foreach (string filePath in Directory.GetFiles(inputPath))
                    {
                        if (System.IO.Path.GetExtension(filePath) == ".pac")
                            pacFilePaths.Add(filePath);
                    }

                    if (pacFilePaths.Count <= 0)
                    {
                        ErrorBox("Extraction Error!", "Error! The input folder path has no .pac files!");
                        return;
                    }

                    foreach (string pacFilePath in pacFilePaths)
                    {
                        using var pack = FF16PackLib.FF16Pack.Open(pacFilePath);

                        Console.WriteLine("{0}", pacFilePath);
                        Console.WriteLine($"Pack Info:");
                        Console.WriteLine($"- Num Files: {pack.GetNumFiles()}");
                        Console.WriteLine($"- Chunks: {pack.GetNumChunks()}");
                        Console.WriteLine($"- Header Encryption: {pack.HeaderEncrypted}");
                        Console.WriteLine($"- Uses Chunks: {pack.UseChunks}");
                    }
                }
                else
                {
                    using var pack = FF16PackLib.FF16Pack.Open(inputPath);

                    Console.WriteLine("{0}", inputPath);
                    Console.WriteLine($"Pack Info:");
                    Console.WriteLine($"- Num Files: {pack.GetNumFiles()}");
                    Console.WriteLine($"- Chunks: {pack.GetNumChunks()}");
                    Console.WriteLine($"- Header Encryption: {pack.HeaderEncrypted}");
                    Console.WriteLine($"- Uses Chunks: {pack.UseChunks}");
                }
            }
            catch (Exception ex)
            {
                ErrorBox("Archive Parsing Error!", string.Format("Error! {0}", ex));
            }
        }

        //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| BACKGROUND WORKER EVENTS ||||||||||||||||||||||||||||||||||
        //This fancy stuff allows us to do work in async!

        private void BackgroundWorker_RunWorkerCompleted_Extract(object? sender, RunWorkerCompletedEventArgs e)
        {
            isWorking = false;
            UpdateUI();
        }

        private void BackgroundWorker_DoWork_Extract(object? sender, DoWorkEventArgs e)
        {
            Extract();
        }

        private void BackgroundWorker_RunWorkerCompleted_ListFiles(object? sender, RunWorkerCompletedEventArgs e)
        {
            isWorking = false;
            UpdateUI();

            BigTextWindow bigTextWindow = new BigTextWindow(consoleTextRedirect);
            bigTextWindow.ShowDialog(); //Use showdialog so we can freeze the previous window
        }

        private void BackgroundWorker_DoWork_ListFiles(object? sender, DoWorkEventArgs e)
        {
            ListFiles();
        }

        //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| STATIC UTILITY FUNCTIONS ||||||||||||||||||||||||||||||||||

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

        public static void ErrorBox(string title, string contents) => MessageBox.Show(contents, title, MessageBoxButton.OK, MessageBoxImage.Error);

        public static void InfoBox(string title, string contents) => MessageBox.Show(contents, title, MessageBoxButton.OK, MessageBoxImage.Information);

        //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||
        //|||||||||||||||||||||||||||||||||| XAML UI START/UPDATE ||||||||||||||||||||||||||||||||||

        public void InitalizeUI()
        {
            this.Title = WindowTitle;
            ui_extractiomode_combobox.SelectedIndex = 0;

            extractionBackgroundWorker = new BackgroundWorker();
            extractionBackgroundWorker.DoWork += BackgroundWorker_DoWork_Extract;
            extractionBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted_Extract;

            listFilesBackgroundWorker = new BackgroundWorker();
            listFilesBackgroundWorker.DoWork += BackgroundWorker_DoWork_ListFiles;
            listFilesBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted_ListFiles;

            consoleTextRedirect = new ConsoleTextRedirect();
            Console.SetOut(consoleTextRedirect);

            UpdateUI();
        }

        private void UpdateUI()
        {
            //NOTE: Bit of a janky way UI wise to hide all of the UI and show a little text that says progress is happening...
            //We do have the infrastructure to build a progress bar but for now this should suffice.

            Visibility mainElementVisibility = isWorking ? Visibility.Hidden : Visibility.Visible;

            ui_working_label.Visibility = isWorking ? Visibility.Visible : Visibility.Hidden;

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

        private void ui_input_textbox_TextChanged(object sender, TextChangedEventArgs e) => inputPath = ui_input_textbox.Text;

        private void ui_input_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GetPath(ref inputPath, inputPathIsFolder);
            ui_input_textbox.Text = inputPath;
        }

        private void ui_input_button_Click(object sender, RoutedEventArgs e)
        {
            GetPath(ref inputPath, inputPathIsFolder);
            ui_input_textbox.Text = inputPath;
        }

        private void ui_output_textbox_TextChanged(object sender, TextChangedEventArgs e) => outputPath = ui_output_textbox.Text;

        private void ui_output_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GetPath(ref outputPath, true);
            ui_output_textbox.Text = outputPath;
        }

        private void ui_output_button_Click(object sender, RoutedEventArgs e)
        {
            GetPath(ref outputPath, true);
            ui_output_textbox.Text = outputPath;
        }

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

        private void ui_extractiomode_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            extractionMode = (ExtractionMode)ui_extractiomode_combobox.SelectedIndex;
            UpdateUI();
        }

        private void ui_gamefile_textbox_TextChanged(object sender, TextChangedEventArgs e) => gameFile = ui_gamefile_textbox.Text;
    }
}