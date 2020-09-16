using System;
using System.Collections.Generic;

namespace GameServer.Logic.JingJiChang
{
	
	public class JingJiChangConstants
	{
		
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

		
		public static readonly int RobotBothX = 4684;

		
		public static readonly int RobotBothY = 4684;

		
		public static readonly int CanChallengeNum = 3;

		
		public static readonly int RankingListMaxNum = 5000;

		
		public static readonly int Enter_Type_Free = 0;

		
		public static readonly int Enter_Type_Vip = 1;

		
		public static readonly long Challenge_CD_Time = 180000L;

		
		public static readonly long RankingReward_CD_Time = 86400000L;

		
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

		
		public static readonly int[] ZhanShiFiveCombotSkillList = new int[]
		{
			100,
			187,
			188,
			189,
			190
		};

		
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

		
		public static readonly int[] FaShiFiveCombotSkillList = new int[]
		{
			200,
			287,
			288,
			289,
			290
		};

		
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

		
		public static readonly int[] GongJianShouFiveCombotSkillList = new int[]
		{
			300,
			388,
			389,
			390,
			391
		};

		
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

		
		public static readonly int[] StrMagicSwordFiveCombotSkillList = new int[]
		{
			10000,
			10088,
			10089,
			10090,
			10091
		};

		
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

		
		public static readonly int[] IntMagicSwordFiveCombotSkillList = new int[]
		{
			10100,
			10188,
			10189,
			10190,
			10191
		};

		
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

		
		public static readonly int[] SummonerFiveCombotSkillList = new int[]
		{
			11000,
			11088,
			11089,
			11090,
			11091
		};

		
		public static readonly int SummonerSkillFirst = 11007;

		
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
