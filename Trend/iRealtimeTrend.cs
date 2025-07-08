using ATSCADA.ToolExtensions.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace ATSCADA.iWinTools.Trend
{
    public partial class iRealtimeTrend : UserControl
    {
        private string collection = "";

        private List<TrendParametter> trendParametters = new List<TrendParametter>();

        private List<TrendTag> trendTags;

        private bool gridLine = false;

        private Color panelColorInitial = Color.White;

        private Color panelColor = Color.Transparent;

        private iDriver driver;

        [Category("ATSCADA Settings")]
        [Description("Select driver object.")]
        public iDriver Driver
        {
            get => driver;
            set
            {
                if (driver != null) driver.ConstructionCompleted -= Driver_ConstructionCompleted;
                driver = value;
                if (driver != null) driver.ConstructionCompleted += Driver_ConstructionCompleted;
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Select tags to draw realtime trends.")]
        [Editor(typeof(RealtimeTrendSettingsEditor), typeof(UITypeEditor))]
        public string Collection
        {
            get => this.collection;
            set
            {
                if (this.collection == value) return;

                this.collection = value;
                Deserialization();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Select type of data updating: By Timer, By Event or By Both")]
        public UpdateType UpdateType { get; set; } = UpdateType.Timer;

        [Category("ATSCADA Settings")]
        [Description("This is for the rate (milisecond) of timer if updating by Timer")]
        public double TimerRate { get; set; } = 1000;

        [Category("ATSCADA Settings")]
        [Description("Sample number in a trend.")]
        public uint SampleNumInTrend { get; set; } = 30;

        [Category("ATSCADA Settings")]
        [Description("Allow or not allow gridline view")]
        public bool GridLine
        {
            get => this.gridLine;
            set
            {
                if (this.gridLine == value) return;
                this.gridLine = value;

                if (value)
                {
                    trendViewer.GraphPane.XAxis.MajorGrid.IsVisible = true;
                    trendViewer.GraphPane.YAxis.MajorGrid.IsVisible = true;
                }
                else
                {
                    trendViewer.GraphPane.XAxis.MajorGrid.IsVisible = false;
                    trendViewer.GraphPane.YAxis.MajorGrid.IsVisible = false;
                }

                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Choose color init for the trend panel.")]
        [TypeConverter(typeof(Color))]
        public Color PanelColorInitial
        {
            get { return this.panelColorInitial; }
            set
            {
                if (this.panelColorInitial == value) return;

                this.panelColorInitial = value;
                trendViewer.GraphPane.Chart.Fill = new Fill(this.panelColorInitial, this.panelColor, -45F);

                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Choose color for the trend panel.")]
        [TypeConverter(typeof(Color))]
        public Color PanelColor
        {
            get { return this.panelColor; }
            set
            {
                if (this.panelColor == value) return;

                this.panelColor = value;
                trendViewer.GraphPane.Chart.Fill = new Fill(this.panelColorInitial, this.panelColor, -45F);

                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Title of trend.")]
        public string Title
        {
            get => trendViewer.GraphPane.Title.Text;
            set
            {
                if (trendViewer.GraphPane.Title.Text == value) return;

                trendViewer.GraphPane.Title.Text = value;
                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Title of datetime axis.")]
        public string DateTimeAxisTitle
        {
            get => trendViewer.GraphPane.XAxis.Title.Text;
            set
            {
                if (trendViewer.GraphPane.XAxis.Title.Text == value) return;

                trendViewer.GraphPane.XAxis.Title.Text = value;
                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Title of value axis.")]
        public string ValueAxisTitle
        {
            get => trendViewer.GraphPane.YAxis.Title.Text;
            set
            {
                if (trendViewer.GraphPane.YAxis.Title.Text == value) return;

                trendViewer.GraphPane.YAxis.Title.Text = value;
                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Format display of datetime.")]
        public string FormatDateTime
        {
            get => trendViewer.GraphPane.XAxis.Scale.Format;
            set
            {
                if (trendViewer.GraphPane.XAxis.Scale.Format == value) return;

                trendViewer.GraphPane.XAxis.Scale.Format = value;
                trendViewer.Invalidate();
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Symbol type of line char.")]
        public SymbolType SymbolType { get; set; } = SymbolType.Circle;

        [Category("ATSCADA Settings")]
        [Description("Format display of value.")]
        public string FormatValue { get; set; } = "0.00";

        public iRealtimeTrend()
        {
            InitializeComponent();

            trendViewer.GraphPane.XAxis.Type = AxisType.Date;
            trendViewer.GraphPane.Title.Text = "Realtime Trend";
            trendViewer.GraphPane.XAxis.Title.Text = "DateTime";
            trendViewer.GraphPane.YAxis.Title.Text = "Value";
            trendViewer.GraphPane.XAxis.Scale.Format = "dd/MM/yyyy \n HH:mm:ss";

            trendViewer.PointValueEvent += TrendViewer_PointValueEvent;
        }

        private string TrendViewer_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            var dateTime = System.DateTime.FromOADate(curve[iPt].X).ToString(FormatDateTime);
            var value = curve[iPt].Y.ToString(FormatValue);
            return $"Name : {curve.Label.Text}\r{DateTimeAxisTitle} : {dateTime}\r{ValueAxisTitle} : {value}";
        }

        private void Driver_ConstructionCompleted()
        {
            if (this.trendParametters.Count == 0) return;

            this.trendTags = this.trendParametters.Select(
                x => new TrendTag(this.driver, x)).ToList();

            if (this.trendTags == null) return;
            if (this.trendTags.Count == 0) return;

            ActionUpdateTrend actionUpdateTrend = ActionUpdateTrendFactory.GetActionUpdateTrend(
                trendTags,
                SampleNumInTrend,
                TimerRate,
                UpdateType);

            actionUpdateTrend.Updated += (sender, e) => UpdateTrend();
            actionUpdateTrend.Start();
        }

        private void Deserialization()
        {
            var dataSerialization = Collection.Split('|');
            var countItems = dataSerialization.Length;
            if (countItems == 0) return;

            this.trendParametters.Clear();
            for (int index = 0; index < countItems; index++)
            {
                var data = dataSerialization[index].Split('&');
                if (data.Length != 6) continue;

                var tagName = data[0];
                var alias = data[1];
                var resultParseType = Enum.TryParse(data[2], out TrendType type);
                var fillColor = Color.FromName(data[3]);
                var lineColor = Color.FromName(data[4]);
                var resultParseLineWidth = float.TryParse(data[5], out float lineWidth);

                if (!resultParseType || !resultParseLineWidth) return;

                this.trendParametters.Add(new TrendParametter()
                {
                    TagName = tagName,
                    Alias = alias,
                    Type = type,
                    FillColor = fillColor,
                    LineColor = lineColor,
                    LineWidth = lineWidth
                });
            }
        }

        private void UpdateTrend()
        {
            this.SynchronizedInvokeAction(() =>
            {
                trendViewer.GraphPane.CurveList.Clear();

                var countTrendPoint = this.trendTags[0].TrendPoints.Count;
                trendViewer.GraphPane.XAxis.Scale.Min = ConvertDateToXdate(this.trendTags[0].TrendPoints[0].TimeStamp);
                trendViewer.GraphPane.XAxis.Scale.Max = ConvertDateToXdate(this.trendTags[0].TrendPoints[countTrendPoint - 1].TimeStamp);

                foreach (var trendTag in this.trendTags)
                {
                    switch (trendTag.Parametter.Type)
                    {
                        case TrendType.Line:
                            UpdateTrendLine(trendTag);
                            break;
                        case TrendType.Bar:
                            UpdateTrendBar(trendTag);
                            break;
                        default:
                            break;
                    }
                }

                this.trendViewer.AxisChange();
                this.trendViewer.Invalidate();
            });                      
        }

        private XDate ConvertDateToXdate(System.DateTime date)
        {
            return new XDate(date.ToOADate());
        }

        private void UpdateTrendLine(TrendTag trendTag)
        {
            var pointPairs = new PointPairList();
            foreach (var trendPoint in trendTag.TrendPoints)
            {
                var timeStamp = ConvertDateToXdate(trendPoint.TimeStamp);
                if (double.TryParse(trendPoint.Value, out double value))
                    pointPairs.Add(timeStamp, value);
            }

            var lineItem = trendViewer.GraphPane.AddCurve(
                trendTag.Parametter.Alias,
                pointPairs,
                trendTag.Parametter.LineColor,
                SymbolType);

            lineItem.Line.Fill = new Fill(Color.White, trendTag.Parametter.FillColor, 45F);
            lineItem.Line.Width = trendTag.Parametter.LineWidth;
            lineItem.Symbol.Fill = new Fill(Color.White);
        }

        private void UpdateTrendBar(TrendTag trendTag)
        {
            var pointPairs = new PointPairList();
            foreach (var trendPoint in trendTag.TrendPoints)
            {
                var timeStamp = ConvertDateToXdate(trendPoint.TimeStamp);
                if (double.TryParse(trendPoint.Value, out double value))
                    pointPairs.Add(timeStamp, value);
            }

            var barItem = trendViewer.GraphPane.AddBar(
                trendTag.Parametter.Alias,
                pointPairs,
                trendTag.Parametter.LineColor);

            barItem.Bar.Fill = new Fill(trendTag.Parametter.FillColor, Color.White, trendTag.Parametter.FillColor);
        }
    }
}
