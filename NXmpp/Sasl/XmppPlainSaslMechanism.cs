//#region Licence
//
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
//
//#endregion
//
//using System.Xml.Linq;
//using NXmpp.Core;
//
//namespace NXmpp.Sasl
//{
//	internal class XmppPlainSaslMechanism : PlainSaslMechanism
//	{
//		private readonly IXmppSyncConnection _connection;
//
//		internal XmppPlainSaslMechanism(IXmppSyncConnection connection)
//		{
//			_connection = connection;
//		}
//
//		public override void InitiateAndSendCredentials(string mechanism, string credentials)
//		{
//			var authElement = new XElement(XName.Get("auth", Namespaces.XmppSasl), new XAttribute(XName.Get("mechanism"), mechanism), credentials);
//			_connection.Write(authElement);
//		}
//	}
//}
