using System;
using MoBro.Plugin.CoinGecko;
using MoBro.Plugin.SDK;

// create and start the plugin to test it locally
var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .WithSetting(Ids.SettingUpdateFrequency, "1")
  .WithSetting(Ids.SettingCurrency, "usd")
  .WithSetting(Ids.SettingCoins, "btc,eth,grc")
  .Build();

Console.ReadLine();
