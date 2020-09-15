using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using GameServer.Core.Executor;
using Server.Tools;

namespace KF.Remoting
{
	// Token: 0x02000041 RID: 65
	public class MyServerChannelSink : IServerChannelSink, IChannelSinkBase
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060002DB RID: 731 RVA: 0x0002990C File Offset: 0x00027B0C
		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060002DC RID: 732 RVA: 0x00029920 File Offset: 0x00027B20
		public IServerChannelSink NextChannelSink
		{
			get
			{
				return this.nextSink;
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00029938 File Offset: 0x00027B38
		public MyServerChannelSink(object nextSink)
		{
			this.nextSink = (nextSink as IServerChannelSink);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0002994F File Offset: 0x00027B4F
		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessResponse(sinkStack, state, msg, headers, stream);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00029968 File Offset: 0x00027B68
		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return this.nextSink.GetResponseStream(sinkStack, state, msg, headers);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0002998C File Offset: 0x00027B8C
		ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			long startTicks = TimeUtil.NOW();
			string uri = null;
			long inputLength = 0L;
			if (requestStream != null && requestHeaders != null)
			{
				uri = requestHeaders["__RequestUri"].ToString();
			}
			ServerProcessing process = this.nextSink.ProcessMessage(sinkStack, requestMsg, requestHeaders, requestStream, out responseMsg, out responseHeaders, out responseStream);
			if (null != responseMsg)
			{
				object methodName = responseMsg.Properties["__MethodName"];
				if (uri != null && methodName != null)
				{
					string cmdName = uri.ToString() + methodName.ToString();
					CmdMonitor.RecordCmdDetail(cmdName, TimeUtil.NOW() - startTicks, inputLength, responseStream.Length, TCPProcessCmdResults.RESULT_OK);
				}
				else
				{
					LogManager.WriteExceptionUseCache(string.Format("IServerChannelSink.ProcessMessage#uri={0},methodName={1}", uri, methodName));
				}
			}
			else
			{
				LogManager.WriteExceptionUseCache(string.Format("IServerChannelSink.ProcessMessage#uri={0},responseMsg=null", uri));
			}
			return process;
		}

		// Token: 0x040001B0 RID: 432
		private IServerChannelSink nextSink;
	}
}
