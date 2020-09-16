using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;

namespace GameServer.Logic
{
	
	public class NPC : IObject
	{
		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_NPC;
			}
		}

		
		public int GetObjectID()
		{
			return this.NpcID;
		}

		
		
		
		public long LastLifeMagicTick { get; set; }

		
		
		
		public Point CurrentGrid
		{
			get
			{
				return this.GridPoint;
			}
			set
			{
				this.GridPoint = value;
			}
		}

		
		
		
		public Point CurrentPos
		{
			get
			{
				return this._CurrentPos;
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.GridPoint = new Point((double)((int)(value.X / (double)gameMap.MapGridWidth)), (double)((int)(value.Y / (double)gameMap.MapGridHeight)));
				this._CurrentPos = value;
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

		
		public int NpcID;

		
		public int MapCode = -1;

		
		public Point GridPoint;

		
		public int CopyMapID = -1;

		
		public byte[] RoleBufferData = null;

		
		private Point _CurrentPos = new Point(0.0, 0.0);

		
		public bool ShowNpc = true;
	}
}
