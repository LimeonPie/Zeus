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

namespace Zeus
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow() {
            InitializeComponent();
            if (Properties.Settings.Default.isLogMeasurements == true) {
                logPlotMode.IsChecked = true;
                normalPlotMode.IsChecked = false;
            }
            else {
                normalPlotMode.IsChecked = true;
            }
            specifyPlaceToSave.IsChecked = Properties.Settings.Default.isSpecifySaveLocation;
        }

        private void OnCancel(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnSaveChanges(object sender, RoutedEventArgs e) {
            // Сохраняем настройки размерности графиков
            if (logPlotMode.IsChecked == true) {
                Properties.Settings.Default.isLogMeasurements = true;
            }
            else if (normalPlotMode.IsChecked == true) {
                Properties.Settings.Default.isLogMeasurements = false;
            }

            // Сохраняем настройки выбора места сохранения
            if (specifyPlaceToSave.IsChecked == true) {
                Properties.Settings.Default.isSpecifySaveLocation = true;
            }
            else {
                Properties.Settings.Default.isSpecifySaveLocation = false;
            }
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
