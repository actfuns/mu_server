using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace KF.Remoting
{
	
	public class MyClientChannelSink : IClientChannelSink, IChannelSinkBase
	{
		
		
		public IDictionary Properties
		{
			get
			{
				return null;
			}
		}

		
		
		public IClientChannelSink NextChannelSink
		{
			get
			{
				return this.nextSink;
			}
		}

		
		public MyClientChannelSink(object nextSink)
		{
			this.nextSink = (nextSink as IClientChannelSink);
		}

		
		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
		}

		
		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			this.nextSink.AsyncProcessResponse(sinkStack, state, headers, stream);
		}

		
		public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
		{
			return this.nextSink.GetRequestStream(msg, headers);
		}

		
		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			this.nextSink.ProcessMessage(msg, requestHeaders, requestStream, out responseHeaders, out responseStream);
		}

		
		private IClientChannelSink nextSink;
	}
}
