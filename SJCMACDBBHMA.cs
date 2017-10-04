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
	[Description("The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.")]
	public class SJCMACDBBHMA : Indicator
	{
		#region Variables
		private int		bandperiod 			= 10;
		private int		fast				= 12;
		private int		slow				= 26;
		private int		smooth				= 5;
		private int		dotsize				= 1;
		private double	stdDevNumber		= 1.0;
		private Color	dotsUpInside		= Color.Yellow;
		private Color   dotsUpOutside		= Color.Yellow;
		private Color 	dotsDownInside		= Color.Magenta;
		private Color   dotsDownOutside		= Color.Magenta;
		private Color	dotsRim				= Color.Black;
//		private Color	bbAverage			= Color.LightSkyBlue;	// Commented out by The Wizard Feb 18, 2011
		private Color	bbAverageUp			= Color.Blue;			// added by TheWizard Feb 18, 2011
		private Color	bbAverageDn			= Color.Red;			// added by TheWizard Feb 18, 2011
		private Color	bbUpper				= Color.Black;
		private Color	bbLower				= Color.Black;
		private Color	zeroPositive		= Color.DimGray;
		private Color	zeroNegative		= Color.DimGray;
		private Color 	zeroCross			= Color.Transparent;
		private Color	connector			= Color.White;
		private bool 	init 				= false;
		private bool	conservative		= false;
		private bool	colorbackground 	= false;				// added by TheWizard March 15, 2011
		private bool	colorALLbackgrounds = false;				// added by TheWizard March 15, 2011
		private Color	backgroundcolorUp 	= Color.Green;			// added by TheWizard March 15, 2011
		private Color	backgroundcolorDn 	= Color.Red;			// added by TheWizard March 15, 2011
		private int		opacity				= 50;					// added by TheWizard March 15, 2011
		private bool	paintbars			= false;				// added by TheWizard April 2, 2011
		private bool	hollowsolid			= false;				// added by TheWizard April 2, 2011
		private Color	barcolorup			= Color.Lime;			// added by TheWizard April 2, 2011
		private Color	barcolordn			= Color.Red;			// added by TheWizard April 2, 2011
		private Color	candleoutlinecolorup= Color.Black;			// added by TheWizard April 2, 2011
		private Color	candleoutlinecolordn= Color.Black;			// added by TheWizard April 2, 2011
		private bool 	drawDotOnPricePanel = false;				// added by TheWizard March 15, 2011
		private bool	updotdrawn			= false;				// added by TheWizard March 15, 2011
		private bool	downdotdrawn		= false;				// added by TheWizard March 15, 2011
		
		private bool	zerolinecrosssound	= false;
		private string 	longwavfilename		= "long.wav";			// added by TheWizard March 15, 2011
		private string 	shortwavfilename	= "short.wav";			// added by TheWizard March 15, 2011
		
		private bool	bbviolationsound 	= false;
		private string 	bbviolationupsound 	= "Alert2.wav";
		private string 	bbviolationdnsound 	= "Alert2.wav";
		
		private bool	showhistogram		= false;
			
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
			Add(new Plot(new Pen(Color.Black, 1), PlotStyle.Line, "Average"));
			Add(new Plot(new Pen(Color.Black, 1), PlotStyle.Line, "Upper"));
			Add(new Plot(new Pen(Color.Black, 1), PlotStyle.Line, "Lower"));
			Add(new Plot(new Pen(Color.Black, 1), PlotStyle.Line, "ZeroLine"));
			Add(new Plot(new Pen(Color.Black, 8), PlotStyle.Dot, "MACDCross"));
			Add(new Plot(new Pen(Color.Black, 2), PlotStyle.Line, "BBMACDLine"));
			Add(new Plot(new Pen(BBdotUpColor, 2), PlotStyle.Dot, "BBdotUp"));			// 08 CCIdotUp   // added by TheWizard March 15, 2011
			Add(new Plot(new Pen(BBdotDnColor, 2), PlotStyle.Dot, "BBdotDn"));			// 09 CCIdotDn   // added by TheWizard March 15, 2011
			Add(new Plot(new Pen(Color.Black,3), PlotStyle.Bar, "Hist"));				// 10
			
			Plots[0].Pen.DashStyle = DashStyle.Dot;
			Plots[1].Pen.DashStyle = DashStyle.Dot;
			Plots[2].Pen.DashStyle = DashStyle.Dash;
			Plots[5].Pen.DashStyle = DashStyle.Dash;
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
			BBMACDLine.Set(macdValue);
			BBMACDFrame.Set(macdValue);
			
			//double avg = EMA(BBMACD,bandperiod)[0];
			double avg = EMAMACD[0];
			Average.Set(avg);
			ZeroLine.Set(0);

			//double stdDevValue = StdDev(BBMACD,bandperiod)[0];
			double stdDevValue = SDBB[0];
			Upper.Set(avg + StdDevNumber * stdDevValue);
			Lower.Set(avg - StdDevNumber * stdDevValue);
			
			
//			if(Rising(Average))
//				if(paintbars)
//				{
//					BarColor = bbAverageUp;
//					CandleOutlineColor = candleoutlinecolorup;
//				}
//			if(Falling(Average))
//				if(paintbars)
//				{
//					BarColor = bbAverageDn;
//					CandleOutlineColor = candleoutlinecolorup;
//				}
			
			if (Rising(BBMACD))
			{
				if (BBMACD[0] < Upper[0])
				{
					PlotColors[0][0] = DotsUpInside;
					if(showhistogram)
					{
					Hist.Set((macdValue-avg));
					PlotColors[10][0] = DotsUpInside;
					}
					updotdrawn = false;				// added by TheWizard March 15, 2011
				}
				else
				{
					PlotColors[0][0] = DotsUpOutside;
					if(showhistogram)
					{
					Hist.Set((macdValue-avg));
					PlotColors[10][0] = DotsUpOutside;
					}
					if(drawDotOnPricePanel)			// added by TheWizard March 15, 2011
					if(updotdrawn == false)			// added by TheWizard March 15, 2011
					{
					DrawDot("UpDots"+CurrentBar, true, 0, Low[0]-dotSeparation*TickSize, BBdotUpColor);   // added by TheWizard March 15, 2011
					DrawDot("UpDots2"+CurrentBar, true, 0, Low[0]-dotSeparation*(TickSize*2), BBdotUpColor);
					updotdrawn = true;				// added by TheWizard March 15, 2011
					downdotdrawn = false;			// added by TheWizard March 15, 2011
					if(bbviolationsound) if(FirstTickOfBar) PlaySound(bbviolationupsound);
					}
				}
				if (paintbars)
				{
					BarColor = barcolorup;
					CandleOutlineColor = candleoutlinecolorup;
				}
			}
			else
			{	
				if (BBMACD[0] > Lower[0])
				{
					PlotColors[0][0] = DotsDownInside;
					if(showhistogram)
					{
					Hist.Set((macdValue-avg));
					PlotColors[10][0] = DotsDownInside;
					}
					downdotdrawn = false;			// added by TheWizard March 15, 2011
				}
				else
				{
					PlotColors[0][0] = DotsDownOutside;
					if(showhistogram)
					{
					Hist.Set((macdValue-avg));
					PlotColors[10][0] = DotsDownOutside;
					}
					if(drawDotOnPricePanel)			// added by TheWizard March 15, 2011
					if(downdotdrawn == false)		// added by TheWizard March 15, 2011
					{		
					DrawDot("DnDots"+CurrentBar, true, 0, High[0]+dotSeparation*TickSize, BBdotDnColor);   // added by TheWizard March 15, 2011
					DrawDot("DnDots2"+CurrentBar, true, 0, High[0]+dotSeparation*(TickSize*2), BBdotDnColor);
					downdotdrawn = true;		// added by TheWizard March 15, 2011
					updotdrawn = false;			// added by TheWizard March 15, 2011
					if(bbviolationsound) if(FirstTickOfBar) PlaySound(bbviolationdnsound);
					}
				}
				if (paintbars)
				{
					BarColor = barcolordn;
					CandleOutlineColor = candleoutlinecolordn;
				}
			}
			if(BBMACD[0] > avg)
			if(conservative)
			{
				PlotColors[2][0] = BBAverageUp;
				if (BBMACD[0] > 0) PlotColors[5][0] = ZeroPositive;
				if (BBMACD[0] < 0) PlotColors[5][0] = ZeroNegative;
			}
			else
			{
				PlotColors[2][0] = BBAverageUp;
				PlotColors[5][0] = ZeroPositive;
				if(colorbackground) BackColor = Color.FromArgb(opacity,backgroundcolorUp);
				if(colorALLbackgrounds) BackColorAll = Color.FromArgb(opacity,backgroundcolorUp);
			}

			if(BBMACD[0] < avg)
			if(conservative)
			{
				PlotColors[2][0] = BBAverageDn;
				if (BBMACD[0] > 0) PlotColors[5][0] = ZeroPositive;
				if (BBMACD[0] < 0) PlotColors[5][0] = ZeroNegative;
			}
			else
			{
				PlotColors[2][0] = BBAverageDn;
				PlotColors[5][0] = ZeroNegative;
				if(colorbackground) BackColor = Color.FromArgb(opacity,backgroundcolorDn);
				if(colorALLbackgrounds) BackColorAll = Color.FromArgb(opacity,backgroundcolorDn);
			}

			//PlotColors[2][0] = BBAverage;
			PlotColors[3][0] = BBUpper;
			PlotColors[4][0] = BBLower;
			PlotColors[6][0] = ZeroCross;
			PlotColors[7][0] = Connector;

			if (BBMACD[0] > 0)
			{
				if(conservative)
				{
				if(colorbackground)
				{
					BackColor = Color.FromArgb(opacity,backgroundcolorUp);
				}
				if(colorALLbackgrounds)
				{
					BackColorAll = Color.FromArgb(opacity,backgroundcolorUp);
				}
				}
				if (CurrentBar != 0 && BBMACD[1] <= 0)
				{
					MACDCross.Set(0);
					if(zerolinecrosssound)
					{
					if(FirstTickOfBar) PlaySound(longwavfilename);
					}
				}

				else
					MACDCross.Reset();
			}
			else
			{
				if(conservative)
				{
				if(colorbackground)
				{
					BackColor = Color.FromArgb(opacity,backgroundcolorDn);
				}
				if(colorALLbackgrounds)
				{
					BackColorAll = Color.FromArgb(opacity,backgroundcolorDn);
				}
				}
				if (CurrentBar != 0 && BBMACD[1] > 0)
				{
					MACDCross.Set(0);
					if(zerolinecrosssound)
					{
					if(FirstTickOfBar) PlaySound(shortwavfilename);
					}
				}
				else
					MACDCross.Reset();
			}
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
		public DataSeries Average
		{
			get { return Values[2]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[3]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
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
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries MACDCross
		{
			get { return Values[6]; }
		}

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries BBMACDLine
		{
			get { return Values[7]; }
		}
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BBdotUp
        {
            get { return Values[8]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries BBdotDn
        {
            get { return Values[9]; }
        }
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Hist
		{
			get { return Values[10]; }
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
//		[Description("Select Color")]
//		[Category("Colors")]
//		[Gui.Design.DisplayName("Bollinger Average")]
//		public Color BBAverage
//		{
//			get { return bbAverage; }
//			set { bbAverage = value; }
//		}
//		
//		// Serialize Color object
//		[Browsable(false)]
//		public string BBAverageSerialize
//		{
//			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbAverage); }
//			set { bbAverage = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
//		}
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Average Up")]
		public Color BBAverageUp
		{
			get { return bbAverageUp; }
			set { bbAverageUp = value; }
		}
				
		// Serialize Color object
		[Browsable(false)]
		public string BBAverageUpSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbAverageUp); }
			set { bbAverageUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Bollinger Average Dn")]
		public Color BBAverageDn
		{
			get { return bbAverageDn; }
			set { bbAverageDn = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BBAverageDnSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bbAverageDn); }
			set { bbAverageDn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
		[Gui.Design.DisplayName("Zeroline Positive")]
		public Color ZeroPositive
		{
			get { return zeroPositive; }
			set { zeroPositive = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ZeroPositiveSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zeroPositive); }
			set { zeroPositive = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Zeroline Negative")]
		public Color ZeroNegative
		{
			get { return zeroNegative; }
			set { zeroNegative = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ZeroNegativeSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zeroNegative); }
			set { zeroNegative = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Zero Cross")]
		public Color ZeroCross
		{
			get { return zeroCross; }
			set { zeroCross = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ZeroCrossSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(zeroCross); }
			set { zeroCross = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		/// <summary>
		/// </summary>
		[Description("Select Color")]
		[Category("Colors")]
		[Gui.Design.DisplayName("Connector")]
		public Color Connector
		{
			get { return connector; }
			set { connector = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string ConnectorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(connector); }
			set { connector = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Conservative Mode?")]  // added this section by TheWizard
		[Category("Colors")]
		[Gui.Design.DisplayNameAttribute("Conservative?")]
		public bool Conservative
		{
			get { return conservative; }
			set { conservative = value; }
		}
		[Description("Color Background?")]  // added this section by TheWizard
		[Category("Flooding")]
		[Gui.Design.DisplayNameAttribute("Color Background?")]
		public bool ColorBackground
		{
			get { return colorbackground; }
			set { colorbackground = value; }
		}
		[Description("Color ALL Backgrounds?")]  // added this section by TheWizard
		[Category("Flooding")]
		[Gui.Design.DisplayNameAttribute("Color ALL Backgrounds?")]
		public bool ColorALLBackgrounds
		{
			get { return colorALLbackgrounds; }
			set { colorALLbackgrounds = value; }
		}
		[Description("Background Color Up")]
		[Category("Flooding")]
		[Gui.Design.DisplayName("Background Color Up")]
		public Color BackgroundcolorUp
		{
			get { return backgroundcolorUp; }
			set { backgroundcolorUp = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BackgroundcolorUpSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backgroundcolorUp); }
			set { backgroundcolorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Background Color Dn")]
		[Category("Flooding")]
		[Gui.Design.DisplayName("Background Color Dn")]
		public Color BackgroundcolorDn
		{
			get { return backgroundcolorDn; }
			set { backgroundcolorDn = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string BackgroundcolorDnSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(backgroundcolorDn); }
			set { backgroundcolorDn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Paintbars?")]
		[Category("PriceBars")]
		[Gui.Design.DisplayName("Paintbars?")]
		public bool Paintbars
		{
			get { return paintbars; }
			set { paintbars = value; }
		}
		[Description("Bar Color Up")]
		[Category("PriceBars")]
		[Gui.Design.DisplayName("Bar Color Up")]
		public Color Barcolorup
		{
			get { return barcolorup; }
			set { barcolorup = value; }
		}
		// Serialize Color object
		[Browsable(false)]
		public string BarcolorupSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barcolorup); }
			set { barcolorup = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Bar Color Dn")]
		[Category("PriceBars")]
		[Gui.Design.DisplayName("Bar Color Dn")]
		public Color Barcolordn
		{
			get { return barcolordn; }
			set { barcolordn = value; }
		}
		// Serialize Color object
		[Browsable(false)]
		public string BarcolordnSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(barcolordn); }
			set { barcolordn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Candle Outline Color Up")]
		[Category("PriceBars")]
		[Gui.Design.DisplayName("Candle Outline Color Up")]
		public Color Candleoutlinecolorup
		{
			get { return candleoutlinecolorup; }
			set { candleoutlinecolorup = value; }
		}
		// Serialize Color object
		[Browsable(false)]
		public string CandleoutlinecolorupSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(candleoutlinecolorup); }
			set { candleoutlinecolorup = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Candle Outline Color Dn")]
		[Category("PriceBars")]
		[Gui.Design.DisplayName("Candle Outline Color Dn")]
		public Color Candleoutlinecolordn
		{
			get { return candleoutlinecolordn; }
			set { candleoutlinecolordn = value; }
		}
		// Serialize Color object
		[Browsable(false)]
		public string CandleoutlinecolordnSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(candleoutlinecolordn); }
			set { candleoutlinecolordn = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		

		
		[Description("Background Opacity")]
		[Category("Flooding")]
		[Gui.Design.DisplayNameAttribute("Background Opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}
		
		[Description("Zero Line Cross Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("1.ZeroLine Cross Sound?")]
		public bool Zerolinecrosssound
		{
			get { return zerolinecrosssound; }
			set { zerolinecrosssound = value; }
		}
		[Description("Zero Line Cross UP Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("2.Zeroline Cross UP Sound")]
		public string Longwavfilename
		{
			get { return longwavfilename; }
			set { longwavfilename = value; }
		}
		[Description("Zero Line Cross DN Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("3.Zeroline Cross DN Sound")]
		public string Shortwavfilename
		{
			get { return shortwavfilename; }
			set { shortwavfilename = value; }
		}
		
		[Description("BB Violation Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("4.BB Violation Sound?")]
		public bool Bbviolationsound
		{
			get { return bbviolationsound; }
			set { bbviolationsound = value; }
		}
		[Description("BB Violation UP Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("5.BB Violation UP Sound")]
		public string Bbviolationupsound
		{
			get { return bbviolationupsound; }
			set { bbviolationupsound = value; }
		}
		[Description("BB Violation DN Sound?")]  // added this section by TheWizard
		[Category("Sounds")]
		[Gui.Design.DisplayNameAttribute("6.BB Violation DN Sound")]
		public string Bbviolationdnsound
		{
			get { return bbviolationdnsound; }
			set { bbviolationdnsound = value; }
		}

		
		// BB Dots
	
		private Color bBdotUpColor = Color.Lime;
        [XmlIgnore()]
        [Description("Color of BBdotUp, when Dot goes outside upper BB Band")]
        [Category("BB Violation Dots")]
        [Gui.Design.DisplayNameAttribute("BBdotUpColor")]
        public Color BBdotUpColor
        {get { return bBdotUpColor; } set { bBdotUpColor = value; }        }
        [Browsable(false)]
        public string bBdotUpColorSerialize
        {get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bBdotUpColor); } set { bBdotUpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

		
		
		private Color bBdotDnColor = Color.Red;
        [XmlIgnore()]
        [Description("Color of BBDotDn, when Dot goes outside lower BB Band")]
        [Category("BB Violation Dots")]
        [Gui.Design.DisplayNameAttribute("BBdotDnColor")]
        public Color BBdotDnColor
        {get { return bBdotDnColor; } set { bBdotDnColor = value; }        }
        [Browsable(false)]
        public string bBdotDnColorSerialize
        {get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bBdotDnColor); } set { bBdotDnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
		[Description("Draw Dot on Price Panel?")]
        [Category("BB Violation Dots")]
        [Gui.Design.DisplayName("Draw BB DotOnPricePanel?")]
        public bool DrawDotOnPricePanel
        {
            get { return drawDotOnPricePanel; }
            set { drawDotOnPricePanel = value; }
        }
		
		private int dotSeparation = 2;
		[Description("Number of ticks between the price bar and the Signal Dot.")]
		[Category("BB Violation Dots")]
		[Gui.Design.DisplayName("Ticks from Price Bar?")]
		public int Separation
		{
			get { return dotSeparation; }
			set { dotSeparation = Math.Max(-1, value); }
		}

		[Description("Show Histogram?")]
        [Category("Plots")]
        [Gui.Design.DisplayName("Show Histogram?")]
        public bool Showhistogram
        {
            get { return showhistogram; }
            set { showhistogram = value; }
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
        private SJCMACDBBHMA[] cacheSJCMACDBBHMA = null;

        private static SJCMACDBBHMA checkSJCMACDBBHMA = new SJCMACDBBHMA();

        /// <summary>
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public SJCMACDBBHMA SJCMACDBBHMA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return SJCMACDBBHMA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public SJCMACDBBHMA SJCMACDBBHMA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            if (cacheSJCMACDBBHMA != null)
                for (int idx = 0; idx < cacheSJCMACDBBHMA.Length; idx++)
                    if (cacheSJCMACDBBHMA[idx].BandPeriod == bandPeriod && cacheSJCMACDBBHMA[idx].DotSize == dotSize && cacheSJCMACDBBHMA[idx].Fast == fast && cacheSJCMACDBBHMA[idx].Slow == slow && cacheSJCMACDBBHMA[idx].Smooth == smooth && Math.Abs(cacheSJCMACDBBHMA[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheSJCMACDBBHMA[idx].EqualsInput(input))
                        return cacheSJCMACDBBHMA[idx];

            lock (checkSJCMACDBBHMA)
            {
                checkSJCMACDBBHMA.BandPeriod = bandPeriod;
                bandPeriod = checkSJCMACDBBHMA.BandPeriod;
                checkSJCMACDBBHMA.DotSize = dotSize;
                dotSize = checkSJCMACDBBHMA.DotSize;
                checkSJCMACDBBHMA.Fast = fast;
                fast = checkSJCMACDBBHMA.Fast;
                checkSJCMACDBBHMA.Slow = slow;
                slow = checkSJCMACDBBHMA.Slow;
                checkSJCMACDBBHMA.Smooth = smooth;
                smooth = checkSJCMACDBBHMA.Smooth;
                checkSJCMACDBBHMA.StdDevNumber = stdDevNumber;
                stdDevNumber = checkSJCMACDBBHMA.StdDevNumber;

                if (cacheSJCMACDBBHMA != null)
                    for (int idx = 0; idx < cacheSJCMACDBBHMA.Length; idx++)
                        if (cacheSJCMACDBBHMA[idx].BandPeriod == bandPeriod && cacheSJCMACDBBHMA[idx].DotSize == dotSize && cacheSJCMACDBBHMA[idx].Fast == fast && cacheSJCMACDBBHMA[idx].Slow == slow && cacheSJCMACDBBHMA[idx].Smooth == smooth && Math.Abs(cacheSJCMACDBBHMA[idx].StdDevNumber - stdDevNumber) <= double.Epsilon && cacheSJCMACDBBHMA[idx].EqualsInput(input))
                            return cacheSJCMACDBBHMA[idx];

                SJCMACDBBHMA indicator = new SJCMACDBBHMA();
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

                SJCMACDBBHMA[] tmp = new SJCMACDBBHMA[cacheSJCMACDBBHMA == null ? 1 : cacheSJCMACDBBHMA.Length + 1];
                if (cacheSJCMACDBBHMA != null)
                    cacheSJCMACDBBHMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJCMACDBBHMA = tmp;
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
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCMACDBBHMA SJCMACDBBHMA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJCMACDBBHMA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCMACDBBHMA SJCMACDBBHMA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJCMACDBBHMA(input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCMACDBBHMA SJCMACDBBHMA(int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            return _indicator.SJCMACDBBHMA(Input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }

        /// <summary>
        /// The SJCMACDBBHMA (Moving Average Convergence/Divergence) is a trend following momentum indicator that shows the relationship between two moving averages of prices.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCMACDBBHMA SJCMACDBBHMA(Data.IDataSeries input, int bandPeriod, int dotSize, int fast, int slow, int smooth, double stdDevNumber)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJCMACDBBHMA(input, bandPeriod, dotSize, fast, slow, smooth, stdDevNumber);
        }
    }
}
#endregion
