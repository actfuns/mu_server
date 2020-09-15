using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace KF.Remoting
{
	// Token: 0x02000042 RID: 66
	public class MyClientChannelSink : IClientChannelSink, IChannelSinkBase
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x00029A78 File Offset: 0x00027C78
		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x00029A8C File Offset: 0x00027C8C
		public IClientChannelSink NextChannelSink
		{
			get
			{
				return this.nextSink;
			}
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00029AA4 File Offset: 0x00027CA4
		public MyClientChannelSink(object nextSink)
		{
			this.nextSink = (nextSink as IClientChannelSink);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00029ABB File Offset: 0x00027CBB
		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00029ACF File Offset: 0x00027CCF
		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessResponse(sinkStack, state, headers, stream);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00029AE4 File Offset: 0x00027CE4
		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return this.nextSink.GetRequestStream(msg, headers);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00029B03 File Offset: 0x00027D03
		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			this.nextSink.ProcessMessage(msg, requestHeaders, requestStream, out responseHeaders, out responseStream);
		}

		// Token: 0x040001B1 RID: 433
		private IClientChannelSink nextSink;
	}
}
