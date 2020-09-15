using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x02000376 RID: 886
	public class MonsterStaticInfoMgr
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x000EF7FC File Offset: 0x000ED9FC
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x000EF812 File Offset: 0x000EDA12
		public static XElement AllMonstersXml { get; private set; }

		// Token: 0x06000F3A RID: 3898 RVA: 0x000EF81C File Offset: 0x000EDA1C
		public static void Initialize()
		{
			if (MonsterStaticInfoMgr.AllMonstersXml == null)
			{
				XElement tmpXml = null;
				try
				{
					tmpXml = XElement.Load(Global.GameResPath("Config/Monsters.xml"));
				}
				catch (Exception ex)
				{
				}
				if (tmpXml == null)
				{
					throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", "Config / Monsters.xml"));
				}
				MonsterStaticInfoMgr.AllMonstersXml = tmpXml;
			}
			MonsterStaticInfoMgr.AllInfos.Clear();
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x000EF89C File Offset: 0x000EDA9C
		public static MonsterStaticInfo GetInfo(int MonsterID)
		{
			MonsterStaticInfo info = null;
			MonsterStaticInfo result;
			if (!MonsterStaticInfoMgr.AllInfos.TryGetValue(MonsterID, out info))
			{
				result = null;
			}
			else
			{
				result = info;
			}
			return result;
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x000EF8C7 File Offset: 0x000EDAC7
		public static void SetInfo(int code, MonsterStaticInfo info)
		{
			MonsterStaticInfoMgr.AllInfos[code] = info;
		}

		// Token: 0x04001769 RID: 5993
		private static Dictionary<int, MonsterStaticInfo> AllInfos = new Dictionary<int, MonsterStaticInfo>();
	}
}
