using System;
using System.Text;
using NUnit.Framework;

namespace NXmpp.Tests
{
	[TestFixture]
	public class JIdTests
	{
		[Test]
		public void When_domain_is_null_or_empty_creating_jid_should_throw()
		{
			//xmpp-3920, section 3: domain is the only required element of a Jid
			Assert.Throws<ArgumentNullException>(() => new JId(null));
			Assert.Throws<ArgumentNullException>(() => new JId(string.Empty));
			Assert.Throws<ArgumentNullException>(() => new JId(""));
		}

		[Test]
		public void When_localpart_is_null_or_empty_creating_jid_should_not_throw()
		{
			Assert.DoesNotThrow(() => new JId(null, "domain"));
			Assert.DoesNotThrow(() => new JId(string.Empty, "domain"));
		}

		[Test]
		public void When_resource_is_null_or_empty_creating_jid_should_not_throw()
		{
			Assert.DoesNotThrow(() => new JId(null, "domain", string.Empty));
			Assert.DoesNotThrow(() => new JId(null, "domain", string.Empty));
		}

		[Test]
		public void When_domain_supplied_should_be_same_as_domain_property()
		{
			const string domain = "domain";
			var jid = new JId(domain);
			Assert.AreEqual(domain, jid.Domain);
		}

		[Test]
		public void When_localpart_supplied_should_be_same_as_localpart_property()
		{
			const string localPart = "localpart";
			var jid = new JId(localPart, "domain");
			Assert.AreEqual(localPart, jid.LocalPart);
		}

		[Test]
		public void When_resource_supplied_should_be_same_as_resource_property()
		{
			const string resource = "resource";
			var jid = new JId(null, "domain", resource);
			Assert.AreEqual(resource, jid.Resource);
		}

		[Test]
		public void When_localpart_is_empty_string_property_should_return_null()
		{
			var jid = new JId(string.Empty, "domain");
			Assert.IsNull(jid.LocalPart);
		}

		[Test]
		public void When_resource_is_empty_string_property_should_return_null()
		{
			var jid = new JId(string.Empty, "domain", string.Empty);
			Assert.IsNull(jid.Resource);
		}


		[Test]
		public void When_domain_exceeds_1023_bytes_in_length_create_jid_should_throw()
		{
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < 103; i++ )
			{
				stringBuilder.Append("1234567890");
			}
			Assert.Greater(Encoding.UTF8.GetByteCount(stringBuilder.ToString()), 1023);
			Assert.Throws<InvalidOperationException>(() => new JId(stringBuilder.ToString()));
		}

		[Test]
		public void When_localpart_exceeds_1023_bytes_in_length_create_jid_should_throw()
		{
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < 103; i++)
			{
				stringBuilder.Append("1234567890");
			}
			Assert.Greater(Encoding.UTF8.GetByteCount(stringBuilder.ToString()), 1023);
			Assert.Throws<InvalidOperationException>(() => new JId(stringBuilder.ToString(), "domain"));
		}

		[Test]
		public void When_resource_exceeds_1023_bytes_in_length_create_jid_should_throw()
		{
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < 103; i++)
			{
				stringBuilder.Append("1234567890");
			}
			Assert.Greater(Encoding.UTF8.GetByteCount(stringBuilder.ToString()), 1023);
			Assert.Throws<InvalidOperationException>(() => new JId(null, "domain", stringBuilder.ToString()));
		}

		[Test]
		public void When_jid_as_string_3071_bytes_in_length_parse_should_throw() // http://tools.ietf.org/html/draft-ietf-xmpp-3920bis-03#section-3
		{
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < 310; i++)
			{
				stringBuilder.Append("1234567890");
			}
			Assert.Greater(Encoding.UTF8.GetByteCount(stringBuilder.ToString()), 3071);
			Assert.Throws<InvalidOperationException>(() => JId.Parse(stringBuilder.ToString()));
		}

		[TestCase("domain", null, "domain", null)]
		[TestCase("domain/resource", null, "domain", "resource")]
		[TestCase("user@domain/resource", "user", "domain", "resource")]
		[TestCase("user@domain", "user", "domain", null)]
		public void Given_valid_jid_as_string_should_parse(string jidAsString, string localPart, string domain, string resource)
		{
			var jid = JId.Parse(jidAsString);
			Assert.AreEqual(jid.LocalPart, localPart);
			Assert.AreEqual(jid.Domain, domain);
			Assert.AreEqual(jid.Resource, resource);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("@domain")]
		[TestCase("user@")]
		[TestCase("domain/")]
		[TestCase("@domain/")]
		[TestCase("user/domain@resource")]
		[TestCase("user1@user2@domain/resource")]
		[TestCase("user@domain/resource1/resource2")]
		public void Given_malformed_jid_as_string_when_parse_should_throw(string jidAsString)
		{
			Assert.Throws<InvalidOperationException>( () => JId.Parse(jidAsString));
		}

		[Test]
		public void Given_valid_jid_as_string_when_tryparse_should_return_true_and_jid_parameter_is_set()
		{
			JId jid = null;
			bool parsed = false;
			Assert.DoesNotThrow( () => parsed = JId.TryParse("user@domain/resource", out jid));
			Assert.True(parsed);
			Assert.NotNull(jid);
		}

		[Test]
		public void Given_malformed_jid_as_string_when_tryparse_should_return_false_and_jid_parameter_is_null()
		{
			JId jid = null;
			bool parsed = false;
			Assert.DoesNotThrow(() => parsed = JId.TryParse("user/domain@resource", out jid));
			Assert.False(parsed);
			Assert.Null(jid);
		}
	}
}