Integrates cryptocurrency metrics provided by [CoinGecko](https://www.coingecko.com/).

# Disclaimer

This plugin is developed and provided by ModBros and is not affiliated with CoinGecko.  
It utilizes CoinGecko's public API to fetch cryptocurrency metrics.

# Metrics

Provides the following metrics about the global crypto market:

- Active cryptocurrencies
- Market cap share BTC
- market cap share ETH
- Total market cap
- Total volume

Provides the following individual metrics for all configured coins (see settings):

- Symbol
- Name
- Current price
- Circulating supply
- Total supply
- Total volume
- Price change 24h
- Market cap + Market cap rank
- 24h high + 24h low
- ATH, ATH change, ATH date

# Settings

This plugin exposes the following settings:

| Setting          | Default | Explanation                                                 |
|------------------|---------|-------------------------------------------------------------|
| Update frequency | 10 min  | The frequency (in minutes) to update the metrics.           |
| Coins            | -       | The list of coins to fetch metrics for. (E.g.: BTC,ETH,GRC) |
| Currency         | USD     | The currency to display coin metrics in                     |
