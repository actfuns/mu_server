using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using GameServer.Core.Executor;
using Server.Tools;

namespace KF.Remoting
{
	
	public class MyServerChannelSink : IServerChannelSink, IChannelSinkBase
	{
		
		
		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		
		
		public IServerChannelSink NextChannelSink
		{
			get
			{
				return this.nextSink;
			}
		}

		
		public MyServerChannelSink(object nextSink)
		{
			this.nextSink = (nextSink as IServerChannelSink);
		}

		
		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessResponse(sinkStack, state, msg, headers, stream);
		}

		
		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			return this.nextSink.GetResponseStream(sinkStack, state, msg, headers);
		}

		
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

		
		private IServerChannelSink nextSink;
	}
}
