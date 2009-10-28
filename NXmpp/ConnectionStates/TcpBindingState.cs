using System;
using NXmpp.Core;
using NXmpp.Dns;
using NXmpp.Net;

namespace NXmpp.ConnectionStates {
	internal class TcpBindingState : ConnectionStateBase 
	{

		private readonly IDnsServerLookupFactory _dnsServerLookupFactory;
		private readonly IDnsQueryRequestFactory _dnsQueryRequestFactory;


		internal TcpBindingState(IXmppContext context, IDnsServerLookupFactory dnsServerLookupFactory, IDnsQueryRequestFactory dnsQueryRequestFactory) : base(context)
		{
			_dnsServerLookupFactory = dnsServerLookupFactory;
			_dnsQueryRequestFactory = dnsQueryRequestFactory;
		}

		internal override void Handle() {
			// implementation of TCP Binding http://tools.ietf.org/html/draft-ietf-xmpp-3920bis-03#section-4
		}
	}

	internal class TcpBindingException : ApplicationException 
	{
		internal TcpBindingException(string message) : base(message){}
	}
}