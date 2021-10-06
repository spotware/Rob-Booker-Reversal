using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RobBookerReversal : Indicator
    {
        private MacdCrossOver _macd;

        private StochasticOscillator _stochastic;

        private Color _buySignalColor, _sellSignalColor;

        [Parameter("Source", Group = "MACD")]
        public DataSeries MacdSource { get; set; }

        [Parameter("Long Cycle", DefaultValue = 26, Group = "MACD")]
        public int MacdLongCycle { get; set; }

        [Parameter("Short Cycle", DefaultValue = 12, Group = "MACD")]
        public int MacdShortCycle { get; set; }

        [Parameter("Signal Periods", DefaultValue = 9, Group = "MACD")]
        public int MacdSignalPeriods { get; set; }

        [Parameter("%K Periods", DefaultValue = 9, Group = "Stochastic")]
        public int StochasticKPeriods { get; set; }

        [Parameter("%K Slowing", DefaultValue = 3, Group = "Stochastic")]
        public int StochasticKSlowing { get; set; }

        [Parameter("%D Periods", DefaultValue = 9, Group = "Stochastic")]
        public int StochasticDPeriods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple, Group = "Stochastic")]
        public MovingAverageType StochasticMaType { get; set; }

        [Parameter("Overbought", DefaultValue = 70, Group = "Stochastic")]
        public double StochasticOverbought { get; set; }

        [Parameter("Oversold", DefaultValue = 30, Group = "Stochastic")]
        public double StochasticOversold { get; set; }

        [Parameter("Color", DefaultValue = "Lime", Group = "Buy Signal")]
        public string BuySignalColor { get; set; }

        [Parameter("Color", DefaultValue = "Red", Group = "Sell Signal")]
        public string SellSignalColor { get; set; }

        protected override void Initialize()
        {
            _macd = Indicators.MacdCrossOver(MacdSource, MacdLongCycle, MacdShortCycle, MacdSignalPeriods);
            _stochastic = Indicators.StochasticOscillator(StochasticKPeriods, StochasticKSlowing, StochasticDPeriods, StochasticMaType);

            _buySignalColor = GetColor(BuySignalColor);
            _sellSignalColor = GetColor(SellSignalColor);
        }

        public override void Calculate(int index)
        {
            if (_macd.MACD[index - 1] > 0 && _macd.MACD[index - 2] <= 0 && _stochastic.PercentK[index - 1] <= StochasticOversold)
            {
                Chart.DrawIcon(GetIconName(index - 1, TradeType.Buy), ChartIconType.UpArrow, Bars.OpenTimes[index - 1], Bars.LowPrices[index - 1], _buySignalColor);
            }
            else if (_macd.MACD[index - 1] < 0 && _macd.MACD[index - 2] >= 0 && _stochastic.PercentK[index - 1] >= StochasticOverbought)
            {
                Chart.DrawIcon(GetIconName(index - 1, TradeType.Sell), ChartIconType.DownArrow, Bars.OpenTimes[index - 1], Bars.HighPrices[index - 1], _sellSignalColor);
            }
        }

        private string GetIconName(int index, TradeType tradeType)
        {
            return string.Format("{0}_{1}_RobBookerReversal", index, tradeType);
        }

        private Color GetColor(string colorString, int alpha = 255)
        {
            var color = colorString[0] == '#' ? Color.FromHex(colorString) : Color.FromName(colorString);

            return Color.FromArgb(alpha, color);
        }
    }
}