using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class ShenQiRunData
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, ArtifactItem> ArtifactXmlDict = new Dictionary<int, ArtifactItem>();

		
		public List<ToughnessItem> ToughnessXmlList = new List<ToughnessItem>();

		
		public List<GodItem> GodXmlList = new List<GodItem>();
	}
}
