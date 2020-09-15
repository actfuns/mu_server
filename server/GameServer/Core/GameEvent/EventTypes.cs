using System;

namespace GameServer.Core.GameEvent
{
	// Token: 0x02000102 RID: 258
	public enum EventTypes
	{
		// Token: 0x04000545 RID: 1349
		ZhanMengShiJian,
		// Token: 0x04000546 RID: 1350
		XueSeChengBao,
		// Token: 0x04000547 RID: 1351
		EMoGuangChang,
		// Token: 0x04000548 RID: 1352
		HuangJinBuDui,
		// Token: 0x04000549 RID: 1353
		YeWaiBoss,
		// Token: 0x0400054A RID: 1354
		PKKingHuoDong,
		// Token: 0x0400054B RID: 1355
		TanWeiGouMai,
		// Token: 0x0400054C RID: 1356
		MingXiang,
		// Token: 0x0400054D RID: 1357
		ZhaoHuanUserID,
		// Token: 0x0400054E RID: 1358
		PlayerLevelup,
		// Token: 0x0400054F RID: 1359
		PlayerDead,
		// Token: 0x04000550 RID: 1360
		MonsterDead,
		// Token: 0x04000551 RID: 1361
		PlayerLogout,
		// Token: 0x04000552 RID: 1362
		PlayerLeaveFuBen,
		// Token: 0x04000553 RID: 1363
		PlayerInitGame,
		// Token: 0x04000554 RID: 1364
		PlayerInitGameAsync,
		// Token: 0x04000555 RID: 1365
		MonsterBirthOn,
		// Token: 0x04000556 RID: 1366
		MonsterInjured,
		// Token: 0x04000557 RID: 1367
		MonsterBlooadChanged,
		// Token: 0x04000558 RID: 1368
		MonsterAttacked,
		// Token: 0x04000559 RID: 1369
		MonsterLivingTime,
		// Token: 0x0400055A RID: 1370
		PreGotoLastMap,
		// Token: 0x0400055B RID: 1371
		PreInstallJunQi,
		// Token: 0x0400055C RID: 1372
		PreBangHuiAddMember,
		// Token: 0x0400055D RID: 1373
		PreBangHuiRemoveMember,
		// Token: 0x0400055E RID: 1374
		PreBangHuiChangeZhiWu,
		// Token: 0x0400055F RID: 1375
		PostBangHuiChange,
		// Token: 0x04000560 RID: 1376
		ProcessClickOnNpc,
		// Token: 0x04000561 RID: 1377
		StartPlayGame,
		// Token: 0x04000562 RID: 1378
		OnClientChangeMap,
		// Token: 0x04000563 RID: 1379
		OnCreateMonster,
		// Token: 0x04000564 RID: 1380
		ClientRegionEvent,
		// Token: 0x04000565 RID: 1381
		SevenDayGoal,
		// Token: 0x04000566 RID: 1382
		PreMonsterInjure,
		// Token: 0x04000567 RID: 1383
		AfterMonsterInjure,
		// Token: 0x04000568 RID: 1384
		MonsterToMonsterDead,
		// Token: 0x04000569 RID: 1385
		OnClientChargeItem,
		// Token: 0x0400056A RID: 1386
		OrnamentGoal,
		// Token: 0x0400056B RID: 1387
		PlayerOnline,
		// Token: 0x0400056C RID: 1388
		GameRunning,
		// Token: 0x0400056D RID: 1389
		BeforeProcessMsg,
		// Token: 0x0400056E RID: 1390
		PlayerEnterGame,
		// Token: 0x0400056F RID: 1391
		PlayerInitGameBeBan,
		// Token: 0x04000570 RID: 1392
		PlayerCreateRoleBeBan,
		// Token: 0x04000571 RID: 1393
		PlayerCreateRoleLimit,
		// Token: 0x04000572 RID: 1394
		LoginSuccess,
		// Token: 0x04000573 RID: 1395
		LoginFailByDataCheck,
		// Token: 0x04000574 RID: 1396
		LoginFailByUserBan,
		// Token: 0x04000575 RID: 1397
		LoginFailByTimeout,
		// Token: 0x04000576 RID: 1398
		SocketConnect,
		// Token: 0x04000577 RID: 1399
		SocketClose,
		// Token: 0x04000578 RID: 1400
		UserLogin,
		// Token: 0x04000579 RID: 1401
		UserCreateRole,
		// Token: 0x0400057A RID: 1402
		JingJiChangWin,
		// Token: 0x0400057B RID: 1403
		JingJiChangFailed,
		// Token: 0x0400057C RID: 1404
		PlayerLogoutFinish,
		// Token: 0x0400057D RID: 1405
		RoleKillMonster,
		// Token: 0x0400057E RID: 1406
		OneSecsTimerEvent,
		// Token: 0x0400057F RID: 1407
		MainTaskProgressEvent,
		// Token: 0x04000580 RID: 1408
		OnClientMapChanged,
		// Token: 0x04000581 RID: 1409
		OnClientGetSceneInfo,
		// Token: 0x04000582 RID: 1410
		OnKuaFuLogin,
		// Token: 0x04000583 RID: 1411
		OnKuaFuInitGame,
		// Token: 0x04000584 RID: 1412
		PreZhanDuiChangeMember,
		// Token: 0x04000585 RID: 1413
		PostMemberZhanDuiChange,
		// Token: 0x04000586 RID: 1414
		LeaveMap,
		// Token: 0x04000587 RID: 1415
		SimpleEventTypeCount,
		// Token: 0x04000588 RID: 1416
		Max = 10000
	}
}
