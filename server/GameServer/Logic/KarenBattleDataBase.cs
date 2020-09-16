using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace GameServer.Logic
{
	
	public class KarenBattleDataBase
	{
		
		public object Mutex = new object();

		
		public Dictionary<int, KarenBattleBirthPoint> MapBirthPointDict = new Dictionary<int, KarenBattleBirthPoint>();

		
		public string RoleParamsAwardsDefaultString = "";

		
		public Dictionary<int, KarenFuBenData> FuBenItemData = new Dictionary<int, KarenFuBenData>();
	}
}
