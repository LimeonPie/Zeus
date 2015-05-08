using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
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
using Zeus.Helpers;
using Zeus.Engine;

namespace Zeus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
            LogManager.Session.logMessage("Program is starting");
        }

        private void inputFileButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true) {
                System.Diagnostics.Debug.WriteLine(openFileDialog.FileName);
                LogManager.Session.logMessage("Opening " + openFileDialog.FileName + " input file");
                Engine.Engine.Instance.initSphereWithInputFile(openFileDialog.FileName);
                setInformation();
            }
        }

        private void setInformation() {
            double latitude = Engine.Engine.Instance.lowAtmosphere.latitude;
            double longitude = Engine.Engine.Instance.lowAtmosphere.longitude;
            longitudeTextBox.Text = longitude.ToString();
            latitudeTextBox.Text = latitude.ToString();
            foreach (Element el in Engine.Engine.Instance.lowAtmosphere.aerosolElements) {
                elementsPanel.Text += String.Format("{0} = {1}\n", el.name, el.n0);
            }
        }
    }
}
