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

using NXmpp.Net;

namespace NXmpp
{
	public class Configuration
	{
		/// <summary>
		/// Specify the XmppHost to connection to. If null, xmpp hosts will be resolved using dns srv records and / or the jid name.
		/// </summary>
		public XmppHost XmppHost { get; set; }
	}
}
