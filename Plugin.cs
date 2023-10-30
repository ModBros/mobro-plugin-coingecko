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
using MoBro.Plugin.SDK.Exceptions;
using MoBro.Plugin.SDK.Services;
using Newtonsoft.Json;

namespace MoBro.Plugin.CoinGecko;

public class Plugin : IMoBroPlugin
{
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
    _scheduler.Interval(UpdateMetrics, _updateFrequency, _updateFrequency);
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
      // coins as given by the settings
      var settingsCoins = coinsCsv.Split(",")
        .Select(s => s.Trim().ToLower());

      // all possible coin ids (symbol matching the settings)
      var coinIds = (await _coinsClient.GetCoinList())
        .Where(c => settingsCoins.Contains(c.Symbol.ToLower()))
        .Select(c => c.Id)
        .ToArray();

      // take the coin with the highest market cap for each symbol
      _coinIds = (await GetCoinMarkets(coinIds, _currency))
        .DistinctBy(cm => cm.Symbol)
        .Select(cm => cm.Id)
        .ToArray();
    }

    _logger.LogInformation("Configured coin ids: {CoinIds}", string.Join(",", _coinIds));
  }

  private void RegisterTypesAndCategories()
  {
    // register global category
    _service.Register(MoBroItem.CreateCategory()
      .WithId(Ids.CategoryGlobalId)
      .WithLabel($"{Ids.CategoryGlobalId}_label", $"{Ids.CategoryGlobalId}_desc")
      .Build());
  }

  private async Task RegisterMetrics()
  {
    // global metrics
    var global = await _globalClient.GetGlobal();
    _service.Register(global.Data.MapToItems(_currency));
    _service.UpdateMetricValues(global.Data.MapToMetricValues(_currency));

    // coin metrics
    foreach (var cm in await GetCoinMarkets(_coinIds, _currency))
    {
      _service.Register(cm.MapToItems(_currency));
      _service.UpdateMetricValues(cm.MapToMetricValues());
    }
  }

  private void UpdateMetrics()
  {
    // update global metrics
    var global = _globalClient.GetGlobal().GetAwaiter().GetResult();
    _service.UpdateMetricValues(global.Data.MapToMetricValues(_currency));

    // update coin metrics
    foreach (var cm in GetCoinMarkets(_coinIds, _currency).GetAwaiter().GetResult())
    {
      _service.UpdateMetricValues(cm.MapToMetricValues());
    }
  }

  private async Task<IEnumerable<CoinMarkets>> GetCoinMarkets(string[] coinIds, string currency)
  {
    if (coinIds.Length <= 0) return Enumerable.Empty<CoinMarkets>();

    try
    {
      return await _coinsClient.GetCoinMarkets(
        currency,
        coinIds,
        OrderField.MarketCapDesc,
        coinIds.Length,
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
}