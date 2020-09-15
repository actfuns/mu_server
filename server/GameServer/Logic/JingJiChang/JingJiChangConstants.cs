using System;
using System.Collections.Generic;

namespace GameServer.Logic.JingJiChang
{
	// Token: 0x0200072E RID: 1838
	public class JingJiChangConstants
	{
		// Token: 0x06002CA6 RID: 11430 RVA: 0x0027CE34 File Offset: 0x0027B034
		public static int GetJingJiChangeHighPrioritySkill(int eOccupation, EMagicSwordTowardType eMagicSwordType)
		{
			switch (eOccupation)
			{
			case 0:
				return -1;
			case 1:
				return -1;
			case 2:
				return -1;
			case 3:
				switch (eMagicSwordType)
				{
				case EMagicSwordTowardType.EMST_Strength:
					return -1;
				case EMagicSwordTowardType.EMST_Intelligence:
					return -1;
				}
				break;
			case 5:
				return JingJiChangConstants.SummonerSkillFirst;
			}
			return -1;
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x0027CE9C File Offset: 0x0027B09C
		public static int[] GetJingJiChangeSkillList(GameClient client, int eOccupation, EMagicSwordTowardType eMagicSwordType)
		{
			List<int> ret = new List<int>();
			switch (eOccupation)
			{
			case 0:
				ret.AddRange(JingJiChangConstants.ZhanShiSkillList);
				break;
			case 1:
				ret.AddRange(JingJiChangConstants.FaShiSkillList);
				break;
			case 2:
				ret.AddRange(JingJiChangConstants.GongJianShouSkillList);
				break;
			case 3:
				switch (eMagicSwordType)
				{
				case EMagicSwordTowardType.EMST_Strength:
					ret.AddRange(JingJiChangConstants.StrMagicSwordSkillList);
					break;
				case EMagicSwordTowardType.EMST_Intelligence:
					ret.AddRange(JingJiChangConstants.IntMagicSwordSkillList);
					break;
				}
				break;
			case 5:
				ret.AddRange(JingJiChangConstants.SummonerSkillList);
				break;
			}
			ret.AddRange(ZuoQiManager.getInstance().GetZuoQiSkillList(client));
			return ret.ToArray();
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x0027CF58 File Offset: 0x0027B158
		public static int[] getJingJiChangeFiveCombatSkillList(int eOccupation, EMagicSwordTowardType eMagicSwordType)
		{
			switch (eOccupation)
			{
			case 0:
				return JingJiChangConstants.ZhanShiFiveCombotSkillList;
			case 1:
				return JingJiChangConstants.FaShiFiveCombotSkillList;
			case 2:
				return JingJiChangConstants.GongJianShouFiveCombotSkillList;
			case 3:
				switch (eMagicSwordType)
				{
				case EMagicSwordTowardType.EMST_Strength:
					return JingJiChangConstants.StrMagicSwordFiveCombotSkillList;
				case EMagicSwordTowardType.EMST_Intelligence:
					return JingJiChangConstants.IntMagicSwordFiveCombotSkillList;
				}
				break;
			case 5:
				return JingJiChangConstants.SummonerFiveCombotSkillList;
			}
			return null;
		}

		// Token: 0x04003B42 RID: 15170
		public static readonly int RobotBothX = 4684;

		// Token: 0x04003B43 RID: 15171
		public static readonly int RobotBothY = 4684;

		// Token: 0x04003B44 RID: 15172
		public static readonly int CanChallengeNum = 3;

		// Token: 0x04003B45 RID: 15173
		public static readonly int RankingListMaxNum = 5000;

		// Token: 0x04003B46 RID: 15174
		public static readonly int Enter_Type_Free = 0;

		// Token: 0x04003B47 RID: 15175
		public static readonly int Enter_Type_Vip = 1;

		// Token: 0x04003B48 RID: 15176
		public static readonly long Challenge_CD_Time = 180000L;

		// Token: 0x04003B49 RID: 15177
		public static readonly long RankingReward_CD_Time = 86400000L;

		// Token: 0x04003B4A RID: 15178
		public static readonly int[] ZhanShiSkillList = new int[]
		{
			120,
			105,
			106,
			100,
			187,
			188,
			189,
			190
		};

		// Token: 0x04003B4B RID: 15179
		public static readonly int[] ZhanShiFiveCombotSkillList = new int[]
		{
			100,
			187,
			188,
			189,
			190
		};

		// Token: 0x04003B4C RID: 15180
		public static readonly int[] FaShiSkillList = new int[]
		{
			204,
			206,
			220,
			200,
			287,
			288,
			289,
			290
		};

		// Token: 0x04003B4D RID: 15181
		public static readonly int[] FaShiFiveCombotSkillList = new int[]
		{
			200,
			287,
			288,
			289,
			290
		};

		// Token: 0x04003B4E RID: 15182
		public static readonly int[] GongJianShouSkillList = new int[]
		{
			305,
			301,
			306,
			300,
			388,
			389,
			390,
			391
		};

		// Token: 0x04003B4F RID: 15183
		public static readonly int[] GongJianShouFiveCombotSkillList = new int[]
		{
			300,
			388,
			389,
			390,
			391
		};

		// Token: 0x04003B50 RID: 15184
		public static readonly int[] StrMagicSwordSkillList = new int[]
		{
			10007,
			10001,
			10004,
			10000,
			10088,
			10089,
			10090,
			10091
		};

		// Token: 0x04003B51 RID: 15185
		public static readonly int[] StrMagicSwordFiveCombotSkillList = new int[]
		{
			10000,
			10088,
			10089,
			10090,
			10091
		};

		// Token: 0x04003B52 RID: 15186
		public static readonly int[] IntMagicSwordSkillList = new int[]
		{
			10107,
			10101,
			10104,
			10100,
			10188,
			10189,
			10190,
			10191
		};

		// Token: 0x04003B53 RID: 15187
		public static readonly int[] IntMagicSwordFiveCombotSkillList = new int[]
		{
			10100,
			10188,
			10189,
			10190,
			10191
		};

		// Token: 0x04003B54 RID: 15188
		public static readonly int[] SummonerSkillList = new int[]
		{
			11006,
			11001,
			11004,
			11007,
			11000,
			11088,
			11089,
			11090,
			11091
		};

		// Token: 0x04003B55 RID: 15189
		public static readonly int[] SummonerFiveCombotSkillList = new int[]
		{
			11000,
			11088,
			11089,
			11090,
			11091
		};

		// Token: 0x04003B56 RID: 15190
		public static readonly int SummonerSkillFirst = 11007;

		// Token: 0x04003B57 RID: 15191
		public static readonly int[][] SkillFrameCounts = new int[][]
		{
			new int[]
			{
				120,
				11,
				5
			},
			new int[]
			{
				105,
				15,
				9
			},
			new int[]
			{
				106,
				11,
				5
			},
			new int[]
			{
				100,
				2,
				2
			},
			new int[]
			{
				187,
				2,
				2
			},
			new int[]
			{
				188,
				2,
				2
			},
			new int[]
			{
				189,
				2,
				2
			},
			new int[]
			{
				190,
				7,
				4
			},
			new int[]
			{
				204,
				11,
				6
			},
			new int[]
			{
				206,
				15,
				6
			},
			new int[]
			{
				220,
				15,
				6
			},
			new int[]
			{
				200,
				2,
				2
			},
			new int[]
			{
				287,
				2,
				2
			},
			new int[]
			{
				288,
				2,
				2
			},
			new int[]
			{
				289,
				2,
				2
			},
			new int[]
			{
				290,
				7,
				4
			},
			new int[]
			{
				305,
				11,
				6
			},
			new int[]
			{
				301,
				11,
				6
			},
			new int[]
			{
				306,
				5,
				6
			},
			new int[]
			{
				300,
				2,
				2
			},
			new int[]
			{
				388,
				2,
				2
			},
			new int[]
			{
				389,
				2,
				2
			},
			new int[]
			{
				390,
				2,
				2
			},
			new int[]
			{
				391,
				7,
				4
			},
			new int[]
			{
				10007,
				11,
				4
			},
			new int[]
			{
				10001,
				13,
				4
			},
			new int[]
			{
				10004,
				11,
				4
			},
			new int[]
			{
				10000,
				3,
				3
			},
			new int[]
			{
				10088,
				3,
				3
			},
			new int[]
			{
				10089,
				3,
				3
			},
			new int[]
			{
				10090,
				7,
				6
			},
			new int[]
			{
				10091,
				7,
				4
			},
			new int[]
			{
				10107,
				15,
				5
			},
			new int[]
			{
				10101,
				15,
				4
			},
			new int[]
			{
				10104,
				11,
				5
			},
			new int[]
			{
				10100,
				3,
				3
			},
			new int[]
			{
				10188,
				4,
				4
			},
			new int[]
			{
				10189,
				3,
				3
			},
			new int[]
			{
				10190,
				3,
				3
			},
			new int[]
			{
				10191,
				7,
				4
			},
			new int[]
			{
				11006,
				13,
				7
			},
			new int[]
			{
				11001,
				6,
				6
			},
			new int[]
			{
				11004,
				7,
				7
			},
			new int[]
			{
				11007,
				17,
				7
			},
			new int[]
			{
				11000,
				3,
				2
			},
			new int[]
			{
				11088,
				2,
				1
			},
			new int[]
			{
				11089,
				3,
				2
			},
			new int[]
			{
				11090,
				6,
				3
			},
			new int[]
			{
				11091,
				9,
				2
			}
		};

		// Token: 0x04003B58 RID: 15192
		public static readonly int[] SkillList = new int[]
		{
			120,
			105,
			106,
			100,
			187,
			188,
			189,
			190,
			204,
			206,
			220,
			200,
			287,
			288,
			289,
			290,
			305,
			301,
			306,
			300,
			388,
			389,
			390,
			391
		};

		// Token: 0x04003B59 RID: 15193
		public static readonly int[] ZhanShiSkillFrameCounts = new int[]
		{
			11,
			16,
			11,
			5,
			5,
			6,
			6,
			8
		};

		// Token: 0x04003B5A RID: 15194
		public static readonly int[] FaShiSkillFrameCounts = new int[]
		{
			11,
			13,
			11,
			5,
			5,
			6,
			6,
			8
		};

		// Token: 0x04003B5B RID: 15195
		public static readonly int[] GongJianShouSkilFrameCounts = new int[]
		{
			11,
			11,
			11,
			5,
			5,
			6,
			6,
			8
		};
	}
}
