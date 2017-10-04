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
	/// The PipChange indicator calculates the Net Change in Pips.
	/// </summary>
    [Description("The PipChange indicator calculates the Net Change in Pips.")]
	public class SJC_PipChange : Indicator
	{
		#region Variables
		private int	period	= 6;
		private int	smooth	= 3;
        private int longext  =   80;
        private int shortext  =  20;
		//private DataSeries PipChangeCalc;
		private RSI RSIHigh;
		private RSI RSILow;
		//private int PipChange = 0;

		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Magenta, "PipChange"));
			//PipChangeCalc = new DataSeries(this,MaximumBarsLookBack.Infinite);
			
			Plots[0].Pen.Width = 1;
		
		}



		/// <summary>
		/// Calculates the indicator value(s) at the current index.
		/// </summary>
		protected override void OnBarUpdate()
		{

            double PipChangeOpen = Open[0];//Close[1];
			double PipChangeClose = Close[0];
			double PipChangeValue = (PipChangeClose - PipChangeOpen); // TickSize;
			
			PipChange.Set(PipChangeValue);

            
/*		 
	    //Colour coriteria for PipChange
         if (PipChangeloseValue > longext)
         {
             PlotColors[0][0] = Color.Red;
         }
         else if (PipChangeloseValue < shortext)
         {
             PlotColors[0][0] = Color.Blue;
         }
         else
         {
             PlotColors[0][0] = Color.Magenta;
         }

        //Colour criteria for RSIH 
        if (rsiHighValue > longext)
        {
            PlotColors[1][0] = Color.Yellow;
        }
        else
        {
            PlotColors[1][0] = Color.Transparent;
        }

        //Colour criteria for RSIL
        if (rsiLowValue < shortext)
        {
            PlotColors[2][0] = Color.Yellow;
        }
        else
        {
            PlotColors[2][0] = Color.Transparent;
        }
 */         
		//	Avg.Set(rsiAvg);
		//	Value.Set(rsi);
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries PipChange
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
        private SJC_PipChange[] cacheSJC_PipChange = null;

        private static SJC_PipChange checkSJC_PipChange = new SJC_PipChange();

        /// <summary>
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        public SJC_PipChange SJC_PipChange(int period)
        {
            return SJC_PipChange(Input, period);
        }

        /// <summary>
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        public SJC_PipChange SJC_PipChange(Data.IDataSeries input, int period)
        {
            if (cacheSJC_PipChange != null)
                for (int idx = 0; idx < cacheSJC_PipChange.Length; idx++)
                    if (cacheSJC_PipChange[idx].Period == period && cacheSJC_PipChange[idx].EqualsInput(input))
                        return cacheSJC_PipChange[idx];

            lock (checkSJC_PipChange)
            {
                checkSJC_PipChange.Period = period;
                period = checkSJC_PipChange.Period;

                if (cacheSJC_PipChange != null)
                    for (int idx = 0; idx < cacheSJC_PipChange.Length; idx++)
                        if (cacheSJC_PipChange[idx].Period == period && cacheSJC_PipChange[idx].EqualsInput(input))
                            return cacheSJC_PipChange[idx];

                SJC_PipChange indicator = new SJC_PipChange();
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

                SJC_PipChange[] tmp = new SJC_PipChange[cacheSJC_PipChange == null ? 1 : cacheSJC_PipChange.Length + 1];
                if (cacheSJC_PipChange != null)
                    cacheSJC_PipChange.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_PipChange = tmp;
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
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PipChange SJC_PipChange(int period)
        {
            return _indicator.SJC_PipChange(Input, period);
        }

        /// <summary>
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PipChange SJC_PipChange(Data.IDataSeries input, int period)
        {
            return _indicator.SJC_PipChange(input, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_PipChange SJC_PipChange(int period)
        {
            return _indicator.SJC_PipChange(Input, period);
        }

        /// <summary>
        /// The PipChange indicator calculates the Net Change in Pips.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_PipChange SJC_PipChange(Data.IDataSeries input, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_PipChange(input, period);
        }
    }
}
#endregion
