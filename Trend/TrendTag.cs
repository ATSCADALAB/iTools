using ATSCADA.ToolExtensions.Data;
using System.Collections.Generic;

namespace ATSCADA.iWinTools.Trend
{
    public class TrendTag
    {
        public DataTool DataTag { get; }

        public TrendParametter Parametter { get; }

        public List<TrendPoint> TrendPoints { get; }

        public TrendTag(iDriver driver, TrendParametter parametter)
        {
            Parametter = parametter;
            TrendPoints = new List<TrendPoint>();
            DataTag = new DataTool(driver, parametter.TagName);
        }

        public void Update(uint limit)
        {
            var count = TrendPoints.Count;
            while (count > limit)
            {
                TrendPoints.RemoveAt(0);
                count--;
            }
            TrendPoints.Add(new TrendPoint()
            {
                TimeStamp = System.DateTime.Now,
                Value = DataTag.Value
            });
        }
    }
}
