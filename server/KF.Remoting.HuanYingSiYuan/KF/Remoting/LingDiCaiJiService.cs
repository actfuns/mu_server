using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x02000026 RID: 38
	public class LingDiCaiJiService
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x00017504 File Offset: 0x00015704
		public static LingDiCaiJiService Instance()
		{
			return LingDiCaiJiService._instance;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0001751C File Offset: 0x0001571C
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00017534 File Offset: 0x00015734
		public int KfServerId
		{
			get
			{
				return this._KfServerId;
			}
			set
			{
				this._KfServerId = value;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00017540 File Offset: 0x00015740
		public void InitConfig()
		{
			try
			{
				this.RoleNumMax = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("ManorMaxPlayer", -1);
				LingDiData diGong = new LingDiData
				{
					RoleId = 0,
					LingDiType = 0,
					JunTuanId = -1,
					JunTuanName = "",
					BeginTime = DateTime.MaxValue,
					EndTime = DateTime.MinValue,
					OpenCount = 0,
					ShouWeiList = new List<LingDiShouWei>(),
					RoleData = null
				};
				MySqlDataReader sdr = DbHelperMySQL.ExecuteReader(string.Format("select * from t_lingditequan where lingzhu=1 and lingditype=0", new object[0]), false);
				if (sdr.Read())
				{
					diGong.RoleId = Convert.ToInt32(sdr["rid"].ToString());
					diGong.LingDiType = 0;
					diGong.JunTuanId = Convert.ToInt32(sdr["juntuanid"].ToString());
					diGong.JunTuanName = sdr["juntuanname"].ToString();
					diGong.BeginTime = DateTime.Parse(sdr["begintime"].ToString());
					diGong.EndTime = DateTime.Parse(sdr["endtime"].ToString());
					diGong.OpenCount = Convert.ToInt32(sdr["opencount"].ToString());
					string[] shouwei = sdr["shouwei"].ToString().Split(new char[]
					{
						'|'
					});
					foreach (string shouWeiItem in shouwei)
					{
						string[] item = shouWeiItem.Split(new char[]
						{
							','
						});
						if (item.Length >= 2)
						{
							diGong.ShouWeiList.Add(new LingDiShouWei
							{
								State = Convert.ToInt32(item[0]),
								FreeBuShuTime = DateTime.Parse(item[1])
							});
						}
					}
					if (!sdr.IsDBNull(sdr.GetOrdinal("roledata")))
					{
						diGong.RoleData = (byte[])sdr["roledata"];
					}
				}
				else
				{
					diGong.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					diGong.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					diGong.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					diGong.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
				}
				LingDiData huangMo = new LingDiData
				{
					RoleId = 0,
					LingDiType = 1,
					JunTuanId = -1,
					JunTuanName = "",
					BeginTime = DateTime.MaxValue,
					EndTime = DateTime.MinValue,
					OpenCount = 0,
					ShouWeiList = new List<LingDiShouWei>(),
					RoleData = null
				};
				MySqlDataReader sdr2 = DbHelperMySQL.ExecuteReader(string.Format("select * from t_lingditequan where lingzhu=1 and lingditype=1", new object[0]), false);
				if (sdr2.Read())
				{
					huangMo.RoleId = Convert.ToInt32(sdr2["rid"].ToString());
					huangMo.LingDiType = 1;
					huangMo.JunTuanId = Convert.ToInt32(sdr2["juntuanid"].ToString());
					huangMo.JunTuanName = sdr2["juntuanname"].ToString();
					huangMo.BeginTime = DateTime.Parse(sdr2["begintime"].ToString());
					huangMo.EndTime = DateTime.Parse(sdr2["endtime"].ToString());
					huangMo.OpenCount = Convert.ToInt32(sdr2["opencount"].ToString());
					string[] shouwei = sdr2["shouwei"].ToString().Split(new char[]
					{
						'|'
					});
					foreach (string shouWeiItem in shouwei)
					{
						string[] item = shouWeiItem.Split(new char[]
						{
							','
						});
						if (item.Length >= 2)
						{
							huangMo.ShouWeiList.Add(new LingDiShouWei
							{
								State = Convert.ToInt32(item[0]),
								FreeBuShuTime = DateTime.Parse(item[1])
							});
						}
					}
					if (!sdr2.IsDBNull(sdr2.GetOrdinal("roledata")))
					{
						huangMo.RoleData = (byte[])sdr2["roledata"];
					}
				}
				else
				{
					huangMo.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					huangMo.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					huangMo.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
					huangMo.ShouWeiList.Add(new LingDiShouWei
					{
						State = 0,
						FreeBuShuTime = DateTime.MaxValue
					});
				}
				lock (this.Mutex)
				{
					this.LingDiDataList = new List<LingDiData>();
					this.LingDiDataList.Add(diGong);
					this.LingDiDataList.Add(huangMo);
					this.Initialized = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJiServer 初始化地图信息出错 ex:{0}", ex.Message.ToString()), null, true);
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00017BA0 File Offset: 0x00015DA0
		public bool isLingZhu(int junTuanId)
		{
			bool result;
			lock (this.Mutex)
			{
				if (this.LingDiDataList == null)
				{
					result = false;
				}
				else
				{
					foreach (LingDiData item in this.LingDiDataList)
					{
						if (item.JunTuanId == junTuanId && junTuanId > 0)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00017C64 File Offset: 0x00015E64
		public int SetDoubleOpenTime(int roleId, int lingDiType, DateTime openTime, int openSeconds)
		{
			LingDiData data = null;
			lock (this.Mutex)
			{
				if (this.LingDiDataList[lingDiType] == null)
				{
					return -8;
				}
				this.LingDiDataList[lingDiType].BeginTime = openTime;
				this.LingDiDataList[lingDiType].EndTime = openTime.AddSeconds((double)openSeconds);
				this.LingDiDataList[lingDiType].OpenCount++;
				data = this.LingDiDataList[lingDiType];
			}
			int ret = 0;
			try
			{
				string upd = string.Format("update t_lingditequan set begintime='{0}',endtime='{1}',opencount={2},opttime='{5}' where rid={3} and lingditype={4}", new object[]
				{
					data.BeginTime,
					data.EndTime,
					data.OpenCount,
					roleId,
					lingDiType,
					TimeUtil.NowDateTime().ToString()
				});
				ret = DbHelperMySQL.ExecuteSql(upd);
				if (ret > 0)
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, new AsyncDataItem(KuaFuEventTypes.SyncLingDiDoubleOpenData, new object[]
					{
						data
					}), 0);
					return 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("初始化跨服地图{0}时出错!!!", "领地采集"), null, true);
				return -7;
			}
			return ret;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00017E0C File Offset: 0x0001600C
		public int SetShouWeiTime(int roleId, int lingDiType, DateTime openTime, int index)
		{
			int ret = 0;
			try
			{
				LingDiData data = new LingDiData();
				string shouWei = "";
				lock (this.Mutex)
				{
					if (this.LingDiDataList[lingDiType] == null)
					{
						return -8;
					}
					data = this.LingDiDataList[lingDiType];
					if (this.LingDiDataList[lingDiType].ShouWeiList.Count < index + 1)
					{
						return -8;
					}
					data.ShouWeiList[index].State = 2;
					data.ShouWeiList[index].FreeBuShuTime = DateTime.MaxValue;
					shouWei = data.ShouWeiList[0].State + "," + data.ShouWeiList[0].FreeBuShuTime.ToString();
					for (int i = 1; i < data.ShouWeiList.Count; i++)
					{
						object obj = shouWei;
						shouWei = string.Concat(new object[]
						{
							obj,
							"|",
							data.ShouWeiList[i].State,
							",",
							data.ShouWeiList[i].FreeBuShuTime.ToString()
						});
					}
				}
				string upd = string.Format("update t_lingditequan set shouwei='{0}' where rid={1} and lingditype={2}", shouWei, roleId, lingDiType);
				ret = DbHelperMySQL.ExecuteSql(upd);
				if (ret > 0)
				{
					return 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("初始化跨服地图{0}时出错!!!", "领地采集"), null, true);
				return -7;
			}
			return ret;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00018030 File Offset: 0x00016230
		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			lock (this.Mutex)
			{
				if (lingDiType == 0 && this.RoleNumDiGong >= this.RoleNumMax)
				{
					return -10;
				}
				if (lingDiType == 1 && this.RoleNumHuangMo >= this.RoleNumMax)
				{
					return -10;
				}
				if (this.KfServerId <= 0)
				{
					int kfserverId = 0;
					if (!ClientAgentManager.Instance().SpecialKfFuben(GameTypes.LingDiCaiJi, 0L, this.RoleNumMax, out kfserverId))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", kfserverId), null, true);
						return -1;
					}
					this.KfServerId = kfserverId;
				}
				if (this.KfServerId != 0)
				{
					if (lingDiType == 0)
					{
						this.RoleNumDiGong++;
					}
					else
					{
						this.RoleNumHuangMo++;
					}
				}
			}
			return this.KfServerId;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0001815C File Offset: 0x0001635C
		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			if (this.KfServerId <= 0)
			{
				int kfserverId = 0;
				if (!ClientAgentManager.Instance().SpecialKfFuben(GameTypes.LingDiCaiJi, 0L, this.RoleNumMax, out kfserverId))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", kfserverId), null, true);
					return -1;
				}
				this.KfServerId = kfserverId;
			}
			int result;
			if (this.KfServerId == 0)
			{
				result = -1;
			}
			else if (serverId != this.KfServerId)
			{
				result = this.KfServerId;
			}
			else
			{
				lock (this.Mutex)
				{
					if (lingDiType == 0)
					{
						this.RoleNumDiGong = roleNum;
					}
					else if (lingDiType == 1)
					{
						this.RoleNumHuangMo = roleNum;
					}
					result = this.KfServerId;
				}
			}
			return result;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00018254 File Offset: 0x00016454
		public int GetLingDiRoleNum(int lingDiType)
		{
			if (this.KfServerId <= 0)
			{
				int kfserverId = 0;
				if (!ClientAgentManager.Instance().SpecialKfFuben(GameTypes.LingDiCaiJi, 0L, this.RoleNumMax, out kfserverId))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("LingDiCaiJi分配跨服地图失败,error={0}", kfserverId), null, true);
					return -1;
				}
				this.KfServerId = kfserverId;
			}
			int result;
			if (this.KfServerId == 0)
			{
				result = -1;
			}
			else
			{
				lock (this.Mutex)
				{
					if (lingDiType == 0)
					{
						result = this.RoleNumDiGong;
					}
					else if (lingDiType == 1)
					{
						result = this.RoleNumHuangMo;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0001832C File Offset: 0x0001652C
		public List<LingDiData> GetLingDiData()
		{
			List<LingDiData> ret = new List<LingDiData>();
			lock (this.Mutex)
			{
				if (this.LingDiDataList.Count < 2 || this.LingDiDataList[0] == null || null == this.LingDiDataList[1])
				{
					return ret;
				}
				ret = this.LingDiDataList;
			}
			return ret;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x000183C4 File Offset: 0x000165C4
		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			try
			{
				int oldRid = 0;
				LingDiData data = new LingDiData();
				lock (this.Mutex)
				{
					if (this.LingDiDataList.Count < 2 || this.LingDiDataList[0] == null || null == this.LingDiDataList[1])
					{
						return 0;
					}
					oldRid = this.LingDiDataList[lingDiType].RoleId;
					this.LingDiDataList[lingDiType].RoleId = roleId;
					this.LingDiDataList[lingDiType].OpenCount = 0;
					this.LingDiDataList[lingDiType].BeginTime = DateTime.MaxValue;
					this.LingDiDataList[lingDiType].EndTime = DateTime.MinValue;
					foreach (LingDiShouWei item in this.LingDiDataList[lingDiType].ShouWeiList)
					{
						item.State = 0;
						item.FreeBuShuTime = DateTime.MinValue;
					}
					this.LingDiDataList[lingDiType].JunTuanId = junTuanId;
					this.LingDiDataList[lingDiType].RoleData = roledata;
					this.LingDiDataList[lingDiType].JunTuanName = junTuanName;
					data = this.LingDiDataList[lingDiType];
				}
				string optTime = TimeUtil.NowDateTime().ToString();
				string sql = string.Format("update t_lingditequan set lingzhu=0,opttime='{2}' where rid={0} and lingditype={1};", oldRid, lingDiType, optTime);
				sql += string.Format("replace into t_lingditequan(rid,lingditype,juntuanid,juntuanname,lingzhu,roledata,opttime) values ({0},{1},{2},'{3}',{4},@roledata,'{5}');", new object[]
				{
					roleId,
					lingDiType,
					junTuanId,
					junTuanName,
					zhiWu,
					optTime
				});
				int ret = DbHelperMySQL.ExecuteSqlInsertImg(sql, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("roledata", roledata)
				});
				if (ret > 0)
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(GameTypes.JunTuan, new AsyncDataItem(KuaFuEventTypes.SyncLingDiData, new object[]
					{
						data
					}), 0);
					return 1;
				}
				return ret;
			}
			catch
			{
			}
			return 0;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00018684 File Offset: 0x00016884
		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			try
			{
				this.LingDiDataList[lingDiType].ShouWeiList = shouWeiList;
				string shouwei = shouWeiList[0].State + "," + shouWeiList[0].FreeBuShuTime.ToString();
				for (int i = 1; i < shouWeiList.Count; i++)
				{
					object obj = shouwei;
					shouwei = string.Concat(new object[]
					{
						obj,
						"|",
						shouWeiList[i].State,
						",",
						shouWeiList[i].FreeBuShuTime.ToString()
					});
				}
				string sql = string.Format("update t_lingditequan set shouwei='{0}',opttime='{2}' where lingzhu=1 and lingditype={1};", shouwei, lingDiType, TimeUtil.NowDateTime().ToString());
				return DbHelperMySQL.ExecuteSql(sql);
			}
			catch
			{
			}
			return 0;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0001879C File Offset: 0x0001699C
		public bool GetClientCacheItems(int serverId)
		{
			lock (this.Mutex)
			{
				if (this.Initialized)
				{
					ClientAgent agent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (agent != null && agent.ClientInfo.ClientId > 0)
					{
						int clientId;
						if (!this.BroadcastServerIdHashSet.TryGetValue(serverId, out clientId) || clientId != agent.ClientInfo.ClientId)
						{
							this.BroadcastServerIdHashSet[serverId] = agent.ClientInfo.ClientId;
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x040000EC RID: 236
		private static LingDiCaiJiService _instance = new LingDiCaiJiService();

		// Token: 0x040000ED RID: 237
		private int _KfServerId = 0;

		// Token: 0x040000EE RID: 238
		private object Mutex = new object();

		// Token: 0x040000EF RID: 239
		private int RoleNumDiGong = 0;

		// Token: 0x040000F0 RID: 240
		private int RoleNumHuangMo = 0;

		// Token: 0x040000F1 RID: 241
		private int RoleNumMax = 0;

		// Token: 0x040000F2 RID: 242
		public List<LingDiData> LingDiDataList = new List<LingDiData>();

		// Token: 0x040000F3 RID: 243
		public Dictionary<int, int> BroadcastServerIdHashSet = new Dictionary<int, int>();

		// Token: 0x040000F4 RID: 244
		public bool Initialized = false;
	}
}
