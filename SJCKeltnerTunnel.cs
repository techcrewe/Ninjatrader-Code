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
	/// Keltner Channel. The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
	/// </summary>
	[Description("The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.")]
	public class SJCKeltnerTunnel : Indicator
	{
		#region Variables
		private	int					period				= 17;
		private double				offsetMultiplier	= 0.5;
		private DataSeries		diff;
        private Color upColor = Color.DarkRed;
        private Color dnColor = Color.Blue;
		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(Color.Transparent, "Midline"));
			Add(new Plot(Color.Blue,     "Upper"));
			Add(new Plot(Color.Blue,     "Lower"));

			diff				= new DataSeries(this);

			Overlay				= true;
			PriceTypeSupported	= false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick).
		/// </summary>
		protected override void OnBarUpdate()
		{
			diff.Set(High[0] - Low[0]);

			double middle	= EMA(Typical, Period)[0];
			double offset	= EMA(diff, Period)[0] * offsetMultiplier;

			double upper	= middle + offset;
			double lower	= middle - offset;

			Midline.Set(middle);
			Upper.Set(upper);
			Lower.Set(lower);

            if (Rising(Midline))
            {
                    PlotColors[1][0] = upColor;
                    PlotColors[2][0] = upColor;
            }
            if (Falling(Midline))
            {
                    PlotColors[1][0] = dnColor;
                    PlotColors[2][0] = dnColor;
            }  

		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Numbers of bars used for calculations")]
		[Category("Parameters")]
		public int Period
		{
			get { return period; }
			set { period = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("How much to expand the upper and lower band from the normal offset")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Offset multiplier")]
		public double OffsetMultiplier
		{
			get { return offsetMultiplier; }
			set { offsetMultiplier = Math.Max(0.01, value); }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Midline
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Upper
		{
			get { return Values[1]; }
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Lower
		{
			get { return Values[2]; }
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
        #endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SJCKeltnerTunnel[] cacheSJCKeltnerTunnel = null;

        private static SJCKeltnerTunnel checkSJCKeltnerTunnel = new SJCKeltnerTunnel();

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public SJCKeltnerTunnel SJCKeltnerTunnel(double offsetMultiplier, int period)
        {
            return SJCKeltnerTunnel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public SJCKeltnerTunnel SJCKeltnerTunnel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            if (cacheSJCKeltnerTunnel != null)
                for (int idx = 0; idx < cacheSJCKeltnerTunnel.Length; idx++)
                    if (Math.Abs(cacheSJCKeltnerTunnel[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheSJCKeltnerTunnel[idx].Period == period && cacheSJCKeltnerTunnel[idx].EqualsInput(input))
                        return cacheSJCKeltnerTunnel[idx];

            lock (checkSJCKeltnerTunnel)
            {
                checkSJCKeltnerTunnel.OffsetMultiplier = offsetMultiplier;
                offsetMultiplier = checkSJCKeltnerTunnel.OffsetMultiplier;
                checkSJCKeltnerTunnel.Period = period;
                period = checkSJCKeltnerTunnel.Period;

                if (cacheSJCKeltnerTunnel != null)
                    for (int idx = 0; idx < cacheSJCKeltnerTunnel.Length; idx++)
                        if (Math.Abs(cacheSJCKeltnerTunnel[idx].OffsetMultiplier - offsetMultiplier) <= double.Epsilon && cacheSJCKeltnerTunnel[idx].Period == period && cacheSJCKeltnerTunnel[idx].EqualsInput(input))
                            return cacheSJCKeltnerTunnel[idx];

                SJCKeltnerTunnel indicator = new SJCKeltnerTunnel();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.OffsetMultiplier = offsetMultiplier;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                SJCKeltnerTunnel[] tmp = new SJCKeltnerTunnel[cacheSJCKeltnerTunnel == null ? 1 : cacheSJCKeltnerTunnel.Length + 1];
                if (cacheSJCKeltnerTunnel != null)
                    cacheSJCKeltnerTunnel.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSJCKeltnerTunnel = tmp;
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
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCKeltnerTunnel SJCKeltnerTunnel(double offsetMultiplier, int period)
        {
            return _indicator.SJCKeltnerTunnel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCKeltnerTunnel SJCKeltnerTunnel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            return _indicator.SJCKeltnerTunnel(input, offsetMultiplier, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SJCKeltnerTunnel SJCKeltnerTunnel(double offsetMultiplier, int period)
        {
            return _indicator.SJCKeltnerTunnel(Input, offsetMultiplier, period);
        }

        /// <summary>
        /// The Keltner Channel is a similar indicator to Bollinger Bands. Here the midline is a standard moving average with the upper and lower bands offset by the SMA of the difference between the high and low of the previous bars. The offset multiplier as well as the SMA period is configurable.
        /// </summary>
        /// <returns></returns>
        public Indicator.SJCKeltnerTunnel SJCKeltnerTunnel(Data.IDataSeries input, double offsetMultiplier, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SJCKeltnerTunnel(input, offsetMultiplier, period);
        }
    }
}
#endregion
