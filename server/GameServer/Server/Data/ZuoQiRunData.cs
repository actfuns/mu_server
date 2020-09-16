using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class ZuoQiRunData
	{
		
		public object Mutex = new object();

		
		public HashSet<int> HorseNotice = new HashSet<int>();

		
		public List<MountRandomItem> MountFreeRandomList = new List<MountRandomItem>();

		
		public List<MountRandomItem> MountRandomList = new List<MountRandomItem>();

		
		public List<MountRandomItem> MountPayRandomList = new List<MountRandomItem>();

		
		public List<MountRandomItem> MountFreeRandomListTeQuan = new List<MountRandomItem>();

		
		public List<MountRandomItem> MountRandomListTeQuan = new List<MountRandomItem>();

		
		public List<MountRandomItem> MountPayRandomListTeQuan = new List<MountRandomItem>();

		
		public Dictionary<int, SuperiorDropItem> SuperiorDropDict = new Dictionary<int, SuperiorDropItem>();

		
		public Dictionary<int, SuperiorTypeItem> SuperiorTypeDict = new Dictionary<int, SuperiorTypeItem>();

		
		public Dictionary<int, PokedexItem> PokedexDict = new Dictionary<int, PokedexItem>();

		
		public Dictionary<int, Dictionary<int, AdvancedItem>> AdvancedDict = new Dictionary<int, Dictionary<int, AdvancedItem>>();

		
		public Dictionary<int, LevelUpItem> LevelUpDict = new Dictionary<int, LevelUpItem>();

		
		public Dictionary<int, List<ArrayAdditionItem>> ArrayAdditiionDict = new Dictionary<int, List<ArrayAdditionItem>>();

		
		public List<SuitItem> SuitList = new List<SuitItem>();

		
		public List<HorseEquipAdditionItem> HorseEquipAdditionItemList = new List<HorseEquipAdditionItem>();
	}
}
