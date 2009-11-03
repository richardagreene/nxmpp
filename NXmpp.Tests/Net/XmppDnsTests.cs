#region License

// Copyright 2009 Damian Hickey
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License. 

#endregion

using Common.Logging;
using Moq;
using NUnit.Framework;
using NXmpp.Net;
using System.Linq;

namespace NXmpp.Tests.Net
{
	[TestFixture]
	public class XmppDnsTests
	{
		//These tests require a working internet connection, because of XmppDns dependency on System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()

		private ILog _logger;

		[SetUp]
		public void Setup()
		{
			_logger = new Mock<ILog>(MockBehavior.Loose).Object;
		}

		[Test]
		public void When_srv_query_returns_response_should_return_XmppHosts() //test requires internet connection.
		{
			const string domain = "gmail.com";
			var xmppDns = new XmppDns(_logger);
			XmppHost[] xmppHosts = xmppDns.GetXmppHosts(domain).ToArray();
			Assert.NotNull(xmppHosts);
			Assert.True(xmppHosts.Length > 1);
			Assert.AreEqual(domain, xmppHosts[xmppHosts.Length - 1].HostName);
		}

		[Test]
		public void When_query_returns_last_entry_should_be_domain()
		{
			const string domain = "domain.com";
			var xmppDns = new XmppDns(_logger);
			XmppHost[] xmppHosts = xmppDns.GetXmppHosts(domain).ToArray();
			Assert.NotNull(xmppHosts);
			Assert.True(xmppHosts.Length >= 1);
			Assert.AreEqual(domain, xmppHosts[xmppHosts.Length - 1].HostName);
		}
	}
}