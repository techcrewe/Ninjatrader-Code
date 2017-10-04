// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
	/// </summary>
	[Description("The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.")]
	public class SJC_RSI : Indicator
	{
		#region Variables
		private int	period	= 6;
		private int	smooth	= 3;
        private int longext  =   80;
        private int shortext  =  20;
		private RSI RSIClose;
		private RSI RSIHigh;
		private RSI RSILow;

		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Magenta, "RSIC"));
			//Add(new Plot(Color.Orange, "Avg"));
            Add(new Plot(Color.Yellow, PlotStyle.Dot, "RSIH"));
            Add(new Plot(Color.Yellow, PlotStyle.Dot, "RSIL"));

			Add(new Line(System.Drawing.Color.Red, longext, "Upper"));
			Add(new Line(System.Drawing.Color.Blue, shortext, "Lower"));
            Add(new Line(System.Drawing.Color.DimGray, 40, "ChopLow"));
            Add(new Line(System.Drawing.Color.DimGray, 60, "ChopHigh"));
			Add(new Line(System.Drawing.Color.White, 50, "Centre"));
			
			//Lines[0].Pen.DashStyle = DashStyle.Dot;
			//Lines[1].Pen.DashStyle = DashStyle.Dot;
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 2;
			
			
			

			//avgUp				= new DataSeries(this);
			//avgDown				= new DataSeries(this);
			//down				= new DataSeries(this);
			//up					= new DataSeries(this);
			
			
			Plots[1].Min = LongExt;
        	Plots[2].Max = ShortExt;
			
			
			//DrawRegion("DangerZone", CurrentBar, 0, ChopLow, ChopHigh, Color.Empty, Color.Magenta, 2);
			
		}



		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{

            RSIClose = RSI(Close, Period, Smooth);
			//Print("RSIClose = "+RSIClose[0]);
            RSIHigh = RSI(High, Period, Smooth);
            RSILow = RSI(Low, Period, Smooth);

		
			
			double rsiCloseValue = RSIClose[0];
			//Print("rsiCloseValue = "+rsiCloseValue);
			RSIC.Set(rsiCloseValue);

            
			double rsiHighValue = RSIHigh[0];
			//Print("rsiHighValue = "+rsiHighValue);
			RSIH.Set(rsiHighValue + 5);
			
			double rsiLowValue = RSILow[0];
			//Print("rsiLowValue = "+rsiLowValue);
			RSIL.Set(rsiLowValue - 5);
		 
	    //Colour coriteria for RSIC
         if (rsiCloseValue > longext)
         {
             PlotColors[0][0] = Color.Red;
         }
         else if (rsiCloseValue < shortext)
         {
             PlotColors[0][0] = Color.Blue;
         }
         else
         {
             PlotColors[0][0] = Color.White;
         }

        //Colour criteria for RSIH 
        if (rsiHighValue > longext)
        {
            PlotColors[1][0] = Color.Black;
        }
        else
        {
            PlotColors[1][0] = Color.Transparent;
        }

        //Colour criteria for RSIL
        if (rsiLowValue < shortext)
        {
            PlotColors[2][0] = Color.Black;
        }
        else
        {
            PlotColors[2][0] = Color.Transparent;
        }
          
		//	Avg.Set(rsiAvg);
		//	Value.Set(rsi);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSIC
		{
			get { return Values[0]; }
		}
				
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSIH
		{
			get { return Values[1]; }
		}
		
				/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries RSIL
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of bars for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}

        /// <summary>
        /// </summary>
        [Description("Number of bars for smoothing")]
        [GridCategory("Parameters")]
        public int LongExt
        {
            get { return longext; }
            set { longext = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("Number of bars for smoothing")]
        [GridCategory("Parameters")]
        public int ShortExt
        {
            get { return shortext; }
            set { shortext = Math.Max(1, value); }
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
        private SJC_RSI[] cacheSJC_RSI = null;

        private static SJC_RSI checkSJC_RSI = new SJC_RSI();

        /// <summary>
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public SJC_RSI SJC_RSI(int longExt, int period, int shortExt, int smooth)
        {
            return SJC_RSI(Input, longExt, period, shortExt, smooth);
        }

        /// <summary>
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public SJC_RSI SJC_RSI(Data.IDataSeries input, int longExt, int period, int shortExt, int smooth)
        {
            if (cacheSJC_RSI != null)
                for (int idx = 0; idx < cacheSJC_RSI.Length; idx++)
                    if (cacheSJC_RSI[idx].LongExt == longExt && cacheSJC_RSI[idx].Period == period && cacheSJC_RSI[idx].ShortExt == shortExt && cacheSJC_RSI[idx].Smooth == smooth && cacheSJC_RSI[idx].EqualsInput(input))
                        return cacheSJC_RSI[idx];

            lock (checkSJC_RSI)
            {
                checkSJC_RSI.LongExt = longExt;
                longExt = checkSJC_RSI.LongExt;
                checkSJC_RSI.Period = period;
                period = checkSJC_RSI.Period;
                checkSJC_RSI.ShortExt = shortExt;
                shortExt = checkSJC_RSI.ShortExt;
                checkSJC_RSI.Smooth = smooth;
                smooth = checkSJC_RSI.Smooth;

                if (cacheSJC_RSI != null)
                    for (int idx = 0; idx < cacheSJC_RSI.Length; idx++)
                        if (cacheSJC_RSI[idx].LongExt == longExt && cacheSJC_RSI[idx].Period == period && cacheSJC_RSI[idx].ShortExt == shortExt && cacheSJC_RSI[idx].Smooth == smooth && cacheSJC_RSI[idx].EqualsInput(input))
                            return cacheSJC_RSI[idx];

                SJC_RSI indicator = new SJC_RSI();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LongExt = longExt;
                indicator.Period = period;
                indicator.ShortExt = shortExt;
                indicator.Smooth = smooth;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_RSI[] tmp = new SJC_RSI[cacheSJC_RSI == null ? 1 : cacheSJC_RSI.Length + 1];
                if (cacheSJC_RSI != null)
                    cacheSJC_RSI.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_RSI = tmp;
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
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_RSI SJC_RSI(int longExt, int period, int shortExt, int smooth)
        {
            return _indicator.SJC_RSI(Input, longExt, period, shortExt, smooth);
        }

        /// <summary>
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_RSI SJC_RSI(Data.IDataSeries input, int longExt, int period, int shortExt, int smooth)
        {
            return _indicator.SJC_RSI(input, longExt, period, shortExt, smooth);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_RSI SJC_RSI(int longExt, int period, int shortExt, int smooth)
        {
            return _indicator.SJC_RSI(Input, longExt, period, shortExt, smooth);
        }

        /// <summary>
        /// The RSI (Relative Strength Index) is a price-following oscillator that ranges between 0 and 100.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_RSI SJC_RSI(Data.IDataSeries input, int longExt, int period, int shortExt, int smooth)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_RSI(input, longExt, period, shortExt, smooth);
        }
    }
}
#endregion
