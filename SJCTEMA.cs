// 
// Copyright (C) 2007, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Triple Exponential Moving Average
    /// </summary>
    [Description("Triple Exponential Moving Average")]
	[Gui.Design.DisplayName("SJCTEMA (Triple Exponential Moving Average)")]
    public class SJCTEMA : Indicator
    {
        #region Variables
        private double period1 = 21;
        private double period2 = 3.5;
        private double period3 = 3.5;
		private SJCEMA ema1;
        private SJCEMA ema2;
        private SJCEMA ema3;

        #endregion

        protected override void OnStartUp()
        {
            ema1 = SJCEMA(Input, period1);//SJCEMA(Close, period1);
            ema2 = SJCEMA(ema1, period2);
            ema3 = SJCEMA(ema2, period3);
        }

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.Orange, "SJCTEMA"));
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            Value.Set(ema3[0]);
        }

        #region Properties

        [Description("Number of bars used for calculations")]
        [GridCategory("Parameters")]
        public double Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("Number of bars used for calculations")]
        [GridCategory("Parameters")]
        public double Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("Number of bars used for calculations")]
        [GridCategory("Parameters")]
        public double Period3
        {
            get { return period3; }
            set { period3 = Math.Max(1, value); }
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
        private SJCTEMA[] cacheSJCTEMA = null;

        private static SJCTEMA checkSJCTEMA = new SJCTEMA();

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public SJCTEMA SJCTEMA(double period1, double period2, double period3)
        {
            return SJCTEMA(Input, period1, period2, period3);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public SJCTEMA SJCTEMA(Data.IDataSeries input, double period1, double period2, double period3)
        {
            if (cacheSJCTEMA != null)
                for (int idx = 0; idx < cacheSJCTEMA.Length; idx++)
                    if (Math.Abs(cacheSJCTEMA[idx].Period1 - period1) <= double.Epsilon && Math.Abs(cacheSJCTEMA[idx].Period2 - period2) <= double.Epsilon && Math.Abs(cacheSJCTEMA[idx].Period3 - period3) <= double.Epsilon && cacheSJCTEMA[idx].EqualsInput(input))
                        return cacheSJCTEMA[idx];

            lock (checkSJCTEMA)
            {
                checkSJCTEMA.Period1 = period1;
                period1 = checkSJCTEMA.Period1;
                checkSJCTEMA.Period2 = period2;
                period2 = checkSJCTEMA.Period2;
                checkSJCTEMA.Period3 = period3;
                period3 = checkSJCTEMA.Period3;

                if (cacheSJCTEMA != null)
                    for (int idx = 0; idx < cacheSJCTEMA.Length; idx++)
                        if (Math.Abs(cacheSJCTEMA[idx].Period1 - period1) <= double.Epsilon && Math.Abs(cacheSJCTEMA[idx].Period2 - period2) <= double.Epsilon && Math.Abs(cacheSJCTEMA[idx].Period3 - period3) <= double.Epsilon && cacheSJCTEMA[idx].EqualsInput(input))
                            return cacheSJCTEMA[idx];

                SJCTEMA indicator = new SJCTEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period1 = period1;
                indicator.Period2 = period2;
                indicator.Period3 = period3;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJCTEMA[] tmp = new SJCTEMA[cacheSJCTEMA == null ? 1 : cacheSJCTEMA.Length + 1];
                if (cacheSJCTEMA != null)
                    cacheSJCTEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJCTEMA = tmp;
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
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCTEMA SJCTEMA(double period1, double period2, double period3)
        {
            return _indicator.SJCTEMA(Input, period1, period2, period3);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCTEMA SJCTEMA(Data.IDataSeries input, double period1, double period2, double period3)
        {
            return _indicator.SJCTEMA(input, period1, period2, period3);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCTEMA SJCTEMA(double period1, double period2, double period3)
        {
            return _indicator.SJCTEMA(Input, period1, period2, period3);
        }

        /// <summary>
        /// Triple Exponential Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCTEMA SJCTEMA(Data.IDataSeries input, double period1, double period2, double period3)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJCTEMA(input, period1, period2, period3);
        }
    }
}
#endregion
