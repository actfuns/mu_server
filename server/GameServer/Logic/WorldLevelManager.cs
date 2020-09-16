using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class WorldLevelManager
	{
		
		private WorldLevelManager()
		{
		}

		
		public static WorldLevelManager getInstance()
		{
			return WorldLevelManager.instance;
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.Mutex)
			{
				try
				{
					Dictionary<string, Tuple<int, int>> jieRiLvTypeDict = new Dictionary<string, Tuple<int, int>>();
					Dictionary<string, Dictionary<string, string>> jieRiLvDict = new Dictionary<string, Dictionary<string, string>>();
					fileName = "Config/JieRiGifts/JieRiLvType.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						string id = Global.GetSafeAttributeStr(node, "ID");
						int minLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
						int maxLevel = (int)Global.GetSafeAttributeLong(node, "MaxLevel");
						jieRiLvTypeDict.Add(id, new Tuple<int, int>(minLevel, maxLevel));
					}
					fileName = "Config/JieRiGifts/JieRiLv.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						string NeedFile = Global.GetSafeAttributeStr(node, "NeedFile");
						string JiRiLv = Global.GetSafeAttributeStr(node, "JiRiLv");
						string JieRiFile = Global.GetSafeAttributeStr(node, "JieRiFile");
						if (!(NeedFile == JieRiFile))
						{
							Dictionary<string, string> dict;
							if (!jieRiLvDict.TryGetValue(JiRiLv, out dict))
							{
								dict = new Dictionary<string, string>();
								jieRiLvDict[JiRiLv] = dict;
							}
							dict[NeedFile] = JieRiFile;
						}
					}
					this.JieRiLvTypeDict = jieRiLvTypeDict;
					this.JieRiLvDict = jieRiLvDict;
					this.JieRiStartDay = 0;
					this.JieRiWorldLevel = 0;
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		private string CalcJieRiLv()
		{
			string jieRiLvType = "";
			int offsetDay = TimeUtil.GetOffsetDay(Global.GetJieriStartDay());
			if (this.JieRiStartDay != offsetDay)
			{
				ServerDayData serverDayData = Global.sendToDB<ServerDayData, int>(11004, offsetDay, GameManager.ServerId);
				lock (this.Mutex)
				{
					if (serverDayData != null && serverDayData.Dayid == offsetDay)
					{
						this.JieRiWorldLevel = serverDayData.WorldLevel;
						this.JieRiStartDay = offsetDay;
					}
				}
			}
			lock (this.Mutex)
			{
				if (this.JieRiWorldLevel > 0)
				{
					foreach (KeyValuePair<string, Tuple<int, int>> kv in this.JieRiLvTypeDict)
					{
						if (this.JieRiWorldLevel >= kv.Value.Item1 && this.JieRiWorldLevel <= kv.Value.Item2)
						{
							jieRiLvType = kv.Key;
							break;
						}
					}
				}
			}
			return jieRiLvType;
		}

		
		public string GetJieRiConfigFileName(string filename)
		{
			string result;
			if (filename.Length <= Global.AbsoluteGameResPath.Length)
			{
				result = filename;
			}
			else
			{
				string keyName = filename.Substring(Global.AbsoluteGameResPath.Length);
				string keyName2 = Path.GetFileName(filename);
				string jieRiLvType = this.CalcJieRiLv();
				if (!string.IsNullOrEmpty(jieRiLvType))
				{
					lock (this.Mutex)
					{
						Dictionary<string, string> dict;
						if (this.JieRiLvDict.TryGetValue(jieRiLvType, out dict))
						{
							string jieRiFileName;
							if (dict.TryGetValue(keyName, out jieRiFileName))
							{
								string path = filename.Substring(0, filename.Length - keyName.Length);
								return path + jieRiFileName;
							}
							if (dict.TryGetValue(keyName2, out jieRiFileName))
							{
								string path = filename.Substring(0, filename.Length - keyName2.Length);
								return path + jieRiFileName;
							}
						}
					}
				}
				result = filename;
			}
			return result;
		}

		
		public void ResetWorldLevel()
		{
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (this.m_nResetWorldLevelDayID != dayID)
			{
				int offsetDay = TimeUtil.GetOffsetDayNow();
				string cDate = TimeUtil.GetRealDate(offsetDay).Date.ToString("yyyy-MM-dd");
				ServerDayData serverDayData = Global.sendToDB<ServerDayData, int>(11004, offsetDay, GameManager.ServerId);
				if (serverDayData != null && serverDayData.Dayid == offsetDay)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("从数据加载世界等级:day={0},worldlevel={1}", serverDayData.CDate, serverDayData.WorldLevel), null, true);
					this.m_nWorldLevel = serverDayData.WorldLevel;
					this.m_nResetWorldLevelDayID = dayID;
				}
				else
				{
					TCPOutPacket tcpOutPacket = null;
					string strcmd = string.Format("{0}:{1}", 0, 5);
					TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, TCPOutPacketPool.getInstance(), 269, strcmd, out tcpOutPacket, 0);
					if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
					{
						LogManager.WriteLog(LogTypes.Error, "世界等级装入异常", null, true);
					}
					else
					{
						int nBakResetWorldLevelDayID = this.m_nResetWorldLevelDayID;
						this.m_nResetWorldLevelDayID = dayID;
						PaiHangData paiHangData = DataHelper.BytesToObject<PaiHangData>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
						if (null != paiHangData)
						{
							int nLevelCount = 0;
							int roleCount = 0;
							if (null != paiHangData.PaiHangList)
							{
								int i = 0;
								while (i < 100 && i < paiHangData.PaiHangList.Count)
								{
									roleCount++;
									nLevelCount += paiHangData.PaiHangList[i].Val2 * 100 + paiHangData.PaiHangList[i].Val1;
									i++;
								}
							}
							this.m_nWorldLevel = ((roleCount > 0) ? (nLevelCount / roleCount) : 1);
							serverDayData = new ServerDayData
							{
								Dayid = offsetDay,
								CDate = cDate,
								WorldLevel = this.m_nWorldLevel
							};
							for (;;)
							{
								int dbRet = Global.sendToDB<int, ServerDayData>(11003, serverDayData, GameManager.ServerId);
								if (dbRet >= 0)
								{
									break;
								}
								Thread.Sleep(1000);
							}
							if (0 != nBakResetWorldLevelDayID)
							{
								int count = GameManager.ClientMgr.GetMaxClientCount();
								for (int i = 0; i < count; i++)
								{
									GameClient client = GameManager.ClientMgr.FindClientByNid(i);
									if (null != client)
									{
										this.UpddateWorldLevelBuff(client);
									}
								}
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, "世界等级装入时，获取等级排行榜失败", null, true);
						}
					}
				}
			}
		}

		
		public void UpddateWorldLevelBuff(GameClient client)
		{
			int nMeTotalLevel = client.ClientData.GetRoleData().ChangeLifeCount * 100 + client.ClientData.GetRoleData().Level;
			double nWorldLevelAddPer = Math.Round((double)(this.m_nWorldLevel - nMeTotalLevel) / 100.0, 2) * GameManager.systemParamsList.GetParamValueDoubleByName("WorldLevel", 0.0);
			int nNewBufferGoodsIndexID = (int)(nWorldLevelAddPer * 100.0);
			int nOldBufferGoodsIndexID = -1;
			BufferData bufferData = Global.GetBufferDataByID(client, 99);
			if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
			{
				nOldBufferGoodsIndexID = (int)bufferData.BufferVal;
			}
			if (nNewBufferGoodsIndexID < 0)
			{
				nNewBufferGoodsIndexID = 0;
			}
			if (nOldBufferGoodsIndexID != nNewBufferGoodsIndexID)
			{
				Global.UpdateBufferData(client, BufferItemTypes.MU_WORLDLEVEL, new double[]
				{
					(double)nNewBufferGoodsIndexID
				}, 1, true);
				client.ClientData.nTempWorldLevelPer = nWorldLevelAddPer;
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
		}

		
		private object Mutex = new object();

		
		public int m_nWorldLevel = 0;

		
		public int m_nResetWorldLevelDayID = 0;

		
		public int JieRiStartDay;

		
		public int JieRiWorldLevel;

		
		private Dictionary<string, Tuple<int, int>> JieRiLvTypeDict = new Dictionary<string, Tuple<int, int>>();

		
		private Dictionary<string, Dictionary<string, string>> JieRiLvDict = new Dictionary<string, Dictionary<string, string>>();

		
		private static WorldLevelManager instance = new WorldLevelManager();
	}
}
