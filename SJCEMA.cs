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
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
	/// </summary>
	[Description("The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.")]
	public class SJCEMA : Indicator
	{
		#region Variables
		private double			period		= 14;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Orange, "EMA"));

			Overlay				= true;
		}
		
		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			Value.Set(CurrentBar == 0 ? Input[0] : Input[0] * (2.0 / (1 + Period)) + (1 - (2.0 / (1 + Period))) * Value[1]);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[GridCategory("Parameters")]
		public double Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
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
        private SJCEMA[] cacheSJCEMA = null;

        private static SJCEMA checkSJCEMA = new SJCEMA();

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public SJCEMA SJCEMA(double period)
        {
            return SJCEMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public SJCEMA SJCEMA(Data.IDataSeries input, double period)
        {
            if (cacheSJCEMA != null)
                for (int idx = 0; idx < cacheSJCEMA.Length; idx++)
                    if (Math.Abs(cacheSJCEMA[idx].Period - period) <= double.Epsilon && cacheSJCEMA[idx].EqualsInput(input))
                        return cacheSJCEMA[idx];

            lock (checkSJCEMA)
            {
                checkSJCEMA.Period = period;
                period = checkSJCEMA.Period;

                if (cacheSJCEMA != null)
                    for (int idx = 0; idx < cacheSJCEMA.Length; idx++)
                        if (Math.Abs(cacheSJCEMA[idx].Period - period) <= double.Epsilon && cacheSJCEMA[idx].EqualsInput(input))
                            return cacheSJCEMA[idx];

                SJCEMA indicator = new SJCEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJCEMA[] tmp = new SJCEMA[cacheSJCEMA == null ? 1 : cacheSJCEMA.Length + 1];
                if (cacheSJCEMA != null)
                    cacheSJCEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJCEMA = tmp;
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
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCEMA SJCEMA(double period)
        {
            return _indicator.SJCEMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCEMA SJCEMA(Data.IDataSeries input, double period)
        {
            return _indicator.SJCEMA(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCEMA SJCEMA(double period)
        {
            return _indicator.SJCEMA(Input, period);
        }

        /// <summary>
        /// The Exponential Moving Average is an indicator that shows the average value of a security's price over a period of time. When calculating a moving average. The EMA applies more weight to recent prices than the SMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCEMA SJCEMA(Data.IDataSeries input, double period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJCEMA(input, period);
        }
    }
}
#endregion
