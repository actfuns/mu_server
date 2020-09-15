using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020004DE RID: 1246
	public class GongGaoDataManager
	{
		// Token: 0x06001732 RID: 5938 RVA: 0x0016B884 File Offset: 0x00169A84
		public static void LoadGongGaoData()
		{
			string fullPathFileName = Global.IsolateResPath("Config/Gonggao.xml");
			GongGaoDataManager.strGongGaoXML = File.ReadAllText(fullPathFileName);
			GongGaoDataManager.systemGongGaoMgr.LoadFromXMlFile("Config/Gonggao.xml", "", "ID", 1);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0016B8C4 File Offset: 0x00169AC4
		public static void CheckGongGaoInfo(GameClient client, int nID)
		{
			string strBeginTime = "";
			string strEndTime = "";
			using (Dictionary<int, SystemXmlItem>.ValueCollection.Enumerator enumerator = GongGaoDataManager.systemGongGaoMgr.SystemXmlItemDict.Values.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					SystemXmlItem systemMallItem = enumerator.Current;
					strBeginTime = systemMallItem.GetStringValue("FromDate");
					strEndTime = systemMallItem.GetStringValue("ToDate");
				}
			}
			int nHaveGongGao = 0;
			string strCurrDateTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			if (string.Compare(strCurrDateTime, strBeginTime) >= 0 && string.Compare(strCurrDateTime, strEndTime) <= 0)
			{
				nHaveGongGao = 1;
			}
			int nLianXuLoginReward = 0;
			int nLeiJiLoginReward = 0;
			if (client._IconStateMgr.CheckFuLiLianXuDengLuReward(client))
			{
				nLianXuLoginReward = 1;
			}
			if (client._IconStateMgr.CheckFuLiLeiJiDengLuReward(client))
			{
				nLeiJiLoginReward = 1;
			}
			GongGaoData gongGaoData = new GongGaoData();
			if (1 == nHaveGongGao)
			{
				gongGaoData.strGongGaoInfo = GongGaoDataManager.strGongGaoXML;
			}
			gongGaoData.nHaveGongGao = nHaveGongGao;
			gongGaoData.nLianXuLoginReward = nLianXuLoginReward;
			gongGaoData.nLeiJiLoginReward = nLeiJiLoginReward;
			client.sendCmd<GongGaoData>(nID, gongGaoData, false);
		}

		// Token: 0x04002108 RID: 8456
		public static SystemXmlItems systemGongGaoMgr = new SystemXmlItems();

		// Token: 0x04002109 RID: 8457
		public static string strGongGaoXML = "";
	}
}
