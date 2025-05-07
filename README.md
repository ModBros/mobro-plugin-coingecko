# mobro-plugin-coingecko

![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/ModBros/mobro-plugin-coingecko?label=version)
![GitHub](https://img.shields.io/github/license/ModBros/mobro-plugin-coingecko)
[![MoBro](https://img.shields.io/badge/-MoBro-red.svg)](https://mobro.app)
[![Discord](https://img.shields.io/discord/620204412706750466.svg?color=7389D8&labelColor=6A7EC2&logo=discord&logoColor=ffffff&style=flat-square)](https://discord.com/invite/DSNX4ds)

**CoinGecko plugin for MoBro**

This plugin integrates cryptocurrency metrics from [CoinGecko](https://www.coingecko.com/)
into [MoBro](https://mobro.app).

This plugin is created and maintained by ModBros and is not associated with CoinGecko.  
It uses the [CoinGeckoAsyncApi](https://github.com/tosunthex/CoinGecko) package to access the public CoinGecko API.

## Metrics

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

## Settings

The plugin includes the following configurable options:

| Setting          | Default | Description                                                 |
|------------------|---------|-------------------------------------------------------------|
| Update frequency | 10 min  | Interval (in minutes) for refreshing metrics.               |
| Coins            | -       | A list of coins to track metrics for (e.g., BTC, ETH, GRC). |
| Currency         | USD     | The currency to display all metrics in.                     |

## SDK

This plugin is built using the [MoBro Plugin SDK](https://github.com/ModBros/mobro-plugin-sdk).  
Developer documentation is available at [developer.mobro.app](https://developer.mobro.app).
