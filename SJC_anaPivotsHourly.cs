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
using System.Collections;
using System.Text;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Plots difference between two user defined instruments.
    /// 2012-Jan-17 Updated to allow Manual entry of times for session start and session end 
    /// 
    /// </summary>
    [Description("The indicator plots intraday pivots based on a customizable period.")]
    public class SJC_anaPivotsHourly : Indicator
    {
        #region Variables
           
			private DateTime sessionBegin	= Cbi.Globals.MinDate;
			private DateTime sessionEnd		= Cbi.Globals.MinDate;
			private int period = 240;
			private string start = "00:00";
			private double high = 0.0;
			private double low = 0.0;
			private double close = 0.0;
			private double pp = 0.0;
			private double range = 0;
			private double r1 = 0.0;
			private double r2 = 0.0;
			private double r3 = 0.0;
			private double r4 = 0.0;
			private double s1 = 0.0;
			private double s2 = 0.0;
			private double s3 = 0.0;
			private double s4 = 0.0;
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.LightGreen, PlotStyle.Dot, "PP"));
			Add(new Plot(Color.LightGray,PlotStyle.Dot, "R1"));
			Add(new Plot(Color.LightGray,PlotStyle.Dot, "R2"));
            Add(new Plot(Color.LightGray, PlotStyle.Dot, "R3"));
			Add(new Plot(Color.Transparent,PlotStyle.Dot, "R4"));
			Add(new Plot(Color.LightSeaGreen,PlotStyle.Dot, "S1"));
			Add(new Plot(Color.LightSeaGreen,PlotStyle.Dot, "S2"));
			Add(new Plot(Color.LightSeaGreen,PlotStyle.Dot, "S3"));
            Add(new Plot(Color.Transparent, PlotStyle.Dot, "S4"));
		
			Plots[0].Pen.DashStyle = DashStyle.Solid;
			Plots[1].Pen.DashStyle = DashStyle.Solid;
			Plots[2].Pen.DashStyle = DashStyle.Solid;
			Plots[3].Pen.DashStyle = DashStyle.Solid;
			Plots[4].Pen.DashStyle = DashStyle.Solid;
			Plots[5].Pen.DashStyle = DashStyle.Solid;
			Plots[6].Pen.DashStyle = DashStyle.Solid;
			Plots[7].Pen.DashStyle = DashStyle.Solid;
			Plots[8].Pen.DashStyle = DashStyle.Solid;            
			
			
			Add(PeriodType.Minute, 1);
			BarsRequired 		= 0;
			CalculateOnBarClose	= true;
            Overlay				= true;
			DisplayInDataBox    = true;
			AutoScale			= false;
        }

       /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if(CurrentBars[0] < BarsRequired || CurrentBars[1] < BarsRequired)
				return;
			if (BarsInProgress == 1)
			{	
				if (CurrentBar < 2*period)
					return;
				
				Bars.Session.GetNextBeginEnd(Times[1][0], out sessionBegin, out sessionEnd);
				
				//sessionDate = Date(sessionBegin);
				//sessionBegin = 
				
				sessionBegin = Convert.ToDateTime(start);
				sessionEnd = Convert.ToDateTime(start);
				
                Print("Session Start: " + sessionBegin + "Session End: " + sessionEnd);
				DateTime barTime 		= Times[1][0];
				TimeSpan inSessionTime 	= barTime.Subtract(sessionBegin);
				int	inSessionMinutes 	= 60*inSessionTime.Hours + inSessionTime.Minutes;
				int cutoff 				= inSessionMinutes % period;
				DateTime cutoffTime 	= barTime.Subtract(new TimeSpan(0,cutoff,0));
				int barsAgo				= CurrentBar - Bars.GetBar(cutoffTime);
				high		= MAX(Highs[1],period)[barsAgo];
				low			= MIN (Lows[1],period)[barsAgo];
				close		= Closes[1][barsAgo];
				pp			= (high + low + close)/3;
				range		= high - low;
				r1			= 2 * pp - low;		
				r2			= pp + range;
				r3			= r1 + range;
				r4 			= r3 + (pp - low);
				s1			= 2 * pp - high;
				s2			= pp - range;
				s3			= s1 - range;
				s4			= s3 - (high - pp);
			}
				
			
			if (BarsInProgress == 0)
			{
				if (CurrentBar == 0)
					return;
				if (pp!=0.0)
				{
					PP.Set(pp);
					R1.Set(r1);
					R2.Set(r2);
					R3.Set(r3);
					R4.Set(r4);
					S1.Set(s1);
					S2.Set(s2);
					S3.Set(s3);
					S4.Set(s4);
					if (pp!= PP[1] || r1 != R1[1]|| s1 != S1[1])
					{
						PlotColors[0][0] = Color.Transparent;
						PlotColors[1][0] = Color.Transparent;
						PlotColors[2][0] = Color.Transparent;
						PlotColors[3][0] = Color.Transparent;
						PlotColors[4][0] = Color.Transparent;
						PlotColors[5][0] = Color.Transparent;
						PlotColors[6][0] = Color.Transparent;
						PlotColors[7][0] = Color.Transparent;
						PlotColors[8][0] = Color.Transparent;
					}
				}
			}
		}	
        
		#region Properties
        
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PP
		{
			get { return Values[0]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries R1
		{
			get { return Values[1]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries R2
		{
			get { return Values[2]; }
		}

       /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries R3
		{
			get { return Values[3]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries R4
		{
			get { return Values[4]; }
		}

       /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries S1
		{
			get { return Values[5]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries S2
		{
			get { return Values[6]; }
		}

       /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries S3
		{
			get { return Values[7]; }
		}

       /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries S4
		{
			get { return Values[8]; }
		}

		/// <summary>
		/// </summary>
		[Description("Rolling Period in Minutes")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Rolling Period (Min)")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}
		
				/// <summary>
		/// </summary>
		[Description("Pivot Start Time")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Pivot Start Time (00:00)")]
		public string Start
		{
			get { return start; }
			set { start = value; }
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
        private SJC_anaPivotsHourly[] cacheSJC_anaPivotsHourly = null;

        private static SJC_anaPivotsHourly checkSJC_anaPivotsHourly = new SJC_anaPivotsHourly();

        /// <summary>
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        public SJC_anaPivotsHourly SJC_anaPivotsHourly(int period, string start)
        {
            return SJC_anaPivotsHourly(Input, period, start);
        }

        /// <summary>
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        public SJC_anaPivotsHourly SJC_anaPivotsHourly(Data.IDataSeries input, int period, string start)
        {
            if (cacheSJC_anaPivotsHourly != null)
                for (int idx = 0; idx < cacheSJC_anaPivotsHourly.Length; idx++)
                    if (cacheSJC_anaPivotsHourly[idx].Period == period && cacheSJC_anaPivotsHourly[idx].Start == start && cacheSJC_anaPivotsHourly[idx].EqualsInput(input))
                        return cacheSJC_anaPivotsHourly[idx];

            lock (checkSJC_anaPivotsHourly)
            {
                checkSJC_anaPivotsHourly.Period = period;
                period = checkSJC_anaPivotsHourly.Period;
                checkSJC_anaPivotsHourly.Start = start;
                start = checkSJC_anaPivotsHourly.Start;

                if (cacheSJC_anaPivotsHourly != null)
                    for (int idx = 0; idx < cacheSJC_anaPivotsHourly.Length; idx++)
                        if (cacheSJC_anaPivotsHourly[idx].Period == period && cacheSJC_anaPivotsHourly[idx].Start == start && cacheSJC_anaPivotsHourly[idx].EqualsInput(input))
                            return cacheSJC_anaPivotsHourly[idx];

                SJC_anaPivotsHourly indicator = new SJC_anaPivotsHourly();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Start = start;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_anaPivotsHourly[] tmp = new SJC_anaPivotsHourly[cacheSJC_anaPivotsHourly == null ? 1 : cacheSJC_anaPivotsHourly.Length + 1];
                if (cacheSJC_anaPivotsHourly != null)
                    cacheSJC_anaPivotsHourly.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_anaPivotsHourly = tmp;
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
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_anaPivotsHourly SJC_anaPivotsHourly(int period, string start)
        {
            return _indicator.SJC_anaPivotsHourly(Input, period, start);
        }

        /// <summary>
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_anaPivotsHourly SJC_anaPivotsHourly(Data.IDataSeries input, int period, string start)
        {
            return _indicator.SJC_anaPivotsHourly(input, period, start);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_anaPivotsHourly SJC_anaPivotsHourly(int period, string start)
        {
            return _indicator.SJC_anaPivotsHourly(Input, period, start);
        }

        /// <summary>
        /// The indicator plots intraday pivots based on a customizable period.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_anaPivotsHourly SJC_anaPivotsHourly(Data.IDataSeries input, int period, string start)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_anaPivotsHourly(input, period, start);
        }
    }
}
#endregion
