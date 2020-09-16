using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.Goods;
using GameServer.Logic.Spread;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserReturn;
using GameServer.Logic.WanMota;
using GameServer.Logic.YueKa;
using GameServer.TarotData;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class SafeClientData
	{
		
		public RoleDataEx GetRoleData()
		{
			return this._RoleDataEx;
		}

		
		
		public RoleDataEx RoleData
		{
			set
			{
				lock (this)
				{
					this._RoleDataEx = value;
				}
			}
		}

		
		
		public int RoleID
		{
			get
			{
				return this._RoleDataEx.RoleID;
			}
		}

		
		
		public string RoleName
		{
			get
			{
				return this._RoleDataEx.RoleName;
			}
		}

		
		
		public int RoleSex
		{
			get
			{
				return this._RoleDataEx.RoleSex;
			}
		}

		
		
		
		public int Occupation
		{
			get
			{
				return this._RoleDataEx.Occupation;
			}
			set
			{
				this._RoleDataEx.Occupation = value;
			}
		}

		
		
		
		public int SubOccupation
		{
			get
			{
				return this._RoleDataEx.SubOccupation;
			}
			set
			{
				this._RoleDataEx.SubOccupation = value;
			}
		}

		
		
		public List<int> OccupationList
		{
			get
			{
				return this._RoleDataEx.OccupationList;
			}
		}

		
		
		
		public int Level
		{
			get
			{
				return this._RoleDataEx.Level;
			}
			set
			{
				this._RoleDataEx.Level = value;
			}
		}

		
		
		
		public int ZhanDuiID
		{
			get
			{
				return this._RoleDataEx.ZhanDuiID;
			}
			set
			{
				this._RoleDataEx.ZhanDuiID = value;
			}
		}

		
		
		
		public int ZhanDuiZhiWu
		{
			get
			{
				return this._RoleDataEx.ZhanDuiZhiWu;
			}
			set
			{
				this._RoleDataEx.ZhanDuiZhiWu = value;
			}
		}

		
		
		
		public int Faction
		{
			get
			{
				return this._RoleDataEx.Faction;
			}
			set
			{
				this._RoleDataEx.Faction = value;
			}
		}

		
		
		
		public int JunTuanId
		{
			get
			{
				return this._RoleDataEx.JunTuanId;
			}
			set
			{
				this._RoleDataEx.JunTuanId = value;
			}
		}

		
		
		
		public string JunTuanName
		{
			get
			{
				return this._RoleDataEx.JunTuanName;
			}
			set
			{
				this._RoleDataEx.JunTuanName = value;
			}
		}

		
		
		
		public int JunTuanZhiWu
		{
			get
			{
				return this._RoleDataEx.JunTuanZhiWu;
			}
			set
			{
				this._RoleDataEx.JunTuanZhiWu = value;
			}
		}

		
		
		
		public int CompType
		{
			get
			{
				return this._RoleDataEx.CompType;
			}
			set
			{
				this._RoleDataEx.CompType = value;
			}
		}

		
		
		
		public byte CompZhiWu
		{
			get
			{
				return this._RoleDataEx.CompZhiWu;
			}
			set
			{
				this._RoleDataEx.CompZhiWu = value;
			}
		}

		
		
		
		public int Money1
		{
			get
			{
				return this._RoleDataEx.Money1;
			}
			set
			{
				this._RoleDataEx.Money1 = value;
			}
		}

		
		
		
		public int Money2
		{
			get
			{
				return this._RoleDataEx.Money2;
			}
			set
			{
				this._RoleDataEx.Money2 = value;
			}
		}

		
		
		
		public long Experience
		{
			get
			{
				return Thread.VolatileRead(ref this._RoleDataEx.Experience);
			}
			set
			{
				Thread.VolatileWrite(ref this._RoleDataEx.Experience, value);
			}
		}

		
		
		
		public int PKMode
		{
			get
			{
				return this._RoleDataEx.PKMode;
			}
			set
			{
				this._RoleDataEx.PKMode = value;
			}
		}

		
		
		
		public int PKValue
		{
			get
			{
				return this._RoleDataEx.PKValue;
			}
			set
			{
				this._RoleDataEx.PKValue = value;
			}
		}

		
		
		
		public int MapCode
		{
			get
			{
				return this._RoleDataEx.MapCode;
			}
			set
			{
				this._RoleDataEx.MapCode = value;
			}
		}

		
		
		
		public int PosX
		{
			get
			{
				return this._RoleDataEx.PosX;
			}
			set
			{
				this._RoleDataEx.PosX = value;
			}
		}

		
		
		
		public int PosY
		{
			get
			{
				return this._RoleDataEx.PosY;
			}
			set
			{
				this._RoleDataEx.PosY = value;
				if (null != this.ChangePosHandler)
				{
					this.ChangePosHandler();
				}
			}
		}

		
		
		
		public int RoleDirection
		{
			get
			{
				return this._RoleDataEx.RoleDirection;
			}
			set
			{
				this._RoleDataEx.RoleDirection = value;
			}
		}

		
		
		
		public int LifeV
		{
			get
			{
				return this._RoleDataEx.LifeV;
			}
			set
			{
				lock (this)
				{
					if (this._RoleDataEx.LifeV > 0)
					{
						this.CurrentLifeV = (int)((long)this._CurrentLifeV * (long)value / (long)this._RoleDataEx.LifeV);
					}
					else
					{
						this.CurrentLifeV = value;
					}
					this._RoleDataEx.LifeV = value;
				}
			}
		}

		
		
		
		public int MagicV
		{
			get
			{
				return this._RoleDataEx.MagicV;
			}
			set
			{
				lock (this)
				{
					if (this._RoleDataEx.MagicV > 0)
					{
						this.CurrentMagicV = (int)((long)this._CurrentMagicV * (long)value / (long)this._RoleDataEx.MagicV);
					}
					else
					{
						this.CurrentMagicV = value;
					}
					this._RoleDataEx.MagicV = value;
				}
			}
		}

		
		
		
		public List<OldTaskData> OldTasks
		{
			get
			{
				List<OldTaskData> oldTasks;
				lock (this)
				{
					oldTasks = this._RoleDataEx.OldTasks;
				}
				return oldTasks;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OldTasks = value;
				}
			}
		}

		
		
		
		public int RolePic
		{
			get
			{
				return this._RoleDataEx.RolePic;
			}
			set
			{
				this._RoleDataEx.RolePic = value;
			}
		}

		
		
		
		public int BagNum
		{
			get
			{
				return this._RoleDataEx.BagNum;
			}
			set
			{
				this._RoleDataEx.BagNum = value;
			}
		}

		
		
		
		public List<TaskData> TaskDataList
		{
			get
			{
				List<TaskData> taskDataList;
				lock (this)
				{
					taskDataList = this._RoleDataEx.TaskDataList;
				}
				return taskDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.TaskDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> GoodsDataList
		{
			get
			{
				List<GoodsData> goodsDataList;
				lock (this)
				{
					goodsDataList = this._RoleDataEx.GoodsDataList;
				}
				return goodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.GoodsDataList = value;
				}
			}
		}

		
		
		
		public List<SkillData> SkillDataList
		{
			get
			{
				List<SkillData> skillDataList;
				lock (this)
				{
					skillDataList = this._RoleDataEx.SkillDataList;
				}
				return skillDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SkillDataList = value;
				}
			}
		}

		
		
		
		public string OtherName
		{
			get
			{
				string otherName;
				lock (this)
				{
					otherName = this._RoleDataEx.OtherName;
				}
				return otherName;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OtherName = value;
				}
			}
		}

		
		
		
		public string MainQuickBarKeys
		{
			get
			{
				string mainQuickBarKeys;
				lock (this)
				{
					mainQuickBarKeys = this._RoleDataEx.MainQuickBarKeys;
				}
				return mainQuickBarKeys;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MainQuickBarKeys = value;
				}
			}
		}

		
		
		
		public string OtherQuickBarKeys
		{
			get
			{
				string otherQuickBarKeys;
				lock (this)
				{
					otherQuickBarKeys = this._RoleDataEx.OtherQuickBarKeys;
				}
				return otherQuickBarKeys;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OtherQuickBarKeys = value;
				}
			}
		}

		
		
		
		public int LoginNum
		{
			get
			{
				int loginNum;
				lock (this)
				{
					loginNum = this._RoleDataEx.LoginNum;
				}
				return loginNum;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LoginNum = value;
				}
			}
		}

		
		
		
		public int UserMoney
		{
			get
			{
				int userMoney;
				lock (this)
				{
					userMoney = this._RoleDataEx.UserMoney;
				}
				return userMoney;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.UserMoney = value;
				}
			}
		}

		
		
		
		public int LeftFightSeconds
		{
			get
			{
				int leftFightSeconds;
				lock (this)
				{
					leftFightSeconds = this._RoleDataEx.LeftFightSeconds;
				}
				return leftFightSeconds;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LeftFightSeconds = value;
				}
			}
		}

		
		
		
		public List<FriendData> FriendDataList
		{
			get
			{
				List<FriendData> friendDataList;
				lock (this)
				{
					friendDataList = this._RoleDataEx.FriendDataList;
				}
				return friendDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FriendDataList = value;
				}
			}
		}

		
		
		
		public List<HorseData> HorsesDataList
		{
			get
			{
				List<HorseData> horsesDataList;
				lock (this)
				{
					horsesDataList = this._RoleDataEx.HorsesDataList;
				}
				return horsesDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.HorsesDataList = value;
				}
			}
		}

		
		
		
		public int HorseDbID
		{
			get
			{
				int horseDbID;
				lock (this)
				{
					horseDbID = this._RoleDataEx.HorseDbID;
				}
				return horseDbID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.HorseDbID = value;
				}
			}
		}

		
		
		
		public List<PetData> PetsDataList
		{
			get
			{
				List<PetData> petsDataList;
				lock (this)
				{
					petsDataList = this._RoleDataEx.PetsDataList;
				}
				return petsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PetsDataList = value;
				}
			}
		}

		
		
		
		public int PetDbID
		{
			get
			{
				int petDbID;
				lock (this)
				{
					petDbID = this._RoleDataEx.PetDbID;
				}
				return petDbID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PetDbID = value;
				}
			}
		}

		
		
		
		public int InterPower
		{
			get
			{
				int interPower;
				lock (this)
				{
					interPower = this._RoleDataEx.InterPower;
				}
				return interPower;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.InterPower = value;
				}
			}
		}

		
		
		
		public List<JingMaiData> JingMaiDataList
		{
			get
			{
				List<JingMaiData> jingMaiDataList;
				lock (this)
				{
					jingMaiDataList = this._RoleDataEx.JingMaiDataList;
				}
				return jingMaiDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JingMaiDataList = value;
				}
			}
		}

		
		
		
		public int DJPoint
		{
			get
			{
				int djpoint;
				lock (this)
				{
					djpoint = this._RoleDataEx.DJPoint;
				}
				return djpoint;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.DJPoint = value;
				}
			}
		}

		
		
		
		public int DJTotal
		{
			get
			{
				int djtotal;
				lock (this)
				{
					djtotal = this._RoleDataEx.DJTotal;
				}
				return djtotal;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.DJTotal = value;
				}
			}
		}

		
		
		
		public int DJWincnt
		{
			get
			{
				int djwincnt;
				lock (this)
				{
					djwincnt = this._RoleDataEx.DJWincnt;
				}
				return djwincnt;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.DJWincnt = value;
				}
			}
		}

		
		
		
		public int TotalOnlineSecs
		{
			get
			{
				int totalOnlineSecs;
				lock (this)
				{
					totalOnlineSecs = this._RoleDataEx.TotalOnlineSecs;
				}
				return totalOnlineSecs;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.TotalOnlineSecs = value;
				}
			}
		}

		
		
		
		public int AntiAddictionSecs
		{
			get
			{
				int antiAddictionSecs;
				lock (this)
				{
					antiAddictionSecs = this._RoleDataEx.AntiAddictionSecs;
				}
				return antiAddictionSecs;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AntiAddictionSecs = value;
				}
			}
		}

		
		
		
		public long LastOfflineTime
		{
			get
			{
				long lastOfflineTime;
				lock (this)
				{
					lastOfflineTime = this._RoleDataEx.LastOfflineTime;
				}
				return lastOfflineTime;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LastOfflineTime = value;
				}
			}
		}

		
		
		
		public long BiGuanTime
		{
			get
			{
				long biGuanTime;
				lock (this)
				{
					biGuanTime = this._RoleDataEx.BiGuanTime;
				}
				return biGuanTime;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BiGuanTime = value;
				}
			}
		}

		
		
		
		public int YinLiang
		{
			get
			{
				int yinLiang;
				lock (this)
				{
					yinLiang = this._RoleDataEx.YinLiang;
				}
				return yinLiang;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.YinLiang = value;
				}
			}
		}

		
		
		
		public int TotalJingMaiExp
		{
			get
			{
				int totalJingMaiExp;
				lock (this)
				{
					totalJingMaiExp = this._RoleDataEx.TotalJingMaiExp;
				}
				return totalJingMaiExp;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.TotalJingMaiExp = value;
				}
			}
		}

		
		
		
		public int JingMaiExpNum
		{
			get
			{
				int jingMaiExpNum;
				lock (this)
				{
					jingMaiExpNum = this._RoleDataEx.JingMaiExpNum;
				}
				return jingMaiExpNum;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JingMaiExpNum = value;
				}
			}
		}

		
		
		
		public long RegTime
		{
			get
			{
				long regTime;
				lock (this)
				{
					regTime = this._RoleDataEx.RegTime;
				}
				return regTime;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RegTime = value;
				}
			}
		}

		
		
		
		public int LastHorseID
		{
			get
			{
				int lastHorseID;
				lock (this)
				{
					lastHorseID = this._RoleDataEx.LastHorseID;
				}
				return lastHorseID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LastHorseID = value;
				}
			}
		}

		
		
		
		public List<GoodsData> SaleGoodsDataList
		{
			get
			{
				List<GoodsData> saleGoodsDataList;
				lock (this)
				{
					saleGoodsDataList = this._RoleDataEx.SaleGoodsDataList;
				}
				return saleGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SaleGoodsDataList = value;
				}
			}
		}

		
		
		
		public int DefaultSkillID
		{
			get
			{
				int defaultSkillID;
				lock (this)
				{
					defaultSkillID = this._RoleDataEx.DefaultSkillID;
				}
				return defaultSkillID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.DefaultSkillID = value;
				}
			}
		}

		
		
		
		public int AutoLifeV
		{
			get
			{
				int autoLifeV;
				lock (this)
				{
					autoLifeV = this._RoleDataEx.AutoLifeV;
				}
				return autoLifeV;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AutoLifeV = value;
				}
			}
		}

		
		
		
		public int AutoMagicV
		{
			get
			{
				int autoMagicV;
				lock (this)
				{
					autoMagicV = this._RoleDataEx.AutoMagicV;
				}
				return autoMagicV;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AutoMagicV = value;
				}
			}
		}

		
		
		
		public List<BufferData> BufferDataList
		{
			get
			{
				List<BufferData> bufferDataList;
				lock (this)
				{
					bufferDataList = this._RoleDataEx.BufferDataList;
				}
				return bufferDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BufferDataList = value;
				}
			}
		}

		
		
		
		public List<DailyTaskData> MyDailyTaskDataList
		{
			get
			{
				List<DailyTaskData> myDailyTaskDataList;
				lock (this)
				{
					myDailyTaskDataList = this._RoleDataEx.MyDailyTaskDataList;
				}
				return myDailyTaskDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyDailyTaskDataList = value;
				}
			}
		}

		
		
		
		public DailyJingMaiData MyDailyJingMaiData
		{
			get
			{
				DailyJingMaiData myDailyJingMaiData;
				lock (this)
				{
					myDailyJingMaiData = this._RoleDataEx.MyDailyJingMaiData;
				}
				return myDailyJingMaiData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyDailyJingMaiData = value;
				}
			}
		}

		
		
		
		public int NumSkillID
		{
			get
			{
				int numSkillID;
				lock (this)
				{
					numSkillID = this._RoleDataEx.NumSkillID;
				}
				return numSkillID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.NumSkillID = value;
				}
			}
		}

		
		
		
		public PortableBagData MyPortableBagData
		{
			get
			{
				PortableBagData myPortableBagData;
				lock (this)
				{
					myPortableBagData = this._RoleDataEx.MyPortableBagData;
				}
				return myPortableBagData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyPortableBagData = value;
				}
			}
		}

		
		
		
		public RebornPortableBagData RebornGirdData
		{
			get
			{
				RebornPortableBagData rebornGirdData;
				lock (this)
				{
					rebornGirdData = this._RoleDataEx.RebornGirdData;
				}
				return rebornGirdData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornGirdData = value;
				}
			}
		}

		
		
		
		public HuodongData MyHuodongData
		{
			get
			{
				HuodongData myHuodongData;
				lock (this)
				{
					myHuodongData = this._RoleDataEx.MyHuodongData;
				}
				return myHuodongData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyHuodongData = value;
				}
			}
		}

		
		
		
		public List<FuBenData> FuBenDataList
		{
			get
			{
				List<FuBenData> fuBenDataList;
				lock (this)
				{
					fuBenDataList = this._RoleDataEx.FuBenDataList;
				}
				return fuBenDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FuBenDataList = value;
				}
			}
		}

		
		
		
		public GoodsData CopyMapAwardTmpGoods
		{
			get
			{
				GoodsData copyMapAwardTmpGoods;
				lock (this)
				{
					copyMapAwardTmpGoods = this._CopyMapAwardTmpGoods;
				}
				return copyMapAwardTmpGoods;
			}
			set
			{
				lock (this)
				{
					this._CopyMapAwardTmpGoods = value;
				}
			}
		}

		
		
		
		public int MainTaskID
		{
			get
			{
				int mainTaskID;
				lock (this)
				{
					mainTaskID = this._RoleDataEx.MainTaskID;
				}
				return mainTaskID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MainTaskID = value;
				}
			}
		}

		
		
		
		public int PKPoint
		{
			get
			{
				int pkpoint;
				lock (this)
				{
					pkpoint = this._RoleDataEx.PKPoint;
				}
				return pkpoint;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PKPoint = value;
				}
			}
		}

		
		
		
		public int TmpPKPoint
		{
			get
			{
				int tmpPKPoint;
				lock (this)
				{
					tmpPKPoint = this._TmpPKPoint;
				}
				return tmpPKPoint;
			}
			set
			{
				lock (this)
				{
					this._TmpPKPoint = value;
				}
			}
		}

		
		
		
		public int LianZhan
		{
			get
			{
				int lianZhan;
				lock (this)
				{
					lianZhan = this._RoleDataEx.LianZhan;
				}
				return lianZhan;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LianZhan = value;
				}
			}
		}

		
		
		
		public RoleDailyData MyRoleDailyData
		{
			get
			{
				RoleDailyData myRoleDailyData;
				lock (this)
				{
					myRoleDailyData = this._RoleDataEx.MyRoleDailyData;
				}
				return myRoleDailyData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyRoleDailyData = value;
				}
			}
		}

		
		
		
		public int KillBoss
		{
			get
			{
				int killBoss;
				lock (this)
				{
					killBoss = this._RoleDataEx.KillBoss;
				}
				return killBoss;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.KillBoss = value;
				}
			}
		}

		
		
		
		public YaBiaoData MyYaBiaoData
		{
			get
			{
				YaBiaoData myYaBiaoData;
				lock (this)
				{
					myYaBiaoData = this._RoleDataEx.MyYaBiaoData;
				}
				return myYaBiaoData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyYaBiaoData = value;
				}
			}
		}

		
		
		
		public long BattleNameStart
		{
			get
			{
				long battleNameStart;
				lock (this)
				{
					battleNameStart = this._RoleDataEx.BattleNameStart;
				}
				return battleNameStart;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BattleNameStart = value;
				}
			}
		}

		
		
		
		public int BattleNameIndex
		{
			get
			{
				int battleNameIndex;
				lock (this)
				{
					battleNameIndex = this._RoleDataEx.BattleNameIndex;
				}
				return battleNameIndex;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BattleNameIndex = value;
				}
			}
		}

		
		
		
		public int CZTaskID
		{
			get
			{
				int cztaskID;
				lock (this)
				{
					cztaskID = this._RoleDataEx.CZTaskID;
				}
				return cztaskID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.CZTaskID = value;
				}
			}
		}

		
		
		
		public int BattleNum
		{
			get
			{
				int battleNum;
				lock (this)
				{
					battleNum = this._RoleDataEx.BattleNum;
				}
				return battleNum;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BattleNum = value;
				}
			}
		}

		
		
		
		public int HeroIndex
		{
			get
			{
				int heroIndex;
				lock (this)
				{
					heroIndex = this._RoleDataEx.HeroIndex;
				}
				return heroIndex;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.HeroIndex = value;
				}
			}
		}

		
		
		
		public int ZoneID
		{
			get
			{
				int zoneID;
				lock (this)
				{
					zoneID = this._RoleDataEx.ZoneID;
				}
				return zoneID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.ZoneID = value;
				}
			}
		}

		
		
		
		public string BHName
		{
			get
			{
				string bhname;
				lock (this)
				{
					bhname = this._RoleDataEx.BHName;
				}
				return bhname;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BHName = value;
				}
			}
		}

		
		
		
		public int BHVerify
		{
			get
			{
				int bhverify;
				lock (this)
				{
					bhverify = this._RoleDataEx.BHVerify;
				}
				return bhverify;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BHVerify = value;
				}
			}
		}

		
		
		
		public int BHZhiWu
		{
			get
			{
				int bhzhiWu;
				lock (this)
				{
					bhzhiWu = this._RoleDataEx.BHZhiWu;
				}
				return bhzhiWu;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BHZhiWu = value;
				}
			}
		}

		
		
		
		public int BGDayID1
		{
			get
			{
				int bgdayID;
				lock (this)
				{
					bgdayID = this._RoleDataEx.BGDayID1;
				}
				return bgdayID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BGDayID1 = value;
				}
			}
		}

		
		
		
		public int BGMoney
		{
			get
			{
				int bgmoney;
				lock (this)
				{
					bgmoney = this._RoleDataEx.BGMoney;
				}
				return bgmoney;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BGMoney = value;
				}
			}
		}

		
		
		
		public int BGDayID2
		{
			get
			{
				int bgdayID;
				lock (this)
				{
					bgdayID = this._RoleDataEx.BGDayID2;
				}
				return bgdayID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BGDayID2 = value;
				}
			}
		}

		
		
		
		public int BGGoods
		{
			get
			{
				int bggoods;
				lock (this)
				{
					bggoods = this._RoleDataEx.BGGoods;
				}
				return bggoods;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BGGoods = value;
				}
			}
		}

		
		
		
		public int BangGong
		{
			get
			{
				int bangGong;
				lock (this)
				{
					bangGong = this._RoleDataEx.BangGong;
				}
				return bangGong;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BangGong = value;
				}
			}
		}

		
		
		
		public int HuangHou
		{
			get
			{
				int huangHou;
				lock (this)
				{
					huangHou = this._RoleDataEx.HuangHou;
				}
				return huangHou;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.HuangHou = value;
				}
			}
		}

		
		
		
		public Dictionary<int, int> PaiHangPosDict
		{
			get
			{
				Dictionary<int, int> paiHangPosDict;
				lock (this)
				{
					paiHangPosDict = this._RoleDataEx.PaiHangPosDict;
				}
				return paiHangPosDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PaiHangPosDict = value;
				}
			}
		}

		
		
		
		public int JieBiaoDayID
		{
			get
			{
				int jieBiaoDayID;
				lock (this)
				{
					jieBiaoDayID = this._RoleDataEx.JieBiaoDayID;
				}
				return jieBiaoDayID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JieBiaoDayID = value;
				}
			}
		}

		
		
		
		public int JieBiaoDayNum
		{
			get
			{
				int jieBiaoDayNum;
				lock (this)
				{
					jieBiaoDayNum = this._RoleDataEx.JieBiaoDayNum;
				}
				return jieBiaoDayNum;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JieBiaoDayNum = value;
				}
			}
		}

		
		
		
		public int LastMailID
		{
			get
			{
				int lastMailID;
				lock (this)
				{
					lastMailID = this._RoleDataEx.LastMailID;
				}
				return lastMailID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LastMailID = value;
				}
			}
		}

		
		
		
		public List<VipDailyData> VipDailyDataList
		{
			get
			{
				List<VipDailyData> vipDailyDataList;
				lock (this)
				{
					vipDailyDataList = this._RoleDataEx.VipDailyDataList;
				}
				return vipDailyDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.VipDailyDataList = value;
				}
			}
		}

		
		
		
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen
		{
			get
			{
				YangGongBKDailyJiFenData yangGongBKDailyJiFen;
				lock (this)
				{
					yangGongBKDailyJiFen = this._RoleDataEx.YangGongBKDailyJiFen;
				}
				return yangGongBKDailyJiFen;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.YangGongBKDailyJiFen = value;
				}
			}
		}

		
		
		
		public long OnceAwardFlag
		{
			get
			{
				long onceAwardFlag;
				lock (this)
				{
					onceAwardFlag = this._RoleDataEx.OnceAwardFlag;
				}
				return onceAwardFlag;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OnceAwardFlag = value;
				}
			}
		}

		
		
		
		public int Gold
		{
			get
			{
				int gold;
				lock (this)
				{
					gold = this._RoleDataEx.Gold;
				}
				return gold;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.Gold = value;
				}
			}
		}

		
		
		
		public List<GoodsLimitData> GoodsLimitDataList
		{
			get
			{
				List<GoodsLimitData> goodsLimitDataList;
				lock (this)
				{
					goodsLimitDataList = this._RoleDataEx.GoodsLimitDataList;
				}
				return goodsLimitDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.GoodsLimitDataList = value;
				}
			}
		}

		
		
		
		public Dictionary<string, RoleParamsData> RoleParamsDict
		{
			get
			{
				Dictionary<string, RoleParamsData> roleParamsDict;
				lock (this)
				{
					roleParamsDict = this._RoleDataEx.RoleParamsDict;
				}
				return roleParamsDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RoleParamsDict = value;
				}
			}
		}

		
		
		
		public int BanChat
		{
			get
			{
				int banChat;
				lock (this)
				{
					banChat = this._RoleDataEx.BanChat;
				}
				return banChat;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BanChat = value;
				}
			}
		}

		
		
		
		public int BanLogin
		{
			get
			{
				int banLogin;
				lock (this)
				{
					banLogin = this._RoleDataEx.BanLogin;
				}
				return banLogin;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BanLogin = value;
				}
			}
		}

		
		
		
		public int IsFlashPlayer
		{
			get
			{
				int isFlashPlayer;
				lock (this)
				{
					isFlashPlayer = this._RoleDataEx.IsFlashPlayer;
				}
				return isFlashPlayer;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.IsFlashPlayer = value;
				}
			}
		}

		
		
		
		public int AdmiredCount
		{
			get
			{
				int admiredCount;
				lock (this)
				{
					admiredCount = this._RoleDataEx.AdmiredCount;
				}
				return admiredCount;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AdmiredCount = value;
				}
			}
		}

		
		
		
		public int AdorationCount
		{
			get
			{
				int adorationCount;
				lock (this)
				{
					adorationCount = this._AdorationCount;
				}
				return adorationCount;
			}
			set
			{
				lock (this)
				{
					this._AdorationCount = value;
				}
			}
		}

		
		
		
		public int PKKingAdorationCount
		{
			get
			{
				int pkkingAdorationCount;
				lock (this)
				{
					pkkingAdorationCount = this._PKKingAdorationCount;
				}
				return pkkingAdorationCount;
			}
			set
			{
				lock (this)
				{
					this._PKKingAdorationCount = value;
				}
			}
		}

		
		
		
		public long JingJiNextRewardTime
		{
			get
			{
				long jingJiNextRewardTime;
				lock (this)
				{
					jingJiNextRewardTime = this._JingJiNextRewardTime;
				}
				return jingJiNextRewardTime;
			}
			set
			{
				lock (this)
				{
					this._JingJiNextRewardTime = value;
				}
			}
		}

		
		
		
		public int PKKingAdorationDayID
		{
			get
			{
				int pkkingAdorationDayID;
				lock (this)
				{
					pkkingAdorationDayID = this._PKKingAdorationDayID;
				}
				return pkkingAdorationDayID;
			}
			set
			{
				lock (this)
				{
					this._PKKingAdorationDayID = value;
				}
			}
		}

		
		
		
		public int AutoAssignPropertyPoint
		{
			get
			{
				int autoAssignPropertyPoint;
				lock (this)
				{
					autoAssignPropertyPoint = this._RoleDataEx.AutoAssignPropertyPoint;
				}
				return autoAssignPropertyPoint;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AutoAssignPropertyPoint = value;
				}
			}
		}

		
		
		
		public WingData MyWingData
		{
			get
			{
				WingData myWingData;
				lock (this)
				{
					myWingData = this._RoleDataEx.MyWingData;
				}
				return myWingData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyWingData = value;
				}
			}
		}

		
		
		
		public long StoreYinLiang
		{
			get
			{
				long store_Yinliang;
				lock (this)
				{
					store_Yinliang = this._RoleDataEx.Store_Yinliang;
				}
				return store_Yinliang;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.Store_Yinliang = value;
				}
			}
		}

		
		
		
		public long StoreMoney
		{
			get
			{
				long store_Money;
				lock (this)
				{
					store_Money = this._RoleDataEx.Store_Money;
				}
				return store_Money;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.Store_Money = value;
				}
			}
		}

		
		
		
		public RoleHuiJiData HuiJiData
		{
			get
			{
				return this._RoleDataEx.HuiJiData;
			}
			set
			{
				this._RoleDataEx.HuiJiData = value;
			}
		}

		
		
		
		public RoleBianShenData BianShenData
		{
			get
			{
				return this._RoleDataEx.BianShenData;
			}
			set
			{
				this._RoleDataEx.BianShenData = value;
			}
		}

		
		
		
		public RoleArmorData ArmorData
		{
			get
			{
				return this._RoleDataEx.ArmorData;
			}
			set
			{
				this._RoleDataEx.ArmorData = value;
			}
		}

		
		
		
		public int TotalPropPoint
		{
			get
			{
				int totalPropPoint;
				lock (this)
				{
					totalPropPoint = this._TotalPropPoint;
				}
				return totalPropPoint;
			}
			set
			{
				lock (this)
				{
					this._TotalPropPoint = value;
				}
			}
		}

		
		
		
		public int CombatForce
		{
			get
			{
				int combatForce;
				lock (this)
				{
					combatForce = this._RoleDataEx.CombatForce;
				}
				return combatForce;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.CombatForce = value;
				}
			}
		}

		
		
		
		public int PropStrength
		{
			get
			{
				int propStrength;
				lock (this)
				{
					propStrength = this._PropStrength;
				}
				return propStrength;
			}
			set
			{
				lock (this)
				{
					this._PropStrength = value;
				}
			}
		}

		
		
		
		public int PropIntelligence
		{
			get
			{
				int propIntelligence;
				lock (this)
				{
					propIntelligence = this._PropIntelligence;
				}
				return propIntelligence;
			}
			set
			{
				lock (this)
				{
					this._PropIntelligence = value;
				}
			}
		}

		
		
		
		public int PropDexterity
		{
			get
			{
				int propDexterity;
				lock (this)
				{
					propDexterity = this._PropDexterity;
				}
				return propDexterity;
			}
			set
			{
				lock (this)
				{
					this._PropDexterity = value;
				}
			}
		}

		
		
		
		public int PropConstitution
		{
			get
			{
				int propConstitution;
				lock (this)
				{
					propConstitution = this._PropConstitution;
				}
				return propConstitution;
			}
			set
			{
				lock (this)
				{
					this._PropConstitution = value;
				}
			}
		}

		
		
		
		public int ChangeLifeCount
		{
			get
			{
				return this._RoleDataEx.ChangeLifeCount;
			}
			set
			{
				this._RoleDataEx.ChangeLifeCount = value;
			}
		}

		
		
		public ChangeLifeProp RoleChangeLifeProp
		{
			get
			{
				return this._RoleChangeLifeProp;
			}
		}

		
		
		
		public string PushMessageID
		{
			get
			{
				string pushMessageID;
				lock (this)
				{
					pushMessageID = this._RoleDataEx.PushMessageID;
				}
				return pushMessageID;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PushMessageID = value;
				}
			}
		}

		
		
		
		public long ReportPosTicks
		{
			get
			{
				long reportPosTicks;
				lock (this)
				{
					reportPosTicks = this._ReportPosTicks;
				}
				return reportPosTicks;
			}
			set
			{
				lock (this)
				{
					this._ReportPosTicks = value;
				}
			}
		}

		
		
		
		public long ServerPosTicks
		{
			get
			{
				long serverPosTicks;
				lock (this)
				{
					serverPosTicks = this._ServerPosTicks;
				}
				return serverPosTicks;
			}
			set
			{
				lock (this)
				{
					this._ServerPosTicks = value;
				}
			}
		}

		
		
		
		public int CurrentAction
		{
			get
			{
				int currentAction;
				lock (this)
				{
					currentAction = this._CurrentAction;
				}
				return currentAction;
			}
			set
			{
				lock (this)
				{
					if (this._CurrentAction != value)
					{
						this._CurrentAction = value;
					}
				}
			}
		}

		
		
		
		public double MoveSpeed
		{
			get
			{
				double moveSpeed;
				lock (this)
				{
					moveSpeed = this._MoveSpeed;
				}
				return moveSpeed;
			}
			set
			{
				lock (this)
				{
					this._MoveSpeed = value;
				}
			}
		}

		
		
		
		public Point DestPoint
		{
			get
			{
				Point destPoint;
				lock (this)
				{
					destPoint = this._DestPoint;
				}
				return destPoint;
			}
			set
			{
				lock (this)
				{
					this._DestPoint = value;
				}
			}
		}

		
		
		
		public int CurrentLifeV
		{
			get
			{
				return this._CurrentLifeV;
			}
			set
			{
				int v = value - this._CurrentLifeV;
				if ((long)value > ClientCmdCheck.MinLogAddLifeV && (long)v > ClientCmdCheck.MinLogAddLifeV)
				{
					string log = ClientCmdCheck.GetLifeLogString(this.MapCode, this._CurrentLifeV, this.LifeV, v);
					if (!string.IsNullOrEmpty(log))
					{
						lock (this)
						{
							this.AddLifeAlertList.Enqueue(log);
						}
					}
				}
				if (this._CurrentLifeV > 0 && value < this.MinLife)
				{
					this._CurrentLifeV = this.MinLife;
				}
				else
				{
					this._CurrentLifeV = value;
				}
			}
		}

		
		
		
		public int CurrentMagicV
		{
			get
			{
				return this._CurrentMagicV;
			}
			set
			{
				this._CurrentMagicV = value;
			}
		}

		
		
		
		public int TeamID
		{
			get
			{
				int teamID;
				lock (this)
				{
					teamID = this._TeamID;
				}
				return teamID;
			}
			set
			{
				lock (this)
				{
					this._TeamID = value;
				}
			}
		}

		
		
		
		public int ExchangeID
		{
			get
			{
				int exchangeID;
				lock (this)
				{
					exchangeID = this._ExchangeID;
				}
				return exchangeID;
			}
			set
			{
				lock (this)
				{
					this._ExchangeID = value;
				}
			}
		}

		
		
		
		public long ExchangeTicks
		{
			get
			{
				long exchangeTicks;
				lock (this)
				{
					exchangeTicks = this._ExchangeTicks;
				}
				return exchangeTicks;
			}
			set
			{
				lock (this)
				{
					this._ExchangeTicks = value;
				}
			}
		}

		
		
		
		public int LastMapCode
		{
			get
			{
				int lastMapCode;
				lock (this)
				{
					lastMapCode = this._LastMapCode;
				}
				return lastMapCode;
			}
			set
			{
				lock (this)
				{
					this._LastMapCode = value;
				}
			}
		}

		
		
		
		public int LastPosX
		{
			get
			{
				int lastPosX;
				lock (this)
				{
					lastPosX = this._LastPosX;
				}
				return lastPosX;
			}
			set
			{
				lock (this)
				{
					this._LastPosX = value;
				}
			}
		}

		
		
		
		public int LastPosY
		{
			get
			{
				int lastPosY;
				lock (this)
				{
					lastPosY = this._LastPosY;
				}
				return lastPosY;
			}
			set
			{
				lock (this)
				{
					this._LastPosY = value;
				}
			}
		}

		
		
		
		public StallData StallDataItem
		{
			get
			{
				StallData stallDataItem;
				lock (this)
				{
					stallDataItem = this._StallDataItem;
				}
				return stallDataItem;
			}
			set
			{
				lock (this)
				{
					this._StallDataItem = value;
				}
			}
		}

		
		
		
		public long LastDBHeartTicks
		{
			get
			{
				long lastDBHeartTicks;
				lock (this)
				{
					lastDBHeartTicks = this._LastDBHeartTicks;
				}
				return lastDBHeartTicks;
			}
			set
			{
				lock (this)
				{
					this._LastDBHeartTicks = value;
				}
			}
		}

		
		
		
		public long LastSiteExpTicks
		{
			get
			{
				long lastSiteExpTicks;
				lock (this)
				{
					lastSiteExpTicks = this._LastSiteExpTicks;
				}
				return lastSiteExpTicks;
			}
			set
			{
				lock (this)
				{
					this._LastSiteExpTicks = value;
				}
			}
		}

		
		
		
		public long LastSiteSubPKPointTicks
		{
			get
			{
				long lastSiteSubPKPointTicks;
				lock (this)
				{
					lastSiteSubPKPointTicks = this._LastSiteSubPKPointTicks;
				}
				return lastSiteSubPKPointTicks;
			}
			set
			{
				lock (this)
				{
					this._LastSiteSubPKPointTicks = value;
				}
			}
		}

		
		
		
		public bool AutoFighting
		{
			get
			{
				bool autoFighting;
				lock (this)
				{
					autoFighting = this._AutoFighting;
				}
				return autoFighting;
			}
			set
			{
				lock (this)
				{
					this._AutoFighting = value;
				}
			}
		}

		
		
		
		public long LastAutoFightTicks
		{
			get
			{
				long lastAutoFightTicks;
				lock (this)
				{
					lastAutoFightTicks = this._LastAutoFightTicks;
				}
				return lastAutoFightTicks;
			}
			set
			{
				lock (this)
				{
					this._LastAutoFightTicks = value;
				}
			}
		}

		
		
		
		public int AutoFightingProctect
		{
			get
			{
				int autoFightingProctect;
				lock (this)
				{
					autoFightingProctect = this._AutoFightingProctect;
				}
				return autoFightingProctect;
			}
			set
			{
				lock (this)
				{
					this._AutoFightingProctect = value;
				}
			}
		}

		
		
		
		public int DJRoomID
		{
			get
			{
				int djroomID;
				lock (this)
				{
					djroomID = this._DJRoomID;
				}
				return djroomID;
			}
			set
			{
				lock (this)
				{
					this._DJRoomID = value;
				}
			}
		}

		
		
		
		public int DJRoomTeamID
		{
			get
			{
				int djroomTeamID;
				lock (this)
				{
					djroomTeamID = this._DJRoomTeamID;
				}
				return djroomTeamID;
			}
			set
			{
				lock (this)
				{
					this._DJRoomTeamID = value;
				}
			}
		}

		
		
		
		public bool ViewDJRoomDlg
		{
			get
			{
				bool viewDJRoomDlg;
				lock (this)
				{
					viewDJRoomDlg = this._ViewDJRoomDlg;
				}
				return viewDJRoomDlg;
			}
			set
			{
				lock (this)
				{
					this._ViewDJRoomDlg = value;
				}
			}
		}

		
		
		
		public int CopyMapID
		{
			get
			{
				int copyMapID;
				lock (this)
				{
					copyMapID = this._CopyMapID;
				}
				return copyMapID;
			}
			set
			{
				lock (this)
				{
					this._CopyMapID = value;
				}
			}
		}

		
		
		
		public GoodsPackItem LockedGoodsPackItem
		{
			get
			{
				GoodsPackItem goodsPackItem;
				lock (this)
				{
					goodsPackItem = this._GoodsPackItem;
				}
				return goodsPackItem;
			}
			set
			{
				lock (this)
				{
					this._GoodsPackItem = value;
				}
			}
		}

		
		
		
		public int SelectHorseDbID
		{
			get
			{
				int selectHorseDbID;
				lock (this)
				{
					selectHorseDbID = this._SelectHorseDbID;
				}
				return selectHorseDbID;
			}
			set
			{
				lock (this)
				{
					this._SelectHorseDbID = value;
				}
			}
		}

		
		
		
		public int PetRoleID
		{
			get
			{
				int petRoleID;
				lock (this)
				{
					petRoleID = this._PetRoleID;
				}
				return petRoleID;
			}
			set
			{
				lock (this)
				{
					this._PetRoleID = value;
				}
			}
		}

		
		
		
		public List<GoodsData> PortableGoodsDataList
		{
			get
			{
				List<GoodsData> portableGoodsDataList;
				lock (this)
				{
					portableGoodsDataList = this._PortableGoodsDataList;
				}
				return portableGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._PortableGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> JinDanGoodsDataList
		{
			get
			{
				List<GoodsData> jinDanGoodsDataList;
				lock (this)
				{
					jinDanGoodsDataList = this._JinDanGoodsDataList;
				}
				return jinDanGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._JinDanGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> MeditateGoodsDataList
		{
			get
			{
				List<GoodsData> meditateGoodsDataList;
				lock (this)
				{
					meditateGoodsDataList = this._MeditateGoodsDataList;
				}
				return meditateGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._MeditateGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> FashionGoodsDataList
		{
			get
			{
				List<GoodsData> fashionGoodsDataList;
				lock (this)
				{
					fashionGoodsDataList = this._RoleDataEx.FashionGoodsDataList;
				}
				return fashionGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FashionGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> OrnamentGoodsDataList
		{
			get
			{
				List<GoodsData> ornamentGoodsList;
				lock (this)
				{
					ornamentGoodsList = this._RoleDataEx.OrnamentGoodsList;
				}
				return ornamentGoodsList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OrnamentGoodsList = value;
				}
			}
		}

		
		
		
		public Dictionary<int, OrnamentData> OrnamentDataDict
		{
			get
			{
				Dictionary<int, OrnamentData> ornamentDataDict;
				lock (this)
				{
					ornamentDataDict = this._RoleDataEx.OrnamentDataDict;
				}
				return ornamentDataDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.OrnamentDataDict = value;
				}
			}
		}

		
		
		
		public Dictionary<int, ShenJiFuWenData> ShenJiDataDict
		{
			get
			{
				Dictionary<int, ShenJiFuWenData> shenJiDict;
				lock (this)
				{
					shenJiDict = this._RoleDataEx.ShenJiDict;
				}
				return shenJiDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.ShenJiDict = value;
				}
			}
		}

		
		
		
		public AlchemyDataDB AlchemyInfo
		{
			get
			{
				AlchemyDataDB alchemyInfo;
				lock (this)
				{
					alchemyInfo = this._RoleDataEx.AlchemyInfo;
				}
				return alchemyInfo;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.AlchemyInfo = value;
				}
			}
		}

		
		
		
		public List<BuildingData> BuildingDataList
		{
			get
			{
				List<BuildingData> buildingDataList;
				lock (this)
				{
					buildingDataList = this._RoleDataEx.BuildingDataList;
				}
				return buildingDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BuildingDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> DamonGoodsDataList
		{
			get
			{
				List<GoodsData> damonGoodsDataList;
				lock (this)
				{
					damonGoodsDataList = this._RoleDataEx.DamonGoodsDataList;
				}
				return damonGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.DamonGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> PaiZhuDamonGoodsDataList
		{
			get
			{
				List<GoodsData> paiZhuDamonGoodsDataList;
				lock (this)
				{
					paiZhuDamonGoodsDataList = this._RoleDataEx.PaiZhuDamonGoodsDataList;
				}
				return paiZhuDamonGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PaiZhuDamonGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> ElementhrtsList
		{
			get
			{
				List<GoodsData> elementhrtsList;
				lock (this)
				{
					elementhrtsList = this._RoleDataEx.ElementhrtsList;
				}
				return elementhrtsList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.ElementhrtsList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> UsingElementhrtsList
		{
			get
			{
				List<GoodsData> usingElementhrtsList;
				lock (this)
				{
					usingElementhrtsList = this._RoleDataEx.UsingElementhrtsList;
				}
				return usingElementhrtsList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.UsingElementhrtsList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> PetList
		{
			get
			{
				List<GoodsData> petList;
				lock (this)
				{
					petList = this._RoleDataEx.PetList;
				}
				return petList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.PetList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> MountStoreList
		{
			get
			{
				List<GoodsData> mountStoreList;
				lock (this)
				{
					mountStoreList = this._RoleDataEx.MountStoreList;
				}
				return mountStoreList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MountStoreList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> MountEquipList
		{
			get
			{
				List<GoodsData> mountEquipList;
				lock (this)
				{
					mountEquipList = this._RoleDataEx.MountEquipList;
				}
				return mountEquipList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MountEquipList = value;
				}
			}
		}

		
		
		
		public List<MountData> MountList
		{
			get
			{
				List<MountData> mountList;
				lock (this)
				{
					mountList = this._MountList;
				}
				return mountList;
			}
			set
			{
				lock (this)
				{
					this._MountList = value;
				}
			}
		}

		
		
		
		public int IsRide
		{
			get
			{
				int isRide;
				lock (this)
				{
					isRide = this._IsRide;
				}
				return isRide;
			}
			set
			{
				lock (this)
				{
					this._IsRide = value;
				}
			}
		}

		
		
		
		public long ReportPetPosTicks
		{
			get
			{
				long reportPetPosTicks;
				lock (this)
				{
					reportPetPosTicks = this._ReportPetPosTicks;
				}
				return reportPetPosTicks;
			}
			set
			{
				lock (this)
				{
					this._ReportPetPosTicks = value;
				}
			}
		}

		
		
		
		public int PetPosX
		{
			get
			{
				int petPosX;
				lock (this)
				{
					petPosX = this._PetPosX;
				}
				return petPosX;
			}
			set
			{
				lock (this)
				{
					this._PetPosX = value;
				}
			}
		}

		
		
		
		public int PetPosY
		{
			get
			{
				int petPosY;
				lock (this)
				{
					petPosY = this._PetPosY;
				}
				return petPosY;
			}
			set
			{
				lock (this)
				{
					this._PetPosY = value;
				}
			}
		}

		
		
		public EquipPropItem EquipProp
		{
			get
			{
				return this._EquipProp;
			}
		}

		
		
		public WeighItems WeighItems
		{
			get
			{
				return this._WeighItems;
			}
		}

		
		public RoleDataEx GetRoleDataEx()
		{
			return this._RoleDataEx;
		}

		
		
		
		public int LastCheckGridX
		{
			get
			{
				int lastCheckGridX;
				lock (this)
				{
					lastCheckGridX = this._LastCheckGridX;
				}
				return lastCheckGridX;
			}
			set
			{
				lock (this)
				{
					this._LastCheckGridX = value;
				}
			}
		}

		
		
		
		public int LastCheckGridY
		{
			get
			{
				int lastCheckGridY;
				lock (this)
				{
					lastCheckGridY = this._LastCheckGridY;
				}
				return lastCheckGridY;
			}
			set
			{
				lock (this)
				{
					this._LastCheckGridY = value;
				}
			}
		}

		
		
		
		public int BattleKilledNum
		{
			get
			{
				int battleKilledNum;
				lock (this)
				{
					battleKilledNum = this._BattleKilledNum;
				}
				return battleKilledNum;
			}
			set
			{
				lock (this)
				{
					this._BattleKilledNum = value;
				}
			}
		}

		
		
		
		public int ArenaBattleKilledNum
		{
			get
			{
				int arenaBattleKilledNum;
				lock (this)
				{
					arenaBattleKilledNum = this._ArenaBattleKilledNum;
				}
				return arenaBattleKilledNum;
			}
			set
			{
				lock (this)
				{
					this._ArenaBattleKilledNum = value;
				}
			}
		}

		
		
		
		public int HideSelf
		{
			get
			{
				int hideSelf;
				lock (this)
				{
					hideSelf = this._HideSelf;
				}
				return hideSelf;
			}
			set
			{
				lock (this)
				{
					this._HideSelf = value;
				}
			}
		}

		
		
		
		public int HideGM
		{
			get
			{
				int hideGM;
				lock (this)
				{
					hideGM = this._HideGM;
				}
				return hideGM;
			}
			set
			{
				lock (this)
				{
					this._HideGM = value;
				}
			}
		}

		
		
		
		public int BeTrackingRoleID
		{
			get
			{
				int beTrackingRoleID;
				lock (this)
				{
					beTrackingRoleID = this._BeTrackingRoleID;
				}
				return beTrackingRoleID;
			}
			set
			{
				lock (this)
				{
					this._BeTrackingRoleID = value;
				}
			}
		}

		
		
		
		public List<int> TrackingRoleIDList
		{
			get
			{
				List<int> trackingRoleIDList;
				lock (this)
				{
					trackingRoleIDList = this._TrackingRoleIDList;
				}
				return trackingRoleIDList;
			}
			set
			{
				lock (this)
				{
					this._TrackingRoleIDList = value;
				}
			}
		}

		
		
		
		public long LastJugeSafeRegionTicks
		{
			get
			{
				long lastJugeSafeRegionTicks;
				lock (this)
				{
					lastJugeSafeRegionTicks = this._LastJugeSafeRegionTicks;
				}
				return lastJugeSafeRegionTicks;
			}
			set
			{
				lock (this)
				{
					this._LastJugeSafeRegionTicks = value;
				}
			}
		}

		
		
		
		public int AntiAddictionTimeType
		{
			get
			{
				int antiAddictionTimeType;
				lock (this)
				{
					antiAddictionTimeType = this._AntiAddictionTimeType;
				}
				return antiAddictionTimeType;
			}
			set
			{
				lock (this)
				{
					this._AntiAddictionTimeType = value;
				}
			}
		}

		
		
		
		public Dictionary<string, int> JingMaiPropsDict
		{
			get
			{
				Dictionary<string, int> jingMaiPropsDict;
				lock (this)
				{
					jingMaiPropsDict = this._JingMaiPropsDict;
				}
				return jingMaiPropsDict;
			}
			set
			{
				lock (this)
				{
					this._JingMaiPropsDict = value;
				}
			}
		}

		
		
		
		public int JingMaiBodyLevel
		{
			get
			{
				int jingMaiBodyLevel;
				lock (this)
				{
					jingMaiBodyLevel = this._JingMaiBodyLevel;
				}
				return jingMaiBodyLevel;
			}
			set
			{
				lock (this)
				{
					this._JingMaiBodyLevel = value;
				}
			}
		}

		
		
		
		public bool FirstPlayStart
		{
			get
			{
				bool firstPlayStart;
				lock (this)
				{
					firstPlayStart = this._FirstPlayStart;
				}
				return firstPlayStart;
			}
			set
			{
				lock (this)
				{
					this._FirstPlayStart = value;
				}
			}
		}

		
		
		
		public long LastProcessBufferTicks
		{
			get
			{
				long lastProcessBufferTicks;
				lock (this)
				{
					lastProcessBufferTicks = this._LastProcessBufferTicks;
				}
				return lastProcessBufferTicks;
			}
			set
			{
				lock (this)
				{
					this._LastProcessBufferTicks = value;
				}
			}
		}

		
		
		
		public BufferData UpLifeLimitBufferData
		{
			get
			{
				BufferData upLifeLimitBufferData;
				lock (this)
				{
					upLifeLimitBufferData = this._UpLifeLimitBufferData;
				}
				return upLifeLimitBufferData;
			}
			set
			{
				lock (this)
				{
					this._UpLifeLimitBufferData = value;
				}
			}
		}

		
		
		
		public BufferData AddTempAttackBufferData
		{
			get
			{
				BufferData addTempAttackBufferData;
				lock (this)
				{
					addTempAttackBufferData = this._AddTempAttackBufferData;
				}
				return addTempAttackBufferData;
			}
			set
			{
				lock (this)
				{
					this._AddTempAttackBufferData = value;
				}
			}
		}

		
		
		
		public BufferData AddTempDefenseBufferData
		{
			get
			{
				BufferData addTempDefenseBufferData;
				lock (this)
				{
					addTempDefenseBufferData = this._AddTempDefenseBufferData;
				}
				return addTempDefenseBufferData;
			}
			set
			{
				lock (this)
				{
					this._AddTempDefenseBufferData = value;
				}
			}
		}

		
		
		
		public BufferData AntiBossBufferData
		{
			get
			{
				BufferData antiBossBufferData;
				lock (this)
				{
					antiBossBufferData = this._AntiBossBufferData;
				}
				return antiBossBufferData;
			}
			set
			{
				lock (this)
				{
					this._AntiBossBufferData = value;
				}
			}
		}

		
		
		
		public BufferData SheLiZhiYuanBufferData
		{
			get
			{
				BufferData sheLiZhiYuanBufferData;
				lock (this)
				{
					sheLiZhiYuanBufferData = this._SheLiZhiYuanBufferData;
				}
				return sheLiZhiYuanBufferData;
			}
			set
			{
				lock (this)
				{
					this._SheLiZhiYuanBufferData = value;
				}
			}
		}

		
		
		
		public BufferData DiWangZhiYouBufferData
		{
			get
			{
				BufferData diWangZhiYouBufferData;
				lock (this)
				{
					diWangZhiYouBufferData = this._DiWangZhiYouBufferData;
				}
				return diWangZhiYouBufferData;
			}
			set
			{
				lock (this)
				{
					this._DiWangZhiYouBufferData = value;
				}
			}
		}

		
		
		
		public BufferData JunQiBufferData
		{
			get
			{
				BufferData junQiBufferData;
				lock (this)
				{
					junQiBufferData = this._JunQiBufferData;
				}
				return junQiBufferData;
			}
			set
			{
				lock (this)
				{
					this._JunQiBufferData = value;
				}
			}
		}

		
		
		
		public int TempJMChongXueRate
		{
			get
			{
				int tempJMChongXueRate;
				lock (this)
				{
					tempJMChongXueRate = this._TempJMChongXueRate;
				}
				return tempJMChongXueRate;
			}
			set
			{
				lock (this)
				{
					this._TempJMChongXueRate = value;
				}
			}
		}

		
		
		
		public int TempHorseEnchanceRate
		{
			get
			{
				int tempHorseEnchanceRate;
				lock (this)
				{
					tempHorseEnchanceRate = this._TempHorseEnchanceRate;
				}
				return tempHorseEnchanceRate;
			}
			set
			{
				lock (this)
				{
					this._TempHorseEnchanceRate = value;
				}
			}
		}

		
		
		
		public int TempHorseUpLevelRate
		{
			get
			{
				int tempHorseUpLevelRate;
				lock (this)
				{
					tempHorseUpLevelRate = this._TempHorseUpLevelRate;
				}
				return tempHorseUpLevelRate;
			}
			set
			{
				lock (this)
				{
					this._TempHorseUpLevelRate = value;
				}
			}
		}

		
		
		
		public int AutoFightGetThings
		{
			get
			{
				int autoFightGetThings;
				lock (this)
				{
					autoFightGetThings = this._AutoFightGetThings;
				}
				return autoFightGetThings;
			}
			set
			{
				lock (this)
				{
					this._AutoFightGetThings = value;
				}
			}
		}

		
		
		
		public Dictionary<int, long> LastDBCmdTicksDict
		{
			get
			{
				Dictionary<int, long> lastDBCmdTicksDict;
				lock (this)
				{
					lastDBCmdTicksDict = this._LastDBCmdTicksDict;
				}
				return lastDBCmdTicksDict;
			}
			set
			{
				lock (this)
				{
					this._LastDBCmdTicksDict = value;
				}
			}
		}

		
		
		
		public int ClientHeartCount
		{
			get
			{
				int clientHeartCount;
				lock (this)
				{
					clientHeartCount = this._ClientHeartCount;
				}
				return clientHeartCount;
			}
			set
			{
				lock (this)
				{
					this._ClientHeartCount = value;
				}
			}
		}

		
		
		
		public long LastClientHeartTicks
		{
			get
			{
				long lastClientHeartTicks;
				lock (this)
				{
					lastClientHeartTicks = this._LastClientHeartTicks;
				}
				return lastClientHeartTicks;
			}
			set
			{
				lock (this)
				{
					this._LastClientHeartTicks = value;
				}
			}
		}

		
		
		
		public long LastClientServerSubTicks
		{
			get
			{
				long lastClientServerSubTicks;
				lock (this)
				{
					lastClientServerSubTicks = this._LastClientServerSubTicks;
				}
				return lastClientServerSubTicks;
			}
			set
			{
				lock (this)
				{
					this._LastClientServerSubTicks = value;
				}
			}
		}

		
		
		
		public int LastClientServerSubNum
		{
			get
			{
				int lastClientServerSubNum;
				lock (this)
				{
					lastClientServerSubNum = this._LastClientServerSubNum;
				}
				return lastClientServerSubNum;
			}
			set
			{
				lock (this)
				{
					this._LastClientServerSubNum = value;
				}
			}
		}

		
		
		
		public int ClosingClientStep
		{
			get
			{
				int closingClientStep;
				lock (this)
				{
					closingClientStep = this._ClosingClientStep;
				}
				return closingClientStep;
			}
			set
			{
				lock (this)
				{
					this._ClosingClientStep = value;
				}
			}
		}

		
		
		
		public SkillData NumSkillData
		{
			get
			{
				SkillData numSkillData;
				lock (this)
				{
					numSkillData = this._NumSkillData;
				}
				return numSkillData;
			}
			set
			{
				lock (this)
				{
					this._NumSkillData = value;
				}
			}
		}

		
		
		
		public int DefaultSkillLev
		{
			get
			{
				int defaultSkillLev;
				lock (this)
				{
					defaultSkillLev = this._DefaultSkillLev;
				}
				return defaultSkillLev;
			}
			set
			{
				lock (this)
				{
					this._DefaultSkillLev = value;
				}
			}
		}

		
		
		
		public int DefaultSkillUseNum
		{
			get
			{
				int defaultSkillUseNum;
				lock (this)
				{
					defaultSkillUseNum = this._DefaultSkillUseNum;
				}
				return defaultSkillUseNum;
			}
			set
			{
				lock (this)
				{
					this._DefaultSkillUseNum = value;
				}
			}
		}

		
		
		
		public Dictionary<int, long> LastDBSkillCmdTicksDict
		{
			get
			{
				Dictionary<int, long> lastDBSkillCmdTicksDict;
				lock (this)
				{
					lastDBSkillCmdTicksDict = this._LastDBSkillCmdTicksDict;
				}
				return lastDBSkillCmdTicksDict;
			}
			set
			{
				lock (this)
				{
					this._LastDBSkillCmdTicksDict = value;
				}
			}
		}

		
		
		
		public GoodsData WaBaoGoodsData
		{
			get
			{
				GoodsData waBaoGoodsData;
				lock (this)
				{
					waBaoGoodsData = this._WaBaoGoodsData;
				}
				return waBaoGoodsData;
			}
			set
			{
				lock (this)
				{
					this._WaBaoGoodsData = value;
				}
			}
		}

		
		
		public object UserMoneyMutex
		{
			get
			{
				object userMoneyMutex;
				lock (this)
				{
					userMoneyMutex = this._UserMoneyMutex;
				}
				return userMoneyMutex;
			}
		}

		
		
		public object YinLiangMutex
		{
			get
			{
				object yinLiangMutex;
				lock (this)
				{
					yinLiangMutex = this._YinLiangMutex;
				}
				return yinLiangMutex;
			}
		}

		
		
		public object GoldMutex
		{
			get
			{
				object goldMutex;
				lock (this)
				{
					goldMutex = this._GoldMutex;
				}
				return goldMutex;
			}
		}

		
		
		
		public int FuBenSeqID
		{
			get
			{
				int fuBenSeqID;
				lock (this)
				{
					fuBenSeqID = this._FuBenSeqID;
				}
				return fuBenSeqID;
			}
			set
			{
				lock (this)
				{
					this._FuBenSeqID = value;
				}
			}
		}

		
		
		
		public int FuBenID
		{
			get
			{
				int fuBenID;
				lock (this)
				{
					fuBenID = this._FuBenID;
				}
				return fuBenID;
			}
			set
			{
				lock (this)
				{
					this._FuBenID = value;
				}
			}
		}

		
		
		
		public List<int> OnePieceBoxIDList
		{
			get
			{
				List<int> onePieceBoxIDList;
				lock (this)
				{
					onePieceBoxIDList = this._OnePieceBoxIDList;
				}
				return onePieceBoxIDList;
			}
			set
			{
				lock (this)
				{
					this._OnePieceBoxIDList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> FallBaoXiangGoodsList
		{
			get
			{
				List<GoodsData> fallBaoXiangGoodsList;
				lock (this)
				{
					fallBaoXiangGoodsList = this._FallBaoXiangGoodsList;
				}
				return fallBaoXiangGoodsList;
			}
			set
			{
				lock (this)
				{
					this._FallBaoXiangGoodsList = value;
				}
			}
		}

		
		
		
		public long StartPurpleNameTicks
		{
			get
			{
				long startPurpleNameTicks;
				lock (this)
				{
					startPurpleNameTicks = this._StartPurpleNameTicks;
				}
				return startPurpleNameTicks;
			}
			set
			{
				lock (this)
				{
					this._StartPurpleNameTicks = value;
				}
			}
		}

		
		
		
		public int TotalLearnedSkillLevelCount
		{
			get
			{
				int totalLearnedSkillLevelCount;
				lock (this)
				{
					totalLearnedSkillLevelCount = this._TotalLearnedSkillLevelCount;
				}
				return totalLearnedSkillLevelCount;
			}
			set
			{
				lock (this)
				{
					this._TotalLearnedSkillLevelCount = value;
				}
			}
		}

		
		
		
		public long LastProcessMapLimitTimesTicks
		{
			get
			{
				long lastProcessMapLimitTimesTicks;
				lock (this)
				{
					lastProcessMapLimitTimesTicks = this._LastProcessMapLimitTimesTicks;
				}
				return lastProcessMapLimitTimesTicks;
			}
			set
			{
				lock (this)
				{
					this._LastProcessMapLimitTimesTicks = value;
				}
			}
		}

		
		
		
		public int RoleEquipJiFen
		{
			get
			{
				int roleEquipJiFen;
				lock (this)
				{
					roleEquipJiFen = this._RoleEquipJiFen;
				}
				return roleEquipJiFen;
			}
			set
			{
				lock (this)
				{
					this._RoleEquipJiFen = value;
				}
			}
		}

		
		
		
		public int RoleXueWeiNum
		{
			get
			{
				int roleXueWeiNum;
				lock (this)
				{
					roleXueWeiNum = this._RoleXueWeiNum;
				}
				return roleXueWeiNum;
			}
			set
			{
				lock (this)
				{
					this._RoleXueWeiNum = value;
				}
			}
		}

		
		
		
		public int RoleHorseJiFen
		{
			get
			{
				int roleHorseJiFen;
				lock (this)
				{
					roleHorseJiFen = this._RoleHorseJiFen;
				}
				return roleHorseJiFen;
			}
			set
			{
				lock (this)
				{
					this._RoleHorseJiFen = value;
				}
			}
		}

		
		
		public List<QueueCmdItem> QueueCmdItemList
		{
			get
			{
				List<QueueCmdItem> queueCmdItemList;
				lock (this)
				{
					queueCmdItemList = this._QueueCmdItemList;
				}
				return queueCmdItemList;
			}
		}

		
		
		
		public int ChongXueFailedNum
		{
			get
			{
				int chongXueFailedNum;
				lock (this)
				{
					chongXueFailedNum = this._ChongXueFailedNum;
				}
				return chongXueFailedNum;
			}
			set
			{
				lock (this)
				{
					this._ChongXueFailedNum = value;
				}
			}
		}

		
		
		
		public long StartTempHorseIDTicks
		{
			get
			{
				long startTempHorseIDTicks;
				lock (this)
				{
					startTempHorseIDTicks = this._StartTempHorseIDTicks;
				}
				return startTempHorseIDTicks;
			}
			set
			{
				lock (this)
				{
					this._StartTempHorseIDTicks = value;
				}
			}
		}

		
		
		
		public int TempHorseID
		{
			get
			{
				int tempHorseID;
				lock (this)
				{
					tempHorseID = this._TempHorseID;
				}
				return tempHorseID;
			}
			set
			{
				lock (this)
				{
					this._TempHorseID = value;
				}
			}
		}

		
		
		
		public int LoginDayID
		{
			get
			{
				int loginDayID;
				lock (this)
				{
					loginDayID = this._LoginDayID;
				}
				return loginDayID;
			}
			set
			{
				lock (this)
				{
					this._LoginDayID = value;
				}
			}
		}

		
		
		
		public int AllQualityIndex
		{
			get
			{
				int allQualityIndex;
				lock (this)
				{
					allQualityIndex = this._AllQualityIndex;
				}
				return allQualityIndex;
			}
			set
			{
				lock (this)
				{
					this._AllQualityIndex = value;
				}
			}
		}

		
		
		
		public int AllForgeLevelIndex
		{
			get
			{
				int allForgeLevelIndex;
				lock (this)
				{
					allForgeLevelIndex = this._AllForgeLevelIndex;
				}
				return allForgeLevelIndex;
			}
			set
			{
				lock (this)
				{
					this._AllForgeLevelIndex = value;
				}
			}
		}

		
		
		
		public int AllJewelLevelIndex
		{
			get
			{
				int allJewelLevelIndex;
				lock (this)
				{
					allJewelLevelIndex = this._AllJewelLevelIndex;
				}
				return allJewelLevelIndex;
			}
			set
			{
				lock (this)
				{
					this._AllJewelLevelIndex = value;
				}
			}
		}

		
		
		
		public int AllZhuoYueNum
		{
			get
			{
				int allZhuoYueNum;
				lock (this)
				{
					allZhuoYueNum = this._AllZhuoYueNum;
				}
				return allZhuoYueNum;
			}
			set
			{
				lock (this)
				{
					this._AllZhuoYueNum = value;
				}
			}
		}

		
		
		
		public AllThingsCalcItem MyAllThingsCalcItem
		{
			get
			{
				AllThingsCalcItem allThingsCalcItem;
				lock (this)
				{
					allThingsCalcItem = this._AllThingsCalcItem;
				}
				return allThingsCalcItem;
			}
			set
			{
				lock (this)
				{
					this._AllThingsCalcItem = value;
				}
			}
		}

		
		
		
		public YangGongBKItem MyYangGongBKItem
		{
			get
			{
				YangGongBKItem myYangGongBKItem;
				lock (this)
				{
					myYangGongBKItem = this._MyYangGongBKItem;
				}
				return myYangGongBKItem;
			}
			set
			{
				lock (this)
				{
					this._MyYangGongBKItem = value;
				}
			}
		}

		
		
		
		public Dictionary<int, QiZhenGeItemData> QiZhenGeGoodsDict
		{
			get
			{
				Dictionary<int, QiZhenGeItemData> qiZhenGeGoodsDict;
				lock (this)
				{
					qiZhenGeGoodsDict = this._QiZhenGeGoodsDict;
				}
				return qiZhenGeGoodsDict;
			}
			set
			{
				lock (this)
				{
					this._QiZhenGeGoodsDict = value;
				}
			}
		}

		
		
		
		public int QiZhenGeBuyNum
		{
			get
			{
				int qiZhenGeBuyNum;
				lock (this)
				{
					qiZhenGeBuyNum = this._QiZhenGeBuyNum;
				}
				return qiZhenGeBuyNum;
			}
			set
			{
				lock (this)
				{
					this._QiZhenGeBuyNum = value;
				}
			}
		}

		
		
		
		public long EnterMapTicks
		{
			get
			{
				long enterMapTicks;
				lock (this)
				{
					enterMapTicks = this._EnterMapTicks;
				}
				return enterMapTicks;
			}
			set
			{
				lock (this)
				{
					this._EnterMapTicks = value;
				}
			}
		}

		
		
		
		public int TotalUsedMoney
		{
			get
			{
				int totalUsedMoney;
				lock (this)
				{
					totalUsedMoney = this._TotalUsedMoney;
				}
				return totalUsedMoney;
			}
			set
			{
				lock (this)
				{
					this._TotalUsedMoney = value;
				}
			}
		}

		
		
		
		public int TotalGoodsMoney
		{
			get
			{
				int totalGoodsMoney;
				lock (this)
				{
					totalGoodsMoney = this._TotalGoodsMoney;
				}
				return totalGoodsMoney;
			}
			set
			{
				lock (this)
				{
					this._TotalGoodsMoney = value;
				}
			}
		}

		
		
		
		public int ReportWarningGoodsMoney
		{
			get
			{
				int reportWarningGoodsMoney;
				lock (this)
				{
					reportWarningGoodsMoney = this._ReportWarningGoodsMoney;
				}
				return reportWarningGoodsMoney;
			}
			set
			{
				lock (this)
				{
					this._ReportWarningGoodsMoney = value;
				}
			}
		}

		
		
		
		public long LastAttackTicks
		{
			get
			{
				long lastAttackTicks;
				lock (this)
				{
					lastAttackTicks = this._LastAttackTicks;
				}
				return lastAttackTicks;
			}
			set
			{
				lock (this)
				{
					this._LastAttackTicks = value;
				}
			}
		}

		
		
		
		public bool ForceShenFenZheng
		{
			get
			{
				bool forceShenFenZheng;
				lock (this)
				{
					forceShenFenZheng = this._ForceShenFenZheng;
				}
				return forceShenFenZheng;
			}
			set
			{
				lock (this)
				{
					this._ForceShenFenZheng = value;
				}
			}
		}

		
		
		
		public long FSHuDunStart
		{
			get
			{
				long fshuDunStart;
				lock (this)
				{
					fshuDunStart = this._FSHuDunStart;
				}
				return fshuDunStart;
			}
			set
			{
				lock (this)
				{
					this._FSHuDunStart = value;
				}
			}
		}

		
		
		
		public int FSHuDunSeconds
		{
			get
			{
				int fshuDunSeconds;
				lock (this)
				{
					fshuDunSeconds = this._FSHuDunSeconds;
				}
				return fshuDunSeconds;
			}
			set
			{
				lock (this)
				{
					this._FSHuDunSeconds = value;
				}
			}
		}

		
		
		
		public long DSHideStart
		{
			get
			{
				long dshideStart;
				lock (this)
				{
					dshideStart = this._DSHideStart;
				}
				return dshideStart;
			}
			set
			{
				lock (this)
				{
					this._DSHideStart = value;
				}
			}
		}

		
		
		
		public bool WaitingNotifyChangeMap
		{
			get
			{
				bool waitingNotifyChangeMap;
				lock (this)
				{
					waitingNotifyChangeMap = this._WaitingNotifyChangeMap;
				}
				return waitingNotifyChangeMap;
			}
			set
			{
				lock (this)
				{
					this._WaitingNotifyChangeMap = value;
				}
			}
		}

		
		
		
		public int BattleWhichSide
		{
			get
			{
				int battleWhichSide;
				lock (this)
				{
					battleWhichSide = this._BattleWhichSide;
				}
				return battleWhichSide;
			}
			set
			{
				lock (this)
				{
					this._BattleWhichSide = value;
				}
			}
		}

		
		
		
		public int ThisTimeOnlineSecs
		{
			get
			{
				int thisTimeOnlineSecs;
				lock (this)
				{
					thisTimeOnlineSecs = this._ThisTimeOnlineSecs;
				}
				return thisTimeOnlineSecs;
			}
			set
			{
				lock (this)
				{
					this._ThisTimeOnlineSecs = value;
				}
			}
		}

		
		
		public MagicCoolDownMgr MyMagicCoolDownMgr
		{
			get
			{
				MagicCoolDownMgr magicCoolDownMgr;
				lock (this)
				{
					magicCoolDownMgr = this._MagicCoolDownMgr;
				}
				return magicCoolDownMgr;
			}
		}

		
		
		
		public int LastSkillID
		{
			get
			{
				int lastSkillID;
				lock (this)
				{
					lastSkillID = this._LastSkillID;
				}
				return lastSkillID;
			}
			set
			{
				lock (this)
				{
					this._LastSkillID = value;
				}
			}
		}

		
		
		public GoodsCoolDownMgr MyGoodsCoolDownMgr
		{
			get
			{
				GoodsCoolDownMgr goodsCoolDownMgr;
				lock (this)
				{
					goodsCoolDownMgr = this._GoodsCoolDownMgr;
				}
				return goodsCoolDownMgr;
			}
		}

		
		
		
		public string MailSendSecurityCode
		{
			get
			{
				string mailSendSecurityCode;
				lock (this)
				{
					mailSendSecurityCode = this._MailSendSecurityCode;
				}
				return mailSendSecurityCode;
			}
			set
			{
				lock (this)
				{
					this._MailSendSecurityCode = value;
				}
			}
		}

		
		
		
		public long RoleStartMoveTicks
		{
			get
			{
				long roleStartMoveTicks;
				lock (this)
				{
					roleStartMoveTicks = this._RoleStartMoveTicks;
				}
				return roleStartMoveTicks;
			}
			set
			{
				lock (this)
				{
					this._RoleStartMoveTicks = value;
				}
			}
		}

		
		
		
		public long InstantMoveTick
		{
			get
			{
				long instantMoveTick;
				lock (this)
				{
					instantMoveTick = this._InstantMoveTick;
				}
				return instantMoveTick;
			}
			set
			{
				lock (this)
				{
					this._InstantMoveTick = value;
				}
			}
		}

		
		
		
		public string RolePathString
		{
			get
			{
				string rolePathString;
				lock (this)
				{
					rolePathString = this._RolePathString;
				}
				return rolePathString;
			}
			set
			{
				lock (this)
				{
					this._RolePathString = value;
				}
			}
		}

		
		
		
		public double TengXunFCMRate
		{
			get
			{
				double tengXunFCMRate;
				lock (this)
				{
					tengXunFCMRate = this._TengXunFCMRate;
				}
				return tengXunFCMRate;
			}
			set
			{
				lock (this)
				{
					this._TengXunFCMRate = value;
				}
			}
		}

		
		
		
		public Dictionary<string, long> LastDBRoleParamCmdTicksDict
		{
			get
			{
				Dictionary<string, long> lastDBRoleParamCmdTicksDict;
				lock (this)
				{
					lastDBRoleParamCmdTicksDict = this._LastDBRoleParamCmdTicksDict;
				}
				return lastDBRoleParamCmdTicksDict;
			}
			set
			{
				lock (this)
				{
					this._LastDBRoleParamCmdTicksDict = value;
				}
			}
		}

		
		
		
		public Dictionary<int, long> LastDBEquipStrongCmdTicksDict
		{
			get
			{
				Dictionary<int, long> lastDBEquipStrongCmdTicksDict;
				lock (this)
				{
					lastDBEquipStrongCmdTicksDict = this._LastDBEquipStrongCmdTicksDict;
				}
				return lastDBEquipStrongCmdTicksDict;
			}
			set
			{
				lock (this)
				{
					this._LastDBEquipStrongCmdTicksDict = value;
				}
			}
		}

		
		
		
		public uint TotalKilledMonsterNum
		{
			get
			{
				uint totalKilledMonsterNum;
				lock (this)
				{
					totalKilledMonsterNum = this._TotalKilledMonsterNum;
				}
				return totalKilledMonsterNum;
			}
			set
			{
				lock (this)
				{
					this._TotalKilledMonsterNum = value;
				}
			}
		}

		
		
		
		public ushort TimerKilledMonsterNum
		{
			get
			{
				ushort timerKilledMonsterNum;
				lock (this)
				{
					timerKilledMonsterNum = this._TimerKilledMonsterNum;
				}
				return timerKilledMonsterNum;
			}
			set
			{
				lock (this)
				{
					this._TimerKilledMonsterNum = value;
				}
			}
		}

		
		
		
		public uint NextKilledMonsterChengJiuNum
		{
			get
			{
				uint nextKillMonsterChengJiuNum;
				lock (this)
				{
					nextKillMonsterChengJiuNum = this._NextKillMonsterChengJiuNum;
				}
				return nextKillMonsterChengJiuNum;
			}
			set
			{
				lock (this)
				{
					this._NextKillMonsterChengJiuNum = value;
				}
			}
		}

		
		
		
		public int MaxTongQianNum
		{
			get
			{
				int maxTongQianNum;
				lock (this)
				{
					maxTongQianNum = this._MaxTongQianNum;
				}
				return maxTongQianNum;
			}
			set
			{
				lock (this)
				{
					this._MaxTongQianNum = value;
				}
			}
		}

		
		
		
		public uint NextTongQianChengJiuNum
		{
			get
			{
				uint nextTongQianChengJiuNum;
				lock (this)
				{
					nextTongQianChengJiuNum = this._NextTongQianChengJiuNum;
				}
				return nextTongQianChengJiuNum;
			}
			set
			{
				lock (this)
				{
					this._NextTongQianChengJiuNum = value;
				}
			}
		}

		
		
		
		public uint TotalDayLoginNum
		{
			get
			{
				uint totalDayLoginNum;
				lock (this)
				{
					totalDayLoginNum = this._TotalDayLoginNum;
				}
				return totalDayLoginNum;
			}
			set
			{
				lock (this)
				{
					this._TotalDayLoginNum = value;
				}
			}
		}

		
		
		
		public uint ContinuousDayLoginNum
		{
			get
			{
				uint continuousDayLoginNum;
				lock (this)
				{
					continuousDayLoginNum = this._ContinuousDayLoginNum;
				}
				return continuousDayLoginNum;
			}
			set
			{
				lock (this)
				{
					this._ContinuousDayLoginNum = value;
				}
			}
		}

		
		
		
		public int ChengJiuPoints
		{
			get
			{
				int chengJiuPoints;
				lock (this)
				{
					chengJiuPoints = this._ChengJiuPoints;
				}
				return chengJiuPoints;
			}
			set
			{
				lock (this)
				{
					this._ChengJiuPoints = value;
				}
			}
		}

		
		
		
		public int ChengJiuLevel
		{
			get
			{
				int chengJiuLevel;
				lock (this)
				{
					chengJiuLevel = this._ChengJiuLevel;
				}
				return chengJiuLevel;
			}
			set
			{
				lock (this)
				{
					this._ChengJiuLevel = value;
				}
			}
		}

		
		
		
		public int ShenLiJingHuaPoints
		{
			get
			{
				int shenLiJingHuaPoints;
				lock (this)
				{
					shenLiJingHuaPoints = this._ShenLiJingHuaPoints;
				}
				return shenLiJingHuaPoints;
			}
			set
			{
				lock (this)
				{
					this._ShenLiJingHuaPoints = value;
				}
			}
		}

		
		
		
		public int NengLiangSmall
		{
			get
			{
				int nengLiangSmall;
				lock (this)
				{
					nengLiangSmall = this._NengLiangSmall;
				}
				return nengLiangSmall;
			}
			set
			{
				lock (this)
				{
					this._NengLiangSmall = value;
				}
			}
		}

		
		
		
		public int NengLiangMedium
		{
			get
			{
				int nengLiangMedium;
				lock (this)
				{
					nengLiangMedium = this._NengLiangMedium;
				}
				return nengLiangMedium;
			}
			set
			{
				lock (this)
				{
					this._NengLiangMedium = value;
				}
			}
		}

		
		
		
		public int NengLiangBig
		{
			get
			{
				int nengLiangBig;
				lock (this)
				{
					nengLiangBig = this._NengLiangBig;
				}
				return nengLiangBig;
			}
			set
			{
				lock (this)
				{
					this._NengLiangBig = value;
				}
			}
		}

		
		
		
		public int NengLiangSuper
		{
			get
			{
				int nengLiangSuper;
				lock (this)
				{
					nengLiangSuper = this._NengLiangSuper;
				}
				return nengLiangSuper;
			}
			set
			{
				lock (this)
				{
					this._NengLiangSuper = value;
				}
			}
		}

		
		
		
		public int FuWenZhiChen
		{
			get
			{
				int fuWenZhiChen;
				lock (this)
				{
					fuWenZhiChen = this._FuWenZhiChen;
				}
				return fuWenZhiChen;
			}
			set
			{
				lock (this)
				{
					this._FuWenZhiChen = value;
				}
			}
		}

		
		
		
		public int MoBi
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[141];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[141] = (long)value;
				}
			}
		}

		
		
		
		public int LuckStar
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[163];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[163] = (long)value;
				}
			}
		}

		
		
		
		public int TeamRongYao
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[160];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[160] = (long)value;
				}
			}
		}

		
		
		
		public int TeamPoint
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[162];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[162] = (long)value;
				}
			}
		}

		
		
		
		public int KuaFuLueDuoEnterNum
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[134];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[134] = (long)value;
				}
			}
		}

		
		
		
		public int KuaFuLueDuoEnterNumBuyNum
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[135];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[135] = (long)value;
				}
			}
		}

		
		
		
		public int KuaFuLueDuoEnterNumDayID
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[136];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[136] = (long)value;
				}
			}
		}

		
		
		
		public int JueXingPoint
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)this.MoneyData[132];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[132] = (long)value;
				}
			}
		}

		
		
		
		public long JueXingZhiChen
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[133];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[133] = value;
				}
			}
		}

		
		
		
		public long YuanSuJueXingShi
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[144];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[144] = value;
				}
			}
		}

		
		
		
		public long HunJing
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[139];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[139] = value;
				}
			}
		}

		
		
		
		public long MountPoint
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[140];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[140] = value;
				}
			}
		}

		
		
		
		public int WanMoTaNextLayerOrder
		{
			get
			{
				int wanMoTaNextLayerOrder;
				lock (this)
				{
					wanMoTaNextLayerOrder = this._WanMoTaNextLayerOrder;
				}
				return wanMoTaNextLayerOrder;
			}
			set
			{
				lock (this)
				{
					this._WanMoTaNextLayerOrder = value;
				}
			}
		}

		
		
		
		public List<FuWenTabData> FuWenTabList
		{
			get
			{
				List<FuWenTabData> fuWenTabList;
				lock (this)
				{
					fuWenTabList = this._RoleDataEx.FuWenTabList;
				}
				return fuWenTabList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FuWenTabList = value;
				}
			}
		}

		
		
		
		public List<GoodsData> FuWenGoodsDataList
		{
			get
			{
				List<GoodsData> fuWenGoodsDataList;
				lock (this)
				{
					fuWenGoodsDataList = this._RoleDataEx.FuWenGoodsDataList;
				}
				return fuWenGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FuWenGoodsDataList = value;
				}
			}
		}

		
		
		
		public List<Monster> SummonMonstersList
		{
			get
			{
				List<Monster> summonMonstersList;
				lock (this)
				{
					summonMonstersList = this._SummonMonstersList;
				}
				return summonMonstersList;
			}
			set
			{
				lock (this)
				{
					this._SummonMonstersList = value;
				}
			}
		}

		
		public Monster GetSummonMonster(int i)
		{
			Monster monster = null;
			try
			{
				monster = this.SummonMonstersList[i];
			}
			catch
			{
			}
			return monster;
		}

		
		
		
		public long StartAddExpTicks
		{
			get
			{
				long startAddExpTicks;
				lock (this)
				{
					startAddExpTicks = this._StartAddExpTicks;
				}
				return startAddExpTicks;
			}
			set
			{
				lock (this)
				{
					this._StartAddExpTicks = value;
				}
			}
		}

		
		
		
		public Dictionary<int, BufferData> BufferDataDict
		{
			get
			{
				Dictionary<int, BufferData> bufferDataDict;
				lock (this)
				{
					bufferDataDict = this._BufferDataDict;
				}
				return bufferDataDict;
			}
			set
			{
				lock (this)
				{
					this._BufferDataDict = value;
				}
			}
		}

		
		
		
		public long StartAddLifeMagicTicks
		{
			get
			{
				long startAddLifeMagicTicks;
				lock (this)
				{
					startAddLifeMagicTicks = this._StartAddLifeMagicTicks;
				}
				return startAddLifeMagicTicks;
			}
			set
			{
				lock (this)
				{
					this._StartAddLifeMagicTicks = value;
				}
			}
		}

		
		
		
		public long StartAddLifeNoShowTicks
		{
			get
			{
				long startAddLifeNoShowTicks;
				lock (this)
				{
					startAddLifeNoShowTicks = this._StartAddLifeNoShowTicks;
				}
				return startAddLifeNoShowTicks;
			}
			set
			{
				lock (this)
				{
					this._StartAddLifeNoShowTicks = value;
				}
			}
		}

		
		
		
		public long StartAddMaigcNoShowTicks
		{
			get
			{
				long startAddMaigcNoShowTicks;
				lock (this)
				{
					startAddMaigcNoShowTicks = this._StartAddMaigcNoShowTicks;
				}
				return startAddMaigcNoShowTicks;
			}
			set
			{
				lock (this)
				{
					this._StartAddMaigcNoShowTicks = value;
				}
			}
		}

		
		
		
		public long DSStartDSAddLifeNoShowTicks
		{
			get
			{
				long dsstartDSAddLifeNoShowTicks;
				lock (this)
				{
					dsstartDSAddLifeNoShowTicks = this._DSStartDSAddLifeNoShowTicks;
				}
				return dsstartDSAddLifeNoShowTicks;
			}
			set
			{
				lock (this)
				{
					this._DSStartDSAddLifeNoShowTicks = value;
				}
			}
		}

		
		// (add) Token: 0x060036FE RID: 14078 RVA: 0x002F56E4 File Offset: 0x002F38E4
		// (remove) Token: 0x060036FF RID: 14079 RVA: 0x002F5720 File Offset: 0x002F3920
		public event ChangePosEventHandler ChangePosHandler;

		
		
		
		public int RoleIDAttackebByMyself
		{
			get
			{
				int result;
				if (TimeUtil.NOW() - this._LastLogRoleIDAttackebByMyselfTicks > 15000L)
				{
					result = -1;
				}
				else
				{
					lock (this)
					{
						result = this._RoleIDAttackebByMyself;
					}
				}
				return result;
			}
			set
			{
				this._LastLogRoleIDAttackebByMyselfTicks = TimeUtil.NOW();
				lock (this)
				{
					this._RoleIDAttackebByMyself = value;
				}
			}
		}

		
		
		
		public int RoleIDAttackMe
		{
			get
			{
				int result;
				if (TimeUtil.NOW() - this._LastLogRoleIDAttackMeTicks > 15000L)
				{
					result = -1;
				}
				else
				{
					lock (this)
					{
						result = this._RoleIDAttackMe;
					}
				}
				return result;
			}
			set
			{
				this._LastLogRoleIDAttackMeTicks = TimeUtil.NOW();
				lock (this)
				{
					this._RoleIDAttackMe = value;
				}
			}
		}

		
		
		
		public List<int> RoleCommonUseIntPamams
		{
			get
			{
				List<int> roleCommonUseIntPamams;
				lock (this)
				{
					roleCommonUseIntPamams = this._RoleCommonUseIntPamams;
				}
				return roleCommonUseIntPamams;
			}
			set
			{
				lock (this)
				{
					this._RoleCommonUseIntPamams = value;
				}
			}
		}

		
		
		
		public long LastMapLimitUpdateTicks
		{
			get
			{
				long lastMapLimitUpdateTicks;
				lock (this)
				{
					lastMapLimitUpdateTicks = this._LastMapLimitUpdateTicks;
				}
				return lastMapLimitUpdateTicks;
			}
			set
			{
				lock (this)
				{
					this._LastMapLimitUpdateTicks = value;
				}
			}
		}

		
		
		
		public long LastHintToUpdateClientTicks
		{
			get
			{
				long lastHintToUpdateClientTicks;
				lock (this)
				{
					lastHintToUpdateClientTicks = this._LastHintToUpdateClientTicks;
				}
				return lastHintToUpdateClientTicks;
			}
			set
			{
				lock (this)
				{
					this._LastHintToUpdateClientTicks = value;
				}
			}
		}

		
		
		
		public int[] BaseBattleAttributesOfLastTime
		{
			get
			{
				int[] baseBattleAttributesOfLastTime;
				lock (this)
				{
					baseBattleAttributesOfLastTime = this._BaseBattleAttributesOfLastTime;
				}
				return baseBattleAttributesOfLastTime;
			}
			set
			{
				lock (this)
				{
					this._BaseBattleAttributesOfLastTime = value;
				}
			}
		}

		
		
		
		public long LastGoodsLimitUpdateTicks
		{
			get
			{
				long lastGoodsLimitUpdateTicks;
				lock (this)
				{
					lastGoodsLimitUpdateTicks = this._LastGoodsLimitUpdateTicks;
				}
				return lastGoodsLimitUpdateTicks;
			}
			set
			{
				lock (this)
				{
					this._LastGoodsLimitUpdateTicks = value;
				}
			}
		}

		
		
		
		public long LastFashionLimitUpdateTicks
		{
			get
			{
				long lastFashionLimitUpdateTicks;
				lock (this)
				{
					lastFashionLimitUpdateTicks = this._LastFashionLimitUpdateTicks;
				}
				return lastFashionLimitUpdateTicks;
			}
			set
			{
				lock (this)
				{
					this._LastFashionLimitUpdateTicks = value;
				}
			}
		}

		
		
		
		public long LastRoleDeadTicks
		{
			get
			{
				long lastRoleDeadTicks;
				lock (this)
				{
					lastRoleDeadTicks = this._LastRoleDeadTicks;
				}
				return lastRoleDeadTicks;
			}
			set
			{
				lock (this)
				{
					this._LastRoleDeadTicks = value;
				}
			}
		}

		
		
		
		public int MoveAndActionNum
		{
			get
			{
				int moveAndActionNum;
				lock (this)
				{
					moveAndActionNum = this._MoveAndActionNum;
				}
				return moveAndActionNum;
			}
			set
			{
				lock (this)
				{
					this._MoveAndActionNum = value;
				}
			}
		}

		
		
		
		public Dictionary<object, byte> VisibleGrid9Objects
		{
			get
			{
				Dictionary<object, byte> visibleGrid9Objects;
				lock (this)
				{
					visibleGrid9Objects = this._VisibleGrid9Objects;
				}
				return visibleGrid9Objects;
			}
			set
			{
				lock (this)
				{
					this._VisibleGrid9Objects = value;
				}
			}
		}

		
		
		
		public Dictionary<object, byte> VisibleMeGrid9GameClients
		{
			get
			{
				Dictionary<object, byte> visibleMeGrid9GameClients;
				lock (this)
				{
					visibleMeGrid9GameClients = this._VisibleMeGrid9GameClients;
				}
				return visibleMeGrid9GameClients;
			}
			set
			{
				lock (this)
				{
					this._VisibleMeGrid9GameClients = value;
				}
			}
		}

		
		
		
		public List<QiangGouItemData> QiangGouItemList
		{
			get
			{
				List<QiangGouItemData> qiangGouItemList;
				lock (this)
				{
					qiangGouItemList = this._QiangGouItemList;
				}
				return qiangGouItemList;
			}
			set
			{
				lock (this)
				{
					this._QiangGouItemList = value;
				}
			}
		}

		
		
		
		public long ZhongDuStart
		{
			get
			{
				long zhongDuStart;
				lock (this)
				{
					zhongDuStart = this._ZhongDuStart;
				}
				return zhongDuStart;
			}
			set
			{
				lock (this)
				{
					this._ZhongDuStart = value;
				}
			}
		}

		
		
		
		public int ZhongDuSeconds
		{
			get
			{
				int zhongDuSeconds;
				lock (this)
				{
					zhongDuSeconds = this._ZhongDuSeconds;
				}
				return zhongDuSeconds;
			}
			set
			{
				lock (this)
				{
					this._ZhongDuSeconds = value;
				}
			}
		}

		
		
		
		public int FangDuRoleID
		{
			get
			{
				int fangDuRoleID;
				lock (this)
				{
					fangDuRoleID = this._FangDuRoleID;
				}
				return fangDuRoleID;
			}
			set
			{
				lock (this)
				{
					this._FangDuRoleID = value;
				}
			}
		}

		
		
		
		public long DSStartDSSubLifeNoShowTicks
		{
			get
			{
				long dsstartDSSubLifeNoShowTicks;
				lock (this)
				{
					dsstartDSSubLifeNoShowTicks = this._DSStartDSSubLifeNoShowTicks;
				}
				return dsstartDSSubLifeNoShowTicks;
			}
			set
			{
				lock (this)
				{
					this._DSStartDSSubLifeNoShowTicks = value;
				}
			}
		}

		
		
		
		public int JieriChengHao
		{
			get
			{
				int jieriChengHao;
				lock (this)
				{
					jieriChengHao = this._JieriChengHao;
				}
				return jieriChengHao;
			}
			set
			{
				lock (this)
				{
					this._JieriChengHao = value;
				}
			}
		}

		
		
		
		public long SpecialEquipLastUseTicks
		{
			get
			{
				long specialEquipLastUseTicks;
				lock (this)
				{
					specialEquipLastUseTicks = this._SpecialEquipLastUseTicks;
				}
				return specialEquipLastUseTicks;
			}
			set
			{
				lock (this)
				{
					this._SpecialEquipLastUseTicks = value;
				}
			}
		}

		
		
		
		public long DongJieStart
		{
			get
			{
				long dongJieStart;
				lock (this)
				{
					dongJieStart = this._DongJieStart;
				}
				return dongJieStart;
			}
			set
			{
				lock (this)
				{
					this._DongJieStart = value;
				}
			}
		}

		
		
		
		public int DongJieSeconds
		{
			get
			{
				int dongJieSeconds;
				lock (this)
				{
					dongJieSeconds = this._DongJieSeconds;
				}
				return dongJieSeconds;
			}
			set
			{
				lock (this)
				{
					this._DongJieSeconds = value;
				}
			}
		}

		
		public bool IsDongJie()
		{
			bool result;
			if (this.DongJieStart <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks < this.DongJieStart + (long)(this.DongJieSeconds * 1000));
			}
			return result;
		}

		
		
		public object PickUpGoodsPackMutex
		{
			get
			{
				object pickUpGoodsPackMutex;
				lock (this)
				{
					pickUpGoodsPackMutex = this._PickUpGoodsPackMutex;
				}
				return pickUpGoodsPackMutex;
			}
		}

		
		
		
		public long MeditateTicks
		{
			get
			{
				long meditateTicks;
				lock (this)
				{
					meditateTicks = this._MeditateTicks;
				}
				return meditateTicks;
			}
			set
			{
				lock (this)
				{
					this._MeditateTicks = value;
				}
			}
		}

		
		
		
		public int MeditateTime
		{
			get
			{
				int meditateTime;
				lock (this)
				{
					meditateTime = this._MeditateTime;
				}
				return meditateTime;
			}
			set
			{
				lock (this)
				{
					this._MeditateTime = value;
				}
			}
		}

		
		
		
		public int NotSafeMeditateTime
		{
			get
			{
				int notSafeMeditateTime;
				lock (this)
				{
					notSafeMeditateTime = this._NotSafeMeditateTime;
				}
				return notSafeMeditateTime;
			}
			set
			{
				lock (this)
				{
					this._NotSafeMeditateTime = value;
				}
			}
		}

		
		
		
		public int StartMeditate
		{
			get
			{
				int startMeditate;
				lock (this)
				{
					startMeditate = this._StartMeditate;
				}
				return startMeditate;
			}
			set
			{
				lock (this)
				{
					this._StartMeditate = value;
				}
			}
		}

		
		
		
		public int Last10sPosX { get; set; }

		
		
		
		public int Last10sPosY { get; set; }

		
		
		public object StoreYinLiangMutex
		{
			get
			{
				object storeYinLiangMutex;
				lock (this)
				{
					storeYinLiangMutex = this._StoreYinLiangMutex;
				}
				return storeYinLiangMutex;
			}
		}

		
		
		public object StoreMoneyMutex
		{
			get
			{
				object storeMoneyMutex;
				lock (this)
				{
					storeMoneyMutex = this._StoreMoneyMutex;
				}
				return storeMoneyMutex;
			}
		}

		
		
		
		public bool bIsInBloodCastleMap
		{
			get
			{
				return this._bIsInBloodCastleMap;
			}
			set
			{
				this._bIsInBloodCastleMap = value;
			}
		}

		
		
		
		public int BloodCastleAwardPoint
		{
			get
			{
				return this._BloodCastleAwardPoint;
			}
			set
			{
				this._BloodCastleAwardPoint = value;
			}
		}

		
		
		
		public int BloodCastleAwardPointTmp
		{
			get
			{
				return this._BloodCastleAwardPointTmp;
			}
			set
			{
				this._BloodCastleAwardPointTmp = value;
			}
		}

		
		
		
		public int BloodCastleAwardTotalPoint
		{
			get
			{
				return this._BloodCastleAwardTotalPoint;
			}
			set
			{
				this._BloodCastleAwardTotalPoint = value;
			}
		}

		
		
		
		public int CampBattleTotalPoint
		{
			get
			{
				return this._CampBattleTotalPoint;
			}
			set
			{
				this._CampBattleTotalPoint = value;
			}
		}

		
		
		
		public bool bIsInDaimonSquareMap
		{
			get
			{
				return this._bIsInDaimonSquareMap;
			}
			set
			{
				this._bIsInDaimonSquareMap = value;
			}
		}

		
		
		
		public int DaimonSquarePoint
		{
			get
			{
				return this._DaimonSquarePoint;
			}
			set
			{
				this._DaimonSquarePoint = value;
			}
		}

		
		
		
		public int DaimonSquarePointTotalPoint
		{
			get
			{
				return this._DaimonSquarePointTotalPoint;
			}
			set
			{
				this._DaimonSquarePointTotalPoint = value;
			}
		}

		
		
		
		public int KingOfPkCurrentPoint
		{
			get
			{
				return this._KingOfPkCurrentPoint;
			}
			set
			{
				this._KingOfPkCurrentPoint = value;
			}
		}

		
		
		
		public int KingOfPkTopPoint
		{
			get
			{
				return this._KingOfPkTopPoint;
			}
			set
			{
				this._KingOfPkTopPoint = value;
			}
		}

		
		
		
		public long AngelTempleCurrentPoint
		{
			get
			{
				return this._AngelTempleCurrentPoint;
			}
			set
			{
				this._AngelTempleCurrentPoint = value;
			}
		}

		
		
		
		public long AngelTempleTopPoint
		{
			get
			{
				return this._AngelTempleTopPoint;
			}
			set
			{
				this._AngelTempleTopPoint = value;
			}
		}

		
		
		
		public bool bIsInAngelTempleMap
		{
			get
			{
				return this._bIsInAngelTempleMap;
			}
			set
			{
				this._bIsInAngelTempleMap = value;
			}
		}

		
		
		
		public double[] ExcellenceProp
		{
			get
			{
				return this._ExcellenceProp;
			}
			set
			{
				this._ExcellenceProp = value;
			}
		}

		
		public void ResetExcellenceProp()
		{
			for (int i = 0; i < 32; i++)
			{
				this._ExcellenceProp[i] = 0.0;
			}
		}

		
		
		
		public double LuckProp
		{
			get
			{
				return this._LuckProp;
			}
			set
			{
				this._LuckProp = value;
			}
		}

		
		public void ResetLuckyProp()
		{
			this._LuckProp = 0.0;
		}

		
		
		
		public int DayOnlineSecond
		{
			get
			{
				return this._DayOnlineSecond;
			}
			set
			{
				this._DayOnlineSecond = value;
			}
		}

		
		
		
		public int BakDayOnlineSecond
		{
			get
			{
				return this._BakDayOnlineSecond;
			}
			set
			{
				this._BakDayOnlineSecond = value;
			}
		}

		
		
		
		public long DayOnlineRecSecond
		{
			get
			{
				return this._DayOnlineRecSecond;
			}
			set
			{
				this._DayOnlineRecSecond = value;
			}
		}

		
		
		
		public int SeriesLoginNum
		{
			get
			{
				return this._SeriesLoginNum;
			}
			set
			{
				this._SeriesLoginNum = value;
			}
		}

		
		
		
		public List<FallGoodsItem> EverydayOnlineAwardGiftList { get; set; }

		
		
		
		public List<FallGoodsItem> SeriesLoginAwardGiftList { get; set; }

		
		
		
		public int DailyActiveValues
		{
			get
			{
				int dailyActiveValues;
				lock (this)
				{
					dailyActiveValues = this._DailyActiveValues;
				}
				return dailyActiveValues;
			}
			set
			{
				lock (this)
				{
					this._DailyActiveValues = value;
				}
			}
		}

		
		
		
		public int DailyActiveDayID
		{
			get
			{
				int dailyActiveDayID;
				lock (this)
				{
					dailyActiveDayID = this._DailyActiveDayID;
				}
				return dailyActiveDayID;
			}
			set
			{
				lock (this)
				{
					this._DailyActiveDayID = value;
				}
			}
		}

		
		
		
		public uint DailyActiveDayLginCount
		{
			get
			{
				uint dailyActiveDayLginCount;
				lock (this)
				{
					dailyActiveDayLginCount = this._DailyActiveDayLginCount;
				}
				return dailyActiveDayLginCount;
			}
			set
			{
				lock (this)
				{
					this._DailyActiveDayLginCount = value;
				}
			}
		}

		
		
		
		public bool DailyActiveDayLginSetFlag
		{
			get
			{
				bool dailyActiveDayLginSetFlag;
				lock (this)
				{
					dailyActiveDayLginSetFlag = this._DailyActiveDayLginSetFlag;
				}
				return dailyActiveDayLginSetFlag;
			}
			set
			{
				lock (this)
				{
					this._DailyActiveDayLginSetFlag = value;
				}
			}
		}

		
		
		
		public int DailyActiveDayBuyItemInMall
		{
			get
			{
				int dailyActiveDayBuyItemInMall;
				lock (this)
				{
					dailyActiveDayBuyItemInMall = this._DailyActiveDayBuyItemInMall;
				}
				return dailyActiveDayBuyItemInMall;
			}
			set
			{
				lock (this)
				{
					this._DailyActiveDayBuyItemInMall = value;
				}
			}
		}

		
		
		
		public uint DailyTotalKillMonsterNum
		{
			get
			{
				uint dailyTotalKillMonsterNum;
				lock (this)
				{
					dailyTotalKillMonsterNum = this._DailyTotalKillMonsterNum;
				}
				return dailyTotalKillMonsterNum;
			}
			set
			{
				lock (this)
				{
					this._DailyTotalKillMonsterNum = value;
				}
			}
		}

		
		
		
		public uint DailyTotalKillKillBossNum
		{
			get
			{
				uint dailyTotalKillKillBossNum;
				lock (this)
				{
					dailyTotalKillKillBossNum = this._DailyTotalKillKillBossNum;
				}
				return dailyTotalKillKillBossNum;
			}
			set
			{
				lock (this)
				{
					this._DailyTotalKillKillBossNum = value;
				}
			}
		}

		
		
		
		public uint DailyCompleteDailyTaskCount
		{
			get
			{
				uint dailyCompleteDailyTaskCount;
				lock (this)
				{
					dailyCompleteDailyTaskCount = this._DailyCompleteDailyTaskCount;
				}
				return dailyCompleteDailyTaskCount;
			}
			set
			{
				lock (this)
				{
					this._DailyCompleteDailyTaskCount = value;
				}
			}
		}

		
		
		
		public uint DailyNextKillMonsterNum
		{
			get
			{
				uint dailyNextKillMonsterNum;
				lock (this)
				{
					dailyNextKillMonsterNum = this._DailyNextKillMonsterNum;
				}
				return dailyNextKillMonsterNum;
			}
			set
			{
				lock (this)
				{
					this._DailyNextKillMonsterNum = value;
				}
			}
		}

		
		
		
		public int DailyOnlineTimeTmp
		{
			get
			{
				int dailyOnlineTimeTmp;
				lock (this)
				{
					dailyOnlineTimeTmp = this._DailyOnlineTimeTmp;
				}
				return dailyOnlineTimeTmp;
			}
			set
			{
				lock (this)
				{
					this._DailyOnlineTimeTmp = value;
				}
			}
		}

		
		
		
		public bool AllowMarketBuy
		{
			get
			{
				return this._AllowMarketBuy;
			}
			set
			{
				this._AllowMarketBuy = value;
			}
		}

		
		
		
		public int OfflineMarketState
		{
			get
			{
				return this._OfflineMarketState;
			}
			set
			{
				this._OfflineMarketState = value;
			}
		}

		
		
		
		public string MarketName
		{
			get
			{
				return this._MarketName;
			}
			set
			{
				this._MarketName = value;
			}
		}

		
		
		
		public int VipLevel
		{
			get
			{
				int vipLevel;
				lock (this)
				{
					vipLevel = this._VipLevel;
				}
				return vipLevel;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.VIPLevel = value;
					this._VipLevel = value;
				}
			}
		}

		
		
		
		public int VipAwardFlag
		{
			get
			{
				int vipAwardFlag;
				lock (this)
				{
					vipAwardFlag = this._VipAwardFlag;
				}
				return vipAwardFlag;
			}
			set
			{
				lock (this)
				{
					this._VipAwardFlag = value;
				}
			}
		}

		
		
		
		public List<GoodsData> DailyOnLineAwardGift
		{
			get
			{
				List<GoodsData> dailyOnLineAwardGift;
				lock (this)
				{
					dailyOnLineAwardGift = this._DailyOnLineAwardGift;
				}
				return dailyOnLineAwardGift;
			}
			set
			{
				lock (this)
				{
					this._DailyOnLineAwardGift = value;
				}
			}
		}

		
		
		
		public List<GoodsData> SeriesLoginAwardGift
		{
			get
			{
				List<GoodsData> seriesLoginAwardGift;
				lock (this)
				{
					seriesLoginAwardGift = this._SeriesLoginAwardGift;
				}
				return seriesLoginAwardGift;
			}
			set
			{
				lock (this)
				{
					this._SeriesLoginAwardGift = value;
				}
			}
		}

		
		
		
		public int OpenGridTime
		{
			get
			{
				int openGridTime;
				lock (this)
				{
					openGridTime = this._OpenGridTime;
				}
				return openGridTime;
			}
			set
			{
				lock (this)
				{
					this._OpenGridTime = value;
				}
			}
		}

		
		
		
		public int OpenPortableGridTime
		{
			get
			{
				int openPortableGridTime;
				lock (this)
				{
					openPortableGridTime = this._OpenPortableGridTime;
				}
				return openPortableGridTime;
			}
			set
			{
				lock (this)
				{
					this._OpenPortableGridTime = value;
				}
			}
		}

		
		
		
		public Dictionary<int, int> PictureJudgeReferInfo
		{
			get
			{
				return this._RoleDataEx.RolePictureJudgeReferInfo;
			}
			set
			{
				this._RoleDataEx.RolePictureJudgeReferInfo = value;
			}
		}

		
		
		
		public WanMotaInfo WanMoTaProp
		{
			get
			{
				return this._WanMoTaProp;
			}
			set
			{
				this._WanMoTaProp = value;
			}
		}

		
		
		
		public LayerRewardData LayerRewardData
		{
			get
			{
				return this._LayerRewardData;
			}
			set
			{
				this._LayerRewardData = value;
			}
		}

		
		
		
		public SweepWanmota WanMoTaSweeping
		{
			get
			{
				return this._WanMoTaSweeping;
			}
			set
			{
				this._WanMoTaSweeping = value;
			}
		}

		
		
		
		public bool WaitingForChangeMap
		{
			get
			{
				bool waitingForChangeMap;
				lock (this)
				{
					waitingForChangeMap = this._WaitingForChangeMap;
				}
				return waitingForChangeMap;
			}
			set
			{
				lock (this)
				{
					this._WaitingForChangeMap = value;
				}
				this.WaitingNotifyChangeMap = false;
			}
		}

		
		
		
		public Dictionary<int, int> MoJingExchangeInfo
		{
			get
			{
				Dictionary<int, int> moJingExchangeInfo;
				lock (this)
				{
					moJingExchangeInfo = this._MoJingExchangeInfo;
				}
				return moJingExchangeInfo;
			}
			set
			{
				lock (this)
				{
					this._MoJingExchangeInfo = value;
				}
			}
		}

		
		
		
		public int MaxAntiProcessJiaSuSubNum
		{
			get
			{
				int maxAntiProcessJiaSuSubNum;
				lock (this)
				{
					maxAntiProcessJiaSuSubNum = this._MaxAntiProcessJiaSuSubNum;
				}
				return maxAntiProcessJiaSuSubNum;
			}
			set
			{
				lock (this)
				{
					this._MaxAntiProcessJiaSuSubNum = value;
				}
			}
		}

		
		
		
		public List<FuBenData> OldFuBenDataList
		{
			get
			{
				List<FuBenData> oldFuBenDataList;
				lock (this)
				{
					oldFuBenDataList = this._OldFuBenDataList;
				}
				return oldFuBenDataList;
			}
			set
			{
				lock (this)
				{
					this.OldFuBenDataList = value;
				}
			}
		}

		
		
		public object LockUnionPalace
		{
			get
			{
				return this._LockUnionPalace;
			}
		}

		
		
		
		public TalentData MyTalentData
		{
			get
			{
				TalentData myTalentData;
				lock (this)
				{
					myTalentData = this._RoleDataEx.MyTalentData;
				}
				return myTalentData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyTalentData = value;
				}
			}
		}

		
		
		
		public TalentPropData MyTalentPropData
		{
			get
			{
				TalentPropData myTalentPropData;
				lock (this)
				{
					myTalentPropData = this._myTalentPropData;
				}
				return myTalentPropData;
			}
			set
			{
				lock (this)
				{
					this._myTalentPropData = value;
				}
			}
		}

		
		
		
		public SpreadData MySpreadData
		{
			get
			{
				SpreadData mySpreadData;
				lock (this)
				{
					mySpreadData = this._mySpreadData;
				}
				return mySpreadData;
			}
			set
			{
				lock (this)
				{
					this._mySpreadData = value;
				}
			}
		}

		
		
		
		public SpreadVerifyData MySpreadVerifyData
		{
			get
			{
				SpreadVerifyData mySpreadVerifyData;
				lock (this)
				{
					mySpreadVerifyData = this._mySpreadVerifyData;
				}
				return mySpreadVerifyData;
			}
			set
			{
				lock (this)
				{
					this._mySpreadVerifyData = value;
				}
			}
		}

		
		
		public object LockSpread
		{
			get
			{
				return this._lockSpread;
			}
		}

		
		
		
		public Dictionary<int, int> RoleStarConstellationInfo
		{
			get
			{
				return this._RoleDataEx.RoleStarConstellationInfo;
			}
			set
			{
				this._RoleDataEx.RoleStarConstellationInfo = value;
			}
		}

		
		
		public StarConstellationProp RoleStarConstellationProp
		{
			get
			{
				return this._RoleStarConstellationProp;
			}
		}

		
		
		
		public int StarSoul
		{
			get
			{
				return this._StarSoul;
			}
			set
			{
				this._StarSoul = value;
			}
		}

		
		
		
		public int RoleYAngle
		{
			get
			{
				int roleYAngle;
				lock (this)
				{
					roleYAngle = this._RoleYAngle;
				}
				return roleYAngle;
			}
			set
			{
				lock (this)
				{
					this._RoleYAngle = value;
				}
			}
		}

		
		
		
		public uint DisMountTick
		{
			get
			{
				uint disMountTick;
				lock (this)
				{
					disMountTick = this._DisMountTick;
				}
				return disMountTick;
			}
			set
			{
				lock (this)
				{
					this._DisMountTick = value;
				}
			}
		}

		
		
		
		public uint CaiJiStartTick
		{
			get
			{
				uint caiJiStartTick;
				lock (this)
				{
					caiJiStartTick = this._CaiJiStartTick;
				}
				return caiJiStartTick;
			}
			set
			{
				lock (this)
				{
					this._CaiJiStartTick = value;
				}
			}
		}

		
		
		
		public int CaijTargetId
		{
			get
			{
				int caijTargetId;
				lock (this)
				{
					caijTargetId = this._CaijTargetId;
				}
				return caijTargetId;
			}
			set
			{
				lock (this)
				{
					this._CaijTargetId = value;
				}
			}
		}

		
		
		
		public int DailyCrystalCollectNum
		{
			get
			{
				int dailyCrystalCollectNum;
				lock (this)
				{
					dailyCrystalCollectNum = this._DailyCrystalCollectNum;
				}
				return dailyCrystalCollectNum;
			}
			set
			{
				lock (this)
				{
					this._DailyCrystalCollectNum = value;
				}
			}
		}

		
		
		
		public int CrystalCollectDayID
		{
			get
			{
				int crystalCollectDayID;
				lock (this)
				{
					crystalCollectDayID = this._CrystalCollectDayID;
				}
				return crystalCollectDayID;
			}
			set
			{
				lock (this)
				{
					this._CrystalCollectDayID = value;
				}
			}
		}

		
		
		
		public int LingDiCaiJiNum
		{
			get
			{
				int lingDiCaiJiNum;
				lock (this)
				{
					lingDiCaiJiNum = this._LingDiCaiJiNum;
				}
				return lingDiCaiJiNum;
			}
			set
			{
				lock (this)
				{
					this._LingDiCaiJiNum = value;
				}
			}
		}

		
		
		
		public int OnlineActiveVal
		{
			get
			{
				int onlineActiveVal;
				lock (this)
				{
					onlineActiveVal = this._OnlineActiveVal;
				}
				return onlineActiveVal;
			}
			set
			{
				lock (this)
				{
					this._OnlineActiveVal = value;
				}
			}
		}

		
		
		
		public Dictionary<int, LingYuData> LingYuDict
		{
			get
			{
				Dictionary<int, LingYuData> lingYuDict;
				lock (this)
				{
					lingYuDict = this._RoleDataEx.LingYuDict;
				}
				return lingYuDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.LingYuDict = value;
				}
			}
		}

		
		
		
		public GuardStatueDetail MyGuardStatueDetail
		{
			get
			{
				GuardStatueDetail myGuardStatueDetail;
				lock (this)
				{
					myGuardStatueDetail = this._RoleDataEx.MyGuardStatueDetail;
				}
				return myGuardStatueDetail;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyGuardStatueDetail = value;
				}
			}
		}

		
		
		
		public long ShuiJingHuanJingTicks
		{
			get
			{
				long shuiJingHuanJingTicks;
				lock (this)
				{
					shuiJingHuanJingTicks = this._ShuiJingHuanJingTicks;
				}
				return shuiJingHuanJingTicks;
			}
			set
			{
				lock (this)
				{
					this._ShuiJingHuanJingTicks = value;
				}
			}
		}

		
		
		
		public long GetLiPinMaTicks
		{
			get
			{
				long getLiPinMaTicks;
				lock (this)
				{
					getLiPinMaTicks = this._GetLiPinMaTicks;
				}
				return getLiPinMaTicks;
			}
			set
			{
				lock (this)
				{
					this._GetLiPinMaTicks = value;
				}
			}
		}

		
		
		
		public KingOfBattleStoreData KOBattleStoreData
		{
			get
			{
				KingOfBattleStoreData kingOfBattleStroeData;
				lock (this)
				{
					kingOfBattleStroeData = this._KingOfBattleStroeData;
				}
				return kingOfBattleStroeData;
			}
			set
			{
				lock (this)
				{
					this._KingOfBattleStroeData = value;
				}
			}
		}

		
		
		
		public int MagicSwordParam
		{
			get
			{
				int magicSwordParam;
				lock (this)
				{
					magicSwordParam = this._RoleDataEx.MagicSwordParam;
				}
				return magicSwordParam;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MagicSwordParam = value;
				}
			}
		}

		
		public void AddAwardRecord(RoleAwardMsg type, MoneyTypes moneyType, int count)
		{
			this.AddAwardRecord(type, GoodsUtil.GetResGoodsData(moneyType, count), false);
		}

		
		public void AddAwardRecord(RoleAwardMsg type, List<GoodsData> goodsDataList, bool useOld = false)
		{
			if (goodsDataList != null)
			{
				foreach (GoodsData item in goodsDataList)
				{
					this.AddAwardRecord(type, item, useOld);
				}
			}
		}

		
		public void AddAwardRecord(RoleAwardMsg type, string goodsDataListString, bool useOld = false)
		{
			if (!string.IsNullOrEmpty(goodsDataListString))
			{
				AwardsItemList list = new AwardsItemList();
				list.Add(goodsDataListString);
				this.AddAwardRecord(type, Global.ConvertToGoodsDataList(list.Items, -1), useOld);
			}
		}

		
		public void AddAwardRecord(RoleAwardMsg type, AwardsItemList list, bool useOld = false)
		{
			this.AddAwardRecord(type, Global.ConvertToGoodsDataList(list.Items, -1), useOld);
		}

		
		public void AddAwardRecord(RoleAwardMsg type, GoodsData goodsData, bool useOld = false)
		{
			if (null != goodsData)
			{
				if (this._TmpAwardRecordDict.ContainsKey(type))
				{
					if (useOld)
					{
						this._TmpAwardRecordDict[type].Add(goodsData);
					}
					else
					{
						this._TmpAwardRecordDict[type].Add(goodsData);
					}
				}
				else
				{
					List<GoodsData> tempList = new List<GoodsData>();
					tempList.Add(goodsData);
					this._TmpAwardRecordDict[type] = tempList;
				}
			}
		}

		
		public List<GoodsData> GetAwardRecord(RoleAwardMsg type)
		{
			List<GoodsData> result;
			if (this._TmpAwardRecordDict.ContainsKey(type))
			{
				result = this._TmpAwardRecordDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public void ClearAwardRecord(RoleAwardMsg type)
		{
			if (this._TmpAwardRecordDict.ContainsKey(type))
			{
				this._TmpAwardRecordDict[type].Clear();
				if (type == this.RoleAwardMsgType)
				{
					this.RoleAwardMsgType = RoleAwardMsg.None;
				}
			}
		}

		
		
		
		public DateTime LastLoginTime
		{
			get
			{
				return this._LastLoginTime;
			}
			set
			{
				this._LastLoginTime = value;
			}
		}

		
		
		
		public MarriageData MyMarriageData
		{
			get
			{
				MarriageData myMarriageData;
				lock (this)
				{
					myMarriageData = this._RoleDataEx.MyMarriageData;
				}
				return myMarriageData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyMarriageData = value;
				}
			}
		}

		
		
		
		public Dictionary<int, int> MyMarryPartyJoinList
		{
			get
			{
				Dictionary<int, int> myMarryPartyJoinList;
				lock (this)
				{
					myMarryPartyJoinList = this._RoleDataEx.MyMarryPartyJoinList;
				}
				return myMarryPartyJoinList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MyMarryPartyJoinList = value;
				}
			}
		}

		
		
		
		public List<int> GroupMailRecordList
		{
			get
			{
				List<int> groupMailRecordList;
				lock (this)
				{
					groupMailRecordList = this._RoleDataEx.GroupMailRecordList;
				}
				return groupMailRecordList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.GroupMailRecordList = value;
				}
			}
		}

		
		
		public RoleTianTiData TianTiData
		{
			get
			{
				return this._RoleDataEx.TianTiData;
			}
		}

		
		
		
		public MerlinGrowthSaveDBData MerlinData
		{
			get
			{
				MerlinGrowthSaveDBData merlinData;
				lock (this)
				{
					merlinData = this._RoleDataEx.MerlinData;
				}
				return merlinData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MerlinData = value;
				}
			}
		}

		
		
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic
		{
			get
			{
				Dictionary<sbyte, HolyItemData> myHolyItemDataDic;
				lock (this)
				{
					myHolyItemDataDic = this._RoleDataEx.MyHolyItemDataDic;
				}
				return myHolyItemDataDic;
			}
		}

		
		
		public TarotSystemData TarotData
		{
			get
			{
				TarotSystemData tarotData;
				lock (this)
				{
					tarotData = this._RoleDataEx.TarotData;
				}
				return tarotData;
			}
		}

		
		
		
		public JueXingShiData JueXingData
		{
			get
			{
				JueXingShiData jueXingData;
				lock (this)
				{
					jueXingData = this._RoleDataEx.JueXingData;
				}
				return jueXingData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JueXingData = value;
				}
			}
		}

		
		
		
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData
		{
			get
			{
				JingLingYuanSuJueXingData jingLingYuanSuJueXingData;
				lock (this)
				{
					jingLingYuanSuJueXingData = this._RoleDataEx.JingLingYuanSuJueXingData;
				}
				return jingLingYuanSuJueXingData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.JingLingYuanSuJueXingData = value;
				}
			}
		}

		
		
		
		public FluorescentGemData FluorescentGemData
		{
			get
			{
				FluorescentGemData fluorescentGemData;
				lock (this)
				{
					fluorescentGemData = this._RoleDataEx.FluorescentGemData;
				}
				return fluorescentGemData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FluorescentGemData = value;
				}
			}
		}

		
		
		
		public int FluorescentPoint
		{
			get
			{
				int fluorescentPoint;
				lock (this)
				{
					fluorescentPoint = this._RoleDataEx.FluorescentPoint;
				}
				return fluorescentPoint;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.FluorescentPoint = value;
				}
			}
		}

		
		
		
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict
		{
			get
			{
				Dictionary<int, Dictionary<int, SevenDayItemData>> sevenDayActDict;
				lock (this)
				{
					sevenDayActDict = this._RoleDataEx.SevenDayActDict;
				}
				return sevenDayActDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SevenDayActDict = value;
				}
			}
		}

		
		
		
		public List<GoodsData> SoulStoneInBag
		{
			get
			{
				List<GoodsData> soulStonesInBag;
				lock (this)
				{
					soulStonesInBag = this._RoleDataEx.SoulStonesInBag;
				}
				return soulStonesInBag;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SoulStonesInBag = value;
				}
			}
		}

		
		
		
		public List<GoodsData> SoulStoneInUsing
		{
			get
			{
				List<GoodsData> soulStonesInUsing;
				lock (this)
				{
					soulStonesInUsing = this._RoleDataEx.SoulStonesInUsing;
				}
				return soulStonesInUsing;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SoulStonesInUsing = value;
				}
			}
		}

		
		
		
		public long BanTradeToTicks
		{
			get
			{
				long bantTradeToTicks;
				lock (this)
				{
					bantTradeToTicks = this._RoleDataEx.BantTradeToTicks;
				}
				return bantTradeToTicks;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.BantTradeToTicks = value;
				}
			}
		}

		
		
		
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict
		{
			get
			{
				Dictionary<int, SpecActInfoDB> specActInfoDict;
				lock (this)
				{
					specActInfoDict = this._RoleDataEx.SpecActInfoDict;
				}
				return specActInfoDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SpecActInfoDict = value;
				}
			}
		}

		
		
		
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict
		{
			get
			{
				Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> specPriorityActInfoDict;
				lock (this)
				{
					specPriorityActInfoDict = this._RoleDataEx.SpecPriorityActInfoDict;
				}
				return specPriorityActInfoDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.SpecPriorityActInfoDict = value;
				}
			}
		}

		
		
		
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict
		{
			get
			{
				Dictionary<int, EverydayActInfoDB> everydayActInfoDict;
				lock (this)
				{
					everydayActInfoDict = this._RoleDataEx.EverydayActInfoDict;
				}
				return everydayActInfoDict;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.EverydayActInfoDict = value;
				}
			}
		}

		
		
		
		public Dictionary<int, int> ChargeItemPurchaseDict
		{
			get
			{
				Dictionary<int, int> chargeItemPurchaseDict;
				lock (this)
				{
					chargeItemPurchaseDict = this._ChargeItemPurchaseDict;
				}
				return chargeItemPurchaseDict;
			}
			set
			{
				lock (this)
				{
					this._ChargeItemPurchaseDict = value;
				}
			}
		}

		
		
		
		public Dictionary<int, int> ChargeItemDayPurchaseDict
		{
			get
			{
				Dictionary<int, int> chargeItemDayPurchaseDict;
				lock (this)
				{
					chargeItemDayPurchaseDict = this._ChargeItemDayPurchaseDict;
				}
				return chargeItemDayPurchaseDict;
			}
			set
			{
				lock (this)
				{
					this._ChargeItemDayPurchaseDict = value;
				}
			}
		}

		
		
		public object LockFund
		{
			get
			{
				return this._LockFund;
			}
		}

		
		
		
		public List<BHMatchSupportData> BHMatchSupportList
		{
			get
			{
				List<BHMatchSupportData> bhmatchSupportList;
				lock (this)
				{
					bhmatchSupportList = this._BHMatchSupportList;
				}
				return bhmatchSupportList;
			}
			set
			{
				lock (this)
				{
					this._BHMatchSupportList = value;
				}
			}
		}

		
		
		
		public long FuMoMoney
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[146];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[146] = value;
				}
			}
		}

		
		
		
		public long RebornLevelUpPoint
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[151];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[151] = value;
				}
			}
		}

		
		
		
		public long RebornCuiLian
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[152];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[152] = value;
				}
			}
		}

		
		
		
		public long RebornDuanZao
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[153];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[153] = value;
				}
			}
		}

		
		
		
		public long RebornNiePan
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[154];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[154] = value;
				}
			}
		}

		
		
		
		public long RebornFengYin
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[155];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[155] = value;
				}
			}
		}

		
		
		
		public long RebornChongSheng
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[156];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[156] = value;
				}
			}
		}

		
		
		
		public long RebornXuanCai
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[157];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[157] = value;
				}
			}
		}

		
		
		
		public long RebornEquipHole
		{
			get
			{
				long result;
				lock (this)
				{
					result = this.MoneyData[161];
				}
				return result;
			}
			set
			{
				lock (this)
				{
					this.MoneyData[161] = value;
				}
			}
		}

		
		
		
		public List<GoodsData> RebornGoodsDataList
		{
			get
			{
				List<GoodsData> rebornGoodsDataList;
				lock (this)
				{
					rebornGoodsDataList = this._RoleDataEx.RebornGoodsDataList;
				}
				return rebornGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornGoodsDataList = value;
				}
			}
		}

		
		
		
		public int RebornBagNum
		{
			get
			{
				return this._RoleDataEx.RebornBagNum;
			}
			set
			{
				this._RoleDataEx.RebornBagNum = value;
			}
		}

		
		
		
		public int OpenRebornBagTime
		{
			get
			{
				int openRebornBagTime;
				lock (this)
				{
					openRebornBagTime = this._OpenRebornBagTime;
				}
				return openRebornBagTime;
			}
			set
			{
				lock (this)
				{
					this._OpenRebornBagTime = value;
				}
			}
		}

		
		
		
		public RebornStampData RebornYinJi
		{
			get
			{
				RebornStampData rebornYinJi;
				lock (this)
				{
					rebornYinJi = this._RoleDataEx.RebornYinJi;
				}
				return rebornYinJi;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornYinJi = value;
				}
			}
		}

		
		
		
		public int OpenRebornGridTime
		{
			get
			{
				int openRebornGridTime;
				lock (this)
				{
					openRebornGridTime = this._OpenRebornGridTime;
				}
				return openRebornGridTime;
			}
			set
			{
				lock (this)
				{
					this._OpenRebornGridTime = value;
				}
			}
		}

		
		
		
		public List<GoodsData> RebornGoodsStoreList
		{
			get
			{
				List<GoodsData> rebornGoodsStoreList;
				lock (this)
				{
					rebornGoodsStoreList = this._RoleDataEx.RebornGoodsStoreList;
				}
				return rebornGoodsStoreList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornGoodsStoreList = value;
				}
			}
		}

		
		
		
		public int RebornShowEquip
		{
			get
			{
				return this._RoleDataEx.RebornShowEquip;
			}
			set
			{
				this._RoleDataEx.RebornShowEquip = value;
			}
		}

		
		
		
		public int RebornShowModel
		{
			get
			{
				return this._RoleDataEx.RebornShowModel;
			}
			set
			{
				this._RoleDataEx.RebornShowModel = value;
			}
		}

		
		
		
		public List<GoodsData> HolyGoodsDataList
		{
			get
			{
				List<GoodsData> holyGoodsDataList;
				lock (this)
				{
					holyGoodsDataList = this._RoleDataEx.HolyGoodsDataList;
				}
				return holyGoodsDataList;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.HolyGoodsDataList = value;
				}
			}
		}

		
		
		
		public Dictionary<int, RebornEquipData> RebornEquipHoleInfo
		{
			get
			{
				Dictionary<int, RebornEquipData> rebornEquipHoleData;
				lock (this)
				{
					rebornEquipHoleData = this._RoleDataEx.RebornEquipHoleData;
				}
				return rebornEquipHoleData;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornEquipHoleData = value;
				}
			}
		}

		
		
		
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo
		{
			get
			{
				Dictionary<int, MazingerStoreData> mazingerStoreDataInfo;
				lock (this)
				{
					mazingerStoreDataInfo = this._RoleDataEx.MazingerStoreDataInfo;
				}
				return mazingerStoreDataInfo;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.MazingerStoreDataInfo = value;
				}
			}
		}

		
		
		
		public int RebornCount
		{
			get
			{
				int rebornCount;
				lock (this)
				{
					rebornCount = this._RoleDataEx.RebornCount;
				}
				return rebornCount;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornCount = value;
				}
			}
		}

		
		
		
		public int RebornLevel
		{
			get
			{
				int rebornLevel;
				lock (this)
				{
					rebornLevel = this._RoleDataEx.RebornLevel;
				}
				return rebornLevel;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornLevel = value;
				}
			}
		}

		
		
		
		public long RebornExperience
		{
			get
			{
				long rebornExperience;
				lock (this)
				{
					rebornExperience = this._RoleDataEx.RebornExperience;
				}
				return rebornExperience;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornExperience = value;
				}
			}
		}

		
		
		
		public int RebornCombatForce
		{
			get
			{
				int rebornCombatForce;
				lock (this)
				{
					rebornCombatForce = this._RoleDataEx.RebornCombatForce;
				}
				return rebornCombatForce;
			}
			set
			{
				lock (this)
				{
					this._RoleDataEx.RebornCombatForce = value;
				}
			}
		}

		
		
		public string WorldRoleID
		{
			get
			{
				return this._RoleDataEx.WorldRoleID;
			}
		}

		
		
		public int UserPTID
		{
			get
			{
				return this._RoleDataEx.UserPTID;
			}
		}

		
		
		public int ServerPTID
		{
			get
			{
				return this._RoleDataEx.ServerPTID;
			}
		}

		
		
		public string Channel
		{
			get
			{
				return this._RoleDataEx.Channel;
			}
		}

		
		public SafeClientData()
		{
			long[] addFriendTicks = new long[3];
			this._AddFriendTicks = addFriendTicks;
			this._RoleDataEx = null;
			this.AllyList = null;
			this.LockAlly = new object();
			this.ClientExtData = new ClientExtData();
			this.SkillIdHashSet = new HashSet<int>();
			this.BufferDataListHashSet = new HashSet<int>();
			this._CopyMapAwardTmpGoods = null;
			this.LastPKPoint = 0;
			this._TmpPKPoint = 0;
			this._AdorationCount = 0;
			this._PKKingAdorationCount = 0;
			this._JingJiNextRewardTime = -1L;
			this._PKKingAdorationDayID = 0;
			this.PropPointMutex = new object();
			this._TotalPropPoint = 0;
			this.LastNotifyCombatForce = -1;
			this._PropStrength = 0;
			this._PropIntelligence = 0;
			this._PropDexterity = 0;
			this._PropConstitution = 0;
			this._RoleChangeLifeProp = new ChangeLifeProp();
			this._ReportPosTicks = 0L;
			this._ServerPosTicks = 0L;
			this._CurrentAction = 0;
			this._MoveSpeed = 1.0;
			this._DestPoint = new Point(-1.0, -1.0);
			this.AddLifeAlertList = new Queue<string>();
			this._LastMapCode = -1;
			this._LastPosX = -1;
			this._LastPosY = -1;
			this._StallDataItem = null;
			this._LastDBHeartTicks = TimeUtil.NOW();
			this._LastSiteExpTicks = TimeUtil.NOW();
			this._LastSiteSubPKPointTicks = 0L;
			this._AutoFighting = false;
			this._LastAutoFightTicks = 0L;
			this._AutoFightingProctect = 0;
			this._DJRoomID = -1;
			this._DJRoomTeamID = -1;
			this._ViewDJRoomDlg = false;
			this._CopyMapID = -1;
			this._GoodsPackItem = null;
			this._SelectHorseDbID = -1;
			this._PetRoleID = -1;
			this._PortableGoodsDataList = null;
			this._JinDanGoodsDataList = null;
			this._MeditateGoodsDataList = null;
			this._ReportPetPosTicks = 0L;
			this._PetPosX = 0;
			this._PetPosY = 0;
			this._EquipProp = new EquipPropItem();
			this._WeighItems = new WeighItems();
			this._LastCheckGridX = -1;
			this._LastCheckGridY = -1;
			this._BattleKilledNum = 0;
			this._ArenaBattleKilledNum = 0;
			this._HideSelf = 0;
			this._HideGM = 0;
			this._TrackingRoleIDList = new List<int>();
			this._LastJugeSafeRegionTicks = 0L;
			this._AntiAddictionTimeType = 0;
			this._JingMaiPropsDict = null;
			this._JingMaiBodyLevel = 1;
			this._FirstPlayStart = true;
			this._LastProcessBufferTicks = 0L;
			this._UpLifeLimitBufferData = null;
			this._AddTempAttackBufferData = null;
			this._AddTempDefenseBufferData = null;
			this._AntiBossBufferData = null;
			this._SheLiZhiYuanBufferData = null;
			this._DiWangZhiYouBufferData = null;
			this._JunQiBufferData = null;
			this._TempJMChongXueRate = 1;
			this._TempHorseEnchanceRate = 1;
			this._TempHorseUpLevelRate = 1;
			this._AutoFightGetThings = 0;
			this._LastDBCmdTicksDict = new Dictionary<int, long>();
			this._ClientHeartCount = 0;
			this._LastClientHeartTicks = TimeUtil.NOW();
			this._LastClientServerSubTicks = 0L;
			this._LastClientServerSubNum = 0;
			this._ClosingClientStep = 0;
			this._NumSkillData = null;
			this._DefaultSkillLev = 1;
			this._DefaultSkillUseNum = 0;
			this._LastDBSkillCmdTicksDict = new Dictionary<int, long>();
			this._WaBaoGoodsData = null;
			this._UserMoneyMutex = new object();
			this._YinLiangMutex = new object();
			this._GoldMutex = new object();
			this._FuBenSeqID = -1;
			this._FuBenID = -1;
			this.OnePieceMoveLeft = 0;
			this.OnePieceMoveDir = 0;
			this.OnePieceTempEventID = 0;
			this._OnePieceBoxIDList = null;
			this._FallBaoXiangGoodsList = null;
			this.YaoSaiPrisonOptType = YaoSaiOptType.zhengfu;
			this.YaoSaiPrisonTargetID = 0;
			this.YaoSaiPrisonTargetName = "";
			this.DisableChangeRolePurpleName = true;
			this._StartPurpleNameTicks = 0L;
			this.StartLianZhanTicks = 0L;
			this.WaitingLianZhanMS = 0L;
			this.TempLianZhan = 0;
			this._TotalLearnedSkillLevelCount = 0;
			this._LastProcessMapLimitTimesTicks = TimeUtil.NOW();
			this._RoleEquipJiFen = 0;
			this._RoleXueWeiNum = 0;
			this._RoleHorseJiFen = 0;
			this._QueueCmdItemList = new List<QueueCmdItem>();
			this._ChongXueFailedNum = 0;
			this._StartTempHorseIDTicks = 0L;
			this._TempHorseID = 0;
			this._LoginDayID = TimeUtil.NowDateTime().DayOfYear;
			this._AllQualityIndex = 0;
			this._AllForgeLevelIndex = 0;
			this._AllJewelLevelIndex = 0;
			this._AllZhuoYueNum = 0;
			this._AllThingsCalcItem = new AllThingsCalcItem();
			this._MyYangGongBKItem = null;
			this._QiZhenGeGoodsDict = null;
			this._QiZhenGeBuyNum = 0;
			this._EnterMapTicks = 0L;
			this._TotalUsedMoney = 0;
			this._TotalGoodsMoney = 0;
			this._ReportWarningGoodsMoney = 0;
			this._LastAttackTicks = 0L;
			this._ForceShenFenZheng = false;
			this._FSHuDunStart = 0L;
			this._FSHuDunSeconds = 0;
			this._DSHideStart = 0L;
			this._WaitingNotifyChangeMap = false;
			this._BattleWhichSide = 0;
			this._ThisTimeOnlineSecs = 0;
			this._MagicCoolDownMgr = new MagicCoolDownMgr();
			this._LastSkillID = 0;
			this._GoodsCoolDownMgr = new GoodsCoolDownMgr();
			this._MailSendSecurityCode = "";
			this._RoleStartMoveTicks = 0L;
			this._RolePathString = "";
			this._TengXunFCMRate = 1.0;
			this._LastDBRoleParamCmdTicksDict = new Dictionary<string, long>();
			this._LastDBEquipStrongCmdTicksDict = new Dictionary<int, long>();
			this._TotalKilledMonsterNum = 0U;
			this._TimerKilledMonsterNum = 0;
			this._NextKillMonsterChengJiuNum = 0U;
			this._MaxTongQianNum = -1;
			this._NextTongQianChengJiuNum = 0U;
			this._TotalDayLoginNum = 0U;
			this._ContinuousDayLoginNum = 0U;
			this._ChengJiuPoints = 0;
			this._ChengJiuLevel = 0;
			this._ShenLiJingHuaPoints = 0;
			this._NengLiangSmall = 0;
			this._NengLiangMedium = 0;
			this._NengLiangBig = 0;
			this._NengLiangSuper = 0;
			this._FuWenZhiChen = 0;
			this._WanMoTaNextLayerOrder = 0;
			this.ShenShiEquipData = null;
			this.PassiveEffectList = new List<int>();
			this._SummonMonstersList = new List<Monster>();
			this._StartAddExpTicks = 0L;
			this._BufferDataDict = new Dictionary<int, BufferData>();
			this._StartAddLifeMagicTicks = 0L;
			this._StartAddLifeNoShowTicks = 0L;
			this._StartAddMaigcNoShowTicks = 0L;
			this._DSStartDSAddLifeNoShowTicks = 0L;
			this._LastLogRoleIDAttackebByMyselfTicks = 0L;
			this._RoleIDAttackebByMyself = 0;
			this._LastLogRoleIDAttackMeTicks = 0L;
			this._RoleIDAttackMe = 0;
			this._RoleCommonUseIntPamams = new List<int>();
			this._LastMapLimitUpdateTicks = TimeUtil.NOW();
			this._LastHintToUpdateClientTicks = TimeUtil.NOW();
			this._BaseBattleAttributesOfLastTime = new int[3];
			this._LastGoodsLimitUpdateTicks = TimeUtil.NOW();
			this._LastFashionLimitUpdateTicks = TimeUtil.NOW();
			this._LastRoleDeadTicks = TimeUtil.NOW();
			this._MoveAndActionNum = 0;
			this._VisibleGrid9Objects = new Dictionary<object, byte>();
			this._VisibleMeGrid9GameClients = new Dictionary<object, byte>();
			this._QiangGouItemList = null;
			this._ZhongDuStart = 0L;
			this._ZhongDuSeconds = 0;
			this._FangDuRoleID = 0;
			this._DSStartDSSubLifeNoShowTicks = 0L;
			this._JieriChengHao = 0;
			this._SpecialEquipLastUseTicks = 0L;
			this._DongJieStart = 0L;
			this._DongJieSeconds = 0;
			this._PickUpGoodsPackMutex = new object();
			this._MeditateTicks = TimeUtil.NOW();
			this.LastProcessDeadTicks = 0L;
			this._MeditateTime = 0;
			this._NotSafeMeditateTime = 0;
			this._StartMeditate = 0;
			this.LastMovePosTicks = 0L;
			this._StoreYinLiangMutex = new object();
			this._StoreMoneyMutex = new object();
			this._bIsInBloodCastleMap = false;
			this._BloodCastleAwardPoint = 0;
			this._BloodCastleAwardPointTmp = 0;
			this._BloodCastleAwardTotalPoint = 0;
			this._CampBattleTotalPoint = 0;
			this._bIsInDaimonSquareMap = false;
			this._DaimonSquarePoint = 0;
			this._DaimonSquarePointTotalPoint = 0;
			this._KingOfPkCurrentPoint = 0;
			this._KingOfPkTopPoint = 0;
			this._AngelTempleCurrentPoint = 0L;
			this._AngelTempleTopPoint = 0L;
			this._bIsInAngelTempleMap = false;
			this._ExcellenceProp = new double[32];
			this._LuckProp = 0.0;
			this._DayOnlineSecond = 10;
			this._BakDayOnlineSecond = 10;
			this._DayOnlineRecSecond = 10L;
			this._SeriesLoginNum = 0;
			this._DailyActiveValues = 0;
			this._DailyActiveDayID = 0;
			this._DailyActiveDayLginCount = 0U;
			this._DailyActiveDayLginSetFlag = false;
			this._DailyActiveDayBuyItemInMall = 0;
			this._DailyTotalKillMonsterNum = 0U;
			this._DailyTotalKillKillBossNum = 0U;
			this._DailyCompleteDailyTaskCount = 0U;
			this._DailyNextKillMonsterNum = 0U;
			this._DailyOnlineTimeTmp = 60;
			this._AllowMarketBuy = false;
			this._OfflineMarketState = 0;
			this._MarketName = "";
			this._VipLevel = 0;
			this._VipAwardFlag = 0;
			this._DailyOnLineAwardGift = null;
			this._SeriesLoginAwardGift = null;
			this._OpenGridTime = 0;
			this._OpenPortableGridTime = 0;
			this.ActivedTuJianItem = new HashSet<int>();
			this.ActivedTuJianType = new HashSet<int>();
			this._LayerRewardData = null;
			this._WanMoTaSweeping = null;
			this.MapCodeAlreadyList = new List<int>();
			this._WaitingForChangeMap = false;
			this.MoJingExchangeDayID = 0;
			this._MoJingExchangeInfo = null;
			this._MaxAntiProcessJiaSuSubNum = 0;
			this.TempWashPropOperationIndex = 0;
			this.TempWashPropsDict = new Dictionary<int, UpdateGoodsArgs>();
			this.nTempWorldLevelPer = 0.0;
			this.ExtensionProps = new SpriteExtensionProps();
			this.YesterdayDailyTaskData = null;
			this.YesterdayTaofaTaskData = null;
			this.OldResourceInfoDict = null;
			this._OldFuBenDataList = new List<FuBenData>();
			this.achievementRuneData = null;
			this.shenQiData = null;
			this.prestigeMedalData = null;
			this.MyUnionPalaceData = null;
			this._LockUnionPalace = new object();
			this.EveryDayUpDate = 0;
			this.ReturnData = null;
			this._myTalentPropData = new TalentPropData();
			this._mySpreadData = new SpreadData();
			this._mySpreadVerifyData = new SpreadVerifyData();
			this._lockSpread = new object();
			this._RoleStarConstellationProp = new StarConstellationProp();
			this._StarSoul = 0;
			this._RoleYAngle = 0;
			this._DisMountTick = 0U;
			this._CaiJiStartTick = 0U;
			this._CaijTargetId = 0;
			this.gatherNpcID = 0;
			this.gatherTicks = 0L;
			this._DailyCrystalCollectNum = 0;
			this._CrystalCollectDayID = 0;
			this.OldCrystalCollectData = null;
			this._LingDiCaiJiNum = 0;
			this._OnlineActiveVal = 0;
			this.YKDetail = new YueKaDetail();
			this._ShuiJingHuanJingTicks = TimeUtil.NOW() * 10000L;
			this._GetLiPinMaTicks = 0L;
			this._TmpAwardRecordDict = new Dictionary<RoleAwardMsg, List<GoodsData>>();
			this._ReplaceExtArg = new ReplaceExtArg();
			this.MoneyData = new LongCollection();
			this.IsSoulStoneOpened = false;
			this.SpecPriorityActMutex = new object();
			this.ChargeItemMutex = new object();
			this._ChargeItemPurchaseDict = new Dictionary<int, int>();
			this.ChargeItemDayPurchaseDayID = 0;
			this._ChargeItemDayPurchaseDict = new Dictionary<int, int>();
			this.MyFundData = new FundData();
			this._LockFund = new object();
			this.ZhengBaSupportFlags = new List<ZhengBaSupportFlagData>();
			this._BHMatchSupportList = new List<BHMatchSupportData>();
			this._OpenRebornBagTime = 0;
			this._OpenRebornGridTime = 0;
			this.UpdateHongBaoLogTicks = new long[3];
			this.HongBaoLogLists = new List<HongBaoItemData>[3];
			this.DeControlItemArray = new DeControlItemData[177];
        }

		
		public int LastRoleCommonUseIntParamValueListTickCount = 0;

		
		public long _ResetBagTicks = 0L;

		
		public long _RefreshMarketTicks = 0L;

		
		public long _SpriteFightTicks = 0L;

		
		public long _AddBHMemberTicks = 0L;

		
		public long[] _AddFriendTicks;

		
		private RoleDataEx _RoleDataEx;

		
		public int OccupationIndex;

		
		public bool IsMainOccupation;

		
		public int AttackType;

		
		public List<AllyData> AllyList;

		
		public object LockAlly;

		
		public int LingDi;

		
		public long LastJunTuanChatTicks;

		
		public long LastJunTuanBulletinTicks;

		
		public long LastCompChatTicks;

		
		public int EventLastMapCode;

		
		public ClientExtData ClientExtData;

		
		public HashSet<int> SkillIdHashSet;

		
		public HashSet<int> BufferDataListHashSet;

		
		public string platType;

		
		public string launch;

		
		public GoodsData _CopyMapAwardTmpGoods;

		
		public int FuBenPingFenAwardMoJing;

		
		public int LastPKPoint;

		
		private int _TmpPKPoint;

		
		private int _AdorationCount;

		
		private int _PKKingAdorationCount;

		
		private long _JingJiNextRewardTime;

		
		private int _PKKingAdorationDayID;

		
		public object PropPointMutex;

		
		private int _TotalPropPoint;

		
		public int LastNotifyCombatForce;

		
		public long MaxCombatForce;

		
		public int NextCombatForceGiftVal;

		
		private int _PropStrength;

		
		private int _PropIntelligence;

		
		private int _PropDexterity;

		
		private int _PropConstitution;

		
		private ChangeLifeProp _RoleChangeLifeProp;

		
		private long _ReportPosTicks;

		
		private long _ServerPosTicks;

		
		private int _CurrentAction;

		
		private double _MoveSpeed;

		
		private Point _DestPoint;

		
		public int MinLife;

		
		private int _CurrentLifeV;

		
		public Queue<string> AddLifeAlertList;

		
		private int _CurrentMagicV;

		
		public int ArmorV;

		
		public int CurrentArmorV;

		
		public double ArmorPercent;

		
		private int _TeamID;

		
		private int _ExchangeID;

		
		private long _ExchangeTicks;

		
		private int _LastMapCode;

		
		private int _LastPosX;

		
		private int _LastPosY;

		
		private StallData _StallDataItem;

		
		private long _LastDBHeartTicks;

		
		private long _LastSiteExpTicks;

		
		private long _LastSiteSubPKPointTicks;

		
		private bool _AutoFighting;

		
		private long _LastAutoFightTicks;

		
		private int _AutoFightingProctect;

		
		private int _DJRoomID;

		
		private int _DJRoomTeamID;

		
		private bool _ViewDJRoomDlg;

		
		private int _CopyMapID;

		
		private GoodsPackItem _GoodsPackItem;

		
		private int _SelectHorseDbID;

		
		private int _PetRoleID;

		
		public List<GoodsData> _PortableGoodsDataList;

		
		public List<GoodsData> _JinDanGoodsDataList;

		
		public List<GoodsData> _MeditateGoodsDataList;

		
		private List<MountData> _MountList;

		
		private int _IsRide;

		
		private long _ReportPetPosTicks;

		
		private int _PetPosX;

		
		private int _PetPosY;

		
		private EquipPropItem _EquipProp;

		
		public PropsCacheManager PropsCacheManager;

		
		public PropsCacheManager PurePropsCacheManager;

		
		public PropsCacheManager PctPropsCacheManager;

		
		private WeighItems _WeighItems;

		
		public int _LastCheckGridX;

		
		public int _LastCheckGridY;

		
		private int _BattleKilledNum;

		
		private int _ArenaBattleKilledNum;

		
		private int _HideSelf;

		
		private int _HideGM;

		
		public int GuanZhanGM;

		
		private int _BeTrackingRoleID;

		
		private List<int> _TrackingRoleIDList;

		
		public long _LastJugeSafeRegionTicks;

		
		private int _AntiAddictionTimeType;

		
		private Dictionary<string, int> _JingMaiPropsDict;

		
		private int _JingMaiBodyLevel;

		
		private bool _FirstPlayStart;

		
		private long _LastProcessBufferTicks;

		
		private BufferData _UpLifeLimitBufferData;

		
		private BufferData _AddTempAttackBufferData;

		
		private BufferData _AddTempDefenseBufferData;

		
		private BufferData _AntiBossBufferData;

		
		private BufferData _SheLiZhiYuanBufferData;

		
		private BufferData _DiWangZhiYouBufferData;

		
		private BufferData _JunQiBufferData;

		
		private int _TempJMChongXueRate;

		
		private int _TempHorseEnchanceRate;

		
		private int _TempHorseUpLevelRate;

		
		private int _AutoFightGetThings;

		
		private Dictionary<int, long> _LastDBCmdTicksDict;

		
		private int _ClientHeartCount;

		
		private long _LastClientHeartTicks;

		
		private long _LastClientServerSubTicks;

		
		private int _LastClientServerSubNum;

		
		private int _ClosingClientStep;

		
		private SkillData _NumSkillData;

		
		private int _DefaultSkillLev;

		
		private int _DefaultSkillUseNum;

		
		private Dictionary<int, long> _LastDBSkillCmdTicksDict;

		
		private GoodsData _WaBaoGoodsData;

		
		private object _UserMoneyMutex;

		
		private object _YinLiangMutex;

		
		private object _GoldMutex;

		
		private int _FuBenSeqID;

		
		private int _FuBenID;

		
		public int NotifyFuBenID;

		
		public int NotifyFuBenSeqID;

		
		public int OnePieceMoveLeft;

		
		public int OnePieceMoveDir;

		
		public int OnePieceTempEventID;

		
		public List<int> _OnePieceBoxIDList;

		
		public List<GoodsData> _FallBaoXiangGoodsList;

		
		public YaoSaiOptType YaoSaiPrisonOptType;

		
		public int YaoSaiPrisonTargetID;

		
		public string YaoSaiPrisonTargetName;

		
		public bool DisableChangeRolePurpleName;

		
		private long _StartPurpleNameTicks;

		
		public long StartLianZhanTicks;

		
		public long WaitingLianZhanMS;

		
		public int TempLianZhan;

		
		public double LianZhanExpRate;

		
		private int _TotalLearnedSkillLevelCount;

		
		private long _LastProcessMapLimitTimesTicks;

		
		private int _RoleEquipJiFen;

		
		private int _RoleXueWeiNum;

		
		private int _RoleHorseJiFen;

		
		private List<QueueCmdItem> _QueueCmdItemList;

		
		private int _ChongXueFailedNum;

		
		private long _StartTempHorseIDTicks;

		
		private int _TempHorseID;

		
		private int _LoginDayID;

		
		private int _AllQualityIndex;

		
		private int _AllForgeLevelIndex;

		
		private int _AllJewelLevelIndex;

		
		private int _AllZhuoYueNum;

		
		private AllThingsCalcItem _AllThingsCalcItem;

		
		public YangGongBKItem _MyYangGongBKItem;

		
		public Dictionary<int, QiZhenGeItemData> _QiZhenGeGoodsDict;

		
		private int _QiZhenGeBuyNum;

		
		private long _EnterMapTicks;

		
		private int _TotalUsedMoney;

		
		private int _TotalGoodsMoney;

		
		private int _ReportWarningGoodsMoney;

		
		private long _LastAttackTicks;

		
		private bool _ForceShenFenZheng;

		
		private long _FSHuDunStart;

		
		private int _FSHuDunSeconds;

		
		private long _DSHideStart;

		
		private bool _WaitingNotifyChangeMap;

		
		public int WaitingChangeMapToMapCode;

		
		public int WaitingChangeMapToPosX;

		
		public int WaitingChangeMapToPosY;

		
		public int KuaFuChangeMapCode;

		
		private int _BattleWhichSide;

		
		public int BirthSide;

		
		private int _ThisTimeOnlineSecs;

		
		private MagicCoolDownMgr _MagicCoolDownMgr;

		
		private int _LastSkillID;

		
		private GoodsCoolDownMgr _GoodsCoolDownMgr;

		
		public int CurrentMagicCode;

		
		public long CurrentMagicTicks;

		
		public double CurrentMagicCDSubPercent;

		
		public long CurrentMagicActionEndTicks;

		
		public SpriteActionData CurrentActionData;

		
		private string _MailSendSecurityCode;

		
		private long _RoleStartMoveTicks;

		
		public long _InstantMoveTick;

		
		private string _RolePathString;

		
		private double _TengXunFCMRate;

		
		private Dictionary<string, long> _LastDBRoleParamCmdTicksDict;

		
		private Dictionary<int, long> _LastDBEquipStrongCmdTicksDict;

		
		private uint _TotalKilledMonsterNum;

		
		private ushort _TimerKilledMonsterNum;

		
		private uint _NextKillMonsterChengJiuNum;

		
		private int _MaxTongQianNum;

		
		private uint _NextTongQianChengJiuNum;

		
		private uint _TotalDayLoginNum;

		
		private uint _ContinuousDayLoginNum;

		
		private int _ChengJiuPoints;

		
		private int _ChengJiuLevel;

		
		private int _ShenLiJingHuaPoints;

		
		private int _NengLiangSmall;

		
		private int _NengLiangMedium;

		
		private int _NengLiangBig;

		
		private int _NengLiangSuper;

		
		private int _FuWenZhiChen;

		
		private int _WanMoTaNextLayerOrder;

		
		public SkillEquipData ShenShiEquipData;

		
		public List<int> PassiveEffectList;

		
		public List<Monster> _SummonMonstersList;

		
		private long _StartAddExpTicks;

		
		private Dictionary<int, BufferData> _BufferDataDict;

		
		private long _StartAddLifeMagicTicks;

		
		private long _StartAddLifeNoShowTicks;

		
		private long _StartAddMaigcNoShowTicks;

		
		private long _DSStartDSAddLifeNoShowTicks;

		
		private long _LastLogRoleIDAttackebByMyselfTicks;

		
		private int _RoleIDAttackebByMyself;

		
		private long _LastLogRoleIDAttackMeTicks;

		
		private int _RoleIDAttackMe;

		
		public List<int> _RoleCommonUseIntPamams;

		
		private long _LastMapLimitUpdateTicks;

		
		private long _LastHintToUpdateClientTicks;

		
		private int[] _BaseBattleAttributesOfLastTime;

		
		private long _LastGoodsLimitUpdateTicks;

		
		private long _LastFashionLimitUpdateTicks;

		
		private long _LastRoleDeadTicks;

		
		private int _MoveAndActionNum;

		
		public Dictionary<object, byte> _VisibleGrid9Objects;

		
		public Dictionary<object, byte> _VisibleMeGrid9GameClients;

		
		private List<QiangGouItemData> _QiangGouItemList;

		
		private long _ZhongDuStart;

		
		private int _ZhongDuSeconds;

		
		private int _FangDuRoleID;

		
		private long _DSStartDSSubLifeNoShowTicks;

		
		private int _JieriChengHao;

		
		private long _SpecialEquipLastUseTicks;

		
		private long _DongJieStart;

		
		private int _DongJieSeconds;

		
		private object _PickUpGoodsPackMutex;

		
		private long _MeditateTicks;

		
		public long GiveMeditateAwardOffsetTicks;

		
		public int GiveMeditateGoodsInterval;

		
		public long LastProcessDeadTicks;

		
		private int _MeditateTime;

		
		private int _NotSafeMeditateTime;

		
		private int _StartMeditate;

		
		public long LastMovePosTicks;

		
		private object _StoreYinLiangMutex;

		
		private object _StoreMoneyMutex;

		
		private bool _bIsInBloodCastleMap;

		
		private int _BloodCastleAwardPoint;

		
		private int _BloodCastleAwardPointTmp;

		
		private int _BloodCastleAwardTotalPoint;

		
		private int _CampBattleTotalPoint;

		
		private bool _bIsInDaimonSquareMap;

		
		private int _DaimonSquarePoint;

		
		private int _DaimonSquarePointTotalPoint;

		
		private int _KingOfPkCurrentPoint;

		
		private int _KingOfPkTopPoint;

		
		private long _AngelTempleCurrentPoint;

		
		public long m_NotifyInfoTickForSingle;

		
		private long _AngelTempleTopPoint;

		
		private bool _bIsInAngelTempleMap;

		
		private double[] _ExcellenceProp;

		
		private double _LuckProp;

		
		private int _DayOnlineSecond;

		
		private int _BakDayOnlineSecond;

		
		private long _DayOnlineRecSecond;

		
		public int _SeriesLoginNum;

		
		private int _DailyActiveValues;

		
		private int _DailyActiveDayID;

		
		private uint _DailyActiveDayLginCount;

		
		private bool _DailyActiveDayLginSetFlag;

		
		private int _DailyActiveDayBuyItemInMall;

		
		private uint _DailyTotalKillMonsterNum;

		
		private uint _DailyTotalKillKillBossNum;

		
		private uint _DailyCompleteDailyTaskCount;

		
		private uint _DailyNextKillMonsterNum;

		
		private int _DailyOnlineTimeTmp;

		
		private bool _AllowMarketBuy;

		
		private int _OfflineMarketState;

		
		private string _MarketName;

		
		public long VipExp;

		
		private int _VipLevel;

		
		private int _VipAwardFlag;

		
		private List<GoodsData> _DailyOnLineAwardGift;

		
		private List<GoodsData> _SeriesLoginAwardGift;

		
		private int _OpenGridTime;

		
		private int _OpenPortableGridTime;

		
		public Point OpenPortableBagPoint;

		
		public HashSet<int> ActivedTuJianItem;

		
		public HashSet<int> ActivedTuJianType;

		
		private WanMotaInfo _WanMoTaProp;

		
		private LayerRewardData _LayerRewardData;

		
		private SweepWanmota _WanMoTaSweeping;

		
		public List<int> MapCodeAlreadyList;

		
		private bool _WaitingForChangeMap;

		
		public SceneUIClasses SceneType;

		
		public int SceneMapCode;

		
		public int MoJingExchangeDayID;

		
		private Dictionary<int, int> _MoJingExchangeInfo;

		
		private int _MaxAntiProcessJiaSuSubNum;

		
		public int CompleteTaskZhangJie;

		
		public long LastNotifyChangeMapTicks;

		
		public long LastChangeMapTicks;

		
		public int TempWashPropOperationIndex;

		
		public Dictionary<int, UpdateGoodsArgs> TempWashPropsDict;

		
		public double nTempWorldLevelPer;

		
		public SpriteExtensionProps ExtensionProps;

		
		public DailyTaskData YesterdayDailyTaskData;

		
		public DailyTaskData YesterdayTaofaTaskData;

		
		public Dictionary<int, OldResourceInfo> OldResourceInfoDict;

		
		private List<FuBenData> _OldFuBenDataList;

		
		public AchievementRuneData achievementRuneData;

		
		public ShenQiData shenQiData;

		
		public PrestigeMedalData prestigeMedalData;

		
		public UnionPalaceData MyUnionPalaceData;

		
		private object _LockUnionPalace;

		
		public int EveryDayUpDate;

		
		public UserReturnData ReturnData;

		
		private TalentPropData _myTalentPropData;

		
		private SpreadData _mySpreadData;

		
		private SpreadVerifyData _mySpreadVerifyData;

		
		private object _lockSpread;

		
		private StarConstellationProp _RoleStarConstellationProp;

		
		public int _StarSoul;

		
		private int _RoleYAngle;

		
		private uint _DisMountTick;

		
		private uint _CaiJiStartTick;

		
		private int _CaijTargetId;

		
		public long CaiJiTargetUniqueID;

		
		public long CaijGoodsDBId;

		
		public int gatherNpcID;

		
		public long gatherTicks;

		
		private int _DailyCrystalCollectNum;

		
		private int _CrystalCollectDayID;

		
		public OldCaiJiData OldCrystalCollectData;

		
		public int _LingDiCaiJiNum;

		
		private int _OnlineActiveVal;

		
		public YueKaDetail YKDetail;

		
		private long _ShuiJingHuanJingTicks;

		
		private long _GetLiPinMaTicks;

		
		private KingOfBattleStoreData _KingOfBattleStroeData;

		
		public KuaFuLueDuoStoreData KuaFuLueDuoStoreData;

		
		public RoleAwardMsg RoleAwardMsgType;

		
		private Dictionary<RoleAwardMsg, List<GoodsData>> _TmpAwardRecordDict;

		
		private DateTime _LastLoginTime;

		
		public ReplaceExtArg _ReplaceExtArg;

		
		public int SignUpGameType;

		
		public TianTi5v5ZhanDuiRoleData TianTi5v5Data;

		
		public int LangHunLingYuCityAwardsCheckDayId;

		
		public int LangHunLingYuCityAwardsLevelFlags;

		
		public int LangHunLingYuCityAwardsLevelFlagsSelf;

		
		public int LangHunLingYuCityAwardsDay;

		
		public bool LangHunLingYuCityAwardsCanGet;

		
		public long ZhengDuoDataAge;

		
		public LongCollection MoneyData;

		
		public ZuoQiMainData ZuoQiMainData;

		
		public bool IsSoulStoneOpened;

		
		public object SpecPriorityActMutex;

		
		public object ChargeItemMutex;

		
		private Dictionary<int, int> _ChargeItemPurchaseDict;

		
		public int ChargeItemDayPurchaseDayID;

		
		private Dictionary<int, int> _ChargeItemDayPurchaseDict;

		
		public FundData MyFundData;

		
		private object _LockFund;

		
		public List<ZhengBaSupportFlagData> ZhengBaSupportFlags;

		
		private List<BHMatchSupportData> _BHMatchSupportList;

		
		private int _OpenRebornBagTime;

		
		private int _OpenRebornGridTime;

		
		public long[] UpdateHongBaoLogTicks;

		
		public List<HongBaoItemData>[] HongBaoLogLists;

		
		public int LocalRoleID;

		
		public DeControlItemData[] DeControlItemArray;

		
		public GVoiceTypes GVoiceType;

		
		public string GVoicePrioritys;
	}
}
