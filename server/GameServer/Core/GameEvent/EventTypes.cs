using System;

namespace GameServer.Core.GameEvent
{
	
	public enum EventTypes
	{
		
		ZhanMengShiJian,
		
		XueSeChengBao,
		
		EMoGuangChang,
		
		HuangJinBuDui,
		
		YeWaiBoss,
		
		PKKingHuoDong,
		
		TanWeiGouMai,
		
		MingXiang,
		
		ZhaoHuanUserID,
		
		PlayerLevelup,
		
		PlayerDead,
		
		MonsterDead,
		
		PlayerLogout,
		
		PlayerLeaveFuBen,
		
		PlayerInitGame,
		
		PlayerInitGameAsync,
		
		MonsterBirthOn,
		
		MonsterInjured,
		
		MonsterBlooadChanged,
		
		MonsterAttacked,
		
		MonsterLivingTime,
		
		PreGotoLastMap,
		
		PreInstallJunQi,
		
		PreBangHuiAddMember,
		
		PreBangHuiRemoveMember,
		
		PreBangHuiChangeZhiWu,
		
		PostBangHuiChange,
		
		ProcessClickOnNpc,
		
		StartPlayGame,
		
		OnClientChangeMap,
		
		OnCreateMonster,
		
		ClientRegionEvent,
		
		SevenDayGoal,
		
		PreMonsterInjure,
		
		AfterMonsterInjure,
		
		MonsterToMonsterDead,
		
		OnClientChargeItem,
		
		OrnamentGoal,
		
		PlayerOnline,
		
		GameRunning,
		
		BeforeProcessMsg,
		
		PlayerEnterGame,
		
		PlayerInitGameBeBan,
		
		PlayerCreateRoleBeBan,
		
		PlayerCreateRoleLimit,
		
		LoginSuccess,
		
		LoginFailByDataCheck,
		
		LoginFailByUserBan,
		
		LoginFailByTimeout,
		
		SocketConnect,
		
		SocketClose,
		
		UserLogin,
		
		UserCreateRole,
		
		JingJiChangWin,
		
		JingJiChangFailed,
		
		PlayerLogoutFinish,
		
		RoleKillMonster,
		
		OneSecsTimerEvent,
		
		MainTaskProgressEvent,
		
		OnClientMapChanged,
		
		OnClientGetSceneInfo,
		
		OnKuaFuLogin,
		
		OnKuaFuInitGame,
		
		PreZhanDuiChangeMember,
		
		PostMemberZhanDuiChange,
		
		LeaveMap,
		
		SimpleEventTypeCount,
		
		Max = 10000
	}
}
