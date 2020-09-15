using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace KF.Remoting
{
	// Token: 0x0200008E RID: 142
	public class ServiceCompressBehavior : BehaviorExtensionElement, IServiceBehavior
	{
		// Token: 0x06000781 RID: 1921 RVA: 0x00064331 File Offset: 0x00062531
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00064334 File Offset: 0x00062534
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher chDisp = (ChannelDispatcher)channelDispatcherBase;
				foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
				{
					epDisp.DispatchRuntime.MessageInspectors.Add(new MessageInspector());
				}
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x000643EC File Offset: 0x000625EC
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x000643F0 File Offset: 0x000625F0
		public override Type BehaviorType
		{
			get
			{
				return typeof(ServiceCompressBehavior);
			}
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0006440C File Offset: 0x0006260C
		protected override object CreateBehavior()
		{
			return new ServiceCompressBehavior();
		}
	}
}
