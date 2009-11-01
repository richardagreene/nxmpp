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

using System;
using System.Xml.Linq;
using System.Text;

namespace NXmpp.Sasl
{
	internal class XmppDigestMD5SaslMechanism : DigestMD5Mechanism
	{
		private readonly IXmppSyncConnection _connection;

		internal XmppDigestMD5SaslMechanism(IXmppSyncConnection connection)
		{
			_connection = connection;
		}

		public override void Initiate(string mechansim)
		{
			var authElement = new XElement(XName.Get("auth", Namespaces.XmppSasl), new XAttribute(XName.Get("mechanism"), mechansim));
			_connection.Write(authElement);
		}

		public override string ReadInitialChallenge()
		{
			XElement element = _connection.Read();
			switch (element.Name.LocalName)
			{
				case "challenge":
					byte[] buffer = Convert.FromBase64String(element.Value);
					return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
				case "failure":
					throw new ApplicationException("failure");
				default:
					throw new ApplicationException("Unrecognized element");
			}
		}

		public override void SendInitialChallengeResponse(string response)
		{
			var encoding = new UTF8Encoding(false);
			byte[] buffer = encoding.GetBytes(response);
			string encodedResponse = Convert.ToBase64String(buffer);
			_connection.Write(new XElement(XName.Get("response", Namespaces.XmppSasl), encodedResponse));
		}

		public override string ReadRspAuthChallenge()
		{
			XElement element = _connection.Read();
			switch (element.Name.LocalName)
			{
				case "challenge":
					return element.Value;
				case "failure":
					throw new ApplicationException("failure");
				default:
					throw new ApplicationException("Unrecognized element");
			}
		}

		public override void SendRspAuthChallengeResposne()
		{
			_connection.Write(new XElement(XName.Get("response", Namespaces.XmppSasl)));
		}
	}
}