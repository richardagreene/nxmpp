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

using System;
using NXmpp.Core;
using NXmpp.Net;
using Common.Logging;
using System.Collections.Generic;

namespace NXmpp.ConnectionStates
{
	internal class TcpBindingState : ConnectionStateBase
	{
		private readonly IXmppDns _xmppDns;

		internal TcpBindingState(IXmppContext context, IXmppDns xmppDns, Configuration configuration, ILog logger)
			: base(context, configuration, logger)
		{
			_xmppDns = xmppDns;
		}

		internal override void Handle()
		{
			// implementation of TCP Binding http://tools.ietf.org/html/draft-ietf-xmpp-3920bis-03#section-4

			if (Configuration.XmppHost != null)
			{
				//use hostname defined in configuration
			}
			else
			{
				//resolve server base on username domain
				IEnumerable<XmppHost> xmppHosts = _xmppDns.GetXmppHosts(Context.JId.Domain);
				foreach (XmppHost xmppHost in xmppHosts)
				{

				}
			}
		}
	}

	internal class TcpBindingException : ApplicationException
	{
		internal TcpBindingException(string message) : base(message) { }
	}
}