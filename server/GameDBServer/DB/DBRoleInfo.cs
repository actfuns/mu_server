using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;
using GameDBServer.Data.Tarot;
using GameDBServer.Logic;
using GameDBServer.Logic.FluorescentGem;
using GameDBServer.Logic.Rank;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	
	public class DBRoleInfo
	{
		
		
		
		public int RoleID { get; set; }

		
		
		
		public string UserID { get; set; }

		
		
		
		public string RoleName { get; set; }

		
		
		
		public int RoleSex { get; set; }

		
		
		
		public int Occupation { get; set; }

		
		
		
		public int Level { get; set; }

		
		
		
		public int RolePic { get; set; }

		
		
		
		public int Faction { get; set; }

		
		
		
		public int Money1 { get; set; }

		
		
		
		public int Money2 { get; set; }

		
		
		
		public long Experience { get; set; }

		
		
		
		public int PKMode { get; set; }

		
		
		
		public int PKValue { get; set; }

		
		
		
		public string Position { get; set; }

		
		
		
		public string RegTime { get; set; }

		
		
		
		public long LastTime { get; set; }

		
		
		
		public int BagNum { get; set; }

		
		
		
		public int RebornBagNum { get; set; }

		
		
		
		public string OtherName { get; set; }

		
		
		
		public string MainQuickBarKeys { get; set; }

		
		
		
		public string OtherQuickBarKeys { get; set; }

		
		
		
		public int LoginNum { get; set; }

		
		
		
		public int LeftFightSeconds { get; set; }

		
		
		
		public int ServerLineID { get; set; }

		
		
		
		public int HorseDbID { get; set; }

		
		
		
		public int PetDbID { get; set; }

		
		
		
		public int InterPower { get; set; }

		
		
		
		public int TotalOnlineSecs { get; set; }

		
		
		
		public int AntiAddictionSecs { get; set; }

		
		
		
		public long LogOffTime { get; set; }

		
		
		
		public long BiGuanTime { get; set; }

		
		
		
		public int YinLiang { get; set; }

		
		
		
		public int TotalJingMaiExp { get; set; }

		
		
		
		public int JingMaiExpNum { get; set; }

		
		
		
		public int LastHorseID { get; set; }

		
		
		
		public int DefaultSkillID { get; set; }

		
		
		
		public int AutoLifeV { get; set; }

		
		
		
		public int AutoMagicV { get; set; }

		
		
		
		public int NumSkillID { get; set; }

		
		
		
		public int MainTaskID { get; set; }

		
		
		
		public int PKPoint { get; set; }

		
		
		
		public int LianZhan { get; set; }

		
		
		
		public int KillBoss { get; set; }

		
		
		
		public long BattleNameStart { get; set; }

		
		
		
		public int BattleNameIndex { get; set; }

		
		
		
		public int CZTaskID { get; set; }

		
		
		
		public int BattleNum { get; set; }

		
		
		
		public int HeroIndex { get; set; }

		
		
		
		public int LoginDayID { get; set; }

		
		
		
		public int LoginDayNum { get; set; }

		
		
		
		public int ZoneID { get; set; }

		
		
		
		public string BHName { get; set; }

		
		
		
		public int BHVerify { get; set; }

		
		
		
		public int BHZhiWu { get; set; }

		
		
		
		public int BGDayID1 { get; set; }

		
		
		
		public int BGMoney { get; set; }

		
		
		
		public int BGDayID2 { get; set; }

		
		
		
		public int BGGoods { get; set; }

		
		
		
		public int BangGong { get; set; }

		
		
		
		public int HuangHou { get; set; }

		
		
		
		public int JieBiaoDayID { get; set; }

		
		
		
		public int JieBiaoDayNum { get; set; }

		
		
		
		public string UserName { get; set; }

		
		
		
		public int LastMailID { get; set; }

		
		
		
		public long OnceAwardFlag { get; set; }

		
		
		
		public int Gold { get; set; }

		
		
		
		public int BanChat { get; set; }

		
		
		
		public int BanLogin { get; set; }

		
		
		
		public int IsFlashPlayer { get; set; }

		
		
		
		public int ChangeLifeCount { get; set; }

		
		
		
		public int AdmiredCount { get; set; }

		
		
		
		public int CombatForce { get; set; }

		
		
		
		public int AutoAssignPropertyPoint { get; set; }

		
		
		
		public string PushMsgID { get; set; }

		
		
		
		public int VipAwardFlag { get; set; }

		
		
		
		public int VIPLevel { get; set; }

		
		
		
		public long store_yinliang { get; set; }

		
		
		
		public long store_money { get; set; }

		
		
		
		public int MagicSwordParam { get; set; }

		
		
		
		public UserRankValueCache RankValue
		{
			get
			{
				return this.rankValue;
			}
			set
			{
				this.rankValue = value;
			}
		}

		
		
		
		public long UpdateDBPositionTicks { get; set; }

		
		
		
		public long UpdateDBTimeTicks { get; set; }

		
		
		
		public long UpdateDBInterPowerTimeTicks { get; set; }

		
		
		
		public List<OldTaskData> OldTasks { get; set; }

		
		
		
		public List<TaskData> DoingTaskList { get; set; }

		
		
		
		public List<GoodsData> GoodsDataList { get; set; }

		
		
		
		public List<GoodsData> RebornGoodsDataList { get; set; }

		
		
		
		public List<GoodsLimitData> GoodsLimitDataList { get; set; }

		
		
		
		public List<FriendData> FriendDataList { get; set; }

		
		
		
		public List<HorseData> HorsesDataList { get; set; }

		
		
		
		public List<PetData> PetsDataList { get; set; }

		
		
		
		public long LastDJPointDataTikcs { get; set; }

		
		
		
		public DJPointData RoleDJPointData { get; set; }

		
		
		
		public List<JingMaiData> JingMaiDataList { get; set; }

		
		
		
		public List<SkillData> SkillDataList { get; set; }

		
		
		
		public List<BufferData> BufferDataList { get; set; }

		
		
		
		public List<DailyTaskData> MyDailyTaskDataList { get; set; }

		
		
		
		public DailyJingMaiData MyDailyJingMaiData { get; set; }

		
		
		
		public PortableBagData MyPortableBagData { get; set; }

		
		
		
		public RebornPortableBagData RebornGirdData { get; set; }

		
		
		
		public int RebornShowEquip { get; set; }

		
		
		
		public int RebornShowModel { get; set; }

		
		
		
		public bool ExistsMyHuodongData { get; set; }

		
		
		
		public HuodongData MyHuodongData { get; set; }

		
		
		
		public List<FuBenData> FuBenDataList { get; set; }

		
		
		
		public MarriageData MyMarriageData { get; set; }

		
		
		
		public Dictionary<int, int> MyMarryPartyJoinList { get; set; }

		
		
		
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic { get; set; }

		
		
		
		public RoleDailyData MyRoleDailyData { get; set; }

		
		
		
		public YaBiaoData MyYaBiaoData { get; set; }

		
		
		
		public long LastReferenceTicks
		{
			get
			{
				return this._LastReferenceTicks;
			}
			set
			{
				this._LastReferenceTicks = value;
			}
		}

		
		
		
		public Dictionary<int, int> PaiHangPosDict { get; set; }

		
		
		
		public List<VipDailyData> VipDailyDataList { get; set; }

		
		
		
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen { get; set; }

		
		
		
		public WingData MyWingData { get; set; }

		
		
		
		public Dictionary<int, int> PictureJudgeReferInfo { get; set; }

		
		
		
		public Dictionary<int, int> StarConstellationInfo { get; set; }

		
		
		
		public Dictionary<int, LingYuData> LingYuDict { get; set; }

		
		
		
		public GuardStatueDetail MyGuardStatueDetail { get; set; }

		
		
		
		public TalentData MyTalentData { get; set; }

		
		
		
		public string LastIP { get; set; }

		
		
		
		public List<int> GroupMailRecordList { get; set; }

		
		
		
		public MerlinGrowthSaveDBData MerlinData { get; set; }

		
		
		
		public FluorescentGemData FluorescentGemData { get; set; }

		
		
		
		public int FluorescentPoint { get; set; }

		
		
		
		public List<BuildingData> BuildingDataList { get; set; }

		
		
		
		public Dictionary<int, OrnamentData> OrnamentDataDict { get; set; }

		
		
		
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict { get; set; }

		
		
		
		public long BanTradeToTicks { get; set; }

		
		
		
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict { get; set; }

		
		
		
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict { get; set; }

		
		
		
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict { get; set; }

		
		
		
		public AlchemyDataDB AlchemyInfo { get; set; }

		
		
		
		public Dictionary<int, ShenJiFuWenData> ShenJiDict { get; set; }

		
		
		
		public TarotSystemData TarotData { get; set; }

		
		
		
		public List<FuWenTabData> FuWenTabList { get; set; }

		
		
		
		public List<TaoZhuangData> JueXingTaoZhuangList { get; set; }

		
		
		
		public List<MountData> MountList { get; set; }

		
		
		
		public RebornStampData RebornYinJi { get; set; }

		
		public static void DBTableRow2RoleInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, int index)
		{
			dbRoleInfo.RoleID = Convert.ToInt32(cmd.Table.Rows[index]["rid"]);
			dbRoleInfo.UserID = cmd.Table.Rows[index]["userid"].ToString();
			dbRoleInfo.RoleName = cmd.Table.Rows[index]["rname"].ToString();
			dbRoleInfo.RoleSex = Convert.ToInt32(cmd.Table.Rows[index]["sex"]);
			dbRoleInfo.Occupation = Convert.ToInt32(cmd.Table.Rows[index]["occupation"]);
			dbRoleInfo.Level = Convert.ToInt32(cmd.Table.Rows[index]["level"]);
			dbRoleInfo.RolePic = Convert.ToInt32(cmd.Table.Rows[index]["pic"]);
			dbRoleInfo.Faction = Convert.ToInt32(cmd.Table.Rows[index]["faction"]);
			dbRoleInfo.Money1 = Convert.ToInt32(cmd.Table.Rows[index]["money1"]);
			dbRoleInfo.Money2 = Convert.ToInt32(cmd.Table.Rows[index]["money2"]);
			dbRoleInfo.Experience = Convert.ToInt64(cmd.Table.Rows[index]["experience"]);
			dbRoleInfo.PKMode = Convert.ToInt32(cmd.Table.Rows[index]["pkmode"]);
			dbRoleInfo.PKValue = Convert.ToInt32(cmd.Table.Rows[index]["pkvalue"]);
			dbRoleInfo.Position = cmd.Table.Rows[index]["position"].ToString();
			dbRoleInfo.RegTime = cmd.Table.Rows[index]["regtime"].ToString();
			dbRoleInfo.LastTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["lasttime"].ToString());
			dbRoleInfo.BagNum = Convert.ToInt32(cmd.Table.Rows[index]["bagnum"]);
			dbRoleInfo.OtherName = cmd.Table.Rows[index]["othername"].ToString();
			dbRoleInfo.MainQuickBarKeys = cmd.Table.Rows[index]["main_quick_keys"].ToString();
			dbRoleInfo.OtherQuickBarKeys = cmd.Table.Rows[index]["other_quick_keys"].ToString();
			dbRoleInfo.LoginNum = Convert.ToInt32(cmd.Table.Rows[index]["loginnum"].ToString());
			dbRoleInfo.LeftFightSeconds = Convert.ToInt32(cmd.Table.Rows[index]["leftfightsecs"].ToString());
			dbRoleInfo.HorseDbID = Convert.ToInt32(cmd.Table.Rows[index]["horseid"].ToString());
			dbRoleInfo.PetDbID = Convert.ToInt32(cmd.Table.Rows[index]["petid"].ToString());
			dbRoleInfo.InterPower = Convert.ToInt32(cmd.Table.Rows[index]["interpower"].ToString());
			dbRoleInfo.TotalOnlineSecs = Convert.ToInt32(cmd.Table.Rows[index]["totalonlinesecs"].ToString());
			dbRoleInfo.AntiAddictionSecs = Convert.ToInt32(cmd.Table.Rows[index]["antiaddictionsecs"].ToString());
			dbRoleInfo.LogOffTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["logofftime"].ToString());
			dbRoleInfo.BiGuanTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["biguantime"].ToString());
			dbRoleInfo.YinLiang = Convert.ToInt32(cmd.Table.Rows[index]["yinliang"].ToString());
			dbRoleInfo.TotalJingMaiExp = Convert.ToInt32(cmd.Table.Rows[index]["total_jingmai_exp"].ToString());
			dbRoleInfo.JingMaiExpNum = Convert.ToInt32(cmd.Table.Rows[index]["jingmai_exp_num"].ToString());
			dbRoleInfo.LastHorseID = Convert.ToInt32(cmd.Table.Rows[index]["lasthorseid"].ToString());
			dbRoleInfo.DefaultSkillID = Convert.ToInt32(cmd.Table.Rows[index]["skillid"].ToString());
			dbRoleInfo.AutoLifeV = Convert.ToInt32(cmd.Table.Rows[index]["autolife"].ToString());
			dbRoleInfo.AutoMagicV = Convert.ToInt32(cmd.Table.Rows[index]["automagic"].ToString());
			dbRoleInfo.NumSkillID = Convert.ToInt32(cmd.Table.Rows[index]["numskillid"].ToString());
			dbRoleInfo.MainTaskID = Convert.ToInt32(cmd.Table.Rows[index]["maintaskid"].ToString());
			dbRoleInfo.PKPoint = Convert.ToInt32(cmd.Table.Rows[index]["pkpoint"].ToString());
			dbRoleInfo.LianZhan = Convert.ToInt32(cmd.Table.Rows[index]["lianzhan"].ToString());
			dbRoleInfo.KillBoss = Convert.ToInt32(cmd.Table.Rows[index]["killboss"].ToString());
			dbRoleInfo.BattleNameStart = Convert.ToInt64(cmd.Table.Rows[index]["battlenamestart"].ToString());
			dbRoleInfo.BattleNameIndex = Convert.ToInt32(cmd.Table.Rows[index]["battlenameindex"].ToString());
			dbRoleInfo.CZTaskID = Convert.ToInt32(cmd.Table.Rows[index]["cztaskid"].ToString());
			dbRoleInfo.BattleNum = Convert.ToInt32(cmd.Table.Rows[index]["battlenum"].ToString());
			dbRoleInfo.HeroIndex = Convert.ToInt32(cmd.Table.Rows[index]["heroindex"].ToString());
			dbRoleInfo.LoginDayID = Convert.ToInt32(cmd.Table.Rows[index]["logindayid"].ToString());
			dbRoleInfo.LoginDayNum = Convert.ToInt32(cmd.Table.Rows[index]["logindaynum"].ToString());
			dbRoleInfo.ZoneID = Convert.ToInt32(cmd.Table.Rows[index]["zoneid"].ToString());
			dbRoleInfo.BHName = cmd.Table.Rows[index]["bhname"].ToString();
			dbRoleInfo.BHVerify = Convert.ToInt32(cmd.Table.Rows[index]["bhverify"].ToString());
			dbRoleInfo.BHZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["bhzhiwu"].ToString());
			dbRoleInfo.BGDayID1 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid1"].ToString());
			dbRoleInfo.BGMoney = Convert.ToInt32(cmd.Table.Rows[index]["bgmoney"].ToString());
			dbRoleInfo.BGDayID2 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid2"].ToString());
			dbRoleInfo.BGGoods = Convert.ToInt32(cmd.Table.Rows[index]["bggoods"].ToString());
			dbRoleInfo.BangGong = Convert.ToInt32(cmd.Table.Rows[index]["banggong"].ToString());
			dbRoleInfo.HuangHou = Convert.ToInt32(cmd.Table.Rows[index]["huanghou"].ToString());
			dbRoleInfo.JieBiaoDayID = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaodayid"].ToString());
			dbRoleInfo.JieBiaoDayNum = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaonum"].ToString());
			dbRoleInfo.UserName = cmd.Table.Rows[index]["username"].ToString();
			dbRoleInfo.LastMailID = Convert.ToInt32(cmd.Table.Rows[index]["lastmailid"].ToString());
			dbRoleInfo.OnceAwardFlag = Convert.ToInt64(cmd.Table.Rows[index]["onceawardflag"].ToString());
			dbRoleInfo.Gold = Convert.ToInt32(cmd.Table.Rows[index]["money2"].ToString());
			dbRoleInfo.BanChat = Convert.ToInt32(cmd.Table.Rows[index]["banchat"].ToString());
			dbRoleInfo.BanLogin = Convert.ToInt32(cmd.Table.Rows[index]["banlogin"].ToString());
			dbRoleInfo.IsFlashPlayer = Convert.ToInt32(cmd.Table.Rows[index]["isflashplayer"].ToString());
			dbRoleInfo.ChangeLifeCount = Convert.ToInt32(cmd.Table.Rows[index]["changelifecount"].ToString());
			dbRoleInfo.AdmiredCount = Convert.ToInt32(cmd.Table.Rows[index]["admiredcount"].ToString());
			dbRoleInfo.CombatForce = Convert.ToInt32(cmd.Table.Rows[index]["combatforce"].ToString());
			dbRoleInfo.AutoAssignPropertyPoint = Convert.ToInt32(cmd.Table.Rows[index]["autoassignpropertypoint"].ToString());
			dbRoleInfo.store_yinliang = Convert.ToInt64(cmd.Table.Rows[index]["store_yinliang"]);
			dbRoleInfo.store_money = Convert.ToInt64(cmd.Table.Rows[index]["store_money"]);
			dbRoleInfo.MagicSwordParam = Convert.ToInt32(cmd.Table.Rows[index]["magic_sword_param"]);
			dbRoleInfo.FluorescentPoint = Convert.ToInt32(cmd.Table.Rows[index]["fluorescent_point"]);
			dbRoleInfo.BanTradeToTicks = Convert.ToInt64(cmd.Table.Rows[index]["ban_trade_to_ticks"].ToString());
			dbRoleInfo.JunTuanZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["juntuanzhiwu"]);
			dbRoleInfo.HuiJiData.huiji = Convert.ToInt32(cmd.Table.Rows[index]["huiji"]);
			dbRoleInfo.HuiJiData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["huijiexp"]);
			dbRoleInfo.ArmorData.Armor = Convert.ToInt32(cmd.Table.Rows[index]["armor"]);
			dbRoleInfo.ArmorData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["armorexp"]);
			dbRoleInfo.BianShenData.BianShen = Convert.ToInt32(cmd.Table.Rows[index]["bianshen"]);
			dbRoleInfo.BianShenData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["bianshenexp"]);
			dbRoleInfo.RebornBagNum = Convert.ToInt32(cmd.Table.Rows[index]["reborn_bagnum"]);
			dbRoleInfo.RebornShowEquip = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow"]);
			dbRoleInfo.RebornShowModel = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow_model"]);
			dbRoleInfo.ZhanDuiID = Convert.ToInt32(cmd.Table.Rows[index]["zhanduiid"]);
			dbRoleInfo.ZhanDuiZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["zhanduizhiwu"]);
		}

		
		public static void DBTableRow2RoleInfo_Params(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, bool normalOnly)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> dict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					RoleParamsData roleParamsData = new RoleParamsData
					{
						ParamName = cmd.Table.Rows[i]["pname"].ToString(),
						ParamValue = cmd.Table.Rows[i]["pvalue"].ToString()
					};
					roleParamsData.ParamType = RoleParamNameInfo.GetRoleParamType(roleParamsData.ParamName, roleParamsData.ParamValue);
					if (roleParamsData.ParamType.Type <= 0 || !normalOnly)
					{
						dict[roleParamsData.ParamName] = roleParamsData;
					}
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_ParamsEx(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> dict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int idx = Convert.ToInt32(cmd.Table.Rows[i]["idx"].ToString());
					int columnCount = cmd.Table.Rows[i].ItemArray.Length;
					for (int columnIndex = 2; columnIndex < columnCount; columnIndex++)
					{
						RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(idx, columnIndex - 2);
						if (null != roleParamType)
						{
							RoleParamsData roleParamsData = new RoleParamsData
							{
								ParamName = roleParamType.ParamName,
								ParamValue = cmd.Table.Rows[i][columnIndex].ToString(),
								ParamType = roleParamType
							};
							dict[roleParamsData.ParamName] = roleParamsData;
						}
					}
				}
			}
		}

		
		public static void InitFromRoleParams(DBRoleInfo dbRoleInfo)
		{
			string str = Global.GetRoleParamByName(dbRoleInfo, "20017");
			if (!string.IsNullOrEmpty(str))
			{
				string[] ids = str.Split(new char[]
				{
					'$'
				});
				foreach (string s in ids)
				{
					int occu;
					if (int.TryParse(s, out occu) && !dbRoleInfo.OccupationList.Contains(occu))
					{
						dbRoleInfo.OccupationList.Add(occu);
					}
				}
			}
			if (!dbRoleInfo.OccupationList.Contains(dbRoleInfo.Occupation))
			{
				dbRoleInfo.OccupationList.Insert(0, dbRoleInfo.Occupation);
			}
			str = Global.GetRoleParamByName(dbRoleInfo, "10213");
			if (!string.IsNullOrEmpty(str))
			{
				dbRoleInfo.SubOccupation = Global.SafeConvertToInt32(str, 10);
			}
		}

		
		public static void DBTableRow2RoleInfo_OldTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				List<OldTaskData> oldTasks = new List<OldTaskData>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					oldTasks.Add(new OldTaskData
					{
						TaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoCount = Convert.ToInt32(cmd.Table.Rows[i]["count"].ToString())
					});
				}
				dbRoleInfo.OldTasks = oldTasks;
			}
		}

		
		public static void DBTableRow2RoleInfo_DoingTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.DoingTaskList = new List<TaskData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.DoingTaskList.Add(new TaskData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString()),
						DoingTaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoingTaskVal1 = Convert.ToInt32(cmd.Table.Rows[i]["value1"].ToString()),
						DoingTaskVal2 = Convert.ToInt32(cmd.Table.Rows[i]["value2"].ToString()),
						DoingTaskFocus = Convert.ToInt32(cmd.Table.Rows[i]["focus"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						StarLevel = Convert.ToInt32(cmd.Table.Rows[i]["starlevel"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Goods(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsDataList = new List<GoodsData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GoodsData goodsData = new GoodsData
					{
						Id = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						Using = Convert.ToInt32(cmd.Table.Rows[i]["isusing"].ToString()),
						Forge_level = Convert.ToInt32(cmd.Table.Rows[i]["forge_level"].ToString()),
						Starttime = cmd.Table.Rows[i]["starttime"].ToString(),
						Endtime = cmd.Table.Rows[i]["endtime"].ToString(),
						Site = Convert.ToInt32(cmd.Table.Rows[i]["site"].ToString()),
						Quality = Convert.ToInt32(cmd.Table.Rows[i]["quality"].ToString()),
						Props = cmd.Table.Rows[i]["Props"].ToString(),
						GCount = Convert.ToInt32(cmd.Table.Rows[i]["gcount"].ToString()),
						Binding = Convert.ToInt32(cmd.Table.Rows[i]["binding"].ToString()),
						Jewellist = cmd.Table.Rows[i]["jewellist"].ToString(),
						BagIndex = Convert.ToInt32(cmd.Table.Rows[i]["bagindex"].ToString()),
						SaleMoney1 = Convert.ToInt32(cmd.Table.Rows[i]["salemoney1"].ToString()),
						SaleYuanBao = Convert.ToInt32(cmd.Table.Rows[i]["saleyuanbao"].ToString()),
						SaleYinPiao = Convert.ToInt32(cmd.Table.Rows[i]["saleyinpiao"].ToString()),
						AddPropIndex = Convert.ToInt32(cmd.Table.Rows[i]["addpropindex"].ToString()),
						BornIndex = Convert.ToInt32(cmd.Table.Rows[i]["bornindex"].ToString()),
						Lucky = Convert.ToInt32(cmd.Table.Rows[i]["lucky"].ToString()),
						Strong = Convert.ToInt32(cmd.Table.Rows[i]["strong"].ToString()),
						ExcellenceInfo = Convert.ToInt32(cmd.Table.Rows[i]["excellenceinfo"].ToString()),
						AppendPropLev = Convert.ToInt32(cmd.Table.Rows[i]["appendproplev"].ToString()),
						ChangeLifeLevForEquip = Convert.ToInt32(cmd.Table.Rows[i]["equipchangelife"].ToString()),
						JuHunID = Convert.ToInt32(cmd.Table.Rows[i]["juhun"].ToString())
					};
					string washpropsStr = cmd.Table.Rows[i]["washprops"].ToString();
					if (!string.IsNullOrEmpty(washpropsStr))
					{
						try
						{
							byte[] props = Convert.FromBase64String(washpropsStr);
							goodsData.WashProps = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);
						}
						catch
						{
						}
					}
					string ehinfopropsStr = cmd.Table.Rows[i]["ehinfo"].ToString();
					if (!string.IsNullOrEmpty(ehinfopropsStr))
					{
						try
						{
							byte[] props = Convert.FromBase64String(ehinfopropsStr);
							goodsData.ElementhrtsProps = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);
						}
						catch
						{
						}
					}
					dbRoleInfo.GoodsDataList.Add(goodsData);
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_GoodsLimit(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GoodsLimitDataList.Add(new GoodsLimitData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Friends(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FriendDataList = new List<FriendData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FriendDataList.Add(new FriendData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						OtherRoleID = Convert.ToInt32(cmd.Table.Rows[i]["otherid"].ToString()),
						FriendType = Convert.ToInt32(cmd.Table.Rows[i]["friendType"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Horses(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.HorsesDataList = new List<HorseData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.HorsesDataList.Add(new HorseData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						HorseID = Convert.ToInt32(cmd.Table.Rows[i]["horseid"].ToString()),
						BodyID = Convert.ToInt32(cmd.Table.Rows[i]["bodyid"].ToString()),
						PropsNum = cmd.Table.Rows[i]["propsNum"].ToString(),
						PropsVal = cmd.Table.Rows[i]["PropsVal"].ToString(),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[i]["failednum"].ToString()),
						JinJieTempTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["temptime"].ToString()),
						JinJieTempNum = Convert.ToInt32(cmd.Table.Rows[i]["tempnum"].ToString()),
						JinJieFailedDayID = Convert.ToInt32(cmd.Table.Rows[i]["faileddayid"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Pets(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.PetsDataList = new List<PetData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.PetsDataList.Add(new PetData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						PetID = Convert.ToInt32(cmd.Table.Rows[i]["petid"].ToString()),
						PetName = cmd.Table.Rows[i]["petname"].ToString(),
						PetType = Convert.ToInt32(cmd.Table.Rows[i]["pettype"].ToString()),
						FeedNum = Convert.ToInt32(cmd.Table.Rows[i]["feednum"].ToString()),
						ReAliveNum = Convert.ToInt32(cmd.Table.Rows[i]["realivenum"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						PetProps = cmd.Table.Rows[i]["props"].ToString(),
						Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_JingMais(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.JingMaiDataList = new List<JingMaiData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.JingMaiDataList.Add(new JingMaiData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						JingMaiID = Convert.ToInt32(cmd.Table.Rows[i]["jmid"].ToString()),
						JingMaiLevel = Convert.ToInt32(cmd.Table.Rows[i]["jmlevel"].ToString()),
						JingMaiBodyLevel = Convert.ToInt32(cmd.Table.Rows[i]["bodylevel"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Skills(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SkillDataList = new List<SkillData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.SkillDataList.Add(new SkillData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						SkillID = Convert.ToInt32(cmd.Table.Rows[i]["skillid"].ToString()),
						SkillLevel = Convert.ToInt32(cmd.Table.Rows[i]["skilllevel"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_Buffers(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BufferDataList = new List<BufferData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.BufferDataList.Add(new BufferData
					{
						BufferID = Convert.ToInt32(cmd.Table.Rows[i]["bufferid"].ToString()),
						StartTime = Convert.ToInt64(cmd.Table.Rows[i]["starttime"].ToString()),
						BufferSecs = Convert.ToInt32(cmd.Table.Rows[i]["buffersecs"].ToString()),
						BufferVal = Convert.ToInt64(cmd.Table.Rows[i]["bufferval"].ToString()),
						BufferType = 0
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_DailyTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MyDailyTaskDataList)
			{
				dbRoleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
			}
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				DailyTaskData dailyTaskData = new DailyTaskData
				{
					HuanID = Convert.ToInt32(cmd.Table.Rows[i]["huanid"].ToString()),
					RecTime = cmd.Table.Rows[i]["rectime"].ToString(),
					RecNum = Convert.ToInt32(cmd.Table.Rows[i]["recnum"].ToString()),
					TaskClass = Convert.ToInt32(cmd.Table.Rows[i]["taskClass"].ToString()),
					ExtDayID = Convert.ToInt32(cmd.Table.Rows[i]["extdayid"].ToString()),
					ExtNum = Convert.ToInt32(cmd.Table.Rows[i]["extnum"].ToString())
				};
				dbRoleInfo.MyDailyTaskDataList.Add(dailyTaskData);
			}
		}

		
		public static void DBTableRow2RoleInfo_DailyJingMai(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyDailyJingMaiData = new DailyJingMaiData
				{
					JmTime = cmd.Table.Rows[0]["jmtime"].ToString(),
					JmNum = Convert.ToInt32(cmd.Table.Rows[0]["jmnum"].ToString())
				};
			}
		}

		
		public static void DBTableRow2RoleInfo_PortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyPortableBagData = new PortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyPortableBagData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_RebornPortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.RebornGirdData = new RebornPortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.RebornGirdData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_HuodongData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ExistsMyHuodongData = false;
			dbRoleInfo.MyHuodongData = new HuodongData();
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.ExistsMyHuodongData = true;
				dbRoleInfo.MyHuodongData.LastWeekID = cmd.Table.Rows[0]["loginweekid"].ToString();
				dbRoleInfo.MyHuodongData.LastDayID = cmd.Table.Rows[0]["logindayid"].ToString();
				dbRoleInfo.MyHuodongData.LoginNum = Convert.ToInt32(cmd.Table.Rows[0]["loginnum"].ToString());
				dbRoleInfo.MyHuodongData.NewStep = Convert.ToInt32(cmd.Table.Rows[0]["newstep"].ToString());
				dbRoleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["steptime"].ToString());
				dbRoleInfo.MyHuodongData.LastMTime = Convert.ToInt32(cmd.Table.Rows[0]["lastmtime"].ToString());
				dbRoleInfo.MyHuodongData.CurMID = cmd.Table.Rows[0]["curmid"].ToString();
				dbRoleInfo.MyHuodongData.CurMTime = Convert.ToInt32(cmd.Table.Rows[0]["curmtime"].ToString());
				dbRoleInfo.MyHuodongData.SongLiID = Convert.ToInt32(cmd.Table.Rows[0]["songliid"].ToString());
				dbRoleInfo.MyHuodongData.LoginGiftState = Convert.ToInt32(cmd.Table.Rows[0]["logingiftstate"].ToString());
				dbRoleInfo.MyHuodongData.OnlineGiftState = Convert.ToInt32(cmd.Table.Rows[0]["onlinegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimehuodongid"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeDayID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimedayid"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeLoginNum = Convert.ToInt32(cmd.Table.Rows[0]["limittimeloginnum"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeGiftState = Convert.ToInt32(cmd.Table.Rows[0]["limittimegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["everydayonlineawardstep"].ToString());
				dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["geteverydayonlineawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["serieslogingetawardstep"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["seriesloginawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID = cmd.Table.Rows[0]["seriesloginawardgoodsid"].ToString();
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = cmd.Table.Rows[0]["everydayonlineawardgoodsid"].ToString();
			}
		}

		
		public static void DBTableRow2RoleInfo_FuBenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FuBenDataList = new List<FuBenData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FuBenDataList.Add(new FuBenData
					{
						FuBenID = Convert.ToInt32(cmd.Table.Rows[i]["fubenid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						EnterNum = Convert.ToInt32(cmd.Table.Rows[i]["enternum"].ToString()),
						QuickPassTimer = Convert.ToInt32(cmd.Table.Rows[i]["quickpasstimer"].ToString()),
						FinishNum = Convert.ToInt32(cmd.Table.Rows[i]["finishnum"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_HolyItemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyHolyItemDataDic = new Dictionary<sbyte, HolyItemData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					HolyItemData data = null;
					sbyte sShengwuType = Convert.ToSByte(cmd.Table.Rows[i]["shengwu_type"].ToString());
					sbyte sPartSlot = Convert.ToSByte(cmd.Table.Rows[i]["part_slot"].ToString());
					bool bFind = dbRoleInfo.MyHolyItemDataDic.TryGetValue(sShengwuType, out data);
					if (!bFind)
					{
						data = new HolyItemData();
					}
					data.m_sType = sShengwuType;
					HolyItemPartData partdata = null;
					if (!data.m_PartArray.TryGetValue(sPartSlot, out partdata))
					{
						partdata = new HolyItemPartData();
						data.m_PartArray.Add(sPartSlot, partdata);
					}
					partdata.m_sSuit = Convert.ToSByte(cmd.Table.Rows[i]["part_suit"].ToString());
					partdata.m_nSlice = Convert.ToInt32(cmd.Table.Rows[i]["part_slice"].ToString());
					if (!bFind)
					{
						dbRoleInfo.MyHolyItemDataDic.Add(sShengwuType, data);
					}
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_TarotData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int roleId)
		{
			dbRoleInfo.TarotData = new TarotSystemData();
			string str = string.Format("SELECT tarotinfo,kingbuff FROM t_tarot where roleid={0}", roleId);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", str), EventLevels.Important);
			try
			{
				MySQLCommand command = new MySQLCommand(str, connection);
				MySQLDataReader reader = command.ExecuteReaderEx();
				while (reader.Read())
				{
					string str2 = Encoding.UTF8.GetString(reader["tarotinfo"] as byte[]);
					string str3 = Encoding.UTF8.GetString(reader["kingbuff"] as byte[]);
					string[] strArray = str2.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string str4 in strArray)
					{
						TarotCardData item = new TarotCardData(str4);
						dbRoleInfo.TarotData.TarotCardDatas.Add(item);
					}
					dbRoleInfo.TarotData.KingData = new TarotKingData(str3);
				}
				command.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_MarriageData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyMarriageData = new MarriageData
				{
					nSpouseID = Convert.ToInt32(cmd.Table.Rows[0]["spouseid"].ToString()),
					byMarrytype = Convert.ToSByte(cmd.Table.Rows[0]["marrytype"].ToString()),
					nRingID = Convert.ToInt32(cmd.Table.Rows[0]["ringid"].ToString()),
					nGoodwillexp = Convert.ToInt32(cmd.Table.Rows[0]["goodwillexp"].ToString()),
					byGoodwillstar = Convert.ToSByte(cmd.Table.Rows[0]["goodwillstar"].ToString()),
					byGoodwilllevel = Convert.ToSByte(cmd.Table.Rows[0]["goodwilllevel"].ToString()),
					nGivenrose = Convert.ToInt32(cmd.Table.Rows[0]["givenrose"].ToString()),
					strLovemessage = cmd.Table.Rows[0]["lovemessage"].ToString(),
					byAutoReject = Convert.ToSByte(cmd.Table.Rows[0]["autoreject"].ToString()),
					ChangTime = cmd.Table.Rows[0]["changtime"].ToString()
				};
			}
			else
			{
				dbRoleInfo.MyMarriageData = new MarriageData();
			}
		}

		
		public static void DBTableRow2RoleInfo_MarryPartyJoinList(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyMarryPartyJoinList = new Dictionary<int, int>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MyMarryPartyJoinList.Add(Convert.ToInt32(cmd.Table.Rows[i]["partyroleid"].ToString()), Convert.ToInt32(cmd.Table.Rows[i]["joincount"].ToString()));
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_DailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyRoleDailyData = new RoleDailyData
				{
					ExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["expdayid"].ToString()),
					TodayExp = Convert.ToInt32(cmd.Table.Rows[0]["todayexp"].ToString()),
					LingLiDayID = Convert.ToInt32(cmd.Table.Rows[0]["linglidayid"].ToString()),
					TodayLingLi = Convert.ToInt32(cmd.Table.Rows[0]["todaylingli"].ToString()),
					KillBossDayID = Convert.ToInt32(cmd.Table.Rows[0]["killbossdayid"].ToString()),
					TodayKillBoss = Convert.ToInt32(cmd.Table.Rows[0]["todaykillboss"].ToString()),
					FuBenDayID = Convert.ToInt32(cmd.Table.Rows[0]["fubendayid"].ToString()),
					TodayFuBenNum = Convert.ToInt32(cmd.Table.Rows[0]["todayfubennum"].ToString()),
					WuXingDayID = Convert.ToInt32(cmd.Table.Rows[0]["wuxingdayid"].ToString()),
					WuXingNum = Convert.ToInt32(cmd.Table.Rows[0]["wuxingnum"].ToString()),
					RebornExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["reborndayid"].ToString()),
					RebornExpMonster = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpmonster"].ToString()),
					RebornExpSale = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpsale"].ToString())
				};
			}
		}

		
		public static void DBTableRow2RoleInfo_YaBiaoData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyYaBiaoData = new YaBiaoData
				{
					YaBiaoID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaoid"].ToString()),
					StartTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["starttime"].ToString()),
					State = Convert.ToInt32(cmd.Table.Rows[0]["state"].ToString()),
					LineID = Convert.ToInt32(cmd.Table.Rows[0]["lineid"].ToString()),
					TouBao = Convert.ToInt32(cmd.Table.Rows[0]["toubao"].ToString()),
					YaBiaoDayID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaodayid"].ToString()),
					YaBiaoNum = Convert.ToInt32(cmd.Table.Rows[0]["yabiaonum"].ToString()),
					TakeGoods = Convert.ToInt32(cmd.Table.Rows[0]["takegoods"].ToString())
				};
			}
		}

		
		public static void DBTableRow2RoleInfo_VipDailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.VipDailyDataList = new List<VipDailyData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.VipDailyDataList.Add(new VipDailyData
					{
						PriorityType = Convert.ToInt32(cmd.Table.Rows[i]["prioritytype"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedTimes = Convert.ToInt32(cmd.Table.Rows[i]["usedtimes"].ToString())
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_YangGongBKDailyJiFenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData
				{
					JiFen = Convert.ToInt32(cmd.Table.Rows[0]["jifen"].ToString()),
					DayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString()),
					AwardHistory = Convert.ToInt64(cmd.Table.Rows[0]["awardhistory"].ToString())
				};
			}
		}

		
		public static void DBTableRow2RoleInfo_Wings(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyWingData = new WingData
				{
					DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
					WingID = Convert.ToInt32(cmd.Table.Rows[0]["wingid"].ToString()),
					ForgeLevel = Convert.ToInt32(cmd.Table.Rows[0]["forgeLevel"].ToString()),
					AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addtime"].ToString()),
					JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[0]["failednum"].ToString()),
					Using = Convert.ToInt32(cmd.Table.Rows[0]["equiped"].ToString()),
					StarExp = Convert.ToInt32(cmd.Table.Rows[0]["starexp"].ToString()),
					ZhuLingNum = Convert.ToInt32(cmd.Table.Rows[0]["zhulingnum"].ToString()),
					ZhuHunNum = Convert.ToInt32(cmd.Table.Rows[0]["zhuhunnum"].ToString())
				};
			}
		}

		
		public static void DBTableRow2RoleInfo_picturejudgeinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int nID = Convert.ToInt32(cmd.Table.Rows[i]["picturejudgeid"].ToString());
					int nNum = Convert.ToInt32(cmd.Table.Rows[i]["refercount"].ToString());
					Tmpdict[nID] = nNum;
				}
				dbRoleInfo.PictureJudgeReferInfo = Tmpdict;
			}
		}

		
		public static void DBTableRow2RoleInfo_starconstellationinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int nStarsiteid = Convert.ToInt32(cmd.Table.Rows[i]["starsiteid"].ToString());
					int nStarslotid = Convert.ToInt32(cmd.Table.Rows[i]["starslotid"].ToString());
					Tmpdict[nStarsiteid] = nStarslotid;
				}
				dbRoleInfo.StarConstellationInfo = Tmpdict;
			}
		}

		
		public static void DBTableRow2RoleInfo_LingYuInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.LingYuDict = new Dictionary<int, LingYuData>();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				LingYuData lyData = new LingYuData();
				lyData.Type = Convert.ToInt32(cmd.Table.Rows[i]["type"].ToString());
				lyData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
				lyData.Suit = Convert.ToInt32(cmd.Table.Rows[i]["suit"].ToString());
				dbRoleInfo.LingYuDict[lyData.Type] = lyData;
			}
		}

		
		public static void DBTableRow2RoleInfo_GuardStatue(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Level = Convert.ToInt32(cmd.Table.Rows[0]["level"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Suit = Convert.ToInt32(cmd.Table.Rows[0]["suit"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.HasGuardPoint = Convert.ToInt32(cmd.Table.Rows[0]["total_guard_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.ActiveSoulSlot = Convert.ToInt32(cmd.Table.Rows[0]["slot_cnt"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverPoint = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverOffset = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_offset"].ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_GuardSoul(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Clear();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GuardSoulData data = new GuardSoulData();
					data.Type = Convert.ToInt32(cmd.Table.Rows[i]["soul_type"].ToString());
					data.EquipSlot = Convert.ToInt32(cmd.Table.Rows[i]["equip_slot"].ToString());
					dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(data);
				}
			}
		}

		
		public static int QueryRoleID_ByRolename(MySQLConnection conn, string strRoleName)
		{
			List<Tuple<int, string>> idList = DBRoleInfo.QueryRoleIdList_ByRolename_IgnoreDbCmp(conn, strRoleName);
			int roleId = -1;
			if (idList != null)
			{
				Tuple<int, string> tuple = idList.Find((Tuple<int, string> _t) => _t.Item2 == strRoleName);
				roleId = ((tuple != null) ? tuple.Item1 : -1);
			}
			return roleId;
		}

		
		public static List<Tuple<int, string>> QueryRoleIdList_ByRolename_IgnoreDbCmp(MySQLConnection conn, string rolename)
		{
			List<Tuple<int, string>> resultList = new List<Tuple<int, string>>();
			string sql = string.Format("SELECT rid,rname FROM t_roles where rname='{0}'", rolename);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
			MySQLCommand cmd = new MySQLCommand(sql, conn);
			MySQLDataReader reader = cmd.ExecuteReaderEx();
			while (reader.Read())
			{
				int oneRoleId = Convert.ToInt32(reader["rid"].ToString());
				string oneRolename = reader["rname"].ToString();
				resultList.Add(new Tuple<int, string>(oneRoleId, oneRolename));
			}
			cmd.Dispose();
			return resultList;
		}

		
		public static void DBTableRow2RoleInfo_GMailInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GroupMailRecordList = new List<int>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GroupMailRecordList.Add(Convert.ToInt32(cmd.Table.Rows[i]["gmailid"].ToString()));
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_TalentBase(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyTalentData = new TalentData
				{
					IsOpen = true,
					TotalCount = Convert.ToInt32(cmd.Table.Rows[0]["tatalCount"].ToString()),
					Exp = Convert.ToInt64(cmd.Table.Rows[0]["exp"].ToString())
				};
			}
			else
			{
				dbRoleInfo.MyTalentData = new TalentData();
			}
		}

		
		public static void DBTableRow2RoleInfo_TalentEffects(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			Dictionary<int, int> countList = new Dictionary<int, int>();
			countList.Add(1, 0);
			countList.Add(2, 0);
			countList.Add(3, 0);
			dbRoleInfo.MyTalentData.EffectList = new List<TalentEffectItem>();
			if (dbRoleInfo.MyTalentData != null && cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int type = Convert.ToInt32(cmd.Table.Rows[i]["talentType"].ToString());
					int id = Convert.ToInt32(cmd.Table.Rows[i]["effectID"].ToString());
					int level = Convert.ToInt32(cmd.Table.Rows[i]["effectLevel"].ToString());
					TalentEffectItem item = new TalentEffectItem
					{
						ID = id,
						Level = level,
						TalentType = type
					};
					dbRoleInfo.MyTalentData.EffectList.Add(item);
					Dictionary<int, int> dictionary;
					int key;
					(dictionary = countList)[key = type] = dictionary[key] + level;
				}
			}
			dbRoleInfo.MyTalentData.CountList = countList;
		}

		
		public static void DBTableRow2RoleInfo_TianTiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			RoleTianTiData roleTianTiData = new RoleTianTiData
			{
				RoleId = dbRoleInfo.RoleID
			};
			dbRoleInfo.TianTiData = roleTianTiData;
			if (cmd.Table.Rows.Count > 0)
			{
				roleTianTiData.DuanWeiId = Convert.ToInt32(cmd.Table.Rows[0]["duanweiid"].ToString());
				roleTianTiData.DuanWeiJiFen = Convert.ToInt32(cmd.Table.Rows[0]["duanweijifen"].ToString());
				roleTianTiData.DuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["duanweirank"].ToString());
				roleTianTiData.LianSheng = Convert.ToInt32(cmd.Table.Rows[0]["liansheng"].ToString());
				roleTianTiData.FightCount = Convert.ToInt32(cmd.Table.Rows[0]["fightcount"].ToString());
				roleTianTiData.SuccessCount = Convert.ToInt32(cmd.Table.Rows[0]["successcount"].ToString());
				roleTianTiData.TodayFightCount = Convert.ToInt32(cmd.Table.Rows[0]["todayfightcount"].ToString());
				roleTianTiData.LastFightDayId = Convert.ToInt32(cmd.Table.Rows[0]["lastfightdayid"].ToString());
				roleTianTiData.MonthDuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["monthduanweirank"].ToString());
				DateTime.TryParse(cmd.Table.Rows[0]["fetchmonthawarddate"].ToString(), out roleTianTiData.FetchMonthDuanWeiRankAwardsTime);
				roleTianTiData.RongYao = Convert.ToInt32(cmd.Table.Rows[0]["rongyao"].ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_MerlinData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MerlinData)
			{
				dbRoleInfo.MerlinData = new MerlinGrowthSaveDBData();
				for (int i = 0; i < 4; i++)
				{
					dbRoleInfo.MerlinData._ActiveAttr[i] = 0.0;
					dbRoleInfo.MerlinData._UnActiveAttr[i] = 0.0;
				}
			}
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MerlinData._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[0]["roleID"].ToString(), 10);
				dbRoleInfo.MerlinData._Occupation = Global.SafeConvertToInt32(cmd.Table.Rows[0]["occupation"].ToString(), 10);
				dbRoleInfo.MerlinData._Level = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level"].ToString(), 10);
				dbRoleInfo.MerlinData._LevelUpFailNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level_up_fail_num"].ToString(), 10);
				dbRoleInfo.MerlinData._StarNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starNum"].ToString(), 10);
				dbRoleInfo.MerlinData._StarExp = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starExp"].ToString(), 10);
				dbRoleInfo.MerlinData._LuckyPoint = Global.SafeConvertToInt32(cmd.Table.Rows[0]["luckyPoint"].ToString(), 10);
				dbRoleInfo.MerlinData._ToTicks = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["toTicks"].ToString());
				dbRoleInfo.MerlinData._AddTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addTime"].ToString());
				dbRoleInfo.MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeBlow"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActivePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveBlow"].ToString(), 10) / 100);
			}
		}

		
		public static void DBTableRow2RoleInfo_FluorescentGemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.FluorescentGemData)
			{
				dbRoleInfo.FluorescentGemData = new FluorescentGemData();
			}
			dbRoleInfo.FluorescentGemData.GemBagList.Clear();
			dbRoleInfo.FluorescentGemData.GemEquipList.Clear();
			HashSet<int> repeatedSet = new HashSet<int>();
			FluorescentGemSaveDBData data = new FluorescentGemSaveDBData();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				ulong id = Convert.ToUInt64(cmd.Table.Rows[i]["id"].ToString());
				data._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["roleid"].ToString(), 10);
				data._GoodsID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["goodsid"].ToString(), 10);
				data._Position = Global.SafeConvertToInt32(cmd.Table.Rows[i]["position"].ToString(), 10);
				data._GemType = Global.SafeConvertToInt32(cmd.Table.Rows[i]["type"].ToString(), 10);
				data._Bind = Global.SafeConvertToInt32(cmd.Table.Rows[i]["bind"].ToString(), 10);
				int slot = FluorescentGemManager.getInstance().GenerateBagIndex(data._Position, data._GemType);
				if (!repeatedSet.Contains(slot))
				{
					GoodsData tmpGoods = new GoodsData();
					tmpGoods.GoodsID = data._GoodsID;
					tmpGoods.GCount = 1;
					tmpGoods.Binding = data._Bind;
					tmpGoods.Site = 7001;
					repeatedSet.Add(slot);
					tmpGoods.BagIndex = slot;
					dbRoleInfo.FluorescentGemData.GemEquipList.Add(tmpGoods);
				}
				else
				{
					FluorescentGemDBOperate.ForceUnEquipFluorescentGem(DBManager.getInstance(), id);
					LogManager.WriteLog(LogTypes.Error, string.Format("荧光宝石装备栏位置重复，强制删除，rid={0}, goodsid={1}, pos={2}, type={3}, bind={4}", new object[]
					{
						data._RoleID,
						data._GoodsID,
						data._Position,
						data._GemType,
						data._Bind
					}), null, true);
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_BuildingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BuildingDataList = new List<BuildingData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					BuildingData myBuild = new BuildingData();
					myBuild.BuildId = Convert.ToInt32(cmd.Table.Rows[i]["buildid"].ToString());
					myBuild.TaskID_1 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_1"].ToString());
					myBuild.TaskID_2 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_2"].ToString());
					myBuild.TaskID_3 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_3"].ToString());
					myBuild.TaskID_4 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_4"].ToString());
					myBuild.BuildLev = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					myBuild.BuildExp = Convert.ToInt32(cmd.Table.Rows[i]["exp"].ToString());
					myBuild.BuildTime = cmd.Table.Rows[i]["developtime"].ToString();
					dbRoleInfo.BuildingDataList.Add(myBuild);
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_OrnamentData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.OrnamentDataDict = new Dictionary<int, OrnamentData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					OrnamentData myOrnament = new OrnamentData();
					myOrnament.ID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString());
					myOrnament.Param1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					myOrnament.Param2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					dbRoleInfo.OrnamentDataDict[myOrnament.ID] = myOrnament;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_SevenDayActData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.SevenDayActDict = new Dictionary<int, Dictionary<int, SevenDayItemData>>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SevenDayItemData itemData = new SevenDayItemData();
					itemData.AwardFlag = Convert.ToInt32(cmd.Table.Rows[i]["award_flag"].ToString());
					itemData.Params1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					itemData.Params2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					int roleid = Convert.ToInt32(cmd.Table.Rows[i]["roleid"].ToString());
					int actType = Convert.ToInt32(cmd.Table.Rows[i]["act_type"].ToString());
					int id = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString());
					Dictionary<int, SevenDayItemData> itemDict = null;
					if (!dbRoleInfo.SevenDayActDict.TryGetValue(actType, out itemDict))
					{
						itemDict = new Dictionary<int, SevenDayItemData>();
						dbRoleInfo.SevenDayActDict[actType] = itemDict;
					}
					itemDict[id] = itemData;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_SpecialActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecActInfoDB itemData = new SpecActInfoDB();
					itemData.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					itemData.Active = Convert.ToInt16(cmd.Table.Rows[i]["active"].ToString());
					dbRoleInfo.SpecActInfoDict[itemData.ActID] = itemData;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_SpecialPriorityActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecPriorityActInfoDB itemData = new SpecPriorityActInfoDB();
					itemData.TeQuanID = Convert.ToInt32(cmd.Table.Rows[i]["tequanid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(itemData.TeQuanID, itemData.ActID);
					dbRoleInfo.SpecPriorityActInfoDict[kvpKey] = itemData;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_EverydayActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					EverydayActInfoDB itemData = new EverydayActInfoDB();
					itemData.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					itemData.ActiveDay = (int)Convert.ToInt16(cmd.Table.Rows[i]["activeDay"].ToString());
					dbRoleInfo.EverydayActInfoDict[itemData.ActID] = itemData;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_AlchemyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.AlchemyInfo = new AlchemyDataDB();
				dbRoleInfo.AlchemyInfo.RoleID = dbRoleInfo.RoleID;
				dbRoleInfo.AlchemyInfo.BaseData.Element = Convert.ToInt32(cmd.Table.Rows[0]["element"].ToString());
				dbRoleInfo.AlchemyInfo.ElementDayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString());
				dbRoleInfo.AlchemyInfo.rollbackType = cmd.Table.Rows[0]["rollback"].ToString();
				string alchemyValue = cmd.Table.Rows[0]["value"].ToString();
				if (!string.IsNullOrEmpty(alchemyValue))
				{
					string[] alchemyFields = alchemyValue.Split(new char[]
					{
						'|'
					});
					foreach (string item in alchemyFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.AlchemyValue[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
				string todayCost = cmd.Table.Rows[0]["todaycost"].ToString();
				if (!string.IsNullOrEmpty(todayCost))
				{
					string[] costFields = todayCost.Split(new char[]
					{
						'|'
					});
					foreach (string item in costFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.ToDayCost[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
				string histCost = cmd.Table.Rows[0]["histcost"].ToString();
				if (!string.IsNullOrEmpty(histCost))
				{
					string[] costFields = histCost.Split(new char[]
					{
						'|'
					});
					foreach (string item in costFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.HistCost[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_ShenJiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ShenJiDict = new Dictionary<int, ShenJiFuWenData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					ShenJiFuWenData itemData = new ShenJiFuWenData();
					itemData.ShenJiID = Convert.ToInt32(cmd.Table.Rows[i]["sjID"].ToString());
					itemData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					dbRoleInfo.ShenJiDict[itemData.ShenJiID] = itemData;
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_FuWenData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int rid)
		{
			dbRoleInfo.FuWenTabList = new List<FuWenTabData>();
			string str = string.Format("SELECT * FROM t_fuwen where rid={0}", rid);
			try
			{
				MySQLCommand command = new MySQLCommand(str, connection);
				MySQLDataReader reader = command.ExecuteReaderEx();
				while (reader.Read())
				{
					string fuwenEquip = Encoding.UTF8.GetString(reader["fuwenequip"] as byte[]);
					string shenShiEquip = reader["shenshiactive"].ToString();
					List<FuWenTabData> fuWenTabList = dbRoleInfo.FuWenTabList;
					FuWenTabData fuWenTabData = new FuWenTabData();
					fuWenTabData.TabID = Convert.ToInt32(reader["tabid"].ToString());
					fuWenTabData.Name = reader["name"].ToString();
					FuWenTabData fuWenTabData2 = fuWenTabData;
					List<int> fuWenEquipList;
					if (!(fuwenEquip == ""))
					{
						fuWenEquipList = Array.ConvertAll<string, int>(fuwenEquip.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						fuWenEquipList = new List<int>();
					}
					fuWenTabData2.FuWenEquipList = fuWenEquipList;
					FuWenTabData fuWenTabData3 = fuWenTabData;
					List<int> shenShiActiveList;
					if (!(shenShiEquip == ""))
					{
						shenShiActiveList = Array.ConvertAll<string, int>(shenShiEquip.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						shenShiActiveList = new List<int>();
					}
					fuWenTabData3.ShenShiActiveList = shenShiActiveList;
					fuWenTabData.SkillEquip = Convert.ToInt32(reader["skillequip"].ToString());
					fuWenTabData.OwnerID = Convert.ToInt32(reader["rid"].ToString());
					fuWenTabList.Add(fuWenTabData);
				}
				command.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public static void DBTableRow2RoleInfo_JueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.JueXingTaoZhuangList = new List<TaoZhuangData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					List<TaoZhuangData> jueXingTaoZhuangList = dbRoleInfo.JueXingTaoZhuangList;
					TaoZhuangData taoZhuangData = new TaoZhuangData();
					taoZhuangData.ID = Convert.ToInt32(cmd.Table.Rows[i]["suitid"].ToString());
					taoZhuangData.ActiviteList = Array.ConvertAll<string, int>(cmd.Table.Rows[i]["activite"].ToString().Split(new char[]
					{
						','
					}), (string x) => Convert.ToInt32(x)).ToList<int>();
					jueXingTaoZhuangList.Add(taoZhuangData);
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_ZuoQiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MountList = new List<MountData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MountList.Add(new MountData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						IsNew = (cmd.Table.Rows[i]["isnew"].ToString() == "1")
					});
				}
			}
		}

		
		public static void DBTableRow2RoleInfo_JingLingYuanSuJueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				JingLingYuanSuJueXingData data = new JingLingYuanSuJueXingData();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					data.ActiveType = Convert.ToInt32(cmd.Table.Rows[i]["activetype"].ToString());
					string str = cmd.Table.Rows[i]["activeids"].ToString();
					if (str.Length == 0)
					{
						return;
					}
					string[] arr = str.Split(new char[]
					{
						','
					});
					data.ActiveIDs = new int[arr.Length];
					for (int j = 0; j < arr.Length; j++)
					{
						int.TryParse(arr[j], out data.ActiveIDs[j]);
					}
				}
				dbRoleInfo.JingLingYuanSuJueXingData = data;
			}
		}

		
		public bool Query(MySQLConnection conn, int roleID, bool bUseIsdel = true, int tempRoleID = 0)
		{
			LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载角色数据: {0}", roleID), null, true);
			MySQLSelectCommand cmd;
			if (bUseIsdel)
			{
				string[] fields = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] tables = new string[]
				{
					"t_roles"
				};
				object[,] array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields, tables, array, null, null);
			}
			else
			{
				string[] fields2 = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] tables2 = new string[]
				{
					"t_roles"
				};
				object[,] array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields2, tables2, array, null, null);
			}
			bool result;
			if (cmd.Table.Rows.Count <= 0)
			{
				result = false;
			}
			else
			{
				this.PTID = GameDBManager.PTID;
				this.WorldRoleID = string.Format("{0}@{1}", roleID, this.PTID);
				DBRoleInfo.DBTableRow2RoleInfo(this, cmd, 0);
				object[,] array;
				if (GameDBManager.Flag_Splite_RoleParams_Table == 0)
				{
					string[] fields3 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] tables3 = new string[]
					{
						"t_roleparams"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields3, tables3, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, cmd, true);
				}
				else
				{
					string[] fields4 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] tables4 = new string[]
					{
						"t_roleparams_2"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields4, tables4, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, cmd, false);
					string[] fields5 = new string[]
					{
						"*"
					};
					string[] tables5 = new string[]
					{
						"t_roleparams_long"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields5, tables5, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, cmd);
					string[] fields6 = new string[]
					{
						"*"
					};
					string[] tables6 = new string[]
					{
						"t_roleparams_char"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields6, tables6, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, cmd);
				}
				DBRoleInfo.InitFromRoleParams(this);
				string[] fields7 = new string[]
				{
					"rid",
					"taskid",
					"count"
				};
				string[] tables7 = new string[]
				{
					"t_taskslog"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields7, tables7, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OldTasks(this, cmd);
				string[] fields8 = new string[]
				{
					"Id",
					"rid",
					"taskid",
					"focus",
					"value1",
					"value2",
					"addtime",
					"starlevel"
				};
				string[] tables8 = new string[]
				{
					"t_tasks"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields8, tables8, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DoingTasks(this, cmd);
				string[] fields9 = new string[]
				{
					"id",
					"goodsid",
					"isusing",
					"forge_level",
					"starttime",
					"endtime",
					"site",
					"quality",
					"Props",
					"gcount",
					"binding",
					"jewellist",
					"bagindex",
					"salemoney1",
					"saleyuanbao",
					"saleyinpiao",
					"addpropindex",
					"bornindex",
					"lucky",
					"strong",
					"excellenceinfo",
					"appendproplev",
					"equipchangelife",
					"ehinfo",
					"washprops",
					"juhun"
				};
				string[] tables9 = new string[]
				{
					"t_goods"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "gcount";
				array[1, 1] = ">";
				array[1, 2] = 0;
				object[,] whereParamFields = array;
				string[,] whereNoparamFields = null;
				string[,] array2 = new string[1, 2];
				array2[0, 0] = "id";
				array2[0, 1] = "asc";
				cmd = new MySQLSelectCommand(conn, fields9, tables9, whereParamFields, whereNoparamFields, array2);
				DBRoleInfo.DBTableRow2RoleInfo_Goods(this, cmd);
				string[] fields10 = new string[]
				{
					"goodsid",
					"dayid",
					"usednum"
				};
				string[] tables10 = new string[]
				{
					"t_goodslimit"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields10, tables10, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GoodsLimit(this, cmd);
				string[] fields11 = new string[]
				{
					"Id",
					"otherid",
					"friendType"
				};
				string[] tables11 = new string[]
				{
					"t_friends"
				};
				array = new object[1, 3];
				array[0, 0] = "myid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields11, tables11, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Friends(this, cmd);
				string[] fields12 = new string[]
				{
					"Id",
					"horseid",
					"bodyid",
					"propsNum",
					"PropsVal",
					"addtime",
					"failednum",
					"temptime",
					"tempnum",
					"faileddayid"
				};
				string[] tables12 = new string[]
				{
					"t_horses"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields12, tables12, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Horses(this, cmd);
				string[] fields13 = new string[]
				{
					"Id",
					"petid",
					"petname",
					"pettype",
					"feednum",
					"realivenum",
					"addtime",
					"props",
					"level"
				};
				string[] tables13 = new string[]
				{
					"t_pets"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields13, tables13, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Pets(this, cmd);
				string[] fields14 = new string[]
				{
					"Id",
					"jmid",
					"jmlevel",
					"bodylevel"
				};
				string[] tables14 = new string[]
				{
					"t_jingmai"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields14, tables14, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingMais(this, cmd);
				string[] fields15 = new string[]
				{
					"Id",
					"skillid",
					"skilllevel",
					"usednum"
				};
				string[] tables15 = new string[]
				{
					"t_skills"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields15, tables15, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Skills(this, cmd);
				string[] fields16 = new string[]
				{
					"bufferid",
					"starttime",
					"buffersecs",
					"bufferval"
				};
				string[] tables16 = new string[]
				{
					"t_buffer"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields16, tables16, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Buffers(this, cmd);
				string[] fields17 = new string[]
				{
					"huanid",
					"rectime",
					"recnum",
					"taskClass",
					"extdayid",
					"extnum"
				};
				string[] tables17 = new string[]
				{
					"t_dailytasks"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields17, tables17, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyTasks(this, cmd);
				string[] fields18 = new string[]
				{
					"jmtime",
					"jmnum"
				};
				string[] tables18 = new string[]
				{
					"t_dailyjingmai"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields18, tables18, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyJingMai(this, cmd);
				string[] fields19 = new string[]
				{
					"extgridnum"
				};
				string[] tables19 = new string[]
				{
					"t_ptbag"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields19, tables19, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_PortableBag(this, cmd);
				string[] fields20 = new string[]
				{
					"extgridnum"
				};
				string[] tables20 = new string[]
				{
					"t_reborn_storage"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields20, tables20, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_RebornPortableBag(this, cmd);
				string[] fields21 = new string[]
				{
					"loginweekid",
					"logindayid",
					"loginnum",
					"newstep",
					"steptime",
					"lastmtime",
					"curmid",
					"curmtime",
					"songliid",
					"logingiftstate",
					"onlinegiftstate",
					"lastlimittimehuodongid",
					"lastlimittimedayid",
					"limittimeloginnum",
					"limittimegiftstate",
					"everydayonlineawardstep",
					"geteverydayonlineawarddayid",
					"serieslogingetawardstep",
					"seriesloginawarddayid",
					"seriesloginawardgoodsid",
					"everydayonlineawardgoodsid"
				};
				string[] tables21 = new string[]
				{
					"t_huodong"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields21, tables21, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HuodongData(this, cmd);
				string[] fields22 = new string[]
				{
					"fubenid",
					"dayid",
					"enternum",
					"quickpasstimer",
					"finishnum"
				};
				string[] tables22 = new string[]
				{
					"t_fuben"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields22, tables22, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FuBenData(this, cmd);
				string[] fields23 = new string[]
				{
					"spouseid",
					"marrytype",
					"ringid",
					"goodwillexp",
					"goodwillstar",
					"goodwilllevel",
					"givenrose",
					"lovemessage",
					"autoreject",
					"changtime"
				};
				string[] tables23 = new string[]
				{
					"t_marry"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields23, tables23, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarriageData(this, cmd);
				string[] fields24 = new string[]
				{
					"partyroleid",
					"joincount"
				};
				string[] tables24 = new string[]
				{
					"t_marryparty_join"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields24, tables24, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarryPartyJoinList(this, cmd);
				string[] fields25 = new string[]
				{
					"*"
				};
				string[] tables25 = new string[]
				{
					"t_holyitem"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields25, tables25, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HolyItemData(this, cmd);
				string[] fields26 = new string[]
				{
					"*"
				};
				string[] tables26 = new string[]
				{
					"t_dailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields26, tables26, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyData(this, cmd);
				string[] fields27 = new string[]
				{
					"yabiaoid",
					"starttime",
					"state",
					"lineid",
					"toubao",
					"yabiaodayid",
					"yabiaonum",
					"takegoods"
				};
				string[] tables27 = new string[]
				{
					"t_yabiao"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields27, tables27, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YaBiaoData(this, cmd);
				string[] fields28 = new string[]
				{
					"rid",
					"prioritytype",
					"dayid",
					"usedtimes"
				};
				string[] tables28 = new string[]
				{
					"t_vipdailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields28, tables28, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_VipDailyData(this, cmd);
				string[] fields29 = new string[]
				{
					"rid",
					"jifen",
					"dayid",
					"awardhistory"
				};
				string[] tables29 = new string[]
				{
					"t_yangguangbkdailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields29, tables29, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YangGongBKDailyJiFenData(this, cmd);
				string[] fields30 = new string[]
				{
					"Id",
					"wingid",
					"forgeLevel",
					"addtime",
					"failednum",
					"equiped",
					"starexp",
					"zhulingnum",
					"zhuhunnum"
				};
				string[] tables30 = new string[]
				{
					"t_wings"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields30, tables30, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Wings(this, cmd);
				string[] fields31 = new string[]
				{
					"Id",
					"roleid",
					"picturejudgeid",
					"refercount"
				};
				string[] tables31 = new string[]
				{
					"t_picturejudgeinfo"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields31, tables31, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_picturejudgeinfo(this, cmd);
				string[] fields32 = new string[]
				{
					"Id",
					"roleid",
					"starsiteid",
					"starslotid"
				};
				string[] tables32 = new string[]
				{
					"t_starconstellationinfo"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields32, tables32, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_starconstellationinfo(this, cmd);
				string[] fields33 = new string[]
				{
					"roleid",
					"type",
					"level",
					"suit"
				};
				string[] tables33 = new string[]
				{
					"t_lingyu"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields33, tables33, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_LingYuInfo(this, cmd);
				string[] fields34 = new string[]
				{
					"roleid",
					"gmailid"
				};
				string[] tables34 = new string[]
				{
					"t_rolegmail_record"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields34, tables34, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GMailInfo(this, cmd);
				string[] fields35 = new string[]
				{
					"roleid",
					"slot_cnt",
					"level",
					"suit",
					"total_guard_point",
					"lastday_recover_point",
					"lastday_recover_offset"
				};
				string[] tables35 = new string[]
				{
					"t_guard_statue"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields35, tables35, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardStatue(this, cmd);
				string[] fields36 = new string[]
				{
					"roleid",
					"soul_type",
					"equip_slot"
				};
				string[] tables36 = new string[]
				{
					"t_guard_soul"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields36, tables36, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardSoul(this, cmd);
				string[] fields37 = new string[]
				{
					"tatalCount",
					"exp"
				};
				string[] tables37 = new string[]
				{
					"t_talent"
				};
				array = new object[1, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields37, tables37, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentBase(this, cmd);
				string[] fields38 = new string[]
				{
					"talentType",
					"effectID",
					"effectLevel"
				};
				string[] tables38 = new string[]
				{
					"t_talent_effect"
				};
				array = new object[2, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "effectLevel";
				array[1, 1] = ">";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields38, tables38, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentEffects(this, cmd);
				string[] fields39 = new string[]
				{
					"duanweiid",
					"duanweijifen",
					"duanweirank",
					"liansheng",
					"fightcount",
					"successcount",
					"todayfightcount",
					"lastfightdayid",
					"monthduanweirank",
					"fetchmonthawarddate",
					"rongyao"
				};
				string[] tables39 = new string[]
				{
					"t_kf_tianti_role"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields39, tables39, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TianTiData(this, cmd);
				string[] fields40 = new string[]
				{
					"roleID",
					"occupation",
					"level",
					"level_up_fail_num",
					"starNum",
					"starExp",
					"luckyPoint",
					"toTicks",
					"addTime",
					"activeFrozen",
					"activePalsy",
					"activeSpeedDown",
					"activeBlow",
					"unActiveFrozen",
					"unActivePalsy",
					"unActiveSpeedDown",
					"unActiveBlow"
				};
				string[] tables40 = new string[]
				{
					"t_merlin_magic_book"
				};
				array = new object[1, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields40, tables40, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MerlinData(this, cmd);
				string[] fields41 = new string[]
				{
					"id",
					"roleid",
					"goodsid",
					"position",
					"type",
					"bind"
				};
				string[] tables41 = new string[]
				{
					"t_fluorescent_gem_equip"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields41, tables41, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FluorescentGemData(this, cmd);
				string[] fields42 = new string[]
				{
					"buildid",
					"taskid_1",
					"taskid_2",
					"taskid_3",
					"taskid_4",
					"level",
					"exp",
					"developtime"
				};
				string[] tables42 = new string[]
				{
					"t_building"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields42, tables42, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_BuildingData(this, cmd);
				string[] fields43 = new string[]
				{
					"goodsid",
					"param1",
					"param2"
				};
				string[] tables43 = new string[]
				{
					"t_ornament"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields43, tables43, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OrnamentData(this, cmd);
				string[] fields44 = new string[]
				{
					"roleid",
					"act_type",
					"id",
					"award_flag",
					"param1",
					"param2"
				};
				string[] tables44 = new string[]
				{
					"t_seven_day_act"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields44, tables44, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SevenDayActData(this, cmd);
				string[] fields45 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"active"
				};
				string[] tables45 = new string[]
				{
					"t_special_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields45, tables45, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialActivityData(this, cmd);
				string[] fields46 = new string[]
				{
					"rid",
					"tequanid",
					"actid",
					"purchaseNum",
					"countNum"
				};
				string[] tables46 = new string[]
				{
					"t_special_priority_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields46, tables46, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialPriorityActivityData(this, cmd);
				string[] fields47 = new string[]
				{
					"rid",
					"sjID",
					"level"
				};
				string[] tables47 = new string[]
				{
					"t_shenjifuwen"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields47, tables47, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ShenJiData(this, cmd);
				DBRoleInfo.DBTableRow2RoleInfo_TarotData(conn, this, roleID);
				string[] fields48 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"activeDay"
				};
				string[] tables48 = new string[]
				{
					"t_everyday_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields48, tables48, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_EverydayActivityData(this, cmd);
				DBRoleInfo.DBTableRow2RoleInfo_FuWenData(conn, this, roleID);
				string[] fields49 = new string[]
				{
					"rid",
					"element",
					"dayid",
					"value",
					"todaycost",
					"histcost",
					"rollback"
				};
				string[] tables49 = new string[]
				{
					"t_alchemy"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields49, tables49, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_AlchemyData(this, cmd);
				string[] fields50 = new string[]
				{
					"rid",
					"suitid",
					"activite"
				};
				string[] tables50 = new string[]
				{
					"t_juexing"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields50, tables50, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JueXingData(this, cmd);
				string[] fields51 = new string[]
				{
					"rid",
					"goodsid",
					"isnew"
				};
				string[] tables51 = new string[]
				{
					"t_zuoqi"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields51, tables51, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ZuoQiData(this, cmd);
				string[] fields52 = new string[]
				{
					"rid",
					"activetype",
					"activeids"
				};
				string[] tables52 = new string[]
				{
					"t_juexing_jlys"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields52, tables52, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingLingYuanSuJueXingData(this, cmd);
				this.RankValue.Init(roleID);
				result = true;
			}
			return result;
		}

		
		public int PTID;

		
		public string WorldRoleID;

		
		public string Channel;

		
		public int SubOccupation;

		
		public int ZhanDuiID;

		
		public int ZhanDuiZhiWu;

		
		public int JunTuanZhiWu;

		
		public RoleHuiJiData HuiJiData = new RoleHuiJiData();

		
		public RoleBianShenData BianShenData = new RoleBianShenData();

		
		public RoleArmorData ArmorData = new RoleArmorData();

		
		public List<int> OccupationList = new List<int>();

		
		public RoleCustomData roleCustomData;

		
		private UserRankValueCache rankValue = new UserRankValueCache();

		
		public Dictionary<string, RoleParamsData> RoleParamsDict = new Dictionary<string, RoleParamsData>();

		
		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		
		public RoleTianTiData TianTiData;

		
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;
	}
}
