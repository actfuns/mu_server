using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Tmsk.Tools.Tools;

namespace KF.Hosting
{
	
	public static class ConfigData
	{
		
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

		
		public static object Mutex = new object();

		
		public static bool IsMasterServer = false;

		
		public static bool IsPublishServer = false;

		
		public static string ServiceUri = "net.tcp://127.0.0.1:4000/";

		
		public static int ServicePort = 4001;

		
		public static string ServiceHost = "0.0.0.0";

		
		public static List<Tuple<string>> ServiceDefineList = new List<Tuple<string>>();
	}
}
