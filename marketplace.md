Tracks cryptocurrency metrics using data powered by [CoinGecko](https://www.coingecko.com/).

# Disclaimer

This plugin is created and maintained by ModBros and is not associated with CoinGecko.  
It uses CoinGecko's public API to retrieve cryptocurrency metrics.

---

# Metrics

The following global cryptocurrency market metrics are available:

- Number of active cryptocurrencies
- Market cap dominance of BTC
- Market cap dominance of ETH
- Total market capitalization
- Total trading volume

For all configured coins (as specified in settings), the plugin provides:

- Symbol
- Name
- Current price
- Circulating supply
- Total supply
- Total trading volume
- 24-hour price change
- Market capitalization + Market cap rank
- 24-hour high + 24-hour low
- All-time high (ATH), ATH change percentage, ATH date

---

# Settings

The plugin includes the following configurable options:

| Setting          | Default | Description                                                 |
|------------------|---------|-------------------------------------------------------------|
| Update frequency | 10 min  | Interval (in minutes) for refreshing metrics.               |
| Coins            | -       | A list of coins to track metrics for (e.g., BTC, ETH, GRC). |
| Currency         | USD     | The currency to display all metrics in.                     |
