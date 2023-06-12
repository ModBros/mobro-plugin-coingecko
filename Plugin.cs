using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Parameters;
using Microsoft.Extensions.Logging;
using MoBro.Plugin.CoinGecko.Extensions;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Exceptions;
using MoBro.Plugin.SDK.Services;
using Newtonsoft.Json;

namespace MoBro.Plugin.CoinGecko;

public class Plugin : IMoBroPlugin
{
  private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(5);

  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;
  private readonly IMoBroSettings _settings;
  private readonly ILogger _logger;

  private readonly CoinsClient _coinsClient;

  private readonly TimeSpan _updateFrequency;
  private readonly string _currency;

  private string?[] _coinIds = Array.Empty<string?>();

  public Plugin(IMoBroService service, IMoBroScheduler scheduler, IMoBroSettings settings, ILogger logger)
  {
    _service = service;
    _scheduler = scheduler;
    _settings = settings;
    _logger = logger;

    _updateFrequency = TimeSpan.FromMinutes(_settings.GetValue<int>("s_update_frequency"));
    _currency = _settings.GetValue<string>("s_currency");

    // the CoinGecko API requires a user agent to be set
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(".NET/7.0 MoBroPlugin (+https://mobro.app)");
    _coinsClient = new CoinsClient(httpClient, new JsonSerializerSettings());
  }

  public async Task InitAsync()
  {
    // register all common items such as categories and metric types
    RegisterCommonItems();

    // register all coin related metrics
    await RegisterCoinMetrics();

    // schedule a recurring task to update the metrics
    _scheduler.Interval(UpdateMetrics, _updateFrequency, InitialDelay);
  }

  private void RegisterCommonItems()
  {
    // register currency metric type
    _service.Register(MoBroItem.CreateMetricType()
      .WithId(CoinMarketsExtensions.CurrencyTypeId)
      .WithLabel($"t_currency_{_currency}_label")
      .OfValueType(MetricValueType.Numeric)
      .WithBaseUnit(b => b
        .WithLabel($"t_currency_{_currency}_unit_label")
        .WithAbbreviation(GetCurrencySymbol())
        // .WithAbbreviation($"t_currency_{_currency}_unit_abbrev")
        .Build()
      ).Build());

    // register coins category 
    _service.Register(MoBroItem.CreateCategory()
      .WithId(CoinMarketsExtensions.CategoryTypeId)
      .WithLabel("c_coins_label", "c_coins_desc")
      .Build());
  }

  private async Task RegisterCoinMetrics()
  {
    var coinsCsv = _settings.GetValue<string>("s_coins", "");
    if (string.IsNullOrWhiteSpace(coinsCsv)) return;

    var supportedCoins = await _coinsClient.GetCoinList();
    var supportedCoinsDict = supportedCoins
      .DistinctBy(c => c.Symbol)
      .ToDictionary(c => c.Symbol, c => c.Id);

    _coinIds = coinsCsv.Split(",")
      .Select(s => s.Trim().ToLower())
      .Select(s => supportedCoinsDict.GetValueOrDefault(s))
      .Where(s => s != null)
      .ToArray();

    _logger.LogInformation("Set coin ids: {CoinIds}", string.Join(",", _coinIds));

    // register all coin metrics
    foreach (var cm in await GetCoinMarkets())
    {
      _service.Register(cm.MapToItems());
    }
  }

  private void UpdateMetrics()
  {
    // update coin metrics
    foreach (var cm in GetCoinMarkets().GetAwaiter().GetResult())
    {
      _service.UpdateMetricValues(cm.MapToMetricValues());
    }
  }

  private async Task<IEnumerable<CoinMarkets>> GetCoinMarkets()
  {
    if (_coinIds.Length <= 0) return Enumerable.Empty<CoinMarkets>();

    try
    {
      return await _coinsClient.GetCoinMarkets(
        _currency,
        _coinIds,
        OrderField.MarketCapDesc,
        _coinIds.Length,
        1,
        false,
        null,
        null
      );
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Failed to fetch data from CoinGecko API");
      throw new PluginDependencyException("Failed to fetch data from CoinGecko: " + e.Message, e);
    }
  }

  // temporary workaround for localization bug in service
  private string GetCurrencySymbol() => _currency switch
  {
    "btc" => "BTC",
    "eth" => "ETH",
    "eur" => "€",
    "usd" => "$",
    "aud" => "A$",
    "gbp" => "£",
    "chf" => "CHF",
    "pln" => "zł",
    "cad" => "C$",
    "huf" => "Ft",
    "nok" => "kr",
    "sek" => "kr",
    "czk" => "Kč",
    "uah" => "₴",
    _ => throw new ArgumentOutOfRangeException()
  };

  public void Dispose()
  {
    // nothing to dispose
  }
}