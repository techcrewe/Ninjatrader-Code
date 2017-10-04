// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The anaMACDBBLines (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
	/// Optimized execution by predefining instances of external indicators (Zondor August 10 2010)
	/// // TheWizard added background coloring and the option of drawing a dot on the price panel when price exceeds the upper or lower Bolinger band(s)
	/// // also added coloring to the 'BB average' line and UP and DOWN paint bar color options, along with a "conservative" mode option for coloring
	/// // the backround when MACD is above or below '0' vs. coloring the background based on whether or not the MACD is above or below the BB Average
	/// </summary>
	[Description("The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.")]
	public class SJC_MACDBB_MA : Indicator
	{
		#region Variables
		private int		bandperiod 			= 10;
		private int		fast				= 12;
		private int		slow				= 26;
		private int		smooth				= 5;
		private int		dotsize				= 2;
        private int     macdsignal          = 0;
		private int		signal				= 0;
		private double	stdDevNumber		= 1.0;
		private Color	dotsUpInside		= Color.Lime;
		private Color   dotsUpOutside		= Color.Lime;
		private Color 	dotsDownInside		= Color.Red;
		private Color   dotsDownOutside		= Color.Red;
		private Color	dotsRim				= Color.Black;
//		private Color	bbAverage			= Color.LightSkyBlue;	// Commented out by The Wizard Feb 18, 2011
//		private Color	bbAverageUp			= Color.Lime;			// added by TheWizard Feb 18, 2011
//		private Color	bbAverageDn			= Color.Red;			// added by TheWizard Feb 18, 2011
		private Color	bbUpper				= Color.White;
		private Color	bbLower				= Color.White;
		private Color	zero				= Color.Black;
		private Color	signalcol			= Color.Yellow;
		private Color 	zeroCross			= Color.Yellow;
		private Color	connector			= Color.White;
		private bool 	init 				= false;

			
		private MACD BMACD;
		private EMA  EMAMACD;
		private HMA HMAMACD;
		private StdDev SDBB;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Black, 2), PlotStyle.Dot, "BBMACD"));
            Add(new Plot(new Pen(Color.Black, 3), PlotStyle.Dot, "BBMACDFrame"));
			Add(new Plot(new Pen(Color.White, 1), PlotStyle.Line, "Upper"));
			Add(new Plot(new Pen(Color.White, 1), PlotStyle.Line, "Lower"));
			Add(new Plot(new Pen(Color.Yellow, 1), PlotStyle.Line, "Signal"));
            Add(new Plot(new Pen(Color.Black, 1), PlotStyle.Line, "ZeroLine"));
 			
			Plots[0].Pen.DashStyle = DashStyle.Dot;
            Plots[1].Pen.DashStyle = DashStyle.Dot;
			Plots[2].Pen.DashStyle = DashStyle.Solid;
			Plots[3].Pen.DashStyle = DashStyle.Solid;
            Plots[4].Pen.DashStyle = DashStyle.Solid;
            Plots[5].Pen.DashStyle = DashStyle.Solid;
			PlotsConfigurable = true;  // changed from 'false' to 'true' by TheWizard Feb 18, 2011
			PaintPriceMarkers = false;
		}

		protected override void OnStartUp()
		{
			Plots[0].Pen.Width = DotSize;
			Plots[1].Pen.Width = DotSize +1;
			Plots[1].Pen.Color = DotsRim;
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{
			if ( CurrentBars[0] < bandperiod) return;
			
			if(!init)
			{
				BMACD=MACD(Input,fast,slow,smooth);
                EMAMACD=EMA(BMACD,bandperiod);
				HMAMACD=HMA(BMACD,smooth);
				SDBB=StdDev(BMACD,bandperiod);
			}
			init=true;
			double macdValue = HMAMACD[0];//BMACD[0];
			BBMACD.Set(macdValue);
			//BBMACDLine.Set(macdValue);
			BBMACDFrame.Set(macdValue);
			
			//double avg = EMA(BBMACD,bandperiod)[0];
            double avg = EMAMACD[0];
			ZeroLine.Set(0);

			//double stdDevValue = StdDev(BBMACD,bandperiod)[0];
			double stdDevValue = SDBB[0];
			Upper.Set(avg + StdDevNumber * stdDevValue);
			Lower.Set(avg - StdDevNumber * stdDevValue);
			
			//1. Green dots, above bands, sloping up Green dot - text UP - prefer up as easy to understand
            //2. Green dots, above bands, changing to a red dot. - Cell Colour will Change (but may still say UP)
            //3. Consecutive red dots crossing down through the bands - not sure we discussed this one!!! Cell colour will change Put --- to indicate crossing down
            //4. Red dots, below bands, sloping down.Red dot - text DOWN - prefer DOWN as easy to understand
            //5. Red dots, below bands, changing to a green dot.ell Colour will Change (but may still say DOWN)
            //6. Consecutive green dots, crossing up through the bands - not sure we discussed this one!!! Cell colour will change Put +++ to indicate crossing down

            //1 & 4 - would be trending signals - don't trade against these
            //2 & 5 - would be minor alerts for consolidation or the start of direction change
            //3 & 6 - would be major alerts with direction change confirmed by dots about to break through the opposite band
			
			
			
			if (Rising(BBMACD))
			{
				if (BBMACD[0] < Upper[0])
				{
					PlotColors[0][0] = DotsUpInside;
					macdsignal = 1;
				}
				else if ((BBMACD[1] < Upper[1]) && (BBMACD[0] > Upper[0]))
				{
					PlotColors[0][0] = DotsUpOutside;
					macdsignal = 2;
				}
				else
				{
					PlotColors[0][0] = DotsUpOutside;
					macdsignal = 3;
				}
			}
			else
			{
				if (BBMACD[0] > Lower[0])
				{
					PlotColors[0][0] = DotsDownInside;
					macdsignal = -1;
				}
				else if ((BBMACD[1] > Lower[1]) && (BBMACD[0] < Lower[0]))
				{
					PlotColors[0][0] = DotsDownOutside;
					macdsignal = -2;
				}
				
				else
				{
					PlotColors[0][0] = DotsDownOutside;
					macdsignal = -3;
				}

			}	
				
				
			Signal.Set(macdsignal);	
				
				
				
				
				
				
				
				
				
				
/*				
				
				if (BBMACD[0] > BBMACD[1])
            {
                PlotColors[0][0] = DotsUpOutside;
                if ((BBMACD[0] > Upper[0]))  //UP
                {
                    macdsignal = 1;
                    PlotColors[0][0] = DotsUpOutside;
                }
                else if ((BBMACD[1] < Lower[1]) && (BBMACD[0] > Lower[0]))  //UP
                {
                    macdsignal = 6;
                    PlotColors[0][0] = DotsDownInside;
                }
                else if ((BBMACD[1] > BBMACD[2]) && (BBMACD[0] > Lower[0]))   //UP
                {
                    macdsignal = 7;
                    PlotColors[0][0] = DotsUpInside;
                }
                else if ((BBMACD[1] < Upper[1]) && (BBMACD[0] > Upper[0]))    //UP
                {
                    macdsignal = 8;
                    PlotColors[0][0] = DotsUpOutside;
                }
            }
            else //(BBMACD[0] < BBMACD[1])
            {
                PlotColors[0][0] = DotsDownOutside;
                if ((BBMACD[0] < Lower[0]))  //DN
                {
                    macdsignal = 5;
                    PlotColors[0][0] = DotsDownOutside;
                }
                else if ((BBMACD[0] < BBMACD[1]))  //DN
                {
                    macdsignal = 2;
                    PlotColors[0][0] = DotsDownOutside;
                }
                else if ((BBMACD[1] < BBMACD[2]) && (BBMACD[0] < Upper[0]))   //DN
                {
                    macdsignal = 3;
                    PlotColors[0][0] = DotsDownInside;
                }
                else if ((BBMACD[1] > Lower[1]) && (BBMACD[0] < Lower[0]))    //DN
                {
                    macdsignal = 4;
                    PlotColors[0][0] = DotsDownOutside;
                }
            }
				*/
/*
            if ((BBMACD[0] > BBMACD[1]) && (BBMACD[0] > Upper[0]))  //UP
            {
                macdsignal = 1;
                PlotColors[0][0] = DotsUpOutside;
            }
            if ((BBMACD[1] > Upper[1]) && (BBMACD[0] < BBMACD[1]))  //DN
            {
                macdsignal = 2;
                PlotColors[0][0] = DotsDownOutside;
            }
            if ((BBMACD[0] < BBMACD[1]) && (BBMACD[1] < BBMACD[2]) && (BBMACD[0] < Upper[0]))   //DN
            {
                macdsignal = 3;
                PlotColors[0][0] = DotsDownInside;
            }
            if ((BBMACD[0] < BBMACD[1]) && (BBMACD[1] > Lower[1]) && (BBMACD[0] < Lower[0]))    //DN
            {
                macdsignal = 4;
                PlotColors[0][0] = DotsDownOutside;
            }
            
            if ((BBMACD[0] < BBMACD[1]) && (BBMACD[0] < Lower[0]))  //DN
            {
                macdsignal = 5;
                PlotColors[0][0] = DotsDownOutside;
            }
            if ((BBMACD[1] < Lower[1]) && (BBMACD[0] > BBMACD[1]))  //UP
            {
                macdsignal = 6;
                PlotColors[0][0] = DotsDownInside;
            }
            if ((BBMACD[0] > BBMACD[1]) && (BBMACD[1] > BBMACD[2]) && (BBMACD[0] > Lower[0]))   //UP
            {
                macdsignal = 7;
                PlotColors[0][0] = DotsUpInside;
            }
            if ((BBMACD[0] > BBMACD[1]) && (BBMACD[1] < Upper[1]) && (BBMACD[0] > Upper[0]))    //UP
            {
                macdsignal = 8;
                PlotColors[0][0] = DotsUpOutside;
            }
*/

			
			/////PlotColors[2][0] = BBUpper;
			/////PlotColors[3][0] = BBLower;
            /////PlotColors[4][0] = SignalCol;
            /////PlotColors[5][0] = ZeroCol;
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries BBMACD
		{
			get { return Values[0]; }
		}

        /// <summary>
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries BBMACDFrame
        {
            get { return Values[1]; }
        }

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[2]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[3]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Signal
		{
			get { return Values[4]; }
		}

        /// <summary>
        /// </summary>
        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries ZeroLine
        {
            get { return Values[5]; }
        }

		/// <summary>
		/// </summary>
		[Description("Band Period for Bollinger Band")]
		[GridCategory("Parameters")]
		public int BandPeriod
		{
			get { return bandperiod; }
			set { bandperiod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for fast EMA")]
		[GridCategory("Parameters")]
		public int Fast
		{
			get { return fast; }
			set { fast = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for slow EMA")]
		[GridCategory("Parameters")]
		public int Slow
		{
			get { return slow; }
			set { slow = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for smoothing")]
		[GridCategory("Parameters")]
		public int Smooth
		{
			get { return smooth; }
			set { smooth = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Dotsize")]
		[GridCategory("Parameters")]
		public int DotSize
		{
			get { return dotsize; }
			set { dotsize = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Number of standard deviations")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("# of Std. Dev.")]
		public double StdDevNumber
		{
			get { return stdDevNumber; }
			set { stdDevNumber = Math.Max(0, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Dots Up Inside BB")]
		public Color DotsUpInside
		{
			get { return dotsUpInside; }
			set { dotsUpInside = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DotsUpInsideSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dotsUpInside); }
			set { dotsUpInside = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Dots Up Outside BB")]
		public Color DotsUpOutside
		{
			get { return dotsUpOutside; }
			set { dotsUpOutside = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DotsUpOutsideSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dotsUpOutside); }
			set { dotsUpOutside = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Dots Down Inside BB")]
		public Color DotsDownInside
		{
			get { return dotsDownInside; }
			set { dotsDownInside = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DotsDownInsideSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dotsDownInside); }
			set { dotsDownInside = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Dots Down Outside BB")]
		public Color DotsDownOutside
		{
			get { return dotsDownOutside; }
			set { dotsDownOutside = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DotsDownOutsideSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dotsDownOutside); }
			set { dotsDownOutside = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Dots Rim")]
		public Color DotsRim
		{
			get { return dotsRim; }
			set { dotsRim = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DotsRimSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(dotsRim); }
			set { dotsRim = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}


		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Upper Band")]
		public Color BBUpper
		{
			get { return bbUpper; }
			set { bbUpper = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBUpperSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbUpper); }
			set { bbUpper = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Lower Band")]
		public Color BBLower
		{
			get { return bbLower; }
			set { bbLower = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBLowerSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbLower); }
			set { bbLower = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Zeroline")]
		public Color ZeroCol
		{
			get { return zero; }
			set { zero = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ZeroSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zero); }
			set { zero = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}




		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Connector")]
		public Color SignalCol
		{
			get { return signalcol; }
			set { signalcol = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string SignalColSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(signalcol); }
			set { signalcol = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private SJC_MACDBB_MA[] cacheSJC_MACDBB_MA = null;

        private static SJC_MACDBB_MA checkSJC_MACDBB_MA = new SJC_MACDBB_MA();

        /// <summary>
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public SJC_MACDBB_MA SJC_MACDBB_MA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return SJC_MACDBB_MA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public SJC_MACDBB_MA SJC_MACDBB_MA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            if (cacheSJC_MACDBB_MA != null)
                for (int idx = 0; idx < cacheSJC_MACDBB_MA.Length; idx++)
                    if (cacheSJC_MACDBB_MA[idx].BandPeriod == bandPeriod && cacheSJC_MACDBB_MA[idx].DotSize == dotSize && cacheSJC_MACDBB_MA[idx].Fast == fast && cacheSJC_MACDBB_MA[idx].Slow == slow && cacheSJC_MACDBB_MA[idx].Smooth == smooth && Math.Abs(cacheSJC_MACDBB_MA[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheSJC_MACDBB_MA[idx].EqualsInput(input))
                        return cacheSJC_MACDBB_MA[idx];

            lock (checkSJC_MACDBB_MA)
            {
                checkSJC_MACDBB_MA.BandPeriod = bandPeriod;
                bandPeriod = checkSJC_MACDBB_MA.BandPeriod;
                checkSJC_MACDBB_MA.DotSize = dotSize;
                dotSize = checkSJC_MACDBB_MA.DotSize;
                checkSJC_MACDBB_MA.Fast = fast;
                fast = checkSJC_MACDBB_MA.Fast;
                checkSJC_MACDBB_MA.Slow = slow;
                slow = checkSJC_MACDBB_MA.Slow;
                checkSJC_MACDBB_MA.Smooth = smooth;
                smooth = checkSJC_MACDBB_MA.Smooth;
                checkSJC_MACDBB_MA.StdDevNumber = stdDevNumber;
                stdDevNumber = checkSJC_MACDBB_MA.StdDevNumber;

                if (cacheSJC_MACDBB_MA != null)
                    for (int idx = 0; idx < cacheSJC_MACDBB_MA.Length; idx++)
                        if (cacheSJC_MACDBB_MA[idx].BandPeriod == bandPeriod && cacheSJC_MACDBB_MA[idx].DotSize == dotSize && cacheSJC_MACDBB_MA[idx].Fast == fast && cacheSJC_MACDBB_MA[idx].Slow == slow && cacheSJC_MACDBB_MA[idx].Smooth == smooth && Math.Abs(cacheSJC_MACDBB_MA[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheSJC_MACDBB_MA[idx].EqualsInput(input))
                            return cacheSJC_MACDBB_MA[idx];

                SJC_MACDBB_MA indicator = new SJC_MACDBB_MA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BandPeriod = bandPeriod;
                indicator.DotSize = dotSize;
                indicator.Fast = fast;
                indicator.Slow = slow;
                indicator.Smooth = smooth;
                indicator.StdDevNumber = stdDevNumber;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_MACDBB_MA[] tmp = new SJC_MACDBB_MA[cacheSJC_MACDBB_MA == null ? 1 : cacheSJC_MACDBB_MA.Length + 1];
                if (cacheSJC_MACDBB_MA != null)
                    cacheSJC_MACDBB_MA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_MACDBB_MA = tmp;
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
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_MACDBB_MA SJC_MACDBB_MA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJC_MACDBB_MA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_MACDBB_MA SJC_MACDBB_MA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJC_MACDBB_MA(input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_MACDBB_MA SJC_MACDBB_MA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJC_MACDBB_MA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The MACDBBLinesV4 (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_MACDBB_MA SJC_MACDBB_MA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_MACDBB_MA(input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }
    }
}
#endregion
