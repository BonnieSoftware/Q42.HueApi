using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Q42.HueApi.Interfaces;

namespace Q42.HueApi.Tests
{
  [TestClass]
  public class BridgeDiscoveryTests
  {
    [TestMethod]
    public async Task TestHttpBridgeLocator()
    {
      IBridgeLocator locator = new HttpBridgeLocator();

      await TestBridgeLocatorWithTimeout(locator, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task TestSsdpBridgeLocator()
    {
      IBridgeLocator locator = new SsdpBridgeLocator();

      await TestBridgeLocatorWithTimeout(locator, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task TestLocalNetworkScanBridgeLocator()
    {
      IBridgeLocator locator = new LocalNetworkScanBridgeLocator();

      // The timeout here really depends on the network size, latency and the number of CPU
      // It takes roughly 20 seconds for a network of 254 IPs (/24) with an 8-core CPU
      await TestBridgeLocatorWithTimeout(locator, TimeSpan.FromSeconds(30));
    }

    private async Task TestBridgeLocatorWithTimeout(IBridgeLocator locator, TimeSpan timeout)
    {
      var startTime = DateTime.Now;
      var bridgeIPs = await locator.LocateBridgesAsync(timeout);

      var elapsed = DateTime.Now.Subtract(startTime);

      Assert.IsTrue(
        elapsed.Subtract(timeout) < TimeSpan.FromMilliseconds(1000),
        $"Must complete inside the timeout specified ±1s (took {elapsed.TotalMilliseconds}ms)");

      Assert.IsNotNull(bridgeIPs,
        "Must return list");

      Assert.IsTrue(bridgeIPs.Any(),
        "Must find bridges");
    }
  }
}
