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

using System.Collections;
using System.Collections.Generic;
using NXmpp.Core;
using NXmpp.Net;
using Common.Logging;

namespace NXmpp.ConnectionStates
{
	/// <summary>
	/// A enumerator based state machine to handle the various stages of connection to an xmpp server
	/// </summary>
	internal class ConnectionStatesEnumerable : IEnumerable<ConnectionStateBase>
	{
		private readonly IXmppContext _context;
		private readonly Configuration _configuration;
		private readonly ILog _logger;

		internal ConnectionStatesEnumerable(IXmppContext context, Configuration configuration, ILog logger)
		{
			_context = context;
			_configuration = configuration;
			_logger = logger;
		}

		#region IEnumerable<ConnectionStateBase> Members

		public IEnumerator<ConnectionStateBase> GetEnumerator()
		{
			ConnectionStateBase connectionState = new TcpBindingState(_context, new XmppDns(_logger), _configuration, _logger);
			yield return connectionState;
			while (connectionState.NextState != null)
			{
				connectionState = connectionState.NextState;
				yield return connectionState;
			}
			yield return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}