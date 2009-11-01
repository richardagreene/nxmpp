//using System.Text;
//using NXmpp.Extensions;
//
//namespace NXmpp.Sasl
//{
//	public abstract class PlainSaslMechanism
//	{
//		public void Authenticate(string username, string password)
//		{
//			var credentials = new StringBuilder();
//			credentials.Append((char)0);
//			credentials.Append(username);
//			credentials.Append((char)0);
//			credentials.Append(password);
//
//			InitiateAndSendCredentials("PLAIN", credentials.ToString().ToBase64String());
//		}
//
//		public abstract void InitiateAndSendCredentials(string mechansim, string credentials);
//	}
//}
