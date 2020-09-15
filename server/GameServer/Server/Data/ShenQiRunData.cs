using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x0200040C RID: 1036
	public class ShenQiRunData
	{
		// Token: 0x04001B93 RID: 7059
		public object Mutex = new object();

		// Token: 0x04001B94 RID: 7060
		public Dictionary<int, ArtifactItem> ArtifactXmlDict = new Dictionary<int, ArtifactItem>();

		// Token: 0x04001B95 RID: 7061
		public List<ToughnessItem> ToughnessXmlList = new List<ToughnessItem>();

		// Token: 0x04001B96 RID: 7062
		public List<GodItem> GodXmlList = new List<GodItem>();
	}
}
