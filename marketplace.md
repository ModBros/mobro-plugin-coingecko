# Crypto Tracker (CoinGecko)

This plugin integrates real-time cryptocurrency data from [CoinGecko](https://www.coingecko.com/) into your MoBro
dashboard, tracking everything from your favorite coins to global market dominance.

---

## Disclaimer

This plugin is developed by ModBros and is not officially affiliated with CoinGecko.  
Data is retrieved using the public CoinGecko API.

---

## Available Metrics

### Individual Coin Metrics

Track any coin (e.g., `BTC`, `ETH`, `SOL`) with detailed statistics:

- **Price & Change**: Current price in your preferred currency and 24h price change.
- **Market Info**: Market cap, rank, and circulating/total supply.
- **Performance**: 24h High/Low and All-Time High (ATH) details including date and percentage change.

### Global Market Overview

- **Dominance**: Real-time market cap dominance for **BTC** and **ETH**.
- **Totals**: Total market capitalization and aggregate trading volume.
- **Scope**: Number of active cryptocurrencies currently in the market.

---

## Setup

1. **Install** the plugin via the MoBro Marketplace.
2. **Configuration**:
   - **Coins**: List the symbols you wish to monitor (e.g., `BTC, ETH, DOGE`).
   - **Currency**: Set your preferred fiat or crypto currency for value display.
3. Add the metrics to your MoBro dashboard.

---

## Settings

| Setting              | Default  | Description                                                    |
|:---------------------|:---------|:---------------------------------------------------------------|
| **Update Frequency** | `10 min` | How often market data is refreshed via the CoinGecko API.      |
| **Coins**            | -        | A comma-separated list of symbols to track (e.g., `BTC, ETH`). |
| **Currency**         | `USD`    | The target currency for all price and market cap metrics.      |
