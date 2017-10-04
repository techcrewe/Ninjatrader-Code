// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
// 28.10.2010 - Modifications added by Jurbol. Colorcoding up or downslope. www.jurbol.com

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
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
	/// </summary>
	[Description("The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.")]
	public class SJC_PB_MA : Indicator
	{
		#region Variables
		
		private Color upColor = Color.Blue;
		private Color dnColor = Color.Red;
		private Color warnColor = Color.Yellow;
		
		private int	length	= 21;

        private double result1;
		private double result2;
        private double result3;
		private DataSeries EMAInputData1;
        private int returnvalue = 0;
		//private DataSeries EMAInputData2;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "Result"));
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
                //BarColor = upColor;
				CandleOutlineColor = upColor;
                returnvalue = 1;
            }
            else if (result1 < result2 && result1 < result3)
            {
                BarColor = dnColor;
				CandleOutlineColor = dnColor;
                returnvalue = -1;
            }
            else if (result1 > result2 && result1 < result3)
            {
                BarColor = warnColor;
				CandleOutlineColor = warnColor;
                returnvalue = 999;
            }
            else if (result1 < result2 && result1 > result3)
            {
                BarColor = warnColor;
				CandleOutlineColor = warnColor;
                returnvalue = 999;
            }
			//Print("diffvalue = "+diffvalue+"  ,  absvalue = "+absvalue+"  ,  returnvalue = "+returnvalue);//+diffSJCTEMA[0]);
			Result.Set(returnvalue);
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
        private SJC_PB_MA[] cacheSJC_PB_MA = null;

        private static SJC_PB_MA checkSJC_PB_MA = new SJC_PB_MA();

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public SJC_PB_MA SJC_PB_MA(int length)
        {
            return SJC_PB_MA(Input, length);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public SJC_PB_MA SJC_PB_MA(Data.IDataSeries input, int length)
        {
            if (cacheSJC_PB_MA != null)
                for (int idx = 0; idx < cacheSJC_PB_MA.Length; idx++)
                    if (cacheSJC_PB_MA[idx].Length == length && cacheSJC_PB_MA[idx].EqualsInput(input))
                        return cacheSJC_PB_MA[idx];

            lock (checkSJC_PB_MA)
            {
                checkSJC_PB_MA.Length = length;
                length = checkSJC_PB_MA.Length;

                if (cacheSJC_PB_MA != null)
                    for (int idx = 0; idx < cacheSJC_PB_MA.Length; idx++)
                        if (cacheSJC_PB_MA[idx].Length == length && cacheSJC_PB_MA[idx].EqualsInput(input))
                            return cacheSJC_PB_MA[idx];

                SJC_PB_MA indicator = new SJC_PB_MA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_PB_MA[] tmp = new SJC_PB_MA[cacheSJC_PB_MA == null ? 1 : cacheSJC_PB_MA.Length + 1];
                if (cacheSJC_PB_MA != null)
                    cacheSJC_PB_MA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_PB_MA = tmp;
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
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PB_MA SJC_PB_MA(int length)
        {
            return _indicator.SJC_PB_MA(Input, length);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PB_MA SJC_PB_MA(Data.IDataSeries input, int length)
        {
            return _indicator.SJC_PB_MA(input, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PB_MA SJC_PB_MA(int length)
        {
            return _indicator.SJC_PB_MA(Input, length);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA_Slope_Color applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PB_MA SJC_PB_MA(Data.IDataSeries input, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_PB_MA(input, length);
        }
    }
}
#endregion
