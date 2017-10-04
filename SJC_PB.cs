// 
// Paint Bar Indicator identifying direction changes in price action
// Date - V1.0 4th April 2012

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
	/// Paint Bar direction Change indicator
	/// </summary>
	[Description("A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes")]
	public class SJC_PB : Indicator
	{
		#region Variables
		
		private Color upColor = Color.Blue;
		private Color dnColor = Color.Red;
		private Color warnColor = Color.Yellow;
		
		private int	length	= 21;
		private bool filledbody = false;
        private double result1;
		private double result2;
        private double result3;
		private DataSeries EMAInputData1;
		//private DataSeries EMAInputData2;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
		//	Add(new Plot(Color.Orange, "Result"));
		//	Plots[0].Pen.Width = 2;
			EMAInputData1 = new DataSeries(this,MaximumBarsLookBack.Infinite);
			//EMAInputData2 = new DataSeries(this,MaximumBarsLookBack.Infinite);
			
			Overlay				= true;
			PriceTypeSupported	= true;
			CalculateOnBarClose = false;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            result1 = SJCRSU(Input, length, 3.5, 3.5)[0];
			
			EMAInputData1.Set(SJCRSU(Input, length, 3.5, 3.5)[0]);
			result2 = EMA(EMAInputData1, 2)[0];
			
			//EMAInputData1.Set(SJCRSU(Input[0], length, 3.5, 3.5)[0]);
            result3 = EMA(EMAInputData1, 5)[0];

            if (result1 > result2 && result1 > result3)
            {
                if (Open[0] > Close[0])		//Down Bar
				{	
					BarColor = upColor;		//Filled body
					CandleOutlineColor = upColor;
				}
				else						//For Up Bar
				{
					if (filledbody)			//For a filled body
					{
						BarColor = upColor;
						CandleOutlineColor = upColor;
					}
					else					//For a Hollow Body
					{
						BarColor = Color.Transparent;
						CandleOutlineColor = upColor;
					}
				}
            }
            else if (result1 < result2 && result1 < result3)
            {
                if (Open[0] > Close[0])		//Down Bar
				{
					BarColor = dnColor;		//Filled body
					CandleOutlineColor = dnColor;
				}
				else						//For Up Bar
				{
					if (filledbody)			//For a filled body
					{
						BarColor = dnColor;
						CandleOutlineColor = dnColor;
					}
					else					//For a Hollow Body
					{
						BarColor = Color.Transparent;
						CandleOutlineColor = dnColor;
					}
				}	
            }
            else if (result1 > result2 && result1 < result3)
            {
                if (Open[0] > Close[0])		//Down Bar
				{
					BarColor = warnColor;		//Filled body
					CandleOutlineColor = warnColor;
				}
				else						//For Up Bar
				{
					if (filledbody)			//For a filled body
					{
						BarColor = warnColor;
						CandleOutlineColor = warnColor;
					}
					else					//For a Hollow Body
					{
						BarColor = Color.Transparent;
						CandleOutlineColor = warnColor;
					}
				}
            }
            else if (result1 < result2 && result1 > result3)
            {
                if (Open[0] > Close[0])		//Down Bar
				{
					BarColor = warnColor;		//Filled body
					CandleOutlineColor = warnColor;
				}
				else						//For Up Bar
				{
					if (filledbody)			//For a filled body
					{
						BarColor = warnColor;
						CandleOutlineColor = warnColor;
					}
					else					//For a Hollow Body
					{
						BarColor = Color.Transparent;
						CandleOutlineColor = warnColor;
					}
				}
             }
			
			//Print("diffvalue = "+diffvalue+"  ,  absvalue = "+absvalue+"  ,  returnvalue = "+returnvalue);//+diffSJCTEMA[0]);
			//Result.Set(5);
	    }


		#region Properties
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Result
        {
            get { return Values[0]; }
        }
		
		
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Length
		{
			get { return length; }
			set { length = Math.Max(1, value); }
		}
		
		[Description("Filled Or Hollow Candle Body?")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Filled Candle Body?")]
		public bool FilledBody
		{
			get { return filledbody; }
			set { filledbody = value; }
		}
		
		[XmlIgnore()]
        [Description("Color EMA Up")]
        [Category("Colors")]
		[Gui.Design.DisplayNameAttribute("01. Color EMA's Up")]
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(UpColor); }
			set { UpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color EMA Dn")]
        [Category("Colors")]
		[Gui.Design.DisplayNameAttribute("02. Color EMA's Down")]
        public Color DnColor
        {
            get { return dnColor; }
            set { dnColor = value; }
        }
		[Browsable(false)]
		public string DnColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(DnColor); }
			set { DnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
				[XmlIgnore()]
        [Description("Color EMA Dn")]
        [Category("Colors")]
		[Gui.Design.DisplayNameAttribute("02. Color EMA's Down")]
        public Color WarnColor
        {
            get { return warnColor; }
            set { warnColor = value; }
        }
		[Browsable(false)]
		public string WarnColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(WarnColor); }
			set { WarnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private SJC_PB[] cacheSJC_PB = null;

        private static SJC_PB checkSJC_PB = new SJC_PB();

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public SJC_PB SJC_PB(bool filledBody, int length)
        {
            return SJC_PB(Input, filledBody, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public SJC_PB SJC_PB(Data.IDataSeries input, bool filledBody, int length)
        {
            if (cacheSJC_PB != null)
                for (int idx = 0; idx < cacheSJC_PB.Length; idx++)
                    if (cacheSJC_PB[idx].FilledBody == filledBody && cacheSJC_PB[idx].Length == length && cacheSJC_PB[idx].EqualsInput(input))
                        return cacheSJC_PB[idx];

            lock (checkSJC_PB)
            {
                checkSJC_PB.FilledBody = filledBody;
                filledBody = checkSJC_PB.FilledBody;
                checkSJC_PB.Length = length;
                length = checkSJC_PB.Length;

                if (cacheSJC_PB != null)
                    for (int idx = 0; idx < cacheSJC_PB.Length; idx++)
                        if (cacheSJC_PB[idx].FilledBody == filledBody && cacheSJC_PB[idx].Length == length && cacheSJC_PB[idx].EqualsInput(input))
                            return cacheSJC_PB[idx];

                SJC_PB indicator = new SJC_PB();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FilledBody = filledBody;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_PB[] tmp = new SJC_PB[cacheSJC_PB == null ? 1 : cacheSJC_PB.Length + 1];
                if (cacheSJC_PB != null)
                    cacheSJC_PB.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_PB = tmp;
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
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PB SJC_PB(bool filledBody, int length)
        {
            return _indicator.SJC_PB(Input, filledBody, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PB SJC_PB(Data.IDataSeries input, bool filledBody, int length)
        {
            return _indicator.SJC_PB(input, filledBody, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PB SJC_PB(bool filledBody, int length)
        {
            return _indicator.SJC_PB(Input, filledBody, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PB SJC_PB(Data.IDataSeries input, bool filledBody, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_PB(input, filledBody, length);
        }
    }
}
#endregion
