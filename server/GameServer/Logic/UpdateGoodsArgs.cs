using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class UpdateGoodsArgs
	{
		
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

		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public int DbID;

		
		[ProtoMember(3)]
		public List<UpdatePropIndexes> ChangedIndexes = new List<UpdatePropIndexes>();

		
		[ProtoMember(4)]
		private List<int> _WashProps;

		
		[ProtoMember(5)]
		private int _Binding;

		
		[ProtoMember(6)]
		private List<int> _ElementhrtsProps;

		
		[ProtoMember(7)]
		private int _JuHunProps;
	}
}
