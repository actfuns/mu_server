using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class CopyTargetManager
	{
		
		public static void LoadConfig()
		{
			lock (CopyTargetManager._Mutex)
			{
				CopyTargetManager.CopyTargetInfoDict.Clear();
				string fileName = "Config/FuBenMuBiao.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null != xml)
				{
					IEnumerable<XElement> xmlItems = xml.Elements("FuBenMuBiao");
					foreach (XElement xmlItem in xmlItems)
					{
						IEnumerable<XElement> nodes = xmlItem.Elements("MuBiao");
						foreach (XElement node in nodes)
						{
							int fubenID = (int)Global.GetSafeAttributeLong(node, "FuBenID");
							int targetIdx = (int)Global.GetSafeAttributeLong(node, "MuBiaoID");
							string[] strFields = Global.GetSafeAttributeStr(node, "MuBiaoCanShu").Split(new char[]
							{
								'|'
							});
							foreach (string field in strFields)
							{
								CopyTargetInfo InfoData = new CopyTargetInfo();
								string[] strInfo = field.Split(new char[]
								{
									','
								});
								if (2 != strInfo.Length)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("Config/FuBenMuBiao.xml 解析失败 fubenID={0} targetID={1} field={2}", fubenID, targetIdx, field), null, true);
								}
								else
								{
									InfoData.MonsterID = Global.SafeConvertToInt32(strInfo[0]);
									InfoData.Count = Global.SafeConvertToInt32(strInfo[1]);
									if (!CopyTargetManager.CopyTargetInfoDict.ContainsKey(fubenID))
									{
										Dictionary<int, List<CopyTargetInfo>> dictTarget = new Dictionary<int, List<CopyTargetInfo>>();
										dictTarget.Add(targetIdx, new List<CopyTargetInfo>
										{
											InfoData
										});
										CopyTargetManager.CopyTargetInfoDict[fubenID] = dictTarget;
									}
									else if (!CopyTargetManager.CopyTargetInfoDict[fubenID].ContainsKey(targetIdx))
									{
										List<CopyTargetInfo> listTarget = new List<CopyTargetInfo>();
										listTarget.Add(InfoData);
										CopyTargetManager.CopyTargetInfoDict[fubenID][targetIdx] = listTarget;
									}
									else
									{
										CopyTargetManager.CopyTargetInfoDict[fubenID][targetIdx].Add(InfoData);
									}
									CopyTargetKey key = new CopyTargetKey();
									key.fubenID = fubenID;
									key.targetIdx = targetIdx;
									if (!CopyTargetManager.CopyTargetKeyDict.ContainsKey(InfoData.MonsterID))
									{
										List<CopyTargetKey> listTarget2 = new List<CopyTargetKey>();
										listTarget2.Add(key);
										CopyTargetManager.CopyTargetKeyDict[InfoData.MonsterID] = listTarget2;
									}
									else
									{
										CopyTargetManager.CopyTargetKeyDict[InfoData.MonsterID].Add(key);
									}
								}
							}
						}
					}
				}
			}
		}

		
		private static List<CopyTargetInfo> GetConfig(int fubenid, int targetIdx)
		{
			List<CopyTargetInfo> InfoData = null;
			lock (CopyTargetManager._Mutex)
			{
				if (CopyTargetManager.CopyTargetInfoDict.ContainsKey(fubenid))
				{
					if (CopyTargetManager.CopyTargetInfoDict[fubenid].ContainsKey(targetIdx))
					{
						InfoData = CopyTargetManager.CopyTargetInfoDict[fubenid][targetIdx];
					}
				}
			}
			return InfoData;
		}

		
		private static int GetMonsterIdx(int monsterID, int fubenid)
		{
			lock (CopyTargetManager._Mutex)
			{
				if (CopyTargetManager.CopyTargetKeyDict.ContainsKey(monsterID))
				{
					foreach (CopyTargetKey item in CopyTargetManager.CopyTargetKeyDict[monsterID])
					{
						if (item.fubenID == fubenid)
						{
							return item.targetIdx;
						}
					}
				}
			}
			return 0;
		}

		
		public static void ProcessInitGame(GameClient client)
		{
			if (null != client)
			{
				int nBeginIdx = 0;
				List<CopyTargetInfo> copyTargetInfoList;
				for (;;)
				{
					nBeginIdx++;
					copyTargetInfoList = CopyTargetManager.GetConfig(client.ClientData.FuBenID, nBeginIdx);
					if (null == copyTargetInfoList)
					{
						break;
					}
					bool bFinding = false;
					foreach (CopyTargetInfo info in copyTargetInfoList)
					{
						List<object> findMonsters = GameManager.MonsterMgr.FindMonsterByExtensionID(client.CurrentCopyMapID, info.MonsterID);
						bFinding = (findMonsters.Count > 0);
						if (bFinding)
						{
							break;
						}
					}
					if (bFinding)
					{
						goto Block_4;
					}
				}
				return;
				Block_4:
				foreach (CopyTargetInfo info in copyTargetInfoList)
				{
					List<object> findMonsters = GameManager.MonsterMgr.FindMonsterByExtensionID(client.CurrentCopyMapID, info.MonsterID);
					string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.FuBenID,
						nBeginIdx,
						info.MonsterID,
						findMonsters.Count
					});
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strCmd, 877);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		
		public static void ProcessMonsterDead(GameClient client, Monster monster)
		{
			if (null != client)
			{
				if (null != monster)
				{
					int targetIdx = CopyTargetManager.GetMonsterIdx(monster.MonsterInfo.ExtensionID, client.ClientData.FuBenID);
					if (targetIdx > 0)
					{
						List<object> findMonsters = GameManager.MonsterMgr.FindMonsterByExtensionID(monster.CurrentCopyMapID, monster.MonsterInfo.ExtensionID);
						string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.FuBenID,
							targetIdx,
							monster.MonsterInfo.ExtensionID,
							findMonsters.Count
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strCmd, 877);
						if (MapTypes.MarriageCopy == Global.GetMapType(client.ClientData.MapCode))
						{
							if (client.ClientData.MyMarriageData.byMarrytype > 0 && client.ClientData.MyMarriageData.nSpouseID > 0)
							{
								GameClient spouseClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (spouseClient != null && MapTypes.MarriageCopy == Global.GetMapType(spouseClient.ClientData.MapCode))
								{
									spouseClient.sendCmd(877, strCmd, false);
								}
							}
							client.sendCmd(877, strCmd, false);
						}
						else
						{
							TeamData td = null;
							if (client.ClientData.TeamID > 0)
							{
								td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
							}
							if (null == td)
							{
								Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
							}
							else
							{
								lock (td)
								{
									for (int i = 0; i < td.TeamRoles.Count; i++)
									{
										GameClient gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
										if (null != gc)
										{
											if (gc.ClientData.MapCode == client.ClientData.MapCode)
											{
												if (gc.ClientData.CopyMapID == client.ClientData.CopyMapID)
												{
													Global._TCPManager.MySocketListener.SendData(gc.ClientSocket, tcpOutPacket, true);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		private static object _Mutex = new object();

		
		private static Dictionary<int, Dictionary<int, List<CopyTargetInfo>>> CopyTargetInfoDict = new Dictionary<int, Dictionary<int, List<CopyTargetInfo>>>();

		
		private static Dictionary<int, List<CopyTargetKey>> CopyTargetKeyDict = new Dictionary<int, List<CopyTargetKey>>();
	}
}
