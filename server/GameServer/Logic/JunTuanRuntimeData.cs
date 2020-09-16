using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class JunTuanRuntimeData
	{
		
		public object Mutex = new object();

		
		public int LegionsNeed;

		
		public int LegionsCastZuanShi;

		
		public int LegionsCreateCD;

		
		public int LegionsJionCD;

		
		public int LegionsEliteNum;

		
		public int[] LegionProsperityCost;

		
		public TemplateLoader<Dictionary<int, JunTuanRolePermissionInfo>> RolePermissionDict = new TemplateLoader<Dictionary<int, JunTuanRolePermissionInfo>>();

		
		public TemplateLoader<Dictionary<int, JunTuanTaskInfo>> TaskList = new TemplateLoader<Dictionary<int, JunTuanTaskInfo>>();

		
		public List<TimeSpan> TaskStartEndTimeList = new List<TimeSpan>();

		
		public HashSet<int> KillMonsterIds = new HashSet<int>();

		
		public Dictionary<int, int> Task2IdxDict = new Dictionary<int, int>();

		
		public int TaskCount = 0;

		
		public Dictionary<int, JunTuanBaseData> JunTuanBaseDict = new Dictionary<int, JunTuanBaseData>();

		
		public Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		
		public Queue<Tuple<int, int, int, int, long>> JunTuanTaskQueue = new Queue<Tuple<int, int, int, int, long>>();

		
		public List<KFChat> JunTuanChatList = new List<KFChat>();

		
		public HashSet<int> HasUpdateRoleDataHashSet = new HashSet<int>();

		
		public long NextUpdateTicks;
	}
}
