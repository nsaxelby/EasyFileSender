using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EFS.Global.Models;
using EFS.Global.Exceptions;
using EFS.Utilities.Discovery;
using EFS.Global.Enums;

namespace EFS.Utilities.Tests
{
    [TestClass()]
    public class DiscoveryToolsTests
    {
        [TestMethod()]
        public void SendDiscoveryPacketTest_SucceedParse()
        {
            // Normal
            Assert.AreEqual(DiscoveryUtils.GetBroadcastAddress("192.168.0.50"), "192.168.0.255");
            // High bounds
            Assert.AreEqual(DiscoveryUtils.GetBroadcastAddress("254.254.254.254"), "254.254.254.255");
            // Low bounds
            Assert.AreEqual(DiscoveryUtils.GetBroadcastAddress("1.1.1.1"), "1.1.1.255");
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid1()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryUtils.GetBroadcastAddress("192.168.0.256"));
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid2()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryUtils.GetBroadcastAddress("not valid ip"));
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid3()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryUtils.GetBroadcastAddress(""));
        }

        [TestMethod()]
        public void GetDiscoveryPacketStrTest_Valid()
        {
            ClientInfo inputParam = new ClientInfo() { ClientType = ClientTypeEnum.windows.ToString(), IpAddress = "192.168.0.2", Version = VersionNumberEnum.v1.ToString() };
            string expected = "efsip 192.168.0.2 windows v1";
            Assert.AreEqual(DiscoveryUtils.GetDiscoveryPacketStr(inputParam), expected);
        }

        [TestMethod()]
        public void GetClientInfoFromStringTest_Valid()
        {
            string input = "efsip 192.168.0.2 windows v1";
            ClientInfo resultObj = DiscoveryUtils.GetClientInfoFromString(input);
            Assert.AreEqual(resultObj.IpAddress, "192.168.0.2");
            Assert.AreEqual(resultObj.ClientType, "windows");
            Assert.AreEqual(resultObj.Version, "v1");
        }

        [TestMethod()]
        public void GetClientInfoFromStringTest_ExceptionInvalid()
        {
            string input = "efsip 192.168.0.2 windows space v1";
            Assert.ThrowsException<MalformedUDPBroadcastException>(() => DiscoveryUtils.GetClientInfoFromString(input));
        }
    }
}