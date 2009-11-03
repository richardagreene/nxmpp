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

using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Common.Logging;
using DnDns.Query;
using DnDns.Records;
using DnDns.Enums;
using System;

namespace NXmpp.Net
{
	internal class XmppDns : IXmppDns
	{
		private readonly ILog _logger;

		internal XmppDns(ILog logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Gets a array of XmppHosts for a given domain ordered by weight. First by looking up the srv records for the domain, then adding the domain as the host for fall back.
		/// Does not attempt to resolve IP addresses.
		/// </summary>
		/// <param name="domain"></param>
		/// <returns>An array of xmpp hosts.</returns>
		public IEnumerable<XmppHost> GetXmppHosts(string domain)
		{
			var nameServers = new List<IPAddress>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					nameServers.AddRange(networkInterface.GetIPProperties().DnsAddresses);
				}
			}
			if (nameServers.Count == 0)
			{
				return null;
			}

			var xmppSrvRecords = new List<XmppSrvRecord>();
			string srvRecord = "_xmpp-server._tcp." + domain;

			//loop through each dns server and attempt to resolve for xmpp server hosts. Loop stops when a dns server is reachable and returns records
			foreach (IPAddress nameServer in nameServers)
			{
				try
				{
					XmppSrvRecord[] xmppSrvRecordsLookupResult = GetXmppSrvRecords(nameServer, srvRecord);

					// "if the result of the SRV lookup is a single RR with a Target of ".", i.e. the root domain, the initiating 
					// entity MUST abort SRV processing but SHOULD attempt a fallback resolution as described below ( draft-ietf-xmpp-3920bis-03#section-4.2 )
					// TODO: test this. (need to seperate dependency on DnDns in GetXmppSrvRecords() below)
					if (xmppSrvRecordsLookupResult.Length == 1 && xmppSrvRecordsLookupResult[0].HostName == ".") break;
					xmppSrvRecords.AddRange(GetXmppSrvRecords(nameServer, srvRecord));
				}
				catch (SocketException ex)
				{
					_logger.Error("DNS server unreachable. " + ex);
					continue;
				}
				if (xmppSrvRecords.Count > 0) break;
			}
			xmppSrvRecords.Sort();
			List<XmppHost> xmppHosts = xmppSrvRecords.ConvertAll(XmppSrvRecordToXmppHostConverter);
			xmppHosts.Add(new XmppHost(domain, 5222)); //fallback host.
			return xmppHosts.ToArray();
		}

		private static XmppSrvRecord[] GetXmppSrvRecords(IPAddress nameServer, string host)
		{
			//TODO: cache DNS lookups according to srvRecord.DnsHeader.TimeToLive

			var request = new DnsQueryRequest();
			DnsQueryResponse response = request.Resolve(nameServer.ToString(), host, NsType.SRV, NsClass.INET, ProtocolType.Udp);

			var xmppSrvRecords = new List<XmppSrvRecord>();
			foreach (IDnsRecord answer in response.Answers)
			{
				var srvRecord = (SrvRecord) answer;
				xmppSrvRecords.Add(new XmppSrvRecord { HostName = srvRecord.HostName, Port = srvRecord.Port, Weight = srvRecord.Weight});
			}
			return xmppSrvRecords.ToArray();
		}

		private static XmppHost XmppSrvRecordToXmppHostConverter(XmppSrvRecord xmppSrvRecord)
		{
			return new XmppHost(xmppSrvRecord.HostName, xmppSrvRecord.Port);
		}

		private class XmppSrvRecord : IComparable<XmppSrvRecord>
		{
			internal string HostName { get; set; }
			internal ushort Port { get; set; }
			internal ushort Weight { private get; set; }

			#region IComparable<XmppSrvRecord> Members

			public int CompareTo(XmppSrvRecord other)
			{
				return Weight.CompareTo(other.Weight);
			}

			#endregion
		}
	}
}