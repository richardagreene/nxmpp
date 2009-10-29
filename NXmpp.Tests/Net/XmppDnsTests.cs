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
		public void Get_hosts_should_call_dependency_factory_methods()
		{
			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsQueryRequest>(MockBehavior.Loose).Object);

			try
			{
				XmppDns.GetHosts(dnsQueryRequestFactoryMock.Object, "domain", _logger);
			}
			catch {}
			dnsQueryRequestFactoryMock.Verify(m => m.Create());
		}

		[Test]
		public void When_no_dns_servers_respond_should_return_domain_as_host()
		{

			var dnsQueryRequestFactoryMock = new Mock<IDnsQueryRequestFactory>();
			dnsQueryRequestFactoryMock.Setup(m => m.Create()).Returns(() => new Mock<IDnsQueryRequest>(MockBehavior.Loose).Object);

			const string domain = "domain.com";
			XmppHost[] xmppHosts = XmppDns.GetHosts(dnsQueryRequestFactoryMock.Object, domain, _logger);

			Assert.NotNull(xmppHosts);
			Assert.AreEqual(1, xmppHosts.Length);
			Assert.AreEqual(domain, xmppHosts[0].HostName);
		}

		[Test]
		public void When_srv_query_returns_response_should_return_XmppHosts() //test requires internet connection.
		{
			const string domain = "gmail.com";

			XmppHost[] xmppHosts = XmppDns.GetHosts(new DnDnsQueryRequestFactory(), domain, _logger);
			Assert.NotNull(xmppHosts);
			Assert.True(xmppHosts.Length > 1);
			Assert.AreEqual(domain, xmppHosts[xmppHosts.Length - 1].HostName);
		}
	}
}