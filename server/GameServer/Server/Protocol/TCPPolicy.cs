using System;
using System.IO;
using Server.Tools;

namespace Server.Protocol
{
	// Token: 0x0200086C RID: 2156
	internal class TCPPolicy
	{
		// Token: 0x06003CE4 RID: 15588 RVA: 0x003426F8 File Offset: 0x003408F8
		public static void LoadPolicyServerFile(string file)
		{
			TCPPolicy.PolicyServerFileContent = File.ReadAllBytes(file);
			byte[] bytesData = new byte[TCPPolicy.PolicyServerFileContent.Length + 1];
			DataHelper.CopyBytes(bytesData, 0, TCPPolicy.PolicyServerFileContent, 0, TCPPolicy.PolicyServerFileContent.Length);
			bytesData[bytesData.Length - 1] = 0;
			TCPPolicy.PolicyServerFileContent = bytesData;
		}

		// Token: 0x0400473B RID: 18235
		public const string POLICY_STRING = "<policy-file-request/>";

		// Token: 0x0400473C RID: 18236
		public static byte[] PolicyServerFileContent;
	}
}
