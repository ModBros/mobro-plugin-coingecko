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

  private const string HttpClientUserAgent = ".NET/7.0 MoBroPlugin (+https://mobro.app)";

  // injected by MoBro
  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;
  private readonly IMoBroSettings _settings;
  private readonly ILogger _logger;

  // CoinGecko API clients
  private readonly CoinsClient _coinsClient;
  private readonly GlobalClient _globalClient;

  // settings (set to default values)
  private TimeSpan _updateFrequency = TimeSpan.FromMinutes(10);
  private string _currency = "usd";
  private string[] _coinIds = Array.Empty<string>();

  public Plugin(IMoBroService service, IMoBroScheduler scheduler, IMoBroSettings settings, ILogger logger)
  {
    _service = service;
    _scheduler = scheduler;
    _settings = settings;
    _logger = logger;

    // the CoinGecko API requires a user agent to be set
    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(HttpClientUserAgent);
    _coinsClient = new CoinsClient(httpClient, new JsonSerializerSettings());
    _globalClient = new GlobalClient(httpClient, new JsonSerializerSettings());
  }

  public async Task InitAsync()
  {
    // parse settings
    await ParseSettings();

    // register all common items such as categories and metric types
    RegisterTypesAndCategories();

    // register all metrics
    await RegisterMetrics();

    // schedule a recurring task to update the metrics
    _scheduler.Interval(UpdateMetrics, _updateFrequency, InitialDelay);
  }

  private async Task ParseSettings()
  {
    // update frequency
    _updateFrequency = TimeSpan.FromMinutes(_settings.GetValue<int>(Ids.SettingUpdateFrequency));
    _logger.LogInformation("Configured update frequency: {UpdateFrequency}", _updateFrequency);

    // currency
    _currency = _settings.GetValue<string>(Ids.SettingCurrency);
    _logger.LogInformation("Configured currency: {Currency}", _currency);

    // coins
    var coinsCsv = _settings.GetValue<string>(Ids.SettingCoins, "");
    if (!string.IsNullOrWhiteSpace(coinsCsv))
    {
      var supportedCoins = (await _coinsClient.GetCoinList())
        .DistinctBy(c => c.Symbol)
        .ToDictionary(c => c.Symbol, c => c.Id);

      _coinIds = coinsCsv.Split(",")
        .Select(s => s.Trim().ToLower())
        .Select(s => supportedCoins.GetValueOrDefault(s))
        .Where(s => s is not null)
        .Select(s => s!)
        .ToArray();
    }

    _logger.LogInformation("Configured coin ids: {CoinIds}", string.Join(",", _coinIds));
  }

  private void RegisterTypesAndCategories()
  {
    // register currency metric type
    _service.Register(MoBroItem.CreateMetricType()
      .WithId(Ids.TypeCurrencyId)
      .WithLabel($"{Ids.TypeCurrencyId}_{_currency}_label")
      .OfValueType(MetricValueType.Numeric)
      .WithBaseUnit(b => b
        .WithLabel($"{Ids.TypeCurrencyId}_{_currency}_unit_label")
        .WithAbbreviation(GetCurrencySymbol())
        // .WithAbbreviation($"t_currency_{_currency}_unit_abbrev")
        .Build()
      ).Build());

    // register global category
    _service.Register(MoBroItem.CreateCategory()
      .WithId(Ids.CategoryGlobalId)
      .WithLabel($"{Ids.CategoryGlobalId}_label", $"{Ids.CategoryGlobalId}_desc")
      .Build());
  }

  private async Task RegisterMetrics()
  {
    // global metrics
    _service.Register(GlobalDataExtensions.MapToItems());

    // coin metrics
    foreach (var cm in await GetCoinMarkets())
    {
      _service.Register(cm.MapToItems());
    }
  }

  private void UpdateMetrics()
  {
    // update global metrics
    var global = _globalClient.GetGlobal().GetAwaiter().GetResult();
    _service.UpdateMetricValues(global.Data.MapToMetricValues(_currency));

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