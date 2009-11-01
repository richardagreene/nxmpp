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

using System.IO;
using System.Linq;
using NUnit.Framework;
using NXmpp.Sasl;

namespace NXmpp.Tests.Sasl {
	public class DigestMD5ChallengeTests {
		// realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";

		#region Nested type: DigestChallengeAlgorithmTests
		[TestFixture]
		public class DigestChallengeAlgorithmTests {
			[Test]
			public void When_algorithm_ommitted_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}

			[Test]
			public void When_algorithm_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual("md5-sess", digestChallenge.Algorithm);
			}

			[Test]
			public void When_two_algorithm_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeCharsetTests
		[TestFixture]
		public class DigestChallengeCharsetTests {
			[Test]
			public void When_charset_ommitted_should_default() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual("iso-8859-1", digestChallenge.CharacterSet.WebName);
			}

			[Test]
			public void When_charset_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual("utf-8", digestChallenge.CharacterSet.WebName);
			}

			[Test]
			public void When_two_charset_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeCipherOptsTests
		[TestFixture]
		public class DigestChallengeCipherOptsTests {
			[Test]
			public void When_cipheropts_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,cipher=\"3des\"";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.NotNull(digestChallenge.CipherOptions);
				Assert.AreEqual(1, digestChallenge.CipherOptions.Count());
				Assert.Contains("3des", digestChallenge.CipherOptions.ToList());
			}

			[Test]
			public void When_cipheroptsommitted_should_contian_none() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.Null(digestChallenge.CipherOptions);
			}

			[Test]
			public void When_unrecognized_cipher_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,cipher=\"3desXX\"";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeMaxBufTests
		[TestFixture]
		public class DigestChallengeMaxBufTests {
			[Test]
			public void When_maxbuf_ommitted_should_default_to_65536() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual(65536, digestChallenge.MaximumBufferSize);
			}

			[Test]
			public void When_maxbuf_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",maxbuf=10000,charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual(10000, digestChallenge.MaximumBufferSize);
			}

			[Test]
			public void When_two_maxbuf_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",maxbuf=10000,maxbuf=10001,charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeNonceTests
		[TestFixture]
		public class DigestChallengeNonceTests {
			[Test]
			public void When_nonce_omitted_ReadFromString_should_throw() {
				const string challange = "realm=\"somerealm\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}

			[Test]
			public void When_one_nonce_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual("somenonce", digestChallenge.Nonce);
			}

			[Test]
			public void When_two_nonces_supplied_ReadFromString_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeQopOptionsTests
		[TestFixture]
		public class DigestChallengeQopOptionsTests {
			[Test]
			public void When_no_qopoption_supplied_should_default_to_Auth() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.Contains("auth", digestChallenge.QopOptions.ToList());
			}

			[Test]
			public void When_qop_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",qop=\"auth-int\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.Contains("auth", digestChallenge.QopOptions.ToList());
				Assert.Contains("auth-int", digestChallenge.QopOptions.ToList());
			}

			[Test]
			public void When_unrecognised_qop_supplied_should_throw() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth-intX\",charset=utf-8,algorithm=md5-sess";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion

		#region Nested type: DigestChallengeRealmTests
		[TestFixture]
		public class DigestChallengeRealmTests {
			[Test]
			public void When_a_realm_is_supplied_should_contain_value() {
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.Contains("somerealm", digestChallenge.Realms.ToList());
			}

			[Test]
			public void When_a_realm_is_supplied_twice_should_contain_value_once() {
				const string challange = "realm=\"somerealm\",realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual(1, digestChallenge.Realms.Count());
			}

			[Test]
			public void When_realm_is_omitted_ReadFromString_should_not_throw() {
				const string challange = "nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				Assert.DoesNotThrow(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}

			[Test]
			public void When_two_realm_are_supplied_should_contain_both() {
				const string challange = "realm=\"somerealm1\",realm=\"somerealm2\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess";
				var digestChallenge = new DigestMD5Mechanism.DigestMD5Challenge(challange);
				Assert.AreEqual(2, digestChallenge.Realms.Count());
				Assert.Contains("somerealm1", digestChallenge.Realms.ToList());
				Assert.Contains("somerealm2", digestChallenge.Realms.ToList());
			}
		}
		#endregion

		#region Nested type: DigestChallengeStaleTests
		[TestFixture]
		public class DigestChallengeStaleTests {
			[Test]
			public void When_stale_supplied_should_throw() { //stale not supported
				const string challange = "realm=\"somerealm\",nonce=\"somenonce\",qop=\"auth\",charset=utf-8,algorithm=md5-sess,stale=\"true\"";
				Assert.Throws<InvalidDataException>(() => new DigestMD5Mechanism.DigestMD5Challenge(challange));
			}
		}

		#endregion
	}
}