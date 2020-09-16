using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class LuaManager
	{
		
		public string GetUserName(string s)
		{
			return string.Format("对象_{0}", s);
		}

		
		private string ConvertLuaString(string luaString)
		{
			return luaString;
		}

		
		public bool GotoMap(GameClient client, int toMapCode, int toPosX = -1, int toPosY = -1, int direction = -1)
		{
			bool ret = false;
			if (null != client)
			{
				if (client.ClientData.CurrentLifeV <= 0)
				{
					return ret;
				}
				int oldMapCode = client.ClientData.MapCode;
				if (JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode) == 2)
				{
					HuangChengManager.HandleLeaveMapHuangDiRoleChanging(client);
				}
				if (!Global.CanEnterIfMapIsGuMu(client, toMapCode))
				{
					return false;
				}
				if (toMapCode < 0)
				{
					GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toPosX, toPosY, direction);
				}
				else
				{
					bool execGoToMap = true;
					if (JunQiManager.GetLingDiIDBy2MapCode(toMapCode) > 0 && toMapCode != GameManager.MainMapCode)
					{
					}
					if (execGoToMap)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
						{
							ret = true;
							if (oldMapCode != toMapCode)
							{
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, direction, 0);
							}
							else
							{
								client.ClientData.CurrentAction = 0;
								GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toPosX, toPosY, direction);
							}
							Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 6, 1);
						}
					}
				}
			}
			return ret;
		}

		
		public bool AddDynamicMonsters(GameClient client, int monsterID, int addNum, int gridX, int gridY, int radius)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(client.ClientData.MapCode, monsterID, client.ClientData.CopyMapID, addNum, gridX, gridY, radius, 0, SceneUIClasses.Normal, null, null);
			return true;
		}

		
		public bool CallMonstersForGameClient(GameClient client, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1)
		{
			return GameManager.MonsterZoneMgr.CallDynamicMonstersOwnedByRole(client, monsterID, magicLevel, SurvivalTime, callAsType, callNum, 0);
		}

		
		public bool CallMonstersForMonster(Monster owner, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1)
		{
			return GameManager.MonsterZoneMgr.CallDynamicMonstersOwnedByMonster(owner, monsterID, magicLevel, SurvivalTime, callAsType, callNum, 0);
		}

		
		public string GetMapRecordXY(GameClient client, int recordIndex)
		{
			int mapCode = 0;
			int x = 0;
			int y = 0;
			string result;
			if (Global.GetMapRecordDataByField(client, recordIndex, out mapCode, out x, out y))
			{
				result = string.Format("{0},{1}", x, y);
			}
			else
			{
				result = "无";
			}
			return result;
		}

		
		public string GetMapRecordMapName(GameClient client, int recordIndex)
		{
			int mapCode = 0;
			int x = 0;
			int y = 0;
			string result;
			if (Global.GetMapRecordDataByField(client, recordIndex, out mapCode, out x, out y))
			{
				result = Global.GetMapName(mapCode);
			}
			else
			{
				result = "无";
			}
			return result;
		}

		
		public void RecordCurrentMapPosition(GameClient client, int recordIndex)
		{
			Global.ModifyMapRecordData(client, (ushort)client.CurrentMapCode, (ushort)client.CurrentGrid.X, (ushort)client.CurrentGrid.Y, recordIndex);
		}

		
		public void GotoMapRecordXY(GameClient client, int recordIndex)
		{
			int mapCode = 0;
			int gridX = 0;
			int gridY = 0;
			if (Global.GetMapRecordDataByField(client, recordIndex, out mapCode, out gridX, out gridY))
			{
				Point pixel = Global.GridToPixel(mapCode, (double)gridX, (double)gridY);
				this.GotoMap(client, mapCode, (int)pixel.X, (int)pixel.Y, -1);
			}
		}

		
		public int get_level(GameClient client)
		{
			return client.ClientData.Level;
		}

		
		public void AddUserGold(GameClient client, int gold)
		{
			GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, gold, "LUA脚本", client.strUserID);
		}

		
		public void SubUserGold(GameClient client, int gold)
		{
			GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, gold, "无", false);
		}

		
		public void AddUserMoney(GameClient client, int userMoney)
		{
			GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, userMoney, "lua接口", ActivityTypes.None, "");
		}

		
		public void SubUserMoney(GameClient client, int userMoney)
		{
			GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, userMoney, "lua接口", true, true, false, DaiBiSySType.None);
		}

		
		public void AddMoney1(GameClient client, int money1)
		{
			GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money1, "LUA脚本添加绑定金币", false);
		}

		
		public void SubMoney1(GameClient client, int money1)
		{
			GameManager.ClientMgr.SubMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money1, "LUA脚本扣除绑定金币");
		}

		
		public void AddYinLiang(GameClient client, int yinLiang)
		{
			GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "LUA脚本添加金币", false);
		}

		
		public void SubYinLiang(GameClient client, int yinLiang)
		{
			GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "LUA脚本扣除金币", false);
		}

		
		public void AddExp(GameClient client, int exp, bool enableFilter = false, bool writeToDB = false)
		{
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)exp, enableFilter, writeToDB, false, "none");
		}

		
		public void ToUseGoods(GameClient client, int goodsID, int goodsNum, bool usingGoods, out bool ret, out bool usingBinding, out bool usedTimeLimited)
		{
			usingBinding = false;
			usedTimeLimited = false;
			ret = GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsNum, usingGoods, out usingBinding, out usedTimeLimited, false);
		}

		
		public int GetGoodsNumByGoodsID(GameClient client, int goodsID)
		{
			return Global.GetTotalGoodsCountByID(client, goodsID);
		}

		
		public void AddNPCForClient(GameClient client, int npcID, int toX, int toY)
		{
			NPC npc = NPCGeneralManager.GetNPCFromConfig(client.ClientData.MapCode, npcID, toX, toY, 0);
			if (null != npc)
			{
				GameManager.ClientMgr.NotifyMySelfNewNPC(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npc);
			}
		}

		
		public void RemoveNPCForClient(GameClient client, int npcID)
		{
			GameManager.ClientMgr.NotifyMySelfDelNPC(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, npcID);
		}

		
		public void AddNpcToMap(int npcID, int mapCode, int toX, int toY)
		{
			NPC npc = NPCGeneralManager.GetNPCFromConfig(mapCode, npcID, toX, toY, 0);
			if (null != npc)
			{
				NPCGeneralManager.AddNpcToMap(npc);
			}
		}

		
		public void RemoveMapNpc(int mapCode, int npcID)
		{
			NPCGeneralManager.RemoveMapNpc(mapCode, npcID);
		}

		
		public void BroadcastMapRegionEvent(GameClient client, int areaLuaID, int type, int flag)
		{
			GlobalEventSource.getInstance().fireEvent(new ClientRegionEventObject(client, type, flag, areaLuaID));
		}

		
		public void NotifySelfDeco(GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks)
		{
			GameManager.ClientMgr.NotifySelfDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, decoID, decoType, toBody, toX, toY, shakeMap, toX1, toY1, moveTicks, alphaTicks);
		}

		
		public void NotifyOthersMyDeco(GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			GameManager.ClientMgr.NotifyOthersMyDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, decoID, decoType, toBody, toX, toY, shakeMap, toX1, toY1, moveTicks, alphaTicks, null);
		}

		
		public void NotifyAllImportantMsg(GameClient client, string msgText, int typeIndex, int showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, (GameInfoTypeIndexes)typeIndex, (ShowGameInfoTypes)showGameInfoType, errCode, 0, 0, 100, 100);
		}

		
		public void NotifyBangHuiImportantMsg(int faction, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyBangHuiImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, faction, msgText, typeIndex, showGameInfoType, errCode);
		}

		
		public void NotifyImportantMsg(GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, typeIndex, showGameInfoType, errCode);
		}

		
		public void Info(GameClient client, string infoText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(infoText))
			{
				infoText = this.ConvertLuaString(infoText);
				this.NotifyImportantMsg(client, infoText, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		
		public void Hot(GameClient client, string infoText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(infoText))
			{
				infoText = this.ConvertLuaString(infoText);
				this.NotifyImportantMsg(client, infoText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		
		public void Error(GameClient client, string warningText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(warningText))
			{
				warningText = this.ConvertLuaString(warningText);
				this.NotifyImportantMsg(client, warningText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		
		public void HandleTask(GameClient client, int npcID, int extensionID, int goodsID, int taskType)
		{
			ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npcID, extensionID, goodsID, (TaskTypes)taskType, null, 0, -1L, null);
		}

		
		public void SendGameEffect(GameClient client, string effectName, int lifeTicks, int alignMode = 0, string mp3Name = "")
		{
			GameManager.ClientMgr.SendGameEffect(client, effectName, lifeTicks, (GameEffectAlignModes)alignMode, mp3Name);
		}

		
		public void BroadCastGameEffect(int mapCode, int copyMapID, string effectName, int lifeTicks, int alignMode = 0, string mp3Name = "")
		{
			GameManager.ClientMgr.BroadCastGameEffect(mapCode, copyMapID, effectName, lifeTicks, (GameEffectAlignModes)alignMode, mp3Name);
		}

		
		public int GetRoleCommonParamsValue(GameClient client, int type)
		{
			int result;
			if (type == 0)
			{
				result = GameManager.ClientMgr.GetChengJiuPointsValue(client);
			}
			else if (type == 1)
			{
				result = GameManager.ClientMgr.GetZhuangBeiJiFenValue(client);
			}
			else if (type == 3)
			{
				result = GameManager.ClientMgr.GetWuXingValue(client);
			}
			else if (type == 4)
			{
				result = GameManager.ClientMgr.GetZhenQiValue(client);
			}
			else if (type == 5)
			{
				result = GameManager.ClientMgr.GetTianDiJingYuanValue(client);
			}
			else if (type == 27)
			{
				result = GameManager.ClientMgr.GetZaiZaoValue(client);
			}
			else if (type == 6)
			{
				result = GameManager.ClientMgr.GetShiLianLingValue(client);
			}
			else if (type == 7)
			{
				result = GameManager.ClientMgr.GetJingMaiLevelValue(client);
			}
			else if (type == 8)
			{
				result = GameManager.ClientMgr.GetWuXueLevelValue(client);
			}
			else if (type == 9)
			{
				result = GameManager.ClientMgr.GetZuanHuangLevelValue(client);
			}
			else if (type == 10)
			{
				result = GameManager.ClientMgr.GetSystemOpenValue(client);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public int ProcessNPCScript(GameClient client, int scriptID, int npcID)
		{
			return RunNPCScripts.ProcessNPCScript(client, scriptID, npcID);
		}

		
		public void GetNextCityBattleTimeAndBangHui(out bool result, out string sTime, out string sBangHui)
		{
			result = WangChengManager.GetNextCityBattleTimeAndBangHui(out sTime, out sBangHui);
		}

		
		public string GetCityBattleTimeAndBangHuiListString()
		{
			return LuoLanChengZhanManager.getInstance().GetCityBattleTimeAndBangHuiListString();
		}

		
		public int GetRoleLevel(GameClient client)
		{
			return client.ClientData.Level;
		}

		
		public string GetBossFuBenLeftTimeString(GameClient client)
		{
			int bossFuBenID = Global.FindBossFuBenIDByRoleLevel(client.ClientData.Level);
			string result;
			if (bossFuBenID > 0)
			{
				FuBenData fbData = Global.GetFuBenData(client, bossFuBenID);
				if (null != fbData)
				{
					int nFinishNum;
					result = string.Format("{0}", Math.Max(0, Global.GetBossFuBenCanFreeEnterNum(client) - Global.GetFuBenEnterNum(fbData, out nFinishNum)) + Global.GetBossFuBenCanExtEnterNum(client));
				}
				else
				{
					result = string.Format("{0}", Global.GetBossFuBenCanFreeEnterNum(client) + Global.GetBossFuBenCanExtEnterNum(client));
				}
			}
			else
			{
				result = "0";
			}
			return result;
		}

		
		public void EnterBossFuBen(GameClient client)
		{
			int ret = Global.EnterBossFuBen(client);
			if (-1 == ret)
			{
				this.Error(client, string.Format(GLang.GetLang(423, new object[0]), Global.GetBossFuBenMinLevel()), 0);
			}
			else if (-4 == ret)
			{
				this.Error(client, string.Format(GLang.GetLang(424, new object[0]), new object[0]), 0);
			}
			else if (ret < 0)
			{
				this.Error(client, string.Format(GLang.GetLang(425, new object[0]), ret), 0);
			}
		}

		
		public bool IsVip(GameClient client)
		{
			return Global.IsVip(client);
		}

		
		public int GetVipType(GameClient client)
		{
			return Global.GetVipType(client);
		}

		
		public string get_param(GameClient client, string paramName)
		{
			return Global.GetRoleParamByName(client, paramName);
		}

		
		public void set_param(GameClient client, string paramName, string paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue, writeToDB);
		}

		
		public void set_param(GameClient client, string paramName, int paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue.ToString(), writeToDB);
		}

		
		public void set_param(GameClient client, string paramName, double paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue.ToString(), writeToDB);
		}

		
		public int Today()
		{
			return TimeUtil.NowDateTime().DayOfYear;
		}

		
		public int GotoGuMuMap(GameClient client)
		{
			return Global.GotoGuMuMap(client);
		}

		
		public string GetErGuoTouBufferName(GameClient client)
		{
			BufferData bufferData = Global.GetBufferDataByID(client, 48);
			string result;
			if (null == bufferData)
			{
				result = GLang.GetLang(426, new object[0]);
			}
			else if (Global.IsBufferDataOver(bufferData, 0L))
			{
				result = GLang.GetLang(426, new object[0]);
			}
			else
			{
				long goodsID = (long)(0xffff_ffffUL & (ulong)(bufferData.BufferVal >> 32));
				result = Global.GetGoodsNameByID((int)goodsID);
			}
			return result;
		}

		
		public string GetErGuoTouBufferLeftTime(GameClient client)
		{
			BufferData bufferData = Global.GetBufferDataByID(client, 48);
			string result;
			if (null == bufferData)
			{
				result = GLang.GetLang(427, new object[0]);
			}
			else if (Global.IsBufferDataOver(bufferData, 0L))
			{
				result = GLang.GetLang(427, new object[0]);
			}
			else
			{
				result = StringUtil.substitute(GLang.GetLang(428, new object[0]), new object[]
				{
					bufferData.BufferSecs / 60,
					bufferData.BufferSecs % 60
				});
			}
			return result;
		}

		
		public string GetErGuoTouBufferExperience(GameClient client)
		{
			BufferData bufferData = Global.GetBufferDataByID(client, 48);
			string result;
			if (null == bufferData)
			{
				result = "0";
			}
			else if (Global.IsBufferDataOver(bufferData, 0L))
			{
				result = "0";
			}
			else
			{
				RoleSitExpItem roleSitExpItem = null;
				if (client.ClientData.Level < Data.RoleSitExpList.Length)
				{
					roleSitExpItem = Data.RoleSitExpList[client.ClientData.Level];
				}
				if (null != roleSitExpItem)
				{
					int experience = roleSitExpItem.Experience;
					double dblExperience = 1.0;
					if (SpecailTimeManager.JugeIsDoulbeKaoHuo())
					{
						dblExperience += 1.0;
					}
					dblExperience += Global.ProcessTeamZhuFuExperience(client);
					double multiExpNum = (double)(bufferData.BufferVal & (long) 0xffff_ffffUL) - 1.0;
					dblExperience += multiExpNum;
					result = ((int)((double)experience * dblExperience)).ToString();
				}
				else
				{
					result = "0";
				}
			}
			return result;
		}

		
		public string GetErGuoTouTodayLeftUseTimes(GameClient client)
		{
			return (6 - Global.GetErGuoTouTodayNum(client)).ToString();
		}

		
		public string GetStartBuChangTime(GameClient client)
		{
			return Global.GetTimeByBuChang(0, 0, 0, 0);
		}

		
		public string GetEndBuChangTime(GameClient client)
		{
			return Global.GetTimeByBuChang(1, 23, 59, 59);
		}

		
		public long GetBuChangExp(GameClient client)
		{
			return BuChangManager.GetBuChangExp(client);
		}

		
		public int GetBuChangBindYuanBao(GameClient client)
		{
			return BuChangManager.GetBuChangBindYuanBao(client);
		}

		
		public string GetBuChangGoodsNames(GameClient client)
		{
			List<GoodsData> goodsDataList = BuChangManager.GetBuChangGoodsDataList(client);
			string result;
			if (null == goodsDataList)
			{
				result = "";
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					if (i > 0)
					{
						sb.Append(" ");
					}
					sb.AppendFormat("{0}({1})", Global.GetGoodsNameByID(goodsDataList[i].GoodsID), goodsDataList[i].GCount);
				}
				result = sb.ToString();
			}
			return result;
		}

		
		public void GiveBuChang(GameClient client)
		{
			BuChangManager.GiveBuChang(client);
		}

		
		public void PlayBossAnimation(GameClient client, int monsterID, int mapCode, int toX, int toY, int effectX, int effectY)
		{
			GameManager.ClientMgr.NotifyPlayBossAnimation(client, monsterID, mapCode, toX, toY, effectX, effectY);
		}

		
		public void ExecSwitchServerScript(GameClient client, string script)
		{
		}
	}
}
