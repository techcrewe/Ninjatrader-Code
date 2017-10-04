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
	/// The PipATR indicator calculates the Average True Range in Pips.
	/// </summary>
    [Description("The PipATR indicator calculates the Average True Range in Pips.")]
	public class SJC_PipATR : Indicator
	{
		#region Variables
		private int	period	= 6;
		private ATR PipATRCalc;

		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Magenta, "PipATR"));
		
			Plots[0].Pen.Width = 1;
		
		}

		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{

            PipATRCalc = ATR(Inputs[0],Period);
			
			double PipATRValue = PipATRCalc[0] / TickSize;

			PipATR.Set(Math.Truncate(PipATRValue));
		}

	
		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PipATR
		{
			get { return Values[0]; }
		}
				
		/// <summary>
		/// </summary>
//		[Browsable(false)]
//		[XmlIgnore()]
//		public DataSeries RSIH
//		{
//			get { return Values[1]; }
//		}
		
				/// <summary>
		/// </summary>
//		[Browsable(false)]
//		[XmlIgnore()]
//		public DataSeries RSIL
//		{
//			get { return Values[2]; }
//		}
		
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
//		[Description("Number of bars for smoothing")]
//		[GridCategory("Parameters")]
//		public int Smooth
//		{
//			get { return smooth; }
//			set { smooth = Math.Max(1, value); }
//		}

        /// <summary>
        /// </summary>
//        [Description("Number of bars for smoothing")]
//       [GridCategory("Parameters")]
//        public int LongExt
//        {
//            get { return longext; }
//            set { longext = Math.Max(1, value); }
//        }

        /// <summary>
        /// </summary>
//        [Description("Number of bars for smoothing")]
//        [GridCategory("Parameters")]
//        public int ShortExt
//        {
//            get { return shortext; }
//            set { shortext = Math.Max(1, value); }
//        }


		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SJC_PipATR[] cacheSJC_PipATR = null;

        private static SJC_PipATR checkSJC_PipATR = new SJC_PipATR();

        /// <summary>
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        public SJC_PipATR SJC_PipATR(int period)
        {
            return SJC_PipATR(Input, period);
        }

        /// <summary>
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        public SJC_PipATR SJC_PipATR(Data.IDataSeries input, int period)
        {
            if (cacheSJC_PipATR != null)
                for (int idx = 0; idx < cacheSJC_PipATR.Length; idx++)
                    if (cacheSJC_PipATR[idx].Period == period && cacheSJC_PipATR[idx].EqualsInput(input))
                        return cacheSJC_PipATR[idx];

            lock (checkSJC_PipATR)
            {
                checkSJC_PipATR.Period = period;
                period = checkSJC_PipATR.Period;

                if (cacheSJC_PipATR != null)
                    for (int idx = 0; idx < cacheSJC_PipATR.Length; idx++)
                        if (cacheSJC_PipATR[idx].Period == period && cacheSJC_PipATR[idx].EqualsInput(input))
                            return cacheSJC_PipATR[idx];

                SJC_PipATR indicator = new SJC_PipATR();
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

                SJC_PipATR[] tmp = new SJC_PipATR[cacheSJC_PipATR == null ? 1 : cacheSJC_PipATR.Length + 1];
                if (cacheSJC_PipATR != null)
                    cacheSJC_PipATR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_PipATR = tmp;
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
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PipATR SJC_PipATR(int period)
        {
            return _indicator.SJC_PipATR(Input, period);
        }

        /// <summary>
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PipATR SJC_PipATR(Data.IDataSeries input, int period)
        {
            return _indicator.SJC_PipATR(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PipATR SJC_PipATR(int period)
        {
            return _indicator.SJC_PipATR(Input, period);
        }

        /// <summary>
        /// The PipATR indicator calculates the Average True Range in Pips.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PipATR SJC_PipATR(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_PipATR(input, period);
        }
    }
}
#endregion
