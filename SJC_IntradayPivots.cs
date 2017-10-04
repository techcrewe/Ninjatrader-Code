#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion


namespace NinjaTrader.Indicator
{
    [Description("Rolling Hourly Pivots (created by roonius)")]
    public class SJC_IntradayPivots : Indicator
    {
        #region Variables
			private Bars hourlyBars;
			private bool existsHistHourlyData = false;
			private bool isLoaded = false;
			private bool isInit = false;
			private int minutes = 240;
        // Wizard generated variables
        // User defined varias (add any user defined variables below)
        #endregion

        protected override void Initialize()
        {
            Add(new Plot(Color.Green, PlotStyle.Dot, "HPP"));
            Add(new Plot(Color.DarkRed, PlotStyle.Dot, "HS1"));
            Add(new Plot(Color.DarkRed, PlotStyle.Dot, "HS2"));
			Add(new Plot(Color.DarkRed, PlotStyle.Dot, "HS3"));
            Add(new Plot(Color.Blue, PlotStyle.Dot, "HR1"));
            Add(new Plot(Color.Blue, PlotStyle.Dot, "HR2"));
            Add(new Plot(Color.Blue, PlotStyle.Dot, "HR3"));

            CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= false;
			AutoScale = false;
        }

        protected override void OnBarUpdate()
        {
			
			if (CurrentBars[0] < 121) return;
			if (Bars == null) return; 
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday )	return;
			if (Bars.Period.Id == PeriodType.Minute && Bars.Period.Value > minutes/2) return;
			
			
			
			if(!isLoaded && !isInit)
			{
			isInit = true;
			hourlyBars= Data.Bars.GetBars(Bars.Instrument, new Period(PeriodType.Minute, minutes, MarketDataType.Last), Bars.From, Bars.To, (Session) Bars.Session.Clone(), Data.Bars.SplitAdjust, Data.Bars.DividendAdjust);
			existsHistHourlyData	= (hourlyBars.Count <= 1) ? false : true;
			isInit = false;
			isLoaded = true;
			}
			IBar hourlyBar;
			if (CurrentBar == 0) return;
			if (existsHistHourlyData)
			{
				DateTime intradayBarTime = Time[0].AddMinutes(-minutes);
				hourlyBar = hourlyBars.Get(hourlyBars.GetBar(intradayBarTime));
				double	high		= hourlyBar.High;
				double	low			= hourlyBar.Low;
				double	close		= hourlyBar.Close;
				HPP.Set((high + low + close) / 3);
				if(HPP[0] != HPP[1]) DrawRay("pp", true, 1, HPP[0], 0, HPP[0], Plots[0].Pen.Color, Plots[0].Pen.DashStyle, (int)Plots[0].Pen.Width);
				HS1.Set(2 * HPP[0] - high);
				if(HS1[0] != HS1[1]) DrawRay("hs1", true, 1, HS1[0], 0, HS1[0], Plots[1].Pen.Color, Plots[1].Pen.DashStyle, (int)Plots[1].Pen.Width);
				HR1.Set(2 * HPP[0] - low);
				if(HR1[0] != HR1[1]) DrawRay("hr1", true, 1, HR1[0], 0, HR1[0], Plots[4].Pen.Color, Plots[4].Pen.DashStyle, (int)Plots[4].Pen.Width);
				HS2.Set(HPP[0] - (high - low));
				if(HS2[0] != HS2[1]) DrawRay("hs2", true, 1, HS2[0], 0, HS2[0], Plots[2].Pen.Color, Plots[2].Pen.DashStyle, (int)Plots[2].Pen.Width);
				HR2.Set(HPP[0] + (high - low));
				if(HR2[0] != HR2[1]) DrawRay("hr2", true, 1, HR2[0], 0, HR2[0], Plots[5].Pen.Color, Plots[5].Pen.DashStyle, (int)Plots[5].Pen.Width);
                HS3.Set(HS2[0] - (high - low));
                if (HS3[0] != HS3[1]) DrawRay("hs3", true, 1, HS2[0], 0, HS2[0], Plots[3].Pen.Color, Plots[3].Pen.DashStyle, (int)Plots[3].Pen.Width);
                HR3.Set(HR1[0] + (high - low));
                if (HR3[0] != HR3[1]) DrawRay("hr3", true, 1, HR2[0], 0, HR2[0], Plots[6].Pen.Color, Plots[6].Pen.DashStyle, (int)Plots[6].Pen.Width);
			} else return;
        }

        #region Properties
		[Category("Parameters")]
        public int Minutes
        {
            get { return minutes; }
            set { minutes = Math.Min(960,Math.Max(value,15)); }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HPP
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HS1
        {
            get { return Values[1]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HS2
        {
            get { return Values[2]; }
        }
						
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HS3
        {
            get { return Values[3]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HR1
        {
            get { return Values[4]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HR2
        {
            get { return Values[5]; }
        }
		
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries HR3
        {
            get { return Values[6]; }
        }		
		

		
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SJC_IntradayPivots[] cacheSJC_IntradayPivots = null;

        private static SJC_IntradayPivots checkSJC_IntradayPivots = new SJC_IntradayPivots();

        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        public SJC_IntradayPivots SJC_IntradayPivots(int minutes)
        {
            return SJC_IntradayPivots(Input, minutes);
        }

        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        public SJC_IntradayPivots SJC_IntradayPivots(Data.IDataSeries input, int minutes)
        {
            if (cacheSJC_IntradayPivots != null)
                for (int idx = 0; idx < cacheSJC_IntradayPivots.Length; idx++)
                    if (cacheSJC_IntradayPivots[idx].Minutes == minutes && cacheSJC_IntradayPivots[idx].EqualsInput(input))
                        return cacheSJC_IntradayPivots[idx];

            lock (checkSJC_IntradayPivots)
            {
                checkSJC_IntradayPivots.Minutes = minutes;
                minutes = checkSJC_IntradayPivots.Minutes;

                if (cacheSJC_IntradayPivots != null)
                    for (int idx = 0; idx < cacheSJC_IntradayPivots.Length; idx++)
                        if (cacheSJC_IntradayPivots[idx].Minutes == minutes && cacheSJC_IntradayPivots[idx].EqualsInput(input))
                            return cacheSJC_IntradayPivots[idx];

                SJC_IntradayPivots indicator = new SJC_IntradayPivots();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Minutes = minutes;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_IntradayPivots[] tmp = new SJC_IntradayPivots[cacheSJC_IntradayPivots == null ? 1 : cacheSJC_IntradayPivots.Length + 1];
                if (cacheSJC_IntradayPivots != null)
                    cacheSJC_IntradayPivots.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_IntradayPivots = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_IntradayPivots SJC_IntradayPivots(int minutes)
        {
            return _indicator.SJC_IntradayPivots(Input, minutes);
        }

        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_IntradayPivots SJC_IntradayPivots(Data.IDataSeries input, int minutes)
        {
            return _indicator.SJC_IntradayPivots(input, minutes);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_IntradayPivots SJC_IntradayPivots(int minutes)
        {
            return _indicator.SJC_IntradayPivots(Input, minutes);
        }

        /// <summary>
        /// Rolling Hourly Pivots (created by roonius)
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_IntradayPivots SJC_IntradayPivots(Data.IDataSeries input, int minutes)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_IntradayPivots(input, minutes);
        }
    }
}
#endregion
