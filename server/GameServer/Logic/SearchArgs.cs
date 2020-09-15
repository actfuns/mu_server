using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200078A RID: 1930
	public class SearchArgs : IEqualityComparer<SearchArgs>
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06003233 RID: 12851 RVA: 0x002CB548 File Offset: 0x002C9748
		// (set) Token: 0x06003234 RID: 12852 RVA: 0x002CB560 File Offset: 0x002C9760
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

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06003235 RID: 12853 RVA: 0x002CB5B0 File Offset: 0x002C97B0
		// (set) Token: 0x06003236 RID: 12854 RVA: 0x002CB5C8 File Offset: 0x002C97C8
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

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06003237 RID: 12855 RVA: 0x002CB618 File Offset: 0x002C9818
		// (set) Token: 0x06003238 RID: 12856 RVA: 0x002CB630 File Offset: 0x002C9830
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

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06003239 RID: 12857 RVA: 0x002CB680 File Offset: 0x002C9880
		// (set) Token: 0x0600323A RID: 12858 RVA: 0x002CB698 File Offset: 0x002C9898
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

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x0600323B RID: 12859 RVA: 0x002CB6E8 File Offset: 0x002C98E8
		// (set) Token: 0x0600323C RID: 12860 RVA: 0x002CB700 File Offset: 0x002C9900
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

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x0600323D RID: 12861 RVA: 0x002CB750 File Offset: 0x002C9950
		// (set) Token: 0x0600323E RID: 12862 RVA: 0x002CB768 File Offset: 0x002C9968
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

		// Token: 0x0600323F RID: 12863 RVA: 0x002CB7B8 File Offset: 0x002C99B8
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

		// Token: 0x06003240 RID: 12864 RVA: 0x002CB808 File Offset: 0x002C9A08
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

		// Token: 0x06003241 RID: 12865 RVA: 0x002CB87C File Offset: 0x002C9A7C
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

		// Token: 0x06003242 RID: 12866 RVA: 0x002CB8D4 File Offset: 0x002C9AD4
		public bool Equals(SearchArgs x, SearchArgs y)
		{
			return x.OrderFlags == y.OrderFlags && x.ID == y.ID && x.Type == y.Type;
		}

		// Token: 0x06003243 RID: 12867 RVA: 0x002CB920 File Offset: 0x002C9B20
		public int GetHashCode(SearchArgs obj)
		{
			return obj.HashCode;
		}

		// Token: 0x06003244 RID: 12868 RVA: 0x002CB938 File Offset: 0x002C9B38
		private void InternalCalcHash()
		{
			this.OrderFlags = (this.OrderBy << 30) + (this.OrderTypeFlags << 25) + (this.MoneyFlags << 22) + (this.ColorFlags << 14);
			this.HashCode = this.OrderFlags + (this.Type << 8) + this.ID;
		}

		// Token: 0x06003245 RID: 12869 RVA: 0x002CB990 File Offset: 0x002C9B90
		public SearchArgs Clone()
		{
			return new SearchArgs(this);
		}

		// Token: 0x04003E6D RID: 15981
		public static SearchArgs Compare = new SearchArgs();

		// Token: 0x04003E6E RID: 15982
		private int HashCode;

		// Token: 0x04003E6F RID: 15983
		private int OrderFlags;

		// Token: 0x04003E70 RID: 15984
		public int _Type;

		// Token: 0x04003E71 RID: 15985
		public int _ID;

		// Token: 0x04003E72 RID: 15986
		public int _MoneyFlags;

		// Token: 0x04003E73 RID: 15987
		public int _ColorFlags;

		// Token: 0x04003E74 RID: 15988
		public int _OrderBy;

		// Token: 0x04003E75 RID: 15989
		public int _OrderTypeFlags;
	}
}
