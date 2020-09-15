using System;
using System.Runtime.Remoting.Channels;

namespace KF.Remoting
{
	// Token: 0x02000044 RID: 68
	public class MyClientChannelSinkProvider : IClientChannelSinkProvider
	{
		// Token: 0x060002ED RID: 749 RVA: 0x00029B87 File Offset: 0x00027D87
		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00029B8C File Offset: 0x00027D8C
		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
		{
			IClientChannelSink nextClientSink = null;
			if (this.NextSink != null)
			{
				nextClientSink = (this.NextSink as IClientChannelSinkProvider).CreateSink(channel, url, remoteChannelData);
				if (nextClientSink == null)
				{
					return null;
				}
			}
			return new MyClientChannelSink(nextClientSink);
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060002EF RID: 751 RVA: 0x00029BDC File Offset: 0x00027DDC
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x00029BF9 File Offset: 0x00027DF9
		IClientChannelSinkProvider IClientChannelSinkProvider.Next
		{
			get
			{
				return this.NextSink as IClientChannelSinkProvider;
			}
			set
			{
				this.NextSink = value;
			}
		}

		// Token: 0x040001B3 RID: 435
		private object NextSink;
	}
}
