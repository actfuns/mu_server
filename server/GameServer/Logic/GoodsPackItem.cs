using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	
	public class GoodsPackItem : IObject
	{
		
		
		
		public int AutoID { get; set; }

		
		
		
		public int GoodsPackID { get; set; }

		
		
		
		public int OwnerRoleID { get; set; }

		
		
		
		public string OwnerRoleName { get; set; }

		
		
		
		public int GoodsPackType { get; set; }

		
		
		
		public long ProduceTicks { get; set; }

		
		
		
		public int LockedRoleID { get; set; }

		
		
		public Dictionary<int, bool> GoodsIDDict
		{
			get
			{
				return this._GoodsIDDict;
			}
		}

		
		
		public Dictionary<int, int> GoodsIDToRolesDict
		{
			get
			{
				return this._GoodsIDToRolesDict;
			}
		}

		
		
		
		public long OpenPackTicks { get; set; }

		
		
		public Dictionary<int, long> RolesTicksDict
		{
			get
			{
				return this._RolesTicksDict;
			}
		}

		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_GOODSPACK;
			}
		}

		
		
		
		public long AutoOpenPackTicks { get; set; }

		
		
		
		public int OnlyID { get; set; }

		
		public int GetObjectID()
		{
			return this.AutoID;
		}

		
		
		
		public long LastLifeMagicTick { get; set; }

		
		
		
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				return new Point((double)((int)(this.FallPoint.X / (double)gameMap.MapGridWidth)), (double)((int)(this.FallPoint.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.FallPoint = new Point((double)((int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2))), (double)((int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2))));
			}
		}

		
		
		
		public Point CurrentPos
		{
			get
			{
				return this.FallPoint;
			}
			set
			{
				this.FallPoint = value;
			}
		}

		
		
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		
		
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		
		
		
		public Dircetions CurrentDir { get; set; }

		
		
		
		public List<int> PassiveEffectList { get; set; }

		
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		
		public List<int> TeamRoleIDs = null;

		
		public List<long> TeamRoleDamages;

		
		public int TeamID = -1;

		
		private Dictionary<int, bool> _GoodsIDDict = new Dictionary<int, bool>();

		
		private Dictionary<int, int> _GoodsIDToRolesDict = new Dictionary<int, int>();

		
		private Dictionary<int, long> _RolesTicksDict = new Dictionary<int, long>();

		
		public List<GoodsData> GoodsDataList = null;

		
		public int MapCode = -1;

		
		public Point FallPoint;

		
		public int CopyMapID = -1;

		
		public string KilledMonsterName = "";

		
		public int BelongTo = -1;

		
		public int FallLevel = 0;

		
		public int PickRoleID = -1;

		
		public bool CanPickUp = true;
	}
}
