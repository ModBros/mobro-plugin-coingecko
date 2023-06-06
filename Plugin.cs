using System;
using Microsoft.Extensions.Logging;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.CoinGecko;

public class Plugin : IMoBroPlugin
{
  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;
  private readonly IMoBroSettings _settings;
  private readonly ILogger _logger;

  private readonly Random _random;

  public Plugin(IMoBroService service, IMoBroScheduler scheduler, IMoBroSettings settings, ILogger logger)
  {
    // inject all needed services via the constructor
    _service = service; // allows the plugin to interact with the service 
    _scheduler = scheduler; // scheduler for recurring tasks such as updating metrics
    _settings = settings; // used to retrieve the settings values
    _logger = logger;

    _random = new Random();
  }

  public void Init()
  {
    // Init() will be automatically called once upon initialization of the plugin
    // There is also an asynchronous implementation available: InitAsync()

    // get the value of the setting for the update frequency
    var updateFrequency = TimeSpan.FromSeconds(_settings.GetValue<int>("update_frequency"));

    // register all metrics
    RegisterMetrics();

    // set the value for the static metrics
    SetStaticMetricValues();

    // schedule a recurring task to update the dynamic metrics
    _scheduler.Interval(UpdateDynamicMetrics, updateFrequency, updateFrequency);
  }

  private void RegisterMetrics()
  {
    // dynamic number metric (e.g. CPU temperature, load, etc..)
    _service.Register(MoBroItem
      .CreateMetric()
      .WithId("metric_1")
      .WithLabel("Number metric")
      .OfType(CoreMetricType.Numeric)
      .OfCategory(CoreCategory.Miscellaneous)
      .OfNoGroup()
      .Build()
    );
    // static text metric (e.g. CPU name)
    _service.Register(MoBroItem
      .CreateMetric()
      .WithId("metric_2")
      .WithLabel("Text metric")
      .OfType(CoreMetricType.Text)
      .OfCategory(CoreCategory.Miscellaneous)
      .OfNoGroup()
      .AsStaticValue()
      .Build()
    );
  }

  private void SetStaticMetricValues()
  {
    // since this value won't change over time, it's sufficient to set it once
    _service.UpdateMetricValue("metric_2", "Text value");
  }

  private void UpdateDynamicMetrics()
  {
    // update the changing dynamic metrics
    // normally we would retrieve the new value first by e.g. reading a sensor or calling an external API
    _service.UpdateMetricValue("metric_1", _random.Next());
  }

  public void Dispose()
  {
  }
}