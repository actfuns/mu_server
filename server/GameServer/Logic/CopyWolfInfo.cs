using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace GameServer.Logic
{
	
	public class CopyWolfInfo
	{
		
		public CopyWolfWaveInfo GetWaveConfig(int wave)
		{
			CopyWolfWaveInfo result;
			if (this.CopyWolfWaveDic.ContainsKey(wave))
			{
				result = this.CopyWolfWaveDic[wave];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public int GetMonsterHurt(int monsterID)
		{
			int result;
			if (this.MonsterHurtDic.ContainsKey(monsterID))
			{
				result = this.MonsterHurtDic[monsterID];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		
		public int TotalSecs
		{
			get
			{
				return this.PrepareSecs + this.FightingSecs + this.ClearRolesSecs;
			}
		}

		
		public int MapID = 70200;

		
		public int CopyID = 70200;

		
		public int CampID = 1;

		
		public int ScoreRateTime = 0;

		
		public int ScoreRateLife = 0;

		
		public int FortMonsterID = 0;

		
		public Point FortSite = default(Point);

		
		public Dictionary<int, CopyWolfWaveInfo> CopyWolfWaveDic = new Dictionary<int, CopyWolfWaveInfo>();

		
		public Dictionary<int, int> MonsterHurtDic = new Dictionary<int, int>();

		
		public ConcurrentDictionary<int, CopyWolfSceneInfo> SceneDict = new ConcurrentDictionary<int, CopyWolfSceneInfo>();

		
		public int PrepareSecs = 1;

		
		public int FightingSecs = 900;

		
		public int ClearRolesSecs = 15;
	}
}
