#region Licence

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

using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using NXmpp.Extensions;
using NXmpp.Sasl;
using System;

namespace NXmpp.Tests.Sasl
{
	[TestFixture]
	public class XmppDigestMD5SaslMechanismTests
	{
		private Mock<IXmppSyncConnection> _mockXmppSyncConnection;
		private XmppDigestMD5SaslMechanism _xmppDigestMD5SaslMechanism;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_mockXmppSyncConnection = new Mock<IXmppSyncConnection>(MockBehavior.Loose);
			_xmppDigestMD5SaslMechanism = new XmppDigestMD5SaslMechanism(_mockXmppSyncConnection.Object);
			_mockXmppSyncConnection.Setup(m => m.Write(It.IsAny<XElement>()));
		}

		#endregion

		[Test]
		public void When_authenticate_should_write_auth_element()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.ReturnsInOrder(
					new XElement(XName.Get("challenge", Namespaces.XmppSasl), "realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8".ToBase64String()),
					new XElement(XName.Get("challenge", "rspauth=ea40f60335c427b5527b84dbabcdfffd".ToBase64String()))
				);
			_xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host");

			_mockXmppSyncConnection.Verify(m => m.Write(It.Is<XElement>(x => x.Name.LocalName == "auth" && x.Name.Namespace == Namespaces.XmppSasl)));
		}

		[Test]
		public void When_recieved_initial_challange_should_write_response_element()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.ReturnsInOrder(
					new XElement(XName.Get("challenge", Namespaces.XmppSasl), "realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8".ToBase64String()),
					new XElement(XName.Get("challenge", "rspauth=ea40f60335c427b5527b84dbabcdfffd".ToBase64String())));
			_xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host");

			_mockXmppSyncConnection.Verify(m => m.Write(It.Is<XElement>(x => x.Name.LocalName == "response" && x.Name.Namespace == Namespaces.XmppSasl)));
		}

		[Test]
		public void When_recieved_failure_after_auth_should_throw()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.Returns(new XElement(XName.Get("failure", Namespaces.XmppSasl), new XElement("incorrect-encoding")));

			Assert.Throws<ApplicationException>(() => _xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host"));
		}

		[Test]
		public void When_recieved_unknown_element_after_auth_should_throw()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.Returns(new XElement(XName.Get("unknownelement", Namespaces.XmppSasl)));

			Assert.Throws<ApplicationException>(() => _xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host"));
		}

		[Test]
		public void When_recieved_failure_after_response_should_throw()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.ReturnsInOrder(
					new XElement(XName.Get("challenge", Namespaces.XmppSasl), "realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8".ToBase64String()),
					new XElement(XName.Get("failure", Namespaces.XmppSasl), new XElement("temporary-auth-failure")));

			Assert.Throws<ApplicationException>(() => _xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host"));
		}

		[Test]
		public void When_recieved_unknown_element_after_response_should_throw()
		{
			_mockXmppSyncConnection
				.Setup(m => m.Read())
				.ReturnsInOrder(
					new XElement(XName.Get("challenge", Namespaces.XmppSasl), "realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8".ToBase64String()),
					new XElement("unknownelement"));

			Assert.Throws<ApplicationException>(() => _xmppDigestMD5SaslMechanism.Authenticate("username", "password", "realm", "host"));
		}
	}
}