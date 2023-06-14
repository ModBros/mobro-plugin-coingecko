using System;
using System.Collections.Generic;
using CoinGecko.Entities.Response.Global;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.CoinGecko.Extensions;

public static class GlobalDataExtensions
{
  public static IEnumerable<IMoBroItem> MapToItems()
  {
    yield return CoreMetric("active_cryptocurrencies", CoreMetricType.Numeric);
    yield return CurrMetric("total_market_cap");
    yield return CurrMetric("total_volume");
    yield return CoreMetric("market_cap_percentage_btc", CoreMetricType.Usage);
    yield return CoreMetric("market_cap_percentage_eth", CoreMetricType.Usage);
  }

  public static IEnumerable<MetricValue> MapToMetricValues(this GlobalData gd, string currency)
  {
    yield return new MetricValue(MetricId("active_cryptocurrencies"), gd.GetLastUpdated(), gd.ActiveCryptocurrencies);
    yield return new MetricValue(MetricId("total_market_cap"), gd.GetLastUpdated(), gd.TotalMarketCap[currency]);
    yield return new MetricValue(MetricId("total_volume"), gd.GetLastUpdated(), gd.TotalVolume[currency]);
    yield return new MetricValue(MetricId("market_cap_percentage_btc"), gd.GetLastUpdated(), gd.MarketCapPercentage["btc"]);
    yield return new MetricValue(MetricId("market_cap_percentage_eth"), gd.GetLastUpdated(), gd.MarketCapPercentage["eth"]);
  }

  private static DateTime GetLastUpdated(this GlobalData gd)
  {
    return gd.UpdatedAt is null
      ? DateTime.UtcNow
      : DateTimeOffset.FromUnixTimeSeconds(gd.UpdatedAt.Value).UtcDateTime;
  }

  private static IMetric CurrMetric(string key) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(key))
      .WithLabel($"m_global_{key}_label", $"m_global_{key}_desc")
      .OfType(Ids.TypeCurrencyId)
      .OfCategory(Ids.CategoryGlobalId)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

  private static IMetric CoreMetric(string key, CoreMetricType type) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(key))
      .WithLabel($"m_global_{key}_label", $"m_global_{key}_desc")
      .OfType(type)
      .OfCategory(Ids.CategoryGlobalId)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

  private static string MetricId(string key) => $"m_global_{key}";
}