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
	// Token: 0x020007C9 RID: 1993
	public class SafeClientData
	{
		// Token: 0x060034B5 RID: 13493 RVA: 0x002EB96C File Offset: 0x002E9B6C
		public RoleDataEx GetRoleData()
		{
			return this._RoleDataEx;
		}

		// Token: 0x170003C9 RID: 969
		// (set) Token: 0x060034B6 RID: 13494 RVA: 0x002EB984 File Offset: 0x002E9B84
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

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060034B7 RID: 13495 RVA: 0x002EB9CC File Offset: 0x002E9BCC
		public int RoleID
		{
			get
			{
				return this._RoleDataEx.RoleID;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060034B8 RID: 13496 RVA: 0x002EB9EC File Offset: 0x002E9BEC
		public string RoleName
		{
			get
			{
				return this._RoleDataEx.RoleName;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x060034B9 RID: 13497 RVA: 0x002EBA0C File Offset: 0x002E9C0C
		public int RoleSex
		{
			get
			{
				return this._RoleDataEx.RoleSex;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060034BA RID: 13498 RVA: 0x002EBA2C File Offset: 0x002E9C2C
		// (set) Token: 0x060034BB RID: 13499 RVA: 0x002EBA49 File Offset: 0x002E9C49
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

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060034BC RID: 13500 RVA: 0x002EBA58 File Offset: 0x002E9C58
		// (set) Token: 0x060034BD RID: 13501 RVA: 0x002EBA75 File Offset: 0x002E9C75
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

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060034BE RID: 13502 RVA: 0x002EBA84 File Offset: 0x002E9C84
		public List<int> OccupationList
		{
			get
			{
				return this._RoleDataEx.OccupationList;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060034BF RID: 13503 RVA: 0x002EBAA4 File Offset: 0x002E9CA4
		// (set) Token: 0x060034C0 RID: 13504 RVA: 0x002EBAC3 File Offset: 0x002E9CC3
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

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060034C1 RID: 13505 RVA: 0x002EBAD4 File Offset: 0x002E9CD4
		// (set) Token: 0x060034C2 RID: 13506 RVA: 0x002EBAF1 File Offset: 0x002E9CF1
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

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060034C3 RID: 13507 RVA: 0x002EBB00 File Offset: 0x002E9D00
		// (set) Token: 0x060034C4 RID: 13508 RVA: 0x002EBB1D File Offset: 0x002E9D1D
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

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060034C5 RID: 13509 RVA: 0x002EBB2C File Offset: 0x002E9D2C
		// (set) Token: 0x060034C6 RID: 13510 RVA: 0x002EBB4B File Offset: 0x002E9D4B
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

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060034C7 RID: 13511 RVA: 0x002EBB5C File Offset: 0x002E9D5C
		// (set) Token: 0x060034C8 RID: 13512 RVA: 0x002EBB79 File Offset: 0x002E9D79
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

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x060034C9 RID: 13513 RVA: 0x002EBB88 File Offset: 0x002E9D88
		// (set) Token: 0x060034CA RID: 13514 RVA: 0x002EBBA5 File Offset: 0x002E9DA5
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

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x060034CB RID: 13515 RVA: 0x002EBBB4 File Offset: 0x002E9DB4
		// (set) Token: 0x060034CC RID: 13516 RVA: 0x002EBBD1 File Offset: 0x002E9DD1
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

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x060034CD RID: 13517 RVA: 0x002EBBE0 File Offset: 0x002E9DE0
		// (set) Token: 0x060034CE RID: 13518 RVA: 0x002EBBFD File Offset: 0x002E9DFD
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

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x060034CF RID: 13519 RVA: 0x002EBC0C File Offset: 0x002E9E0C
		// (set) Token: 0x060034D0 RID: 13520 RVA: 0x002EBC29 File Offset: 0x002E9E29
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

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060034D1 RID: 13521 RVA: 0x002EBC38 File Offset: 0x002E9E38
		// (set) Token: 0x060034D2 RID: 13522 RVA: 0x002EBC57 File Offset: 0x002E9E57
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

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060034D3 RID: 13523 RVA: 0x002EBC68 File Offset: 0x002E9E68
		// (set) Token: 0x060034D4 RID: 13524 RVA: 0x002EBC87 File Offset: 0x002E9E87
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

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060034D5 RID: 13525 RVA: 0x002EBC98 File Offset: 0x002E9E98
		// (set) Token: 0x060034D6 RID: 13526 RVA: 0x002EBCBA File Offset: 0x002E9EBA
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

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060034D7 RID: 13527 RVA: 0x002EBCD0 File Offset: 0x002E9ED0
		// (set) Token: 0x060034D8 RID: 13528 RVA: 0x002EBCEF File Offset: 0x002E9EEF
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

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060034D9 RID: 13529 RVA: 0x002EBD00 File Offset: 0x002E9F00
		// (set) Token: 0x060034DA RID: 13530 RVA: 0x002EBD1F File Offset: 0x002E9F1F
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

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060034DB RID: 13531 RVA: 0x002EBD30 File Offset: 0x002E9F30
		// (set) Token: 0x060034DC RID: 13532 RVA: 0x002EBD4F File Offset: 0x002E9F4F
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

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x060034DD RID: 13533 RVA: 0x002EBD60 File Offset: 0x002E9F60
		// (set) Token: 0x060034DE RID: 13534 RVA: 0x002EBD7F File Offset: 0x002E9F7F
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

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x060034DF RID: 13535 RVA: 0x002EBD90 File Offset: 0x002E9F90
		// (set) Token: 0x060034E0 RID: 13536 RVA: 0x002EBDB0 File Offset: 0x002E9FB0
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

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x060034E1 RID: 13537 RVA: 0x002EBDE8 File Offset: 0x002E9FE8
		// (set) Token: 0x060034E2 RID: 13538 RVA: 0x002EBE07 File Offset: 0x002EA007
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

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x060034E3 RID: 13539 RVA: 0x002EBE18 File Offset: 0x002EA018
		// (set) Token: 0x060034E4 RID: 13540 RVA: 0x002EBE38 File Offset: 0x002EA038
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

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x060034E5 RID: 13541 RVA: 0x002EBEC8 File Offset: 0x002EA0C8
		// (set) Token: 0x060034E6 RID: 13542 RVA: 0x002EBEE8 File Offset: 0x002EA0E8
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

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x060034E7 RID: 13543 RVA: 0x002EBF78 File Offset: 0x002EA178
		// (set) Token: 0x060034E8 RID: 13544 RVA: 0x002EBFC4 File Offset: 0x002EA1C4
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

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x060034E9 RID: 13545 RVA: 0x002EC010 File Offset: 0x002EA210
		// (set) Token: 0x060034EA RID: 13546 RVA: 0x002EC02F File Offset: 0x002EA22F
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

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x060034EB RID: 13547 RVA: 0x002EC040 File Offset: 0x002EA240
		// (set) Token: 0x060034EC RID: 13548 RVA: 0x002EC05F File Offset: 0x002EA25F
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

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x060034ED RID: 13549 RVA: 0x002EC070 File Offset: 0x002EA270
		// (set) Token: 0x060034EE RID: 13550 RVA: 0x002EC0BC File Offset: 0x002EA2BC
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

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x060034EF RID: 13551 RVA: 0x002EC108 File Offset: 0x002EA308
		// (set) Token: 0x060034F0 RID: 13552 RVA: 0x002EC154 File Offset: 0x002EA354
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

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x060034F1 RID: 13553 RVA: 0x002EC1A0 File Offset: 0x002EA3A0
		// (set) Token: 0x060034F2 RID: 13554 RVA: 0x002EC1EC File Offset: 0x002EA3EC
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

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x060034F3 RID: 13555 RVA: 0x002EC238 File Offset: 0x002EA438
		// (set) Token: 0x060034F4 RID: 13556 RVA: 0x002EC284 File Offset: 0x002EA484
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

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x060034F5 RID: 13557 RVA: 0x002EC2D0 File Offset: 0x002EA4D0
		// (set) Token: 0x060034F6 RID: 13558 RVA: 0x002EC31C File Offset: 0x002EA51C
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

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x060034F7 RID: 13559 RVA: 0x002EC368 File Offset: 0x002EA568
		// (set) Token: 0x060034F8 RID: 13560 RVA: 0x002EC3B4 File Offset: 0x002EA5B4
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

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x060034F9 RID: 13561 RVA: 0x002EC400 File Offset: 0x002EA600
		// (set) Token: 0x060034FA RID: 13562 RVA: 0x002EC44C File Offset: 0x002EA64C
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

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x060034FB RID: 13563 RVA: 0x002EC498 File Offset: 0x002EA698
		// (set) Token: 0x060034FC RID: 13564 RVA: 0x002EC4E4 File Offset: 0x002EA6E4
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

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x060034FD RID: 13565 RVA: 0x002EC530 File Offset: 0x002EA730
		// (set) Token: 0x060034FE RID: 13566 RVA: 0x002EC57C File Offset: 0x002EA77C
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

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x060034FF RID: 13567 RVA: 0x002EC5C8 File Offset: 0x002EA7C8
		// (set) Token: 0x06003500 RID: 13568 RVA: 0x002EC614 File Offset: 0x002EA814
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

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06003501 RID: 13569 RVA: 0x002EC660 File Offset: 0x002EA860
		// (set) Token: 0x06003502 RID: 13570 RVA: 0x002EC6AC File Offset: 0x002EA8AC
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

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06003503 RID: 13571 RVA: 0x002EC6F8 File Offset: 0x002EA8F8
		// (set) Token: 0x06003504 RID: 13572 RVA: 0x002EC744 File Offset: 0x002EA944
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

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06003505 RID: 13573 RVA: 0x002EC790 File Offset: 0x002EA990
		// (set) Token: 0x06003506 RID: 13574 RVA: 0x002EC7DC File Offset: 0x002EA9DC
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

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06003507 RID: 13575 RVA: 0x002EC828 File Offset: 0x002EAA28
		// (set) Token: 0x06003508 RID: 13576 RVA: 0x002EC874 File Offset: 0x002EAA74
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

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06003509 RID: 13577 RVA: 0x002EC8C0 File Offset: 0x002EAAC0
		// (set) Token: 0x0600350A RID: 13578 RVA: 0x002EC90C File Offset: 0x002EAB0C
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

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x0600350B RID: 13579 RVA: 0x002EC958 File Offset: 0x002EAB58
		// (set) Token: 0x0600350C RID: 13580 RVA: 0x002EC9A4 File Offset: 0x002EABA4
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

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x0600350D RID: 13581 RVA: 0x002EC9F0 File Offset: 0x002EABF0
		// (set) Token: 0x0600350E RID: 13582 RVA: 0x002ECA3C File Offset: 0x002EAC3C
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

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x0600350F RID: 13583 RVA: 0x002ECA88 File Offset: 0x002EAC88
		// (set) Token: 0x06003510 RID: 13584 RVA: 0x002ECAD4 File Offset: 0x002EACD4
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

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06003511 RID: 13585 RVA: 0x002ECB20 File Offset: 0x002EAD20
		// (set) Token: 0x06003512 RID: 13586 RVA: 0x002ECB6C File Offset: 0x002EAD6C
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

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06003513 RID: 13587 RVA: 0x002ECBB8 File Offset: 0x002EADB8
		// (set) Token: 0x06003514 RID: 13588 RVA: 0x002ECC04 File Offset: 0x002EAE04
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

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06003515 RID: 13589 RVA: 0x002ECC50 File Offset: 0x002EAE50
		// (set) Token: 0x06003516 RID: 13590 RVA: 0x002ECC9C File Offset: 0x002EAE9C
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

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06003517 RID: 13591 RVA: 0x002ECCE8 File Offset: 0x002EAEE8
		// (set) Token: 0x06003518 RID: 13592 RVA: 0x002ECD34 File Offset: 0x002EAF34
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

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06003519 RID: 13593 RVA: 0x002ECD80 File Offset: 0x002EAF80
		// (set) Token: 0x0600351A RID: 13594 RVA: 0x002ECDCC File Offset: 0x002EAFCC
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

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x0600351B RID: 13595 RVA: 0x002ECE18 File Offset: 0x002EB018
		// (set) Token: 0x0600351C RID: 13596 RVA: 0x002ECE64 File Offset: 0x002EB064
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

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x0600351D RID: 13597 RVA: 0x002ECEB0 File Offset: 0x002EB0B0
		// (set) Token: 0x0600351E RID: 13598 RVA: 0x002ECEFC File Offset: 0x002EB0FC
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

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x0600351F RID: 13599 RVA: 0x002ECF48 File Offset: 0x002EB148
		// (set) Token: 0x06003520 RID: 13600 RVA: 0x002ECF94 File Offset: 0x002EB194
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

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06003521 RID: 13601 RVA: 0x002ECFE0 File Offset: 0x002EB1E0
		// (set) Token: 0x06003522 RID: 13602 RVA: 0x002ED02C File Offset: 0x002EB22C
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

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06003523 RID: 13603 RVA: 0x002ED078 File Offset: 0x002EB278
		// (set) Token: 0x06003524 RID: 13604 RVA: 0x002ED0C4 File Offset: 0x002EB2C4
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

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06003525 RID: 13605 RVA: 0x002ED110 File Offset: 0x002EB310
		// (set) Token: 0x06003526 RID: 13606 RVA: 0x002ED15C File Offset: 0x002EB35C
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

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06003527 RID: 13607 RVA: 0x002ED1A8 File Offset: 0x002EB3A8
		// (set) Token: 0x06003528 RID: 13608 RVA: 0x002ED1F4 File Offset: 0x002EB3F4
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

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06003529 RID: 13609 RVA: 0x002ED240 File Offset: 0x002EB440
		// (set) Token: 0x0600352A RID: 13610 RVA: 0x002ED28C File Offset: 0x002EB48C
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

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600352B RID: 13611 RVA: 0x002ED2D8 File Offset: 0x002EB4D8
		// (set) Token: 0x0600352C RID: 13612 RVA: 0x002ED324 File Offset: 0x002EB524
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

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x0600352D RID: 13613 RVA: 0x002ED370 File Offset: 0x002EB570
		// (set) Token: 0x0600352E RID: 13614 RVA: 0x002ED3BC File Offset: 0x002EB5BC
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

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x0600352F RID: 13615 RVA: 0x002ED408 File Offset: 0x002EB608
		// (set) Token: 0x06003530 RID: 13616 RVA: 0x002ED454 File Offset: 0x002EB654
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

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06003531 RID: 13617 RVA: 0x002ED4A0 File Offset: 0x002EB6A0
		// (set) Token: 0x06003532 RID: 13618 RVA: 0x002ED4EC File Offset: 0x002EB6EC
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

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06003533 RID: 13619 RVA: 0x002ED538 File Offset: 0x002EB738
		// (set) Token: 0x06003534 RID: 13620 RVA: 0x002ED584 File Offset: 0x002EB784
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

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06003535 RID: 13621 RVA: 0x002ED5D0 File Offset: 0x002EB7D0
		// (set) Token: 0x06003536 RID: 13622 RVA: 0x002ED61C File Offset: 0x002EB81C
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

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06003537 RID: 13623 RVA: 0x002ED668 File Offset: 0x002EB868
		// (set) Token: 0x06003538 RID: 13624 RVA: 0x002ED6B4 File Offset: 0x002EB8B4
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

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06003539 RID: 13625 RVA: 0x002ED700 File Offset: 0x002EB900
		// (set) Token: 0x0600353A RID: 13626 RVA: 0x002ED74C File Offset: 0x002EB94C
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

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x0600353B RID: 13627 RVA: 0x002ED798 File Offset: 0x002EB998
		// (set) Token: 0x0600353C RID: 13628 RVA: 0x002ED7E4 File Offset: 0x002EB9E4
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

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x0600353D RID: 13629 RVA: 0x002ED830 File Offset: 0x002EBA30
		// (set) Token: 0x0600353E RID: 13630 RVA: 0x002ED878 File Offset: 0x002EBA78
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

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x0600353F RID: 13631 RVA: 0x002ED8C0 File Offset: 0x002EBAC0
		// (set) Token: 0x06003540 RID: 13632 RVA: 0x002ED90C File Offset: 0x002EBB0C
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

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06003541 RID: 13633 RVA: 0x002ED958 File Offset: 0x002EBB58
		// (set) Token: 0x06003542 RID: 13634 RVA: 0x002ED9A4 File Offset: 0x002EBBA4
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

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06003543 RID: 13635 RVA: 0x002ED9F0 File Offset: 0x002EBBF0
		// (set) Token: 0x06003544 RID: 13636 RVA: 0x002EDA38 File Offset: 0x002EBC38
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

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06003545 RID: 13637 RVA: 0x002EDA80 File Offset: 0x002EBC80
		// (set) Token: 0x06003546 RID: 13638 RVA: 0x002EDACC File Offset: 0x002EBCCC
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

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06003547 RID: 13639 RVA: 0x002EDB18 File Offset: 0x002EBD18
		// (set) Token: 0x06003548 RID: 13640 RVA: 0x002EDB64 File Offset: 0x002EBD64
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

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06003549 RID: 13641 RVA: 0x002EDBB0 File Offset: 0x002EBDB0
		// (set) Token: 0x0600354A RID: 13642 RVA: 0x002EDBFC File Offset: 0x002EBDFC
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

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x0600354B RID: 13643 RVA: 0x002EDC48 File Offset: 0x002EBE48
		// (set) Token: 0x0600354C RID: 13644 RVA: 0x002EDC94 File Offset: 0x002EBE94
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

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x0600354D RID: 13645 RVA: 0x002EDCE0 File Offset: 0x002EBEE0
		// (set) Token: 0x0600354E RID: 13646 RVA: 0x002EDD2C File Offset: 0x002EBF2C
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

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x0600354F RID: 13647 RVA: 0x002EDD78 File Offset: 0x002EBF78
		// (set) Token: 0x06003550 RID: 13648 RVA: 0x002EDDC4 File Offset: 0x002EBFC4
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

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06003551 RID: 13649 RVA: 0x002EDE10 File Offset: 0x002EC010
		// (set) Token: 0x06003552 RID: 13650 RVA: 0x002EDE5C File Offset: 0x002EC05C
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

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06003553 RID: 13651 RVA: 0x002EDEA8 File Offset: 0x002EC0A8
		// (set) Token: 0x06003554 RID: 13652 RVA: 0x002EDEF4 File Offset: 0x002EC0F4
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

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06003555 RID: 13653 RVA: 0x002EDF40 File Offset: 0x002EC140
		// (set) Token: 0x06003556 RID: 13654 RVA: 0x002EDF8C File Offset: 0x002EC18C
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

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06003557 RID: 13655 RVA: 0x002EDFD8 File Offset: 0x002EC1D8
		// (set) Token: 0x06003558 RID: 13656 RVA: 0x002EE024 File Offset: 0x002EC224
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

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06003559 RID: 13657 RVA: 0x002EE070 File Offset: 0x002EC270
		// (set) Token: 0x0600355A RID: 13658 RVA: 0x002EE0BC File Offset: 0x002EC2BC
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

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x0600355B RID: 13659 RVA: 0x002EE108 File Offset: 0x002EC308
		// (set) Token: 0x0600355C RID: 13660 RVA: 0x002EE154 File Offset: 0x002EC354
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

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x0600355D RID: 13661 RVA: 0x002EE1A0 File Offset: 0x002EC3A0
		// (set) Token: 0x0600355E RID: 13662 RVA: 0x002EE1EC File Offset: 0x002EC3EC
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

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x0600355F RID: 13663 RVA: 0x002EE238 File Offset: 0x002EC438
		// (set) Token: 0x06003560 RID: 13664 RVA: 0x002EE284 File Offset: 0x002EC484
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

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06003561 RID: 13665 RVA: 0x002EE2D0 File Offset: 0x002EC4D0
		// (set) Token: 0x06003562 RID: 13666 RVA: 0x002EE31C File Offset: 0x002EC51C
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

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06003563 RID: 13667 RVA: 0x002EE368 File Offset: 0x002EC568
		// (set) Token: 0x06003564 RID: 13668 RVA: 0x002EE3B4 File Offset: 0x002EC5B4
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

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06003565 RID: 13669 RVA: 0x002EE400 File Offset: 0x002EC600
		// (set) Token: 0x06003566 RID: 13670 RVA: 0x002EE44C File Offset: 0x002EC64C
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

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06003567 RID: 13671 RVA: 0x002EE498 File Offset: 0x002EC698
		// (set) Token: 0x06003568 RID: 13672 RVA: 0x002EE4E4 File Offset: 0x002EC6E4
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

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06003569 RID: 13673 RVA: 0x002EE530 File Offset: 0x002EC730
		// (set) Token: 0x0600356A RID: 13674 RVA: 0x002EE57C File Offset: 0x002EC77C
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

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x0600356B RID: 13675 RVA: 0x002EE5C8 File Offset: 0x002EC7C8
		// (set) Token: 0x0600356C RID: 13676 RVA: 0x002EE614 File Offset: 0x002EC814
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

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x0600356D RID: 13677 RVA: 0x002EE660 File Offset: 0x002EC860
		// (set) Token: 0x0600356E RID: 13678 RVA: 0x002EE6AC File Offset: 0x002EC8AC
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

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x0600356F RID: 13679 RVA: 0x002EE6F8 File Offset: 0x002EC8F8
		// (set) Token: 0x06003570 RID: 13680 RVA: 0x002EE744 File Offset: 0x002EC944
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

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06003571 RID: 13681 RVA: 0x002EE790 File Offset: 0x002EC990
		// (set) Token: 0x06003572 RID: 13682 RVA: 0x002EE7DC File Offset: 0x002EC9DC
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

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06003573 RID: 13683 RVA: 0x002EE828 File Offset: 0x002ECA28
		// (set) Token: 0x06003574 RID: 13684 RVA: 0x002EE874 File Offset: 0x002ECA74
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

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06003575 RID: 13685 RVA: 0x002EE8C0 File Offset: 0x002ECAC0
		// (set) Token: 0x06003576 RID: 13686 RVA: 0x002EE90C File Offset: 0x002ECB0C
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

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06003577 RID: 13687 RVA: 0x002EE958 File Offset: 0x002ECB58
		// (set) Token: 0x06003578 RID: 13688 RVA: 0x002EE9A4 File Offset: 0x002ECBA4
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

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06003579 RID: 13689 RVA: 0x002EE9F0 File Offset: 0x002ECBF0
		// (set) Token: 0x0600357A RID: 13690 RVA: 0x002EEA3C File Offset: 0x002ECC3C
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

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x0600357B RID: 13691 RVA: 0x002EEA88 File Offset: 0x002ECC88
		// (set) Token: 0x0600357C RID: 13692 RVA: 0x002EEAD4 File Offset: 0x002ECCD4
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

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x0600357D RID: 13693 RVA: 0x002EEB20 File Offset: 0x002ECD20
		// (set) Token: 0x0600357E RID: 13694 RVA: 0x002EEB6C File Offset: 0x002ECD6C
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

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x0600357F RID: 13695 RVA: 0x002EEBB8 File Offset: 0x002ECDB8
		// (set) Token: 0x06003580 RID: 13696 RVA: 0x002EEC04 File Offset: 0x002ECE04
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

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06003581 RID: 13697 RVA: 0x002EEC50 File Offset: 0x002ECE50
		// (set) Token: 0x06003582 RID: 13698 RVA: 0x002EEC9C File Offset: 0x002ECE9C
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

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06003583 RID: 13699 RVA: 0x002EECE8 File Offset: 0x002ECEE8
		// (set) Token: 0x06003584 RID: 13700 RVA: 0x002EED34 File Offset: 0x002ECF34
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

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06003585 RID: 13701 RVA: 0x002EED80 File Offset: 0x002ECF80
		// (set) Token: 0x06003586 RID: 13702 RVA: 0x002EEDCC File Offset: 0x002ECFCC
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

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06003587 RID: 13703 RVA: 0x002EEE18 File Offset: 0x002ED018
		// (set) Token: 0x06003588 RID: 13704 RVA: 0x002EEE60 File Offset: 0x002ED060
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

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06003589 RID: 13705 RVA: 0x002EEEA8 File Offset: 0x002ED0A8
		// (set) Token: 0x0600358A RID: 13706 RVA: 0x002EEEF0 File Offset: 0x002ED0F0
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

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x0600358B RID: 13707 RVA: 0x002EEF38 File Offset: 0x002ED138
		// (set) Token: 0x0600358C RID: 13708 RVA: 0x002EEF80 File Offset: 0x002ED180
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

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x0600358D RID: 13709 RVA: 0x002EEFC8 File Offset: 0x002ED1C8
		// (set) Token: 0x0600358E RID: 13710 RVA: 0x002EF010 File Offset: 0x002ED210
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

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x0600358F RID: 13711 RVA: 0x002EF058 File Offset: 0x002ED258
		// (set) Token: 0x06003590 RID: 13712 RVA: 0x002EF0A4 File Offset: 0x002ED2A4
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

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06003591 RID: 13713 RVA: 0x002EF0F0 File Offset: 0x002ED2F0
		// (set) Token: 0x06003592 RID: 13714 RVA: 0x002EF13C File Offset: 0x002ED33C
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

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06003593 RID: 13715 RVA: 0x002EF188 File Offset: 0x002ED388
		// (set) Token: 0x06003594 RID: 13716 RVA: 0x002EF1D4 File Offset: 0x002ED3D4
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

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06003595 RID: 13717 RVA: 0x002EF220 File Offset: 0x002ED420
		// (set) Token: 0x06003596 RID: 13718 RVA: 0x002EF26C File Offset: 0x002ED46C
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

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06003597 RID: 13719 RVA: 0x002EF2B8 File Offset: 0x002ED4B8
		// (set) Token: 0x06003598 RID: 13720 RVA: 0x002EF2D5 File Offset: 0x002ED4D5
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

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06003599 RID: 13721 RVA: 0x002EF2E4 File Offset: 0x002ED4E4
		// (set) Token: 0x0600359A RID: 13722 RVA: 0x002EF301 File Offset: 0x002ED501
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

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x0600359B RID: 13723 RVA: 0x002EF310 File Offset: 0x002ED510
		// (set) Token: 0x0600359C RID: 13724 RVA: 0x002EF32D File Offset: 0x002ED52D
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

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x0600359D RID: 13725 RVA: 0x002EF33C File Offset: 0x002ED53C
		// (set) Token: 0x0600359E RID: 13726 RVA: 0x002EF384 File Offset: 0x002ED584
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

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x0600359F RID: 13727 RVA: 0x002EF3CC File Offset: 0x002ED5CC
		// (set) Token: 0x060035A0 RID: 13728 RVA: 0x002EF418 File Offset: 0x002ED618
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

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x060035A1 RID: 13729 RVA: 0x002EF464 File Offset: 0x002ED664
		// (set) Token: 0x060035A2 RID: 13730 RVA: 0x002EF4AC File Offset: 0x002ED6AC
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

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x060035A3 RID: 13731 RVA: 0x002EF4F4 File Offset: 0x002ED6F4
		// (set) Token: 0x060035A4 RID: 13732 RVA: 0x002EF53C File Offset: 0x002ED73C
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

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x060035A5 RID: 13733 RVA: 0x002EF584 File Offset: 0x002ED784
		// (set) Token: 0x060035A6 RID: 13734 RVA: 0x002EF5CC File Offset: 0x002ED7CC
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

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x060035A7 RID: 13735 RVA: 0x002EF614 File Offset: 0x002ED814
		// (set) Token: 0x060035A8 RID: 13736 RVA: 0x002EF65C File Offset: 0x002ED85C
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

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x060035A9 RID: 13737 RVA: 0x002EF6A4 File Offset: 0x002ED8A4
		// (set) Token: 0x060035AA RID: 13738 RVA: 0x002EF6C3 File Offset: 0x002ED8C3
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

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x060035AB RID: 13739 RVA: 0x002EF6D4 File Offset: 0x002ED8D4
		public ChangeLifeProp RoleChangeLifeProp
		{
			get
			{
				return this._RoleChangeLifeProp;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x060035AC RID: 13740 RVA: 0x002EF6EC File Offset: 0x002ED8EC
		// (set) Token: 0x060035AD RID: 13741 RVA: 0x002EF738 File Offset: 0x002ED938
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

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x060035AE RID: 13742 RVA: 0x002EF784 File Offset: 0x002ED984
		// (set) Token: 0x060035AF RID: 13743 RVA: 0x002EF7CC File Offset: 0x002ED9CC
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

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x060035B0 RID: 13744 RVA: 0x002EF814 File Offset: 0x002EDA14
		// (set) Token: 0x060035B1 RID: 13745 RVA: 0x002EF85C File Offset: 0x002EDA5C
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

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x060035B2 RID: 13746 RVA: 0x002EF8A4 File Offset: 0x002EDAA4
		// (set) Token: 0x060035B3 RID: 13747 RVA: 0x002EF8EC File Offset: 0x002EDAEC
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

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x060035B4 RID: 13748 RVA: 0x002EF944 File Offset: 0x002EDB44
		// (set) Token: 0x060035B5 RID: 13749 RVA: 0x002EF98C File Offset: 0x002EDB8C
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

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x060035B6 RID: 13750 RVA: 0x002EF9D4 File Offset: 0x002EDBD4
		// (set) Token: 0x060035B7 RID: 13751 RVA: 0x002EFA1C File Offset: 0x002EDC1C
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

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x060035B8 RID: 13752 RVA: 0x002EFA64 File Offset: 0x002EDC64
		// (set) Token: 0x060035B9 RID: 13753 RVA: 0x002EFA80 File Offset: 0x002EDC80
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

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x060035BA RID: 13754 RVA: 0x002EFB54 File Offset: 0x002EDD54
		// (set) Token: 0x060035BB RID: 13755 RVA: 0x002EFB6E File Offset: 0x002EDD6E
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

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x060035BC RID: 13756 RVA: 0x002EFB78 File Offset: 0x002EDD78
		// (set) Token: 0x060035BD RID: 13757 RVA: 0x002EFBC0 File Offset: 0x002EDDC0
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

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x060035BE RID: 13758 RVA: 0x002EFC08 File Offset: 0x002EDE08
		// (set) Token: 0x060035BF RID: 13759 RVA: 0x002EFC50 File Offset: 0x002EDE50
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

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x060035C0 RID: 13760 RVA: 0x002EFC98 File Offset: 0x002EDE98
		// (set) Token: 0x060035C1 RID: 13761 RVA: 0x002EFCE0 File Offset: 0x002EDEE0
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

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x060035C2 RID: 13762 RVA: 0x002EFD28 File Offset: 0x002EDF28
		// (set) Token: 0x060035C3 RID: 13763 RVA: 0x002EFD70 File Offset: 0x002EDF70
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

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x060035C4 RID: 13764 RVA: 0x002EFDB8 File Offset: 0x002EDFB8
		// (set) Token: 0x060035C5 RID: 13765 RVA: 0x002EFE00 File Offset: 0x002EE000
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

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x060035C6 RID: 13766 RVA: 0x002EFE48 File Offset: 0x002EE048
		// (set) Token: 0x060035C7 RID: 13767 RVA: 0x002EFE90 File Offset: 0x002EE090
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

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x060035C8 RID: 13768 RVA: 0x002EFED8 File Offset: 0x002EE0D8
		// (set) Token: 0x060035C9 RID: 13769 RVA: 0x002EFF20 File Offset: 0x002EE120
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

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x060035CA RID: 13770 RVA: 0x002EFF68 File Offset: 0x002EE168
		// (set) Token: 0x060035CB RID: 13771 RVA: 0x002EFFB0 File Offset: 0x002EE1B0
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

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x060035CC RID: 13772 RVA: 0x002EFFF8 File Offset: 0x002EE1F8
		// (set) Token: 0x060035CD RID: 13773 RVA: 0x002F0040 File Offset: 0x002EE240
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

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x060035CE RID: 13774 RVA: 0x002F0088 File Offset: 0x002EE288
		// (set) Token: 0x060035CF RID: 13775 RVA: 0x002F00D0 File Offset: 0x002EE2D0
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

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x060035D0 RID: 13776 RVA: 0x002F0118 File Offset: 0x002EE318
		// (set) Token: 0x060035D1 RID: 13777 RVA: 0x002F0160 File Offset: 0x002EE360
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

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x060035D2 RID: 13778 RVA: 0x002F01A8 File Offset: 0x002EE3A8
		// (set) Token: 0x060035D3 RID: 13779 RVA: 0x002F01F0 File Offset: 0x002EE3F0
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

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x060035D4 RID: 13780 RVA: 0x002F0238 File Offset: 0x002EE438
		// (set) Token: 0x060035D5 RID: 13781 RVA: 0x002F0280 File Offset: 0x002EE480
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

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x060035D6 RID: 13782 RVA: 0x002F02C8 File Offset: 0x002EE4C8
		// (set) Token: 0x060035D7 RID: 13783 RVA: 0x002F0310 File Offset: 0x002EE510
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

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x060035D8 RID: 13784 RVA: 0x002F0358 File Offset: 0x002EE558
		// (set) Token: 0x060035D9 RID: 13785 RVA: 0x002F03A0 File Offset: 0x002EE5A0
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

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x060035DA RID: 13786 RVA: 0x002F03E8 File Offset: 0x002EE5E8
		// (set) Token: 0x060035DB RID: 13787 RVA: 0x002F0430 File Offset: 0x002EE630
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

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x060035DC RID: 13788 RVA: 0x002F0478 File Offset: 0x002EE678
		// (set) Token: 0x060035DD RID: 13789 RVA: 0x002F04C0 File Offset: 0x002EE6C0
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

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x060035DE RID: 13790 RVA: 0x002F0508 File Offset: 0x002EE708
		// (set) Token: 0x060035DF RID: 13791 RVA: 0x002F0550 File Offset: 0x002EE750
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

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x060035E0 RID: 13792 RVA: 0x002F0598 File Offset: 0x002EE798
		// (set) Token: 0x060035E1 RID: 13793 RVA: 0x002F05E0 File Offset: 0x002EE7E0
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

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x060035E2 RID: 13794 RVA: 0x002F0628 File Offset: 0x002EE828
		// (set) Token: 0x060035E3 RID: 13795 RVA: 0x002F0670 File Offset: 0x002EE870
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

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x060035E4 RID: 13796 RVA: 0x002F06B8 File Offset: 0x002EE8B8
		// (set) Token: 0x060035E5 RID: 13797 RVA: 0x002F0700 File Offset: 0x002EE900
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

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x060035E6 RID: 13798 RVA: 0x002F0748 File Offset: 0x002EE948
		// (set) Token: 0x060035E7 RID: 13799 RVA: 0x002F0790 File Offset: 0x002EE990
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

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x060035E8 RID: 13800 RVA: 0x002F07D8 File Offset: 0x002EE9D8
		// (set) Token: 0x060035E9 RID: 13801 RVA: 0x002F0820 File Offset: 0x002EEA20
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

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x060035EA RID: 13802 RVA: 0x002F0868 File Offset: 0x002EEA68
		// (set) Token: 0x060035EB RID: 13803 RVA: 0x002F08B4 File Offset: 0x002EEAB4
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

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x060035EC RID: 13804 RVA: 0x002F0900 File Offset: 0x002EEB00
		// (set) Token: 0x060035ED RID: 13805 RVA: 0x002F094C File Offset: 0x002EEB4C
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

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x060035EE RID: 13806 RVA: 0x002F0998 File Offset: 0x002EEB98
		// (set) Token: 0x060035EF RID: 13807 RVA: 0x002F09E4 File Offset: 0x002EEBE4
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

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x060035F0 RID: 13808 RVA: 0x002F0A30 File Offset: 0x002EEC30
		// (set) Token: 0x060035F1 RID: 13809 RVA: 0x002F0A7C File Offset: 0x002EEC7C
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

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x060035F2 RID: 13810 RVA: 0x002F0AC8 File Offset: 0x002EECC8
		// (set) Token: 0x060035F3 RID: 13811 RVA: 0x002F0B14 File Offset: 0x002EED14
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

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x060035F4 RID: 13812 RVA: 0x002F0B60 File Offset: 0x002EED60
		// (set) Token: 0x060035F5 RID: 13813 RVA: 0x002F0BAC File Offset: 0x002EEDAC
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

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x060035F6 RID: 13814 RVA: 0x002F0BF8 File Offset: 0x002EEDF8
		// (set) Token: 0x060035F7 RID: 13815 RVA: 0x002F0C44 File Offset: 0x002EEE44
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

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x060035F8 RID: 13816 RVA: 0x002F0C90 File Offset: 0x002EEE90
		// (set) Token: 0x060035F9 RID: 13817 RVA: 0x002F0CDC File Offset: 0x002EEEDC
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

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x060035FA RID: 13818 RVA: 0x002F0D28 File Offset: 0x002EEF28
		// (set) Token: 0x060035FB RID: 13819 RVA: 0x002F0D74 File Offset: 0x002EEF74
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

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x060035FC RID: 13820 RVA: 0x002F0DC0 File Offset: 0x002EEFC0
		// (set) Token: 0x060035FD RID: 13821 RVA: 0x002F0E0C File Offset: 0x002EF00C
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

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x060035FE RID: 13822 RVA: 0x002F0E58 File Offset: 0x002EF058
		// (set) Token: 0x060035FF RID: 13823 RVA: 0x002F0EA4 File Offset: 0x002EF0A4
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

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06003600 RID: 13824 RVA: 0x002F0EF0 File Offset: 0x002EF0F0
		// (set) Token: 0x06003601 RID: 13825 RVA: 0x002F0F3C File Offset: 0x002EF13C
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

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06003602 RID: 13826 RVA: 0x002F0F88 File Offset: 0x002EF188
		// (set) Token: 0x06003603 RID: 13827 RVA: 0x002F0FD4 File Offset: 0x002EF1D4
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

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06003604 RID: 13828 RVA: 0x002F1020 File Offset: 0x002EF220
		// (set) Token: 0x06003605 RID: 13829 RVA: 0x002F1068 File Offset: 0x002EF268
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

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06003606 RID: 13830 RVA: 0x002F10B0 File Offset: 0x002EF2B0
		// (set) Token: 0x06003607 RID: 13831 RVA: 0x002F10F8 File Offset: 0x002EF2F8
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

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06003608 RID: 13832 RVA: 0x002F1140 File Offset: 0x002EF340
		// (set) Token: 0x06003609 RID: 13833 RVA: 0x002F1188 File Offset: 0x002EF388
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

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x0600360A RID: 13834 RVA: 0x002F11D0 File Offset: 0x002EF3D0
		// (set) Token: 0x0600360B RID: 13835 RVA: 0x002F1218 File Offset: 0x002EF418
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

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x0600360C RID: 13836 RVA: 0x002F1260 File Offset: 0x002EF460
		// (set) Token: 0x0600360D RID: 13837 RVA: 0x002F12A8 File Offset: 0x002EF4A8
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

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x0600360E RID: 13838 RVA: 0x002F12F0 File Offset: 0x002EF4F0
		public EquipPropItem EquipProp
		{
			get
			{
				return this._EquipProp;
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x0600360F RID: 13839 RVA: 0x002F1308 File Offset: 0x002EF508
		public WeighItems WeighItems
		{
			get
			{
				return this._WeighItems;
			}
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x002F1320 File Offset: 0x002EF520
		public RoleDataEx GetRoleDataEx()
		{
			return this._RoleDataEx;
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06003611 RID: 13841 RVA: 0x002F1338 File Offset: 0x002EF538
		// (set) Token: 0x06003612 RID: 13842 RVA: 0x002F1380 File Offset: 0x002EF580
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

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06003613 RID: 13843 RVA: 0x002F13C8 File Offset: 0x002EF5C8
		// (set) Token: 0x06003614 RID: 13844 RVA: 0x002F1410 File Offset: 0x002EF610
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

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06003615 RID: 13845 RVA: 0x002F1458 File Offset: 0x002EF658
		// (set) Token: 0x06003616 RID: 13846 RVA: 0x002F14A0 File Offset: 0x002EF6A0
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

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06003617 RID: 13847 RVA: 0x002F14E8 File Offset: 0x002EF6E8
		// (set) Token: 0x06003618 RID: 13848 RVA: 0x002F1530 File Offset: 0x002EF730
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

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06003619 RID: 13849 RVA: 0x002F1578 File Offset: 0x002EF778
		// (set) Token: 0x0600361A RID: 13850 RVA: 0x002F15C0 File Offset: 0x002EF7C0
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

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x0600361B RID: 13851 RVA: 0x002F1608 File Offset: 0x002EF808
		// (set) Token: 0x0600361C RID: 13852 RVA: 0x002F1650 File Offset: 0x002EF850
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

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x0600361D RID: 13853 RVA: 0x002F1698 File Offset: 0x002EF898
		// (set) Token: 0x0600361E RID: 13854 RVA: 0x002F16E0 File Offset: 0x002EF8E0
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

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x0600361F RID: 13855 RVA: 0x002F1728 File Offset: 0x002EF928
		// (set) Token: 0x06003620 RID: 13856 RVA: 0x002F1770 File Offset: 0x002EF970
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

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06003621 RID: 13857 RVA: 0x002F17B8 File Offset: 0x002EF9B8
		// (set) Token: 0x06003622 RID: 13858 RVA: 0x002F1800 File Offset: 0x002EFA00
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

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06003623 RID: 13859 RVA: 0x002F1848 File Offset: 0x002EFA48
		// (set) Token: 0x06003624 RID: 13860 RVA: 0x002F1890 File Offset: 0x002EFA90
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

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06003625 RID: 13861 RVA: 0x002F18D8 File Offset: 0x002EFAD8
		// (set) Token: 0x06003626 RID: 13862 RVA: 0x002F1920 File Offset: 0x002EFB20
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

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06003627 RID: 13863 RVA: 0x002F1968 File Offset: 0x002EFB68
		// (set) Token: 0x06003628 RID: 13864 RVA: 0x002F19B0 File Offset: 0x002EFBB0
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

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06003629 RID: 13865 RVA: 0x002F19F8 File Offset: 0x002EFBF8
		// (set) Token: 0x0600362A RID: 13866 RVA: 0x002F1A40 File Offset: 0x002EFC40
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

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x0600362B RID: 13867 RVA: 0x002F1A88 File Offset: 0x002EFC88
		// (set) Token: 0x0600362C RID: 13868 RVA: 0x002F1AD0 File Offset: 0x002EFCD0
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

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x0600362D RID: 13869 RVA: 0x002F1B18 File Offset: 0x002EFD18
		// (set) Token: 0x0600362E RID: 13870 RVA: 0x002F1B60 File Offset: 0x002EFD60
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

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x0600362F RID: 13871 RVA: 0x002F1BA8 File Offset: 0x002EFDA8
		// (set) Token: 0x06003630 RID: 13872 RVA: 0x002F1BF0 File Offset: 0x002EFDF0
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

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06003631 RID: 13873 RVA: 0x002F1C38 File Offset: 0x002EFE38
		// (set) Token: 0x06003632 RID: 13874 RVA: 0x002F1C80 File Offset: 0x002EFE80
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

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06003633 RID: 13875 RVA: 0x002F1CC8 File Offset: 0x002EFEC8
		// (set) Token: 0x06003634 RID: 13876 RVA: 0x002F1D10 File Offset: 0x002EFF10
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06003635 RID: 13877 RVA: 0x002F1D58 File Offset: 0x002EFF58
		// (set) Token: 0x06003636 RID: 13878 RVA: 0x002F1DA0 File Offset: 0x002EFFA0
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

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06003637 RID: 13879 RVA: 0x002F1DE8 File Offset: 0x002EFFE8
		// (set) Token: 0x06003638 RID: 13880 RVA: 0x002F1E30 File Offset: 0x002F0030
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

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06003639 RID: 13881 RVA: 0x002F1E78 File Offset: 0x002F0078
		// (set) Token: 0x0600363A RID: 13882 RVA: 0x002F1EC0 File Offset: 0x002F00C0
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

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x0600363B RID: 13883 RVA: 0x002F1F08 File Offset: 0x002F0108
		// (set) Token: 0x0600363C RID: 13884 RVA: 0x002F1F50 File Offset: 0x002F0150
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

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x0600363D RID: 13885 RVA: 0x002F1F98 File Offset: 0x002F0198
		// (set) Token: 0x0600363E RID: 13886 RVA: 0x002F1FE0 File Offset: 0x002F01E0
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

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x0600363F RID: 13887 RVA: 0x002F2028 File Offset: 0x002F0228
		// (set) Token: 0x06003640 RID: 13888 RVA: 0x002F2070 File Offset: 0x002F0270
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

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06003641 RID: 13889 RVA: 0x002F20B8 File Offset: 0x002F02B8
		// (set) Token: 0x06003642 RID: 13890 RVA: 0x002F2100 File Offset: 0x002F0300
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

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06003643 RID: 13891 RVA: 0x002F2148 File Offset: 0x002F0348
		// (set) Token: 0x06003644 RID: 13892 RVA: 0x002F2190 File Offset: 0x002F0390
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

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06003645 RID: 13893 RVA: 0x002F21D8 File Offset: 0x002F03D8
		// (set) Token: 0x06003646 RID: 13894 RVA: 0x002F2220 File Offset: 0x002F0420
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

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06003647 RID: 13895 RVA: 0x002F2268 File Offset: 0x002F0468
		// (set) Token: 0x06003648 RID: 13896 RVA: 0x002F22B0 File Offset: 0x002F04B0
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

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06003649 RID: 13897 RVA: 0x002F22F8 File Offset: 0x002F04F8
		// (set) Token: 0x0600364A RID: 13898 RVA: 0x002F2340 File Offset: 0x002F0540
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

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x0600364B RID: 13899 RVA: 0x002F2388 File Offset: 0x002F0588
		// (set) Token: 0x0600364C RID: 13900 RVA: 0x002F23D0 File Offset: 0x002F05D0
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

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600364D RID: 13901 RVA: 0x002F2418 File Offset: 0x002F0618
		// (set) Token: 0x0600364E RID: 13902 RVA: 0x002F2460 File Offset: 0x002F0660
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

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600364F RID: 13903 RVA: 0x002F24A8 File Offset: 0x002F06A8
		// (set) Token: 0x06003650 RID: 13904 RVA: 0x002F24F0 File Offset: 0x002F06F0
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

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06003651 RID: 13905 RVA: 0x002F2538 File Offset: 0x002F0738
		// (set) Token: 0x06003652 RID: 13906 RVA: 0x002F2580 File Offset: 0x002F0780
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

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06003653 RID: 13907 RVA: 0x002F25C8 File Offset: 0x002F07C8
		// (set) Token: 0x06003654 RID: 13908 RVA: 0x002F2610 File Offset: 0x002F0810
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

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06003655 RID: 13909 RVA: 0x002F2658 File Offset: 0x002F0858
		// (set) Token: 0x06003656 RID: 13910 RVA: 0x002F26A0 File Offset: 0x002F08A0
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

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06003657 RID: 13911 RVA: 0x002F26E8 File Offset: 0x002F08E8
		// (set) Token: 0x06003658 RID: 13912 RVA: 0x002F2730 File Offset: 0x002F0930
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

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06003659 RID: 13913 RVA: 0x002F2778 File Offset: 0x002F0978
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

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x0600365A RID: 13914 RVA: 0x002F27C0 File Offset: 0x002F09C0
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

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x0600365B RID: 13915 RVA: 0x002F2808 File Offset: 0x002F0A08
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

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x0600365C RID: 13916 RVA: 0x002F2850 File Offset: 0x002F0A50
		// (set) Token: 0x0600365D RID: 13917 RVA: 0x002F2898 File Offset: 0x002F0A98
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

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x0600365E RID: 13918 RVA: 0x002F28E0 File Offset: 0x002F0AE0
		// (set) Token: 0x0600365F RID: 13919 RVA: 0x002F2928 File Offset: 0x002F0B28
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

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06003660 RID: 13920 RVA: 0x002F2970 File Offset: 0x002F0B70
		// (set) Token: 0x06003661 RID: 13921 RVA: 0x002F29B8 File Offset: 0x002F0BB8
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

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06003662 RID: 13922 RVA: 0x002F2A00 File Offset: 0x002F0C00
		// (set) Token: 0x06003663 RID: 13923 RVA: 0x002F2A48 File Offset: 0x002F0C48
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

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06003664 RID: 13924 RVA: 0x002F2A90 File Offset: 0x002F0C90
		// (set) Token: 0x06003665 RID: 13925 RVA: 0x002F2AD8 File Offset: 0x002F0CD8
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

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06003666 RID: 13926 RVA: 0x002F2B20 File Offset: 0x002F0D20
		// (set) Token: 0x06003667 RID: 13927 RVA: 0x002F2B68 File Offset: 0x002F0D68
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

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06003668 RID: 13928 RVA: 0x002F2BB0 File Offset: 0x002F0DB0
		// (set) Token: 0x06003669 RID: 13929 RVA: 0x002F2BF8 File Offset: 0x002F0DF8
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

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x0600366A RID: 13930 RVA: 0x002F2C40 File Offset: 0x002F0E40
		// (set) Token: 0x0600366B RID: 13931 RVA: 0x002F2C88 File Offset: 0x002F0E88
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

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x0600366C RID: 13932 RVA: 0x002F2CD0 File Offset: 0x002F0ED0
		// (set) Token: 0x0600366D RID: 13933 RVA: 0x002F2D18 File Offset: 0x002F0F18
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

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x0600366E RID: 13934 RVA: 0x002F2D60 File Offset: 0x002F0F60
		// (set) Token: 0x0600366F RID: 13935 RVA: 0x002F2DA8 File Offset: 0x002F0FA8
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

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06003670 RID: 13936 RVA: 0x002F2DF0 File Offset: 0x002F0FF0
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

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06003671 RID: 13937 RVA: 0x002F2E38 File Offset: 0x002F1038
		// (set) Token: 0x06003672 RID: 13938 RVA: 0x002F2E80 File Offset: 0x002F1080
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

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06003673 RID: 13939 RVA: 0x002F2EC8 File Offset: 0x002F10C8
		// (set) Token: 0x06003674 RID: 13940 RVA: 0x002F2F10 File Offset: 0x002F1110
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

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06003675 RID: 13941 RVA: 0x002F2F58 File Offset: 0x002F1158
		// (set) Token: 0x06003676 RID: 13942 RVA: 0x002F2FA0 File Offset: 0x002F11A0
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

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06003677 RID: 13943 RVA: 0x002F2FE8 File Offset: 0x002F11E8
		// (set) Token: 0x06003678 RID: 13944 RVA: 0x002F3030 File Offset: 0x002F1230
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

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06003679 RID: 13945 RVA: 0x002F3078 File Offset: 0x002F1278
		// (set) Token: 0x0600367A RID: 13946 RVA: 0x002F30C0 File Offset: 0x002F12C0
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

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x0600367B RID: 13947 RVA: 0x002F3108 File Offset: 0x002F1308
		// (set) Token: 0x0600367C RID: 13948 RVA: 0x002F3150 File Offset: 0x002F1350
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

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x0600367D RID: 13949 RVA: 0x002F3198 File Offset: 0x002F1398
		// (set) Token: 0x0600367E RID: 13950 RVA: 0x002F31E0 File Offset: 0x002F13E0
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

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x0600367F RID: 13951 RVA: 0x002F3228 File Offset: 0x002F1428
		// (set) Token: 0x06003680 RID: 13952 RVA: 0x002F3270 File Offset: 0x002F1470
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

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06003681 RID: 13953 RVA: 0x002F32B8 File Offset: 0x002F14B8
		// (set) Token: 0x06003682 RID: 13954 RVA: 0x002F3300 File Offset: 0x002F1500
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

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06003683 RID: 13955 RVA: 0x002F3348 File Offset: 0x002F1548
		// (set) Token: 0x06003684 RID: 13956 RVA: 0x002F3390 File Offset: 0x002F1590
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

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06003685 RID: 13957 RVA: 0x002F33D8 File Offset: 0x002F15D8
		// (set) Token: 0x06003686 RID: 13958 RVA: 0x002F3420 File Offset: 0x002F1620
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

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06003687 RID: 13959 RVA: 0x002F3468 File Offset: 0x002F1668
		// (set) Token: 0x06003688 RID: 13960 RVA: 0x002F34B0 File Offset: 0x002F16B0
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

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06003689 RID: 13961 RVA: 0x002F34F8 File Offset: 0x002F16F8
		// (set) Token: 0x0600368A RID: 13962 RVA: 0x002F3540 File Offset: 0x002F1740
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

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600368B RID: 13963 RVA: 0x002F3588 File Offset: 0x002F1788
		// (set) Token: 0x0600368C RID: 13964 RVA: 0x002F35D0 File Offset: 0x002F17D0
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

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x0600368D RID: 13965 RVA: 0x002F3618 File Offset: 0x002F1818
		// (set) Token: 0x0600368E RID: 13966 RVA: 0x002F3660 File Offset: 0x002F1860
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

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x0600368F RID: 13967 RVA: 0x002F36A8 File Offset: 0x002F18A8
		// (set) Token: 0x06003690 RID: 13968 RVA: 0x002F36F0 File Offset: 0x002F18F0
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

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06003691 RID: 13969 RVA: 0x002F3738 File Offset: 0x002F1938
		// (set) Token: 0x06003692 RID: 13970 RVA: 0x002F3780 File Offset: 0x002F1980
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

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06003693 RID: 13971 RVA: 0x002F37C8 File Offset: 0x002F19C8
		// (set) Token: 0x06003694 RID: 13972 RVA: 0x002F3810 File Offset: 0x002F1A10
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

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06003695 RID: 13973 RVA: 0x002F3858 File Offset: 0x002F1A58
		// (set) Token: 0x06003696 RID: 13974 RVA: 0x002F38A0 File Offset: 0x002F1AA0
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

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06003697 RID: 13975 RVA: 0x002F38E8 File Offset: 0x002F1AE8
		// (set) Token: 0x06003698 RID: 13976 RVA: 0x002F3930 File Offset: 0x002F1B30
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

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06003699 RID: 13977 RVA: 0x002F3978 File Offset: 0x002F1B78
		// (set) Token: 0x0600369A RID: 13978 RVA: 0x002F39C0 File Offset: 0x002F1BC0
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

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x0600369B RID: 13979 RVA: 0x002F3A08 File Offset: 0x002F1C08
		// (set) Token: 0x0600369C RID: 13980 RVA: 0x002F3A50 File Offset: 0x002F1C50
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

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x0600369D RID: 13981 RVA: 0x002F3A98 File Offset: 0x002F1C98
		// (set) Token: 0x0600369E RID: 13982 RVA: 0x002F3AE0 File Offset: 0x002F1CE0
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

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x0600369F RID: 13983 RVA: 0x002F3B28 File Offset: 0x002F1D28
		// (set) Token: 0x060036A0 RID: 13984 RVA: 0x002F3B70 File Offset: 0x002F1D70
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

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x060036A1 RID: 13985 RVA: 0x002F3BB8 File Offset: 0x002F1DB8
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

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x060036A2 RID: 13986 RVA: 0x002F3C00 File Offset: 0x002F1E00
		// (set) Token: 0x060036A3 RID: 13987 RVA: 0x002F3C48 File Offset: 0x002F1E48
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

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x060036A4 RID: 13988 RVA: 0x002F3C90 File Offset: 0x002F1E90
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

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x060036A5 RID: 13989 RVA: 0x002F3CD8 File Offset: 0x002F1ED8
		// (set) Token: 0x060036A6 RID: 13990 RVA: 0x002F3D20 File Offset: 0x002F1F20
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

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060036A7 RID: 13991 RVA: 0x002F3D68 File Offset: 0x002F1F68
		// (set) Token: 0x060036A8 RID: 13992 RVA: 0x002F3DB0 File Offset: 0x002F1FB0
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

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x060036A9 RID: 13993 RVA: 0x002F3DF8 File Offset: 0x002F1FF8
		// (set) Token: 0x060036AA RID: 13994 RVA: 0x002F3E40 File Offset: 0x002F2040
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

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x060036AB RID: 13995 RVA: 0x002F3E88 File Offset: 0x002F2088
		// (set) Token: 0x060036AC RID: 13996 RVA: 0x002F3ED0 File Offset: 0x002F20D0
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

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x060036AD RID: 13997 RVA: 0x002F3F18 File Offset: 0x002F2118
		// (set) Token: 0x060036AE RID: 13998 RVA: 0x002F3F60 File Offset: 0x002F2160
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

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x060036AF RID: 13999 RVA: 0x002F3FA8 File Offset: 0x002F21A8
		// (set) Token: 0x060036B0 RID: 14000 RVA: 0x002F3FF0 File Offset: 0x002F21F0
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x060036B1 RID: 14001 RVA: 0x002F4038 File Offset: 0x002F2238
		// (set) Token: 0x060036B2 RID: 14002 RVA: 0x002F4080 File Offset: 0x002F2280
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

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x060036B3 RID: 14003 RVA: 0x002F40C8 File Offset: 0x002F22C8
		// (set) Token: 0x060036B4 RID: 14004 RVA: 0x002F4110 File Offset: 0x002F2310
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

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x060036B5 RID: 14005 RVA: 0x002F4158 File Offset: 0x002F2358
		// (set) Token: 0x060036B6 RID: 14006 RVA: 0x002F41A0 File Offset: 0x002F23A0
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

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x060036B7 RID: 14007 RVA: 0x002F41E8 File Offset: 0x002F23E8
		// (set) Token: 0x060036B8 RID: 14008 RVA: 0x002F4230 File Offset: 0x002F2430
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

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x060036B9 RID: 14009 RVA: 0x002F4278 File Offset: 0x002F2478
		// (set) Token: 0x060036BA RID: 14010 RVA: 0x002F42C0 File Offset: 0x002F24C0
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

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x060036BB RID: 14011 RVA: 0x002F4308 File Offset: 0x002F2508
		// (set) Token: 0x060036BC RID: 14012 RVA: 0x002F4350 File Offset: 0x002F2550
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

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x060036BD RID: 14013 RVA: 0x002F4398 File Offset: 0x002F2598
		// (set) Token: 0x060036BE RID: 14014 RVA: 0x002F43E0 File Offset: 0x002F25E0
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

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x060036BF RID: 14015 RVA: 0x002F4428 File Offset: 0x002F2628
		// (set) Token: 0x060036C0 RID: 14016 RVA: 0x002F4470 File Offset: 0x002F2670
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

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x060036C1 RID: 14017 RVA: 0x002F44B8 File Offset: 0x002F26B8
		// (set) Token: 0x060036C2 RID: 14018 RVA: 0x002F4500 File Offset: 0x002F2700
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

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x060036C3 RID: 14019 RVA: 0x002F4548 File Offset: 0x002F2748
		// (set) Token: 0x060036C4 RID: 14020 RVA: 0x002F4590 File Offset: 0x002F2790
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

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x060036C5 RID: 14021 RVA: 0x002F45D8 File Offset: 0x002F27D8
		// (set) Token: 0x060036C6 RID: 14022 RVA: 0x002F4620 File Offset: 0x002F2820
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

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x060036C7 RID: 14023 RVA: 0x002F4668 File Offset: 0x002F2868
		// (set) Token: 0x060036C8 RID: 14024 RVA: 0x002F46B0 File Offset: 0x002F28B0
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

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x060036C9 RID: 14025 RVA: 0x002F46F8 File Offset: 0x002F28F8
		// (set) Token: 0x060036CA RID: 14026 RVA: 0x002F4740 File Offset: 0x002F2940
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

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x060036CB RID: 14027 RVA: 0x002F4788 File Offset: 0x002F2988
		// (set) Token: 0x060036CC RID: 14028 RVA: 0x002F47D0 File Offset: 0x002F29D0
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

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x060036CD RID: 14029 RVA: 0x002F4818 File Offset: 0x002F2A18
		// (set) Token: 0x060036CE RID: 14030 RVA: 0x002F4860 File Offset: 0x002F2A60
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

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x060036CF RID: 14031 RVA: 0x002F48A8 File Offset: 0x002F2AA8
		// (set) Token: 0x060036D0 RID: 14032 RVA: 0x002F48F0 File Offset: 0x002F2AF0
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

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x060036D1 RID: 14033 RVA: 0x002F4938 File Offset: 0x002F2B38
		// (set) Token: 0x060036D2 RID: 14034 RVA: 0x002F498C File Offset: 0x002F2B8C
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

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x060036D3 RID: 14035 RVA: 0x002F49E0 File Offset: 0x002F2BE0
		// (set) Token: 0x060036D4 RID: 14036 RVA: 0x002F4A34 File Offset: 0x002F2C34
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

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x060036D5 RID: 14037 RVA: 0x002F4A88 File Offset: 0x002F2C88
		// (set) Token: 0x060036D6 RID: 14038 RVA: 0x002F4ADC File Offset: 0x002F2CDC
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

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x060036D7 RID: 14039 RVA: 0x002F4B30 File Offset: 0x002F2D30
		// (set) Token: 0x060036D8 RID: 14040 RVA: 0x002F4B84 File Offset: 0x002F2D84
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

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060036D9 RID: 14041 RVA: 0x002F4BD8 File Offset: 0x002F2DD8
		// (set) Token: 0x060036DA RID: 14042 RVA: 0x002F4C2C File Offset: 0x002F2E2C
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

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x060036DB RID: 14043 RVA: 0x002F4C80 File Offset: 0x002F2E80
		// (set) Token: 0x060036DC RID: 14044 RVA: 0x002F4CD4 File Offset: 0x002F2ED4
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

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x060036DD RID: 14045 RVA: 0x002F4D28 File Offset: 0x002F2F28
		// (set) Token: 0x060036DE RID: 14046 RVA: 0x002F4D7C File Offset: 0x002F2F7C
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

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x060036DF RID: 14047 RVA: 0x002F4DD0 File Offset: 0x002F2FD0
		// (set) Token: 0x060036E0 RID: 14048 RVA: 0x002F4E24 File Offset: 0x002F3024
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

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x060036E1 RID: 14049 RVA: 0x002F4E78 File Offset: 0x002F3078
		// (set) Token: 0x060036E2 RID: 14050 RVA: 0x002F4EC8 File Offset: 0x002F30C8
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

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x060036E3 RID: 14051 RVA: 0x002F4F18 File Offset: 0x002F3118
		// (set) Token: 0x060036E4 RID: 14052 RVA: 0x002F4F68 File Offset: 0x002F3168
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

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x060036E5 RID: 14053 RVA: 0x002F4FB8 File Offset: 0x002F31B8
		// (set) Token: 0x060036E6 RID: 14054 RVA: 0x002F5008 File Offset: 0x002F3208
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

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x060036E7 RID: 14055 RVA: 0x002F5058 File Offset: 0x002F3258
		// (set) Token: 0x060036E8 RID: 14056 RVA: 0x002F50A8 File Offset: 0x002F32A8
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

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x060036E9 RID: 14057 RVA: 0x002F50F8 File Offset: 0x002F32F8
		// (set) Token: 0x060036EA RID: 14058 RVA: 0x002F5140 File Offset: 0x002F3340
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

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x060036EB RID: 14059 RVA: 0x002F5188 File Offset: 0x002F3388
		// (set) Token: 0x060036EC RID: 14060 RVA: 0x002F51D4 File Offset: 0x002F33D4
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

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x060036ED RID: 14061 RVA: 0x002F5220 File Offset: 0x002F3420
		// (set) Token: 0x060036EE RID: 14062 RVA: 0x002F526C File Offset: 0x002F346C
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

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x060036EF RID: 14063 RVA: 0x002F52B8 File Offset: 0x002F34B8
		// (set) Token: 0x060036F0 RID: 14064 RVA: 0x002F5300 File Offset: 0x002F3500
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

		// Token: 0x060036F1 RID: 14065 RVA: 0x002F5348 File Offset: 0x002F3548
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

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x060036F2 RID: 14066 RVA: 0x002F5384 File Offset: 0x002F3584
		// (set) Token: 0x060036F3 RID: 14067 RVA: 0x002F53CC File Offset: 0x002F35CC
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

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x060036F4 RID: 14068 RVA: 0x002F5414 File Offset: 0x002F3614
		// (set) Token: 0x060036F5 RID: 14069 RVA: 0x002F545C File Offset: 0x002F365C
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

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x060036F6 RID: 14070 RVA: 0x002F54A4 File Offset: 0x002F36A4
		// (set) Token: 0x060036F7 RID: 14071 RVA: 0x002F54EC File Offset: 0x002F36EC
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

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x060036F8 RID: 14072 RVA: 0x002F5534 File Offset: 0x002F3734
		// (set) Token: 0x060036F9 RID: 14073 RVA: 0x002F557C File Offset: 0x002F377C
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

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x060036FA RID: 14074 RVA: 0x002F55C4 File Offset: 0x002F37C4
		// (set) Token: 0x060036FB RID: 14075 RVA: 0x002F560C File Offset: 0x002F380C
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

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x060036FC RID: 14076 RVA: 0x002F5654 File Offset: 0x002F3854
		// (set) Token: 0x060036FD RID: 14077 RVA: 0x002F569C File Offset: 0x002F389C
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

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060036FE RID: 14078 RVA: 0x002F56E4 File Offset: 0x002F38E4
		// (remove) Token: 0x060036FF RID: 14079 RVA: 0x002F5720 File Offset: 0x002F3920
		public event ChangePosEventHandler ChangePosHandler;

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06003700 RID: 14080 RVA: 0x002F575C File Offset: 0x002F395C
		// (set) Token: 0x06003701 RID: 14081 RVA: 0x002F57C4 File Offset: 0x002F39C4
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

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06003702 RID: 14082 RVA: 0x002F5814 File Offset: 0x002F3A14
		// (set) Token: 0x06003703 RID: 14083 RVA: 0x002F587C File Offset: 0x002F3A7C
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

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06003704 RID: 14084 RVA: 0x002F58CC File Offset: 0x002F3ACC
		// (set) Token: 0x06003705 RID: 14085 RVA: 0x002F5914 File Offset: 0x002F3B14
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

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06003706 RID: 14086 RVA: 0x002F595C File Offset: 0x002F3B5C
		// (set) Token: 0x06003707 RID: 14087 RVA: 0x002F59A4 File Offset: 0x002F3BA4
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

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06003708 RID: 14088 RVA: 0x002F59EC File Offset: 0x002F3BEC
		// (set) Token: 0x06003709 RID: 14089 RVA: 0x002F5A34 File Offset: 0x002F3C34
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

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x0600370A RID: 14090 RVA: 0x002F5A7C File Offset: 0x002F3C7C
		// (set) Token: 0x0600370B RID: 14091 RVA: 0x002F5AC4 File Offset: 0x002F3CC4
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

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x0600370C RID: 14092 RVA: 0x002F5B0C File Offset: 0x002F3D0C
		// (set) Token: 0x0600370D RID: 14093 RVA: 0x002F5B54 File Offset: 0x002F3D54
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

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x0600370E RID: 14094 RVA: 0x002F5B9C File Offset: 0x002F3D9C
		// (set) Token: 0x0600370F RID: 14095 RVA: 0x002F5BE4 File Offset: 0x002F3DE4
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

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06003710 RID: 14096 RVA: 0x002F5C2C File Offset: 0x002F3E2C
		// (set) Token: 0x06003711 RID: 14097 RVA: 0x002F5C74 File Offset: 0x002F3E74
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

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06003712 RID: 14098 RVA: 0x002F5CBC File Offset: 0x002F3EBC
		// (set) Token: 0x06003713 RID: 14099 RVA: 0x002F5D04 File Offset: 0x002F3F04
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

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06003714 RID: 14100 RVA: 0x002F5D4C File Offset: 0x002F3F4C
		// (set) Token: 0x06003715 RID: 14101 RVA: 0x002F5D94 File Offset: 0x002F3F94
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

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06003716 RID: 14102 RVA: 0x002F5DDC File Offset: 0x002F3FDC
		// (set) Token: 0x06003717 RID: 14103 RVA: 0x002F5E24 File Offset: 0x002F4024
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

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06003718 RID: 14104 RVA: 0x002F5E6C File Offset: 0x002F406C
		// (set) Token: 0x06003719 RID: 14105 RVA: 0x002F5EB4 File Offset: 0x002F40B4
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

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x0600371A RID: 14106 RVA: 0x002F5EFC File Offset: 0x002F40FC
		// (set) Token: 0x0600371B RID: 14107 RVA: 0x002F5F44 File Offset: 0x002F4144
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

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x0600371C RID: 14108 RVA: 0x002F5F8C File Offset: 0x002F418C
		// (set) Token: 0x0600371D RID: 14109 RVA: 0x002F5FD4 File Offset: 0x002F41D4
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

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x0600371E RID: 14110 RVA: 0x002F601C File Offset: 0x002F421C
		// (set) Token: 0x0600371F RID: 14111 RVA: 0x002F6064 File Offset: 0x002F4264
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

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06003720 RID: 14112 RVA: 0x002F60AC File Offset: 0x002F42AC
		// (set) Token: 0x06003721 RID: 14113 RVA: 0x002F60F4 File Offset: 0x002F42F4
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

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06003722 RID: 14114 RVA: 0x002F613C File Offset: 0x002F433C
		// (set) Token: 0x06003723 RID: 14115 RVA: 0x002F6184 File Offset: 0x002F4384
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

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06003724 RID: 14116 RVA: 0x002F61CC File Offset: 0x002F43CC
		// (set) Token: 0x06003725 RID: 14117 RVA: 0x002F6214 File Offset: 0x002F4414
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

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06003726 RID: 14118 RVA: 0x002F625C File Offset: 0x002F445C
		// (set) Token: 0x06003727 RID: 14119 RVA: 0x002F62A4 File Offset: 0x002F44A4
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

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06003728 RID: 14120 RVA: 0x002F62EC File Offset: 0x002F44EC
		// (set) Token: 0x06003729 RID: 14121 RVA: 0x002F6334 File Offset: 0x002F4534
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

		// Token: 0x0600372A RID: 14122 RVA: 0x002F637C File Offset: 0x002F457C
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

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x0600372B RID: 14123 RVA: 0x002F63C8 File Offset: 0x002F45C8
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

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x0600372C RID: 14124 RVA: 0x002F6410 File Offset: 0x002F4610
		// (set) Token: 0x0600372D RID: 14125 RVA: 0x002F6458 File Offset: 0x002F4658
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

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x0600372E RID: 14126 RVA: 0x002F64A0 File Offset: 0x002F46A0
		// (set) Token: 0x0600372F RID: 14127 RVA: 0x002F64E8 File Offset: 0x002F46E8
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

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06003730 RID: 14128 RVA: 0x002F6530 File Offset: 0x002F4730
		// (set) Token: 0x06003731 RID: 14129 RVA: 0x002F6578 File Offset: 0x002F4778
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

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06003732 RID: 14130 RVA: 0x002F65C0 File Offset: 0x002F47C0
		// (set) Token: 0x06003733 RID: 14131 RVA: 0x002F6608 File Offset: 0x002F4808
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

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06003734 RID: 14132 RVA: 0x002F6650 File Offset: 0x002F4850
		// (set) Token: 0x06003735 RID: 14133 RVA: 0x002F6667 File Offset: 0x002F4867
		public int Last10sPosX { get; set; }

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06003736 RID: 14134 RVA: 0x002F6670 File Offset: 0x002F4870
		// (set) Token: 0x06003737 RID: 14135 RVA: 0x002F6687 File Offset: 0x002F4887
		public int Last10sPosY { get; set; }

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06003738 RID: 14136 RVA: 0x002F6690 File Offset: 0x002F4890
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

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06003739 RID: 14137 RVA: 0x002F66D8 File Offset: 0x002F48D8
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

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600373A RID: 14138 RVA: 0x002F6720 File Offset: 0x002F4920
		// (set) Token: 0x0600373B RID: 14139 RVA: 0x002F6738 File Offset: 0x002F4938
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

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x0600373C RID: 14140 RVA: 0x002F6744 File Offset: 0x002F4944
		// (set) Token: 0x0600373D RID: 14141 RVA: 0x002F675C File Offset: 0x002F495C
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

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x0600373E RID: 14142 RVA: 0x002F6768 File Offset: 0x002F4968
		// (set) Token: 0x0600373F RID: 14143 RVA: 0x002F6780 File Offset: 0x002F4980
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

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06003740 RID: 14144 RVA: 0x002F678C File Offset: 0x002F498C
		// (set) Token: 0x06003741 RID: 14145 RVA: 0x002F67A4 File Offset: 0x002F49A4
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

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06003742 RID: 14146 RVA: 0x002F67B0 File Offset: 0x002F49B0
		// (set) Token: 0x06003743 RID: 14147 RVA: 0x002F67C8 File Offset: 0x002F49C8
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

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06003744 RID: 14148 RVA: 0x002F67D4 File Offset: 0x002F49D4
		// (set) Token: 0x06003745 RID: 14149 RVA: 0x002F67EC File Offset: 0x002F49EC
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

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06003746 RID: 14150 RVA: 0x002F67F8 File Offset: 0x002F49F8
		// (set) Token: 0x06003747 RID: 14151 RVA: 0x002F6810 File Offset: 0x002F4A10
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

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06003748 RID: 14152 RVA: 0x002F681C File Offset: 0x002F4A1C
		// (set) Token: 0x06003749 RID: 14153 RVA: 0x002F6834 File Offset: 0x002F4A34
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

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x0600374A RID: 14154 RVA: 0x002F6840 File Offset: 0x002F4A40
		// (set) Token: 0x0600374B RID: 14155 RVA: 0x002F6858 File Offset: 0x002F4A58
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

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x0600374C RID: 14156 RVA: 0x002F6864 File Offset: 0x002F4A64
		// (set) Token: 0x0600374D RID: 14157 RVA: 0x002F687C File Offset: 0x002F4A7C
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

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x0600374E RID: 14158 RVA: 0x002F6888 File Offset: 0x002F4A88
		// (set) Token: 0x0600374F RID: 14159 RVA: 0x002F68A0 File Offset: 0x002F4AA0
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

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06003750 RID: 14160 RVA: 0x002F68AC File Offset: 0x002F4AAC
		// (set) Token: 0x06003751 RID: 14161 RVA: 0x002F68C4 File Offset: 0x002F4AC4
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

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06003752 RID: 14162 RVA: 0x002F68D0 File Offset: 0x002F4AD0
		// (set) Token: 0x06003753 RID: 14163 RVA: 0x002F68E8 File Offset: 0x002F4AE8
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

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06003754 RID: 14164 RVA: 0x002F68F4 File Offset: 0x002F4AF4
		// (set) Token: 0x06003755 RID: 14165 RVA: 0x002F690C File Offset: 0x002F4B0C
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

		// Token: 0x06003756 RID: 14166 RVA: 0x002F6918 File Offset: 0x002F4B18
		public void ResetExcellenceProp()
		{
			for (int i = 0; i < 32; i++)
			{
				this._ExcellenceProp[i] = 0.0;
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06003757 RID: 14167 RVA: 0x002F694C File Offset: 0x002F4B4C
		// (set) Token: 0x06003758 RID: 14168 RVA: 0x002F6964 File Offset: 0x002F4B64
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

		// Token: 0x06003759 RID: 14169 RVA: 0x002F696E File Offset: 0x002F4B6E
		public void ResetLuckyProp()
		{
			this._LuckProp = 0.0;
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x0600375A RID: 14170 RVA: 0x002F6980 File Offset: 0x002F4B80
		// (set) Token: 0x0600375B RID: 14171 RVA: 0x002F6998 File Offset: 0x002F4B98
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

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x0600375C RID: 14172 RVA: 0x002F69A4 File Offset: 0x002F4BA4
		// (set) Token: 0x0600375D RID: 14173 RVA: 0x002F69BC File Offset: 0x002F4BBC
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

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x0600375E RID: 14174 RVA: 0x002F69C8 File Offset: 0x002F4BC8
		// (set) Token: 0x0600375F RID: 14175 RVA: 0x002F69E0 File Offset: 0x002F4BE0
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

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06003760 RID: 14176 RVA: 0x002F69EC File Offset: 0x002F4BEC
		// (set) Token: 0x06003761 RID: 14177 RVA: 0x002F6A04 File Offset: 0x002F4C04
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

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06003762 RID: 14178 RVA: 0x002F6A10 File Offset: 0x002F4C10
		// (set) Token: 0x06003763 RID: 14179 RVA: 0x002F6A27 File Offset: 0x002F4C27
		public List<FallGoodsItem> EverydayOnlineAwardGiftList { get; set; }

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06003764 RID: 14180 RVA: 0x002F6A30 File Offset: 0x002F4C30
		// (set) Token: 0x06003765 RID: 14181 RVA: 0x002F6A47 File Offset: 0x002F4C47
		public List<FallGoodsItem> SeriesLoginAwardGiftList { get; set; }

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06003766 RID: 14182 RVA: 0x002F6A50 File Offset: 0x002F4C50
		// (set) Token: 0x06003767 RID: 14183 RVA: 0x002F6A98 File Offset: 0x002F4C98
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

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06003768 RID: 14184 RVA: 0x002F6AE0 File Offset: 0x002F4CE0
		// (set) Token: 0x06003769 RID: 14185 RVA: 0x002F6B28 File Offset: 0x002F4D28
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

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x0600376A RID: 14186 RVA: 0x002F6B70 File Offset: 0x002F4D70
		// (set) Token: 0x0600376B RID: 14187 RVA: 0x002F6BB8 File Offset: 0x002F4DB8
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

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x0600376C RID: 14188 RVA: 0x002F6C00 File Offset: 0x002F4E00
		// (set) Token: 0x0600376D RID: 14189 RVA: 0x002F6C48 File Offset: 0x002F4E48
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

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600376E RID: 14190 RVA: 0x002F6C90 File Offset: 0x002F4E90
		// (set) Token: 0x0600376F RID: 14191 RVA: 0x002F6CD8 File Offset: 0x002F4ED8
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

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06003770 RID: 14192 RVA: 0x002F6D20 File Offset: 0x002F4F20
		// (set) Token: 0x06003771 RID: 14193 RVA: 0x002F6D68 File Offset: 0x002F4F68
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

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06003772 RID: 14194 RVA: 0x002F6DB0 File Offset: 0x002F4FB0
		// (set) Token: 0x06003773 RID: 14195 RVA: 0x002F6DF8 File Offset: 0x002F4FF8
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

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06003774 RID: 14196 RVA: 0x002F6E40 File Offset: 0x002F5040
		// (set) Token: 0x06003775 RID: 14197 RVA: 0x002F6E88 File Offset: 0x002F5088
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

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06003776 RID: 14198 RVA: 0x002F6ED0 File Offset: 0x002F50D0
		// (set) Token: 0x06003777 RID: 14199 RVA: 0x002F6F18 File Offset: 0x002F5118
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

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06003778 RID: 14200 RVA: 0x002F6F60 File Offset: 0x002F5160
		// (set) Token: 0x06003779 RID: 14201 RVA: 0x002F6FA8 File Offset: 0x002F51A8
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

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x0600377A RID: 14202 RVA: 0x002F6FF0 File Offset: 0x002F51F0
		// (set) Token: 0x0600377B RID: 14203 RVA: 0x002F7008 File Offset: 0x002F5208
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

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x0600377C RID: 14204 RVA: 0x002F7014 File Offset: 0x002F5214
		// (set) Token: 0x0600377D RID: 14205 RVA: 0x002F702C File Offset: 0x002F522C
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

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x0600377E RID: 14206 RVA: 0x002F7038 File Offset: 0x002F5238
		// (set) Token: 0x0600377F RID: 14207 RVA: 0x002F7050 File Offset: 0x002F5250
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

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06003780 RID: 14208 RVA: 0x002F705C File Offset: 0x002F525C
		// (set) Token: 0x06003781 RID: 14209 RVA: 0x002F70A4 File Offset: 0x002F52A4
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

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06003782 RID: 14210 RVA: 0x002F70F8 File Offset: 0x002F52F8
		// (set) Token: 0x06003783 RID: 14211 RVA: 0x002F7140 File Offset: 0x002F5340
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

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06003784 RID: 14212 RVA: 0x002F7188 File Offset: 0x002F5388
		// (set) Token: 0x06003785 RID: 14213 RVA: 0x002F71D0 File Offset: 0x002F53D0
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

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06003786 RID: 14214 RVA: 0x002F7218 File Offset: 0x002F5418
		// (set) Token: 0x06003787 RID: 14215 RVA: 0x002F7260 File Offset: 0x002F5460
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

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06003788 RID: 14216 RVA: 0x002F72A8 File Offset: 0x002F54A8
		// (set) Token: 0x06003789 RID: 14217 RVA: 0x002F72F0 File Offset: 0x002F54F0
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

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x0600378A RID: 14218 RVA: 0x002F7338 File Offset: 0x002F5538
		// (set) Token: 0x0600378B RID: 14219 RVA: 0x002F7380 File Offset: 0x002F5580
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

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x0600378C RID: 14220 RVA: 0x002F73C8 File Offset: 0x002F55C8
		// (set) Token: 0x0600378D RID: 14221 RVA: 0x002F73E5 File Offset: 0x002F55E5
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

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x0600378E RID: 14222 RVA: 0x002F73F4 File Offset: 0x002F55F4
		// (set) Token: 0x0600378F RID: 14223 RVA: 0x002F740C File Offset: 0x002F560C
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

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06003790 RID: 14224 RVA: 0x002F7418 File Offset: 0x002F5618
		// (set) Token: 0x06003791 RID: 14225 RVA: 0x002F7430 File Offset: 0x002F5630
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

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06003792 RID: 14226 RVA: 0x002F743C File Offset: 0x002F563C
		// (set) Token: 0x06003793 RID: 14227 RVA: 0x002F7454 File Offset: 0x002F5654
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

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06003794 RID: 14228 RVA: 0x002F7460 File Offset: 0x002F5660
		// (set) Token: 0x06003795 RID: 14229 RVA: 0x002F74A8 File Offset: 0x002F56A8
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

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06003796 RID: 14230 RVA: 0x002F74F8 File Offset: 0x002F56F8
		// (set) Token: 0x06003797 RID: 14231 RVA: 0x002F7540 File Offset: 0x002F5740
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

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003798 RID: 14232 RVA: 0x002F7588 File Offset: 0x002F5788
		// (set) Token: 0x06003799 RID: 14233 RVA: 0x002F75D0 File Offset: 0x002F57D0
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

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x0600379A RID: 14234 RVA: 0x002F7618 File Offset: 0x002F5818
		// (set) Token: 0x0600379B RID: 14235 RVA: 0x002F7660 File Offset: 0x002F5860
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

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x0600379C RID: 14236 RVA: 0x002F76A8 File Offset: 0x002F58A8
		public object LockUnionPalace
		{
			get
			{
				return this._LockUnionPalace;
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x0600379D RID: 14237 RVA: 0x002F76C0 File Offset: 0x002F58C0
		// (set) Token: 0x0600379E RID: 14238 RVA: 0x002F770C File Offset: 0x002F590C
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

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x0600379F RID: 14239 RVA: 0x002F7758 File Offset: 0x002F5958
		// (set) Token: 0x060037A0 RID: 14240 RVA: 0x002F77A0 File Offset: 0x002F59A0
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

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x060037A1 RID: 14241 RVA: 0x002F77E8 File Offset: 0x002F59E8
		// (set) Token: 0x060037A2 RID: 14242 RVA: 0x002F7830 File Offset: 0x002F5A30
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

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x060037A3 RID: 14243 RVA: 0x002F7878 File Offset: 0x002F5A78
		// (set) Token: 0x060037A4 RID: 14244 RVA: 0x002F78C0 File Offset: 0x002F5AC0
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

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x060037A5 RID: 14245 RVA: 0x002F7908 File Offset: 0x002F5B08
		public object LockSpread
		{
			get
			{
				return this._lockSpread;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x060037A6 RID: 14246 RVA: 0x002F7920 File Offset: 0x002F5B20
		// (set) Token: 0x060037A7 RID: 14247 RVA: 0x002F793D File Offset: 0x002F5B3D
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

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x060037A8 RID: 14248 RVA: 0x002F794C File Offset: 0x002F5B4C
		public StarConstellationProp RoleStarConstellationProp
		{
			get
			{
				return this._RoleStarConstellationProp;
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x060037A9 RID: 14249 RVA: 0x002F7964 File Offset: 0x002F5B64
		// (set) Token: 0x060037AA RID: 14250 RVA: 0x002F797C File Offset: 0x002F5B7C
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

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x060037AB RID: 14251 RVA: 0x002F7988 File Offset: 0x002F5B88
		// (set) Token: 0x060037AC RID: 14252 RVA: 0x002F79D0 File Offset: 0x002F5BD0
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

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x060037AD RID: 14253 RVA: 0x002F7A18 File Offset: 0x002F5C18
		// (set) Token: 0x060037AE RID: 14254 RVA: 0x002F7A60 File Offset: 0x002F5C60
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

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x060037AF RID: 14255 RVA: 0x002F7AA8 File Offset: 0x002F5CA8
		// (set) Token: 0x060037B0 RID: 14256 RVA: 0x002F7AF0 File Offset: 0x002F5CF0
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

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x060037B1 RID: 14257 RVA: 0x002F7B38 File Offset: 0x002F5D38
		// (set) Token: 0x060037B2 RID: 14258 RVA: 0x002F7B80 File Offset: 0x002F5D80
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

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x060037B3 RID: 14259 RVA: 0x002F7BC8 File Offset: 0x002F5DC8
		// (set) Token: 0x060037B4 RID: 14260 RVA: 0x002F7C10 File Offset: 0x002F5E10
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

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x060037B5 RID: 14261 RVA: 0x002F7C58 File Offset: 0x002F5E58
		// (set) Token: 0x060037B6 RID: 14262 RVA: 0x002F7CA0 File Offset: 0x002F5EA0
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

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060037B7 RID: 14263 RVA: 0x002F7CE8 File Offset: 0x002F5EE8
		// (set) Token: 0x060037B8 RID: 14264 RVA: 0x002F7D30 File Offset: 0x002F5F30
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

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060037B9 RID: 14265 RVA: 0x002F7D78 File Offset: 0x002F5F78
		// (set) Token: 0x060037BA RID: 14266 RVA: 0x002F7DC0 File Offset: 0x002F5FC0
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

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060037BB RID: 14267 RVA: 0x002F7E08 File Offset: 0x002F6008
		// (set) Token: 0x060037BC RID: 14268 RVA: 0x002F7E54 File Offset: 0x002F6054
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

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060037BD RID: 14269 RVA: 0x002F7EA0 File Offset: 0x002F60A0
		// (set) Token: 0x060037BE RID: 14270 RVA: 0x002F7EEC File Offset: 0x002F60EC
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

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060037BF RID: 14271 RVA: 0x002F7F38 File Offset: 0x002F6138
		// (set) Token: 0x060037C0 RID: 14272 RVA: 0x002F7F80 File Offset: 0x002F6180
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

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060037C1 RID: 14273 RVA: 0x002F7FC8 File Offset: 0x002F61C8
		// (set) Token: 0x060037C2 RID: 14274 RVA: 0x002F8010 File Offset: 0x002F6210
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

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x060037C3 RID: 14275 RVA: 0x002F8058 File Offset: 0x002F6258
		// (set) Token: 0x060037C4 RID: 14276 RVA: 0x002F80A0 File Offset: 0x002F62A0
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

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x060037C5 RID: 14277 RVA: 0x002F80E8 File Offset: 0x002F62E8
		// (set) Token: 0x060037C6 RID: 14278 RVA: 0x002F8134 File Offset: 0x002F6334
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

		// Token: 0x060037C7 RID: 14279 RVA: 0x002F8180 File Offset: 0x002F6380
		public void AddAwardRecord(RoleAwardMsg type, MoneyTypes moneyType, int count)
		{
			this.AddAwardRecord(type, GoodsUtil.GetResGoodsData(moneyType, count), false);
		}

		// Token: 0x060037C8 RID: 14280 RVA: 0x002F8194 File Offset: 0x002F6394
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

		// Token: 0x060037C9 RID: 14281 RVA: 0x002F81FC File Offset: 0x002F63FC
		public void AddAwardRecord(RoleAwardMsg type, string goodsDataListString, bool useOld = false)
		{
			if (!string.IsNullOrEmpty(goodsDataListString))
			{
				AwardsItemList list = new AwardsItemList();
				list.Add(goodsDataListString);
				this.AddAwardRecord(type, Global.ConvertToGoodsDataList(list.Items, -1), useOld);
			}
		}

		// Token: 0x060037CA RID: 14282 RVA: 0x002F823C File Offset: 0x002F643C
		public void AddAwardRecord(RoleAwardMsg type, AwardsItemList list, bool useOld = false)
		{
			this.AddAwardRecord(type, Global.ConvertToGoodsDataList(list.Items, -1), useOld);
		}

		// Token: 0x060037CB RID: 14283 RVA: 0x002F8254 File Offset: 0x002F6454
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

		// Token: 0x060037CC RID: 14284 RVA: 0x002F82DC File Offset: 0x002F64DC
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

		// Token: 0x060037CD RID: 14285 RVA: 0x002F8314 File Offset: 0x002F6514
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

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060037CE RID: 14286 RVA: 0x002F8364 File Offset: 0x002F6564
		// (set) Token: 0x060037CF RID: 14287 RVA: 0x002F837C File Offset: 0x002F657C
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

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x060037D0 RID: 14288 RVA: 0x002F8388 File Offset: 0x002F6588
		// (set) Token: 0x060037D1 RID: 14289 RVA: 0x002F83D4 File Offset: 0x002F65D4
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

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x060037D2 RID: 14290 RVA: 0x002F8420 File Offset: 0x002F6620
		// (set) Token: 0x060037D3 RID: 14291 RVA: 0x002F846C File Offset: 0x002F666C
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

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060037D4 RID: 14292 RVA: 0x002F84B8 File Offset: 0x002F66B8
		// (set) Token: 0x060037D5 RID: 14293 RVA: 0x002F8504 File Offset: 0x002F6704
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

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060037D6 RID: 14294 RVA: 0x002F8550 File Offset: 0x002F6750
		public RoleTianTiData TianTiData
		{
			get
			{
				return this._RoleDataEx.TianTiData;
			}
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060037D7 RID: 14295 RVA: 0x002F8570 File Offset: 0x002F6770
		// (set) Token: 0x060037D8 RID: 14296 RVA: 0x002F85BC File Offset: 0x002F67BC
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

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060037D9 RID: 14297 RVA: 0x002F8608 File Offset: 0x002F6808
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

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x060037DA RID: 14298 RVA: 0x002F8654 File Offset: 0x002F6854
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

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x060037DB RID: 14299 RVA: 0x002F86A0 File Offset: 0x002F68A0
		// (set) Token: 0x060037DC RID: 14300 RVA: 0x002F86EC File Offset: 0x002F68EC
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

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x060037DD RID: 14301 RVA: 0x002F8738 File Offset: 0x002F6938
		// (set) Token: 0x060037DE RID: 14302 RVA: 0x002F8784 File Offset: 0x002F6984
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

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x060037DF RID: 14303 RVA: 0x002F87D0 File Offset: 0x002F69D0
		// (set) Token: 0x060037E0 RID: 14304 RVA: 0x002F881C File Offset: 0x002F6A1C
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

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x060037E1 RID: 14305 RVA: 0x002F8868 File Offset: 0x002F6A68
		// (set) Token: 0x060037E2 RID: 14306 RVA: 0x002F88B4 File Offset: 0x002F6AB4
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

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x060037E3 RID: 14307 RVA: 0x002F8900 File Offset: 0x002F6B00
		// (set) Token: 0x060037E4 RID: 14308 RVA: 0x002F894C File Offset: 0x002F6B4C
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

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x060037E5 RID: 14309 RVA: 0x002F8998 File Offset: 0x002F6B98
		// (set) Token: 0x060037E6 RID: 14310 RVA: 0x002F89E4 File Offset: 0x002F6BE4
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

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x060037E7 RID: 14311 RVA: 0x002F8A30 File Offset: 0x002F6C30
		// (set) Token: 0x060037E8 RID: 14312 RVA: 0x002F8A7C File Offset: 0x002F6C7C
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

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x060037E9 RID: 14313 RVA: 0x002F8AC8 File Offset: 0x002F6CC8
		// (set) Token: 0x060037EA RID: 14314 RVA: 0x002F8B14 File Offset: 0x002F6D14
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

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x060037EB RID: 14315 RVA: 0x002F8B60 File Offset: 0x002F6D60
		// (set) Token: 0x060037EC RID: 14316 RVA: 0x002F8BAC File Offset: 0x002F6DAC
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

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x060037ED RID: 14317 RVA: 0x002F8BF8 File Offset: 0x002F6DF8
		// (set) Token: 0x060037EE RID: 14318 RVA: 0x002F8C44 File Offset: 0x002F6E44
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

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x060037EF RID: 14319 RVA: 0x002F8C90 File Offset: 0x002F6E90
		// (set) Token: 0x060037F0 RID: 14320 RVA: 0x002F8CDC File Offset: 0x002F6EDC
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

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x060037F1 RID: 14321 RVA: 0x002F8D28 File Offset: 0x002F6F28
		// (set) Token: 0x060037F2 RID: 14322 RVA: 0x002F8D70 File Offset: 0x002F6F70
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

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x060037F3 RID: 14323 RVA: 0x002F8DB8 File Offset: 0x002F6FB8
		// (set) Token: 0x060037F4 RID: 14324 RVA: 0x002F8E00 File Offset: 0x002F7000
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

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x060037F5 RID: 14325 RVA: 0x002F8E48 File Offset: 0x002F7048
		public object LockFund
		{
			get
			{
				return this._LockFund;
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x060037F6 RID: 14326 RVA: 0x002F8E60 File Offset: 0x002F7060
		// (set) Token: 0x060037F7 RID: 14327 RVA: 0x002F8EA8 File Offset: 0x002F70A8
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

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x060037F8 RID: 14328 RVA: 0x002F8EF0 File Offset: 0x002F70F0
		// (set) Token: 0x060037F9 RID: 14329 RVA: 0x002F8F40 File Offset: 0x002F7140
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

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x060037FA RID: 14330 RVA: 0x002F8F90 File Offset: 0x002F7190
		// (set) Token: 0x060037FB RID: 14331 RVA: 0x002F8FE0 File Offset: 0x002F71E0
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

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x060037FC RID: 14332 RVA: 0x002F9030 File Offset: 0x002F7230
		// (set) Token: 0x060037FD RID: 14333 RVA: 0x002F9080 File Offset: 0x002F7280
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

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x060037FE RID: 14334 RVA: 0x002F90D0 File Offset: 0x002F72D0
		// (set) Token: 0x060037FF RID: 14335 RVA: 0x002F9120 File Offset: 0x002F7320
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

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06003800 RID: 14336 RVA: 0x002F9170 File Offset: 0x002F7370
		// (set) Token: 0x06003801 RID: 14337 RVA: 0x002F91C0 File Offset: 0x002F73C0
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

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06003802 RID: 14338 RVA: 0x002F9210 File Offset: 0x002F7410
		// (set) Token: 0x06003803 RID: 14339 RVA: 0x002F9260 File Offset: 0x002F7460
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

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06003804 RID: 14340 RVA: 0x002F92B0 File Offset: 0x002F74B0
		// (set) Token: 0x06003805 RID: 14341 RVA: 0x002F9300 File Offset: 0x002F7500
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

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06003806 RID: 14342 RVA: 0x002F9350 File Offset: 0x002F7550
		// (set) Token: 0x06003807 RID: 14343 RVA: 0x002F93A0 File Offset: 0x002F75A0
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

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06003808 RID: 14344 RVA: 0x002F93F0 File Offset: 0x002F75F0
		// (set) Token: 0x06003809 RID: 14345 RVA: 0x002F9440 File Offset: 0x002F7640
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

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x0600380A RID: 14346 RVA: 0x002F9490 File Offset: 0x002F7690
		// (set) Token: 0x0600380B RID: 14347 RVA: 0x002F94DC File Offset: 0x002F76DC
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

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x0600380C RID: 14348 RVA: 0x002F9528 File Offset: 0x002F7728
		// (set) Token: 0x0600380D RID: 14349 RVA: 0x002F9547 File Offset: 0x002F7747
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

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x0600380E RID: 14350 RVA: 0x002F9558 File Offset: 0x002F7758
		// (set) Token: 0x0600380F RID: 14351 RVA: 0x002F95A0 File Offset: 0x002F77A0
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

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06003810 RID: 14352 RVA: 0x002F95E8 File Offset: 0x002F77E8
		// (set) Token: 0x06003811 RID: 14353 RVA: 0x002F9634 File Offset: 0x002F7834
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

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06003812 RID: 14354 RVA: 0x002F9680 File Offset: 0x002F7880
		// (set) Token: 0x06003813 RID: 14355 RVA: 0x002F96C8 File Offset: 0x002F78C8
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

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06003814 RID: 14356 RVA: 0x002F9710 File Offset: 0x002F7910
		// (set) Token: 0x06003815 RID: 14357 RVA: 0x002F975C File Offset: 0x002F795C
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

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06003816 RID: 14358 RVA: 0x002F97A8 File Offset: 0x002F79A8
		// (set) Token: 0x06003817 RID: 14359 RVA: 0x002F97C5 File Offset: 0x002F79C5
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

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06003818 RID: 14360 RVA: 0x002F97D4 File Offset: 0x002F79D4
		// (set) Token: 0x06003819 RID: 14361 RVA: 0x002F97F1 File Offset: 0x002F79F1
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

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x0600381A RID: 14362 RVA: 0x002F9800 File Offset: 0x002F7A00
		// (set) Token: 0x0600381B RID: 14363 RVA: 0x002F984C File Offset: 0x002F7A4C
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

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x0600381C RID: 14364 RVA: 0x002F9898 File Offset: 0x002F7A98
		// (set) Token: 0x0600381D RID: 14365 RVA: 0x002F98E4 File Offset: 0x002F7AE4
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

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x0600381E RID: 14366 RVA: 0x002F9930 File Offset: 0x002F7B30
		// (set) Token: 0x0600381F RID: 14367 RVA: 0x002F997C File Offset: 0x002F7B7C
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

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06003820 RID: 14368 RVA: 0x002F99C8 File Offset: 0x002F7BC8
		// (set) Token: 0x06003821 RID: 14369 RVA: 0x002F9A14 File Offset: 0x002F7C14
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

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06003822 RID: 14370 RVA: 0x002F9A60 File Offset: 0x002F7C60
		// (set) Token: 0x06003823 RID: 14371 RVA: 0x002F9AAC File Offset: 0x002F7CAC
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

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06003824 RID: 14372 RVA: 0x002F9AF8 File Offset: 0x002F7CF8
		// (set) Token: 0x06003825 RID: 14373 RVA: 0x002F9B44 File Offset: 0x002F7D44
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

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06003826 RID: 14374 RVA: 0x002F9B90 File Offset: 0x002F7D90
		// (set) Token: 0x06003827 RID: 14375 RVA: 0x002F9BDC File Offset: 0x002F7DDC
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

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06003828 RID: 14376 RVA: 0x002F9C28 File Offset: 0x002F7E28
		public string WorldRoleID
		{
			get
			{
				return this._RoleDataEx.WorldRoleID;
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06003829 RID: 14377 RVA: 0x002F9C48 File Offset: 0x002F7E48
		public int UserPTID
		{
			get
			{
				return this._RoleDataEx.UserPTID;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x0600382A RID: 14378 RVA: 0x002F9C68 File Offset: 0x002F7E68
		public int ServerPTID
		{
			get
			{
				return this._RoleDataEx.ServerPTID;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x0600382B RID: 14379 RVA: 0x002F9C88 File Offset: 0x002F7E88
		public string Channel
		{
			get
			{
				return this._RoleDataEx.Channel;
			}
		}

		// Token: 0x0600382C RID: 14380 RVA: 0x002F9CA8 File Offset: 0x002F7EA8
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

		// Token: 0x04003FC2 RID: 16322
		public int LastRoleCommonUseIntParamValueListTickCount = 0;

		// Token: 0x04003FC3 RID: 16323
		public long _ResetBagTicks = 0L;

		// Token: 0x04003FC4 RID: 16324
		public long _RefreshMarketTicks = 0L;

		// Token: 0x04003FC5 RID: 16325
		public long _SpriteFightTicks = 0L;

		// Token: 0x04003FC6 RID: 16326
		public long _AddBHMemberTicks = 0L;

		// Token: 0x04003FC7 RID: 16327
		public long[] _AddFriendTicks;

		// Token: 0x04003FC8 RID: 16328
		private RoleDataEx _RoleDataEx;

		// Token: 0x04003FC9 RID: 16329
		public int OccupationIndex;

		// Token: 0x04003FCA RID: 16330
		public bool IsMainOccupation;

		// Token: 0x04003FCB RID: 16331
		public int AttackType;

		// Token: 0x04003FCC RID: 16332
		public List<AllyData> AllyList;

		// Token: 0x04003FCD RID: 16333
		public object LockAlly;

		// Token: 0x04003FCE RID: 16334
		public int LingDi;

		// Token: 0x04003FCF RID: 16335
		public long LastJunTuanChatTicks;

		// Token: 0x04003FD0 RID: 16336
		public long LastJunTuanBulletinTicks;

		// Token: 0x04003FD1 RID: 16337
		public long LastCompChatTicks;

		// Token: 0x04003FD2 RID: 16338
		public int EventLastMapCode;

		// Token: 0x04003FD3 RID: 16339
		public ClientExtData ClientExtData;

		// Token: 0x04003FD4 RID: 16340
		public HashSet<int> SkillIdHashSet;

		// Token: 0x04003FD5 RID: 16341
		public HashSet<int> BufferDataListHashSet;

		// Token: 0x04003FD6 RID: 16342
		public string platType;

		// Token: 0x04003FD7 RID: 16343
		public string launch;

		// Token: 0x04003FD8 RID: 16344
		public GoodsData _CopyMapAwardTmpGoods;

		// Token: 0x04003FD9 RID: 16345
		public int FuBenPingFenAwardMoJing;

		// Token: 0x04003FDA RID: 16346
		public int LastPKPoint;

		// Token: 0x04003FDB RID: 16347
		private int _TmpPKPoint;

		// Token: 0x04003FDC RID: 16348
		private int _AdorationCount;

		// Token: 0x04003FDD RID: 16349
		private int _PKKingAdorationCount;

		// Token: 0x04003FDE RID: 16350
		private long _JingJiNextRewardTime;

		// Token: 0x04003FDF RID: 16351
		private int _PKKingAdorationDayID;

		// Token: 0x04003FE0 RID: 16352
		public object PropPointMutex;

		// Token: 0x04003FE1 RID: 16353
		private int _TotalPropPoint;

		// Token: 0x04003FE2 RID: 16354
		public int LastNotifyCombatForce;

		// Token: 0x04003FE3 RID: 16355
		public long MaxCombatForce;

		// Token: 0x04003FE4 RID: 16356
		public int NextCombatForceGiftVal;

		// Token: 0x04003FE5 RID: 16357
		private int _PropStrength;

		// Token: 0x04003FE6 RID: 16358
		private int _PropIntelligence;

		// Token: 0x04003FE7 RID: 16359
		private int _PropDexterity;

		// Token: 0x04003FE8 RID: 16360
		private int _PropConstitution;

		// Token: 0x04003FE9 RID: 16361
		private ChangeLifeProp _RoleChangeLifeProp;

		// Token: 0x04003FEA RID: 16362
		private long _ReportPosTicks;

		// Token: 0x04003FEB RID: 16363
		private long _ServerPosTicks;

		// Token: 0x04003FEC RID: 16364
		private int _CurrentAction;

		// Token: 0x04003FED RID: 16365
		private double _MoveSpeed;

		// Token: 0x04003FEE RID: 16366
		private Point _DestPoint;

		// Token: 0x04003FEF RID: 16367
		public int MinLife;

		// Token: 0x04003FF0 RID: 16368
		private int _CurrentLifeV;

		// Token: 0x04003FF1 RID: 16369
		public Queue<string> AddLifeAlertList;

		// Token: 0x04003FF2 RID: 16370
		private int _CurrentMagicV;

		// Token: 0x04003FF3 RID: 16371
		public int ArmorV;

		// Token: 0x04003FF4 RID: 16372
		public int CurrentArmorV;

		// Token: 0x04003FF5 RID: 16373
		public double ArmorPercent;

		// Token: 0x04003FF6 RID: 16374
		private int _TeamID;

		// Token: 0x04003FF7 RID: 16375
		private int _ExchangeID;

		// Token: 0x04003FF8 RID: 16376
		private long _ExchangeTicks;

		// Token: 0x04003FF9 RID: 16377
		private int _LastMapCode;

		// Token: 0x04003FFA RID: 16378
		private int _LastPosX;

		// Token: 0x04003FFB RID: 16379
		private int _LastPosY;

		// Token: 0x04003FFC RID: 16380
		private StallData _StallDataItem;

		// Token: 0x04003FFD RID: 16381
		private long _LastDBHeartTicks;

		// Token: 0x04003FFE RID: 16382
		private long _LastSiteExpTicks;

		// Token: 0x04003FFF RID: 16383
		private long _LastSiteSubPKPointTicks;

		// Token: 0x04004000 RID: 16384
		private bool _AutoFighting;

		// Token: 0x04004001 RID: 16385
		private long _LastAutoFightTicks;

		// Token: 0x04004002 RID: 16386
		private int _AutoFightingProctect;

		// Token: 0x04004003 RID: 16387
		private int _DJRoomID;

		// Token: 0x04004004 RID: 16388
		private int _DJRoomTeamID;

		// Token: 0x04004005 RID: 16389
		private bool _ViewDJRoomDlg;

		// Token: 0x04004006 RID: 16390
		private int _CopyMapID;

		// Token: 0x04004007 RID: 16391
		private GoodsPackItem _GoodsPackItem;

		// Token: 0x04004008 RID: 16392
		private int _SelectHorseDbID;

		// Token: 0x04004009 RID: 16393
		private int _PetRoleID;

		// Token: 0x0400400A RID: 16394
		public List<GoodsData> _PortableGoodsDataList;

		// Token: 0x0400400B RID: 16395
		public List<GoodsData> _JinDanGoodsDataList;

		// Token: 0x0400400C RID: 16396
		public List<GoodsData> _MeditateGoodsDataList;

		// Token: 0x0400400D RID: 16397
		private List<MountData> _MountList;

		// Token: 0x0400400E RID: 16398
		private int _IsRide;

		// Token: 0x0400400F RID: 16399
		private long _ReportPetPosTicks;

		// Token: 0x04004010 RID: 16400
		private int _PetPosX;

		// Token: 0x04004011 RID: 16401
		private int _PetPosY;

		// Token: 0x04004012 RID: 16402
		private EquipPropItem _EquipProp;

		// Token: 0x04004013 RID: 16403
		public PropsCacheManager PropsCacheManager;

		// Token: 0x04004014 RID: 16404
		public PropsCacheManager PurePropsCacheManager;

		// Token: 0x04004015 RID: 16405
		public PropsCacheManager PctPropsCacheManager;

		// Token: 0x04004016 RID: 16406
		private WeighItems _WeighItems;

		// Token: 0x04004017 RID: 16407
		public int _LastCheckGridX;

		// Token: 0x04004018 RID: 16408
		public int _LastCheckGridY;

		// Token: 0x04004019 RID: 16409
		private int _BattleKilledNum;

		// Token: 0x0400401A RID: 16410
		private int _ArenaBattleKilledNum;

		// Token: 0x0400401B RID: 16411
		private int _HideSelf;

		// Token: 0x0400401C RID: 16412
		private int _HideGM;

		// Token: 0x0400401D RID: 16413
		public int GuanZhanGM;

		// Token: 0x0400401E RID: 16414
		private int _BeTrackingRoleID;

		// Token: 0x0400401F RID: 16415
		private List<int> _TrackingRoleIDList;

		// Token: 0x04004020 RID: 16416
		public long _LastJugeSafeRegionTicks;

		// Token: 0x04004021 RID: 16417
		private int _AntiAddictionTimeType;

		// Token: 0x04004022 RID: 16418
		private Dictionary<string, int> _JingMaiPropsDict;

		// Token: 0x04004023 RID: 16419
		private int _JingMaiBodyLevel;

		// Token: 0x04004024 RID: 16420
		private bool _FirstPlayStart;

		// Token: 0x04004025 RID: 16421
		private long _LastProcessBufferTicks;

		// Token: 0x04004026 RID: 16422
		private BufferData _UpLifeLimitBufferData;

		// Token: 0x04004027 RID: 16423
		private BufferData _AddTempAttackBufferData;

		// Token: 0x04004028 RID: 16424
		private BufferData _AddTempDefenseBufferData;

		// Token: 0x04004029 RID: 16425
		private BufferData _AntiBossBufferData;

		// Token: 0x0400402A RID: 16426
		private BufferData _SheLiZhiYuanBufferData;

		// Token: 0x0400402B RID: 16427
		private BufferData _DiWangZhiYouBufferData;

		// Token: 0x0400402C RID: 16428
		private BufferData _JunQiBufferData;

		// Token: 0x0400402D RID: 16429
		private int _TempJMChongXueRate;

		// Token: 0x0400402E RID: 16430
		private int _TempHorseEnchanceRate;

		// Token: 0x0400402F RID: 16431
		private int _TempHorseUpLevelRate;

		// Token: 0x04004030 RID: 16432
		private int _AutoFightGetThings;

		// Token: 0x04004031 RID: 16433
		private Dictionary<int, long> _LastDBCmdTicksDict;

		// Token: 0x04004032 RID: 16434
		private int _ClientHeartCount;

		// Token: 0x04004033 RID: 16435
		private long _LastClientHeartTicks;

		// Token: 0x04004034 RID: 16436
		private long _LastClientServerSubTicks;

		// Token: 0x04004035 RID: 16437
		private int _LastClientServerSubNum;

		// Token: 0x04004036 RID: 16438
		private int _ClosingClientStep;

		// Token: 0x04004037 RID: 16439
		private SkillData _NumSkillData;

		// Token: 0x04004038 RID: 16440
		private int _DefaultSkillLev;

		// Token: 0x04004039 RID: 16441
		private int _DefaultSkillUseNum;

		// Token: 0x0400403A RID: 16442
		private Dictionary<int, long> _LastDBSkillCmdTicksDict;

		// Token: 0x0400403B RID: 16443
		private GoodsData _WaBaoGoodsData;

		// Token: 0x0400403C RID: 16444
		private object _UserMoneyMutex;

		// Token: 0x0400403D RID: 16445
		private object _YinLiangMutex;

		// Token: 0x0400403E RID: 16446
		private object _GoldMutex;

		// Token: 0x0400403F RID: 16447
		private int _FuBenSeqID;

		// Token: 0x04004040 RID: 16448
		private int _FuBenID;

		// Token: 0x04004041 RID: 16449
		public int NotifyFuBenID;

		// Token: 0x04004042 RID: 16450
		public int NotifyFuBenSeqID;

		// Token: 0x04004043 RID: 16451
		public int OnePieceMoveLeft;

		// Token: 0x04004044 RID: 16452
		public int OnePieceMoveDir;

		// Token: 0x04004045 RID: 16453
		public int OnePieceTempEventID;

		// Token: 0x04004046 RID: 16454
		public List<int> _OnePieceBoxIDList;

		// Token: 0x04004047 RID: 16455
		public List<GoodsData> _FallBaoXiangGoodsList;

		// Token: 0x04004048 RID: 16456
		public YaoSaiOptType YaoSaiPrisonOptType;

		// Token: 0x04004049 RID: 16457
		public int YaoSaiPrisonTargetID;

		// Token: 0x0400404A RID: 16458
		public string YaoSaiPrisonTargetName;

		// Token: 0x0400404B RID: 16459
		public bool DisableChangeRolePurpleName;

		// Token: 0x0400404C RID: 16460
		private long _StartPurpleNameTicks;

		// Token: 0x0400404D RID: 16461
		public long StartLianZhanTicks;

		// Token: 0x0400404E RID: 16462
		public long WaitingLianZhanMS;

		// Token: 0x0400404F RID: 16463
		public int TempLianZhan;

		// Token: 0x04004050 RID: 16464
		public double LianZhanExpRate;

		// Token: 0x04004051 RID: 16465
		private int _TotalLearnedSkillLevelCount;

		// Token: 0x04004052 RID: 16466
		private long _LastProcessMapLimitTimesTicks;

		// Token: 0x04004053 RID: 16467
		private int _RoleEquipJiFen;

		// Token: 0x04004054 RID: 16468
		private int _RoleXueWeiNum;

		// Token: 0x04004055 RID: 16469
		private int _RoleHorseJiFen;

		// Token: 0x04004056 RID: 16470
		private List<QueueCmdItem> _QueueCmdItemList;

		// Token: 0x04004057 RID: 16471
		private int _ChongXueFailedNum;

		// Token: 0x04004058 RID: 16472
		private long _StartTempHorseIDTicks;

		// Token: 0x04004059 RID: 16473
		private int _TempHorseID;

		// Token: 0x0400405A RID: 16474
		private int _LoginDayID;

		// Token: 0x0400405B RID: 16475
		private int _AllQualityIndex;

		// Token: 0x0400405C RID: 16476
		private int _AllForgeLevelIndex;

		// Token: 0x0400405D RID: 16477
		private int _AllJewelLevelIndex;

		// Token: 0x0400405E RID: 16478
		private int _AllZhuoYueNum;

		// Token: 0x0400405F RID: 16479
		private AllThingsCalcItem _AllThingsCalcItem;

		// Token: 0x04004060 RID: 16480
		public YangGongBKItem _MyYangGongBKItem;

		// Token: 0x04004061 RID: 16481
		public Dictionary<int, QiZhenGeItemData> _QiZhenGeGoodsDict;

		// Token: 0x04004062 RID: 16482
		private int _QiZhenGeBuyNum;

		// Token: 0x04004063 RID: 16483
		private long _EnterMapTicks;

		// Token: 0x04004064 RID: 16484
		private int _TotalUsedMoney;

		// Token: 0x04004065 RID: 16485
		private int _TotalGoodsMoney;

		// Token: 0x04004066 RID: 16486
		private int _ReportWarningGoodsMoney;

		// Token: 0x04004067 RID: 16487
		private long _LastAttackTicks;

		// Token: 0x04004068 RID: 16488
		private bool _ForceShenFenZheng;

		// Token: 0x04004069 RID: 16489
		private long _FSHuDunStart;

		// Token: 0x0400406A RID: 16490
		private int _FSHuDunSeconds;

		// Token: 0x0400406B RID: 16491
		private long _DSHideStart;

		// Token: 0x0400406C RID: 16492
		private bool _WaitingNotifyChangeMap;

		// Token: 0x0400406D RID: 16493
		public int WaitingChangeMapToMapCode;

		// Token: 0x0400406E RID: 16494
		public int WaitingChangeMapToPosX;

		// Token: 0x0400406F RID: 16495
		public int WaitingChangeMapToPosY;

		// Token: 0x04004070 RID: 16496
		public int KuaFuChangeMapCode;

		// Token: 0x04004071 RID: 16497
		private int _BattleWhichSide;

		// Token: 0x04004072 RID: 16498
		public int BirthSide;

		// Token: 0x04004073 RID: 16499
		private int _ThisTimeOnlineSecs;

		// Token: 0x04004074 RID: 16500
		private MagicCoolDownMgr _MagicCoolDownMgr;

		// Token: 0x04004075 RID: 16501
		private int _LastSkillID;

		// Token: 0x04004076 RID: 16502
		private GoodsCoolDownMgr _GoodsCoolDownMgr;

		// Token: 0x04004077 RID: 16503
		public int CurrentMagicCode;

		// Token: 0x04004078 RID: 16504
		public long CurrentMagicTicks;

		// Token: 0x04004079 RID: 16505
		public double CurrentMagicCDSubPercent;

		// Token: 0x0400407A RID: 16506
		public long CurrentMagicActionEndTicks;

		// Token: 0x0400407B RID: 16507
		public SpriteActionData CurrentActionData;

		// Token: 0x0400407C RID: 16508
		private string _MailSendSecurityCode;

		// Token: 0x0400407D RID: 16509
		private long _RoleStartMoveTicks;

		// Token: 0x0400407E RID: 16510
		public long _InstantMoveTick;

		// Token: 0x0400407F RID: 16511
		private string _RolePathString;

		// Token: 0x04004080 RID: 16512
		private double _TengXunFCMRate;

		// Token: 0x04004081 RID: 16513
		private Dictionary<string, long> _LastDBRoleParamCmdTicksDict;

		// Token: 0x04004082 RID: 16514
		private Dictionary<int, long> _LastDBEquipStrongCmdTicksDict;

		// Token: 0x04004083 RID: 16515
		private uint _TotalKilledMonsterNum;

		// Token: 0x04004084 RID: 16516
		private ushort _TimerKilledMonsterNum;

		// Token: 0x04004085 RID: 16517
		private uint _NextKillMonsterChengJiuNum;

		// Token: 0x04004086 RID: 16518
		private int _MaxTongQianNum;

		// Token: 0x04004087 RID: 16519
		private uint _NextTongQianChengJiuNum;

		// Token: 0x04004088 RID: 16520
		private uint _TotalDayLoginNum;

		// Token: 0x04004089 RID: 16521
		private uint _ContinuousDayLoginNum;

		// Token: 0x0400408A RID: 16522
		private int _ChengJiuPoints;

		// Token: 0x0400408B RID: 16523
		private int _ChengJiuLevel;

		// Token: 0x0400408C RID: 16524
		private int _ShenLiJingHuaPoints;

		// Token: 0x0400408D RID: 16525
		private int _NengLiangSmall;

		// Token: 0x0400408E RID: 16526
		private int _NengLiangMedium;

		// Token: 0x0400408F RID: 16527
		private int _NengLiangBig;

		// Token: 0x04004090 RID: 16528
		private int _NengLiangSuper;

		// Token: 0x04004091 RID: 16529
		private int _FuWenZhiChen;

		// Token: 0x04004092 RID: 16530
		private int _WanMoTaNextLayerOrder;

		// Token: 0x04004093 RID: 16531
		public SkillEquipData ShenShiEquipData;

		// Token: 0x04004094 RID: 16532
		public List<int> PassiveEffectList;

		// Token: 0x04004095 RID: 16533
		public List<Monster> _SummonMonstersList;

		// Token: 0x04004096 RID: 16534
		private long _StartAddExpTicks;

		// Token: 0x04004097 RID: 16535
		private Dictionary<int, BufferData> _BufferDataDict;

		// Token: 0x04004098 RID: 16536
		private long _StartAddLifeMagicTicks;

		// Token: 0x04004099 RID: 16537
		private long _StartAddLifeNoShowTicks;

		// Token: 0x0400409A RID: 16538
		private long _StartAddMaigcNoShowTicks;

		// Token: 0x0400409B RID: 16539
		private long _DSStartDSAddLifeNoShowTicks;

		// Token: 0x0400409D RID: 16541
		private long _LastLogRoleIDAttackebByMyselfTicks;

		// Token: 0x0400409E RID: 16542
		private int _RoleIDAttackebByMyself;

		// Token: 0x0400409F RID: 16543
		private long _LastLogRoleIDAttackMeTicks;

		// Token: 0x040040A0 RID: 16544
		private int _RoleIDAttackMe;

		// Token: 0x040040A1 RID: 16545
		public List<int> _RoleCommonUseIntPamams;

		// Token: 0x040040A2 RID: 16546
		private long _LastMapLimitUpdateTicks;

		// Token: 0x040040A3 RID: 16547
		private long _LastHintToUpdateClientTicks;

		// Token: 0x040040A4 RID: 16548
		private int[] _BaseBattleAttributesOfLastTime;

		// Token: 0x040040A5 RID: 16549
		private long _LastGoodsLimitUpdateTicks;

		// Token: 0x040040A6 RID: 16550
		private long _LastFashionLimitUpdateTicks;

		// Token: 0x040040A7 RID: 16551
		private long _LastRoleDeadTicks;

		// Token: 0x040040A8 RID: 16552
		private int _MoveAndActionNum;

		// Token: 0x040040A9 RID: 16553
		public Dictionary<object, byte> _VisibleGrid9Objects;

		// Token: 0x040040AA RID: 16554
		public Dictionary<object, byte> _VisibleMeGrid9GameClients;

		// Token: 0x040040AB RID: 16555
		private List<QiangGouItemData> _QiangGouItemList;

		// Token: 0x040040AC RID: 16556
		private long _ZhongDuStart;

		// Token: 0x040040AD RID: 16557
		private int _ZhongDuSeconds;

		// Token: 0x040040AE RID: 16558
		private int _FangDuRoleID;

		// Token: 0x040040AF RID: 16559
		private long _DSStartDSSubLifeNoShowTicks;

		// Token: 0x040040B0 RID: 16560
		private int _JieriChengHao;

		// Token: 0x040040B1 RID: 16561
		private long _SpecialEquipLastUseTicks;

		// Token: 0x040040B2 RID: 16562
		private long _DongJieStart;

		// Token: 0x040040B3 RID: 16563
		private int _DongJieSeconds;

		// Token: 0x040040B4 RID: 16564
		private object _PickUpGoodsPackMutex;

		// Token: 0x040040B5 RID: 16565
		private long _MeditateTicks;

		// Token: 0x040040B6 RID: 16566
		public long GiveMeditateAwardOffsetTicks;

		// Token: 0x040040B7 RID: 16567
		public int GiveMeditateGoodsInterval;

		// Token: 0x040040B8 RID: 16568
		public long LastProcessDeadTicks;

		// Token: 0x040040B9 RID: 16569
		private int _MeditateTime;

		// Token: 0x040040BA RID: 16570
		private int _NotSafeMeditateTime;

		// Token: 0x040040BB RID: 16571
		private int _StartMeditate;

		// Token: 0x040040BC RID: 16572
		public long LastMovePosTicks;

		// Token: 0x040040BD RID: 16573
		private object _StoreYinLiangMutex;

		// Token: 0x040040BE RID: 16574
		private object _StoreMoneyMutex;

		// Token: 0x040040BF RID: 16575
		private bool _bIsInBloodCastleMap;

		// Token: 0x040040C0 RID: 16576
		private int _BloodCastleAwardPoint;

		// Token: 0x040040C1 RID: 16577
		private int _BloodCastleAwardPointTmp;

		// Token: 0x040040C2 RID: 16578
		private int _BloodCastleAwardTotalPoint;

		// Token: 0x040040C3 RID: 16579
		private int _CampBattleTotalPoint;

		// Token: 0x040040C4 RID: 16580
		private bool _bIsInDaimonSquareMap;

		// Token: 0x040040C5 RID: 16581
		private int _DaimonSquarePoint;

		// Token: 0x040040C6 RID: 16582
		private int _DaimonSquarePointTotalPoint;

		// Token: 0x040040C7 RID: 16583
		private int _KingOfPkCurrentPoint;

		// Token: 0x040040C8 RID: 16584
		private int _KingOfPkTopPoint;

		// Token: 0x040040C9 RID: 16585
		private long _AngelTempleCurrentPoint;

		// Token: 0x040040CA RID: 16586
		public long m_NotifyInfoTickForSingle;

		// Token: 0x040040CB RID: 16587
		private long _AngelTempleTopPoint;

		// Token: 0x040040CC RID: 16588
		private bool _bIsInAngelTempleMap;

		// Token: 0x040040CD RID: 16589
		private double[] _ExcellenceProp;

		// Token: 0x040040CE RID: 16590
		private double _LuckProp;

		// Token: 0x040040CF RID: 16591
		private int _DayOnlineSecond;

		// Token: 0x040040D0 RID: 16592
		private int _BakDayOnlineSecond;

		// Token: 0x040040D1 RID: 16593
		private long _DayOnlineRecSecond;

		// Token: 0x040040D2 RID: 16594
		public int _SeriesLoginNum;

		// Token: 0x040040D3 RID: 16595
		private int _DailyActiveValues;

		// Token: 0x040040D4 RID: 16596
		private int _DailyActiveDayID;

		// Token: 0x040040D5 RID: 16597
		private uint _DailyActiveDayLginCount;

		// Token: 0x040040D6 RID: 16598
		private bool _DailyActiveDayLginSetFlag;

		// Token: 0x040040D7 RID: 16599
		private int _DailyActiveDayBuyItemInMall;

		// Token: 0x040040D8 RID: 16600
		private uint _DailyTotalKillMonsterNum;

		// Token: 0x040040D9 RID: 16601
		private uint _DailyTotalKillKillBossNum;

		// Token: 0x040040DA RID: 16602
		private uint _DailyCompleteDailyTaskCount;

		// Token: 0x040040DB RID: 16603
		private uint _DailyNextKillMonsterNum;

		// Token: 0x040040DC RID: 16604
		private int _DailyOnlineTimeTmp;

		// Token: 0x040040DD RID: 16605
		private bool _AllowMarketBuy;

		// Token: 0x040040DE RID: 16606
		private int _OfflineMarketState;

		// Token: 0x040040DF RID: 16607
		private string _MarketName;

		// Token: 0x040040E0 RID: 16608
		public long VipExp;

		// Token: 0x040040E1 RID: 16609
		private int _VipLevel;

		// Token: 0x040040E2 RID: 16610
		private int _VipAwardFlag;

		// Token: 0x040040E3 RID: 16611
		private List<GoodsData> _DailyOnLineAwardGift;

		// Token: 0x040040E4 RID: 16612
		private List<GoodsData> _SeriesLoginAwardGift;

		// Token: 0x040040E5 RID: 16613
		private int _OpenGridTime;

		// Token: 0x040040E6 RID: 16614
		private int _OpenPortableGridTime;

		// Token: 0x040040E7 RID: 16615
		public Point OpenPortableBagPoint;

		// Token: 0x040040E8 RID: 16616
		public HashSet<int> ActivedTuJianItem;

		// Token: 0x040040E9 RID: 16617
		public HashSet<int> ActivedTuJianType;

		// Token: 0x040040EA RID: 16618
		private WanMotaInfo _WanMoTaProp;

		// Token: 0x040040EB RID: 16619
		private LayerRewardData _LayerRewardData;

		// Token: 0x040040EC RID: 16620
		private SweepWanmota _WanMoTaSweeping;

		// Token: 0x040040ED RID: 16621
		public List<int> MapCodeAlreadyList;

		// Token: 0x040040EE RID: 16622
		private bool _WaitingForChangeMap;

		// Token: 0x040040EF RID: 16623
		public SceneUIClasses SceneType;

		// Token: 0x040040F0 RID: 16624
		public int SceneMapCode;

		// Token: 0x040040F1 RID: 16625
		public int MoJingExchangeDayID;

		// Token: 0x040040F2 RID: 16626
		private Dictionary<int, int> _MoJingExchangeInfo;

		// Token: 0x040040F3 RID: 16627
		private int _MaxAntiProcessJiaSuSubNum;

		// Token: 0x040040F4 RID: 16628
		public int CompleteTaskZhangJie;

		// Token: 0x040040F5 RID: 16629
		public long LastNotifyChangeMapTicks;

		// Token: 0x040040F6 RID: 16630
		public long LastChangeMapTicks;

		// Token: 0x040040F7 RID: 16631
		public int TempWashPropOperationIndex;

		// Token: 0x040040F8 RID: 16632
		public Dictionary<int, UpdateGoodsArgs> TempWashPropsDict;

		// Token: 0x040040F9 RID: 16633
		public double nTempWorldLevelPer;

		// Token: 0x040040FA RID: 16634
		public SpriteExtensionProps ExtensionProps;

		// Token: 0x040040FB RID: 16635
		public DailyTaskData YesterdayDailyTaskData;

		// Token: 0x040040FC RID: 16636
		public DailyTaskData YesterdayTaofaTaskData;

		// Token: 0x040040FD RID: 16637
		public Dictionary<int, OldResourceInfo> OldResourceInfoDict;

		// Token: 0x040040FE RID: 16638
		private List<FuBenData> _OldFuBenDataList;

		// Token: 0x040040FF RID: 16639
		public AchievementRuneData achievementRuneData;

		// Token: 0x04004100 RID: 16640
		public ShenQiData shenQiData;

		// Token: 0x04004101 RID: 16641
		public PrestigeMedalData prestigeMedalData;

		// Token: 0x04004102 RID: 16642
		public UnionPalaceData MyUnionPalaceData;

		// Token: 0x04004103 RID: 16643
		private object _LockUnionPalace;

		// Token: 0x04004104 RID: 16644
		public int EveryDayUpDate;

		// Token: 0x04004105 RID: 16645
		public UserReturnData ReturnData;

		// Token: 0x04004106 RID: 16646
		private TalentPropData _myTalentPropData;

		// Token: 0x04004107 RID: 16647
		private SpreadData _mySpreadData;

		// Token: 0x04004108 RID: 16648
		private SpreadVerifyData _mySpreadVerifyData;

		// Token: 0x04004109 RID: 16649
		private object _lockSpread;

		// Token: 0x0400410A RID: 16650
		private StarConstellationProp _RoleStarConstellationProp;

		// Token: 0x0400410B RID: 16651
		public int _StarSoul;

		// Token: 0x0400410C RID: 16652
		private int _RoleYAngle;

		// Token: 0x0400410D RID: 16653
		private uint _DisMountTick;

		// Token: 0x0400410E RID: 16654
		private uint _CaiJiStartTick;

		// Token: 0x0400410F RID: 16655
		private int _CaijTargetId;

		// Token: 0x04004110 RID: 16656
		public long CaiJiTargetUniqueID;

		// Token: 0x04004111 RID: 16657
		public long CaijGoodsDBId;

		// Token: 0x04004112 RID: 16658
		public int gatherNpcID;

		// Token: 0x04004113 RID: 16659
		public long gatherTicks;

		// Token: 0x04004114 RID: 16660
		private int _DailyCrystalCollectNum;

		// Token: 0x04004115 RID: 16661
		private int _CrystalCollectDayID;

		// Token: 0x04004116 RID: 16662
		public OldCaiJiData OldCrystalCollectData;

		// Token: 0x04004117 RID: 16663
		public int _LingDiCaiJiNum;

		// Token: 0x04004118 RID: 16664
		private int _OnlineActiveVal;

		// Token: 0x04004119 RID: 16665
		public YueKaDetail YKDetail;

		// Token: 0x0400411A RID: 16666
		private long _ShuiJingHuanJingTicks;

		// Token: 0x0400411B RID: 16667
		private long _GetLiPinMaTicks;

		// Token: 0x0400411C RID: 16668
		private KingOfBattleStoreData _KingOfBattleStroeData;

		// Token: 0x0400411D RID: 16669
		public KuaFuLueDuoStoreData KuaFuLueDuoStoreData;

		// Token: 0x0400411E RID: 16670
		public RoleAwardMsg RoleAwardMsgType;

		// Token: 0x0400411F RID: 16671
		private Dictionary<RoleAwardMsg, List<GoodsData>> _TmpAwardRecordDict;

		// Token: 0x04004120 RID: 16672
		private DateTime _LastLoginTime;

		// Token: 0x04004121 RID: 16673
		public ReplaceExtArg _ReplaceExtArg;

		// Token: 0x04004122 RID: 16674
		public int SignUpGameType;

		// Token: 0x04004123 RID: 16675
		public TianTi5v5ZhanDuiRoleData TianTi5v5Data;

		// Token: 0x04004124 RID: 16676
		public int LangHunLingYuCityAwardsCheckDayId;

		// Token: 0x04004125 RID: 16677
		public int LangHunLingYuCityAwardsLevelFlags;

		// Token: 0x04004126 RID: 16678
		public int LangHunLingYuCityAwardsLevelFlagsSelf;

		// Token: 0x04004127 RID: 16679
		public int LangHunLingYuCityAwardsDay;

		// Token: 0x04004128 RID: 16680
		public bool LangHunLingYuCityAwardsCanGet;

		// Token: 0x04004129 RID: 16681
		public long ZhengDuoDataAge;

		// Token: 0x0400412A RID: 16682
		public LongCollection MoneyData;

		// Token: 0x0400412B RID: 16683
		public ZuoQiMainData ZuoQiMainData;

		// Token: 0x0400412C RID: 16684
		public bool IsSoulStoneOpened;

		// Token: 0x0400412D RID: 16685
		public object SpecPriorityActMutex;

		// Token: 0x0400412E RID: 16686
		public object ChargeItemMutex;

		// Token: 0x0400412F RID: 16687
		private Dictionary<int, int> _ChargeItemPurchaseDict;

		// Token: 0x04004130 RID: 16688
		public int ChargeItemDayPurchaseDayID;

		// Token: 0x04004131 RID: 16689
		private Dictionary<int, int> _ChargeItemDayPurchaseDict;

		// Token: 0x04004132 RID: 16690
		public FundData MyFundData;

		// Token: 0x04004133 RID: 16691
		private object _LockFund;

		// Token: 0x04004134 RID: 16692
		public List<ZhengBaSupportFlagData> ZhengBaSupportFlags;

		// Token: 0x04004135 RID: 16693
		private List<BHMatchSupportData> _BHMatchSupportList;

		// Token: 0x04004136 RID: 16694
		private int _OpenRebornBagTime;

		// Token: 0x04004137 RID: 16695
		private int _OpenRebornGridTime;

		// Token: 0x04004138 RID: 16696
		public long[] UpdateHongBaoLogTicks;

		// Token: 0x04004139 RID: 16697
		public List<HongBaoItemData>[] HongBaoLogLists;

		// Token: 0x0400413A RID: 16698
		public int LocalRoleID;

		// Token: 0x0400413B RID: 16699
		public DeControlItemData[] DeControlItemArray;

		// Token: 0x0400413C RID: 16700
		public GVoiceTypes GVoiceType;

		// Token: 0x0400413D RID: 16701
		public string GVoicePrioritys;
	}
}
