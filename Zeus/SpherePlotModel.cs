using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace Zeus
{

    // Класс для представления модели графиков

    class SpherePlotModel
    {

        // TODO!!!
        // Настроить 

        public PlotModel CurrentModel { get; private set; }

        public SpherePlotModel(string title, double[] data) {
            this.CurrentModel = new PlotModel { Title = title };
            List<ColumnItem> list = new List<ColumnItem>();
            ColumnSeries series = new ColumnSeries();
            foreach (double value in data) {
                ColumnItem item = new ColumnItem(value);
                series.Items.Add(item);
            }
            this.CurrentModel.Series.Add(series);
        }
    }
}
