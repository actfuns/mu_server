using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic.Copy;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	// Token: 0x020004C9 RID: 1225
	internal class LuoLanFaZhenCopySceneManager
	{
		// Token: 0x0600168F RID: 5775 RVA: 0x001600E4 File Offset: 0x0015E2E4
		public static int getAwardRate(int FuBenID, int FuBenSeqID)
		{
			int result;
			if (FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
			{
				result = 1;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(FuBenSeqID);
				if (null == fubenData)
				{
					result = 1;
				}
				else if (fubenData.SpecailBossLeftNum == 0)
				{
					result = LuoLanFaZhenCopySceneManager.SpecialAwardRate;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00160138 File Offset: 0x0015E338
		public static void initialize()
		{
			try
			{
				int[] nParams = GameManager.systemParamsList.GetParamValueIntArrayByName("LuoLanFaZhen", ',');
				if (nParams.Length != 5)
				{
					throw new Exception("systemParamsList.LuoLanFaZhen参数数量应该是5");
				}
				LuoLanFaZhenCopySceneManager.SpecialBossID = nParams[0];
				LuoLanFaZhenCopySceneManager.SpecialMapCode = nParams[1];
				LuoLanFaZhenCopySceneManager.SpecialAwardRate = nParams[2];
				LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID = nParams[3];
				LuoLanFaZhenCopySceneManager.SpecialTeleRate = nParams[4];
				SystemXmlItem systemFuBenItem = null;
				if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID, out systemFuBenItem) && systemFuBenItem != null)
				{
					LuoLanFaZhenCopySceneManager.FinalBossID = systemFuBenItem.GetIntValue("BossID", -1);
				}
				LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.LoadFromXMlFile("Config/LuoLanFaZhen.xml", "", "MapID", 0);
				List<int> listMapCode = LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.SystemXmlItemDict.Keys.ToList<int>();
				foreach (int mapcode in listMapCode)
				{
					SystemXmlItem systemFazhenMap = null;
					if (LuoLanFaZhenCopySceneManager.systemLuoLanFaZhen.SystemXmlItemDict.TryGetValue(mapcode, out systemFazhenMap) && systemFazhenMap != null)
					{
						SystemFazhenMapData sysMapData = new SystemFazhenMapData();
						sysMapData.MapCode = mapcode;
						int[] specailParams = systemFazhenMap.GetIntArrayValue("TeShuMapID", '|');
						if (specailParams != null && specailParams.Length >= 3)
						{
							sysMapData.SpecialDestMapCode = specailParams[0];
							sysMapData.SpecialDestX = specailParams[1];
							sysMapData.SpecialDestY = specailParams[2];
						}
						else
						{
							sysMapData.SpecialDestMapCode = -1;
							sysMapData.SpecialDestX = -1;
							sysMapData.SpecialDestY = -1;
						}
						int[] gateIds = systemFazhenMap.GetIntArrayValue("ChuanSongMenID", '|');
						string strDestMapTemp = systemFazhenMap.GetStringValue("MuDidiID");
						string[] strDestMapTemp2 = strDestMapTemp.Split(new char[]
						{
							'|'
						});
						if (gateIds.Length != strDestMapTemp2.Length)
						{
							throw new Exception("LuoLanFaZhen.xml传送门数量和目标地图数量不一致");
						}
						for (int i = 0; i < gateIds.Length; i++)
						{
							sysMapData.listGateID.Add(gateIds[i]);
						}
						for (int i = 0; i < strDestMapTemp2.Length; i++)
						{
							string[] strDestMapTemp3 = strDestMapTemp2[i].Split(new char[]
							{
								','
							});
							sysMapData.listDestMapCode.Add(Convert.ToInt32(strDestMapTemp3[0]));
						}
						LuoLanFaZhenCopySceneManager.m_AllMapGatesStaticData[mapcode] = sysMapData;
					}
				}
			}
			catch (Exception)
			{
				throw new Exception("罗兰法阵配置项出错");
			}
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x001603FC File Offset: 0x0015E5FC
		public static bool OnFubenOver(int FubenSeqID)
		{
			lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
			{
				LuoLanFaZhenCopySceneManager.AllFazhenFubenData.Remove(FubenSeqID);
			}
			return true;
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00160454 File Offset: 0x0015E654
		public static SingleLuoLanFaZhenFubenData GetFubenData(int FubenSeqID)
		{
			SingleLuoLanFaZhenFubenData result;
			if (FubenSeqID <= 0)
			{
				result = null;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = null;
				lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
				{
					if (!LuoLanFaZhenCopySceneManager.AllFazhenFubenData.TryGetValue(FubenSeqID, out fubenData))
					{
						return null;
					}
				}
				result = fubenData;
			}
			return result;
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x001604C4 File Offset: 0x0015E6C4
		public static bool IsLuoLanFaZhen(int FubenID)
		{
			return FubenID == LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID;
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x001604E0 File Offset: 0x0015E6E0
		public static bool IsLuoLanFaZhenMap(int mapcode)
		{
			return null != FuBenManager.FindMapCodeByFuBenID(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID, mapcode);
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x00160504 File Offset: 0x0015E704
		public static bool EnterFubenMapWhenLogin(GameClient client)
		{
			return null != LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0016052C File Offset: 0x0015E72C
		public static bool OnEnterFubenMap(GameClient client, int oldmapcode, bool isLogin)
		{
			return LuoLanFaZhenCopySceneManager.TryAddFubenData(client.ClientData.FuBenSeqID, client.ClientData.FuBenID, client.ClientData.CopyMapID, client.ClientData.MapCode);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x0016056F File Offset: 0x0015E76F
		public static void OnLeaveFubenMap(GameClient client, int toMapCode)
		{
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00160574 File Offset: 0x0015E774
		public static void CreateRandomGates(int MapCode, FazhenMapData MapData)
		{
			SystemFazhenMapData sysMapData = null;
			if (LuoLanFaZhenCopySceneManager.m_AllMapGatesStaticData.TryGetValue(MapCode, out sysMapData))
			{
				if (null != sysMapData)
				{
					List<int> randgates = new List<int>();
					foreach (int gateid in sysMapData.listGateID)
					{
						int index = Global.GetRandomNumber(0, randgates.Count + 1);
						randgates.Insert(index, gateid);
					}
					if (randgates.Count == sysMapData.listDestMapCode.Count)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(MapData.MapCode, out gameMap) && null != gameMap)
						{
							lock (MapData.Telegates)
							{
								for (int i = 0; i < randgates.Count; i++)
								{
									MapTeleport mapTeleport = null;
									if (gameMap.MapTeleportDict.TryGetValue(sysMapData.listGateID[i], out mapTeleport) && null != mapTeleport)
									{
										SingleFazhenTelegateData newGatedata = new SingleFazhenTelegateData();
										newGatedata.usedAlready = false;
										newGatedata.gateId = randgates[i];
										newGatedata.destMapCode = sysMapData.listDestMapCode[i];
										newGatedata.SpecialDestMapCode = sysMapData.SpecialDestMapCode;
										newGatedata.SpecialDestX = sysMapData.SpecialDestX;
										newGatedata.SpecialDestY = sysMapData.SpecialDestY;
										newGatedata.destX = mapTeleport.ToX;
										newGatedata.destY = mapTeleport.ToY;
										MapData.Telegates[newGatedata.gateId] = newGatedata;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00160788 File Offset: 0x0015E988
		protected static bool TryAddFubenData(int _FubenSeqID, int _FubenID, int _MapID, int _MapCode)
		{
			bool result;
			if (_FubenSeqID <= 0 || _FubenID <= 0 || _MapID <= 0 || _MapCode <= 0)
			{
				result = false;
			}
			else
			{
				FazhenMapData mapdata = null;
				SingleLuoLanFaZhenFubenData fubenData = null;
				lock (LuoLanFaZhenCopySceneManager.AllFazhenFubenData)
				{
					if (!LuoLanFaZhenCopySceneManager.AllFazhenFubenData.TryGetValue(_FubenSeqID, out fubenData) || null == fubenData)
					{
						fubenData = new SingleLuoLanFaZhenFubenData
						{
							FubenID = _FubenID,
							FubenSeqID = _FubenSeqID
						};
						mapdata = new FazhenMapData
						{
							CopyMapID = _MapID,
							MapCode = _MapCode
						};
						LuoLanFaZhenCopySceneManager.CreateRandomGates(_MapCode, mapdata);
						fubenData.MapDatas[_MapID] = mapdata;
						LuoLanFaZhenCopySceneManager.AllFazhenFubenData[_FubenSeqID] = fubenData;
					}
					else
					{
						lock (fubenData.MapDatas)
						{
							if (!fubenData.MapDatas.TryGetValue(_MapID, out mapdata) || null == mapdata)
							{
								mapdata = new FazhenMapData
								{
									CopyMapID = _MapID,
									MapCode = _MapCode
								};
								LuoLanFaZhenCopySceneManager.CreateRandomGates(_MapCode, mapdata);
								fubenData.MapDatas[_MapID] = mapdata;
							}
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x00160920 File Offset: 0x0015EB20
		public static TCPProcessCmdResults OnTeleport(GameClient client, int teleportID, TCPOutPacketPool pool, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			TCPProcessCmdResults result;
			if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID || client.ClientData.FuBenSeqID <= 0)
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					result = TCPProcessCmdResults.RESULT_FAILED;
				}
				else
				{
					FazhenMapData mapdata = null;
					SingleFazhenTelegateData teledata = null;
					lock (fubenData.MapDatas)
					{
						if (!fubenData.MapDatas.TryGetValue(client.ClientData.CopyMapID, out mapdata) || null == mapdata)
						{
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					if (mapdata.MapCode != client.ClientData.MapCode || mapdata.CopyMapID != client.ClientData.CopyMapID)
					{
						result = TCPProcessCmdResults.RESULT_FAILED;
					}
					else
					{
						lock (mapdata.Telegates)
						{
							if (!mapdata.Telegates.TryGetValue(teleportID, out teledata) || null == teledata)
							{
								return TCPProcessCmdResults.RESULT_FAILED;
							}
						}
						if (teledata.destMapCode <= 0)
						{
							result = TCPProcessCmdResults.RESULT_FAILED;
						}
						else
						{
							bool TeleToSpecial = false;
							if (teledata.SpecialDestMapCode > 0)
							{
								if (0 != fubenData.SpecailBossLeftNum)
								{
									int rand = Global.GetRandomNumber(0, 100);
									if (rand < LuoLanFaZhenCopySceneManager.SpecialTeleRate)
									{
										TeleToSpecial = true;
									}
								}
							}
							if (TeleToSpecial)
							{
								GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, teleportID, teledata.SpecialDestMapCode, teledata.SpecialDestX, teledata.SpecialDestY, client.ClientData.RoleDirection, 123);
							}
							else
							{
								bool NeedSend = false;
								lock (teledata)
								{
									if (!teledata.usedAlready)
									{
										teledata.usedAlready = true;
										NeedSend = true;
									}
								}
								if (NeedSend)
								{
									FazhenMapProtoData senddata = new FazhenMapProtoData();
									senddata.listTelegate = new List<FazhenTelegateProtoData>();
									senddata.SrcMapCode = mapdata.MapCode;
									FazhenTelegateProtoData gatedata_s = new FazhenTelegateProtoData();
									gatedata_s.gateId = teledata.gateId;
									gatedata_s.DestMapCode = teledata.destMapCode;
									senddata.listTelegate.Add(gatedata_s);
									LuoLanFaZhenCopySceneManager.BroadMapData<FazhenMapProtoData>(685, senddata, mapdata.MapCode, client.ClientData.FuBenSeqID);
								}
								GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, teleportID, teledata.destMapCode, teledata.destX, teledata.destY, client.ClientData.RoleDirection, 123);
							}
							result = TCPProcessCmdResults.RESULT_OK;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x00160C58 File Offset: 0x0015EE58
		public static bool OnKillMonster(GameClient client, Monster monster)
		{
			bool result;
			if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
			{
				result = false;
			}
			else
			{
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					result = false;
				}
				else
				{
					List<int> listMapCodes = null;
					bool bKillBoss = false;
					if (monster.MonsterInfo.ExtensionID == LuoLanFaZhenCopySceneManager.FinalBossID)
					{
						fubenData.FinalBossLeftNum = 0;
						bKillBoss = true;
					}
					else if (monster.MonsterInfo.ExtensionID == LuoLanFaZhenCopySceneManager.SpecialBossID)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
						if (null == fuBenInfoItem)
						{
							return false;
						}
						fubenData.SpecailBossLeftNum = 0;
						string msg = StringUtil.substitute(GLang.GetLang(98, new object[0]), new object[]
						{
							client.ClientData.RoleName
						});
						listMapCodes = SingletonTemplate<CopyTeamManager>.Instance().GetTeamCopyMapCodes(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID);
						if (null == listMapCodes)
						{
							return false;
						}
						foreach (int mapCode in listMapCodes)
						{
							LuoLanFaZhenCopySceneManager.BroadMapMessage(msg, mapCode, client.ClientData.FuBenSeqID);
						}
						bKillBoss = true;
					}
					if (bKillBoss)
					{
						if (null == listMapCodes)
						{
							listMapCodes = SingletonTemplate<CopyTeamManager>.Instance().GetTeamCopyMapCodes(LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID);
						}
						if (null == listMapCodes)
						{
							return false;
						}
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID,
							LuoLanFaZhenCopySceneManager.FinalBossID,
							fubenData.FinalBossLeftNum,
							LuoLanFaZhenCopySceneManager.SpecialBossID,
							fubenData.SpecailBossLeftNum
						});
						foreach (int mapCode in listMapCodes)
						{
							LuoLanFaZhenCopySceneManager.BroadMapData(760, cmdData, mapCode, client.ClientData.FuBenSeqID);
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x00160EDC File Offset: 0x0015F0DC
		public static TCPProcessCmdResults ProcessFazhenTeleportCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int MapCode = 0;
				if (!int.TryParse(fields[1], out MapCode))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessFazhenTeleportCMD, roleID={0}, MapCode={1}", roleID, fields[1]), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientData.MapCode != MapCode)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				FazhenMapData mapData = null;
				lock (fubenData.MapDatas)
				{
					fubenData.MapDatas.TryGetValue(client.ClientData.CopyMapID, out mapData);
					if (null == mapData)
					{
						return TCPProcessCmdResults.RESULT_OK;
					}
				}
				FazhenMapProtoData senddata = new FazhenMapProtoData();
				senddata.listTelegate = new List<FazhenTelegateProtoData>();
				senddata.SrcMapCode = mapData.MapCode;
				lock (mapData.Telegates)
				{
					List<int> listGateID = mapData.Telegates.Keys.ToList<int>();
					if (null != listGateID)
					{
						foreach (int gateid in listGateID)
						{
							SingleFazhenTelegateData gatedata = mapData.Telegates[gateid];
							if (null != gatedata)
							{
								FazhenTelegateProtoData gatedata_s = new FazhenTelegateProtoData();
								gatedata_s.gateId = gateid;
								if (gatedata.usedAlready || LuoLanFaZhenCopySceneManager.GM_OpenState == 1)
								{
									gatedata_s.DestMapCode = gatedata.destMapCode;
								}
								else
								{
									gatedata_s.DestMapCode = 0;
								}
								senddata.listTelegate.Add(gatedata_s);
							}
						}
					}
				}
				byte[] bytes = DataHelper.ObjectToBytes<FazhenMapProtoData>(senddata);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 685);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessFazhenTeleportCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x001612C8 File Offset: 0x0015F4C8
		public static TCPProcessCmdResults ProcessFazhenBossCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientData.FuBenID != LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				SingleLuoLanFaZhenFubenData fubenData = LuoLanFaZhenCopySceneManager.GetFubenData(client.ClientData.FuBenSeqID);
				if (null == fubenData)
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					LuoLanFaZhenCopySceneManager.LuoLanFaZhenFubenID,
					LuoLanFaZhenCopySceneManager.FinalBossID,
					fubenData.FinalBossLeftNum,
					LuoLanFaZhenCopySceneManager.SpecialBossID,
					fubenData.SpecailBossLeftNum
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 760);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessFazhenBossCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x001614E0 File Offset: 0x0015F6E0
		public static void BroadMapData<T>(int cmdID, T cmdData, int mapCode, int FuBenSeqID)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.FuBenSeqID == FuBenSeqID)
						{
							c.sendCmd<T>(cmdID, cmdData, false);
						}
					}
				}
			}
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x00161560 File Offset: 0x0015F760
		public static void BroadMapData(int cmdID, string cmdData, int mapCode, int FuBenSeqID)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.FuBenSeqID == FuBenSeqID)
						{
							c.sendCmd(cmdID, cmdData, false);
						}
					}
				}
			}
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x001615E0 File Offset: 0x0015F7E0
		public static void BroadMapMessage(string msg, int mapCode, int FuBenSeqID)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.FuBenSeqID == FuBenSeqID)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, msg, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
					}
				}
			}
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x00161679 File Offset: 0x0015F879
		public static void GM_SetOpenState(int openstate)
		{
			LuoLanFaZhenCopySceneManager.GM_OpenState = openstate;
		}

		// Token: 0x04002065 RID: 8293
		public static int GM_OpenState = 0;

		// Token: 0x04002066 RID: 8294
		public static SystemXmlItems systemLuoLanFaZhen = new SystemXmlItems();

		// Token: 0x04002067 RID: 8295
		private static Dictionary<int, SingleLuoLanFaZhenFubenData> AllFazhenFubenData = new Dictionary<int, SingleLuoLanFaZhenFubenData>();

		// Token: 0x04002068 RID: 8296
		private static Dictionary<int, SystemFazhenMapData> m_AllMapGatesStaticData = new Dictionary<int, SystemFazhenMapData>();

		// Token: 0x04002069 RID: 8297
		public static int LuoLanFaZhenFubenID = 4201;

		// Token: 0x0400206A RID: 8298
		public static int SpecialTeleRate = 10;

		// Token: 0x0400206B RID: 8299
		protected static int FinalBossID = 0;

		// Token: 0x0400206C RID: 8300
		protected static int SpecialBossID = 0;

		// Token: 0x0400206D RID: 8301
		protected static int SpecialMapCode = 0;

		// Token: 0x0400206E RID: 8302
		protected static int SpecialAwardRate = 0;
	}
}
