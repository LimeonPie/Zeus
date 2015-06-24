using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
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

    // Файл интерфейса основого окна
    // Дипломная работа
    // ИВТ(б)-411 Миняев Илья

    public partial class MainWindow : Window
    {

        public MainWindow() {
            InitializeComponent();
            if (Properties.Settings.Default.isShowAdditionalPlots == false) {
                hideVelocities();
            }
            progressBar.Visibility = Visibility.Hidden;
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

        private void hideVelocities() {
            electronVelocityPlotView.Visibility = Visibility.Collapsed;
            ionPlusVelocityPlotView.Visibility = Visibility.Collapsed;
            ionMinusVelocityPlotView.Visibility = Visibility.Collapsed;
            electronVelocityTab.Visibility = Visibility.Collapsed;
            ionPlusVelocityTab.Visibility = Visibility.Collapsed;
            ionMinusVelocityTab.Visibility = Visibility.Collapsed;
        }

        private void showVelocities() {
            electronVelocityPlotView.Visibility = Visibility.Visible;
            ionPlusVelocityPlotView.Visibility = Visibility.Visible;
            ionMinusVelocityPlotView.Visibility = Visibility.Visible;
            electronVelocityTab.Visibility = Visibility.Visible;
            ionPlusVelocityTab.Visibility = Visibility.Visible;
            ionMinusVelocityTab.Visibility = Visibility.Visible;
        }

        private void drawPlots() {
            if (Engine.Engine.Instance.lowAtmosphere != null) {
                bool needLog = Properties.Settings.Default.isLogMeasurements;

                SpherePlotModel electronModel = new SpherePlotModel(PLOT.ELECTRON_LINE, needLog);
                electronPlotView.Model = electronModel.CurrentModel;

                SpherePlotModel ionPlusModel = new SpherePlotModel(PLOT.ION_PLUS_LINE, needLog);
                ionPositivePlotView.Model = ionPlusModel.CurrentModel;

                SpherePlotModel ionMinusModel = new SpherePlotModel(PLOT.ION_MINUS_LINE, needLog);
                ionNegativePlotView.Model = ionMinusModel.CurrentModel;

                SpherePlotModel allModel = new SpherePlotModel(PLOT.ALL_LINE, needLog);
                allChargesPlotView.Model = allModel.CurrentModel;

                SpherePlotModel electronVelocityModel = new SpherePlotModel(PLOT.ELECTRON_VELOCITY_LINE, needLog); //PLOT.ELECTRON_VELOCITY_LINE
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
            
        }

        private void OnCalculationsEnded(object sender, SphereEventArgs e) {
            writeToStatusBar(Properties.Resources.CalculationsEndText);
            MessageBoxResult alert = MessageBox.Show(Properties.Resources.CalculationsEndAlertDescText, Properties.Resources.CalculationsEndText, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            double electicity = Engine.Engine.Instance.getElectro();
            resultTextBlock.Text = String.Format("{0} = {1}", Zeus.Properties.Resources.Electricity, electicity.ToString("#.###E0"));
            clearStatusWithDelay(2000);
            drawPlots();
        }

        private void OnProgressValueChanged(object sender, SphereEventArgs e) {
            this.Dispatcher.Invoke(new Action(delegate() {
                this.updateStatus(e);
            }));
        }

        private void updateStatus(SphereEventArgs e) {
            double statusPercentage = ((double)((int)e.state + 1) / (double)Engine.Engine.Instance.lowAtmosphere.capacity) * 100;
            string status = string.Empty;
            double number = 0;
            switch (e.process) {
                case PROCESS.CALCULATION_MAIN:
                    status = Properties.Resources.CalculationsCurrentState;
                    number = e.state;
                    break;
                case PROCESS.PRECALCULATION_N:
                    status = Properties.Resources.PreCalculateNProgress;
                    number = e.height;
                    break;
                case PROCESS.PRECALCULATION_Q:
                    status = Properties.Resources.PreCalculateQProgress;
                    number = e.height;
                    break;
                default:
                    status = Properties.Resources.CalculationsCurrentState;
                    number = e.state;
                    break;
            }
            writeToStatusBar(String.Format(status, number));
            progressBar.Value = statusPercentage;
            procentLabel.Content = Math.Round(statusPercentage).ToString() + " %";
            // Если все закончилось
            if ((e.process == PROCESS.PRECALCULATION_Q) && (statusPercentage == 100)) {
                writeToStatusBar(Properties.Resources.PreCalculationsEnd);
                clearStatusWithDelay(2000);
            }
        }

        private async void clearStatusWithDelay(int delay) {
            await Task.Delay(delay);
            statusLabel.Content = "";
            procentLabel.Content = "";
            progressBar.Value = 0;
            progressBar.Visibility = Visibility.Hidden;
        }

        private void OnSave(object sender, RoutedEventArgs e) {
            string filename = string.Empty;
            if (Properties.Settings.Default.isSpecifySaveLocation == true) {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Json files (*.json)|*.json";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == true) {
                    filename = saveFileDialog.FileName;
                }
            }
            else {
                filename = Constants.appJsonPath + "/output.json";
            }

            if (string.IsNullOrEmpty(filename) == false) {
                // Сохраняем в файл
                LogManager.Session.logMessage("Saving to " + filename + " output file");
                try {
                    Engine.Engine.Instance.saveToFile(filename);
                    writeToStatusBar(Zeus.Properties.Resources.SaveStatusSucces);
                    clearStatusWithDelay(2000);
                }
                catch (IOException error) {
                    MessageBoxResult alert = MessageBox.Show(error.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.Session.logMessage("Cannot save output file to " + filename + " cause of " + error.Message);
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
                    launchButton.IsEnabled = true;
                    progressBar.Visibility = Visibility.Visible;
                    bool error = Engine.Engine.Instance.initSphereWithInputFile(openFileDialog.FileName);
                    if (error == false) {
                        MessageBoxResult alert = MessageBox.Show(Properties.Resources.ErrorSeeLogs, Properties.Resources.ErrorCritical, MessageBoxButton.OK, MessageBoxImage.Error);
                        if (alert == MessageBoxResult.OK || alert == MessageBoxResult.Cancel) {
                            Application.Current.Shutdown();
                        }
                    }
                    else {
                        Engine.Engine.Instance.lowAtmosphere.preCalculateNProgessChanged += OnProgressValueChanged;
                        Engine.Engine.Instance.lowAtmosphere.preCalculateQProgessChanged += OnProgressValueChanged;
                        Thread thread = new Thread(Engine.Engine.Instance.preCaluculate);
                        thread.Start();
                        setInformation();
                        Engine.Engine.Instance.lowAtmosphere.stateCalculated += OnProgressValueChanged;
                        Engine.Engine.Instance.lowAtmosphere.calculationsDone += OnCalculationsEnded;
                    }
                }
                catch (IOException error) {
                    MessageBoxResult alert = MessageBox.Show(error.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    LogManager.Session.logMessage("Cannot open input file " + openFileDialog.FileName + " cause of " + error.Message);
                }
                
            }
        }

        // Запуск расчета
        private void OnLaunch(object sender, RoutedEventArgs e) {
            progressBar.Visibility = Visibility.Visible;
            Engine.Engine.Instance.launchComputations();
        }

        private void OnClose(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnAbout(object sender, RoutedEventArgs e) {
            AboutWindow about = new AboutWindow();
            about.Show();
        }

        private void OnSettings(object sender, RoutedEventArgs e) {
            SettingsWindow settings = new SettingsWindow();
            settings.Closing += updateMainForSettings;
            settings.Show();
        }

        private void updateMainForSettings(object sender, CancelEventArgs e) {
            if (Properties.Settings.Default.isShowAdditionalPlots == false) {
                hideVelocities();
            }
            else {
                showVelocities();
            }
            drawPlots();
        }

        private void OnHelp(object sender, RoutedEventArgs e) {
            LogManager.Session.logMessage("Opening help file");
            try {
                System.Diagnostics.Process.Start(Constants.appHelpPath + "ZeusHelp.chm");
            }
            catch (IOException error) {
                MessageBoxResult alert = MessageBox.Show(error.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                LogManager.Session.logMessage("Cannot open help file cause of " + error.Message);
            }
        }
    }
}
