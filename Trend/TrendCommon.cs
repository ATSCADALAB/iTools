using ATSCADA.ToolExtensions.Data;
using ATSCADA.ToolExtensions.PropertyEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Trend
{
    public enum UpdateType
    {
        Timer,
        Event,
        All
    }

    public enum TrendType
    {
        Line,
        Bar
    }

    public class TrendPoint
    {
        public System.DateTime TimeStamp { get; set; }

        public string Value { get; set; }

    }

    public class TrendParametter
    {
        public string TagName { get; set; }

        public string Alias { get; set; }

        public TrendType Type { get; set; }

        public Color FillColor { get; set; }

        public Color LineColor { get; set; }

        public float LineWidth { get; set; }        
    }

    public class TrendUpdatedEventArgs : EventArgs
    {
        public List<TrendTag> TrendTags { get; set; }

        public uint Limit { get; set; }
    }

    public abstract class ActionUpdateTrend
    {
        protected List<TrendTag> trendTags;

        protected uint limit;

        public event EventHandler<TrendUpdatedEventArgs> Updated;

        public ActionUpdateTrend(List<TrendTag> trendTags, uint limit)
        {
            this.trendTags = trendTags;
            this.limit = limit;
        }

        public abstract void Start();

        public void Update()
        {
            if (this.trendTags == null) return;
            if (this.trendTags.Count == 0) return;

            foreach (var trendTag in this.trendTags)
                trendTag.Update(limit);

            OnStatusChanged(new TrendUpdatedEventArgs()
            {
                TrendTags = this.trendTags,
                Limit = this.limit
            });
        }

        private void OnStatusChanged(TrendUpdatedEventArgs e)
        {
            EventHandler<TrendUpdatedEventArgs> handler;
            lock (this) { handler = Updated; }
            handler?.Invoke(this, e);
        }
    }

    public class UpdateTrendByTimer : ActionUpdateTrend
    {        
        private readonly System.Timers.Timer tmrUpdate;
        public UpdateTrendByTimer(List<TrendTag> trendTags, uint limit, double timeRate)
            : base(trendTags, limit)
        {            
            this.tmrUpdate = new System.Timers.Timer();
            this.tmrUpdate.AutoReset = false;
            this.tmrUpdate.Interval = timeRate;
            this.tmrUpdate.Elapsed += TmrUpdate_Elapsed;
        }

        public override void Start()
        {
            if (this.trendTags == null) return;
            if (this.trendTags.Count == 0) return;
            
            this.tmrUpdate.Start();
        }

        private void TmrUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.tmrUpdate.Stop();
                Update();
            }
            finally
            {                
                this.tmrUpdate.Start();
            }
        }
    }

    public class UpdateTrendByEvent : ActionUpdateTrend
    {        
        public UpdateTrendByEvent(List<TrendTag> trendTags, uint limit)
            : base(trendTags, limit)
        {    
        }

        public override void Start()
        {
            if (this.trendTags == null) return;
            if (this.trendTags.Count == 0) return;

            foreach (var trendTag in this.trendTags)
                if (trendTag.DataTag.IsTag)
                    trendTag.DataTag.Tag.TagValueChanged += (sender, e) => Update();
        }
    }

    public class UpdateTrendByAll : ActionUpdateTrend
    {       
        private readonly System.Timers.Timer tmrUpdate;
        public UpdateTrendByAll(List<TrendTag> trendTags, uint limit, double timeRate)
            : base(trendTags, limit)
        {
            
            this.tmrUpdate = new System.Timers.Timer();
            this.tmrUpdate.AutoReset = false;
            this.tmrUpdate.Interval = timeRate;
            this.tmrUpdate.Elapsed += TmrUpdate_Elapsed;
        }

        public override void Start()
        {
            if (this.trendTags == null) return;
            if (this.trendTags.Count == 0) return;
            
            this.tmrUpdate.Start();

            foreach (var trendTag in this.trendTags)
                if (trendTag.DataTag.IsTag)
                    trendTag.DataTag.Tag.TagValueChanged += (sender, e) => Update();
        }

        private void TmrUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.tmrUpdate.Stop();
                Update();
            }
            finally
            {                
                this.tmrUpdate.Start();
            }
        }
    }

    public static class ActionUpdateTrendFactory
    {
        public static ActionUpdateTrend GetActionUpdateTrend(
            List<TrendTag> trendTags, 
            uint limit, 
            double timeRate, 
            UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.Timer:
                    return new UpdateTrendByTimer(trendTags, limit, timeRate);
                case UpdateType.Event:
                    return new UpdateTrendByEvent(trendTags, limit);
                case UpdateType.All:
                    return new UpdateTrendByAll(trendTags, limit, timeRate);
                default:
                    return new UpdateTrendByTimer(trendTags, limit, timeRate);
            }
        }
    }

    public class RealtimeTrendSettingsEditor : PropertyEditorBase
    {
        private frmRealtimeTrendSettings control;

        protected override Control GetEditControl(string PropertyName, object CurrentValue)
        {
            this.control = new frmRealtimeTrendSettings()
            {
                DataSerialization = (string)CurrentValue
            };

            return this.control;
        }

        protected override object GetEditedValue(Control EditControl, string PropertyName, object OldValue)
        {
            if (this.control == null) return OldValue;
            return this.control.IsCanceled ? OldValue : this.control.DataSerialization;
        }
    }
}
