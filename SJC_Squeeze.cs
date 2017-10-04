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
	public class SJC_Squeeze : Indicator
	{
		#region Variables
		
		private Color buyColor1 = Color.Blue;
        private Color buyColor2 = Color.RoyalBlue;
		private Color sellColor1 = Color.Red;
		private Color sellColor2 = Color.DarkRed;
        private Color biasUpColor = Color.Blue;
        private Color biasDnColor = Color.Red;
        private Color _SqzColor = Color.Blue;
        private Color _BiasColor = Color.Red;
		
		private int	length	= 21;
		private bool ctTrend = true;
        //private double result1;
		//private double result2;
        //private double result3;
		private DataSeries SQZresult;
		private DataSeries BIASresult;
		private DataSeries EMAresult;
		private DataSeries EMAInputData1;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			//Add(new Plot(_SqzColor, PlotStyle.Bar, "Squeeze"));
			Add(new Plot(_BiasColor, PlotStyle.Dot, "Bias"));
			Add(new Plot(_SqzColor, PlotStyle.Bar, "Squeeze"));
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 3;
			
			SQZresult = new DataSeries(this,MaximumBarsLookBack.Infinite);
			BIASresult = new DataSeries(this,MaximumBarsLookBack.Infinite);
			EMAresult = new DataSeries(this,MaximumBarsLookBack.Infinite);
			EMAInputData1 = new DataSeries(this,MaximumBarsLookBack.Infinite);
			
			Overlay				= true;
			PriceTypeSupported	= true;
			CalculateOnBarClose = false;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
            
			if (CurrentBars[0] < length) return;
			
			SQZresult.Set(SJCRSU(Input, length, 4, 4)[0]);

			EMAInputData1.Set(SJCRSU(Input, length, 3.5, 3.5)[0]);
            EMAresult.Set(EMA(EMAInputData1, 7)[0]);
            
            double SqueezeValue = SQZresult[0];
			Squeeze.Set(100 * SqueezeValue);

            Print("Squeeze = "+SQZresult[0]);

			if (SQZresult[0] > SQZresult[1] && SQZresult[0] > 0)
			{
                _SqzColor = buyColor1;
         	}
			
			if (SQZresult[0] < SQZresult[1] && SQZresult[0] < 0)
			{
                _SqzColor = sellColor1;
         	}
			
			if (SQZresult[0] > SQZresult[1] && SQZresult[0] < 0)
			{
                _SqzColor = sellColor2;
         	}
			
			if (SQZresult[0] < SQZresult[1] && SQZresult[0] > 0)
			{
                _SqzColor = buyColor2;
         	}
			
			
			Print("ctTrend = "+ctTrend);
						
			if (ctTrend && SQZresult[0] > 0 && SQZresult[0] < EMAresult[0])
			{
                _SqzColor = sellColor2;
         	}	
			if (ctTrend && SQZresult[0] < 0 && SQZresult[0] > EMAresult[0])
			{
                _SqzColor = buyColor2;
         	}
            PlotColors[1][0] = _SqzColor;

            BIASresult.Set(SJC_NLR(0,9).NLRprice[0] - KAMA(Typical, 2, 11, 11)[0]);
            //BIASresult.Set(SJC_NLR.NLRprice[0] - KAMA(Typical, 2, 11, 11)[0]);
			//double BiasValue = BIASresult[0];
            Bias.Set(0);

            if (BIASresult[0] > 0)
            {
                _BiasColor = biasUpColor;
            }
            if (BIASresult[0] < 0)
            {
                _BiasColor = biasDnColor;
            }

            PlotColors[0][0] = _BiasColor;
			
			
			//Print("Bias = " + BIASresult[0]);
			
	    }


		#region Properties
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Bias
		{
			get { return Values[0]; }
		}
				
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Squeeze
        {
            get { return Values[1]; }
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
		
		[Description("Counter trend mode?")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Counter Trend?")]
		public bool CounterTrend
		{
			get { return ctTrend; }
			set { ctTrend = value; }
		}
		
		[XmlIgnore()]
        [Description("Buy Color 1")]
        [Category("Colors")]
		[Gui.Design.DisplayNameAttribute("01. Buy Color 1")]
        public Color BuyColor1
        {
            get { return buyColor1; }
            set { buyColor1 = value; }
        }
		[Browsable(false)]
		public string BuyColor1Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(BuyColor1); }
			set { BuyColor1 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

        [XmlIgnore()]
        [Description("Buy Color 2")]
        [Category("Colors")]
        [Gui.Design.DisplayNameAttribute("01. Buy Color 2")]
        public Color BuyColor2
        {
            get { return buyColor2; }
            set { buyColor2 = value; }
        }
        [Browsable(false)]
        public string BuyColor2Serialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(BuyColor2); }
            set { BuyColor2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }


        [XmlIgnore()]
        [Description("Sell Color 1")]
        [Category("Colors")]
        [Gui.Design.DisplayNameAttribute("01. Sell Color 1")]
        public Color SellColor1
        {
            get { return sellColor1; }
            set { sellColor1 = value; }
        }
        [Browsable(false)]
        public string SellColor1Serialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(SellColor1); }
            set { SellColor1 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Sell Color 2")]
        [Category("Colors")]
        [Gui.Design.DisplayNameAttribute("01. Sell Color 2")]
        public Color SellColor2
        {
            get { return sellColor2; }
            set { sellColor2 = value; }
        }
        [Browsable(false)]
        public string SellColor2Serialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(SellColor2); }
            set { SellColor2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Bias Up Color")]
        [Category("Colors")]
        [Gui.Design.DisplayNameAttribute("05. Bias Up Color")]
        public Color BiasUpColor
        {
            get { return biasUpColor; }
            set { biasUpColor = value; }
        }
        [Browsable(false)]
        public string BiasUpColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(BiasUpColor); }
            set { BiasUpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }

        [XmlIgnore()]
        [Description("Bias Down Color")]
        [Category("Colors")]
        [Gui.Design.DisplayNameAttribute("06. Bias Down Color")]
        public Color BiasDnColor
        {
            get { return biasDnColor; }
            set { biasDnColor = value; }
        }
        [Browsable(false)]
        public string BiasDnColorSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(BiasDnColor); }
            set { BiasDnColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
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
        private SJC_Squeeze[] cacheSJC_Squeeze = null;

        private static SJC_Squeeze checkSJC_Squeeze = new SJC_Squeeze();

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public SJC_Squeeze SJC_Squeeze(bool counterTrend, int length)
        {
            return SJC_Squeeze(Input, counterTrend, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public SJC_Squeeze SJC_Squeeze(Data.IDataSeries input, bool counterTrend, int length)
        {
            if (cacheSJC_Squeeze != null)
                for (int idx = 0; idx < cacheSJC_Squeeze.Length; idx++)
                    if (cacheSJC_Squeeze[idx].CounterTrend == counterTrend && cacheSJC_Squeeze[idx].Length == length && cacheSJC_Squeeze[idx].EqualsInput(input))
                        return cacheSJC_Squeeze[idx];

            lock (checkSJC_Squeeze)
            {
                checkSJC_Squeeze.CounterTrend = counterTrend;
                counterTrend = checkSJC_Squeeze.CounterTrend;
                checkSJC_Squeeze.Length = length;
                length = checkSJC_Squeeze.Length;

                if (cacheSJC_Squeeze != null)
                    for (int idx = 0; idx < cacheSJC_Squeeze.Length; idx++)
                        if (cacheSJC_Squeeze[idx].CounterTrend == counterTrend && cacheSJC_Squeeze[idx].Length == length && cacheSJC_Squeeze[idx].EqualsInput(input))
                            return cacheSJC_Squeeze[idx];

                SJC_Squeeze indicator = new SJC_Squeeze();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CounterTrend = counterTrend;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_Squeeze[] tmp = new SJC_Squeeze[cacheSJC_Squeeze == null ? 1 : cacheSJC_Squeeze.Length + 1];
                if (cacheSJC_Squeeze != null)
                    cacheSJC_Squeeze.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_Squeeze = tmp;
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
        public Indicator.SJC_Squeeze SJC_Squeeze(bool counterTrend, int length)
        {
            return _indicator.SJC_Squeeze(Input, counterTrend, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_Squeeze SJC_Squeeze(Data.IDataSeries input, bool counterTrend, int length)
        {
            return _indicator.SJC_Squeeze(input, counterTrend, length);
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
        public Indicator.SJC_Squeeze SJC_Squeeze(bool counterTrend, int length)
        {
            return _indicator.SJC_Squeeze(Input, counterTrend, length);
        }

        /// <summary>
        /// A paint bar indicator showing the prevailing trend in price action. Also highlights direction changes
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_Squeeze SJC_Squeeze(Data.IDataSeries input, bool counterTrend, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_Squeeze(input, counterTrend, length);
        }
    }
}
#endregion
