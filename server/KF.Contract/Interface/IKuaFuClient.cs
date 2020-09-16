using System;
using KF.Contract.Data;

namespace KF.Contract.Interface
{
	
	public interface IKuaFuClient
	{
		
		void EventCallBackHandler(int eventType, params object[] args);

		
		object GetDataFromClientServer(int dataType, params object[] args);

		
		int GetNewFuBenSeqId();

		
		int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0);

		
		int OnRoleChangeState(int roleId, int state, int age);
	}
}
