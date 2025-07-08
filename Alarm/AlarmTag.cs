using ATSCADA.ToolExtensions.Data;
using System;

namespace ATSCADA.iWinTools.Alarm
{
    public class AlarmTag
    {
        private readonly Condition[] conditions;

        private readonly DataTool dataTracking;

        private readonly DataTool dataLowLevel;

        private readonly DataTool dataHighLevel;

        public AlarmParametter Parametter { get; }

        public Condition ActiveCondition { get; private set; }

        public event EventHandler<AlarmStatusChangedEventArgs> StatusChanged;

        public AlarmTag(iDriver driver, AlarmParametter parametter)
        {
            Parametter = parametter;

            this.conditions = new Condition[4]
            {
                new Condition(AlarmStatus.Alarm, "Alarm"),
                new Condition(AlarmStatus.SetPoint, "Set Point"),
                new Condition(AlarmStatus.LowAlarm, "Low Alarm"),
                new Condition(AlarmStatus.HighAlarm, "High Alarm")
            };

            this.dataTracking = new DataTool(driver, parametter.Tracking);
            if (!this.dataTracking.IsTag) return;

            ActiveCondition = Condition.Normal;

            this.dataLowLevel = new DataTool(driver, parametter.LowLevel);
            this.dataHighLevel = new DataTool(driver, parametter.HighLevel);

            this.dataTracking.Tag.TagValueChanged += (sender, e) => CheckAlarm(true);
            this.dataTracking.Tag.TagStatusChanged += (sender, e) => CheckAlarm(true);

            //if (this.dataLowLevel.IsTag)
            //{
            //    this.dataLowLevel.Tag.TagValueChanged += (sender, e) => CheckAlarm(true);
            //    this.dataLowLevel.Tag.TagStatusChanged += (sender, e) => CheckAlarm(true);
            //}
                

            //if (this.dataHighLevel.IsTag)
            //{
            //    this.dataHighLevel.Tag.TagValueChanged += (sender, e) => CheckAlarm(true);
            //    this.dataHighLevel.Tag.TagStatusChanged += (sender, e) => CheckAlarm(true);
            //}                

            CheckAlarm(false);
        }

        public void CheckAlarm(bool forceRaiseEvent)
        {
            if (this.dataTracking.Tag.Status != "Good") return;
            if (this.dataLowLevel.IsTag && this.dataLowLevel.Tag.Status != "Good") return;
            if (this.dataHighLevel.IsTag && this.dataHighLevel.Tag.Status != "Good") return;

            if (!double.TryParse(this.dataTracking.Value, out double trackingValue) ||
                !double.TryParse(this.dataLowLevel.Value, out double lowValue) ||
                !double.TryParse(this.dataHighLevel.Value, out double highValue))
                return;

            var timeStamp = System.DateTime.Now;
            var alarmItem = new AlarmItem()
            {
                TrackingName = Parametter.Tracking,
                TrackingAlias = Parametter.Alias,
                TrackingValue = trackingValue,
                LowLevel = lowValue,
                HighLevel = highValue
            };

            if (highValue == lowValue)
            {
                if (highValue == trackingValue)
                {
                    OnAlarm(new AlarmStatusChangedEventArgs()
                    {
                        TimeStamp = timeStamp,
                        Condition = conditions[0],
                        AlarmItem = alarmItem
                    },
                    forceRaiseEvent);
                    return;
                }
            }
            else
            {
                if (highValue < lowValue)
                {
                    OnAlarm(new AlarmStatusChangedEventArgs()
                    {
                        TimeStamp = timeStamp,
                        Condition = conditions[1],
                        AlarmItem = alarmItem
                    },
                    forceRaiseEvent);
                    return;
                }
                else if (trackingValue <= lowValue)
                {
                    OnAlarm(new AlarmStatusChangedEventArgs()
                    {
                        TimeStamp = timeStamp,
                        Condition = conditions[2],
                        AlarmItem = alarmItem
                    },
                    forceRaiseEvent);
                    return;
                }
                else if (trackingValue >= highValue)
                {
                    OnAlarm(new AlarmStatusChangedEventArgs()
                    {
                        TimeStamp = timeStamp,
                        Condition = conditions[3],
                        AlarmItem = alarmItem
                    },
                    forceRaiseEvent);
                    return;
                }
            }

            OffAlarm(new AlarmStatusChangedEventArgs()
            {
                TimeStamp = timeStamp,
                Condition = Condition.Normal,
                AlarmItem = alarmItem
            },
            forceRaiseEvent);
        }

        private void OnAlarm(AlarmStatusChangedEventArgs e, bool forceRaiseEvent)
        {
            if (ActiveCondition.Status != e.Condition.Status)
            {
                ActiveCondition = new Condition(e.Condition.Status, e.Condition.Message);

                if (!forceRaiseEvent) return;
                OnStatusChanged(e);
            }
        }

        private void OffAlarm(AlarmStatusChangedEventArgs e, bool forceRaiseEvent)
        {
            if (ActiveCondition.Status != AlarmStatus.Normal)
            {
                ActiveCondition = Condition.Normal;

                if (!forceRaiseEvent) return;
                OnStatusChanged(e);
            }
        }

        private void OnStatusChanged(AlarmStatusChangedEventArgs e)
        {
            EventHandler<AlarmStatusChangedEventArgs> handler;
            lock (this) { handler = StatusChanged; }
            handler?.Invoke(this, e);
        }
    }
}
