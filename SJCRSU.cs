#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class SJCRSU : Indicator
    {
        #region Variables
        // Wizard generated variables
            private double period1 = 21; // Default setting for Period1
            private double period2 = 3.5; // Default setting for Period2
            private double period3 = 3.5; // Default setting for Period3
        // User defined variables (add any user defined variables below)
		private double returnvalue;
		private DataSeries pricediff;
		private DataSeries priceabs;
		private DataSeries myReturnSeries;
		private DataSeries diffSJCTEMA;
		private DataSeries absSJCTEMA;
		//private double pricediff;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.White), PlotStyle.Line, "PRSU"));
            Overlay				= false;
			
			pricediff = new DataSeries(this,MaximumBarsLookBack.Infinite);
            priceabs = new DataSeries(this,MaximumBarsLookBack.Infinite);
			diffSJCTEMA = new DataSeries(this,MaximumBarsLookBack.Infinite);
			absSJCTEMA = new DataSeries(this,MaximumBarsLookBack.Infinite);
            //myReturnSeries = new DataSeries(this, MaximumBarsLookBack.Infinite);
		//	priceabs.Set(0);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			
			if (CurrentBars[0] < period1) return;
			
			pricediff.Set(Input[0]-Input[1]);   //assign data to "pricediff" data series
			double pricediffValue = pricediff[0];   //assign current value of pricediff series to pricediffValue
			
			
            priceabs.Set(Math.Abs(Input[1]));   //assign data to "pricediff" data series***********FAILS WITH THIS LINE
			//double test = Math.Abs(Input[1]);   //THIS LINE WORKS FINE
            double priceabsValue = priceabs[0];   //assign current value of pricediff series to pricediffValue
			
            diffSJCTEMA.Set(100 * (SJCTEMA(pricediff, period1, period2, period3))[0]);  //assign data to diffSJCTEMA data series
            absSJCTEMA.Set(SJCTEMA(priceabs, period1, period2, period3)[0]);       //assign data to absSJCTEMA data series

            double diffvalue = diffSJCTEMA[0];  //assign current value of diffSJCTEMA series to diffvalue
            double absvalue = absSJCTEMA[0];  //assign current value of absSJCTEMA series to absvalue

			
			
			
			if (absvalue !=0)
			{
				//myReturnSeries.Set(diffSJCTEMA[0] / absSJCTEMA[0]);
                returnvalue = diffvalue / absvalue;
			}
			else
			{
				//myReturnSeries.Set(0);
                returnvalue = 0;
			}
			
			//Print("diffvalue = "+diffvalue+"  ,  absvalue = "+absvalue+"  ,  returnvalue = "+returnvalue);//+diffSJCTEMA[0]);
			
            PRSU.Set(returnvalue);


//            myReturnSeries.Set(returnvalue);
//            PRSU.Set(myReturnSeries[0]);
			//PRSU.Set(diffValue[0]);
			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PRSU
        {
            get { return Values[0]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Period1
        {
            get { return period1; }
            set { period1 = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double Period2
        {
            get { return period2; }
            set { period2 = Math.Max(1, value); }
        }

        [Description("")]
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
        private SJCRSU[] cacheSJCRSU = null;

        private static SJCRSU checkSJCRSU = new SJCRSU();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public SJCRSU SJCRSU(double period1, double period2, double period3)
        {
            return SJCRSU(Input, period1, period2, period3);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public SJCRSU SJCRSU(Data.IDataSeries input, double period1, double period2, double period3)
        {
            if (cacheSJCRSU != null)
                for (int idx = 0; idx < cacheSJCRSU.Length; idx++)
                    if (Math.Abs(cacheSJCRSU[idx].Period1 - period1) <= double.Epsilon && Math.Abs(cacheSJCRSU[idx].Period2 - period2) <= double.Epsilon && Math.Abs(cacheSJCRSU[idx].Period3 - period3) <= double.Epsilon && cacheSJCRSU[idx].EqualsInput(input))
                        return cacheSJCRSU[idx];

            lock (checkSJCRSU)
            {
                checkSJCRSU.Period1 = period1;
                period1 = checkSJCRSU.Period1;
                checkSJCRSU.Period2 = period2;
                period2 = checkSJCRSU.Period2;
                checkSJCRSU.Period3 = period3;
                period3 = checkSJCRSU.Period3;

                if (cacheSJCRSU != null)
                    for (int idx = 0; idx < cacheSJCRSU.Length; idx++)
                        if (Math.Abs(cacheSJCRSU[idx].Period1 - period1) <= double.Epsilon && Math.Abs(cacheSJCRSU[idx].Period2 - period2) <= double.Epsilon && Math.Abs(cacheSJCRSU[idx].Period3 - period3) <= double.Epsilon && cacheSJCRSU[idx].EqualsInput(input))
                            return cacheSJCRSU[idx];

                SJCRSU indicator = new SJCRSU();
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

                SJCRSU[] tmp = new SJCRSU[cacheSJCRSU == null ? 1 : cacheSJCRSU.Length + 1];
                if (cacheSJCRSU != null)
                    cacheSJCRSU.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJCRSU = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCRSU SJCRSU(double period1, double period2, double period3)
        {
            return _indicator.SJCRSU(Input, period1, period2, period3);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCRSU SJCRSU(Data.IDataSeries input, double period1, double period2, double period3)
        {
            return _indicator.SJCRSU(input, period1, period2, period3);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCRSU SJCRSU(double period1, double period2, double period3)
        {
            return _indicator.SJCRSU(Input, period1, period2, period3);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCRSU SJCRSU(Data.IDataSeries input, double period1, double period2, double period3)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJCRSU(input, period1, period2, period3);
        }
    }
}
#endregion
