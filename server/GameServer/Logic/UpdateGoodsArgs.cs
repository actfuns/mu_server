using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020004EE RID: 1262
	[ProtoContract]
	public class UpdateGoodsArgs
	{
		// Token: 0x06001776 RID: 6006 RVA: 0x0016FD10 File Offset: 0x0016DF10
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
						}
					}
					else
					{
						gd.Binding = this.Binding;
					}
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06001777 RID: 6007 RVA: 0x0016FDE4 File Offset: 0x0016DFE4
		// (set) Token: 0x06001778 RID: 6008 RVA: 0x0016FDFC File Offset: 0x0016DFFC
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x0016FE64 File Offset: 0x0016E064
		// (set) Token: 0x0600177A RID: 6010 RVA: 0x0016FE7C File Offset: 0x0016E07C
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

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x0016FEE4 File Offset: 0x0016E0E4
		// (set) Token: 0x0600177C RID: 6012 RVA: 0x0016FEFC File Offset: 0x0016E0FC
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

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600177D RID: 6013 RVA: 0x0016FF64 File Offset: 0x0016E164
		// (set) Token: 0x0600177E RID: 6014 RVA: 0x0016FF7C File Offset: 0x0016E17C
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

		// Token: 0x04002175 RID: 8565
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04002176 RID: 8566
		[ProtoMember(2)]
		public int DbID;

		// Token: 0x04002177 RID: 8567
		[ProtoMember(3)]
		public List<UpdatePropIndexes> ChangedIndexes = new List<UpdatePropIndexes>();

		// Token: 0x04002178 RID: 8568
		[ProtoMember(4)]
		private List<int> _WashProps;

		// Token: 0x04002179 RID: 8569
		[ProtoMember(5)]
		private int _Binding;

		// Token: 0x0400217A RID: 8570
		[ProtoMember(6)]
		private List<int> _ElementhrtsProps;

		// Token: 0x0400217B RID: 8571
		[ProtoMember(7)]
		private int _JuHunProps;
	}
}
