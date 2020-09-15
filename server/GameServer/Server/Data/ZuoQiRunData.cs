using System;
using System.Collections.Generic;

namespace Server.Data
{
	// Token: 0x0200085C RID: 2140
	public class ZuoQiRunData
	{
		// Token: 0x040046BF RID: 18111
		public object Mutex = new object();

		// Token: 0x040046C0 RID: 18112
		public HashSet<int> HorseNotice = new HashSet<int>();

		// Token: 0x040046C1 RID: 18113
		public List<MountRandomItem> MountFreeRandomList = new List<MountRandomItem>();

		// Token: 0x040046C2 RID: 18114
		public List<MountRandomItem> MountRandomList = new List<MountRandomItem>();

		// Token: 0x040046C3 RID: 18115
		public List<MountRandomItem> MountPayRandomList = new List<MountRandomItem>();

		// Token: 0x040046C4 RID: 18116
		public List<MountRandomItem> MountFreeRandomListTeQuan = new List<MountRandomItem>();

		// Token: 0x040046C5 RID: 18117
		public List<MountRandomItem> MountRandomListTeQuan = new List<MountRandomItem>();

		// Token: 0x040046C6 RID: 18118
		public List<MountRandomItem> MountPayRandomListTeQuan = new List<MountRandomItem>();

		// Token: 0x040046C7 RID: 18119
		public Dictionary<int, SuperiorDropItem> SuperiorDropDict = new Dictionary<int, SuperiorDropItem>();

		// Token: 0x040046C8 RID: 18120
		public Dictionary<int, SuperiorTypeItem> SuperiorTypeDict = new Dictionary<int, SuperiorTypeItem>();

		// Token: 0x040046C9 RID: 18121
		public Dictionary<int, PokedexItem> PokedexDict = new Dictionary<int, PokedexItem>();

		// Token: 0x040046CA RID: 18122
		public Dictionary<int, Dictionary<int, AdvancedItem>> AdvancedDict = new Dictionary<int, Dictionary<int, AdvancedItem>>();

		// Token: 0x040046CB RID: 18123
		public Dictionary<int, LevelUpItem> LevelUpDict = new Dictionary<int, LevelUpItem>();

		// Token: 0x040046CC RID: 18124
		public Dictionary<int, List<ArrayAdditionItem>> ArrayAdditiionDict = new Dictionary<int, List<ArrayAdditionItem>>();

		// Token: 0x040046CD RID: 18125
		public List<SuitItem> SuitList = new List<SuitItem>();

		// Token: 0x040046CE RID: 18126
		public List<HorseEquipAdditionItem> HorseEquipAdditionItemList = new List<HorseEquipAdditionItem>();
	}
}
