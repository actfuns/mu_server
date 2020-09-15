using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200034F RID: 847
	public class KuaFuMapManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		// Token: 0x06000E54 RID: 3668 RVA: 0x000E2CFC File Offset: 0x000E0EFC
		public static KuaFuMapManager getInstance()
		{
			return KuaFuMapManager.instance;
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x000E2D14 File Offset: 0x000E0F14
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x000E2D38 File Offset: 0x000E0F38
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuBossManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000E2D78 File Offset: 0x000E0F78
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1141, 2, 4, KuaFuMapManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1140, 1, 1, KuaFuMapManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x000E2DBC File Offset: 0x000E0FBC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x000E2DD0 File Offset: 0x000E0FD0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000E5A RID: 3674 RVA: 0x000E2DE4 File Offset: 0x000E0FE4
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000E5B RID: 3675 RVA: 0x000E2DF8 File Offset: 0x000E0FF8
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1140:
				result = this.ProcessGetKuaFuLineDataListCmd(client, nID, bytes, cmdParams);
				break;
			case 1141:
				result = this.ProcessKuaFuMapEnterCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x000E2E40 File Offset: 0x000E1040
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.LineMap2KuaFuLineDataDict.Clear();
					this.RuntimeData.ServerMap2KuaFuLineDataDict.Clear();
					this.RuntimeData.KuaFuMapServerIdDict.Clear();
					this.RuntimeData.MapCode2KuaFuLineDataDict.Clear();
					fileName = "Config/MapLine.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int mapMaxOnlineCount = (int)Global.GetSafeAttributeLong(node, "MaxNum");
						int mapType = (int)ConfigHelper.GetElementAttributeValueLong(node, "Type", 0L);
						string str = Global.GetSafeAttributeStr(node, "Line");
						if (!string.IsNullOrEmpty(str))
						{
							string[] mapLineStrs = str.Split(new char[]
							{
								'|'
							});
							foreach (string mapLineStr in mapLineStrs)
							{
								KuaFuLineData kuaFuLineData = new KuaFuLineData();
								string[] mapLineParams = mapLineStr.Split(new char[]
								{
									','
								});
								kuaFuLineData.Line = int.Parse(mapLineParams[0]);
								kuaFuLineData.MapCode = int.Parse(mapLineParams[1]);
								if (mapLineParams.Length >= 3)
								{
									kuaFuLineData.ServerId = int.Parse(mapLineParams[2]);
								}
								kuaFuLineData.MapType = mapType;
								kuaFuLineData.MaxOnlineCount = mapMaxOnlineCount;
								this.RuntimeData.LineMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.Line, kuaFuLineData.MapCode), kuaFuLineData);
								List<KuaFuLineData> list = null;
								if (kuaFuLineData.ServerId > 0)
								{
									if (this.RuntimeData.ServerMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.ServerId, kuaFuLineData.MapCode), kuaFuLineData))
									{
										if (!this.RuntimeData.KuaFuMapServerIdDict.TryGetValue(kuaFuLineData.ServerId, out list))
										{
											list = new List<KuaFuLineData>();
											this.RuntimeData.KuaFuMapServerIdDict.TryAdd(kuaFuLineData.ServerId, list);
										}
										list.Add(kuaFuLineData);
									}
								}
								if (!this.RuntimeData.MapCode2KuaFuLineDataDict.TryGetValue(kuaFuLineData.MapCode, out list))
								{
									list = new List<KuaFuLineData>();
									this.RuntimeData.MapCode2KuaFuLineDataDict.TryAdd(kuaFuLineData.MapCode, list);
								}
								list.Add(kuaFuLineData);
							}
						}
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x000E31A8 File Offset: 0x000E13A8
		public bool IsKuaFuMap(int mapCode)
		{
			return this.RuntimeData.MapCode2KuaFuLineDataDict.ContainsKey(mapCode);
		}

		// Token: 0x06000E5E RID: 3678 RVA: 0x000E31D8 File Offset: 0x000E13D8
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal || SceneUIClasses.Comp == sceneType || SceneUIClasses.ChongShengMap == sceneType;
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x000E3218 File Offset: 0x000E1418
		public bool ProcessGetKuaFuLineDataListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int mapCode = Global.SafeConvertToInt32(cmdParams[0]);
				if (Global.GetMapSceneType(mapCode) == SceneUIClasses.ChongShengMap)
				{
					List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
					client.sendCmd<List<KuaFuLineData>>(nID, list, false);
				}
				else
				{
					List<KuaFuLineData> list = YongZheZhanChangClient.getInstance().GetKuaFuLineDataList(mapCode) as List<KuaFuLineData>;
					client.sendCmd<List<KuaFuLineData>>(nID, list, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x000E32D4 File Offset: 0x000E14D4
		public bool ProcessKuaFuMapEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int toMapCode = Global.SafeConvertToInt32(cmdParams[0]);
				int line = Global.SafeConvertToInt32(cmdParams[1]);
				int toBoss = 0;
				int teleportId = 0;
				if (cmdParams.Length >= 3)
				{
					toBoss = Global.SafeConvertToInt32(cmdParams[2]);
				}
				if (cmdParams.Length >= 4)
				{
					teleportId = Global.SafeConvertToInt32(cmdParams[3]);
				}
				KuaFuLineData kuaFuLineData;
				if (!KuaFuMapManager.getInstance().IsKuaFuMap(toMapCode))
				{
					result = -12;
				}
				else if (!this.RuntimeData.LineMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(line, toMapCode), out kuaFuLineData))
				{
					result = -12;
				}
				else if (!Global.CanEnterMap(client, toMapCode) || (toMapCode == client.ClientData.MapCode && kuaFuLineData.MapType != 1))
				{
					result = -12;
				}
				else
				{
					if (toMapCode == client.ClientData.MapCode && kuaFuLineData.MapType == 1)
					{
						List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(toMapCode) as List<KuaFuLineData>;
						if (null == list)
						{
							result = -12;
							goto IL_67F;
						}
						KuaFuLineData currentLineData = list.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
						if (currentLineData != null && currentLineData.Line == kuaFuLineData.Line)
						{
							result = -4011;
							goto IL_67F;
						}
					}
					if (!KuaFuMapManager.getInstance().IsKuaFuMap(client.ClientData.MapCode) && !this.CheckMap(client))
					{
						result = -21;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						result = -12;
					}
					else if (kuaFuLineData.OnlineCount >= kuaFuLineData.MaxOnlineCount)
					{
						result = -100;
					}
					else
					{
						int fromMapCode = client.ClientData.MapCode;
						if (teleportId > 0)
						{
							GameMap fromGameMap = null;
							if (!GameManager.MapMgr.DictMaps.TryGetValue(fromMapCode, out fromGameMap))
							{
								result = -3;
								goto IL_67F;
							}
							MapTeleport mapTeleport = null;
							if (!fromGameMap.MapTeleportDict.TryGetValue(teleportId, out mapTeleport) || mapTeleport.ToMapID != toMapCode)
							{
								result = -12;
								goto IL_67F;
							}
							if (Global.GetTwoPointDistance(client.CurrentPos, new Point((double)mapTeleport.X, (double)mapTeleport.Y)) > 800.0)
							{
								result = -301;
								goto IL_67F;
							}
						}
						KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						int kuaFuServerId;
						if (kuaFuLineData.MapType == 1)
						{
							if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Reborn, true))
							{
								result = -400;
								goto IL_67F;
							}
							string signToken;
							string signKey;
							int rt = KuaFuWorldClient.getInstance().EnterPTKuaFuMap(client.ServerId, client.ClientData.LocalRoleID, client.ClientData.ServerPTID, kuaFuLineData.MapCode, kuaFuLineData.Line, kuaFuServerLoginData, out signToken, out signKey);
							if (rt == -4010)
							{
								KuaFuWorldRoleData kuaFuWorldRoleData = new KuaFuWorldRoleData
								{
									LocalRoleID = client.ClientData.LocalRoleID,
									UserID = client.strUserID,
									WorldRoleID = client.ClientData.WorldRoleID,
									Channel = client.ClientData.Channel,
									PTID = client.ClientData.ServerPTID,
									ServerID = client.ServerId,
									ZoneID = client.ClientData.ZoneID
								};
								rt = KuaFuWorldClient.getInstance().RegPTKuaFuRoleData(ref kuaFuWorldRoleData);
								rt = KuaFuWorldClient.getInstance().EnterPTKuaFuMap(client.ServerId, client.ClientData.LocalRoleID, client.ClientData.ServerPTID, kuaFuLineData.MapCode, kuaFuLineData.Line, kuaFuServerLoginData, out signToken, out signKey);
							}
							if (rt < 0)
							{
								result = rt;
								goto IL_67F;
							}
							KFRebornRoleData rebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
							if (null == rebornRoleData)
							{
								result = KuaFuWorldClient.getInstance().Reborn_RoleReborn(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, client.ClientData.RoleName, client.ClientData.RebornLevel);
								if (result < 0)
								{
									goto IL_67F;
								}
								LogManager.WriteLog(LogTypes.Analysis, string.Format("Reborn_RoleReborn ptId={0} roleId={1} roleName={2} rebornLevel={3}", new object[]
								{
									client.ClientData.ServerPTID,
									client.ClientData.LocalRoleID,
									client.ClientData.RoleName,
									client.ClientData.RebornLevel
								}), null, true);
							}
							kuaFuServerLoginData.PTID = client.ClientData.ServerPTID;
							kuaFuServerLoginData.RoleId = client.ClientData.LocalRoleID;
							kuaFuServerLoginData.SignToken = signToken;
							kuaFuServerLoginData.TempRoleID = rt;
							kuaFuServerLoginData.SignCode = MD5Helper.get_md5_string(kuaFuServerLoginData.SignDataString() + signKey).ToLower();
							kuaFuServerId = kuaFuServerLoginData.TargetServerID;
						}
						else
						{
							kuaFuServerLoginData.SignCode = null;
							kuaFuServerId = YongZheZhanChangClient.getInstance().EnterKuaFuMap(client.ClientData.LocalRoleID, kuaFuLineData.MapCode, kuaFuLineData.Line, client.ServerId, Global.GetClientKuaFuServerLoginData(client));
						}
						kuaFuServerLoginData.Line = line;
						if (kuaFuServerId > 0)
						{
							bool flag = 0 == 0;
							int needMoney = (teleportId > 0) ? 0 : Global.GetMapTransNeedMoney(toMapCode);
							if (Global.GetTotalBindTongQianAndTongQianVal(client) < needMoney)
							{
								GameManager.ClientMgr.NotifyImportantMsg(client, StringUtil.substitute(GLang.GetLang(171, new object[0]), new object[]
								{
									needMoney,
									Global.GetMapName(toMapCode)
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
								result = -9;
								Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							}
							else
							{
								int[] enterFlags = new int[5];
								enterFlags[0] = fromMapCode;
								enterFlags[1] = teleportId;
								enterFlags[2] = toBoss;
								Global.SaveRoleParamsIntListToDB(client, new List<int>(enterFlags), "EnterKuaFuMapFlag", true);
								GlobalNew.RecordSwitchKuaFuServerLog(client);
								client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
							}
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
							result = kuaFuServerId;
						}
					}
				}
				IL_67F:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x000E39B0 File Offset: 0x000E1BB0
		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			client.ClientData.MapCode = (int)kuaFuServerLoginData.GameId;
			client.ClientData.PosX = 0;
			client.ClientData.PosY = 0;
			List<int> enterFlags = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (enterFlags != null && enterFlags.Count >= 5)
			{
				int fromMapCode = enterFlags[0];
				int fromTeleport = enterFlags[1];
				int targetBossId = enterFlags[2];
				if (fromMapCode > 0 && fromTeleport > 0)
				{
					GameMap fromGameMap = null;
					MapTeleport mapTeleport = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(fromMapCode, out fromGameMap) && fromGameMap.MapTeleportDict.TryGetValue(fromTeleport, out mapTeleport))
					{
						GameMap toGameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(mapTeleport.ToMapID, out toGameMap) && toGameMap.CanMove(mapTeleport.ToX / toGameMap.MapGridWidth, mapTeleport.ToY / toGameMap.MapGridHeight))
						{
							client.ClientData.MapCode = mapTeleport.ToMapID;
							client.ClientData.PosX = mapTeleport.ToX;
							client.ClientData.PosY = mapTeleport.ToY;
						}
					}
				}
				if (targetBossId > 0)
				{
					Global.ProcessVipLevelUp(client);
					if (Global.IsVip(client) && (long)client.ClientData.VipLevel >= GameManager.systemParamsList.GetParamValueIntByName("VIPBossChuanSong", 4))
					{
						int bossX;
						int bossY;
						int radis;
						if (GameManager.MonsterZoneMgr.GetMonsterBirthPoint(client.ClientData.MapCode, targetBossId, out bossX, out bossY, out radis))
						{
							radis = 1;
							Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, bossX, bossY, radis);
							client.ClientData.PosX = (int)newPos.X;
							client.ClientData.PosY = (int)newPos.Y;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x000E3BD0 File Offset: 0x000E1DD0
		public void OnStartPlayGame(GameClient client)
		{
			bool bUserTeleport = false;
			List<int> enterFlags = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (enterFlags != null && enterFlags.Count >= 5 && enterFlags[1] > 0)
			{
				bUserTeleport = true;
			}
			if (!bUserTeleport)
			{
				KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				int mapCode = (int)kuaFuServerLoginData.GameId;
				int needMoney = Global.GetMapTransNeedMoney(mapCode);
				if (needMoney > 0 && Global.SubBindTongQianAndTongQian(client, needMoney, "地图传送"))
				{
					GameManager.ClientMgr.NotifyImportantMsg(client, StringUtil.substitute(GLang.GetLang(172, new object[0]), new object[]
					{
						needMoney,
						Global.GetMapName(mapCode)
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
			Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
			{
				enterFlags[0],
				0,
				0,
				0,
				0
			}), "EnterKuaFuMapFlag", true);
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x000E3CD4 File Offset: 0x000E1ED4
		public void TimerProc(object sender, EventArgs e)
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			if (KuaFuManager.getInstance().CanKuaFuLogin())
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (int mapCode in this.RuntimeData.MapCode2KuaFuLineDataDict.Keys)
					{
						dict[mapCode] = 0;
					}
				}
			}
			List<int> list = dict.Keys.ToList<int>();
			foreach (int mapCode in list)
			{
				dict[mapCode] = GameManager.ClientMgr.GetMapClientsCount(mapCode);
			}
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				KuaFuWorldClient.getInstance().UpdateKuaFuMapClientCount(dict);
			}
			else
			{
				YongZheZhanChangClient.getInstance().UpdateKuaFuMapClientCount(dict);
			}
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x000E3E28 File Offset: 0x000E2028
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KuaFuMap");
		}

		// Token: 0x04001659 RID: 5721
		public const SceneUIClasses ManagerType = SceneUIClasses.KuaFuMap;

		// Token: 0x0400165A RID: 5722
		private static KuaFuMapManager instance = new KuaFuMapManager();

		// Token: 0x0400165B RID: 5723
		public KuaFuMapData RuntimeData = new KuaFuMapData();
	}
}
