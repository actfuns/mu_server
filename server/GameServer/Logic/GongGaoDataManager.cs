using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class GongGaoDataManager
	{
		
		public static void LoadGongGaoData()
		{
			string fullPathFileName = Global.IsolateResPath("Config/Gonggao.xml");
			GongGaoDataManager.strGongGaoXML = File.ReadAllText(fullPathFileName);
			GongGaoDataManager.systemGongGaoMgr.LoadFromXMlFile("Config/Gonggao.xml", "", "ID", 1);
		}

		
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

		
		public static SystemXmlItems systemGongGaoMgr = new SystemXmlItems();

		
		public static string strGongGaoXML = "";
	}
}
