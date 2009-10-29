using System.Collections;
using System.Collections.Generic;
using NXmpp.Core;

namespace NXmpp.ConnectionStates {
	/// <summary>
	/// A enumerator based state machine to handle the various stages of connection to an xmpp server
	/// </summary>
	internal class ConnectionStatesEnumerable : IEnumerable<ConnectionStateBase> 
	{
		private readonly IXmppContext _context;

		internal ConnectionStatesEnumerable(IXmppContext context)
		{
			_context = context;
		}

		#region IEnumerable<ConnectionStateBase> Members

		public IEnumerator<ConnectionStateBase> GetEnumerator() {
			ConnectionStateBase connectionState = new TcpBindingState(_context);
			yield return connectionState;
			while (connectionState.NextState != null) {
				connectionState = connectionState.NextState;
				yield return connectionState;
			}
			yield return null;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}
}