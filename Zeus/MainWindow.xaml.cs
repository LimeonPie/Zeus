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
using OxyPlot;
using OxyPlot.Wpf;

namespace Zeus
{

    public partial class MainWindow : Window
    {

        public MainWindow() {
            InitializeComponent();
            LogManager.Session.logMessage("Program is starting");
        }

        // Запись в лейблы информации из файла
        private void setInformation() {
            double latitude = Engine.Engine.Instance.lowAtmosphere.latitude;
            double longitude = Engine.Engine.Instance.lowAtmosphere.longitude;
            longitudeTextBox.Text = longitude.ToString();
            latitudeTextBox.Text = latitude.ToString();
            foreach (Zeus.Engine.Element el in Engine.Engine.Instance.lowAtmosphere.aerosolElements) {
                elementsPanel.Text += String.Format("{0} = {1}\n", el.name, el.n0);
            }
        }

        // Запись в обычный лэйбл статус бара
        private void writeToStatusBar(string message) {
            statusLabel.Content = message;
        }

        private void drawPlot() {
            //SpherePlotModel model = new SpherePlotModel("test", Engine.Engine.Instance.lowAtmosphere.neGrid);
            //plotView.Model = model.CurrentModel;
            SpherePlotModel electronModel = new SpherePlotModel(PLOT.ELECTRON_LINE);
            electronPlotView.Model = electronModel.CurrentModel;

            SpherePlotModel ionPlusModel = new SpherePlotModel(PLOT.ION_PLUS_LINE);
            ionPositivePlotView.Model = ionPlusModel.CurrentModel;

            SpherePlotModel ionMinusModel = new SpherePlotModel(PLOT.ION_MINUS_LINE);
            ionNegativePlotView.Model = ionMinusModel.CurrentModel;

            SpherePlotModel allModel = new SpherePlotModel(PLOT.ALL_LINE);
            allChargesPlotView.Model = allModel.CurrentModel;
            
        }

        private void OnCalculationsEnded(object sender, SphereEventArgs e) {
            writeToStatusBar(Properties.Resources.CalculationsEndText);
            MessageBoxResult alert = MessageBox.Show(Properties.Resources.CalculationsEndAlertDescText, Properties.Resources.CalculationsEndText, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            resultTextBlock.Text = String.Format("{0} = {1}", Zeus.Properties.Resources.Electricity, e.result.ToString("#.###E0"));
            clearStatusWithDelay(2000);
            drawPlot();
        }

        private void OnProgressValueChanged(object sender, SphereEventArgs e) {
            double value = ((double)(e.state+1) / (double)Engine.Engine.Instance.lowAtmosphere.capacity) * 100;
            writeToStatusBar(String.Format(Properties.Resources.CalculationsCurrentState, e.state));
            progressBar.Value = value;
            procentLabel.Content = Math.Round(value).ToString() + " %";
        }

        private async void clearStatusWithDelay(int delay) {
            await Task.Delay(delay);
            statusLabel.Content = "";
            procentLabel.Content = "";
            progressBar.Value = 0;
        }

        private void OnSave(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json files (*.json)|*.json";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == true) {
                LogManager.Session.logMessage("Saving to " + saveFileDialog.FileName + " output file");
                try {
                    Engine.Engine.Instance.saveToFile(saveFileDialog.FileName);
                    writeToStatusBar(Zeus.Properties.Resources.SaveStatusSucces);
                    clearStatusWithDelay(2000);
                }
                catch (IOException error) {
                    MessageBoxResult alert = MessageBox.Show(error.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.Session.logMessage("Cannot save output file to " + saveFileDialog.FileName + " cause of " + error.Message);
                }
            }
        }

        // Открытие файла
        private void OnOpenFile(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true) {
                LogManager.Session.logMessage("Opening " + openFileDialog.FileName + " input file");
                try {
                    Engine.Engine.Instance.initSphereWithInputFile(openFileDialog.FileName);
                    setInformation();
                    Engine.Engine.Instance.lowAtmosphere.stateCalculated += OnProgressValueChanged;
                    Engine.Engine.Instance.lowAtmosphere.calculationsDone += OnCalculationsEnded;
                }
                catch (IOException error) {
                    MessageBoxResult alert = MessageBox.Show(error.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.Session.logMessage("Cannot open input file " + openFileDialog.FileName + " cause of " + error.Message);
                }
                
            }
        }

        // Запуск расчета
        private void OnLaunch(object sender, RoutedEventArgs e) {
            Engine.Engine.Instance.launchComputations();
        }

        private void OnClose(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
