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
	// Token: 0x020007ED RID: 2029
	public class LuaManager
	{
		// Token: 0x06003956 RID: 14678 RVA: 0x0030B39C File Offset: 0x0030959C
		public string GetUserName(string s)
		{
			return string.Format("对象_{0}", s);
		}

		// Token: 0x06003957 RID: 14679 RVA: 0x0030B3BC File Offset: 0x003095BC
		private string ConvertLuaString(string luaString)
		{
			return luaString;
		}

		// Token: 0x06003958 RID: 14680 RVA: 0x0030B3D0 File Offset: 0x003095D0
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

		// Token: 0x06003959 RID: 14681 RVA: 0x0030B574 File Offset: 0x00309774
		public bool AddDynamicMonsters(GameClient client, int monsterID, int addNum, int gridX, int gridY, int radius)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(client.ClientData.MapCode, monsterID, client.ClientData.CopyMapID, addNum, gridX, gridY, radius, 0, SceneUIClasses.Normal, null, null);
			return true;
		}

		// Token: 0x0600395A RID: 14682 RVA: 0x0030B5B4 File Offset: 0x003097B4
		public bool CallMonstersForGameClient(GameClient client, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1)
		{
			return GameManager.MonsterZoneMgr.CallDynamicMonstersOwnedByRole(client, monsterID, magicLevel, SurvivalTime, callAsType, callNum, 0);
		}

		// Token: 0x0600395B RID: 14683 RVA: 0x0030B5DC File Offset: 0x003097DC
		public bool CallMonstersForMonster(Monster owner, int monsterID, int magicLevel, int SurvivalTime, int callAsType = 1001, int callNum = 1)
		{
			return GameManager.MonsterZoneMgr.CallDynamicMonstersOwnedByMonster(owner, monsterID, magicLevel, SurvivalTime, callAsType, callNum, 0);
		}

		// Token: 0x0600395C RID: 14684 RVA: 0x0030B604 File Offset: 0x00309804
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

		// Token: 0x0600395D RID: 14685 RVA: 0x0030B654 File Offset: 0x00309854
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

		// Token: 0x0600395E RID: 14686 RVA: 0x0030B694 File Offset: 0x00309894
		public void RecordCurrentMapPosition(GameClient client, int recordIndex)
		{
			Global.ModifyMapRecordData(client, (ushort)client.CurrentMapCode, (ushort)client.CurrentGrid.X, (ushort)client.CurrentGrid.Y, recordIndex);
		}

		// Token: 0x0600395F RID: 14687 RVA: 0x0030B6D0 File Offset: 0x003098D0
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

		// Token: 0x06003960 RID: 14688 RVA: 0x0030B724 File Offset: 0x00309924
		public int get_level(GameClient client)
		{
			return client.ClientData.Level;
		}

		// Token: 0x06003961 RID: 14689 RVA: 0x0030B744 File Offset: 0x00309944
		public void AddUserGold(GameClient client, int gold)
		{
			GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, gold, "LUA脚本", client.strUserID);
		}

		// Token: 0x06003962 RID: 14690 RVA: 0x0030B792 File Offset: 0x00309992
		public void SubUserGold(GameClient client, int gold)
		{
			GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, gold, "无", false);
		}

		// Token: 0x06003963 RID: 14691 RVA: 0x0030B7C8 File Offset: 0x003099C8
		public void AddUserMoney(GameClient client, int userMoney)
		{
			GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, userMoney, "lua接口", ActivityTypes.None, "");
		}

		// Token: 0x06003964 RID: 14692 RVA: 0x0030B80C File Offset: 0x00309A0C
		public void SubUserMoney(GameClient client, int userMoney)
		{
			GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, userMoney, "lua接口", true, true, false, DaiBiSySType.None);
		}

		// Token: 0x06003965 RID: 14693 RVA: 0x0030B84E File Offset: 0x00309A4E
		public void AddMoney1(GameClient client, int money1)
		{
			GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money1, "LUA脚本添加绑定金币", false);
		}

		// Token: 0x06003966 RID: 14694 RVA: 0x0030B882 File Offset: 0x00309A82
		public void SubMoney1(GameClient client, int money1)
		{
			GameManager.ClientMgr.SubMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, money1, "LUA脚本扣除绑定金币");
		}

		// Token: 0x06003967 RID: 14695 RVA: 0x0030B8B5 File Offset: 0x00309AB5
		public void AddYinLiang(GameClient client, int yinLiang)
		{
			GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "LUA脚本添加金币", false);
		}

		// Token: 0x06003968 RID: 14696 RVA: 0x0030B8E9 File Offset: 0x00309AE9
		public void SubYinLiang(GameClient client, int yinLiang)
		{
			GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "LUA脚本扣除金币", false);
		}

		// Token: 0x06003969 RID: 14697 RVA: 0x0030B91D File Offset: 0x00309B1D
		public void AddExp(GameClient client, int exp, bool enableFilter = false, bool writeToDB = false)
		{
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)exp, enableFilter, writeToDB, false, "none");
		}

		// Token: 0x0600396A RID: 14698 RVA: 0x0030B938 File Offset: 0x00309B38
		public void ToUseGoods(GameClient client, int goodsID, int goodsNum, bool usingGoods, out bool ret, out bool usingBinding, out bool usedTimeLimited)
		{
			usingBinding = false;
			usedTimeLimited = false;
			ret = GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsNum, usingGoods, out usingBinding, out usedTimeLimited, false);
		}

		// Token: 0x0600396B RID: 14699 RVA: 0x0030B984 File Offset: 0x00309B84
		public int GetGoodsNumByGoodsID(GameClient client, int goodsID)
		{
			return Global.GetTotalGoodsCountByID(client, goodsID);
		}

		// Token: 0x0600396C RID: 14700 RVA: 0x0030B9A0 File Offset: 0x00309BA0
		public void AddNPCForClient(GameClient client, int npcID, int toX, int toY)
		{
			NPC npc = NPCGeneralManager.GetNPCFromConfig(client.ClientData.MapCode, npcID, toX, toY, 0);
			if (null != npc)
			{
				GameManager.ClientMgr.NotifyMySelfNewNPC(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npc);
			}
		}

		// Token: 0x0600396D RID: 14701 RVA: 0x0030B9EF File Offset: 0x00309BEF
		public void RemoveNPCForClient(GameClient client, int npcID)
		{
			GameManager.ClientMgr.NotifyMySelfDelNPC(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, npcID);
		}

		// Token: 0x0600396E RID: 14702 RVA: 0x0030BA20 File Offset: 0x00309C20
		public void AddNpcToMap(int npcID, int mapCode, int toX, int toY)
		{
			NPC npc = NPCGeneralManager.GetNPCFromConfig(mapCode, npcID, toX, toY, 0);
			if (null != npc)
			{
				NPCGeneralManager.AddNpcToMap(npc);
			}
		}

		// Token: 0x0600396F RID: 14703 RVA: 0x0030BA4B File Offset: 0x00309C4B
		public void RemoveMapNpc(int mapCode, int npcID)
		{
			NPCGeneralManager.RemoveMapNpc(mapCode, npcID);
		}

		// Token: 0x06003970 RID: 14704 RVA: 0x0030BA56 File Offset: 0x00309C56
		public void BroadcastMapRegionEvent(GameClient client, int areaLuaID, int type, int flag)
		{
			GlobalEventSource.getInstance().fireEvent(new ClientRegionEventObject(client, type, flag, areaLuaID));
		}

		// Token: 0x06003971 RID: 14705 RVA: 0x0030BA70 File Offset: 0x00309C70
		public void NotifySelfDeco(GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks)
		{
			GameManager.ClientMgr.NotifySelfDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, decoID, decoType, toBody, toX, toY, shakeMap, toX1, toY1, moveTicks, alphaTicks);
		}

		// Token: 0x06003972 RID: 14706 RVA: 0x0030BAB0 File Offset: 0x00309CB0
		public void NotifyOthersMyDeco(GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			GameManager.ClientMgr.NotifyOthersMyDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, decoID, decoType, toBody, toX, toY, shakeMap, toX1, toY1, moveTicks, alphaTicks, null);
		}

		// Token: 0x06003973 RID: 14707 RVA: 0x0030BAF4 File Offset: 0x00309CF4
		public void NotifyAllImportantMsg(GameClient client, string msgText, int typeIndex, int showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, (GameInfoTypeIndexes)typeIndex, (ShowGameInfoTypes)showGameInfoType, errCode, 0, 0, 100, 100);
		}

		// Token: 0x06003974 RID: 14708 RVA: 0x0030BB37 File Offset: 0x00309D37
		public void NotifyBangHuiImportantMsg(int faction, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyBangHuiImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, faction, msgText, typeIndex, showGameInfoType, errCode);
		}

		// Token: 0x06003975 RID: 14709 RVA: 0x0030BB69 File Offset: 0x00309D69
		public void NotifyImportantMsg(GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = this.ConvertLuaString(msgText);
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, typeIndex, showGameInfoType, errCode);
		}

		// Token: 0x06003976 RID: 14710 RVA: 0x0030BB9C File Offset: 0x00309D9C
		public void Info(GameClient client, string infoText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(infoText))
			{
				infoText = this.ConvertLuaString(infoText);
				this.NotifyImportantMsg(client, infoText, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		// Token: 0x06003977 RID: 14711 RVA: 0x0030BBD0 File Offset: 0x00309DD0
		public void Hot(GameClient client, string infoText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(infoText))
			{
				infoText = this.ConvertLuaString(infoText);
				this.NotifyImportantMsg(client, infoText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x0030BC04 File Offset: 0x00309E04
		public void Error(GameClient client, string warningText, int errCode = 0)
		{
			if (!string.IsNullOrEmpty(warningText))
			{
				warningText = this.ConvertLuaString(warningText);
				this.NotifyImportantMsg(client, warningText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, errCode);
			}
		}

		// Token: 0x06003979 RID: 14713 RVA: 0x0030BC38 File Offset: 0x00309E38
		public void HandleTask(GameClient client, int npcID, int extensionID, int goodsID, int taskType)
		{
			ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npcID, extensionID, goodsID, (TaskTypes)taskType, null, 0, -1L, null);
		}

		// Token: 0x0600397A RID: 14714 RVA: 0x0030BC6C File Offset: 0x00309E6C
		public void SendGameEffect(GameClient client, string effectName, int lifeTicks, int alignMode = 0, string mp3Name = "")
		{
			GameManager.ClientMgr.SendGameEffect(client, effectName, lifeTicks, (GameEffectAlignModes)alignMode, mp3Name);
		}

		// Token: 0x0600397B RID: 14715 RVA: 0x0030BC81 File Offset: 0x00309E81
		public void BroadCastGameEffect(int mapCode, int copyMapID, string effectName, int lifeTicks, int alignMode = 0, string mp3Name = "")
		{
			GameManager.ClientMgr.BroadCastGameEffect(mapCode, copyMapID, effectName, lifeTicks, (GameEffectAlignModes)alignMode, mp3Name);
		}

		// Token: 0x0600397C RID: 14716 RVA: 0x0030BC98 File Offset: 0x00309E98
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

		// Token: 0x0600397D RID: 14717 RVA: 0x0030BDE0 File Offset: 0x00309FE0
		public int ProcessNPCScript(GameClient client, int scriptID, int npcID)
		{
			return RunNPCScripts.ProcessNPCScript(client, scriptID, npcID);
		}

		// Token: 0x0600397E RID: 14718 RVA: 0x0030BDFA File Offset: 0x00309FFA
		public void GetNextCityBattleTimeAndBangHui(out bool result, out string sTime, out string sBangHui)
		{
			result = WangChengManager.GetNextCityBattleTimeAndBangHui(out sTime, out sBangHui);
		}

		// Token: 0x0600397F RID: 14719 RVA: 0x0030BE08 File Offset: 0x0030A008
		public string GetCityBattleTimeAndBangHuiListString()
		{
			return LuoLanChengZhanManager.getInstance().GetCityBattleTimeAndBangHuiListString();
		}

		// Token: 0x06003980 RID: 14720 RVA: 0x0030BE2C File Offset: 0x0030A02C
		public int GetRoleLevel(GameClient client)
		{
			return client.ClientData.Level;
		}

		// Token: 0x06003981 RID: 14721 RVA: 0x0030BE4C File Offset: 0x0030A04C
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

		// Token: 0x06003982 RID: 14722 RVA: 0x0030BEE4 File Offset: 0x0030A0E4
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

		// Token: 0x06003983 RID: 14723 RVA: 0x0030BF98 File Offset: 0x0030A198
		public bool IsVip(GameClient client)
		{
			return Global.IsVip(client);
		}

		// Token: 0x06003984 RID: 14724 RVA: 0x0030BFB0 File Offset: 0x0030A1B0
		public int GetVipType(GameClient client)
		{
			return Global.GetVipType(client);
		}

		// Token: 0x06003985 RID: 14725 RVA: 0x0030BFC8 File Offset: 0x0030A1C8
		public string get_param(GameClient client, string paramName)
		{
			return Global.GetRoleParamByName(client, paramName);
		}

		// Token: 0x06003986 RID: 14726 RVA: 0x0030BFE1 File Offset: 0x0030A1E1
		public void set_param(GameClient client, string paramName, string paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue, writeToDB);
		}

		// Token: 0x06003987 RID: 14727 RVA: 0x0030BFEF File Offset: 0x0030A1EF
		public void set_param(GameClient client, string paramName, int paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue.ToString(), writeToDB);
		}

		// Token: 0x06003988 RID: 14728 RVA: 0x0030C003 File Offset: 0x0030A203
		public void set_param(GameClient client, string paramName, double paramValue, bool writeToDB = false)
		{
			Global.UpdateRoleParamByName(client, paramName, paramValue.ToString(), writeToDB);
		}

		// Token: 0x06003989 RID: 14729 RVA: 0x0030C018 File Offset: 0x0030A218
		public int Today()
		{
			return TimeUtil.NowDateTime().DayOfYear;
		}

		// Token: 0x0600398A RID: 14730 RVA: 0x0030C038 File Offset: 0x0030A238
		public int GotoGuMuMap(GameClient client)
		{
			return Global.GotoGuMuMap(client);
		}

		// Token: 0x0600398B RID: 14731 RVA: 0x0030C050 File Offset: 0x0030A250
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

		// Token: 0x0600398C RID: 14732 RVA: 0x0030C0C4 File Offset: 0x0030A2C4
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

		// Token: 0x0600398D RID: 14733 RVA: 0x0030C160 File Offset: 0x0030A360
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

		// Token: 0x0600398E RID: 14734 RVA: 0x0030C25C File Offset: 0x0030A45C
		public string GetErGuoTouTodayLeftUseTimes(GameClient client)
		{
			return (6 - Global.GetErGuoTouTodayNum(client)).ToString();
		}

		// Token: 0x0600398F RID: 14735 RVA: 0x0030C280 File Offset: 0x0030A480
		public string GetStartBuChangTime(GameClient client)
		{
			return Global.GetTimeByBuChang(0, 0, 0, 0);
		}

		// Token: 0x06003990 RID: 14736 RVA: 0x0030C29C File Offset: 0x0030A49C
		public string GetEndBuChangTime(GameClient client)
		{
			return Global.GetTimeByBuChang(1, 23, 59, 59);
		}

		// Token: 0x06003991 RID: 14737 RVA: 0x0030C2BC File Offset: 0x0030A4BC
		public long GetBuChangExp(GameClient client)
		{
			return BuChangManager.GetBuChangExp(client);
		}

		// Token: 0x06003992 RID: 14738 RVA: 0x0030C2D4 File Offset: 0x0030A4D4
		public int GetBuChangBindYuanBao(GameClient client)
		{
			return BuChangManager.GetBuChangBindYuanBao(client);
		}

		// Token: 0x06003993 RID: 14739 RVA: 0x0030C2EC File Offset: 0x0030A4EC
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

		// Token: 0x06003994 RID: 14740 RVA: 0x0030C389 File Offset: 0x0030A589
		public void GiveBuChang(GameClient client)
		{
			BuChangManager.GiveBuChang(client);
		}

		// Token: 0x06003995 RID: 14741 RVA: 0x0030C393 File Offset: 0x0030A593
		public void PlayBossAnimation(GameClient client, int monsterID, int mapCode, int toX, int toY, int effectX, int effectY)
		{
			GameManager.ClientMgr.NotifyPlayBossAnimation(client, monsterID, mapCode, toX, toY, effectX, effectY);
		}

		// Token: 0x06003996 RID: 14742 RVA: 0x0030C3AC File Offset: 0x0030A5AC
		public void ExecSwitchServerScript(GameClient client, string script)
		{
		}
	}
}
