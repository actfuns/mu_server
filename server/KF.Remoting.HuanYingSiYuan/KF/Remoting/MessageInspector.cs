using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GameServer.Core.Executor;

namespace KF.Remoting
{
	// Token: 0x0200008C RID: 140
	public class MessageInspector : IDispatchMessageInspector
	{
		// Token: 0x0600077C RID: 1916 RVA: 0x00064234 File Offset: 0x00062434
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

		// Token: 0x0600077D RID: 1917 RVA: 0x00064298 File Offset: 0x00062498
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			MessageInspector.cmdInfo cmd = (MessageInspector.cmdInfo)correlationState;
			if (null != cmd)
			{
				CmdMonitor.RecordCmdDetail(cmd.cmdName, TimeUtil.NOW() - cmd.receiveTicks, (long)cmd.cmdSize, TCPProcessCmdResults.RESULT_OK);
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x000642D8 File Offset: 0x000624D8
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

		// Token: 0x0200008D RID: 141
		private class cmdInfo
		{
			// Token: 0x0400040D RID: 1037
			public long receiveTicks;

			// Token: 0x0400040E RID: 1038
			public int cmdSize;

			// Token: 0x0400040F RID: 1039
			public string cmdName;
		}
	}
}
