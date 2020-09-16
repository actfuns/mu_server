using System;
using System.Collections.Generic;
using GameServer.Logic;
using KF.Contract.Data;

namespace Server.Data
{
	
	public class LingDiCaiJiRunData
	{
		
		public object Mutex = new object();

		
		public bool KuaFuSyncNeed = true;

		
		public List<LingDiData> LingDiDataList = new List<LingDiData>();

		
		public Dictionary<int, SortedList<long, List<object>>> NormalShuiJingQueue = new Dictionary<int, SortedList<long, List<object>>>();

		
		public Dictionary<int, SortedList<long, List<object>>> ChaoShuiJingQueue = new Dictionary<int, SortedList<long, List<object>>>();

		
		public Dictionary<int, List<LingDiShouWeiMonsterItem>> ShouWeiQueue = new Dictionary<int, List<LingDiShouWeiMonsterItem>>();

		
		public List<RoleData4Selector> LingZhuRoleDataList = new List<RoleData4Selector>(2);

		
		public Dictionary<int, Dictionary<int, Monster>> ShouWeiMonster = new Dictionary<int, Dictionary<int, Monster>>();

		
		public List<bool> DoubleOpenState = new List<bool>();
	}
}
