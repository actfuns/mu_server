using System;

namespace GameServer.Logic
{
	
	public enum RegressActiveOpcode
	{
		
		RegressActiveSucc = 1,
		
		RegressActiveOpenErr,
		
		RegressActiveNotIn,
		
		RegressActiveGetRegTime,
		
		RegressActiveGetFile,
		
		RegressActiveUserInfoErr,
		
		RegressActiveSignCheckFail,
		
		RegressActiveSignSelectFail,
		
		RegressActiveSignConfErr,
		
		RegressActiveSignDayErr,
		
		RegressActiveSignGetAwardFail,
		
		RegressActiveSignGiveAwardFail,
		
		RegressActiveSignRebornBagFail,
		
		RegressActiveSignBaseBagFail,
		
		RegressActiveSignRecordFail,
		
		RegressActiveSignGetInfoFail,
		
		RegressActiveSignNotInfo,
		
		RegressActiveSignCheckTypeErr,
		
		RegressActiveSignCaleDayErr,
		
		RegressActiveSignHas,
		
		RegressActiveSignNotDay,
		
		RegressActiveStoreConfErr,
		
		RegressActiveStoreCheckFail,
		
		RegressActiveStoreBuyFail,
		
		RegressActiveStoreCheckDayFail,
		
		RegressActiveStoreCheckGoodFail,
		
		RegressActiveStoreCheckParmErr,
		
		RegressActiveStoreUserYuanBaoFail,
		
		RegressActiveStoreInsertInfoErr,
		
		RegressActiveInputGetInfoErr,
		
		RegressActiveInputHas,
		
		RegressActiveInputConfErr,
		
		RegressActiveInputCheckAwardErr,
		
		RegressActiveInputGiveAwardErr,
		
		RegressActiveBuyGetInfoErr,
		
		RegressActiveGetSignInfoErr,
		
		RegressActiveUpdateInputInfoErr
	}
}
