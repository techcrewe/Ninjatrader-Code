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
    /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
    /// </summary>
    [Description("Non-Linear Regression Function. Returns NLR value and StdDev for each bar")]
    public class SJC_NLR : Indicator
    {
        #region Variables
        // Define Input Variables
        private int length = 9;//double length = 9; 
        //private DataSeries price = Close; 
        private int desiredBar = 0;
        private double NLRpriceValue = 0; 
        private double StdDevValue = 0;

        // Code variables
		private double AvgX = 0;
        private double AvgY = 0;
        private double N = 0;
        private double XM = 0;
        private double YM = 0;
        private double XM2 = 0;
        private double SXX = 0;
        private double SXY = 0;
        private double SYY = 0;
        private double SSYY = 0;
        private double SXX2 = 0;
        private double SX2X2 = 0;
        private double SYX2 = 0;
        private double ACoeff = 0;
        private double BCoeff = 0;
        private double CCoeff = 0;
        private double X = 0;
        private double Y = 0;
        private double MaxLength = 100;
        private double S = 0;
        private double TX = 0;
        private double TY = 0;
        private double RV = 0;
        private double ERR = 0;
		private double value99 = 0;

        double[] YValue = new double[100];
        double[] XValue = new double[100];

        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Line, "NLRprice"));
            Add(new Plot(Color.Red, PlotStyle.Line, "SDev"));
            Overlay	= false;
			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            N=length;
			Print("Length = "+length);
			//Print("CurrentBars = " + CurrentBars[0]);
			if ( CurrentBars[0] < length) return; //N+1) return;
			
			length = 9;//Length;
			Print("CurrentBars = " + CurrentBars[0]);
			
			//if (CurrentBar == 1)
            //{
            //    N = length;
			//	Print("starting here N = " +N);
            //{  be sure array size is protected  } 
            //    if (N > MaxLength) N = MaxLength;
            //    if (N < 2) N=2;
            //}
			
			//N = 3;//length;
            Print("Length = "+N);
			//{ fill arrays }
            for (int value1 = 1; value1 < N+1; value1++)
            {
                XValue[value1] = value1;
				
                YValue[value1] = Close[value1 - 1];

            }
			//Print("YValue= " + YValue[9]);
						
            //{ calc averages of x,y pairs }
            AvgX = 0;
            AvgY = 0;
            for (int value2 = 1; value2 < N+1; value2++)
            {
                AvgX = AvgX + XValue[value2];
                AvgY = AvgY + YValue[value2];
            }
			Print("AvgYsum = " + AvgY);
            if (N != 0)
            {
                AvgX = AvgX / N;
                AvgY = AvgY / N;
            }
			
			Print("AvgY_1 = " + AvgY);
			
            //{ do regression and calc 3 coefficients }
            SXX = 0;
            SXY = 0;
            SYY = 0;
            SXX2 = 0;
            SX2X2 = 0;
            SYX2 = 0;
            for (int value1 = 1; value1 < N+1; value1++)
            {
                XM = XValue[value1] - AvgX; 
                YM = YValue[value1] - AvgY;
                XM2 = XValue[value1]*XValue[value1] - AvgX*AvgX;
                SXX = SXX + XM*XM;
                SXY = SXY + XM*YM;
                SYY = SYY + YM*YM;
                SXX2 = SXX2 + XM*XM2;
                SX2X2 = SX2X2 + XM2*XM2;
                SYX2 = SYX2 + YM*XM2;
            }
			
            value99 = SXX*SX2X2 - SXX2*SXX2;
            if (value99 != 0)
            {
                BCoeff = ( SXY*SX2X2 - SYX2*SXX2 ) / value99 ;
                CCoeff = ( SXX*SYX2 - SXX2*SXY ) / value99;
            }

            ACoeff = AvgY - BCoeff*AvgX - CCoeff*AvgX*AvgX;

            //{ calc estimated price for desired bar
            //  DesiredBar: use + for past and - for future
            //  eg., for next bar in future:
            //  DesiredBar = -1 }
            NLRpriceValue = ACoeff + BCoeff*desiredBar + CCoeff*desiredBar*desiredBar;

            //OUTPUT NLRprice
            NLRprice.Set(NLRpriceValue);

			
            //{  calc std dev  }
            S = 0;
            AvgY = 0;

            for (int value1 = 1; value1 < N+1; value1++)
            {
                TY = YValue[value1];
                TX = XValue[value1];
                RV = ACoeff + BCoeff*XValue[value1]+ 
                CCoeff*XValue[value1]*XValue[value1];
                ERR = TY - RV;
                AvgY = AvgY + TY;
                S = S + ERR*ERR;
            }
			
			//Print(NLRprice[0]);
			
            if (N-1 != 0)
            {
                if (S/(N-1) > 0)
                {
                    S = Math.Sqrt(S /(N-1));
                }
            }
			S = Math.Sqrt(S /(N-1));
            //SDev.Set(S);
			Print("NLR = "+NLRprice[0]+" , SDev = "+SDev[0]);
        }
		
        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries NLRprice
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SDev
        {
            get { return Values[1]; }
        }
        
        [Description("")]
        [GridCategory("Parameters")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DesiredBar
        {
            get { return desiredBar; }
			set { length = value; }
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
        private SJC_NLR[] cacheSJC_NLR = null;

        private static SJC_NLR checkSJC_NLR = new SJC_NLR();

        /// <summary>
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        public SJC_NLR SJC_NLR(int desiredBar, int length)
        {
            return SJC_NLR(Input, desiredBar, length);
        }

        /// <summary>
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        public SJC_NLR SJC_NLR(Data.IDataSeries input, int desiredBar, int length)
        {
            if (cacheSJC_NLR != null)
                for (int idx = 0; idx < cacheSJC_NLR.Length; idx++)
                    if (cacheSJC_NLR[idx].DesiredBar == desiredBar && cacheSJC_NLR[idx].Length == length && cacheSJC_NLR[idx].EqualsInput(input))
                        return cacheSJC_NLR[idx];

            lock (checkSJC_NLR)
            {
                checkSJC_NLR.DesiredBar = desiredBar;
                desiredBar = checkSJC_NLR.DesiredBar;
                checkSJC_NLR.Length = length;
                length = checkSJC_NLR.Length;

                if (cacheSJC_NLR != null)
                    for (int idx = 0; idx < cacheSJC_NLR.Length; idx++)
                        if (cacheSJC_NLR[idx].DesiredBar == desiredBar && cacheSJC_NLR[idx].Length == length && cacheSJC_NLR[idx].EqualsInput(input))
                            return cacheSJC_NLR[idx];

                SJC_NLR indicator = new SJC_NLR();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.DesiredBar = desiredBar;
                indicator.Length = length;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJC_NLR[] tmp = new SJC_NLR[cacheSJC_NLR == null ? 1 : cacheSJC_NLR.Length + 1];
                if (cacheSJC_NLR != null)
                    cacheSJC_NLR.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJC_NLR = tmp;
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
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_NLR SJC_NLR(int desiredBar, int length)
        {
            return _indicator.SJC_NLR(Input, desiredBar, length);
        }

        /// <summary>
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_NLR SJC_NLR(Data.IDataSeries input, int desiredBar, int length)
        {
            return _indicator.SJC_NLR(input, desiredBar, length);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJC_NLR SJC_NLR(int desiredBar, int length)
        {
            return _indicator.SJC_NLR(Input, desiredBar, length);
        }

        /// <summary>
        /// Non-Linear Regression Function. Returns NLR value and StdDev for each bar
        /// </summary>
        /// <returns></returns>
        public Indicator.SJC_NLR SJC_NLR(Data.IDataSeries input, int desiredBar, int length)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJC_NLR(input, desiredBar, length);
        }
    }
}
#endregion
