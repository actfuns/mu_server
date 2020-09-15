using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000C8 RID: 200
	[ProtoContract]
	public class UpdateGoodsArgs
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x00009060 File Offset: 0x00007260
		public void CopyPropsTo(GoodsData gd)
		{
			lock (this.ChangedIndexes)
			{
				foreach (UpdatePropIndexes idx in this.ChangedIndexes)
				{
					UpdatePropIndexes updatePropIndexes = idx;
					if (updatePropIndexes != UpdatePropIndexes.binding)
					{
						switch (updatePropIndexes)
						{
						case UpdatePropIndexes.WashProps:
							gd.WashProps = this.WashProps;
							break;
						case UpdatePropIndexes.ElementhrtsProps:
							gd.ElementhrtsProps = this.ElementhrtsProps;
							break;
						case UpdatePropIndexes.JuHun:
							gd.JuHunID = this.JuHunProps;
							break;
						}
					}
					else
					{
						gd.Binding = this.Binding;
					}
				}
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x00009144 File Offset: 0x00007344
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000915C File Offset: 0x0000735C
		public List<int> WashProps
		{
			get
			{
				return this._WashProps;
			}
			set
			{
				lock (this)
				{
					if (!this.ChangedIndexes.Contains(UpdatePropIndexes.WashProps))
					{
						this.ChangedIndexes.Add(UpdatePropIndexes.WashProps);
					}
					this._WashProps = value;
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x000091C4 File Offset: 0x000073C4
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x000091DC File Offset: 0x000073DC
		public int Binding
		{
			get
			{
				return this._Binding;
			}
			set
			{
				lock (this)
				{
					if (!this.ChangedIndexes.Contains(UpdatePropIndexes.binding))
					{
						this.ChangedIndexes.Add(UpdatePropIndexes.binding);
					}
					this._Binding = value;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00009244 File Offset: 0x00007444
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000925C File Offset: 0x0000745C
		public List<int> ElementhrtsProps
		{
			get
			{
				return this._ElementhrtsProps;
			}
			set
			{
				lock (this)
				{
					if (!this.ChangedIndexes.Contains(UpdatePropIndexes.ElementhrtsProps))
					{
						this.ChangedIndexes.Add(UpdatePropIndexes.ElementhrtsProps);
					}
					this._ElementhrtsProps = value;
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x000092C4 File Offset: 0x000074C4
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x000092DC File Offset: 0x000074DC
		public int JuHunProps
		{
			get
			{
				return this._JuHunProps;
			}
			set
			{
				lock (this)
				{
					if (!this.ChangedIndexes.Contains(UpdatePropIndexes.JuHun))
					{
						this.ChangedIndexes.Add(UpdatePropIndexes.JuHun);
					}
					this._JuHunProps = value;
				}
			}
		}

		// Token: 0x04000572 RID: 1394
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04000573 RID: 1395
		[ProtoMember(2)]
		public int DbID;

		// Token: 0x04000574 RID: 1396
		[ProtoMember(3)]
		public List<UpdatePropIndexes> ChangedIndexes = new List<UpdatePropIndexes>();

		// Token: 0x04000575 RID: 1397
		[ProtoMember(4)]
		private List<int> _WashProps;

		// Token: 0x04000576 RID: 1398
		[ProtoMember(5)]
		private int _Binding;

		// Token: 0x04000577 RID: 1399
		[ProtoMember(6)]
		private List<int> _ElementhrtsProps;

		// Token: 0x04000578 RID: 1400
		[ProtoMember(7)]
		private int _JuHunProps;
	}
}
