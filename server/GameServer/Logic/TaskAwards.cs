using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class TaskAwards
	{
		
		private void ParseTaskAwardsItem(string awardsStr, out AwardsItemData taskAwards)
		{
			taskAwards = null;
			string[] fields = awardsStr.Split(new char[]
			{
				','
			});
			if (fields.Length == 7)
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(Convert.ToInt32(fields[0]), out systemGoods))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析任务装备奖励时，物品不存在: GoodsID={0}", Convert.ToInt32(fields[0])), null, true);
				}
				else
				{
					taskAwards = new AwardsItemData
					{
						Occupation = ((systemGoods == null) ? -1 : Global.GetMainOccupationByGoodsID(Convert.ToInt32(fields[0]))),
						RoleSex = ((systemGoods == null) ? -1 : systemGoods.GetIntValue("ToSex", -1)),
						GoodsID = Convert.ToInt32(fields[0]),
						GoodsNum = Convert.ToInt32(fields[1]),
						Binding = Convert.ToInt32(fields[2]),
						Level = Convert.ToInt32(fields[3]),
						AppendLev = Convert.ToInt32(fields[4]),
						IsHaveLuckyProp = Convert.ToInt32(fields[5]),
						ExcellencePorpValue = Convert.ToInt32(fields[6]),
						EndTime = "1900-01-01 12:00:00"
					};
				}
			}
		}

		
		private void ParseOtherAwardsItem(string awardsStr, out AwardsItemData otherAwards, string goodsEndTime)
		{
			otherAwards = null;
			string[] fields = awardsStr.Split(new char[]
			{
				','
			});
			if (fields.Length == 7)
			{
				if (string.IsNullOrEmpty(goodsEndTime) || Global.DateTimeTicks(goodsEndTime) <= 0L)
				{
					goodsEndTime = "1900-01-01 12:00:00";
				}
				otherAwards = new AwardsItemData
				{
					Occupation = -1,
					RoleSex = -1,
					GoodsID = Convert.ToInt32(fields[0]),
					GoodsNum = Convert.ToInt32(fields[1]),
					Binding = Convert.ToInt32(fields[2]),
					Level = Convert.ToInt32(fields[3]),
					AppendLev = Convert.ToInt32(fields[4]),
					IsHaveLuckyProp = Convert.ToInt32(fields[5]),
					ExcellencePorpValue = Convert.ToInt32(fields[6]),
					EndTime = goodsEndTime
				};
			}
		}

		
		private void ParseAwards(SystemXmlItem systemTask, out List<AwardsItemData> taskAwardsList, out List<AwardsItemData> otherAwardsList)
		{
			List<AwardsItemData> list;
			otherAwardsList = (list = null);
			taskAwardsList = list;
			AwardsItemData awardsItem = null;
			string taskAwardsString = systemTask.GetStringValue("Taskaward").Trim();
			if (!string.IsNullOrEmpty(taskAwardsString))
			{
				string[] taskAwardsFields = taskAwardsString.Split(new char[]
				{
					'|'
				});
				if (null != taskAwardsFields)
				{
					taskAwardsList = new List<AwardsItemData>();
					for (int i = 0; i < taskAwardsFields.Length; i++)
					{
						awardsItem = null;
						this.ParseTaskAwardsItem(taskAwardsFields[i], out awardsItem);
						if (null != awardsItem)
						{
							taskAwardsList.Add(awardsItem);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析任务装备奖励失败: TaskID={0}", systemTask.GetIntValue("ID", -1)), null, true);
						}
					}
				}
			}
			string goodsEndTime = systemTask.GetStringValue("GoodsEndTime").Trim();
			string otherAwardsString = systemTask.GetStringValue("OtherTaskaward").Trim();
			if (!string.IsNullOrEmpty(otherAwardsString))
			{
				string[] otherAwardsFields = otherAwardsString.Split(new char[]
				{
					'|'
				});
				if (null != otherAwardsFields)
				{
					otherAwardsList = new List<AwardsItemData>();
					for (int i = 0; i < otherAwardsFields.Length; i++)
					{
						awardsItem = null;
						this.ParseOtherAwardsItem(otherAwardsFields[i], out awardsItem, goodsEndTime);
						if (null != awardsItem)
						{
							otherAwardsList.Add(awardsItem);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("解析任务其他奖励失败: TaskID={0}", systemTask.GetIntValue("ID", -1)), null, true);
						}
					}
				}
			}
		}

		
		public List<AwardsItemData> FindTaskAwards(int taskID)
		{
			List<AwardsItemData> awardsList = null;
			lock (this._TaskAwardsDict)
			{
				if (this._TaskAwardsDict.TryGetValue(taskID, out awardsList))
				{
					return awardsList;
				}
			}
			SystemXmlItem systemTask = null;
			List<AwardsItemData> result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = null;
			}
			else
			{
				List<AwardsItemData> otherAwardsList = null;
				this.ParseAwards(systemTask, out awardsList, out otherAwardsList);
				if (null != awardsList)
				{
					lock (this._TaskAwardsDict)
					{
						this._TaskAwardsDict[taskID] = awardsList;
					}
				}
				result = awardsList;
			}
			return result;
		}

		
		public List<AwardsItemData> FindOtherAwards(int taskID)
		{
			List<AwardsItemData> awardsList = null;
			lock (this._OtherAwardsDict)
			{
				if (this._OtherAwardsDict.TryGetValue(taskID, out awardsList))
				{
					return awardsList;
				}
			}
			SystemXmlItem systemTask = null;
			List<AwardsItemData> result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = null;
			}
			else
			{
				List<AwardsItemData> taskAwardsList = null;
				this.ParseAwards(systemTask, out taskAwardsList, out awardsList);
				if (null != awardsList)
				{
					lock (this._OtherAwardsDict)
					{
						this._OtherAwardsDict[taskID] = awardsList;
					}
				}
				result = awardsList;
			}
			return result;
		}

		
		public int FindMoney(int taskID)
		{
			int money = -1;
			lock (this._MoneyDict)
			{
				if (this._MoneyDict.TryGetValue(taskID, out money))
				{
					return money;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = money;
			}
			else
			{
				money = systemTask.GetIntValue("BindMoneyaward", -1);
				lock (this._MoneyDict)
				{
					this._MoneyDict[taskID] = money;
				}
				result = money;
			}
			return result;
		}

		
		public long FindExperience(GameClient client, int taskID)
		{
			long experience = -1L;
			lock (this._ExperienceDict)
			{
				if (this._ExperienceDict.TryGetValue(taskID, out experience))
				{
					if (experience < 0L)
					{
						experience = this.CalcLuaScript(client, taskID, null, "ExpLua");
					}
					return experience;
				}
			}
			SystemXmlItem systemTask = null;
			long result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = experience;
			}
			else
			{
				experience = systemTask.GetLongValue("Experienceaward");
				lock (this._ExperienceDict)
				{
					this._ExperienceDict[taskID] = experience;
				}
				if (experience < 0L)
				{
					experience = this.CalcLuaScript(client, taskID, systemTask, "ExpLua");
				}
				result = experience;
			}
			return result;
		}

		
		public int FindYinLiang(int taskID)
		{
			int yinLiang = -1;
			lock (this._YinLiangDict)
			{
				if (this._YinLiangDict.TryGetValue(taskID, out yinLiang))
				{
					return yinLiang;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = yinLiang;
			}
			else
			{
				yinLiang = systemTask.GetIntValue("Moneyaward", -1);
				lock (this._YinLiangDict)
				{
					this._YinLiangDict[taskID] = yinLiang;
				}
				result = yinLiang;
			}
			return result;
		}

		
		public int FindBindYuanBao(int taskID)
		{
			int bindYuanBao = -1;
			lock (this._BindYuanBaoDict)
			{
				if (this._BindYuanBaoDict.TryGetValue(taskID, out bindYuanBao))
				{
					return bindYuanBao;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = bindYuanBao;
			}
			else
			{
				bindYuanBao = systemTask.GetIntValue("BindYuanBao", -1);
				lock (this._BindYuanBaoDict)
				{
					this._BindYuanBaoDict[taskID] = bindYuanBao;
				}
				result = bindYuanBao;
			}
			return result;
		}

		
		public int FindLingLi(int taskID)
		{
			int lingLi = -1;
			lock (this._LingLiDict)
			{
				if (this._LingLiDict.TryGetValue(taskID, out lingLi))
				{
					return lingLi;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = lingLi;
			}
			else
			{
				lingLi = systemTask.GetIntValue("LingLi", -1);
				lock (this._LingLiDict)
				{
					this._LingLiDict[taskID] = lingLi;
				}
				result = lingLi;
			}
			return result;
		}

		
		public int FindBlessPoint(int taskID)
		{
			int blessPoint = -1;
			lock (this._BlessPointDict)
			{
				if (this._BlessPointDict.TryGetValue(taskID, out blessPoint))
				{
					return blessPoint;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = blessPoint;
			}
			else
			{
				blessPoint = systemTask.GetIntValue("BlessPoint", -1);
				lock (this._BlessPointDict)
				{
					this._BlessPointDict[taskID] = blessPoint;
				}
				result = blessPoint;
			}
			return result;
		}

		
		public int FindZhenQi(GameClient client, int taskID)
		{
			int zhenQi = -1;
			lock (this._ZhenQiDict)
			{
				if (this._ZhenQiDict.TryGetValue(taskID, out zhenQi))
				{
					if (zhenQi < 0)
					{
						zhenQi = (int)this.CalcLuaScript(client, taskID, null, "ZhenQiLua");
					}
					return zhenQi;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = zhenQi;
			}
			else
			{
				zhenQi = systemTask.GetIntValue("ZhenQi", -1);
				lock (this._ZhenQiDict)
				{
					this._ZhenQiDict[taskID] = zhenQi;
				}
				if (zhenQi < 0)
				{
					zhenQi = (int)this.CalcLuaScript(client, taskID, systemTask, "ZhenQiLua");
				}
				result = zhenQi;
			}
			return result;
		}

		
		public int FindLieSha(GameClient client, int taskID)
		{
			int lieSha = -1;
			lock (this._LieShaDict)
			{
				if (this._LieShaDict.TryGetValue(taskID, out lieSha))
				{
					if (lieSha < 0)
					{
						lieSha = (int)this.CalcLuaScript(client, taskID, null, "LieShaLua");
					}
					return lieSha;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = lieSha;
			}
			else
			{
				lieSha = systemTask.GetIntValue("LieSha", -1);
				lock (this._LieShaDict)
				{
					this._LieShaDict[taskID] = lieSha;
				}
				if (lieSha < 0)
				{
					lieSha = (int)this.CalcLuaScript(client, taskID, systemTask, "LieShaLua");
				}
				result = lieSha;
			}
			return result;
		}

		
		public int FindWuXing(GameClient client, int taskID)
		{
			int wuXing = -1;
			lock (this._WuXingDict)
			{
				if (this._WuXingDict.TryGetValue(taskID, out wuXing))
				{
					if (wuXing < 0)
					{
						wuXing = (int)this.CalcLuaScript(client, taskID, null, "WuXingLua");
					}
					return wuXing;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = wuXing;
			}
			else
			{
				wuXing = systemTask.GetIntValue("WuXing", -1);
				lock (this._WuXingDict)
				{
					this._WuXingDict[taskID] = wuXing;
				}
				if (wuXing < 0)
				{
					wuXing = (int)this.CalcLuaScript(client, taskID, systemTask, "WuXingLua");
				}
				result = wuXing;
			}
			return result;
		}

		
		public int FindNeedYuanBao(GameClient client, int taskID)
		{
			int needYuanBao = -1;
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = needYuanBao;
			}
			else
			{
				needYuanBao = (int)this.CalcLuaScript(client, taskID, systemTask, "DoubleAwardLua");
				result = needYuanBao;
			}
			return result;
		}

		
		public int FindJunGong(GameClient client, int taskID)
		{
			int junGong = -1;
			lock (this._JunGongDict)
			{
				if (this._JunGongDict.TryGetValue(taskID, out junGong))
				{
					return junGong;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = junGong;
			}
			else
			{
				junGong = systemTask.GetIntValue("JunGong", -1);
				lock (this._JunGongDict)
				{
					this._JunGongDict[taskID] = junGong;
				}
				result = junGong;
			}
			return result;
		}

		
		public int FindRongYu(GameClient client, int taskID)
		{
			int rongYu = -1;
			lock (this._RongYuDict)
			{
				if (this._RongYuDict.TryGetValue(taskID, out rongYu))
				{
					return rongYu;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = rongYu;
			}
			else
			{
				rongYu = systemTask.GetIntValue("RongYu", -1);
				lock (this._RongYuDict)
				{
					this._RongYuDict[taskID] = rongYu;
				}
				result = rongYu;
			}
			return result;
		}

		
		public int FindMoJing(GameClient client, int taskID)
		{
			int nJingYuan = -1;
			lock (this._JingYuanDict)
			{
				if (this._JingYuanDict.TryGetValue(taskID, out nJingYuan))
				{
					return nJingYuan;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = nJingYuan;
			}
			else
			{
				nJingYuan = systemTask.GetIntValue("MoJing", -1);
				lock (this._JingYuanDict)
				{
					this._JingYuanDict[taskID] = nJingYuan;
				}
				result = nJingYuan;
			}
			return result;
		}

		
		public int FindXingHun(GameClient client, int taskID)
		{
			int nXinghun = -1;
			lock (this._XinHunAwardDict)
			{
				if (this._XinHunAwardDict.TryGetValue(taskID, out nXinghun))
				{
					return nXinghun;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = nXinghun;
			}
			else
			{
				nXinghun = systemTask.GetIntValue("Xinghun", -1);
				lock (this._XinHunAwardDict)
				{
					this._XinHunAwardDict[taskID] = nXinghun;
				}
				result = nXinghun;
			}
			return result;
		}

		
		public int FindCompDonate(GameClient client, int taskID)
		{
			int nCompDonate = -1;
			lock (this._CompDonateAwardDict)
			{
				if (this._CompDonateAwardDict.TryGetValue(taskID, out nCompDonate))
				{
					return nCompDonate;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = nCompDonate;
			}
			else
			{
				nCompDonate = systemTask.GetIntValue("AwardCompHonor", -1);
				lock (this._CompDonateAwardDict)
				{
					this._CompDonateAwardDict[taskID] = nCompDonate;
				}
				result = nCompDonate;
			}
			return result;
		}

		
		public int FindCompJunXian(GameClient client, int taskID)
		{
			int nCompJunXian = -1;
			lock (this._CompJunXianAwardDict)
			{
				if (this._CompJunXianAwardDict.TryGetValue(taskID, out nCompJunXian))
				{
					return nCompJunXian;
				}
			}
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = nCompJunXian;
			}
			else
			{
				nCompJunXian = systemTask.GetIntValue("AwardCompFeast", -1);
				lock (this._CompJunXianAwardDict)
				{
					this._CompJunXianAwardDict[taskID] = nCompJunXian;
				}
				result = nCompJunXian;
			}
			return result;
		}

		
		public void ClearAllDictionary()
		{
			lock (this._TaskAwardsDict)
			{
				this._TaskAwardsDict.Clear();
			}
			lock (this._OtherAwardsDict)
			{
				this._OtherAwardsDict.Clear();
			}
			lock (this._MoneyDict)
			{
				this._MoneyDict.Clear();
			}
			lock (this._ExperienceDict)
			{
				this._ExperienceDict.Clear();
			}
			lock (this._YinLiangDict)
			{
				this._YinLiangDict.Clear();
			}
			lock (this._BindYuanBaoDict)
			{
				this._BindYuanBaoDict.Clear();
			}
			lock (this._LingLiDict)
			{
				this._LingLiDict.Clear();
			}
			lock (this._BlessPointDict)
			{
				this._BlessPointDict.Clear();
			}
			lock (this._ZhenQiDict)
			{
				this._ZhenQiDict.Clear();
			}
			lock (this._LieShaDict)
			{
				this._LieShaDict.Clear();
			}
			lock (this._WuXingDict)
			{
				this._WuXingDict.Clear();
			}
			lock (this._NeedYuanBaoDict)
			{
				this._NeedYuanBaoDict.Clear();
			}
			lock (this._JunGongDict)
			{
				this._JunGongDict.Clear();
			}
			lock (this._RongYuDict)
			{
				this._RongYuDict.Clear();
			}
		}

		
		private long CalcLuaScript(GameClient client, int taskID, SystemXmlItem systemTask, string itemName)
		{
			if (null == systemTask)
			{
				if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
				{
					return -1L;
				}
			}
			long ret = -1L;
			string luaScriptFileName = systemTask.GetStringValue(itemName);
			long result2;
			if (string.IsNullOrEmpty(luaScriptFileName))
			{
				result2 = ret;
			}
			else
			{
				luaScriptFileName = DataHelper.CurrentDirectory + "scripts/tasks/" + luaScriptFileName;
				object[] result = Global.ExcuteLuaFunction(client, luaScriptFileName, "calcTaskAwards", null, null);
				if (result != null && result.Length > 0)
				{
					ret = (long)result[0];
				}
				result2 = ret;
			}
			return result2;
		}

		
		private Dictionary<int, List<AwardsItemData>> _TaskAwardsDict = new Dictionary<int, List<AwardsItemData>>();

		
		private Dictionary<int, List<AwardsItemData>> _OtherAwardsDict = new Dictionary<int, List<AwardsItemData>>();

		
		private Dictionary<int, int> _MoneyDict = new Dictionary<int, int>();

		
		private Dictionary<int, long> _ExperienceDict = new Dictionary<int, long>();

		
		private Dictionary<int, int> _YinLiangDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _BindYuanBaoDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _LingLiDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _BlessPointDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _ZhenQiDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _LieShaDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _WuXingDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _NeedYuanBaoDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _JunGongDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _RongYuDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _JingYuanDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _XinHunAwardDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _CompDonateAwardDict = new Dictionary<int, int>();

		
		private Dictionary<int, int> _CompJunXianAwardDict = new Dictionary<int, int>();
	}
}
