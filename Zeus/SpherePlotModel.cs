using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using Zeus.Engine;

namespace Zeus
{

    // Класс для представления модели графиков

    public enum PLOT
    {
        ELECTRON,
        ION_PLUS,
        ION_MINUS,
        ALL,
    };

    class SpherePlotModel
    {

        // TODO!!!
        // Настроить 

        public PlotModel CurrentModel { get; private set; }

        public SpherePlotModel(string title, double[] data) {
            this.CurrentModel = new PlotModel { Title = title };
            ColumnSeries series = new ColumnSeries();
            foreach (double value in data) {
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
            }
            this.CurrentModel.Series.Add(series);
        }

        public SpherePlotModel(PLOT plotType) {
            this.CurrentModel = new PlotModel();
            switch (plotType) {
                case PLOT.ELECTRON:
                    createElectronModel();
                    break;
                case PLOT.ION_PLUS:
                    createIonPlusModel();
                    break;
                case PLOT.ALL:
                    createAllModel();
                    break;
                case PLOT.ION_MINUS:
                    createIonMinusModel();
                    break;
                default:
                    createAllModel();
                    break;
            }
        }

        private void createElectronModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.ElectronConc;

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(yAxis);

            CategoryAxis xAxis = new CategoryAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Bottom,
            };

            ColumnSeries series = new ColumnSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.neGrid[i];
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        private void createIonPlusModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.PositiveIonConc;

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(yAxis);

            CategoryAxis xAxis = new CategoryAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Bottom,
            };

            ColumnSeries series = new ColumnSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.nipGrid[i];
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        private void createIonMinusModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.NegativeIonConc;

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(yAxis);

            CategoryAxis xAxis = new CategoryAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Bottom,
            };
            
            ColumnSeries series = new ColumnSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.ninGrid[i];
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        private void createAllModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.AllChargesConc;
            this.CurrentModel.LegendPlacement = LegendPlacement.Outside;
            this.CurrentModel.LegendPosition = LegendPosition.BottomCenter;
            this.CurrentModel.LegendOrientation = LegendOrientation.Horizontal;

            ColumnSeries electronSeries = new ColumnSeries();
            electronSeries.Title = Zeus.Properties.Resources.ElectronConcShort;
            ColumnSeries ionPlusSeries = new ColumnSeries();
            ionPlusSeries.Title = Zeus.Properties.Resources.PositiveIonShort;
            ColumnSeries ionMinusSeries = new ColumnSeries();
            ionMinusSeries.Title = Zeus.Properties.Resources.NegativeIonShort;

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(yAxis);

            CategoryAxis xAxis = new CategoryAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Bottom,
            };

            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double electron = Engine.Engine.Instance.lowAtmosphere.neGrid[i];
                double ionPlus = Engine.Engine.Instance.lowAtmosphere.nipGrid[i];
                double ionMinus = Engine.Engine.Instance.lowAtmosphere.ninGrid[i];
                electronSeries.Items.Add(new ColumnItem(electron));
                ionPlusSeries.Items.Add(new ColumnItem(ionPlus));
                ionMinusSeries.Items.Add(new ColumnItem(ionMinus));

                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }

            this.CurrentModel.Series.Add(electronSeries);
            this.CurrentModel.Series.Add(ionPlusSeries);
            this.CurrentModel.Series.Add(ionMinusSeries);

            this.CurrentModel.Axes.Add(xAxis);
        }

    }
}
