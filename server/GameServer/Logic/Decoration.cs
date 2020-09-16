using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;

namespace GameServer.Logic
{
	
	public class Decoration : IObject
	{
		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_DECO;
			}
		}

		
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
				return new Point((double)((int)(this.Pos.X / (double)gameMap.MapGridWidth)), (double)((int)(this.Pos.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.Pos = new Point((double)((int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2))), (double)((int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2))));
			}
		}

		
		
		
		public Point CurrentPos
		{
			get
			{
				return this.Pos;
			}
			set
			{
				this.Pos = value;
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

		
		public int AutoID;

		
		public int DecoID;

		
		public int MapCode = -1;

		
		public Point Pos;

		
		public int CopyMapID = -1;

		
		public long StartTicks = 0L;

		
		public int MaxLiveTicks = 0;

		
		public int AlphaTicks = 0;
	}
}
