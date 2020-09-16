using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;

namespace GameServer.Logic
{
	
	public class LianZhiManager : IManager
	{
		
		public static LianZhiManager GetInstance()
		{
			return LianZhiManager.Instance;
		}

		
		public bool initialize()
		{
			this.InitConfig();
			TCPCmdDispatcher.getInstance().registerProcessor(668, 3, LianZhiCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_LIANZHI));
			TCPCmdDispatcher.getInstance().registerProcessor(669, 1, LianZhiCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_QUERY_LIANZHICOUNT));
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void InitConfig()
		{
			try
			{
				this.JinBiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("JinBiLianZhi", ',');
				this.BangZuanLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("BangZuanLianZhi", ',');
				this.ZuanShiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("ZuanShiLianZhi", ',');
				this.VIPJinBiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiLianZhi", ',');
				this.VIPBangZuanLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPBangZuanLianZhi", ',');
				this.VIPZuanShiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPZuanShiLianZhi", ',');
				this.ConfigLoadSuccess = true;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载炼制系统配置信息是出错: {0}", ex.ToString()));
			}
		}

		
		public bool QueryLianZhiCount(GameClient client)
		{
			List<int> result = new List<int>();
			int roleID = client.ClientData.RoleID;
			int vipLevel = client.ClientData.VipLevel;
			int nID = 669;
			result.Add(1);
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			bool result2;
			if (!this.ConfigLoadSuccess)
			{
				result[0] = -3;
				result.Add(0);
				result.Add(0);
				result.Add(0);
				client.sendCmd<List<int>>(nID, result, false);
				result2 = true;
			}
			else
			{
				int lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiCount");
				int lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiDayID");
				int lianZhiMaxCount = this.JinBiLianZhi[2] + this.VIPJinBiLianZhi[Math.Min(this.VIPJinBiLianZhi.Length - 1, vipLevel)];
				if (lianZhiDayID != dayID)
				{
					lianZhiCount = 0;
				}
				result.Add(lianZhiCount);
				lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanCount");
				lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanDayID");
				lianZhiMaxCount = this.BangZuanLianZhi[2] + this.VIPBangZuanLianZhi[Math.Min(this.VIPBangZuanLianZhi.Length - 1, vipLevel)];
				if (lianZhiDayID != dayID)
				{
					lianZhiCount = 0;
				}
				result.Add(lianZhiCount);
				lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiCount");
				lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiDayID");
				lianZhiMaxCount = this.ZuanShiLianZhi[4] + this.VIPZuanShiLianZhi[Math.Min(this.VIPZuanShiLianZhi.Length - 1, vipLevel)];
				if (lianZhiDayID != dayID)
				{
					lianZhiCount = 0;
				}
				result.Add(lianZhiCount);
				client.sendCmd<List<int>>(nID, result, false);
				result2 = true;
			}
			return result2;
		}

		
		public bool ExecLianZhi(GameClient client, int type, int count)
		{
			int roleID = client.ClientData.RoleID;
			int vipLevel = client.ClientData.VipLevel;
			int nID = 668;
			string useMsg = "炼制系统";
			List<int> result = new List<int>();
			result.Add(1);
			result.Add(type);
			result.Add(count);
			if (!this.ConfigLoadSuccess)
			{
				result[0] = -3;
				client.sendCmd<List<int>>(nID, result, false);
			}
			else if (type < 0 || type > 2)
			{
				result[0] = -5;
				client.sendCmd<List<int>>(nID, result, false);
			}
			else
			{
				int needJinBi = 0;
				int needBangZuan = 0;
				int needZuanShi = 0;
				long addExp = 0L;
				int addXingHun = 0;
				int addJinBi = 0;
				int lianZhiCount = 0;
				int lianZhiDayID = -1;
				int lianZhiMaxCount = 0;
				int dayID = TimeUtil.NowDateTime().DayOfYear;
				if (type == 0)
				{
					useMsg = "金币炼制";
					lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiCount");
					lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiDayID");
					lianZhiMaxCount = this.JinBiLianZhi[2] + this.VIPJinBiLianZhi[Math.Min(this.VIPJinBiLianZhi.Length - 1, vipLevel)];
					needJinBi = this.JinBiLianZhi[0];
					addExp = (long)this.JinBiLianZhi[1];
					ProcessTask.ProcessAddTaskVal(client, TaskTypes.LianZhi_JinBi, -1, 1, new object[0]);
					double awardmuti = 0.0;
					double awardmuticount = 0.0;
					double awardcount = 0.0;
					JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != activity)
					{
						JieRiMultConfig config = activity.GetConfig(6);
						if (null != config)
						{
							awardmuticount += config.GetMult();
						}
						config = activity.GetConfig(9);
						if (null != config)
						{
							awardmuti += config.GetMult();
						}
					}
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != spAct)
					{
						awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						awardcount += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					awardmuti = Math.Max(1.0, awardmuti);
					awardmuticount = Math.Max(1.0, awardmuticount);
					addExp = (long)((int)((double)addExp * awardmuti));
					lianZhiMaxCount = lianZhiMaxCount * (int)awardmuticount + (int)awardcount;
				}
				else if (type == 1)
				{
					useMsg = "绑钻炼制";
					lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanCount");
					lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanDayID");
					lianZhiMaxCount = this.BangZuanLianZhi[2] + this.VIPBangZuanLianZhi[Math.Min(this.VIPBangZuanLianZhi.Length - 1, vipLevel)];
					needBangZuan = this.BangZuanLianZhi[0];
					addXingHun = this.BangZuanLianZhi[1];
					double awardmuti = 0.0;
					double awardmuticount = 0.0;
					double awardcount = 0.0;
					JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != activity)
					{
						JieRiMultConfig config = activity.GetConfig(6);
						if (null != config)
						{
							awardmuticount += config.GetMult();
						}
						config = activity.GetConfig(9);
						if (null != config)
						{
							awardmuti += config.GetMult();
						}
					}
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != spAct)
					{
						awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						awardcount += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					awardmuti = Math.Max(1.0, awardmuti);
					awardmuticount = Math.Max(1.0, awardmuticount);
					addXingHun = (int)((double)addXingHun * awardmuti);
					lianZhiMaxCount = lianZhiMaxCount * (int)awardmuticount + (int)awardcount;
				}
				else if (type == 2)
				{
					useMsg = "钻石炼制";
					lianZhiCount = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiCount");
					lianZhiDayID = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiDayID");
					lianZhiMaxCount = this.ZuanShiLianZhi[4] + this.VIPZuanShiLianZhi[Math.Min(this.VIPZuanShiLianZhi.Length - 1, vipLevel)];
					needZuanShi = this.ZuanShiLianZhi[0];
					addExp = (long)this.ZuanShiLianZhi[1];
					addXingHun = this.ZuanShiLianZhi[2];
					addJinBi = this.ZuanShiLianZhi[3];
					double awardmuti = 0.0;
					double awardmuticount = 0.0;
					double awardcount = 0.0;
					JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != activity)
					{
						JieRiMultConfig config = activity.GetConfig(6);
						if (null != config)
						{
							awardmuticount += config.GetMult();
						}
						config = activity.GetConfig(9);
						if (null != config)
						{
							awardmuti += config.GetMult();
						}
					}
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != spAct)
					{
						awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						awardcount += spAct.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					awardmuti = Math.Max(1.0, awardmuti);
					awardmuticount = Math.Max(1.0, awardmuticount);
					addExp = (long)((int)((double)addExp * awardmuti));
					addXingHun = (int)((double)addXingHun * awardmuti);
					addJinBi = (int)((double)addJinBi * awardmuti);
					lianZhiMaxCount = lianZhiMaxCount * (int)awardmuticount + (int)awardcount;
				}
				if (lianZhiDayID != dayID)
				{
					lianZhiCount = 0;
				}
				if (count <= 0)
				{
					count = lianZhiMaxCount - lianZhiCount;
				}
				if (count <= 0 || lianZhiCount + count > lianZhiMaxCount)
				{
					result[0] = -16;
					client.sendCmd<List<int>>(nID, result, false);
				}
				else
				{
					needJinBi *= count;
					needBangZuan *= count;
					needZuanShi *= count;
					addExp *= (long)count;
					addXingHun *= count;
					addJinBi *= count;
					addExp = Global.GetExpMultiByZhuanShengExpXiShu(client, addExp);
					if (needJinBi > 0 && !Global.SubBindTongQianAndTongQian(client, needJinBi, useMsg))
					{
						result[0] = -9;
						client.sendCmd<List<int>>(nID, result, false);
					}
					else if (needBangZuan > 0 && !GameManager.ClientMgr.SubUserGold(client, needBangZuan, useMsg))
					{
						result[0] = -17;
						client.sendCmd<List<int>>(nID, result, false);
					}
					else if (needZuanShi > 0 && !GameManager.ClientMgr.SubUserMoney(client, needZuanShi, useMsg, true, true, true, true, DaiBiSySType.None))
					{
						result[0] = -10;
						client.sendCmd<List<int>>(nID, result, false);
					}
					else
					{
						if (addExp > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
						}
						if (addJinBi > 0)
						{
							GameManager.ClientMgr.AddMoney1(client, addJinBi, useMsg, true);
						}
						if (addXingHun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, addXingHun, useMsg, true, true);
						}
						lianZhiCount += count;
						lianZhiDayID = dayID;
						if (type == 0)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JinBiZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiJinBiCount", lianZhiCount, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiJinBiDayID", lianZhiDayID, true);
						}
						else if (type == 1)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.BangZuanZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiBangZuanCount", lianZhiCount, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiBangZuanDayID", lianZhiDayID, true);
						}
						else if (type == 2)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ZuanShiZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiZuanShiCount", lianZhiCount, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiZuanShiDayID", lianZhiDayID, true);
						}
						client.sendCmd<List<int>>(nID, result, false);
					}
				}
			}
			return true;
		}

		
		private static LianZhiManager Instance = new LianZhiManager();

		
		private int[] JinBiLianZhi = null;

		
		private int[] VIPJinBiLianZhi = null;

		
		private int[] BangZuanLianZhi = null;

		
		private int[] VIPBangZuanLianZhi = null;

		
		private int[] ZuanShiLianZhi = null;

		
		private int[] VIPZuanShiLianZhi = null;

		
		private bool ConfigLoadSuccess = false;
	}
}
