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

using NUnit.Framework;
using Moq;
using NXmpp.Sasl;

namespace NXmpp.Tests.Sasl
{
	[TestFixture]
	public class DigestMD5MechanismTests
	{
		[Test]
		public void Veriy_digest_md5_mechanism_behaviour()  //better name for this?
		{
			var mockDigestMD5Mechanism = new Mock<DigestMD5Mechanism>();

			mockDigestMD5Mechanism.Setup(m => m.Initiate("DIGEST-MD5")).AtMostOnce();
			mockDigestMD5Mechanism.Setup(m => m.ReadInitialChallenge()).Returns("realm=\"elwood.innosoft.com\",nonce=\"OA6MG9tEQGm2hh\",qop=\"auth\",algorithm=md5-sess,charset=utf-8").AtMostOnce();
			mockDigestMD5Mechanism.Setup(m => m.SendInitialChallengeResponse(It.IsAny<string>())).AtMostOnce();
			mockDigestMD5Mechanism.Setup(m => m.ReadRspAuthChallenge()).Returns("rspauth=ea40f60335c427b5527b84dbabcdfffd").AtMostOnce();
			mockDigestMD5Mechanism.Setup(m => m.SendRspAuthChallengeResposne()).AtMostOnce();
			mockDigestMD5Mechanism.Object.Authenticate("username", "password", "realm", "hostname");

			mockDigestMD5Mechanism.VerifyAll();
		}
	}
}
