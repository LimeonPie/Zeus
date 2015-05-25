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
        ELECTRON_BAR,
        ELECTRON_LINE,
        ION_PLUS_BAR,
        ION_PLUS_LINE,
        ION_MINUS_BAR,
        ION_MINUS_LINE,
        ALL_LINE,
        ALL_BAR,
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
                case PLOT.ELECTRON_BAR:
                    createElectronBarModel();
                    break;
                case PLOT.ION_PLUS_BAR:
                    createIonPlusBarModel();
                    break;
                case PLOT.ION_MINUS_BAR:
                    createIonMinusBarModel();
                    break;
                case PLOT.ELECTRON_LINE:
                    createElectronPolyLineModel();
                    break;
                case PLOT.ION_PLUS_LINE:
                    createIonPlusPolyLineModel();
                    break;
                case PLOT.ION_MINUS_LINE:
                    createIonMinusPolyLineModel();
                    break;
                case PLOT.ALL_LINE:
                    createAllPolyLineModel();
                    break;
                case PLOT.ALL_BAR:
                    createAllBarModel();
                    break;
                default:
                    createAllBarModel();
                    break;
            }
        }

        public void createElectronPolyLineModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.ElectronConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double height = i * Engine.Engine.Instance.lowAtmosphere.delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.neGrid[i].value;
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createElectronBarModel() {
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
                double value = Engine.Engine.Instance.lowAtmosphere.neGrid[i].value;
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createIonPlusPolyLineModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.PositiveIonConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double height = i * Engine.Engine.Instance.lowAtmosphere.delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.nipGrid[i].value;
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createIonPlusBarModel() {
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
                double value = Engine.Engine.Instance.lowAtmosphere.nipGrid[i].value;
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createIonMinusPolyLineModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.NegativeIonConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double height = i * Engine.Engine.Instance.lowAtmosphere.delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.ninGrid[i].value;
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createIonMinusBarModel() {
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
                double value = Engine.Engine.Instance.lowAtmosphere.ninGrid[i].value;
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((Engine.Engine.Instance.lowAtmosphere.delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createAllPolyLineModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.AllChargesConc;
            this.CurrentModel.LegendPlacement = LegendPlacement.Outside;
            this.CurrentModel.LegendPosition = LegendPosition.BottomCenter;
            this.CurrentModel.LegendOrientation = LegendOrientation.Horizontal;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries electronSeries = new LineSeries();
            electronSeries.Title = Zeus.Properties.Resources.ElectronConcShort;
            electronSeries.Color = OxyColors.Blue;
            LineSeries ionPlusSeries = new LineSeries();
            ionPlusSeries.Title = Zeus.Properties.Resources.PositiveIonShort;
            ionPlusSeries.Color = OxyColors.Red;
            LineSeries ionMinusSeries = new LineSeries();
            ionMinusSeries.Title = Zeus.Properties.Resources.NegativeIonShort;
            ionMinusSeries.Color = OxyColors.Green;

            for (int i = 0; i < Engine.Engine.Instance.lowAtmosphere.capacity; i++) {
                double height = i * Engine.Engine.Instance.lowAtmosphere.delta;
                double electron = Engine.Engine.Instance.lowAtmosphere.neGrid[i].value;
                double ionPlus = Engine.Engine.Instance.lowAtmosphere.nipGrid[i].value;
                double ionMinus = Engine.Engine.Instance.lowAtmosphere.ninGrid[i].value;
                electronSeries.Points.Add(new DataPoint(electron, height));
                ionPlusSeries.Points.Add(new DataPoint(ionPlus, height));
                ionMinusSeries.Points.Add(new DataPoint(ionMinus, height));
            }
            this.CurrentModel.Series.Add(electronSeries);
            this.CurrentModel.Series.Add(ionPlusSeries);
            this.CurrentModel.Series.Add(ionMinusSeries);
        }

        private void createAllBarModel() {
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
                double electron = Engine.Engine.Instance.lowAtmosphere.neGrid[i].value;
                double ionPlus = Engine.Engine.Instance.lowAtmosphere.nipGrid[i].value;
                double ionMinus = Engine.Engine.Instance.lowAtmosphere.ninGrid[i].value;
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
