using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EFS.Global.Models;
using EFS.Global.Exceptions;

namespace EFS.Utilities.Tests
{
    [TestClass()]
    public class DiscoveryToolsTests
    {
        [TestMethod()]
        public void SendDiscoveryPacketTest_SucceedParse()
        {
            // Normal
            Assert.AreEqual(DiscoveryTools.ObtainBroadcastAddress("192.168.0.50"), "192.168.0.255");
            // High bounds
            Assert.AreEqual(DiscoveryTools.ObtainBroadcastAddress("254.254.254.254"), "254.254.254.255");
            // Low bounds
            Assert.AreEqual(DiscoveryTools.ObtainBroadcastAddress("1.1.1.1"), "1.1.1.255");
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid1()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryTools.ObtainBroadcastAddress("192.168.0.256"));
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid2()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryTools.ObtainBroadcastAddress("not valid ip"));
        }

        [TestMethod()]
        public void SendDiscoveryPacketTest_ExceptionInvalid3()
        {
            Assert.ThrowsException<FormatException>(() => DiscoveryTools.ObtainBroadcastAddress(""));
        }

        [TestMethod()]
        public void GetDiscoveryPacketStrTest_Valid()
        {
            ClientInfo inputParam = new ClientInfo() { ClientType = "windows", IpAddress = "192.168.0.2", Version = "v1" };
            string expected = "efsip 192.168.0.2 windows v1";
            Assert.AreEqual(DiscoveryTools.GetDiscoveryPacketStr(inputParam), expected);
        }

        [TestMethod()]
        public void GetClientInfoFromStringTest_Valid()
        {
            string input = "efsip 192.168.0.2 windows v1";
            ClientInfo resultObj = DiscoveryTools.GetClientInfoFromString(input);
            Assert.AreEqual(resultObj.IpAddress, "192.168.0.2");
            Assert.AreEqual(resultObj.ClientType, "windows");
            Assert.AreEqual(resultObj.Version, "v1");
        }

        [TestMethod()]
        public void GetClientInfoFromStringTest_ExceptionInvalid()
        {
            string input = "efsip 192.168.0.2 windows space v1";
            Assert.ThrowsException<MalformedUDPBroadcastException>(() => DiscoveryTools.GetClientInfoFromString(input));
        }
    }
}