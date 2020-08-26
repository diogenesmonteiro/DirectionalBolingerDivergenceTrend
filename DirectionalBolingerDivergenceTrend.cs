#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

namespace NinjaTrader.NinjaScript.Strategies
{
	public class DirectionalBolingerDivergenceTrendV2Code : Strategy
	{
		private EMA EMA1;
		private EMA EMA2;
		private Bollinger Bollinger1;
		private Bollinger Bollinger2;
		private StochRSI StochRSI1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= @"Enter the description for your new custom Strategy here.";
				Name						= "DirectionalBolingerDivergenceTrendV2Code";
				Calculate					= Calculate.OnBarClose;
				EntriesPerDirection				= 1;
				EntryHandling					= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy			= true;
				ExitOnSessionCloseSeconds			= 30;
				IsFillLimitOnTouch				= false;
				MaximumBarsLookBack				= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution				= OrderFillResolution.Standard;
				Slippage					= 0;
				StartBehavior					= StartBehavior.WaitUntilFlat;
				TimeInForce					= TimeInForce.Gtc;
				TraceOrders					= true;
				RealtimeErrorHandling				= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling				= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade				= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				IsInstantiatedOnEachOptimizationIteration	= true;
				StopLoss					= 8;
				TakeProfit					= 4;
				MAPeriod					= 200;
				BollingerStdDev					= 2;
				BollingerPeriod					= 14;
				TrendTrade					= @"TrendTrade";
				CounterTrendTrade				= @"CounterTrendTrade";
				StochRSIPeriod					= 14;
				StochRSIMax					= 1;
				StochRSIMin					= 0;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				EMA1					= EMA(Close, Convert.ToInt32(MAPeriod));
				EMA2					= EMA(Close, Convert.ToInt32(5));
				Bollinger1				= Bollinger(Close, BollingerStdDev, Convert.ToInt32(BollingerPeriod));
				Bollinger2				= Bollinger(Close, BollingerStdDev, Convert.ToInt32(BollingerPeriod));
				StochRSI1				= StochRSI(Close, 14);
				EMA1.Plots[0].Brush 			= Brushes.Goldenrod;
				EMA2.Plots[0].Brush 			= Brushes.Goldenrod;
				Bollinger1.Plots[0].Brush 		= Brushes.Lime;
				Bollinger1.Plots[1].Brush 		= Brushes.Blue;
				Bollinger1.Plots[2].Brush 		= Brushes.Red;
				StochRSI1.Plots[0].Brush 		= Brushes.DarkCyan;
				AddChartIndicator(EMA1);
				AddChartIndicator(EMA2);
				AddChartIndicator(Bollinger1);
				AddChartIndicator(StochRSI1);
				SetProfitTarget(Convert.ToString(TrendTrade), CalculationMode.Ticks, TakeProfit);
				SetStopLoss(Convert.ToString(TrendTrade), CalculationMode.Ticks, StopLoss, false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((EMA1[0] > EMA1[1])
				 && (CrossAbove(Close, EMA2, 1))
				 && (Close[0] > Bollinger1.Middle[0])
				 && (Bollinger2.Upper[0] > Bollinger2.Upper[1])
				 && (Bollinger2.Lower[0] < Bollinger2.Lower[1])
				 && (StochRSI1[0] >= StochRSIMax))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), Convert.ToString(TrendTrade));
			}
			
			 // Set 2
			if ((EMA1[0] < EMA1[1])
				 && (CrossBelow(Close, EMA2, 1))
				 && (Close[0] < Bollinger1.Middle[0])
				 && (Bollinger2.Upper[0] > Bollinger2.Upper[1])
				 && (Bollinger2.Lower[0] < Bollinger2.Lower[1])
				 && (StochRSI1[0] <= StochRSIMin))
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), Convert.ToString(TrendTrade));
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLoss", Order=1, GroupName="Parameters")]
		public int StopLoss
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TakeProfit", Order=2, GroupName="Parameters")]
		public int TakeProfit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="MAPeriod", Order=3, GroupName="Parameters")]
		public int MAPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BollingerStdDev", Order=4, GroupName="Parameters")]
		public int BollingerStdDev
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BollingerPeriod", Order=5, GroupName="Parameters")]
		public int BollingerPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TrendTrade", Order=6, GroupName="Parameters")]
		public string TrendTrade
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="CounterTrendTrade", Order=7, GroupName="Parameters")]
		public string CounterTrendTrade
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StochRSIPeriod", Order=8, GroupName="Parameters")]
		public int StochRSIPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StochRSIMax", Order=9, GroupName="Parameters")]
		public int StochRSIMax
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="StochRSIMin", Order=10, GroupName="Parameters")]
		public int StochRSIMin
		{ get; set; }
		#endregion

	}
}
