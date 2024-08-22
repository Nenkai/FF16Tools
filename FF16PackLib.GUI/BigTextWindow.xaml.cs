using FF16Pack.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FF16PackLib.GUI;

/// <summary>
/// Utility GUI window, that displays redirected outputs from the console.
/// </summary>
public partial class BigTextWindow : Window
{
    private TextRedirector consoleTextRedirect;

    public BigTextWindow(TextRedirector consoleTextRedirect)
    {
        InitializeComponent();

        this.Title = MainWindow.WindowTitle + " (DEBUG OUTPUT)";
        this.consoleTextRedirect = consoleTextRedirect;

        ui_textbox.Text = consoleTextRedirect.text;
    }
}
