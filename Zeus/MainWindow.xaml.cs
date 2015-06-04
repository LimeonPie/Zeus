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
            // Записываем входную информацию
            double latitude = Engine.Engine.Instance.lowAtmosphere.latitude;
            double longitude = Engine.Engine.Instance.lowAtmosphere.longitude;
            double gridStep = Engine.Engine.Instance.lowAtmosphere.delta;
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", Zeus.Properties.Resources.LongitudeText, longitude);
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", Zeus.Properties.Resources.LatitudeText, latitude);
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", Zeus.Properties.Resources.EternityFlux, Constants.eternityFlux.ToString("#.###E0"));
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", Zeus.Properties.Resources.GridStep, gridStep);
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", "dt", Constants.dt);
            inputDataTextBlock.Text += String.Format("{0} = {1}\n", Zeus.Properties.Resources.TimeInterval, Constants.timeInterval);

            // Записываем входные элементы
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
            bool needLog = false;
            if (normalPlot.IsChecked == true) needLog = false;
            else if (logPlot.IsChecked == true) needLog = true;

            SpherePlotModel electronModel = new SpherePlotModel(PLOT.ELECTRON_LINE, needLog);
            electronPlotView.Model = electronModel.CurrentModel;

            SpherePlotModel ionPlusModel = new SpherePlotModel(PLOT.ION_PLUS_LINE, needLog);
            ionPositivePlotView.Model = ionPlusModel.CurrentModel;

            SpherePlotModel ionMinusModel = new SpherePlotModel(PLOT.ION_MINUS_LINE, needLog);
            ionNegativePlotView.Model = ionMinusModel.CurrentModel;

            SpherePlotModel allModel = new SpherePlotModel(PLOT.ALL_LINE, needLog);
            allChargesPlotView.Model = allModel.CurrentModel;

            SpherePlotModel electronVelocityModel = new SpherePlotModel(PLOT.ELECTRON_VELOCITY_LINE, needLog);
            electronVelocityPlotView.Model = electronVelocityModel.CurrentModel;

            SpherePlotModel ionPlusVelocityModel = new SpherePlotModel(PLOT.ION_PLUS_VELOCITY_LINE, needLog);
            ionPlusVelocityPlotView.Model = ionPlusVelocityModel.CurrentModel;

            SpherePlotModel ionMinusVelocityModel = new SpherePlotModel(PLOT.ION_MINUS_VELOCITY_LINE, needLog);
            ionMinusVelocityPlotView.Model = ionMinusVelocityModel.CurrentModel;

            SpherePlotModel fluxModel = new SpherePlotModel(PLOT.FLUX_LINE, needLog);
            eternityFluxPlotView.Model = fluxModel.CurrentModel;

            SpherePlotModel activeElementModel = new SpherePlotModel(PLOT.ACTIVE_ELEMENT_LINE, needLog);
            nitrogenPlotView.Model = activeElementModel.CurrentModel;

            SpherePlotModel aerosolsModel = new SpherePlotModel(PLOT.AEROSOL_LINE, needLog);
            aerosolPlotView.Model = aerosolsModel.CurrentModel;

            SpherePlotModel temperatureModel = new SpherePlotModel(PLOT.TEMPERATURE_HEAT, needLog);
            temperaturePlotView.Model = temperatureModel.CurrentModel;
            
        }

        private void OnCalculationsEnded(object sender, SphereEventArgs e) {
            writeToStatusBar(Properties.Resources.CalculationsEndText);
            MessageBoxResult alert = MessageBox.Show(Properties.Resources.CalculationsEndAlertDescText, Properties.Resources.CalculationsEndText, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            double electicity = Engine.Engine.Instance.getElectro();
            resultTextBlock.Text = String.Format("{0} = {1}", Zeus.Properties.Resources.Electricity, electicity.ToString("#.###E0"));
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

        private void OnRedraw(object sender, RoutedEventArgs e) {
            drawPlot();
        }
    }
}
