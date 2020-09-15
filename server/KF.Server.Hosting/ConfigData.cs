using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Tmsk.Tools.Tools;

namespace KF.Hosting
{
	// Token: 0x02000002 RID: 2
	public static class ConfigData
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static bool InitConfig()
		{
			lock (ConfigData.Mutex)
			{
				XElement xmlFile = ConfigHelper.Load(Process.GetCurrentProcess().MainModule.FileName + ".config");
				ConfigData.ServiceUri = ConfigHelper.GetElementAttributeValue(xmlFile, "add", "key", "ServiceUri", "value", ConfigData.ServiceUri);
				ConfigData.ServiceHost = ConfigHelper.GetElementAttributeValue(xmlFile, "add", "key", "ServiceHost", "value", ConfigData.ServiceHost);
				ConfigData.ServicePort = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "ServicePort", "value", (long)ConfigData.ServicePort);
				foreach (XElement item in ConfigHelper.GetXElements(xmlFile, "wellknown"))
				{
					string objectUri = ConfigHelper.GetElementAttributeValue(item, "objectUri", "");
					ConfigData.ServiceDefineList.Add(new Tuple<string>(objectUri));
				}
			}
			return true;
		}

		// Token: 0x04000001 RID: 1
		public static object Mutex = new object();

		// Token: 0x04000002 RID: 2
		public static bool IsMasterServer = false;

		// Token: 0x04000003 RID: 3
		public static bool IsPublishServer = false;

		// Token: 0x04000004 RID: 4
		public static string ServiceUri = "net.tcp://127.0.0.1:4000/";

		// Token: 0x04000005 RID: 5
		public static int ServicePort = 4001;

		// Token: 0x04000006 RID: 6
		public static string ServiceHost = "0.0.0.0";

		// Token: 0x04000007 RID: 7
		public static List<Tuple<string>> ServiceDefineList = new List<Tuple<string>>();
	}
}
