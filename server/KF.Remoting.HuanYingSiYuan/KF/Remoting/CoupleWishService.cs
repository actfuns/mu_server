using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x02000013 RID: 19
	internal class CoupleWishService
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00008C04 File Offset: 0x00006E04
		private CoupleWishService()
		{
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00008C78 File Offset: 0x00006E78
		public static CoupleWishService getInstance()
		{
			return CoupleWishService._Instance;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00008C90 File Offset: 0x00006E90
		public void StartUp()
		{
			try
			{
				this._Config.Load(KuaFuServerManager.GetResourcePath(CoupleWishConsts.RankAwardCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath(CoupleWishConsts.WishTypeCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath(CoupleWishConsts.YanHuiCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes));
				this.ReloadSyncData();
				this.WishRecordMgr = new CoupleWishRecordManager(this.SyncData.ThisWeek.Week);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "CoupleWishService.StartUp failed!", ex, true);
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00008D18 File Offset: 0x00006F18
		private void ReloadSyncData()
		{
			lock (this.Mutex)
			{
				DateTime now = TimeUtil.NowDateTime();
				this.SyncData.ThisWeek.ModifyTime = now;
				this.SyncData.ThisWeek.Week = this.CurrRankWeek(now);
				this.SyncData.ThisWeek.RankList = this.Persistence.LoadRankFromDb(this.SyncData.ThisWeek.Week);
				this.SyncData.ThisWeek.BuildIndex();
				int i = 1;
				while (i < this.SyncData.ThisWeek.RankList.Count && !this.IsNeedSort)
				{
					this.IsNeedSort = (this.SyncData.ThisWeek.RankList[i].CompareTo(this.SyncData.ThisWeek.RankList[i - 1]) < 0);
					i++;
				}
				this.CheckSortRank();
				this.CheckSaveRank();
				this.SyncData.LastWeek.ModifyTime = now;
				this.SyncData.LastWeek.Week = this.CurrRankWeek(now.AddDays(-7.0));
				this.SyncData.LastWeek.RankList = this.Persistence.LoadRankFromDb(this.SyncData.LastWeek.Week);
				this.SyncData.LastWeek.BuildIndex();
				this.SyncData.Statue = this.Persistence.LoadCoupleStatue(this.SyncData.LastWeek.Week);
				if (this.SyncData.LastWeek.RankList.Count > 0 && this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Rank == 1 && this.SyncData.Statue.DbCoupleId != this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().DbCoupleId)
				{
					this.SyncData.Statue = new CoupleWishSyncStatueData();
					this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
					this.SyncData.Statue.Week = this.SyncData.LastWeek.Week;
					this.SyncData.Statue.DbCoupleId = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().DbCoupleId;
					this.SyncData.Statue.Man = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Man;
					this.SyncData.Statue.Wife = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Wife;
					this.Persistence.WriteStatueData(this.SyncData.Statue);
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00009038 File Offset: 0x00007238
		public void Update()
		{
			if (this.WishRecordMgr != null)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
					{
						this.UpdateFrameCount += 1U;
						if (this.LastUpdateTime.DayOfYear != now.DayOfYear && TimeUtil.GetWeekDay1To7(now) == 1)
						{
							lock (this.Mutex)
							{
								this.CheckSortRank();
								this.CheckSaveRank();
								this.ReloadSyncData();
								this.WishRecordMgr.UpdateWeek(this.SyncData.ThisWeek.Week);
							}
						}
						if (this.UpdateFrameCount % 30U == 0U)
						{
							this.CheckSortRank();
						}
						if (this.UpdateFrameCount % 600U == 0U)
						{
							this.CheckSaveRank();
							this.WishRecordMgr.ClearUnActiveRecord();
						}
						this.LastUpdateTime = now;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, "CoupleWishService.Update failed!", ex, true);
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000091E8 File Offset: 0x000073E8
		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			DateTime now = TimeUtil.NowDateTime();
			long nowMs = now.Ticks / 10000L;
			int result;
			try
			{
				lock (this.Mutex)
				{
					if (this.SyncData.ThisWeek.Week != this.CurrRankWeek(now))
					{
						result = -11000;
					}
					else
					{
						CoupleWishTypeConfig wishCfg = this._Config.WishTypeCfgList.Find((CoupleWishTypeConfig _w) => _w.WishType == req.WishType);
						if (wishCfg == null)
						{
							result = -3;
						}
						else
						{
							if (wishCfg.CooldownTime > 0)
							{
								if (this.WishCdControls.ContainsKey(req.WishType) && nowMs - this.WishCdControls[req.WishType] < (long)(wishCfg.CooldownTime * 1000))
								{
									return -30;
								}
							}
							CoupleWishCoupleDataK coupleData;
							if (req.IsWishRank)
							{
								int idx;
								if (!this.SyncData.ThisWeek.CoupleIdex.TryGetValue(req.ToCoupleId, out idx))
								{
									return -11;
								}
								coupleData = this.SyncData.ThisWeek.RankList[idx];
								coupleData.BeWishedNum += wishCfg.GetWishNum;
								if (req.ToManSelector != null && req.ToWifeSelector != null)
								{
									coupleData.Man = req.ToMan;
									coupleData.ManSelector = req.ToManSelector;
									coupleData.Wife = req.ToWife;
									coupleData.WifeSelector = req.ToWifeSelector;
									this.Persistence.WriteCoupleData(this.SyncData.ThisWeek.Week, coupleData);
								}
							}
							else
							{
								if (req.ToManSelector == null || req.ToWifeSelector == null)
								{
									return -11003;
								}
								if (!this.IsValidCoupleIfExist(req.ToMan.RoleId, req.ToWife.RoleId))
								{
									return -11003;
								}
								bool bFirstCreate = false;
								int idx;
								if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(req.ToMan.RoleId, out idx))
								{
									bFirstCreate = true;
									coupleData = new CoupleWishCoupleDataK();
									coupleData.DbCoupleId = this.Persistence.GetNextDbCoupleId();
									coupleData.Rank = this.SyncData.ThisWeek.RankList.Count + 1;
								}
								else
								{
									coupleData = this.SyncData.ThisWeek.RankList[idx];
								}
								coupleData.Man = req.ToMan;
								coupleData.ManSelector = req.ToManSelector;
								coupleData.Wife = req.ToWife;
								coupleData.WifeSelector = req.ToWifeSelector;
								coupleData.BeWishedNum += wishCfg.GetWishNum;
								if (!this.Persistence.WriteCoupleData(this.SyncData.ThisWeek.Week, coupleData))
								{
									coupleData.BeWishedNum -= wishCfg.GetWishNum;
									return -15;
								}
								if (bFirstCreate)
								{
									this.SyncData.ThisWeek.RankList.Add(coupleData);
									this.SyncData.ThisWeek.BuildIndex();
								}
							}
							this.IsNeedSort = true;
							if (this.SyncData.ThisWeek.RankList.Count <= CoupleWishConsts.MaxRankNum || this.SyncData.ThisWeek.RankList.Last<CoupleWishCoupleDataK>().Rank <= CoupleWishConsts.MaxRankNum)
							{
								this.CheckSortRank();
							}
							this.WishCdControls[req.WishType] = nowMs;
							this.WishRecordMgr.AddWishRecord(req.From, req.WishType, req.WishTxt, coupleData.DbCoupleId, coupleData.Man, coupleData.Wife);
							result = 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = -11003;
			}
			return result;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000096F8 File Offset: 0x000078F8
		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			List<CoupleWishWishRecordData> result;
			try
			{
				lock (this.Mutex)
				{
					result = this.WishRecordMgr.GetWishRecord(roleId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000976C File Offset: 0x0000796C
		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			CoupleWishSyncData result;
			try
			{
				lock (this.Mutex)
				{
					CoupleWishSyncData syncData = new CoupleWishSyncData();
					if (oldThisWeek != this.SyncData.ThisWeek.ModifyTime && TimeUtil.RandomDispatchTime(oldThisWeek, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						syncData.ThisWeek = this.SyncData.ThisWeek.SimpleClone();
					}
					else
					{
						syncData.ThisWeek.ModifyTime = oldThisWeek;
					}
					if (oldLastWeek != this.SyncData.LastWeek.ModifyTime && TimeUtil.RandomDispatchTime(oldLastWeek, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						syncData.LastWeek = this.SyncData.LastWeek.SimpleClone();
					}
					else
					{
						syncData.LastWeek.ModifyTime = oldLastWeek;
					}
					if (oldStatue != this.SyncData.Statue.ModifyTime && TimeUtil.RandomDispatchTime(oldStatue, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						syncData.Statue = this.SyncData.Statue.SimpleClone();
					}
					else
					{
						syncData.Statue.ModifyTime = oldStatue;
					}
					result = syncData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = null;
			}
			return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000990C File Offset: 0x00007B0C
		public int CoupleWishPreDivorce(int man, int wife)
		{
			int result;
			lock (this.Mutex)
			{
				DateTime now = TimeUtil.NowDateTime();
				if (!this.IsValidCoupleIfExist(man, wife))
				{
					int week;
					if (!this._Config.IsInWishTime(now, out week))
					{
						result = 1;
					}
					else
					{
						result = -11003;
					}
				}
				else
				{
					int week;
					if (this._Config.IsInWishTime(now, out week))
					{
						int idx;
						if (this.SyncData.ThisWeek.RoleIndex.TryGetValue(man, out idx))
						{
							CoupleWishCoupleDataK data = this.SyncData.ThisWeek.RankList[idx];
							if (!this.Persistence.ClearCoupleData(data.DbCoupleId))
							{
								return -15;
							}
							this.SyncData.ThisWeek.RankList.RemoveAt(idx);
							this.SyncData.ThisWeek.BuildIndex();
							this.IsNeedSort = true;
						}
					}
					if (this.SyncData.Statue.DbCoupleId > 0 && this.SyncData.Statue.Man != null && this.SyncData.Statue.Wife != null)
					{
						if (this.SyncData.Statue.Man.RoleId == man && this.SyncData.Statue.Wife.RoleId == wife && this.SyncData.Statue.IsDivorced != 1)
						{
							int oldDivorced = this.SyncData.Statue.IsDivorced;
							this.SyncData.Statue.IsDivorced = 1;
							if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
							{
								this.SyncData.Statue.IsDivorced = oldDivorced;
								return -15;
							}
							this.SyncData.Statue.ModifyTime = now;
						}
					}
					result = 1;
				}
			}
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00009B48 File Offset: 0x00007D48
		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			int result;
			lock (this.Mutex)
			{
				if (this.SyncData.Statue.DbCoupleId > 0 && this.SyncData.Statue.DbCoupleId == toCoupleId && this.SyncData.Statue.ManRoleDataEx != null && this.SyncData.Statue.WifeRoleDataEx != null)
				{
					this.SyncData.Statue.BeAdmireCount++;
					this.Persistence.WriteStatueData(this.SyncData.Statue);
				}
				this.Persistence.AddAdmireLog(fromRole, fromZone, admireType, toCoupleId, this.SyncData.LastWeek.Week);
				this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
				result = 1;
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00009C4C File Offset: 0x00007E4C
		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			int result;
			lock (this.Mutex)
			{
				if (this.SyncData.Statue.DbCoupleId != toCoupleId)
				{
					result = -12;
				}
				else if (this.SyncData.Statue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
				{
					result = -16;
				}
				else
				{
					this.SyncData.Statue.YanHuiJoinNum++;
					if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
					{
						this.SyncData.Statue.YanHuiJoinNum--;
						result = -15;
					}
					else
					{
						this.Persistence.AddYanHuiJoinLog(fromRole, fromZone, toCoupleId, this.SyncData.LastWeek.Week);
						this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
						result = 1;
					}
				}
			}
			return result;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00009D64 File Offset: 0x00007F64
		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			if (req != null && req.ManStatue != null && req.WifeStatue != null)
			{
				lock (this.Mutex)
				{
					if (this.SyncData.Statue.DbCoupleId == req.DbCoupleId)
					{
						byte[] oldManEx = this.SyncData.Statue.ManRoleDataEx;
						byte[] oldWifeEx = this.SyncData.Statue.WifeRoleDataEx;
						this.SyncData.Statue.ManRoleDataEx = req.ManStatue;
						this.SyncData.Statue.WifeRoleDataEx = req.WifeStatue;
						if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
						{
							this.SyncData.Statue.ManRoleDataEx = oldManEx;
							this.SyncData.Statue.WifeRoleDataEx = oldWifeEx;
						}
					}
					this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00009E94 File Offset: 0x00008094
		private void CheckSortRank()
		{
			lock (this.Mutex)
			{
				if (this.IsNeedSort)
				{
					this.IsNeedSort = false;
					this.SyncData.ThisWeek.RankList.Sort();
					List<int> eachRankMinLimit = new List<int>();
					foreach (CoupleWishRankAwardConfig cfg in this._Config.RankAwardCfgList)
					{
						if (cfg.EndRank <= 0)
						{
							break;
						}
						for (int i = cfg.StartRank; i <= cfg.EndRank; i++)
						{
							eachRankMinLimit.Add(cfg.MinWishNum);
						}
					}
					int currRank = 1;
					int currIdx = 0;
					while (currIdx < this.SyncData.ThisWeek.RankList.Count)
					{
						if (currRank - 1 >= 0 && currRank - 1 < eachRankMinLimit.Count)
						{
							if (this.SyncData.ThisWeek.RankList[currIdx].BeWishedNum >= eachRankMinLimit[currRank - 1])
							{
								this.SyncData.ThisWeek.RankList[currIdx].Rank = currRank;
								currRank++;
								currIdx++;
							}
							else
							{
								currRank++;
							}
						}
						else
						{
							this.SyncData.ThisWeek.RankList[currIdx].Rank = currRank;
							currRank++;
							currIdx++;
						}
					}
					this.SyncData.ThisWeek.ModifyTime = TimeUtil.NowDateTime();
					this.SyncData.ThisWeek.BuildIndex();
					this.IsNeedSaveRank = true;
				}
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000A0BC File Offset: 0x000082BC
		private void CheckSaveRank()
		{
			lock (this.Mutex)
			{
				if (this.IsNeedSaveRank)
				{
					LogManager.WriteLog(LogTypes.Error, "CoupleWishService.CheckSaveRank begin", null, true);
					this.Persistence.UpdateRand2Db(this.SyncData.ThisWeek.RankList);
					LogManager.WriteLog(LogTypes.Error, "CoupleWishService.CheckSaveRank end", null, true);
					this.IsNeedSaveRank = false;
				}
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000A150 File Offset: 0x00008350
		private bool IsValidCoupleIfExist(int man, int wife)
		{
			bool result;
			lock (this.Mutex)
			{
				int manIdx;
				if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(man, out manIdx))
				{
					manIdx = -1;
				}
				int wifeIdx;
				if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(wife, out wifeIdx))
				{
					wifeIdx = -1;
				}
				if (manIdx != wifeIdx)
				{
					result = false;
				}
				else
				{
					if (manIdx != -1)
					{
						CoupleWishCoupleDataK coupleData = this.SyncData.ThisWeek.RankList[manIdx];
						if ((coupleData.Man.RoleId != man || coupleData.Wife.RoleId != wife) && (coupleData.Man.RoleId != wife || coupleData.Wife.RoleId != man))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000A264 File Offset: 0x00008464
		private int CurrRankWeek(DateTime time)
		{
			return TimeUtil.MakeFirstWeekday(time);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000A27C File Offset: 0x0000847C
		public void OnStopServer()
		{
			try
			{
				SysConOut.WriteLine("开始检测是否刷新情侣排行榜到数据库...");
				lock (this.Mutex)
				{
					this.CheckSortRank();
					this.CheckSaveRank();
				}
				SysConOut.WriteLine("结束检测是否刷新情侣排行榜到数据库...");
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		// Token: 0x0400005F RID: 95
		private static readonly CoupleWishService _Instance = new CoupleWishService();

		// Token: 0x04000060 RID: 96
		private object Mutex = new object();

		// Token: 0x04000061 RID: 97
		private CoupleWishSyncData SyncData = new CoupleWishSyncData();

		// Token: 0x04000062 RID: 98
		private DateTime LastUpdateTime = DateTime.MinValue;

		// Token: 0x04000063 RID: 99
		private uint UpdateFrameCount = 0U;

		// Token: 0x04000064 RID: 100
		private CoupleWishRecordManager WishRecordMgr = null;

		// Token: 0x04000065 RID: 101
		private CoupleWishConfig _Config = new CoupleWishConfig();

		// Token: 0x04000066 RID: 102
		private CoupleWishPersistence Persistence = CoupleWishPersistence.getInstance();

		// Token: 0x04000067 RID: 103
		private Dictionary<int, long> WishCdControls = new Dictionary<int, long>();

		// Token: 0x04000068 RID: 104
		private bool IsNeedSort = false;

		// Token: 0x04000069 RID: 105
		private bool IsNeedSaveRank = false;
	}
}
