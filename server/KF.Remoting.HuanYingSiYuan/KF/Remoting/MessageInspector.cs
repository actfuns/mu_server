using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GameServer.Core.Executor;

namespace KF.Remoting
{
	
	public class MessageInspector : IDispatchMessageInspector
	{
		
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			string action = request.Headers.Action;
			object result;
			if (action.Length > 20)
			{
				MessageInspector.cmdInfo cmd = new MessageInspector.cmdInfo
				{
					cmdName = action.Substring(19),
					cmdSize = 0,
					receiveTicks = TimeUtil.NOW()
				};
				result = cmd;
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			MessageInspector.cmdInfo cmd = (MessageInspector.cmdInfo)correlationState;
			if (null != cmd)
			{
				CmdMonitor.RecordCmdDetail(cmd.cmdName, TimeUtil.NOW() - cmd.receiveTicks, (long)cmd.cmdSize, TCPProcessCmdResults.RESULT_OK);
			}
		}

		
		public static string GetHeader(string headerName)
		{
			string result;
			if (OperationContext.Current.IncomingMessageHeaders.FindHeader(headerName, "http://tempuri.org") >= 0)
			{
				result = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(headerName, "http://tempuri.org");
			}
			else
			{
				result = null;
			}
            return ((OperationContext.Current.IncomingMessageHeaders.FindHeader(headerName, "http://tempuri.org") < 0) ? null : OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(headerName, "http://tempuri.org"));

		}

		
		private class cmdInfo
		{
			
			public long receiveTicks;

			
			public int cmdSize;

			
			public string cmdName;
		}
	}
}
