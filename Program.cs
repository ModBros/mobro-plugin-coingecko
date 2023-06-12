using System;
using MoBro.Plugin.SDK.Testing;
using MoBro.Plugin.CoinGecko;

// create and start the plugin to test it locally
var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .WithSetting("s_update_frequency", "1")
  .WithSetting("s_currency", "usd")
  .WithSetting("s_coins", "btc,eth,grc")
  .Build();

Console.ReadLine();
