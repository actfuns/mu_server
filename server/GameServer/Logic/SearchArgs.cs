using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class SearchArgs : IEqualityComparer<SearchArgs>
	{
		
		
		
		public int Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				lock (this)
				{
					this._Type = value;
					this.InternalCalcHash();
				}
			}
		}

		
		
		
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				lock (this)
				{
					this._ID = value;
					this.InternalCalcHash();
				}
			}
		}

		
		
		
		public int MoneyFlags
		{
			get
			{
				return this._MoneyFlags;
			}
			set
			{
				lock (this)
				{
					this._MoneyFlags = value;
					this.InternalCalcHash();
				}
			}
		}

		
		
		
		public int ColorFlags
		{
			get
			{
				return this._ColorFlags;
			}
			set
			{
				lock (this)
				{
					this._ColorFlags = value;
					this.InternalCalcHash();
				}
			}
		}

		
		
		
		public int OrderBy
		{
			get
			{
				return this._OrderBy;
			}
			set
			{
				lock (this)
				{
					this._OrderBy = value;
					this.InternalCalcHash();
				}
			}
		}

		
		
		
		public int OrderTypeFlags
		{
			get
			{
				return this._OrderTypeFlags;
			}
			set
			{
				lock (this)
				{
					this._OrderTypeFlags = value;
					this.InternalCalcHash();
				}
			}
		}

		
		private SearchArgs()
		{
			this._Type = 0;
			this._ID = 0;
			this._MoneyFlags = 0;
			this._OrderBy = 0;
			this._ColorFlags = 0;
			this._OrderTypeFlags = 0;
			this.HashCode = 0;
			this.OrderFlags = 0;
		}

		
		public SearchArgs(SearchArgs args)
		{
			this._Type = args.Type;
			this._ID = args.ID;
			this._MoneyFlags = args.MoneyFlags;
			this._OrderBy = args.OrderBy;
			this._ColorFlags = args.ColorFlags;
			this._OrderTypeFlags = args.OrderTypeFlags;
			this.OrderFlags = 0;
			this.HashCode = 0;
			this.InternalCalcHash();
		}

		
		public SearchArgs(int id, int type, int moneyFlags, int colorFlags, int orderBy, int orderTypeFlags)
		{
			this._ID = id;
			this._Type = type;
			this._MoneyFlags = moneyFlags;
			this._OrderBy = orderBy;
			this._ColorFlags = colorFlags;
			this._OrderTypeFlags = orderTypeFlags;
			this.OrderFlags = 0;
			this.HashCode = 0;
			this.InternalCalcHash();
		}

		
		public bool Equals(SearchArgs x, SearchArgs y)
		{
			return x.OrderFlags == y.OrderFlags && x.ID == y.ID && x.Type == y.Type;
		}

		
		public int GetHashCode(SearchArgs obj)
		{
			return obj.HashCode;
		}

		
		private void InternalCalcHash()
		{
			this.OrderFlags = (this.OrderBy << 30) + (this.OrderTypeFlags << 25) + (this.MoneyFlags << 22) + (this.ColorFlags << 14);
			this.HashCode = this.OrderFlags + (this.Type << 8) + this.ID;
		}

		
		public SearchArgs Clone()
		{
			return new SearchArgs(this);
		}

		
		public static SearchArgs Compare = new SearchArgs();

		
		private int HashCode;

		
		private int OrderFlags;

		
		public int _Type;

		
		public int _ID;

		
		public int _MoneyFlags;

		
		public int _ColorFlags;

		
		public int _OrderBy;

		
		public int _OrderTypeFlags;
	}
}
