using System;
using System.Collections.Generic;
using System.Xml.Linq;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting.IPStatistics
{
	// Token: 0x02000021 RID: 33
	public class IPStatisticsPersistence
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x0000C90F File Offset: 0x0000AB0F
		private IPStatisticsPersistence()
		{
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000C930 File Offset: 0x0000AB30
		public void LoadConfig()
		{
			try
			{
				List<StatisticsControl> MyIPControlList = new List<StatisticsControl>();
				string file = "Config\\IPStaristicsConfig.xml";
				XElement xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath(file, KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null != xml)
				{
					foreach (XElement xmlItem in xml.Elements())
					{
						MyIPControlList.Add(new StatisticsControl
						{
							ID = Convert.ToInt32(xmlItem.Attribute("ID").Value.ToString()),
							ParamType = Convert.ToInt32(xmlItem.Attribute("ParamType").Value.ToString()),
							ParamLimit = Convert.ToInt32(xmlItem.Attribute("ParamLimit").Value.ToString()),
							ComParamType = Convert.ToInt32(xmlItem.Attribute("ComParamType").Value.ToString()),
							ComParamLimit = Convert.ToDouble(xmlItem.Attribute("ComParamLimit").Value.ToString()),
							OperaType = Convert.ToInt32(xmlItem.Attribute("OperaType").Value.ToString()),
							OperaParam = Convert.ToInt32(xmlItem.Attribute("OperaParam").Value.ToString())
						});
					}
					file = "Config\\IPPassList.xml";
					xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath(file, KuaFuServerManager.ResourcePathTypes.GameRes));
					if (null != xml)
					{
						List<IPPassList> NewIPPassList = new List<IPPassList>();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							IPPassList value = new IPPassList();
							value.ID = Convert.ToInt32(xmlItem.Attribute("ID").Value.ToString());
							string MinIP = xmlItem.Attribute("MinIP").Value.ToString();
							value.MinIP = IpHelper.IpToInt(MinIP);
							string MaxIP = xmlItem.Attribute("MaxIP").Value.ToString();
							value.MaxIP = IpHelper.IpToInt(MaxIP);
							NewIPPassList.Add(value);
						}
						this._IPControlList = MyIPControlList;
						this._IPPassList = NewIPPassList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "IPStaristicsConfig.InitConfig failed!", ex, true);
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000CC48 File Offset: 0x0000AE48
		public bool isCanPassIP(long ipAsInt)
		{
			bool result;
			lock (this._IPPassList)
			{
				if (this._IPPassList == null || this._IPPassList.Count == 0)
				{
					result = false;
				}
				else
				{
					foreach (IPPassList data in this._IPPassList)
					{
						if (data.MinIP <= ipAsInt && data.MaxIP >= ipAsInt)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x040000C2 RID: 194
		public static readonly IPStatisticsPersistence Instance = new IPStatisticsPersistence();

		// Token: 0x040000C3 RID: 195
		public List<StatisticsControl> _IPControlList = new List<StatisticsControl>();

		// Token: 0x040000C4 RID: 196
		public List<IPPassList> _IPPassList = new List<IPPassList>();
	}
}
