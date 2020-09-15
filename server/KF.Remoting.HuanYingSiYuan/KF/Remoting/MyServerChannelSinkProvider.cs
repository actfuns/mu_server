using System;
using System.Collections;
using System.Runtime.Remoting.Channels;

namespace KF.Remoting
{
	// Token: 0x02000043 RID: 67
	public class MyServerChannelSinkProvider : IServerChannelSinkProvider
	{
		// Token: 0x060002E8 RID: 744 RVA: 0x00029B19 File Offset: 0x00027D19
		public MyServerChannelSinkProvider(IDictionary properties, ICollection providerData)
		{
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00029B24 File Offset: 0x00027D24
		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink nextServerSink = null;
			if (this.NextSink != null)
			{
				nextServerSink = (this.NextSink as IServerChannelSinkProvider).CreateSink(channel);
			}
			return new MyServerChannelSink(nextServerSink);
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00029B5D File Offset: 0x00027D5D
		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060002EB RID: 747 RVA: 0x00029B60 File Offset: 0x00027D60
		// (set) Token: 0x060002EC RID: 748 RVA: 0x00029B7D File Offset: 0x00027D7D
		public IServerChannelSinkProvider Next
		{
			get
			{
				return this.NextSink as IServerChannelSinkProvider;
			}
			set
			{
				this.NextSink = value;
			}
		}

		// Token: 0x040001B2 RID: 434
		private object NextSink;
	}
}
