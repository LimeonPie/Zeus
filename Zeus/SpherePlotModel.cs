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
        FLUX_LINE,
        ACTIVE_ELEMENT_LINE,
        AEROSOL_LINE,
        ELECTRON_VELOCITY_LINE,
        ION_PLUS_VELOCITY_LINE,
        ION_MINUS_VELOCITY_LINE,
        TEMPERATURE_HEAT,
    };

    class SpherePlotModel
    {

        public PlotModel CurrentModel { get; private set; }
        public double delta { get; set; }
        public double capacity { get; set; }

        public SpherePlotModel(string title, double[] data) {
            this.CurrentModel = new PlotModel { Title = title };
            ColumnSeries series = new ColumnSeries();
            foreach (double value in data) {
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
            }
            this.CurrentModel.Series.Add(series);
        }

        public SpherePlotModel(PLOT plotType, bool needLog) {
            this.CurrentModel = new PlotModel();
            /*if (Engine.Engine.Instance.lowAtmosphere.delta < 1000) {
                delta = 1000;
                capacity = (Engine.Engine.Instance.lowAtmosphere.topBoundary - Engine.Engine.Instance.lowAtmosphere.botBoundary) / delta + 1;
            }*/
            //else {
                delta = Engine.Engine.Instance.lowAtmosphere.delta;
                capacity = Engine.Engine.Instance.lowAtmosphere.capacity;
            //}
            switch (plotType) {
                case PLOT.ELECTRON_BAR:
                    createElectronBarModel(needLog);
                    break;
                case PLOT.ELECTRON_LINE:
                    createElectronPolyLineModel(needLog);
                    break;
                case PLOT.ELECTRON_VELOCITY_LINE:
                    createElectronVelocityPolyLineModel(needLog);
                    break;
                case PLOT.ION_PLUS_BAR:
                    createIonPlusBarModel(needLog);
                    break;
                case PLOT.ION_PLUS_LINE:
                    createIonPlusPolyLineModel(needLog);
                    break;
                case PLOT.ION_PLUS_VELOCITY_LINE:
                    createIonPlusVelocityPolyLineModel(needLog);
                    break;
                case PLOT.ION_MINUS_BAR:
                    createIonMinusBarModel(needLog);
                    break;
                case PLOT.ION_MINUS_LINE:
                    createIonMinusPolyLineModel(needLog);
                    break;
                case PLOT.ION_MINUS_VELOCITY_LINE:
                    createIonMinusVelocityPolyLineModel(needLog);
                    break;
                case PLOT.ALL_LINE:
                    createAllPolyLineModel(needLog);
                    break;
                case PLOT.ALL_BAR:
                    createAllBarModel(needLog);
                    break;
                case PLOT.FLUX_LINE:
                    createFluxModel(needLog);
                    break;
                case PLOT.ACTIVE_ELEMENT_LINE:
                    createActiveElementModel(needLog);
                    break;
                case PLOT.AEROSOL_LINE:
                    createAerosolModel(needLog);
                    break;
                case PLOT.TEMPERATURE_HEAT:
                    createTemperatureHeatModel();
                    break;
                default:
                    createAllBarModel(needLog);
                    break;
            }
        }

        public void createFluxModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.EternityFlux;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.EternityFlux,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();

            for (int i = 0; i < capacity; i++) {
                double height = i * delta;
                double flux = 0;
                string key = Engine.Engine.Instance.lowAtmosphere.activeElement.photonCrossSections.Keys.ElementAt(0);
                //flux = Engine.Engine.Instance.lowAtmosphere.photonFlux(Engine.Engine.Instance.lowAtmosphere.activeElement, key, height);
                flux = Engine.Engine.Instance.lowAtmosphere.photonFlux3(Engine.Engine.Instance.lowAtmosphere.activeElement, height);
                //flux = Engine.Engine.Instance.lowAtmosphere.qNIST(Engine.Engine.Instance.lowAtmosphere.activeElement, height);
                if (needLog) flux = Math.Log10(flux);
                series.Points.Add(new DataPoint(flux, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        public void createElectronPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.ElectronConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 1; i < capacity; i++) {
                double height = i * delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.electronGrid[i].value;
                if (needLog) conc = Math.Log10(conc);
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createElectronBarModel(bool needLog) {
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
            for (int i = 1; i < capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.electronGrid[i].value;
                if (needLog) value = Math.Log10(value);
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createIonPlusPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.PositiveIonConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 1; i < capacity; i++) {
                double height = i * delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.ionPlusGrid[i].value;
                if (needLog) conc = Math.Log10(conc);
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createIonPlusBarModel(bool needLog) {
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
            for (int i = 1; i < capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.ionPlusGrid[i].value;
                if (needLog) value = Math.Log10(value);
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createIonMinusPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.NegativeIonConc;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 1; i < capacity; i++) {
                double height = i * delta;
                double conc = Engine.Engine.Instance.lowAtmosphere.ionMinusGrid[i].value;
                if (needLog) conc = Math.Log10(conc);
                series.Points.Add(new DataPoint(conc, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        private void createIonMinusBarModel(bool needLog) {
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
            for (int i = 1; i < capacity; i++) {
                double value = Engine.Engine.Instance.lowAtmosphere.ionMinusGrid[i].value;
                if (needLog) value = Math.Log10(value);
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
                xAxis.Labels.Add((delta * i).ToString());
            }
            this.CurrentModel.Series.Add(series);
            this.CurrentModel.Axes.Add(xAxis);
        }

        public void createAllPolyLineModel(bool needLog) {
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
            if (needLog) xAxis.StringFormat = "#.#";
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

            for (int i = 1; i < capacity; i++) {
                double height = i * delta;
                double electron = Engine.Engine.Instance.lowAtmosphere.electronGrid[i].value;
                double ionPlus = Engine.Engine.Instance.lowAtmosphere.ionPlusGrid[i].value;
                double ionMinus = Engine.Engine.Instance.lowAtmosphere.ionMinusGrid[i].value;
                if (needLog) {
                    electron = Math.Log10(electron);
                    ionPlus = Math.Log10(ionPlus);
                    ionMinus = Math.Log10(ionMinus);
                }
                electronSeries.Points.Add(new DataPoint(electron, height));
                ionPlusSeries.Points.Add(new DataPoint(ionPlus, height));
                ionMinusSeries.Points.Add(new DataPoint(ionMinus, height));
            }
            this.CurrentModel.Series.Add(electronSeries);
            this.CurrentModel.Series.Add(ionPlusSeries);
            this.CurrentModel.Series.Add(ionMinusSeries);
        }

        private void createAllBarModel(bool needLog) {
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

            for (int i = 1; i < capacity; i++) {
                double electron = Engine.Engine.Instance.lowAtmosphere.electronGrid[i].value;
                double ionPlus = Engine.Engine.Instance.lowAtmosphere.ionPlusGrid[i].value;
                double ionMinus = Engine.Engine.Instance.lowAtmosphere.ionMinusGrid[i].value;
                if (needLog) {
                    electron = Math.Log10(electron);
                    ionPlus = Math.Log10(ionPlus);
                    ionMinus = Math.Log10(ionMinus);
                }
                electronSeries.Items.Add(new ColumnItem(electron));
                ionPlusSeries.Items.Add(new ColumnItem(ionPlus));
                ionMinusSeries.Items.Add(new ColumnItem(ionMinus));

                xAxis.Labels.Add((delta * i).ToString());
            }

            this.CurrentModel.Series.Add(electronSeries);
            this.CurrentModel.Series.Add(ionPlusSeries);
            this.CurrentModel.Series.Add(ionMinusSeries);

            this.CurrentModel.Axes.Add(xAxis);
        }

        private void createAerosolModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.Aerosol;
            this.CurrentModel.LegendPlacement = LegendPlacement.Outside;
            this.CurrentModel.LegendPosition = LegendPosition.BottomCenter;
            this.CurrentModel.LegendOrientation = LegendOrientation.Horizontal;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries[] aerosolSeries = new LineSeries[Engine.Engine.Instance.lowAtmosphere.aerosolElements.Count];

            for (int i = 0; i < aerosolSeries.Length; i++) {
                aerosolSeries[i] = new LineSeries();
                aerosolSeries[i].Title = Engine.Engine.Instance.lowAtmosphere.aerosolElements.ElementAt(i).name;
            }

            for (double height = 0; height <= Engine.Engine.Instance.lowAtmosphere.topBoundary; height += 5000) {

                for (int i = 0; i < aerosolSeries.Length; i++) {
                    double concentration = Engine.Engine.Instance.lowAtmosphere.aerosolElements.ElementAt(i).getNForHeight(height);
                    if (needLog) concentration = Math.Log10(concentration);
                    aerosolSeries[i].Points.Add(new DataPoint(concentration, height));
                }
            }

            foreach (LineSeries series in aerosolSeries) {
                this.CurrentModel.Series.Add(series);
            }
        }

        private void createActiveElementModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.Nitrogen;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Concentration,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();

            for (double height = 0; height <= Engine.Engine.Instance.lowAtmosphere.topBoundary; height += 5000) {
                double concentration = Engine.Engine.Instance.lowAtmosphere.activeElement.getNForHeight(height);
                if (needLog) concentration = Math.Log10(concentration);
                series.Points.Add(new DataPoint(concentration, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        public void createElectronVelocityPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.ElectronVelocity;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Velocity,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < capacity; i++) {
                double height = i * delta;
                double electronVelocity = Engine.Engine.Instance.lowAtmosphere.electronVelocityGrid[i].value;
                if (needLog) electronVelocity = Math.Log10(electronVelocity);
                series.Points.Add(new DataPoint(electronVelocity, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        public void createIonPlusVelocityPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.IonPlusVelocity;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Velocity,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < capacity; i++) {
                double height = i * delta;
                double ionPlusVelocity = Engine.Engine.Instance.lowAtmosphere.ionPlusVelocityGrid[i].value;
                if (needLog) ionPlusVelocity = Math.Log10(ionPlusVelocity);
                series.Points.Add(new DataPoint(ionPlusVelocity, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        public void createIonMinusVelocityPolyLineModel(bool needLog) {
            this.CurrentModel.Title = Zeus.Properties.Resources.IonMinusVelocity;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Velocity,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                StringFormat = "#.###E0",
            };
            if (needLog) xAxis.StringFormat = "#.#";
            this.CurrentModel.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Height,
                Position = AxisPosition.Left,
                Minimum = 0,
                StringFormat = "#",
            };
            this.CurrentModel.Axes.Add(yAxis);

            LineSeries series = new LineSeries();
            for (int i = 0; i < capacity; i++) {
                double height = i * delta;
                double ionMinusVelocity = Engine.Engine.Instance.lowAtmosphere.ionMinusVelocityGrid[i].value;
                if (needLog) ionMinusVelocity = Math.Log10(ionMinusVelocity);
                series.Points.Add(new DataPoint(ionMinusVelocity, height));
            }
            this.CurrentModel.Series.Add(series);
        }

        public void createTemperatureHeatModel() {
            this.CurrentModel.Title = Zeus.Properties.Resources.TemperatureDistribution;

            LinearAxis xAxis = new LinearAxis() {
                Title = Zeus.Properties.Resources.Temperature,
                Position = AxisPosition.Bottom,
                Minimum = 0,
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
            foreach (string key in Engine.Sphere.temperature.Keys) {
                double height = Convert.ToDouble(key);
                double temperature = Engine.Sphere.temperature[key];
                series.Points.Add(new DataPoint(temperature, height));
            }
            this.CurrentModel.Series.Add(series);
        }
    }
}
