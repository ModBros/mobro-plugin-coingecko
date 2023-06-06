using System;
using MoBro.Plugin.SDK.Testing;
using MoBro.Plugin.CoinGecko;

// create and start the plugin to test it locally
var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .Build();

Console.ReadLine();
