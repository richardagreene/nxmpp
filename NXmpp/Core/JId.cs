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
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NXmpp.Core
{
	public class JId {

		private string _localPart;
		private string _domain;
		private string _resource;

		internal JId(string domain) : this(null, domain, null) { }

		internal JId(string localPart, string domain) : this(localPart, domain, null) { }

		internal JId(string localPart, string domain, string resource) {
			if (string.IsNullOrEmpty(domain))
			{
				throw new ArgumentNullException("domain");
			}
			LocalPart = localPart;
			Domain = domain;
			Resource = resource;
		}

		public string LocalPart
		{
			get
			{
				return _localPart;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_localPart = null;
				}
				else if (Encoding.UTF8.GetByteCount(value) > 1023)
				{
					throw new InvalidOperationException("LocalPart byte length exceeds 1023");
				}
				else
				{
					_localPart = value;
				}
			}
		}

		public string Domain
		{
			get
			{
				return _domain;
			}
			set
			{
				if (Encoding.UTF8.GetByteCount(value) > 1023)
				{
					throw new InvalidOperationException("Domain byte length exceeds 1023");
				}
				_domain = value;
			}
		}

		public string Resource
		{
			get
			{
				return _resource;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_resource = null;
				}
				else if (!string.IsNullOrEmpty(value) && Encoding.UTF8.GetByteCount(value) > 1023)
				{
					throw new InvalidOperationException("Resource byte length exceed 1023");
				}
				else
				{
					_resource = value;
				}
			}
		}

		public static JId Parse(string jidAsString)
		{
			if(string.IsNullOrEmpty(jidAsString))
			{
				throw new InvalidOperationException("Malformed jidAsString: null or empty. At a minimum a domain is required");
			}
			if(Encoding.UTF8.GetByteCount(jidAsString) > 3071)
			{
				throw new InvalidOperationException("Malformed jidAsString: byte length exceeds 3071");
			}

			if(CountCharOccuranceInString(jidAsString, '@') > 1)
			{
				throw new InvalidOperationException("Malformed jidAsString: too many occurances of '@' character");
			}

			if (CountCharOccuranceInString(jidAsString, '/') > 1)
			{
				throw new InvalidOperationException("Malformed jidAsString: too many occurances of '/' character");
			}

			if (jidAsString.StartsWith("@")) //see http://tools.ietf.org/html/draft-ietf-xmpp-3920bis-03#section-3 3.3
			{
				throw new InvalidOperationException("Malformed jidAsString: localPart defined but cannont be zero bytes"); 
			}

			if (jidAsString.EndsWith("/")) //see http://tools.ietf.org/html/draft-ietf-xmpp-3920bis-03#section-3 3.4
			{
				throw new InvalidOperationException("Malformed jidAsString: resource defined but cannont be zero bytes");
			}

			if(jidAsString.Contains('@') && jidAsString.Contains('/') && jidAsString.IndexOf('@') > jidAsString.IndexOf('/'))
			{
				throw new InvalidOperationException("Malformed jidAsString: '@' must come before '/'");
			}

			string localPart = null;
			string resource = null;

			string[] parts = jidAsString.Split('@');
			if(parts.Length == 2)
			{
				localPart = parts[0];
				parts = parts[1].Split('/');
			}
			else
			{
				parts = parts[0].Split('/');
			}
			string domain = parts[0];
			if(parts.Length == 2)
			{
				resource = parts[1];	
			}
			if(string.IsNullOrEmpty(domain))
			{
				throw new InvalidOperationException("Malformed jidAsString: domain is required");
			}
			return new JId(localPart, domain, resource);
		}

		private static int CountCharOccuranceInString(IEnumerable<char> s, char c)
		{
			return (from character in s where character == c select character).Count();
		}

		public static bool TryParse(string jidAsString, out JId jid)
		{
			try
			{
				jid = Parse(jidAsString);
				return true;
			}
			catch
			{
				jid = null;
				return false;
			}
		}
	}
}