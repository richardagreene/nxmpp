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
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace NXmpp.Sasl
{
	public abstract class DigestMD5Mechanism
	{
		public void Authenticate(string username, string password, string realm, string hostname)
		{
			Initiate("DIGEST-MD5");
			string challenge = ReadInitialChallenge();
			var digestMD5Authentication = new DigestMD5Authentication(challenge);
			SendInitialChallengeResponse(digestMD5Authentication.GetDigestResponse(username, password, realm, hostname));
			string rspAuth =  ReadRspAuthChallenge(); //TODO: what to do with rspauth?
			SendRspAuthChallengeResposne();
		}

		public abstract void Initiate(string mechansim);

		public abstract string ReadInitialChallenge();

		public abstract void SendInitialChallengeResponse(string response);

		public abstract string ReadRspAuthChallenge();

		public abstract void SendRspAuthChallengeResposne();

		internal class DigestMD5Authentication
		{
			private readonly DigestMD5Challenge digestChallenge;
			private readonly Random random = new Random();

			public DigestMD5Authentication(string challenge)
			{
				digestChallenge = new DigestMD5Challenge(challenge);
			}

			public string[] Realms
			{
				get { return digestChallenge.Realms; }
			}

			public string GetDigestResponse(string username, string password, string realm, string hostName)
			{
				return GetDigestResponse(username, password, realm, hostName, GenerateNonce(), "xmpp");
			}

			//used for testing algorithm (need to be able specifiy nonce)
			internal string GetDigestResponse(string username, string password, string realm, string hostName, string cnonce, string srvType)
			{
				var responseString = new StringBuilder();
				responseString.Append("username=\"" + username + "\"");
				responseString.Append(",realm=\"" + realm + "\"");
				responseString.Append(",nonce=\"" + digestChallenge.Nonce + "\"");
				responseString.Append(",cnonce=\"" + cnonce + "\"");
				responseString.Append(",nc=00000001");
				responseString.Append(",qop=" + DigestMD5Challenge.SupportedQopType);
				string digestUri = srvType + "/" + hostName;
				responseString.Append(",digest-uri=\"" + digestUri + "\"");
				responseString.Append(",charset=" + digestChallenge.CharacterSet.WebName);

				//rfc2831 section 2.1.2.1 Response Value
				var cryptHandler = new MD5CryptoServiceProvider();
				string a1Hash =
					MD5HashAsHexString(cryptHandler,
									   MD5Hash(cryptHandler, username + ":" + realm + ":" + password)
										.Concat(Encoding.ASCII.GetBytes(":" + digestChallenge.Nonce + ":" + cnonce))
										.ToArray()
						);
				string a2Hash = MD5HashAsHexString(cryptHandler, "AUTHENTICATE:" + digestUri);
				string response = a1Hash + ":" + digestChallenge.Nonce + ":00000001:" + cnonce + ":auth:" + a2Hash;
				string responseHash = MD5HashAsHexString(cryptHandler, response);

				responseString.Append(",response=" + responseHash);
				return responseString.ToString();
			}

			private string GenerateNonce()
			{
				return random.Next(1234567, 9999999).ToString();
			}

			private static string MD5HashAsHexString(HashAlgorithm cryptHandler, string s)
			{
				return MD5HashAsHexString(cryptHandler, Encoding.ASCII.GetBytes(s));
			}

			// Hash an input and return the hash as a 32 character hexadecimal lowercase string.
			private static string MD5HashAsHexString(HashAlgorithm cryptHandler, byte[] data)
			{
				data = cryptHandler.ComputeHash(data);
				return BitConverter.ToString(data).Replace("-", "").ToLower();
			}

			private static byte[] MD5Hash(HashAlgorithm cryptHandler, string s)
			{
				return cryptHandler.ComputeHash(Encoding.ASCII.GetBytes(s));
			}
		}

		internal class DigestMD5Challenge
		{
			internal const string SupportedQopType = "auth";
			private static readonly string[] CipherOptionsTable = new[] { "des", "3des", "rc4", "rc4-40", "rc4-56" };

			public DigestMD5Challenge(string challenge)
			{
				string[] nameValuePairs = challenge.Split(',');
				var realms = new HashSet<string>();
				string nonce = null;
				var qopOptions = new HashSet<string>();
				bool? stale = null;
				int? maxBuf = null;
				Encoding charset = null;
				string algorithm = null;
				var cipherOptions = new HashSet<string>();

				foreach (string nameValuePair in nameValuePairs)
				{
					string[] nameValue = nameValuePair.Split('=');
					string directive = nameValue[0];
					string directiveValue = StripQuotes(nameValue[1]);
					if (string.IsNullOrEmpty(directiveValue))
					{
						throw new InvalidDataException("null/empty directive value detected for " + directive + " directive");
					}
					IsDirectiveRealm(directive, directiveValue, ref realms);
					IsDirectiveNonce(directive, directiveValue, ref nonce);
					IsDirectiveQopOptions(directive, directiveValue, ref qopOptions);
					IsDirectiveStale(directive, directiveValue, ref stale);
					IsDirectiveMaxBuf(directive, directiveValue, ref maxBuf);
					IsDirectiveCharset(directive, directiveValue, ref charset);
					IsDirectiveAlgorithm(directive, directiveValue, ref algorithm);
					IsDirectiveCipherOpts(directive, directiveValue, ref cipherOptions);
					//auth-param is ignored
				}
				if (string.IsNullOrEmpty(nonce))
				{
					throw new InvalidDataException("nonce not supplied");
				}
				if (stale.HasValue)
				{
					throw new InvalidDataException("stale should not be supplied on inital authentication");
				}
				if (string.IsNullOrEmpty(algorithm))
				{
					throw new InvalidDataException("algorithm not supplied");
				}
				if (qopOptions.Contains("auth-conf") && cipherOptions.Count == 0)
				{
					throw new InvalidDataException("at least one cipher-opts is required when AuthConf is supplied in qop-options");
				}
				Realms = realms.ToArray();
				Nonce = nonce;
				if(qopOptions.Count == 0) //if no qop supplied, add 'auth' as a default.
				{
					qopOptions.Add(SupportedQopType);
				}
				if (!qopOptions.Contains(SupportedQopType))
				{
					throw new InvalidDataException("no supported qop value");
				}
				QopOptions = qopOptions;
				Stale = stale;
				MaximumBufferSize = maxBuf.HasValue ? maxBuf.Value : 65536;
				CharacterSet = charset ?? Encoding.GetEncoding("ISO8859-1");
				Algorithm = algorithm;
				if (cipherOptions.Count > 0)
				{
					CipherOptions = cipherOptions;
				}
			}

			internal string[] Realms { get; private set; }
			internal string Nonce { get; private set; }
			internal IEnumerable<string> QopOptions { get; private set; }
			internal int MaximumBufferSize { get; private set; }
			internal bool? Stale { get; private set; }
			internal Encoding CharacterSet { get; private set; }
			internal string Algorithm { get; private set; }
			internal IEnumerable<string> CipherOptions { get; private set; }

			private static string StripQuotes(string s)
			{
				return string.IsNullOrEmpty(s) ? s : s.Replace("\"", "");
			}

			private static void IsDirectiveRealm(string directive, string directiveValue, ref HashSet<string> realms)
			{
				if (directive != "realm") return;
				if (!string.IsNullOrEmpty(directiveValue))
				{
					//don't want to add empty
					realms.Add(directiveValue);
				}
			}

			private static void IsDirectiveNonce(string directive, string directiveValue, ref string nonce)
			{
				if (directive != "nonce") return;
				if (!string.IsNullOrEmpty(nonce))
				{
					throw new InvalidDataException("Nonce directive provided more than once");
				}
				nonce = directiveValue;
			}

			private static void IsDirectiveQopOptions(string directive, string directiveValue, ref HashSet<string> qopOptions)
			{
				if (directive != "qop") return;
				qopOptions.Add(directiveValue);
			}

			private static void IsDirectiveStale(string directive, string directiveValue, ref bool? stale)
			{
				if (directive != "stale") return;
				if (stale.HasValue)
				{
					throw new InvalidDataException("stale supplied more than once");
				}
				bool staleValue;
				try
				{
					staleValue = bool.Parse(directiveValue);
				}
				catch (Exception ex)
				{
					throw new InvalidDataException("exception parsing maxbuf value", ex);
				}
				stale = staleValue;
			}

			private static void IsDirectiveMaxBuf(string directive, string directiveValue, ref int? maxBuf)
			{
				if (directive != "maxbuf") return;
				if (maxBuf.HasValue)
				{
					throw new InvalidDataException("maxbuf supplied more than once");
				}
				int maxBufValue;
				try
				{
					maxBufValue = int.Parse(directiveValue);
				}
				catch (Exception ex)
				{
					throw new InvalidDataException("exception parsing maxbuf value", ex);
				}
				maxBuf = maxBufValue;
			}

			private static void IsDirectiveCharset(string directive, string directiveValue, ref Encoding charset)
			{
				if (directive == "charset")
				{
					if (charset != null)
					{
						throw new InvalidDataException("charset supplied more than once");
					}
					try
					{
						charset = Encoding.GetEncoding(directiveValue);
					}
					catch (Exception ex)
					{
						throw new InvalidDataException("unknown charset", ex);
					}
				}
			}

			private static void IsDirectiveAlgorithm(string directive, string directiveValue, ref string algorithm)
			{
				if (directive != "algorithm") return;
				if (algorithm != null)
				{
					throw new InvalidDataException("algorithm supplied more than once");
				}
				algorithm = directiveValue;
			}

			private static void IsDirectiveCipherOpts(string directive, string directiveValue, ref HashSet<string> cipherOptionTypes)
			{
				if (directive != "cipher") return;
				if (!CipherOptionsTable.Contains(directiveValue))
				{
					throw new InvalidDataException("Unrecognized cipher");
				}
				cipherOptionTypes.Add(directiveValue);
			}
		}
	}
}
