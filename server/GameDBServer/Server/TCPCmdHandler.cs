using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.DB.DBController;
using GameDBServer.Logic;
using GameDBServer.Logic.Activity;
using GameDBServer.Logic.FluorescentGem;
using GameDBServer.Logic.GuardStatue;
using GameDBServer.Logic.MerlinMagicBook;
using GameDBServer.Logic.Name;
using GameDBServer.Logic.Ornament;
using GameDBServer.Logic.Rank;
using GameDBServer.Logic.Talent;
using GameDBServer.Logic.Tarot;
using GameDBServer.Logic.Ten;
using GameDBServer.Logic.Wing;
using GameDBServer.Tools;
using GameServer.Core.AssemblyPatch;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Server
{
	// Token: 0x02000208 RID: 520
	public class TCPCmdHandler
	{
		// Token: 0x06000AB8 RID: 2744 RVA: 0x00066218 File Offset: 0x00064418
		public static TCPProcessCmdResults ProcessCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_FAILED;
			tcpOutPacket = null;
			TCPProcessCmdResults procRst = AssemblyPatchManager.getInstance().ProcessMsg(client, nID, data, count);
			TCPProcessCmdResults result2;
			if (TCPProcessCmdResults.RESUTL_CONTINUE != procRst)
			{
				result2 = procRst;
			}
			else
			{
				if (nID <= 927)
				{
					if (nID <= 404)
					{
						if (nID > 157)
						{
							if (nID <= 259)
							{
								if (nID != 163)
								{
									switch (nID)
									{
									case 191:
										result = TCPCmdHandler.ProcessGetDJPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 192:
									case 193:
									case 194:
									case 203:
										goto IL_2F47;
									case 195:
										result = TCPCmdHandler.ProcessQueryNameByIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 196:
										result = TCPCmdHandler.ProcessAddHorseCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 197:
										result = TCPCmdHandler.ProcessAddPetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 198:
										result = TCPCmdHandler.ProcessGetHorseListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 199:
										result = TCPCmdHandler.ProcessGetOtherHorseListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 200:
										result = TCPCmdHandler.ProcessGetPetListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 201:
										result = TCPCmdHandler.ProcessModHorseCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 202:
										result = TCPCmdHandler.ProcessModPetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 204:
										result = TCPCmdHandler.ProcessGetGoodsListBySiteCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									default:
										switch (nID)
										{
										case 257:
											result = TCPCmdHandler.ProcessGetRandomNameNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
											goto IL_2F5B;
										case 258:
											goto IL_2F47;
										case 259:
											result = TCPCmdHandler.ProcessGetFuBenHistDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
											goto IL_2F5B;
										default:
											goto IL_2F47;
										}
										break;
									}
								}
							}
							else
							{
								if (nID == 269)
								{
									result = TCPCmdHandler.ProcessGetPaiHangListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								switch (nID)
								{
								case 275:
									break;
								case 276:
								case 277:
								case 278:
								case 279:
								case 280:
								case 281:
								case 282:
								case 283:
								case 284:
								case 285:
								case 286:
								case 288:
								case 290:
								case 292:
								case 293:
								case 296:
								case 301:
								case 307:
								case 311:
								case 312:
								case 316:
								case 320:
								case 321:
								case 322:
								case 323:
								case 324:
								case 329:
								case 330:
								case 331:
								case 332:
								case 333:
								case 334:
								case 336:
								case 337:
								case 338:
								case 339:
								case 347:
								case 348:
								case 349:
								case 351:
								case 352:
								case 353:
								case 354:
								case 356:
								case 357:
								case 358:
								case 359:
								case 360:
								case 361:
								case 362:
								case 367:
								case 368:
								case 369:
								case 370:
								case 371:
									goto IL_2F47;
								case 287:
									result = TCPCmdHandler.ProcessGetChongZhiJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 289:
									result = TCPCmdHandler.ProcessGetFuBenHistListDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 291:
									result = TCPCmdHandler.ProcessGetOtherHorseDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 294:
									result = TCPCmdHandler.ProcessGetBangHuiListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 295:
									result = TCPCmdHandler.ProcessCreateBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 297:
									result = TCPCmdHandler.ProcessQueryBangHuiDetailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 298:
									result = TCPCmdHandler.ProcessUpdateBangHuiBulletinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 299:
									result = TCPCmdHandler.ProcessGetBHMemberDataListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 300:
									result = TCPCmdHandler.ProcessUpdateBHVerifyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 302:
									result = TCPCmdHandler.ProcessAddBHMemberCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 303:
									result = TCPCmdHandler.ProcessRemoveBHMemberCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 304:
									result = TCPCmdHandler.ProcessQuitFromBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 305:
									result = TCPCmdHandler.ProcessDestroyBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 306:
									result = TCPCmdHandler.ProcessBangHuiVerifyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 308:
									result = TCPCmdHandler.ProcessChgBHMemberZhiWuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 309:
									result = TCPCmdHandler.ProcessChgBHMemberChengHaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 310:
									result = TCPCmdHandler.ProcessSearchRolesFromDBCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 313:
									result = TCPCmdHandler.ProcessGetBangGongHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 314:
									result = TCPCmdHandler.ProcessDonateBGMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 315:
									result = TCPCmdHandler.ProcessDonateBGGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 317:
									result = TCPCmdHandler.ProcessGetBangQiInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 318:
									result = TCPCmdHandler.ProcessRenameBangQiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 319:
									result = TCPCmdHandler.ProcessUpLevelBangQiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 325:
									result = TCPCmdHandler.ProcessGetBHLingDiInfoDictByBHIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 326:
									result = TCPCmdHandler.ProcessSetLingDiTaxCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 327:
									result = TCPCmdHandler.ProcessTakeLingDiTaxMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 328:
									result = TCPCmdHandler.ProcessGetHuangDiBHInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 335:
									result = TCPCmdHandler.ProcessQueryQiZhenGeBuyHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 340:
									result = TCPCmdHandler.ProcessGetHuangDiRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 341:
									result = TCPCmdHandler.ProcessAddHuangFeiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 342:
									result = TCPCmdHandler.ProcessRemoveHuangFeiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 343:
									result = TCPCmdHandler.ProcessGetHuangFeiDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 344:
									result = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 345:
									result = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 346:
									result = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 350:
									result = TCPCmdHandler.ProcessAddLingDiTaxMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 355:
									result = TCPCmdHandler.ProcessGetGoodsByDbIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 363:
									result = TCPCmdHandler.ProcessGetUserMailListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 364:
									result = TCPCmdHandler.ProcessGetUserMailDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 365:
									result = TCPCmdHandler.ProcessFetchMailGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 366:
									result = TCPCmdHandler.ProcessDeleteUserMailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 372:
									result = TCPCmdHandler.ProcessSprQueryInputFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 373:
									result = TCPCmdHandler.ProcessSprQueryInputJiaSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 374:
									result = TCPCmdHandler.ProcessSprQueryInputKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 375:
									result = TCPCmdHandler.ProcessSprQueryLevelKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 376:
									result = TCPCmdHandler.ProcessSprQueryEquipKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 377:
									result = TCPCmdHandler.ProcessSprQueryHorseKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 378:
									result = TCPCmdHandler.ProcessSprQueryJingMaiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 379:
									result = TCPCmdHandler.ProcessSprQueryAwardHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 380:
									result = TCPCmdHandler.ProcessExcuteInputFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 381:
									result = TCPCmdHandler.ProcessExcuteInputJiaSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 382:
									result = TCPCmdHandler.ProcessExcuteInputKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 383:
									result = TCPCmdHandler.ProcessExcuteLevelKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 384:
									result = TCPCmdHandler.ProcessExcuteEquipKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 385:
									result = TCPCmdHandler.ProcessExcuteHorseKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 386:
									result = TCPCmdHandler.ProcessExcuteJingMaiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								default:
									switch (nID)
									{
									case 402:
										result = TCPCmdHandler.ProcessQueryShengXiaoGuessHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 403:
										goto IL_2F47;
									case 404:
										result = TCPCmdHandler.ProcessQueryShengXiaoGuessHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									default:
										goto IL_2F47;
									}
									break;
								}
							}
							result = TCPCmdHandler.ProcessGetOtherAttrib2DataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						if (nID <= 125)
						{
							switch (nID)
							{
							case 98:
								result = TCPCmdHandler.ProcessPreRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 99:
								result = TCPCmdHandler.ProcessUnPreRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 100:
								break;
							case 101:
								result = TCPCmdHandler.ProcessGetRoleListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 102:
								result = TCPCmdHandler.ProcessCreateRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 103:
								result = TCPCmdHandler.ProcessRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 104:
								result = TCPCmdHandler.ProcessInitGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 125)
								{
									result = TCPCmdHandler.ProcessNewTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
						else
						{
							switch (nID)
							{
							case 140:
								result = TCPCmdHandler.ProcessCompleteTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 141:
								break;
							case 142:
								result = TCPCmdHandler.ProcessGetFriendsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 143:
								result = TCPCmdHandler.ProcessAddFriendCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 144:
								result = TCPCmdHandler.ProcessRemoveFriendCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 154)
								{
									result = TCPCmdHandler.ProcessAbandonTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								if (nID == 157)
								{
									result = TCPCmdHandler.ProcessSpriteChatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
					}
					else if (nID <= 631)
					{
						if (nID <= 524)
						{
							if (nID == 418)
							{
								result = TCPCmdHandler.ProcessTakeLingDiDailyAwardCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 452:
								result = TCPCmdHandler.ProcessQueryZaJinDanHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 453:
								result = TCPCmdHandler.ProcessQueryZaJinDanHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 454:
							case 455:
							case 456:
							case 457:
							case 458:
							case 459:
							case 460:
							case 466:
							case 474:
							case 477:
							case 478:
							case 479:
							case 494:
							case 495:
							case 496:
							case 497:
							case 498:
							case 499:
							case 500:
							case 502:
							case 505:
							case 506:
							case 507:
							case 508:
							case 510:
							case 511:
								break;
							case 461:
								result = TCPCmdHandler.ProcessQueryJieriDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 462:
								result = TCPCmdHandler.ProcessQueryJieriDengLuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 463:
								result = TCPCmdHandler.ProcessQueryJieriVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 464:
								result = TCPCmdHandler.ProcessQueryJieriCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 465:
								result = TCPCmdHandler.ProcessQueryJieriCZLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 467:
								result = TCPCmdHandler.ProcessQueryJieriXiaoFeiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 468:
								result = TCPCmdHandler.ProcessQueryJieriCZKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 469:
								result = TCPCmdHandler.ProcessExecuteJieriDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 470:
								result = TCPCmdHandler.ProcessExecuteJieriDengLuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 471:
								result = TCPCmdHandler.ProcessExecuteJieriVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 472:
								result = TCPCmdHandler.ProcessExecuteJieriCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 473:
								result = TCPCmdHandler.ProcessExecuteJieriCZLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 475:
								result = TCPCmdHandler.ProcessExecuteJieriXiaoFeiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 476:
								result = TCPCmdHandler.ProcessExecuteJieriCZKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 480:
								result = TCPCmdHandler.ProcessQueryHeFuDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 481:
								result = TCPCmdHandler.ProcessQueryHeFuVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 482:
								result = TCPCmdHandler.ProcessQueryHeFuCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 483:
								result = TCPCmdHandler.ProcessQueryHeFuCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 484:
								result = TCPCmdHandler.ProcessQueryHeFuPKKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 485:
								result = TCPCmdHandler.ProcessQueryHeFuWCKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 486:
								result = TCPCmdHandler.ProcessQueryXinCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 487:
								result = TCPCmdHandler.ProcessExecuteHeFuDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 488:
								result = TCPCmdHandler.ProcessExecuteHeFuVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 489:
								result = TCPCmdHandler.ProcessExecuteHeFuCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 490:
								result = TCPCmdHandler.ProcessExecuteHeFuCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 491:
								result = TCPCmdHandler.ProcessExecuteHeFuPKKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 492:
								result = TCPCmdHandler.ProcessExecuteHeFuWCKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 493:
								result = TCPCmdHandler.ProcessExecuteXinCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 501:
								result = TCPCmdHandler.ProcessQueryActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 503:
								result = TCPCmdHandler.ProcessQueryYueDuChouJiangHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 504:
								result = TCPCmdHandler.ProcessQueryYueDuChouJiangHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 509:
								result = TCPCmdHandler.ProcessChangeLifeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 512:
								result = TCPCmdHandler.ProcessGetUsingGoodsDataListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 524)
								{
									result = TCPCmdHandler.ProcessCompleteFlashSceneCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
						else
						{
							if (nID == 528)
							{
								result = TCPCmdHandler.ProcessAdmiredPlayerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							if (nID == 560)
							{
								result = TCPCmdHandler.ProcessSetAutoAssignPropertyPointCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 629:
								result = NewZoneActiveMgr.ProcessGetNewzoneActiveAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 630:
								goto IL_2F5B;
							case 631:
								result = NewZoneActiveMgr.ProcessQueryActiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
						}
					}
					else if (nID <= 684)
					{
						if (nID == 649)
						{
							result = TCPCmdHandler.ProcessGetUserMailCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						if (nID == 671)
						{
							result = CFirstChargeMgr.ProcessQueryUserFirstCharge(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 683:
							result = TCPCmdHandler.ProcessQueryJieriTotalConsumeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 684:
							result = TCPCmdHandler.ProcessExecuteJieriTotalConsumeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
					}
					else
					{
						if (nID == 711)
						{
							result = TCPCmdHandler.ProcessGetBangHuiFuBenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 908:
							result = TCPCmdHandler.ProcessQueryThemeDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 909:
							result = TCPCmdHandler.ProcessExecuteThemeDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							if (nID == 927)
							{
								result = TCPCmdHandler.ProcessExecuteJieriFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
				}
				else if (nID <= 13321)
				{
					if (nID <= 10224)
					{
						if (nID <= 1240)
						{
							switch (nID)
							{
							case 947:
								result = TCPCmdHandler.ProcessQueryDanBiChongZhiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 948:
								result = TCPCmdHandler.ProcessExecuteDanBiChongZhiJiangLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 1111:
									result = TCPCmdHandler.ProcessGVoiceSetPrioritysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 1112:
									result = TCPCmdHandler.ProcessGVoiceGetPrioritysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								default:
									if (nID == 1240)
									{
										result = TCPCmdHandler.ProcessChgJunTuanMemberZhiWuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									}
									break;
								}
								break;
							}
						}
						else
						{
							if (nID == 1500)
							{
								result = TCPCmdHandler.ProcessGetInputPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 1806:
								result = TCPCmdHandler.ProcessQueryJieRiMeiRiLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 1807:
								result = TCPCmdHandler.ProcessExecuteJieriMeiRiLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 10001:
									result = TCPCmdHandler.ProcessUpdatePosCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10002:
									result = TCPCmdHandler.ProcessUpdateExpLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10003:
									result = TCPCmdHandler.ProcessUpdateInterPowerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10004:
									result = TCPCmdHandler.ProcessUpdateMoney1Cmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10005:
									result = TCPCmdHandler.ProcessAddGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10006:
									result = TCPCmdHandler.ProcessUpdateGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10007:
									result = TCPCmdHandler.ProcessUpdateTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10008:
									result = TCPCmdHandler.ProcessUpdatePKModeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10009:
									result = TCPCmdHandler.ProcessUpdatePKValCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10010:
									result = TCPCmdHandler.ProcessModKeysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10011:
									result = TCPCmdHandler.ProcessUpdateUserMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10012:
									result = TCPCmdHandler.ProcessUpdateUserYinLiangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10013:
									result = TCPCmdHandler.ProcessMoveGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10014:
									result = TCPCmdHandler.ProcessUpdateLeftFightSecsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10015:
									result = TCPCmdHandler.ProcessRoleOnLineGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10016:
									result = TCPCmdHandler.ProcessRoleHeartGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10017:
									result = TCPCmdHandler.ProcessRoleOffLineGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10018:
									result = TCPCmdHandler.ProcessGetChatMsgListCmd(client, dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10019:
									result = TCPCmdHandler.ProcessHorseOnCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10020:
									result = TCPCmdHandler.ProcessHorseOffCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10021:
									result = TCPCmdHandler.ProcessPetOutCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10022:
									result = TCPCmdHandler.ProcessPetInCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10023:
									result = TCPCmdHandler.ProcessGetAddDJPointCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10024:
									result = TCPCmdHandler.ProcessUpDianJiangLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10025:
									result = TCPCmdHandler.ProcessRegUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10026:
									result = TCPCmdHandler.ProcessBanRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10027:
									result = TCPCmdHandler.ProcessBanRoleChatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10028:
									result = TCPCmdHandler.ProcessGetBanRoleChatDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10029:
									result = TCPCmdHandler.ProcessAddBullMsgCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10030:
									result = TCPCmdHandler.ProcessRemoveBullMsgCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10031:
									result = TCPCmdHandler.ProcessGetBullMsgDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10032:
									result = TCPCmdHandler.ProcessUpdateOnlineTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10033:
									result = TCPCmdHandler.ProcessGameConfigDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10034:
									result = TCPCmdHandler.ProcessGameConfigItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10035:
									result = TCPCmdHandler.ProcessResetBigGuanCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10036:
									result = TCPCmdHandler.ProcessAddSkillCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10037:
									result = TCPCmdHandler.ProcessUpSkillInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10038:
									result = TCPCmdHandler.ProcessUpdateJingMaiExpCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10039:
									result = TCPCmdHandler.ProcessUpdateSkillIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10040:
									result = TCPCmdHandler.ProcessUpdateAutoDrinkCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10041:
									result = TCPCmdHandler.ProcessUpdateDailyTaskDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10042:
									result = TCPCmdHandler.ProcessUpdateDailyJingMaiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10043:
									result = TCPCmdHandler.ProcessUpdateNumSkillIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10044:
									result = TCPCmdHandler.ProcessUpdatePBInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10045:
									result = TCPCmdHandler.ProcessUpdateHuoDongInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10046:
									result = TCPCmdHandler.ProcessSubChongZhiJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10047:
									result = TCPCmdHandler.ProcessUseLiPinMaCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10048:
									result = TCPCmdHandler.ProcessUpdateFuBenDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10049:
									result = TCPCmdHandler.ProcessGetFuBenSeqIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10050:
									result = TCPCmdHandler.ProcessUpdateRoleDailyDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10051:
									result = TCPCmdHandler.ProcessUpdateBufferItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10052:
									result = TCPCmdHandler.ProcessUnDelRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10053:
									result = TCPCmdHandler.ProcessAddFuBenHistDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10054:
									result = TCPCmdHandler.ProcessUpdateLianZhanCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10055:
									result = TCPCmdHandler.ProcessUpdateKillBossCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10056:
									result = TCPCmdHandler.ProcessUpdateRoleStatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10057:
									result = TCPCmdHandler.ProcessUpdateYaBiaoDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10058:
									result = TCPCmdHandler.ProcessUpdateYaBiaoDataStateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10059:
									result = TCPCmdHandler.ProcessUpdateBattleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10060:
									result = TCPCmdHandler.ProcessAddMallBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10061:
									result = TCPCmdHandler.ProcessGetLiPinMaInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10062:
									result = TCPCmdHandler.ProcessUpdateCZTaskIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10063:
									result = TCPCmdHandler.ProcessGetTotalOnlineNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10064:
									result = TCPCmdHandler.ProcessUpdateBattleNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10065:
									result = TCPCmdHandler.ProcessUpdateHeroIndexCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10066:
									result = TCPCmdHandler.ProcessForceReloadPaiHangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10067:
									result = TCPCmdHandler.ProcessAddYinPiaoBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10068:
									result = TCPCmdHandler.ProcessDelRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10069:
									result = TCPCmdHandler.ProcessQueryUserMoneyByNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10070:
									result = TCPCmdHandler.ProcessQueryBHMGRListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10071:
									result = TCPCmdHandler.ProcessUpdateBangGongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10072:
									result = TCPCmdHandler.ProcessUpdateBHTongQianCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10073:
									result = TCPCmdHandler.ProcessGetBHJunQiListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10074:
									result = TCPCmdHandler.ProcessGetBHLingDiDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10075:
									result = TCPCmdHandler.ProcessUpdateLingDiForBHCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10076:
									result = TCPCmdHandler.ProcessGetLeaderRoleIDByBHIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10077:
									result = TCPCmdHandler.ProcessAddBHTongQianCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10078:
									result = TCPCmdHandler.ProcessAddQiZhenGeBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10079:
									result = TCPCmdHandler.ProcessUpdateJieBiaoInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10080:
									result = TCPCmdHandler.ProcessAddRefreshQiZhenRecCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10081:
									result = TCPCmdHandler.ProcessClrCachingRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10082:
									result = TCPCmdHandler.ProcessAddMoneyWarningCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10083:
									result = TCPCmdHandler.ProcessQueryChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10084:
									result = TCPCmdHandler.ProcessAddYinLiangBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10085:
									result = TCPCmdHandler.ProcessAddBangGongBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10086:
									result = TCPCmdHandler.ProcessSendUserMailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10087:
									result = TCPCmdHandler.ProcessGetUserMailDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10088:
									result = TCPCmdHandler.ProcessGetRoleIDByRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10089:
									result = TCPCmdHandler.ProcessDBQueryLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10090:
									result = TCPCmdHandler.ProcessDBUpdateLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10091:
									result = TCPCmdHandler.ProcessUpdateDailyVipDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10092:
									result = TCPCmdHandler.ProcessUpdateYangGongBKDailyJiFenDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10093:
									result = TCPCmdHandler.ProcessUpdateSingleTimeAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10094:
									result = TCPCmdHandler.ProcessAddShengXiaoGuessHisotryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10095:
									result = TCPCmdHandler.ProcessUpdateUserGoldCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10096:
									result = TCPCmdHandler.ProcessAddGoldBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10097:
									result = TCPCmdHandler.ProcessUpdateRoleBagNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10098:
									result = TCPCmdHandler.ProcessSetLingDiWarRequestCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10099:
									result = TCPCmdHandler.ProcessUpdateGoodsLimitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10100:
									result = TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10101:
									result = TCPCmdHandler.ProcessAddQiangGouBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10102:
									goto IL_2F5B;
								case 10103:
									goto IL_2F5B;
								case 10104:
									goto IL_2F5B;
								case 10105:
									goto IL_2F5B;
								case 10106:
									result = TCPCmdHandler.ProcessGetBangHuiMiniDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10107:
									result = TCPCmdHandler.ProcessAddBuyItemFromNpcCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10108:
									result = TCPCmdHandler.ProcessAddZaJinDanHisotryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10109:
									result = TCPCmdHandler.ProcessQueryQiangGouBuyItemInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10110:
									result = TCPCmdHandler.ProcessQueryFirstChongZhiDaLiByUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10111:
									result = TCPCmdHandler.ProcessQueryKaiFuOnlineAwardRoleIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10112:
									result = TCPCmdHandler.ProcessAddKaiFuOnlineAwardCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10113:
									result = TCPCmdHandler.ProcessAddGiveUserMoneyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10114:
									result = TCPCmdHandler.ProcessQueryKaiFuOnlineAwardListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10115:
									result = TCPCmdHandler.ProcessAddExchange1ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10116:
									result = TCPCmdHandler.ProcessAddExchange2ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10117:
									result = TCPCmdHandler.ProcessAddExchange3ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10118:
									result = TCPCmdHandler.ProcessAddFallGoodsItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10119:
									result = TCPCmdHandler.ProcessUpdateRolePropsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10120:
									result = TCPCmdHandler.ProcessQueryDayChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10121:
									result = TCPCmdHandler.ProcessQueryDayChongZhiDaLiByUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10122:
									result = TCPCmdHandler.ProcessClrAllCachingRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10123:
									result = TCPCmdHandler.ProcessQueryXingYunChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10124:
									result = TCPCmdHandler.ProcessExcuteXingYunChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10125:
									result = TCPCmdHandler.ProcessExcuteAddYueDuChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10126:
									result = TCPCmdHandler.ProcessExecuteChangeOccupationCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10127:
									result = TCPCmdHandler.ProcessQueryBloodCastleEnterCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10128:
									result = TCPCmdHandler.ProcessUpdateBloodCastleEnterCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10129:
									result = TCPCmdHandler.ProcessQueryFuBenHisInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10130:
									result = TCPCmdHandler.ProcessCleanDataWhenFreshPlayerLogOutCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10131:
									result = TCPCmdHandler.ProcessFinishFreshPlayerStatusCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10132:
									result = TCPCmdHandler.ProcessChangeTaskStarLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10133:
									result = TCPCmdHandler.ProcessUpdateRoleSomeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10134:
									result = TCPCmdHandler.ProcessQueryDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10135:
									result = TCPCmdHandler.ProcessQueryRoleDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10136:
									result = TCPCmdHandler.ProcessUpdateRoleDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10137:
									result = TCPCmdHandler.ProcessQueryEveryDayOnLineAwardGiftInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10151:
									result = TCPCmdHandler.ProcessUpdatePushMessageInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10152:
									result = TCPCmdHandler.ProcessQueryPushMsgUerListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10153:
									result = TCPCmdHandler.ProcessAddWingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10154:
									result = TCPCmdHandler.ProcessModWingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10155:
									result = TCPCmdHandler.ProcessReferPictureJudgeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10156:
									result = TCPCmdHandler.ProcessQueryMoJingExchangeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10157:
									result = TCPCmdHandler.ProcessUpdateMoJingExchangeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10160:
									result = RechargeRepayActiveMgr.ProcessQueryActiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10161:
									result = RechargeRepayActiveMgr.ProcessGetActiveAwards(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10162:
									result = TCPCmdHandler.ProcessUpdateAccountActiveCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10163:
									result = CGetOldResourceManager.ProcessQueryGetResourceInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10164:
									result = CGetOldResourceManager.ProcessUpdateGetResourceInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10165:
									result = TCPCmdHandler.ProcessUpdateGoodsCmd2(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10166:
									result = TCPCmdHandler.ProcessUpdateStarConstellationCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10167:
									result = Global.SaveConsumeLog(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10168:
									result = TCPCmdHandler.ProcessQueryVipLevelAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10169:
									result = TCPCmdHandler.ProcessUpdateVipLevelAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10171:
									result = CFirstChargeMgr.FirstChargeConfig(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10172:
									result = TCPCmdHandler.ProcessUpdateBangHuiFuBenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10173:
									result = TCPCmdHandler.ProcessAddRoleStoreYinliang(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10174:
									result = TCPCmdHandler.ProcessAddRoleStoreMoney(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10175:
									result = TCPCmdHandler.ProcessGMUpdateBangLevel(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10176:
									result = TCPCmdHandler.ProcessUpdateLingYu(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10177:
									result = GroupMailManager.RequestNewGroupMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10178:
									result = GroupMailManager.ModifyGMailRecord(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10179:
									result = TCPCmdHandler.ProcessQueryRoleMoneyInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10180:
									result = TCPCmdHandler.ProcessAutoCompletionTaskByTaskID(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10181:
									result = TCPCmdHandler.ProcessRoleBuyYueKaButOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10182:
									result = TCPCmdHandler.ProcessRoleHuobiOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10183:
									result = TCPCmdHandler.ProcessUsrSetSecPwd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10184:
									result = TCPCmdHandler.ProcessGetUsrSecondPassword(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10185:
									result = TCPCmdHandler.ProcessUpdateMarriageDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10186:
									result = TCPCmdHandler.ProcessGetMarriageDataCmd(dbMgr, pool, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10187:
									result = TCPCmdHandler.ProcessQueryMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10188:
									result = TCPCmdHandler.ProcessAddMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10189:
									result = TCPCmdHandler.ProcessRemoveMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10190:
									result = TCPCmdHandler.ProcessIncMarryPartyJoin(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10191:
									result = TCPCmdHandler.ProcessClearMarryPartyJoin(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10203:
									result = MerlinMagicBookManager.getInstance().ProcessInsertMerlinDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10204:
									result = MerlinMagicBookManager.getInstance().ProcessUpdateMerlinDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10205:
									result = MerlinMagicBookManager.getInstance().ProcessQueryMerlinDataCmd(dbMgr, pool, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10206:
									result = TCPCmdHandler.ProcessUpdateHolyItemDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10207:
									result = FluorescentGemManager.getInstance().ProcessResetBagDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10208:
									result = FluorescentGemManager.getInstance().ProcessUpdateFluorescentPointCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10209:
									result = FluorescentGemManager.getInstance().ProcessEquipGemCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10210:
									result = FluorescentGemManager.getInstance().ProcessUnEquipGemCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10211:
									result = FluorescentGemManager.getInstance().ProcessModifyFluorescentPointCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10220:
									result = TCPCmdHandler.ProcessQueryRoleMiniInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10221:
									result = TCPCmdHandler.ProcessSprQueryUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10222:
									result = TCPCmdHandler.ProcessSprUpdateUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10223:
									result = TCPCmdHandler.ProcessUpdateZhengDuoUsedTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10224:
									result = TCPCmdHandler.ProcessGetZhengDuoUsedTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
					}
					else if (nID <= 13121)
					{
						switch (nID)
						{
						case 11000:
							result = TCPCmdHandler.ProcessGetServerListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11001:
							result = TCPCmdHandler.ProcessOnlineServerHeartCmd(client, dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11002:
							result = TCPCmdHandler.ProcessGetServerIdCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11003:
							result = TCPCmdHandler.ProcessSetServerDayDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11004:
							result = TCPCmdHandler.ProcessGetServerDayDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11005:
							result = TCPCmdHandler.ProcessGetServerPTIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 13000:
								result = TCPCmdHandler.ProcessGMSetTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13001:
								result = TCPCmdHandler.ProcessQueryUserIdValueCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 13108:
									result = TalentManager.ProcTalentModify(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13109:
									result = TalentManager.ProcTalentEffectModify(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13110:
									result = TalentManager.ProcTalentEffectClear(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13111:
									result = TCPCmdHandler.ProcessGmBanCheck(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13112:
									result = TCPCmdHandler.ProcessGmBanLog(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13113:
									result = TCPCmdHandler.ProcessTenInitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13114:
									result = TCPCmdHandler.ProcessSpreadAwardGetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13115:
									result = TCPCmdHandler.ProcessSpreadAwardUpdateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13120:
									result = TCPCmdHandler.ProcessActivateStateGetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13121:
									result = TCPCmdHandler.ProcessActivateStateSetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
							break;
						}
					}
					else
					{
						switch (nID)
						{
						case 13150:
							result = TCPCmdHandler.ProcessInputPointsExchangeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13151:
							result = TCPCmdHandler.ProcessUpdateInputPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13152:
							result = TCPCmdHandler.ProcessUpdateInputPointsUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13153:
						case 13154:
						case 13155:
						case 13156:
						case 13157:
						case 13158:
						case 13159:
						case 13165:
						case 13166:
						case 13167:
						case 13168:
						case 13169:
						case 13174:
							break;
						case 13160:
							result = TCPCmdHandler.ProcessUpdateSpecActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13161:
							result = TCPCmdHandler.ProcessDeleteSpecActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13162:
							result = TCPCmdHandler.ProcessGetSpecJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13163:
							result = TCPCmdHandler.ProcessUpdateSpecJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13164:
							result = TCPCmdHandler.ProcessGetSpecActInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13170:
							result = TCPCmdHandler.ProcessUpdateEveryActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13171:
							result = TCPCmdHandler.ProcessDeleteEveryActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13172:
							result = TCPCmdHandler.ProcessGetEveryJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13173:
							result = TCPCmdHandler.ProcessUpdateEveryJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13175:
							result = TCPCmdHandler.ProcessUpdateSpecPriorityActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13176:
							result = TCPCmdHandler.ProcessDeleteSpecPriorityActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13177:
							result = TCPCmdHandler.ProcessQueryPeriodChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 13200:
								result = SingletonTemplate<JieriGiveActHandler>.Instance().ProcRoleJieriGiveToOther(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13201:
								result = SingletonTemplate<JieriGiveActHandler>.Instance().ProcessGetJieriGiveAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13202:
								result = SingletonTemplate<JieriGiveActHandler>.Instance().ProcQueryJieriGiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13203:
								result = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcLoadJieriGiveKingRank(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13204:
								result = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcLoadRoleJieriGiveKing(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13205:
								result = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcGetJieriGiveKingAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13206:
								result = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcLoadJieriRecvKingRank(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13207:
								result = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcLoadRoleJieriRecvKing(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13208:
								result = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcGetJieriRecvKingAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13209:
							case 13212:
							case 13213:
							case 13216:
							case 13217:
							case 13218:
							case 13219:
							case 13220:
							case 13221:
							case 13222:
								break;
							case 13210:
								result = SingletonTemplate<GuardStatueHandler>.Instance().ProcUpdateRoleGuardStatue(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13211:
								result = SingletonTemplate<GuardStatueHandler>.Instance().ProcUpdateRoleGuardSoul(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13214:
								result = SingletonTemplate<JieriLianXuChargeActHandler>.Instance().ProcQueryActInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13215:
								result = SingletonTemplate<JieriLianXuChargeActHandler>.Instance().ProcUpdateAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13223:
								result = OrnamentManager.ProcessUpdateOrnamentDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 13320:
									result = UserMoneyMgr.ProcessGetChargeItemData(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13321:
									result = UserMoneyMgr.ProcessDelChargeItemData(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
							break;
						}
					}
				}
				else if (nID <= 20003)
				{
					if (nID <= 14021)
					{
						if (nID == 13400)
						{
							result = TCPCmdHandler.ProcessUpdateOnePieceTreasureLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 14001:
							result = SingletonTemplate<NameManager>.Instance().ProcChangeName(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14002:
							result = SingletonTemplate<NameManager>.Instance().ProcQueryEachRoleInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14003:
						case 14004:
						case 14007:
						case 14008:
							break;
						case 14005:
							result = SingletonTemplate<JieriPlatChargeKingActHandler>.Instance().ProcGetJieriPlatChargeKingList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14006:
							result = SingletonTemplate<NameManager>.Instance().ProcChangeBangHuiName(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14009:
							result = SingletonTemplate<NameManager>.Instance().ProcAddBangHuiChangeNameTimes(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 14020:
								result = TCPCmdHandler.ProcessBHMatchLoadSupportFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 14021:
								result = TCPCmdHandler.ProcessBHMatchUpdateSupportFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
					else
					{
						switch (nID)
						{
						case 14101:
							result = TCPCmdHandler.ProcessUpdateFuMoAcceptMap(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14102:
							result = TCPCmdHandler.ProcessAddFuMoMoneyGiveMail(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14103:
							result = TCPCmdHandler.ProcessFuMoMailIndexCount(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14104:
							result = TCPCmdHandler.ProcessAddFuMoMoneyGiveMailTemp(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14105:
							result = TCPCmdHandler.ProcessGetFuMoMoneyMapAcceptNum(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14106:
							result = TCPCmdHandler.ProcessGetFuMoMoneyMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14107:
							result = TCPCmdHandler.ProcessGetFuMoMoneyMailMapList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14108:
							result = TCPCmdHandler.ProcessUpdateFuMoMoneyMailMap(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14109:
							result = TCPCmdHandler.ProcessDeleteFuMoMail(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14110:
							result = TCPCmdHandler.ProcessDeleteFuMoMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14111:
							result = TCPCmdHandler.ProcessUpdateFuMoMailReadState(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14112:
						case 14113:
						case 14114:
						case 14121:
						case 14123:
						case 14124:
						case 14125:
						case 14126:
						case 14127:
						case 14128:
						case 14129:
							break;
						case 14115:
							result = TCPCmdHandler.ProcessRebornYinJiInsertInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14116:
							result = TCPCmdHandler.ProcessRebornYinJiUpdateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14117:
							result = TCPCmdHandler.ProcessGetRebornYinJiInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14118:
							result = RebornEquip.ProcessUpdateRoleRebornBagNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14119:
							result = RebornEquip.ProcessUpdateRebornStorageInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14120:
							result = RebornEquip.ProcessUpdateRoleRebornShowEquipCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14122:
							result = RebornEquip.ProcessUpdateRoleRebornShowModelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14130:
							result = UserRegressActiveManager.ProcessGetRegressActiveMinTime(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14131:
							result = UserRegressActiveManager.ProcessUpdateEverySignCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14132:
							result = UserRegressActiveManager.ProcessSprQueryUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14133:
							result = UserRegressActiveManager.ProcessDBQueryUserAllLimitGoodsUsedNumInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14134:
							result = UserRegressActiveManager.ProcessDBQueryLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14135:
							result = UserRegressActiveManager.ProcessDBUpdateUserLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14136:
							result = UserRegressActiveManager.ProcessRergressQueryUserInputMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14137:
							result = UserRegressActiveManager.ProcessSprQueryDayUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14138:
							result = UserRegressActiveManager.ProcessUpdateSprQueryDayUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							if (nID == 20000)
							{
								result = TCPCmdHandler.ProcessAddItemLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							if (nID == 20003)
							{
								result = TCPCmdHandler.ProcessUpdateRoleKuaFuDayLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
				}
				else if (nID <= 20314)
				{
					if (nID == 20100)
					{
						result = TarotManager.ProcessUpdateTarotDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					switch (nID)
					{
					case 20304:
						result = TCPCmdHandler.ProcessGetRoleParamCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					case 20305:
						result = TCPCmdHandler.ProcessUpdateWebOldPlayerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					default:
						if (nID == 20314)
						{
							result = TCPCmdHandler.ProcessGetGoodsListBySiteRangeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						break;
					}
				}
				else
				{
					if (nID == 20398)
					{
						result = TCPCmdHandler.ProcessSubRoleHuobiOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					if (nID == 21000)
					{
						result = TCPCmdHandler.ProcessFacebookInitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					if (nID == 30010)
					{
						result = TCPCmdHandler.ProcessGetZoneIdByRid(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
				}
				IL_2F47:
				result = TCPCmdDispatcher.getInstance().dispathProcessor(client, nID, data, count);
				IL_2F5B:
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x00069188 File Offset: 0x00067388
		private static TCPProcessCmdResults ProcessGetUsrSecondPassword(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string usrid = fields[0];
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(usrid);
				if (dbUserInfo == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的user不存在，CMD={0}, userID={1}", (TCPGameServerCmds)nID, usrid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}", usrid, dbUserInfo.SecPwd), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x00069304 File Offset: 0x00067504
		private static TCPProcessCmdResults ProcessUsrSetSecPwd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userid = fields[0];
				string secPwd = fields[1];
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userid);
				if (dbUserInfo == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的user不存在，CMD={0}, userID={1}", (TCPGameServerCmds)nID, userid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool updateOK = false;
				if (DBWriter.UpdateUsrSecondPassword(dbMgr, userid, secPwd))
				{
					dbUserInfo.SecPwd = secPwd;
					updateOK = true;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, updateOK ? "0:1" : "0", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x000694A8 File Offset: 0x000676A8
		private static TCPProcessCmdResults ProcessRoleHuobiOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				string huobiType = fields[1];
				int modifyValue = Convert.ToInt32(fields[2]);
				if (string.IsNullOrEmpty(huobiType))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("被修改货币的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string rolename = dbRoleInfo.RoleName;
				if (!("jinbi" == huobiType))
				{
					if (!("bangjin" == huobiType))
					{
						if (!("zuanshi" == huobiType))
						{
							if (!("bangzuan" == huobiType))
							{
								if ("mojing" == huobiType)
								{
									string key = "TianDiJingYuan";
									long newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, (long)modifyValue, null);
									string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
									byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
									return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
								}
								if (!("chengjiu" == huobiType))
								{
									if (!("shengwang" == huobiType))
									{
										if (!("xinghun" == huobiType))
										{
											if ("lingjing" == huobiType)
											{
												string key = "MUMoHe";
												long newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, (long)modifyValue, null);
												string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
												byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
												return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
											}
											if (!("fenmo" == huobiType))
											{
												if ("zaizao" == huobiType)
												{
													string key = "ZaiZaoPoint";
													long newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, (long)modifyValue, null);
													string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
													byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
													return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
												}
												LogManager.WriteLog(LogTypes.Error, string.Format("未注册的GM修改货币类型,Rolename:{0},Huobi:{1},Modify:{2}", rolename, huobiType, modifyValue), null, true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x00069900 File Offset: 0x00067B00
		private static TCPProcessCmdResults ProcessSubRoleHuobiOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Global.SafeConvertToInt32(fields[0], 10);
				int huobiIndex = Global.SafeConvertToInt32(fields[1], 10);
				long modifyValue = Global.SafeConvertToInt64(fields[2], 10);
				if (huobiIndex == 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("被修改货币的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string rolename = dbRoleInfo.RoleName;
				int num = huobiIndex;
				if (num <= 40)
				{
					if (num <= 8)
					{
						if (num != 1)
						{
							if (num == 8)
							{
								long newVal = (long)dbRoleInfo.YinLiang + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string cmd = string.Format("{0}:{1}", dbRoleInfo.RoleID, modifyValue);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateUserYinLiangCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
						}
						else
						{
							long newVal = (long)dbRoleInfo.Money1 + modifyValue;
							if (newVal < -2147483648L || newVal > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string cmd = string.Format("{0}:{1}", dbRoleInfo.RoleID, newVal);
							byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
							return TCPCmdHandler.ProcessUpdateMoney1Cmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
						}
					}
					else
					{
						switch (num)
						{
						case 13:
						{
							string key = "TianDiJingYuan";
							long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
							if (newVal < -2147483648L || newVal > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
							string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
							byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
						}
						case 14:
							break;
						case 15:
						{
							string key = "ZJDJiFen";
							long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
							if (newVal < -2147483648L || newVal > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
							string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
							byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
						}
						default:
							if (num == 40)
							{
								DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(dbRoleInfo.UserID);
								if (null == dbUserInfo)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								long newVal = (long)dbUserInfo.Money + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string cmd = string.Format("{0}:{1}", dbRoleInfo.RoleID, modifyValue);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateUserMoneyCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							break;
						}
					}
				}
				else if (num <= 101)
				{
					if (num != 50)
					{
						if (num == 101)
						{
							string key = "ZaiZaoPoint";
							long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
							if (newVal < -2147483648L || newVal > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
							string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
							byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
						}
					}
					else
					{
						long newVal = (long)dbRoleInfo.Gold + modifyValue;
						if (newVal < -2147483648L || newVal > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						string cmd = string.Format("{0}:{1}", dbRoleInfo.RoleID, modifyValue);
						byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
						return TCPCmdHandler.ProcessUpdateUserGoldCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
					}
				}
				else
				{
					switch (num)
					{
					case 106:
					{
						string key = "MUMoHe";
						long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
						if (newVal < -2147483648L || newVal > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
						string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
						byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
					}
					case 107:
					{
						string key = "ElementPowder";
						long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
						if (newVal < -2147483648L || newVal > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
						string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
						byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
					}
					case 108:
						break;
					case 109:
					{
						long newVal = (long)dbRoleInfo.FluorescentPoint + modifyValue;
						if (newVal < -2147483648L || newVal > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int nPoint = (int)newVal;
						int ret = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, dbRoleInfo.RoleID, nPoint) ? dbRoleInfo.RoleID : 0;
						lock (dbRoleInfo)
						{
							dbRoleInfo.FluorescentPoint = nPoint;
						}
						string cmd = string.Format("{0}:{1}", ret, nPoint);
						byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
					}
					default:
						if (num != 119)
						{
							switch (num)
							{
							case 129:
							{
								string key = "10187";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 130:
							{
								long newVal = (long)dbRoleInfo.AlchemyInfo.BaseData.Element + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string cmd = string.Format("{0}:{1}", dbRoleInfo.RoleID, modifyValue);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return SingletonTemplate<AlchemyManager>.Instance().ProcessUpdateAlchemyElement(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 131:
							{
								string key = "10194";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 132:
							{
								string key = "10208";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 133:
							{
								string key = "10209";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 134:
							{
								string key = "10217";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							case 135:
							{
								string key = "10204";
								long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
								if (newVal < -2147483648L || newVal > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
								string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
								byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
							}
							}
						}
						else
						{
							string key = "10153";
							long newVal = Global.GetRoleParamsInt64(dbRoleInfo, key) + modifyValue;
							if (newVal < -2147483648L || newVal > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							newVal = Global.ModifyRoleParamLongByName(dbMgr, dbRoleInfo, key, modifyValue, null);
							string cmd = string.Format("{0}:{1}:{2}", dbRoleInfo.RoleID, key, newVal);
							byte[] bytesCmd = new UTF8Encoding().GetBytes(cmd);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytesCmd, bytesCmd.Length, out tcpOutPacket);
						}
						break;
					}
				}
				LogManager.WriteLog(LogTypes.Error, " -modifyRoleHuobi 未注册的货币类型:" + huobiIndex, null, true);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0006A780 File Offset: 0x00068980
		private static TCPProcessCmdResults ProcessRoleBuyYueKaButOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int beginOffsetDay = Convert.ToInt32(fields[1]);
				string key = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					string[] oldFields = null;
					string oldParamValue = "";
					string oldParamValueStr = Global.GetRoleParamByName(dbRoleInfo, key);
					if (!string.IsNullOrEmpty(oldParamValueStr))
					{
						oldFields = oldParamValueStr.Split(new char[]
						{
							','
						});
						if (oldFields.Length == 5 && oldFields[0] == "1")
						{
							oldParamValue = oldParamValueStr;
						}
					}
					string paramValue;
					if (string.IsNullOrEmpty(oldParamValue))
					{
						paramValue = string.Format("1,{0},{1},{2},0", beginOffsetDay, beginOffsetDay + 30, beginOffsetDay);
					}
					else
					{
						paramValue = string.Format("{0},{1},{2},{3},{4}", new object[]
						{
							oldFields[0],
							oldFields[1],
							Convert.ToInt32(oldFields[2]) + 30,
							oldFields[3],
							oldFields[4]
						});
					}
					Global.UpdateRoleParamByName(dbMgr, dbRoleInfo, key, paramValue, null);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 0), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0006AA80 File Offset: 0x00068C80
		private static TCPProcessCmdResults ProcessRegUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int serverLineID = Convert.ToInt32(fields[1]);
				int state = Convert.ToInt32(fields[2]);
				int ret = 1;
				long logoutServerTicks = 0L;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (dbUserInfo == null)
				{
					ret = -10;
				}
				else
				{
					ret = 1;
					if (!UserOnlineManager.RegisterUserID(userID, serverLineID, state))
					{
						ret = 0;
					}
					else if (state == 0)
					{
						lock (dbUserInfo)
						{
							logoutServerTicks = dbUserInfo.LogoutServerTicks;
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", ret, logoutServerTicks);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0006AC84 File Offset: 0x00068E84
		private static TCPProcessCmdResults ProcessGetRoleListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int zoneID = Convert.ToInt32(fields[1]);
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				string strcmd;
				if (null == dbUserInfo)
				{
					strcmd = string.Format("{0}:{1}", 0, "");
				}
				else
				{
					int nRoleCount = 0;
					string roleList = "";
					lock (dbUserInfo)
					{
						for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
						{
							if (dbUserInfo.ListRoleZoneIDs[i] == zoneID)
							{
								int PreDelLeftSeconds = GameDBManager.PreDelRoleMgr.CalcPreDeleteRoleLeftSeconds(dbUserInfo.ListRolePreRemoveTime[i]);
								nRoleCount++;
								roleList += string.Format("{0}${1}${2}${3}${4}${5}${6}|", new object[]
								{
									dbUserInfo.ListRoleIDs[i],
									dbUserInfo.ListRoleSexes[i],
									dbUserInfo.ListRoleOccups[i],
									dbUserInfo.ListRoleNames[i],
									dbUserInfo.ListRoleLevels[i],
									dbUserInfo.ListRoleChangeLifeCount[i],
									PreDelLeftSeconds
								});
							}
						}
					}
					roleList = roleList.Trim(new char[]
					{
						'|'
					});
					strcmd = string.Format("{0}:{1}", nRoleCount, roleList);
				}
				byte[] bytesCmd = new UTF8Encoding().GetBytes(strcmd);
				tcpOutPacket = pool.Pop();
				tcpOutPacket.PacketCmdID = 101;
				tcpOutPacket.FinalWriteData(bytesCmd, 0, bytesCmd.Length);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0006AFB8 File Offset: 0x000691B8
		private static TCPProcessCmdResults ProcessGetRandomNameNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int sex = Convert.ToInt32(fields[0]);
				string preName = PreNamesManager.GetRandomName(sex);
				string strcmd = string.Format("{0}:{1}", sex, preName);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0006B0F4 File Offset: 0x000692F4
		private static TCPProcessCmdResults ProcessCreateRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 7 && fields.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				string userName = fields[1];
				int sex = Convert.ToInt32(fields[2]);
				int occup = Convert.ToInt32(fields[3]);
				string rolename = fields[4].Split(new char[]
				{
					'$'
				})[0];
				int zoneID = Convert.ToInt32(fields[5]);
				int nMagicSwordParam = Convert.ToInt32(fields[6]);
				bool bIsCanCreateMagicSword = false;
				bool bIsCanCreateSummoner = false;
				if (occup == 5 && fields.Length == 8)
				{
					bIsCanCreateSummoner = (Global.SafeConvertToInt32(fields[7], 10) > 0);
				}
				string strcmd;
				if (DBWriter.CheckRoleCountFull(dbMgr))
				{
					strcmd = string.Format("{0}:{1}", -2, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，服务器角色已满，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, rolename))
				{
					strcmd = string.Format("{0}:{1}", -3, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，角色名在数据库中乱码，CMD={0}, RoleName={1}", (TCPGameServerCmds)nID, rolename), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(rolename) || dbMgr.IsRolenameExist(rolename))
				{
					strcmd = string.Format("{0}:{1}", -1, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，角色重名，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nInitLevel = 1;
				int nInitChangeLifeCount = 0;
				int totalRoleCount = 0;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null != dbUserInfo)
				{
					lock (dbUserInfo)
					{
						for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
						{
							if (dbUserInfo.ListRoleZoneIDs[i] == zoneID)
							{
								if (occup == 3)
								{
									if (!bIsCanCreateMagicSword)
									{
										if (dbUserInfo.ListRoleChangeLifeCount[i] > 3 || (dbUserInfo.ListRoleChangeLifeCount[i] == 3 && dbUserInfo.ListRoleLevels[i] >= 1))
										{
											bIsCanCreateMagicSword = true;
											nInitLevel = 1;
											nInitChangeLifeCount = 2;
										}
									}
								}
								else if (occup == 5)
								{
									if (!bIsCanCreateSummoner)
									{
										if (dbUserInfo.ListRoleChangeLifeCount[i] > 3 || (dbUserInfo.ListRoleChangeLifeCount[i] == 3 && dbUserInfo.ListRoleLevels[i] >= 1))
										{
											bIsCanCreateSummoner = true;
											nInitLevel = 1;
											nInitChangeLifeCount = 2;
										}
									}
								}
								totalRoleCount++;
							}
						}
					}
				}
				if (occup == 3 && !bIsCanCreateMagicSword)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(rolename);
					strcmd = string.Format("{0}:{1}", -5, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("创建魔剑士条件不足，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (occup == 5 && !bIsCanCreateSummoner)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(rolename);
					strcmd = string.Format("{0}:{1}", -5, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("创建召唤师条件不足，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (totalRoleCount >= 4)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(rolename);
					strcmd = string.Format("{0}:{1}", -1000, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Warning, string.Format("添加新角色失败, 数量超过了4个，是否外挂操作，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1000), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (PreNamesManager.SetUsedPreName(rolename))
				{
					DBWriter.UpdatePreNameUsedState(dbMgr, rolename, 1);
				}
				int roleID = DBWriter.CreateRole(dbMgr, userID, userName, sex, occup, rolename, zoneID, 50, 1, nMagicSwordParam, 50);
				if (0 > roleID)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(rolename);
					strcmd = string.Format("{0}:{1}", roleID, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					DBWriter.UpdateRolePBInfo(dbMgr, roleID, 60);
					DBWriter.UpdateRoleRebornStorageInfo(dbMgr, roleID, 60);
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null != dbRoleInfo)
					{
						Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					}
					dbUserInfo = dbMgr.GetDBUserInfo(userID);
					if (null != dbUserInfo)
					{
						lock (dbUserInfo)
						{
							dbUserInfo.ListRoleIDs.Add(roleID);
							dbUserInfo.ListRoleSexes.Add(sex);
							dbUserInfo.ListRoleOccups.Add(occup);
							dbUserInfo.ListRoleNames.Add(rolename);
							dbUserInfo.ListRoleLevels.Add(nInitLevel);
							dbUserInfo.ListRoleZoneIDs.Add(zoneID);
							dbUserInfo.ListRoleChangeLifeCount.Add(nInitChangeLifeCount);
							dbUserInfo.ListRolePreRemoveTime.Add("");
						}
					}
					int nValue = DBQuery.QueryVipLevelAwardFlagInfoByUserID(dbMgr, userID, dbRoleInfo.RoleID, dbRoleInfo.ZoneID);
					if (nValue > 0)
					{
						DBWriter.UpdateVipLevelAwardFlagInfoByRoleID(dbMgr, roleID, nValue, dbRoleInfo.ZoneID);
						dbRoleInfo.VipAwardFlag = nValue;
					}
					strcmd = string.Format("{0}:{1}", 1, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						roleID,
						sex,
						occup,
						rolename,
						nInitLevel,
						nInitChangeLifeCount
					}));
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 102);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0006BAE0 File Offset: 0x00069CE0
		private static TCPProcessCmdResults ProcessUnPreRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int roleID = Convert.ToInt32(fields[1]);
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (dbUserInfo == null || null == dbRoleInfo)
				{
					strcmd = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.PreDelRoleMgr.IfInPreDeleteState(roleID))
				{
					strcmd = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.PreDelRoleMgr.RemovePreDeleteRole(dbUserInfo, dbRoleInfo))
				{
					strcmd = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}", roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 99);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0006BD60 File Offset: 0x00069F60
		private static TCPProcessCmdResults ProcessPreRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int roleID = Convert.ToInt32(fields[1]);
				DateTime Now = DateTime.Now;
				string strcmd;
				if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(roleID))
				{
					strcmd = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 98);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool ret = false;
				bool hasrole = DBQuery.GetUserRole(dbMgr, userID, roleID);
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (dbRoleInfo != null && dbRoleInfo.ZhanDuiZhiWu > 0)
				{
					strcmd = string.Format("{0}:{1}", -4029, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (hasrole)
				{
					ret = DBWriter.PreRemoveRole(dbMgr, roleID, Now);
				}
				if (!ret || dbUserInfo == null || null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 98);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbUserInfo)
				{
					int index = dbUserInfo.ListRoleIDs.IndexOf(roleID);
					if (index >= 0 && index < dbUserInfo.ListRoleIDs.Count)
					{
						dbUserInfo.ListRolePreRemoveTime[index] = Now.ToString();
					}
				}
				GameDBManager.PreDelRoleMgr.AddPreDeleteRole(roleID, Now);
				int PreDelLeftSeconds = GameDBManager.PreDelRoleMgr.CalcPreDeleteRoleLeftSeconds(Now.ToString());
				strcmd = string.Format("{0}:{1}", roleID, PreDelLeftSeconds);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 98);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0006C0D0 File Offset: 0x0006A2D0
		private static TCPProcessCmdResults ProcessRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int roleID = Convert.ToInt32(fields[1]);
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (dbRoleInfo != null && dbRoleInfo.ZhanDuiZhiWu > 0)
				{
					strcmd = string.Format("{0}:{1}", -4029, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool ret = false;
				bool hasrole = DBQuery.GetUserRole(dbMgr, userID, roleID);
				if (hasrole)
				{
					ret = DBWriter.RemoveRole(dbMgr, roleID);
				}
				if (!ret || dbUserInfo == null || null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GameDBManager.PreDelRoleMgr.HandleDeleteRole(dbUserInfo, dbRoleInfo);
				strcmd = string.Format("{0}:{1}", roleID, dbRoleInfo.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 103);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0006C330 File Offset: 0x0006A530
		private static TCPProcessCmdResults ProcessInitGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int roleID = Convert.ToInt32(fields[1]);
				int tempRoleID = Convert.ToInt32(fields[2]);
				string channel = fields[3];
				RoleDataEx roleDataEx = new RoleDataEx();
				bool failed = false;
				if (DBQuery.IsBlackUserID(dbMgr, userID))
				{
					failed = true;
					roleDataEx.RoleID = -70;
					LogManager.WriteLog(LogTypes.Error, string.Format("用户被禁止登陆，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					failed = true;
					roleDataEx.RoleID = -2;
					LogManager.WriteLog(LogTypes.Error, string.Format("获取用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
				}
				else
				{
					bool hasrole = false;
					foreach (int role in dbUserInfo.ListRoleIDs)
					{
						if (role == roleID)
						{
							hasrole = true;
							break;
						}
					}
					if (!hasrole)
					{
						failed = true;
						roleDataEx.RoleID = -2;
						LogManager.WriteLog(LogTypes.Error, string.Format("获取用户角色数据失败，CMD={0}, UserID={1}, RoleID={2}", (TCPGameServerCmds)nID, userID, roleID), null, true);
					}
					else
					{
						lock (dbUserInfo)
						{
							roleDataEx.UserMoney = dbUserInfo.Money;
							roleDataEx.PushMessageID = dbUserInfo.PushMessageID;
						}
						if (tempRoleID > 0)
						{
							SingletonTemplate<RoleMapper>.Instance().SetTempRoleID(roleID, tempRoleID);
						}
					}
				}
				if (dbUserInfo != null && !failed)
				{
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null == dbRoleInfo)
					{
						roleDataEx.RoleID = -1;
						LogManager.WriteLog(LogTypes.Error, string.Format("获取用户数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					}
					else if (BanManager.IsBanRoleName(Global.FormatRoleName(dbRoleInfo)) > 0 || BanManager.IsBanRoleName(dbRoleInfo.RoleID + "$rid") > 0)
					{
						roleDataEx.RoleID = -10;
						LogManager.WriteLog(LogTypes.Error, string.Format("被游戏管理员禁止登陆，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					}
					else if (dbRoleInfo.BanLogin > 0)
					{
						roleDataEx.RoleID = -10;
						LogManager.WriteLog(LogTypes.Error, string.Format("被游戏管理员禁止登陆，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					}
					else if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(roleID))
					{
						roleDataEx.RoleID = -11;
						LogManager.WriteLog(LogTypes.Error, string.Format("角色处在预删除状态，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					}
					else
					{
						dbRoleInfo.Channel = channel;
						dbRoleInfo.LastTime = TimeUtil.NOW();
						if (dbRoleInfo.IsFlashPlayer == 1)
						{
							lock (dbUserInfo)
							{
								int nIndex = -1;
								for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
								{
									if (dbUserInfo.ListRoleIDs[i] == dbRoleInfo.RoleID)
									{
										nIndex = i;
										break;
									}
								}
								DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, roleID);
								dbRoleInfo.Experience = 0L;
								dbRoleInfo.MainTaskID = 0;
								dbRoleInfo.MainQuickBarKeys = "";
								DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, roleID);
								dbRoleInfo.Level = 1;
								dbUserInfo.ListRoleLevels[nIndex] = 1;
								DBWriter.UpdateRoleGoodsForFlashPlayerWhenLogOut(dbMgr, roleID);
								dbRoleInfo.GoodsDataList = new List<GoodsData>();
								DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, roleID);
								dbRoleInfo.DoingTaskList = new List<TaskData>();
								dbRoleInfo.OldTasks = new List<OldTaskData>();
							}
						}
						CacheManager.AddRoleMiniInfo((long)roleID, dbRoleInfo.ZoneID, dbRoleInfo.UserID);
						Global.DBRoleInfo2RoleDataEx(dbRoleInfo, roleDataEx);
						Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
						GlobalEventSource.getInstance().fireEvent(new PlayerLoginEventObject(dbRoleInfo));
						roleDataEx.userMiniData = dbUserInfo.GetUserMiniData(userID, roleID, dbRoleInfo.ZoneID);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0006C918 File Offset: 0x0006AB18
		private static TCPProcessCmdResults ProcessRoleOnLineGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int serverLineID = Convert.ToInt32(fields[1]);
				int loginNum = Convert.ToInt32(fields[2]);
				string ip = fields[3];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayID = DateTime.Now.DayOfYear;
				int loginDayID = 0;
				int loginDayNum = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.ServerLineID = serverLineID;
					dbRoleInfo.LoginNum = loginNum;
					dbRoleInfo.LastTime = DateTime.Now.Ticks / 10000L;
					if (dayID != dbRoleInfo.LoginDayID)
					{
						dbRoleInfo.LoginDayNum++;
						dbRoleInfo.LoginDayID = dayID;
					}
					loginDayID = dbRoleInfo.LoginDayID;
					loginDayNum = dbRoleInfo.LoginDayNum;
					dbRoleInfo.LastIP = ip;
				}
				DBWriter.UpdateRoleLoginInfo(dbMgr, roleID, loginNum, loginDayID, loginDayNum, dbRoleInfo.UserID, dbRoleInfo.ZoneID, ip);
				RoleOnlineManager.UpdateRoleOnlineTicks(roleID);
				DBWriter.InsertCityInfo(dbMgr, ip, dbRoleInfo.UserID);
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0006CBF0 File Offset: 0x0006ADF0
		private static TCPProcessCmdResults ProcessUpdateAccountActiveCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserActiveInfo.getInstance().UpdateAccountActiveInfo(dbMgr, fields[0]);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0006CD28 File Offset: 0x0006AF28
		private static TCPProcessCmdResults ProcessRoleHeartGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RoleOnlineManager.UpdateRoleOnlineTicks(roleID);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0006CEC4 File Offset: 0x0006B0C4
		private static TCPProcessCmdResults ProcessRoleOffLineGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length < 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int serverLineID = Convert.ToInt32(fields[1]);
				string ip = fields[2];
				int activeVal = Convert.ToInt32(fields[3]);
				long logoutServerTicks = 0L;
				if (fields.Length >= 5)
				{
					logoutServerTicks = Convert.ToInt64(fields[4]);
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int pkMode = 0;
				int horseDbID = 0;
				int petDbID = 0;
				int onlineSecs = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.ServerLineID = -1;
					dbRoleInfo.LogOffTime = DateTime.Now.Ticks / 10000L;
					pkMode = dbRoleInfo.PKMode;
					horseDbID = dbRoleInfo.HorseDbID;
					petDbID = dbRoleInfo.PetDbID;
					onlineSecs = Math.Min((int)((dbRoleInfo.LogOffTime - dbRoleInfo.LastTime) / 1000L), 86400);
				}
				dbRoleInfo.RankValue.Clear();
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(dbRoleInfo.UserID);
				if (null != dbUserInfo)
				{
					lock (dbUserInfo)
					{
						dbUserInfo.LogoutServerTicks = logoutServerTicks;
					}
				}
				DBWriter.UpdateRoleLogOff(dbMgr, roleID, dbRoleInfo.UserID, dbRoleInfo.ZoneID, ip, onlineSecs);
				DBWriter.UpdatePKMode(dbMgr, roleID, pkMode);
				DBWriter.UpdateRoleHorse(dbMgr, roleID, horseDbID);
				DBWriter.UpdateRolePet(dbMgr, roleID, petDbID);
				RoleOnlineManager.RemoveRoleOnlineTicks(roleID);
				DBWriter.UpdateCityInfoLogoutTime(dbMgr, ip, dbRoleInfo.UserID, onlineSecs, activeVal);
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0006D22C File Offset: 0x0006B42C
		private static TCPProcessCmdResults ProcessGetServerListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误 , CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string lineToken = fields[0];
				string userID = fields[1];
				int verSign = Convert.ToInt32(fields[2]);
				int rolesCount = 0;
				DBUserInfo dbUserInfo = dbMgr.dbUserMgr.FindDBUserInfo(userID);
				if (null != dbUserInfo)
				{
					lock (dbUserInfo)
					{
						rolesCount = dbUserInfo.ListRoleNames.Count;
					}
				}
				ServerListData serverListData = new ServerListData
				{
					RetCode = 0,
					RolesCount = rolesCount,
					LineDataList = null
				};
				List<LineData> lineDataList = new List<LineData>();
				if (verSign != 20111128)
				{
					serverListData.RetCode = -1;
				}
				else
				{
					List<LineItem> itemList = LineManager.GetLineItemList();
					for (int i = 0; i < itemList.Count; i++)
					{
						lineDataList.Add(Global.LineItemToLineData(itemList[i]));
					}
					serverListData.LineDataList = lineDataList;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ServerListData>(serverListData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0006D470 File Offset: 0x0006B670
		private static TCPProcessCmdResults ProcessOnlineServerHeartCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误 , CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				int serverLineNum = Convert.ToInt32(fields[1]);
				int serverLineCount = Convert.ToInt32(fields[2]);
				if (serverLineCount <= 0)
				{
					UserOnlineManager.ClearUserIDsByServerLineID(serverLineID);
				}
				LineManager.UpdateLineHeart(client, serverLineID, serverLineNum, "");
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0006D5E0 File Offset: 0x0006B7E0
		private static TCPProcessCmdResults ProcessGetServerIdCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] bytes;
			try
			{
				bytes = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			bytes = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x0006D65C File Offset: 0x0006B85C
		private static TCPProcessCmdResults ProcessGetServerPTIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] bytes;
			try
			{
				bytes = DataHelper.ObjectToBytes<int>(GameDBManager.PTID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			bytes = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0006D6D8 File Offset: 0x0006B8D8
		private static TCPProcessCmdResults ProcessGetServerDayDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] bytes;
			try
			{
				int dayid = DataHelper.BytesToObject<int>(data, 0, count);
				ServerDayData serverDayData = new ServerDayData();
				try
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string sql = string.Format("select dayid,cdate,worldlevel from t_server_days where dayid={0}", dayid);
						using (MySQLDataReader reader = conn.ExecuteReader(sql, new MySQLParameter[0]))
						{
							if (reader.Read())
							{
								serverDayData.Dayid = Convert.ToInt32(reader["dayid"].ToString());
								serverDayData.CDate = reader["cdate"].ToString();
								serverDayData.WorldLevel = Convert.ToInt32(reader["worldlevel"].ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				bytes = DataHelper.ObjectToBytes<ServerDayData>(serverDayData);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			bytes = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0006D85C File Offset: 0x0006BA5C
		private static TCPProcessCmdResults ProcessSetServerDayDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] bytes;
			try
			{
				ServerDayData serverDayData = DataHelper.BytesToObject<ServerDayData>(data, 0, count);
				try
				{
					if (null != serverDayData)
					{
						using (MyDbConnection3 conn = new MyDbConnection3(false))
						{
							string sql = string.Format("replace into t_server_days(dayid,cdate,worldlevel) values({0},'{1}',{2})", serverDayData.Dayid, serverDayData.CDate, serverDayData.WorldLevel);
							conn.ExecuteNonQuery(sql, 0);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				bytes = DataHelper.ObjectToBytes<int>(1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			bytes = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0006D96C File Offset: 0x0006BB6C
		private static TCPProcessCmdResults ProcessNewTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int npcID = Convert.ToInt32(fields[1]);
				int taskID = Convert.ToInt32(fields[2]);
				int focus = Convert.ToInt32(fields[3]);
				int nStarLevel = Convert.ToInt32(fields[4]);
				DateTime now = DateTime.Now;
				string today = now.ToString("yyyy-MM-dd HH:mm:ss");
				long ticks = now.Ticks / 10000L;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.NewTask(dbMgr, roleID, npcID, taskID, today, focus, nStarLevel);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						taskID,
						ticks,
						ret
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("添加任务失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.DoingTaskList)
						{
							dbRoleInfo.DoingTaskList = new List<TaskData>();
						}
						dbRoleInfo.DoingTaskList.Add(new TaskData
						{
							DbID = ret,
							DoingTaskID = taskID,
							DoingTaskVal1 = 0,
							DoingTaskVal2 = 0,
							DoingTaskFocus = focus,
							AddDateTime = ticks,
							StarLevel = nStarLevel
						});
					}
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						taskID,
						ticks,
						ret
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0006DCD4 File Offset: 0x0006BED4
		private static TCPProcessCmdResults ProcessUpdatePosCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int mapCode = Convert.ToInt32(fields[1]);
				int direction = Convert.ToInt32(fields[2]);
				int posX = Convert.ToInt32(fields[3]);
				int posY = Convert.ToInt32(fields[4]);
				string strcmd = "";
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long ticks = DateTime.Now.Ticks / 10000L;
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					mapCode,
					direction,
					posX,
					posY
				});
				bool updateDBRolePosition = true;
				lock (dbRoleInfo)
				{
					dbRoleInfo.Position = strcmd;
				}
				bool ret = true;
				if (updateDBRolePosition)
				{
					ret = DBWriter.UpdateRolePosition(dbMgr, roleID, strcmd);
				}
				if (!ret)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色位置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0006DFB4 File Offset: 0x0006C1B4
		private static TCPProcessCmdResults ProcessUpdateExpLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int level = Convert.ToInt32(fields[1]);
				long experience = Convert.ToInt64(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleExpLevel(dbMgr, roleID, level, experience))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色经验和级别失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					string userID = "";
					lock (dbRoleInfo)
					{
						dbRoleInfo.Level = level;
						dbRoleInfo.Experience = experience;
						userID = dbRoleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					if (userID != "")
					{
						DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
						if (null != dbUserInfo)
						{
							lock (dbUserInfo)
							{
								for (int i = 0; i < dbUserInfo.ListRoleLevels.Count; i++)
								{
									if (dbUserInfo.ListRoleIDs[i] == roleID)
									{
										dbUserInfo.ListRoleLevels[i] = level;
									}
								}
							}
						}
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0006E2FC File Offset: 0x0006C4FC
		private static TCPProcessCmdResults ProcessUpdateInterPowerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int interPower = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool updateDBInterPower = true;
				lock (dbRoleInfo)
				{
					dbRoleInfo.InterPower = interPower;
				}
				bool ret = true;
				if (updateDBInterPower)
				{
					ret = DBWriter.UpdateRoleInterPower(dbMgr, roleID, interPower);
				}
				string strcmd;
				if (!ret)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色内力失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x0006E564 File Offset: 0x0006C764
		private static TCPProcessCmdResults ProcessUpdateMoney1Cmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int meney = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleMoney1(dbMgr, roleID, meney))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.Money1 = meney;
					}
					Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					strcmd = string.Format("{0}:{1}", roleID, meney);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x0006E7C0 File Offset: 0x0006C9C0
		private static TCPProcessCmdResults ProcessAddGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 19)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int quality = Convert.ToInt32(fields[3]);
				string props = fields[4];
				int forgeLevel = Convert.ToInt32(fields[5]);
				int binding = Convert.ToInt32(fields[6]);
				int site = Convert.ToInt32(fields[7]);
				string jewelList = fields[8];
				int bagindex = Convert.ToInt32(fields[9]);
				string startTime = fields[10];
				startTime = startTime.Replace("$", ":");
				string endTime = fields[11];
				endTime = endTime.Replace("$", ":");
				int addPropIndex = Convert.ToInt32(fields[12]);
				int bornIndex = Convert.ToInt32(fields[13]);
				int lucky = Convert.ToInt32(fields[14]);
				int strong = Convert.ToInt32(fields[15]);
				int ExcellenceProperty = Convert.ToInt32(fields[16]);
				int nAppendPropLev = Convert.ToInt32(fields[17]);
				int nEquipChangeLife = Convert.ToInt32(fields[18]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.NewGoods(dbMgr, roleID, goodsID, goodsNum, quality, props, forgeLevel, binding, site, jewelList, bagindex, startTime, endTime, addPropIndex, bornIndex, lucky, strong, ExcellenceProperty, nAppendPropLev, nEquipChangeLife);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", ret, cmdData);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色添加新物品失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.GoodsDataList)
						{
							dbRoleInfo.GoodsDataList = new List<GoodsData>();
						}
						dbRoleInfo.GoodsDataList.Add(new GoodsData
						{
							Id = ret,
							GoodsID = goodsID,
							Using = 0,
							Forge_level = forgeLevel,
							Starttime = startTime,
							Endtime = endTime,
							Site = site,
							Quality = quality,
							Props = props,
							GCount = goodsNum,
							Binding = binding,
							Jewellist = jewelList,
							BagIndex = bagindex,
							AddPropIndex = addPropIndex,
							BornIndex = bornIndex,
							Lucky = lucky,
							Strong = strong,
							ExcellenceInfo = ExcellenceProperty,
							AppendPropLev = nAppendPropLev,
							ChangeLifeLevForEquip = nEquipChangeLife
						});
						if (-1000 == site)
						{
							dbRoleInfo.MyPortableBagData.GoodsUsedGridNum++;
						}
					}
					strcmd = string.Format("{0}:{1}", ret, cmdData);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0006EC08 File Offset: 0x0006CE08
		private static TCPProcessCmdResults ProcessUpdateGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 23)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int id = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (null == Global.GetGoodsDataByDbID(dbRoleInfo, id))
				{
					strcmd = string.Format("{0}:{1}", id, -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateGoods(dbMgr, id, fields, 2);
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", id, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新物品数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null != dbRoleInfo.GoodsDataList)
						{
							for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
							{
								if (dbRoleInfo.GoodsDataList[i].Id == id)
								{
									int gcount = DataHelper.ConvertToInt32(fields[9], dbRoleInfo.GoodsDataList[i].GCount);
									if (gcount > 0)
									{
										int newSite = DataHelper.ConvertToInt32(fields[6], dbRoleInfo.GoodsDataList[i].Site);
										if (dbRoleInfo.GoodsDataList[i].Site == 0 && -1000 == newSite)
										{
											dbRoleInfo.MyPortableBagData.GoodsUsedGridNum++;
										}
										else if (dbRoleInfo.GoodsDataList[i].Site != 0 || newSite != -1)
										{
											if (dbRoleInfo.GoodsDataList[i].Site == -1000 && newSite == 0)
											{
												dbRoleInfo.MyPortableBagData.GoodsUsedGridNum--;
											}
											else if (dbRoleInfo.GoodsDataList[i].Site != -1 || newSite != 0)
											{
												if (dbRoleInfo.GoodsDataList[i].Site == 15000 && 15001 == newSite)
												{
													dbRoleInfo.RebornGirdData.GoodsUsedGridNum++;
												}
												else if (dbRoleInfo.GoodsDataList[i].Site != 15000 || newSite != -1)
												{
													if (dbRoleInfo.GoodsDataList[i].Site == 15001 && newSite == 15000)
													{
														dbRoleInfo.RebornGirdData.GoodsUsedGridNum--;
													}
													else if (dbRoleInfo.GoodsDataList[i].Site == -1 && newSite == 15000)
													{
													}
												}
											}
										}
										dbRoleInfo.GoodsDataList[i].Using = DataHelper.ConvertToInt32(fields[2], dbRoleInfo.GoodsDataList[i].Using);
										dbRoleInfo.GoodsDataList[i].Forge_level = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.GoodsDataList[i].Forge_level);
										dbRoleInfo.GoodsDataList[i].Starttime = DataHelper.ConvertToStr(fields[4], dbRoleInfo.GoodsDataList[i].Starttime);
										dbRoleInfo.GoodsDataList[i].Endtime = DataHelper.ConvertToStr(fields[5], dbRoleInfo.GoodsDataList[i].Endtime);
										dbRoleInfo.GoodsDataList[i].Site = newSite;
										dbRoleInfo.GoodsDataList[i].Quality = DataHelper.ConvertToInt32(fields[7], dbRoleInfo.GoodsDataList[i].Quality);
										dbRoleInfo.GoodsDataList[i].Props = DataHelper.ConvertToStr(fields[8], dbRoleInfo.GoodsDataList[i].Props);
										dbRoleInfo.GoodsDataList[i].GCount = gcount;
										dbRoleInfo.GoodsDataList[i].Jewellist = DataHelper.ConvertToStr(fields[10], dbRoleInfo.GoodsDataList[i].Jewellist);
										dbRoleInfo.GoodsDataList[i].BagIndex = DataHelper.ConvertToInt32(fields[11], dbRoleInfo.GoodsDataList[i].BagIndex);
										dbRoleInfo.GoodsDataList[i].SaleMoney1 = DataHelper.ConvertToInt32(fields[12], dbRoleInfo.GoodsDataList[i].SaleMoney1);
										dbRoleInfo.GoodsDataList[i].SaleYuanBao = DataHelper.ConvertToInt32(fields[13], dbRoleInfo.GoodsDataList[i].SaleYuanBao);
										dbRoleInfo.GoodsDataList[i].SaleYinPiao = DataHelper.ConvertToInt32(fields[14], dbRoleInfo.GoodsDataList[i].SaleYinPiao);
										dbRoleInfo.GoodsDataList[i].Binding = DataHelper.ConvertToInt32(fields[15], dbRoleInfo.GoodsDataList[i].Binding);
										dbRoleInfo.GoodsDataList[i].AddPropIndex = DataHelper.ConvertToInt32(fields[16], dbRoleInfo.GoodsDataList[i].AddPropIndex);
										dbRoleInfo.GoodsDataList[i].BornIndex = DataHelper.ConvertToInt32(fields[17], dbRoleInfo.GoodsDataList[i].BornIndex);
										dbRoleInfo.GoodsDataList[i].Lucky = DataHelper.ConvertToInt32(fields[18], dbRoleInfo.GoodsDataList[i].Lucky);
										dbRoleInfo.GoodsDataList[i].Strong = DataHelper.ConvertToInt32(fields[19], dbRoleInfo.GoodsDataList[i].Strong);
										dbRoleInfo.GoodsDataList[i].ExcellenceInfo = DataHelper.ConvertToInt32(fields[20], dbRoleInfo.GoodsDataList[i].ExcellenceInfo);
										dbRoleInfo.GoodsDataList[i].AppendPropLev = DataHelper.ConvertToInt32(fields[21], dbRoleInfo.GoodsDataList[i].AppendPropLev);
										dbRoleInfo.GoodsDataList[i].ChangeLifeLevForEquip = DataHelper.ConvertToInt32(fields[22], dbRoleInfo.GoodsDataList[i].ChangeLifeLevForEquip);
									}
									else
									{
										if (GameDBManager.Flag_t_goods_delete_immediately)
										{
											DBWriter.MoveGoodsDataToBackupTable(dbMgr, id);
										}
										if (dbRoleInfo.GoodsDataList[i].Site == -1000)
										{
											dbRoleInfo.MyPortableBagData.GoodsUsedGridNum--;
										}
										dbRoleInfo.GoodsDataList.RemoveAt(i);
									}
									break;
								}
							}
						}
					}
					strcmd = string.Format("{0}:{1}", id, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0006F534 File Offset: 0x0006D734
		private static TCPProcessCmdResults ProcessUpdateTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int taskID = Convert.ToInt32(fields[1]);
				int dbID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateTask(dbMgr, dbID, fields, 3);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, taskID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新任务数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null != dbRoleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbRoleInfo.DoingTaskList.Count; i++)
							{
								if (dbRoleInfo.DoingTaskList[i].DoingTaskID == taskID)
								{
									dbRoleInfo.DoingTaskList[i].DoingTaskFocus = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.DoingTaskList[i].DoingTaskFocus);
									dbRoleInfo.DoingTaskList[i].DoingTaskVal1 = DataHelper.ConvertToInt32(fields[4], dbRoleInfo.DoingTaskList[i].DoingTaskVal1);
									dbRoleInfo.DoingTaskList[i].DoingTaskVal2 = DataHelper.ConvertToInt32(fields[5], dbRoleInfo.DoingTaskList[i].DoingTaskVal2);
									break;
								}
							}
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, taskID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0006F88C File Offset: 0x0006DA8C
		private static TCPProcessCmdResults ProcessCompleteTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int npcID = Convert.ToInt32(fields[1]);
				int taskID = Convert.ToInt32(fields[2]);
				int dbID = Convert.ToInt32(fields[3]);
				int isMainTask = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.CompleteTask(dbMgr, roleID, npcID, taskID, dbID, isMainTask))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, taskID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("角色完成任务数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					bool needUpdateMainTaskID = false;
					lock (dbRoleInfo)
					{
						if (isMainTask > 0 && taskID > dbRoleInfo.MainTaskID)
						{
							dbRoleInfo.MainTaskID = taskID;
							needUpdateMainTaskID = true;
						}
						if (null != dbRoleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbRoleInfo.DoingTaskList.Count; i++)
							{
								if (dbRoleInfo.DoingTaskList[i].DoingTaskID == taskID)
								{
									dbRoleInfo.DoingTaskList.RemoveAt(i);
									break;
								}
							}
						}
						if (null == dbRoleInfo.OldTasks)
						{
							dbRoleInfo.OldTasks = new List<OldTaskData>();
						}
						int findIndex = -1;
						for (int i = 0; i < dbRoleInfo.OldTasks.Count; i++)
						{
							if (dbRoleInfo.OldTasks[i].TaskID == taskID)
							{
								findIndex = i;
								break;
							}
						}
						if (findIndex >= 0)
						{
							dbRoleInfo.OldTasks[findIndex].DoCount++;
						}
						else
						{
							dbRoleInfo.OldTasks.Add(new OldTaskData
							{
								TaskID = taskID,
								DoCount = 1
							});
						}
					}
					if (needUpdateMainTaskID && isMainTask > 0)
					{
						DBWriter.UpdateRoleMainTaskID(dbMgr, roleID, taskID);
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, taskID, 0);
					if (isMainTask > 0)
					{
						Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0006FCA0 File Offset: 0x0006DEA0
		private static TCPProcessCmdResults ProcessGetFriendsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<FriendData> friendDataList = null;
				friendDataList = new List<FriendData>();
				int i;
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.FriendDataList)
					{
						dbRoleInfo.FriendDataList = new List<FriendData>();
					}
					for (i = 0; i < dbRoleInfo.FriendDataList.Count; i++)
					{
						friendDataList.Add(new FriendData
						{
							DbID = dbRoleInfo.FriendDataList[i].DbID,
							OtherRoleID = dbRoleInfo.FriendDataList[i].OtherRoleID,
							FriendType = dbRoleInfo.FriendDataList[i].FriendType
						});
					}
				}
				List<FriendData> toSendFriendDataList = new List<FriendData>();
				i = 0;
				while (i < friendDataList.Count)
				{
					FriendData friendData = friendDataList[i];
					DBRoleInfo otherDbRoleInfo = dbMgr.FindDBRoleInfo(ref friendData.OtherRoleID);
					if (null != otherDbRoleInfo)
					{
						friendDataList[i].OtherRoleName = Global.FormatRoleName(otherDbRoleInfo);
						friendDataList[i].OtherLevel = otherDbRoleInfo.Level;
						friendDataList[i].Occupation = otherDbRoleInfo.Occupation;
						friendDataList[i].OnlineState = Global.GetRoleOnlineState(otherDbRoleInfo);
						friendDataList[i].Position = otherDbRoleInfo.Position;
						friendDataList[i].FriendChangeLifeLev = otherDbRoleInfo.ChangeLifeCount;
						friendDataList[i].FriendCombatForce = otherDbRoleInfo.CombatForce;
						friendDataList[i].SpouseId = ((otherDbRoleInfo.MyMarriageData != null) ? otherDbRoleInfo.MyMarriageData.nSpouseID : 0);
						friendDataList[i].ZhanDuiID = otherDbRoleInfo.ZhanDuiID;
						goto IL_427;
					}
					if (DBQuery.GetFriendData(dbMgr, friendData))
					{
						goto IL_427;
					}
					if (!DBWriter.RemoveFriend(dbMgr, friendData.DbID, roleID))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("删除好友时失败，CMD={0}, RoleID={1}, friendDbID={2}", (TCPGameServerCmds)nID, roleID, friendData.DbID), null, true);
					}
					else
					{
						lock (dbRoleInfo)
						{
							if (null == dbRoleInfo.FriendDataList)
							{
								dbRoleInfo.FriendDataList = new List<FriendData>();
							}
							int findIndex = -1;
							for (int loop = 0; loop < dbRoleInfo.FriendDataList.Count; loop++)
							{
								if (dbRoleInfo.FriendDataList[loop].DbID == friendData.DbID)
								{
									findIndex = loop;
									break;
								}
							}
							if (findIndex >= 0)
							{
								dbRoleInfo.FriendDataList.RemoveAt(findIndex);
							}
						}
					}
					string gmCmdData = string.Format("-removefriend {0} {1}", roleID, friendData.DbID);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
					IL_439:
					i++;
					continue;
					IL_427:
					toSendFriendDataList.Add(friendDataList[i]);
					goto IL_439;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<FriendData>>(toSendFriendDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x000701A8 File Offset: 0x0006E3A8
		private static TCPProcessCmdResults ProcessAddFriendCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dbID = Convert.ToInt32(fields[0]);
				int roleID = Convert.ToInt32(fields[1]);
				string otherName = fields[2];
				int friendType = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool alreadyExists = false;
				int type0Count = 0;
				int type1Count = 0;
				int type2Count = 0;
				List<FriendData> findFriendDataList = new List<FriendData>();
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.FriendDataList)
					{
						for (int i = 0; i < dbRoleInfo.FriendDataList.Count; i++)
						{
							findFriendDataList.Add(dbRoleInfo.FriendDataList[i]);
						}
					}
				}
				for (int i = 0; i < findFriendDataList.Count; i++)
				{
					string existsOtherRoleName = string.Empty;
					DBRoleInfo otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref findFriendDataList[i].OtherRoleID);
					if (null != otherDbRoleInfo)
					{
						existsOtherRoleName = Global.FormatRoleName(otherDbRoleInfo);
					}
					if (!string.IsNullOrEmpty(existsOtherRoleName) && existsOtherRoleName == otherName && findFriendDataList[i].FriendType == friendType)
					{
						alreadyExists = true;
					}
					if (findFriendDataList[i].FriendType == 0)
					{
						type0Count++;
					}
					else if (findFriendDataList[i].FriendType == 1)
					{
						type1Count++;
					}
					else
					{
						type2Count++;
					}
				}
				bool canAdded = !alreadyExists;
				if (canAdded)
				{
					if (friendType == 0)
					{
						if (type0Count >= 50)
						{
							canAdded = false;
						}
					}
					else if (friendType == 1)
					{
						if (type1Count >= 20)
						{
							canAdded = false;
						}
					}
					else if (type2Count >= 20)
					{
						canAdded = false;
					}
				}
				FriendData friendData;
				if (canAdded)
				{
					int otherID = dbMgr.DBRoleMgr.FindDBRoleID(otherName);
					if (-1 == otherID)
					{
						friendData = new FriendData
						{
							DbID = -10000,
							OtherRoleID = 0,
							OtherRoleName = "",
							OtherLevel = 1,
							Occupation = 0,
							OnlineState = 0,
							Position = "",
							FriendType = friendType
						};
						LogManager.WriteLog(LogTypes.Error, string.Format("添加好友找有时查找对方角色ID失败，CMD={0}, RoleID={1}, OtherName={2}", (TCPGameServerCmds)nID, roleID, otherName), null, true);
					}
					else
					{
						int ret = DBWriter.AddFriend(dbMgr, dbID, roleID, otherID, friendType);
						if (ret < 0)
						{
							friendData = new FriendData
							{
								DbID = ret,
								OtherRoleID = 0,
								OtherRoleName = "",
								OtherLevel = 1,
								Occupation = 0,
								OnlineState = 0,
								Position = "",
								FriendType = 0
							};
							LogManager.WriteLog(LogTypes.Error, string.Format("添加好友到数据库失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						}
						else
						{
							lock (dbRoleInfo)
							{
								if (null == dbRoleInfo.FriendDataList)
								{
									dbRoleInfo.FriendDataList = new List<FriendData>();
								}
								int findIndex = -1;
								for (int i = 0; i < dbRoleInfo.FriendDataList.Count; i++)
								{
									if (dbRoleInfo.FriendDataList[i].DbID == ret)
									{
										findIndex = i;
										break;
									}
								}
								if (findIndex >= 0)
								{
									dbRoleInfo.FriendDataList[findIndex].OtherRoleID = otherID;
									dbRoleInfo.FriendDataList[findIndex].FriendType = friendType;
								}
								else
								{
									dbRoleInfo.FriendDataList.Add(new FriendData
									{
										DbID = ret,
										OtherRoleID = otherID,
										FriendType = friendType
									});
								}
							}
							DBRoleInfo otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref otherID);
							if (null == otherDbRoleInfo)
							{
								friendData = new FriendData
								{
									DbID = -10000,
									OtherRoleID = 0,
									OtherRoleName = "",
									OtherLevel = 1,
									Occupation = 0,
									OnlineState = 0,
									Position = "",
									FriendType = friendType
								};
							}
							else
							{
								friendData = new FriendData
								{
									DbID = ret,
									OtherRoleID = otherDbRoleInfo.RoleID,
									OtherRoleName = Global.FormatRoleName(otherDbRoleInfo),
									OtherLevel = otherDbRoleInfo.Level,
									Occupation = otherDbRoleInfo.Occupation,
									OnlineState = Global.GetRoleOnlineState(otherDbRoleInfo),
									Position = otherDbRoleInfo.Position,
									FriendType = friendType,
									FriendChangeLifeLev = otherDbRoleInfo.ChangeLifeCount,
									FriendCombatForce = otherDbRoleInfo.CombatForce,
									SpouseId = ((otherDbRoleInfo.MyMarriageData != null) ? otherDbRoleInfo.MyMarriageData.nSpouseID : 0)
								};
							}
						}
					}
				}
				else
				{
					friendData = new FriendData
					{
						DbID = (alreadyExists ? -10002 : -10001),
						OtherRoleID = 0,
						OtherRoleName = "",
						OtherLevel = 1,
						Occupation = 0,
						OnlineState = 0,
						Position = "",
						FriendType = friendType
					};
					LogManager.WriteLog(LogTypes.Error, string.Format("添加好友时已经存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FriendData>(friendData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00070958 File Offset: 0x0006EB58
		private static TCPProcessCmdResults ProcessRemoveFriendCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dbID = Convert.ToInt32(fields[0]);
				int roleID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.RemoveFriend(dbMgr, dbID, roleID))
				{
					strcmd = string.Format("{0}:{1}:{2}", dbID, roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除好友时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.FriendDataList)
						{
							dbRoleInfo.FriendDataList = new List<FriendData>();
						}
						int findIndex = -1;
						for (int i = 0; i < dbRoleInfo.FriendDataList.Count; i++)
						{
							if (dbRoleInfo.FriendDataList[i].DbID == dbID)
							{
								findIndex = i;
								break;
							}
						}
						if (findIndex >= 0)
						{
							dbRoleInfo.FriendDataList.RemoveAt(findIndex);
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", dbID, roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x00070C3C File Offset: 0x0006EE3C
		private static TCPProcessCmdResults ProcessUpdatePKModeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int pkMode = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!true)
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, pkMode, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色PK模式时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.PKMode = pkMode;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, pkMode, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x00070E94 File Offset: 0x0006F094
		private static TCPProcessCmdResults ProcessUpdatePKValCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int pkValue = Convert.ToInt32(fields[1]);
				int pkPoint = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdatePKValues(dbMgr, roleID, pkValue, pkPoint))
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						pkValue,
						pkPoint,
						-1
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色PK值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.PKValue = pkValue;
						dbRoleInfo.PKPoint = pkPoint;
					}
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						pkValue,
						pkPoint,
						0
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x0007114C File Offset: 0x0006F34C
		private static TCPProcessCmdResults ProcessAbandonTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				int taskID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.DeleteTask(dbMgr, roleID, taskID, dbID))
				{
					strcmd = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除任务时失败，CMD={0}, RoleID={1}, TaskID={2}", (TCPGameServerCmds)nID, roleID, taskID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null != dbRoleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbRoleInfo.DoingTaskList.Count; i++)
							{
								if (dbRoleInfo.DoingTaskList[i].DoingTaskID == taskID)
								{
									dbRoleInfo.DoingTaskList.RemoveAt(i);
									break;
								}
							}
						}
					}
					strcmd = string.Format("{0}", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00071408 File Offset: 0x0006F608
		private static TCPProcessCmdResults ProcessGMSetTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				List<int> taskList = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (taskList != null && taskList.Count >= 2)
				{
					int roleID = taskList[0];
					int taskID = taskList[taskList.Count - 1];
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null != dbRoleInfo)
					{
						lock (dbRoleInfo)
						{
							DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, roleID);
							DBWriter.GMSetTask(dbMgr, roleID, taskID, taskList);
							DBWriter.UpdateRoleMainTaskID(dbMgr, roleID, taskID);
							dbRoleInfo.MainTaskID = taskID;
							dbRoleInfo.OldTasks = new List<OldTaskData>();
							dbRoleInfo.DoingTaskList = new List<TaskData>();
							for (int i = 1; i < taskList.Count; i++)
							{
								dbRoleInfo.OldTasks.Add(new OldTaskData
								{
									TaskID = taskList[i],
									DoCount = 1
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			byte[] bytes = DataHelper.ObjectToBytes<int>(0);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00071588 File Offset: 0x0006F788
		private static TCPProcessCmdResults ProcessModKeysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int type = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string keys = fields[2];
				string strcmd;
				if (!DBWriter.UpdateRoleKeys(dbMgr, roleID, type, keys))
				{
					strcmd = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色映射键时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (type == 0)
						{
							dbRoleInfo.MainQuickBarKeys = keys;
						}
						else
						{
							dbRoleInfo.OtherQuickBarKeys = keys;
						}
					}
					strcmd = string.Format("{0}", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x000717F4 File Offset: 0x0006F9F4
		private static TCPProcessCmdResults ProcessUpdateUserMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2 && fields.Length != 3 && fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserMoney = Convert.ToInt32(fields[1]);
				int activeid = 0;
				string param = "";
				if (fields.Length >= 3)
				{
					activeid = Global.SafeConvertToInt32(fields[2], 10);
				}
				if (fields.Length >= 4)
				{
					param = fields[3];
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				string strcmd;
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = activeid;
				if (num == 23)
				{
					DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
					DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
					int hasgettimes = 0;
					string huoDongKeyStr = param;
					string lastgettime = "";
					DBQuery.GetAwardHistoryForUser(dbMgr, dbRoleInfo.UserID, 23, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						int currday = Global.GetOffsetDay(DateTime.Now);
						if (Global.GetOffsetDay(DateTime.Parse(lastgettime)) == currday)
						{
							strcmd = string.Format("{0}:{1}", roleID, -5);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					hasgettimes++;
					lock (dbRoleInfo)
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbRoleInfo.UserID, activeid, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbRoleInfo.UserID, activeid, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						}
						if (ret < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("更新玩家合服充值返利领取记录失败！！！！！！！！，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						}
					}
				}
				bool failed = false;
				int userMoney = 0;
				lock (dbUserInfo)
				{
					dbUserInfo.Money += addOrSubUserMoney;
					userMoney = dbUserInfo.Money;
					if (failed)
					{
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (addOrSubUserMoney != 0)
					{
						if (!DBWriter.UpdateUserInfo(dbMgr, dbUserInfo))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的元宝失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
							strcmd = string.Format("{0}:{1}", roleID, -4);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				int nUserMoney = 0;
				int nRealMoney = 0;
				DBQuery.QueryUserMoneyByUserID(dbMgr, dbUserInfo.UserID, out nUserMoney, out nRealMoney);
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				strcmd = string.Format("{0}:{1}:{2}", roleID, userMoney, nRealMoney);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00071D64 File Offset: 0x0006FF64
		private static TCPProcessCmdResults ProcessUpdateUserYinLiangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserYinLiang = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				int userYinLiang = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.YinLiang += addOrSubUserYinLiang;
					userYinLiang = dbRoleInfo.YinLiang;
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserYinLiang != 0)
				{
					if (!DBWriter.UpdateRoleYinLiang(dbMgr, roleID, userYinLiang))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色银两失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				strcmd = string.Format("{0}:{1}", roleID, userYinLiang);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0007202C File Offset: 0x0007022C
		private static TCPProcessCmdResults ProcessMoveGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3 && fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsOwnerRoleID = Convert.ToInt32(fields[1]);
				int goodsDbID = Convert.ToInt32(fields[2]);
				int site = 0;
				if (fields.Length == 4)
				{
					site = Convert.ToInt32(fields[3]);
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时查找角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						goodsOwnerRoleID,
						goodsDbID,
						-1
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo2 = dbMgr.GetDBRoleInfo(ref goodsOwnerRoleID);
				if (null == dbRoleInfo2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时查找物品拥有者角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, goodsOwnerRoleID), null, true);
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						goodsOwnerRoleID,
						goodsDbID,
						-2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (null == Global.GetGoodsDataByDbID(dbRoleInfo2, goodsDbID))
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						goodsOwnerRoleID,
						goodsDbID,
						-1000
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.MoveGoods(dbMgr, roleID, goodsDbID, goodsOwnerRoleID, site);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时修改数据库失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						goodsOwnerRoleID,
						goodsDbID,
						ret
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GoodsData gd = null;
				lock (dbRoleInfo2)
				{
					if (null != dbRoleInfo2.GoodsDataList)
					{
						for (int i = 0; i < dbRoleInfo2.GoodsDataList.Count; i++)
						{
							if (dbRoleInfo2.GoodsDataList[i].Id == goodsDbID)
							{
								gd = dbRoleInfo2.GoodsDataList[i];
								dbRoleInfo2.GoodsDataList.Remove(gd);
								gd.Site = site;
								break;
							}
						}
					}
				}
				if (null == gd)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						goodsOwnerRoleID,
						goodsDbID,
						-1000
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.GoodsDataList)
					{
						dbRoleInfo.GoodsDataList = new List<GoodsData>();
					}
					dbRoleInfo.GoodsDataList.Add(gd);
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					goodsOwnerRoleID,
					goodsDbID,
					0
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0007259C File Offset: 0x0007079C
		private static TCPProcessCmdResults ProcessUpdateLeftFightSecsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int leftFightSecs = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleLeftFightSecs(dbMgr, roleID, leftFightSecs))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色的剩余挂机时间时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.LeftFightSeconds = leftFightSecs;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x000727F0 File Offset: 0x000709F0
		private static TCPProcessCmdResults ProcessQueryNameByIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string otherName = fields[1];
				int opCode = Convert.ToInt32(fields[2]);
				int myServerLineID = -1;
				if (roleID > 0)
				{
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null == dbRoleInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					myServerLineID = dbRoleInfo.ServerLineID;
				}
				int onlineState = -1;
				int otherRoleID = dbMgr.DBRoleMgr.FindDBRoleID(otherName);
				if (otherRoleID == -1)
				{
					DBRoleInfo otherDdbRoleInfo = dbMgr.GetDBRoleInfo(otherName);
					if (otherDdbRoleInfo != null)
					{
						otherRoleID = otherDdbRoleInfo.RoleID;
					}
				}
				if (-1 != otherRoleID)
				{
					DBRoleInfo otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
					if (null != otherDbRoleInfo)
					{
						int roleOnlineState = Global.GetRoleOnlineState(otherDbRoleInfo);
						if (1 == roleOnlineState)
						{
							onlineState = 0;
							if (otherDbRoleInfo.ServerLineID != myServerLineID)
							{
								onlineState = otherDbRoleInfo.ServerLineID;
							}
						}
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					roleID,
					otherName,
					opCode,
					otherRoleID,
					onlineState
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00072A94 File Offset: 0x00070C94
		private static TCPProcessCmdResults ProcessQueryUserMoneyByNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string otherName = fields[1];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				string strcmd;
				if (userID == "")
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						userID,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int inputMoneyInPeriod = DBQuery.GetUserInputMoney(dbMgr, userID, dbRoleInfo.ZoneID, "2000-01-01 00:00:00", "2050-01-01 00:00:00");
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = Global.TransMoneyToYuanBao(inputMoneyInPeriod);
				int userMoney = roleYuanBaoInPeriod;
				int realMoney = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					userID,
					userMoney,
					realMoney
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00072D20 File Offset: 0x00070F20
		private static TCPProcessCmdResults ProcessSpriteChatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (Global.ProcessGMMsg(fields))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (fields.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[8]);
				List<LineItem> itemList = LineManager.GetLineItemList();
				if (null != itemList)
				{
					for (int i = 0; i < itemList.Count; i++)
					{
						if (itemList[i].LineID != serverLineID)
						{
							if (serverLineID == 0)
							{
								ChatMsgManager.AddChatMsg(itemList[i].LineID, cmdData);
							}
							else if (serverLineID == -1000)
							{
								if (TCPManager.CurrentClient != null && TCPManager.CurrentClient.LineId != itemList[i].LineID)
								{
									ChatMsgManager.AddChatMsg(itemList[i].LineID, cmdData);
								}
							}
							else if (itemList[i].LineID < 9000)
							{
								ChatMsgManager.AddChatMsg(itemList[i].LineID, cmdData);
							}
						}
					}
				}
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00072FA0 File Offset: 0x000711A0
		private static TCPProcessCmdResults ProcessGetChatMsgListCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				int serverLineNum = Convert.ToInt32(fields[1]);
				int serverLineCount = Convert.ToInt32(fields[2]);
				if (serverLineCount <= 0)
				{
					UserOnlineManager.ClearUserIDsByServerLineID(serverLineID);
				}
				LineManager.UpdateLineHeart(client, serverLineID, serverLineNum, fields[3]);
				tcpOutPacket = ChatMsgManager.GetWaitingChatMsg(pool, nID, serverLineID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x000730F8 File Offset: 0x000712F8
		private static TCPProcessCmdResults ProcessAddHorseCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int horseID = Convert.ToInt32(fields[1]);
				int bodyID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string today = now.ToString("yyyy-MM-dd HH:mm:ss");
				long ticks = now.Ticks / 10000L;
				HorseData horseData = null;
				int ret = DBWriter.NewHorse(dbMgr, roleID, horseID, bodyID, today);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的坐骑失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.HorsesDataList)
						{
							dbRoleInfo.HorsesDataList = new List<HorseData>();
						}
						horseData = new HorseData
						{
							DbID = ret,
							HorseID = horseID,
							BodyID = bodyID,
							AddDateTime = ticks
						};
						dbRoleInfo.HorsesDataList.Add(horseData);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horseData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x000733AC File Offset: 0x000715AC
		private static TCPProcessCmdResults ProcessAddPetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int petID = Convert.ToInt32(fields[1]);
				string petName = fields[2];
				int petType = Convert.ToInt32(fields[3]);
				string props = fields[4];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string today = now.ToString("yyyy-MM-dd HH:mm:ss");
				long ticks = now.Ticks / 10000L;
				PetData petData = null;
				int ret = DBWriter.NewPet(dbMgr, roleID, petID, petName, petType, props, today);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的宠物失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.PetsDataList)
						{
							dbRoleInfo.PetsDataList = new List<PetData>();
						}
						petData = new PetData
						{
							DbID = ret,
							PetID = petID,
							PetName = petName,
							PetType = petType,
							FeedNum = 0,
							ReAliveNum = 0,
							AddDateTime = ticks,
							PetProps = props
						};
						dbRoleInfo.PetsDataList.Add(petData);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PetData>(petData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00073690 File Offset: 0x00071890
		private static TCPProcessCmdResults ProcessGetHorseListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<HorseData> horsesDataList = null;
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.HorsesDataList)
					{
						horsesDataList = dbRoleInfo.HorsesDataList.GetRange(0, dbRoleInfo.HorsesDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(horsesDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00073890 File Offset: 0x00071A90
		private static TCPProcessCmdResults ProcessGetOtherHorseListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int otherRoleID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<HorseData> horsesDataList = null;
				DBRoleInfo otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
				if (null == otherDbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(horsesDataList, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (otherDbRoleInfo)
				{
					if (null != otherDbRoleInfo.HorsesDataList)
					{
						horsesDataList = otherDbRoleInfo.HorsesDataList.GetRange(0, otherDbRoleInfo.HorsesDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(horsesDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x00073ACC File Offset: 0x00071CCC
		private static TCPProcessCmdResults ProcessGetPetListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PetData> petsDataList = null;
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.PetsDataList)
					{
						petsDataList = dbRoleInfo.PetsDataList.GetRange(0, dbRoleInfo.PetsDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<PetData>>(petsDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x00073CCC File Offset: 0x00071ECC
		private static TCPProcessCmdResults ProcessModHorseCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 11)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				HorseData horseData = null;
				int ret = DBWriter.UpdateHorse(dbMgr, dbID, fields, 2);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新时坐骑失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null != dbRoleInfo.HorsesDataList)
						{
							for (int i = 0; i < dbRoleInfo.HorsesDataList.Count; i++)
							{
								if (dbRoleInfo.HorsesDataList[i].DbID == dbID)
								{
									int isdel = DataHelper.ConvertToInt32(fields[2], 0);
									if (isdel <= 0)
									{
										dbRoleInfo.HorsesDataList[i].HorseID = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.HorsesDataList[i].HorseID);
										dbRoleInfo.HorsesDataList[i].BodyID = DataHelper.ConvertToInt32(fields[4], dbRoleInfo.HorsesDataList[i].BodyID);
										dbRoleInfo.HorsesDataList[i].PropsNum = DataHelper.ConvertToStr(fields[5], dbRoleInfo.HorsesDataList[i].PropsNum);
										dbRoleInfo.HorsesDataList[i].PropsVal = DataHelper.ConvertToStr(fields[6], dbRoleInfo.HorsesDataList[i].PropsVal);
										dbRoleInfo.HorsesDataList[i].JinJieFailedNum = DataHelper.ConvertToInt32(fields[7], dbRoleInfo.HorsesDataList[i].JinJieFailedNum);
										dbRoleInfo.HorsesDataList[i].JinJieTempTime = DataHelper.ConvertToTicks(fields[8], dbRoleInfo.HorsesDataList[i].JinJieTempTime);
										dbRoleInfo.HorsesDataList[i].JinJieTempNum = DataHelper.ConvertToInt32(fields[9], dbRoleInfo.HorsesDataList[i].JinJieTempNum);
										dbRoleInfo.HorsesDataList[i].JinJieFailedDayID = DataHelper.ConvertToInt32(fields[10], dbRoleInfo.HorsesDataList[i].JinJieFailedDayID);
										horseData = dbRoleInfo.HorsesDataList[i];
									}
									else
									{
										horseData = dbRoleInfo.HorsesDataList[i];
										horseData.HorseID = -1;
										dbRoleInfo.HorsesDataList.RemoveAt(i);
									}
									break;
								}
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horseData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00074118 File Offset: 0x00072318
		private static TCPProcessCmdResults ProcessModPetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 10)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				PetData petData = Global.GetPetDataByDbID(dbRoleInfo, dbID);
				if (null != petData)
				{
					int ret = DBWriter.UpdatePet(dbMgr, dbID, fields, 2);
					if (ret < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新时宠物失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					}
					else
					{
						lock (dbRoleInfo)
						{
							int isdel = DataHelper.ConvertToInt32(fields[7], 0);
							if (isdel <= 0)
							{
								petData.PetName = DataHelper.ConvertToStr(fields[2], petData.PetName);
								petData.PetType = DataHelper.ConvertToInt32(fields[3], petData.PetType);
								petData.FeedNum = DataHelper.ConvertToInt32(fields[4], petData.FeedNum);
								petData.ReAliveNum = DataHelper.ConvertToInt32(fields[5], petData.ReAliveNum);
								petData.PetProps = DataHelper.ConvertToStr(fields[6], petData.PetProps);
								petData.AddDateTime = DataHelper.ConvertToTicks(fields[8], petData.AddDateTime);
								petData.Level = DataHelper.ConvertToInt32(fields[9], petData.Level);
							}
							else
							{
								petData.PetID = -1;
								dbRoleInfo.PetsDataList.Remove(petData);
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PetData>(petData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x00074424 File Offset: 0x00072624
		private static TCPProcessCmdResults ProcessHorseOnCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.HorseDbID = dbID;
					dbRoleInfo.LastHorseID = dbID;
				}
				string strcmd = string.Format("{0}:{1}:{2}", roleID, dbID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x00074638 File Offset: 0x00072838
		private static TCPProcessCmdResults ProcessHorseOffCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.HorseDbID = 0;
				}
				string strcmd = string.Format("{0}:{1}:{2}", roleID, dbID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00074844 File Offset: 0x00072A44
		private static TCPProcessCmdResults ProcessPetOutCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.PetDbID = dbID;
				}
				string strcmd = string.Format("{0}:{1}:{2}", roleID, dbID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00074A50 File Offset: 0x00072C50
		private static TCPProcessCmdResults ProcessPetInCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.PetDbID = 0;
				}
				string strcmd = string.Format("{0}:{1}:{2}", roleID, dbID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00074C5C File Offset: 0x00072E5C
		private static TCPProcessCmdResults ProcessGetGoodsListBySiteCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int site = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<GoodsData> goodsDataList = null;
				lock (dbRoleInfo)
				{
					goodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, site);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(goodsDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00074E44 File Offset: 0x00073044
		private static TCPProcessCmdResults ProcessGetGoodsListBySiteRangeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int siteBegin = Convert.ToInt32(fields[1]);
				int siteEnd = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<GoodsData> goodsDataList = null;
				lock (dbRoleInfo)
				{
					goodsDataList = Global.GetGoodsDataListBySiteRange(dbRoleInfo, siteBegin, siteEnd);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(goodsDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00075038 File Offset: 0x00073238
		private static TCPProcessCmdResults ProcessGetAddDJPointCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int djPoint = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dbID = -1;
				lock (dbRoleInfo)
				{
					dbID = dbRoleInfo.RoleDJPointData.DbID;
					dbRoleInfo.RoleDJPointData.DJPoint += djPoint;
					dbRoleInfo.RoleDJPointData.Total++;
					dbRoleInfo.RoleDJPointData.Wincnt += ((djPoint > 0) ? 1 : 0);
				}
				int ret = DBWriter.AddRoleDJPoint(dbMgr, dbID, roleID, djPoint);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新一个用户角色的战将积分时错误，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (dbRoleInfo.RoleDJPointData.DbID < 0)
						{
							dbRoleInfo.RoleDJPointData.DbID = dbID;
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", roleID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0007532C File Offset: 0x0007352C
		private static TCPProcessCmdResults ProcessGetDJPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DJPointsData djPointsData = new DJPointsData
				{
					SelfDJPointData = Global.GetRoleDJPointData(dbRoleInfo),
					HotDJPointDataList = GameDBManager.SysDJPointsHotList.GetDJPointsHostList(dbMgr)
				};
				for (int i = 0; i < djPointsData.HotDJPointDataList.Count; i++)
				{
					DBRoleInfo otherDbRoleInfo;
					if (roleID == djPointsData.HotDJPointDataList[i].RoleID)
					{
						otherDbRoleInfo = dbRoleInfo;
					}
					else
					{
						otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref djPointsData.HotDJPointDataList[i].RoleID);
					}
					if (null == otherDbRoleInfo)
					{
						djPointsData.HotDJPointDataList[i].djRoleInfoData = null;
					}
					else
					{
						DJRoleInfoData djRoleInfoData = new DJRoleInfoData
						{
							RoleID = ((otherDbRoleInfo != null) ? otherDbRoleInfo.RoleID : -1),
							RoleName = ((otherDbRoleInfo != null) ? otherDbRoleInfo.RoleName : "未知"),
							Level = ((otherDbRoleInfo != null) ? otherDbRoleInfo.Level : 0),
							Occupation = ((otherDbRoleInfo != null) ? otherDbRoleInfo.Occupation : 0),
							OnlineState = Global.GetRoleOnlineState(otherDbRoleInfo)
						};
						djPointsData.HotDJPointDataList[i].djRoleInfoData = djRoleInfoData;
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DJPointsData>(djPointsData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00075608 File Offset: 0x00073808
		private static TCPProcessCmdResults ProcessUpDianJiangLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				int jingMaiBodyLevel = Convert.ToInt32(fields[2]);
				int jingMaiID = Convert.ToInt32(fields[3]);
				int jingMaiLevel = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpRoleJingMai(dbMgr, roleID, dbID, jingMaiBodyLevel, jingMaiID, jingMaiLevel);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入一个用户角色的经脉时错误，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.JingMaiDataList)
						{
							dbRoleInfo.JingMaiDataList = new List<JingMaiData>();
						}
						if (dbID <= 0)
						{
							dbRoleInfo.JingMaiDataList.Add(new JingMaiData
							{
								DbID = ret,
								JingMaiID = jingMaiID,
								JingMaiLevel = jingMaiLevel,
								JingMaiBodyLevel = jingMaiBodyLevel
							});
						}
						else
						{
							for (int i = 0; i < dbRoleInfo.JingMaiDataList.Count; i++)
							{
								if (dbRoleInfo.JingMaiDataList[i].DbID == dbID)
								{
									dbRoleInfo.JingMaiDataList[i].JingMaiLevel = jingMaiLevel;
								}
							}
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", ret, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00075928 File Offset: 0x00073B28
		private static TCPProcessCmdResults ProcessBanRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = fields[0];
				int state = Convert.ToInt32(fields[1]);
				BanManager.BanRoleName(roleName, state);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x00075A6C File Offset: 0x00073C6C
		private static TCPProcessCmdResults ProcessBanRoleChatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = fields[0];
				int banHours = Convert.ToInt32(fields[1]);
				BanChatManager.AddBanRoleName(roleName, banHours);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00075BB0 File Offset: 0x00073DB0
		private static TCPProcessCmdResults ProcessGetBanRoleChatDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				tcpOutPacket = BanChatManager.GetBanChatDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x00075CD0 File Offset: 0x00073ED0
		private static TCPProcessCmdResults ProcessAddBullMsgCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string msgID = fields[0];
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int interval = Global.SafeConvertToInt32(fields[3], 10);
				string bulletinText = fields[4];
				BulletinMsgData bulletinMsgData = GameDBManager.BulletinMsgMgr.AddBulletinMsg(msgID, fromDate, toDate, interval, bulletinText);
				DBWriter.NewBulletinText(dbMgr, msgID, fromDate, toDate, interval, bulletinText);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00075E54 File Offset: 0x00074054
		private static TCPProcessCmdResults ProcessRemoveBullMsgCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string msgID = fields[0];
				GameDBManager.BulletinMsgMgr.RemoveBulletinMsg(msgID);
				DBWriter.RemoveBulletinText(dbMgr, msgID);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x00075F98 File Offset: 0x00074198
		private static TCPProcessCmdResults ProcessGetBullMsgDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				tcpOutPacket = GameDBManager.BulletinMsgMgr.GetBulletinMsgDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x000760C0 File Offset: 0x000742C0
		private static TCPProcessCmdResults ProcessUpdateOnlineTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int totalOnlineSecs = Convert.ToInt32(fields[1]);
				int antiAddictionSecs = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool updateDBTime = true;
				lock (dbRoleInfo)
				{
					dbRoleInfo.TotalOnlineSecs = totalOnlineSecs;
					dbRoleInfo.AntiAddictionSecs = antiAddictionSecs;
				}
				if (updateDBTime)
				{
					DBWriter.UpdateRoleOnlineSecs(dbMgr, roleID, totalOnlineSecs, antiAddictionSecs);
					Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				}
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x000762F0 File Offset: 0x000744F0
		private static TCPProcessCmdResults ProcessGameConfigDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				tcpOutPacket = GameDBManager.GameConfigMgr.GetGameConfigDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x00076418 File Offset: 0x00074618
		private static TCPProcessCmdResults ProcessGameConfigItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string paramName = fields[0];
				string paramValue = fields[1];
				GameDBManager.GameConfigMgr.UpdateGameConfigItem(paramName, paramValue);
				DBWriter.UpdateGameConfig(dbMgr, paramName, paramValue);
				string gmCmdData = string.Format("-config {0} {1}", paramName, paramValue);
				ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00076580 File Offset: 0x00074780
		private static TCPProcessCmdResults ProcessResetBigGuanCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				long biguanTime = Convert.ToInt64(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					dbRoleInfo.BiGuanTime = biguanTime;
				}
				DBWriter.UpdateRoleBiGuanTime(dbMgr, roleID, biguanTime);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00076780 File Offset: 0x00074980
		private static TCPProcessCmdResults ProcessAddSkillCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int skillID = Convert.ToInt32(fields[1]);
				int skillLevel = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddSkill(dbMgr, roleID, skillID, skillLevel);
				if (ret > 0)
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.SkillDataList)
						{
							dbRoleInfo.SkillDataList = new List<SkillData>();
						}
						dbRoleInfo.SkillDataList.Add(new SkillData
						{
							DbID = ret,
							SkillID = skillID,
							SkillLevel = skillLevel,
							UsedNum = 0
						});
					}
				}
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x000769F4 File Offset: 0x00074BF4
		private static TCPProcessCmdResults ProcessUpSkillInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int skillDbID = Convert.ToInt32(fields[1]);
				int skillLevel = Convert.ToInt32(fields[2]);
				int usedNum = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.SkillDataList)
					{
						for (int i = 0; i < dbRoleInfo.SkillDataList.Count; i++)
						{
							if (dbRoleInfo.SkillDataList[i].DbID == skillDbID)
							{
								dbRoleInfo.SkillDataList[i].SkillLevel = skillLevel;
								dbRoleInfo.SkillDataList[i].UsedNum = usedNum;
								break;
							}
						}
					}
				}
				bool ret = DBWriter.UpdateSkillInfo(dbMgr, skillDbID, skillLevel, usedNum);
				string strcmd = string.Format("{0}", ret ? 1 : 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00076C8C File Offset: 0x00074E8C
		private static TCPProcessCmdResults ProcessUpdateJingMaiExpCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int jingMaiExp = Convert.ToInt32(fields[1]);
				int jingMaiExpNum = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalJingMaiExp = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.JingMaiExpNum = jingMaiExpNum;
					dbRoleInfo.TotalJingMaiExp += jingMaiExp;
					totalJingMaiExp = dbRoleInfo.TotalJingMaiExp;
				}
				bool ret = DBWriter.UpdateJingMaiExp(dbMgr, roleID, jingMaiExpNum, totalJingMaiExp);
				string strcmd = string.Format("{0}", ret ? 1 : 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00076EC0 File Offset: 0x000750C0
		private static TCPProcessCmdResults ProcessUpdateSkillIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int defSkillID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleDefSkillID(dbMgr, roleID, defSkillID))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色缺省技能ID失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.DefaultSkillID = defSkillID;
					}
					strcmd = string.Format("{0}:{1}", roleID, defSkillID);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00077114 File Offset: 0x00075314
		private static TCPProcessCmdResults ProcessUpdateJieBiaoInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int jieBiaoDayID = Convert.ToInt32(fields[1]);
				int jieBiaoDayNum = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleJieBiaoInfo(dbMgr, roleID, jieBiaoDayID, jieBiaoDayNum))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色劫镖信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.JieBiaoDayID = jieBiaoDayID;
						dbRoleInfo.JieBiaoDayNum = jieBiaoDayNum;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, jieBiaoDayID, jieBiaoDayNum);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00077384 File Offset: 0x00075584
		private static TCPProcessCmdResults ProcessUpdateAutoDrinkCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int autoLifeV = Convert.ToInt32(fields[1]);
				int autoMagicV = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleAutoDrink(dbMgr, roleID, autoLifeV, autoMagicV))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, -1, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色自动喝药设置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.AutoLifeV = autoLifeV;
						dbRoleInfo.AutoMagicV = autoMagicV;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, autoLifeV, autoMagicV);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x000775FC File Offset: 0x000757FC
		private static TCPProcessCmdResults ProcessUpdateBufferItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bufferID = Convert.ToInt32(fields[1]);
				long startTime = Convert.ToInt64(fields[2]);
				int bufferSecs = Convert.ToInt32(fields[3]);
				long bufferVal = Convert.ToInt64(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleBufferItem(dbMgr, roleID, bufferID, startTime, bufferSecs, bufferVal);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色Buffer项失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.BufferDataList)
						{
							dbRoleInfo.BufferDataList = new List<BufferData>();
						}
						int findIndex = -1;
						for (int i = 0; i < dbRoleInfo.BufferDataList.Count; i++)
						{
							if (dbRoleInfo.BufferDataList[i].BufferID == bufferID)
							{
								findIndex = i;
								break;
							}
						}
						if (-1 == findIndex)
						{
							dbRoleInfo.BufferDataList.Add(new BufferData
							{
								BufferID = bufferID,
								StartTime = startTime,
								BufferSecs = bufferSecs,
								BufferVal = bufferVal,
								BufferType = 0
							});
						}
						else
						{
							dbRoleInfo.BufferDataList[findIndex].BufferID = bufferID;
							dbRoleInfo.BufferDataList[findIndex].StartTime = startTime;
							dbRoleInfo.BufferDataList[findIndex].BufferSecs = bufferSecs;
							dbRoleInfo.BufferDataList[findIndex].BufferVal = bufferVal;
						}
					}
					strcmd = string.Format("{0}:{1}", roleID, ret);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0007798C File Offset: 0x00075B8C
		private static TCPProcessCmdResults ProcessUnDelRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
				}
				string otherRoleName = fields[0];
				DBWriter.UnRemoveRole(dbMgr, otherRoleName);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00077A8C File Offset: 0x00075C8C
		private static TCPProcessCmdResults ProcessDelRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string otherRoleName = fields[0];
				DBWriter.RemoveRoleByName(dbMgr, otherRoleName);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x00077BC4 File Offset: 0x00075DC4
		private static TCPProcessCmdResults ProcessUpdateDailyTaskDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int huanID = Convert.ToInt32(fields[1]);
				string rectime = fields[2];
				int recnum = Convert.ToInt32(fields[3]);
				int taskClass = Convert.ToInt32(fields[4]);
				int extDayID = Convert.ToInt32(fields[5]);
				int extNum = Convert.ToInt32(fields[6]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.MyDailyTaskDataList)
					{
						dbRoleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
					}
					bool found = false;
					DailyTaskData dailyTaskData = null;
					for (int i = 0; i < dbRoleInfo.MyDailyTaskDataList.Count; i++)
					{
						if (dbRoleInfo.MyDailyTaskDataList[i].TaskClass == taskClass)
						{
							found = true;
							dailyTaskData = dbRoleInfo.MyDailyTaskDataList[i];
							break;
						}
					}
					if (!found)
					{
						dailyTaskData = new DailyTaskData();
						dbRoleInfo.MyDailyTaskDataList.Add(dailyTaskData);
					}
					dailyTaskData.HuanID = huanID;
					dailyTaskData.RecTime = rectime;
					dailyTaskData.RecNum = recnum;
					dailyTaskData.TaskClass = taskClass;
					dailyTaskData.ExtDayID = extDayID;
					dailyTaskData.ExtNum = extNum;
				}
				DBWriter.UpdateRoleDailyTaskData(dbMgr, roleID, huanID, rectime, recnum, taskClass, extDayID, extNum);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00077ED8 File Offset: 0x000760D8
		private static TCPProcessCmdResults ProcessUpdateDailyJingMaiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string jmTime = fields[1];
				int jmNum = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.MyDailyJingMaiData)
					{
						dbRoleInfo.MyDailyJingMaiData = new DailyJingMaiData();
					}
					dbRoleInfo.MyDailyJingMaiData.JmTime = jmTime;
					dbRoleInfo.MyDailyJingMaiData.JmNum = jmNum;
				}
				DBWriter.UpdateRoleDailyJingMaiData(dbMgr, roleID, jmTime, jmNum);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x00078120 File Offset: 0x00076320
		private static TCPProcessCmdResults ProcessUpdateNumSkillIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int numSkillID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleNumSkillID(dbMgr, roleID, numSkillID))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色自动喝药设置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.NumSkillID = numSkillID;
					}
					strcmd = string.Format("{0}:{1}", roleID, numSkillID);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x00078374 File Offset: 0x00076574
		private static TCPProcessCmdResults ProcessUpdatePBInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int extGridNum = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRolePBInfo(dbMgr, roleID, extGridNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.MyPortableBagData.ExtGridNum = extGridNum;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x000785D4 File Offset: 0x000767D4
		private static TCPProcessCmdResults ProcessUpdateRoleBagNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int extGridNum = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleBagNum(dbMgr, roleID, extGridNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.BagNum = extGridNum;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00078830 File Offset: 0x00076A30
		private static TCPProcessCmdResults ProcessUpdateHuoDongInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 22)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool existsMyHuodongData = false;
				lock (dbRoleInfo)
				{
					existsMyHuodongData = dbRoleInfo.ExistsMyHuodongData;
				}
				if (!existsMyHuodongData)
				{
					DBWriter.CreateHuoDong(dbMgr, roleID);
					lock (dbRoleInfo)
					{
						dbRoleInfo.ExistsMyHuodongData = true;
					}
				}
				int ret = DBWriter.UpdateHuoDong(dbMgr, roleID, fields, 1);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色送礼活动信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.MyHuodongData.LastWeekID = DataHelper.ConvertToStr(fields[1], dbRoleInfo.MyHuodongData.LastWeekID);
						dbRoleInfo.MyHuodongData.LastDayID = DataHelper.ConvertToStr(fields[2], dbRoleInfo.MyHuodongData.LastDayID);
						dbRoleInfo.MyHuodongData.LoginNum = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.MyHuodongData.LoginNum);
						dbRoleInfo.MyHuodongData.NewStep = DataHelper.ConvertToInt32(fields[4], dbRoleInfo.MyHuodongData.NewStep);
						dbRoleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(fields[5], dbRoleInfo.MyHuodongData.StepTime);
						dbRoleInfo.MyHuodongData.LastMTime = DataHelper.ConvertToInt32(fields[6], dbRoleInfo.MyHuodongData.LastMTime);
						dbRoleInfo.MyHuodongData.CurMID = DataHelper.ConvertToStr(fields[7], dbRoleInfo.MyHuodongData.CurMID);
						dbRoleInfo.MyHuodongData.CurMTime = DataHelper.ConvertToInt32(fields[8], dbRoleInfo.MyHuodongData.CurMTime);
						dbRoleInfo.MyHuodongData.SongLiID = DataHelper.ConvertToInt32(fields[9], dbRoleInfo.MyHuodongData.SongLiID);
						dbRoleInfo.MyHuodongData.LoginGiftState = DataHelper.ConvertToInt32(fields[10], dbRoleInfo.MyHuodongData.LoginGiftState);
						dbRoleInfo.MyHuodongData.OnlineGiftState = DataHelper.ConvertToInt32(fields[11], dbRoleInfo.MyHuodongData.OnlineGiftState);
						dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID = DataHelper.ConvertToInt32(fields[12], dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID);
						dbRoleInfo.MyHuodongData.LastLimitTimeDayID = DataHelper.ConvertToInt32(fields[13], dbRoleInfo.MyHuodongData.LastLimitTimeDayID);
						dbRoleInfo.MyHuodongData.LimitTimeLoginNum = DataHelper.ConvertToInt32(fields[14], dbRoleInfo.MyHuodongData.LimitTimeLoginNum);
						dbRoleInfo.MyHuodongData.LimitTimeGiftState = DataHelper.ConvertToInt32(fields[15], dbRoleInfo.MyHuodongData.LimitTimeGiftState);
						dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep = DataHelper.ConvertToInt32(fields[16], dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep);
						dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = DataHelper.ConvertToInt32(fields[17], dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID);
						dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep = DataHelper.ConvertToInt32(fields[18], dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep);
						dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID = DataHelper.ConvertToInt32(fields[19], dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID);
						dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID = DataHelper.ConvertToStr(fields[20], dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID);
						dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = DataHelper.ConvertToStr(fields[21], dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID);
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00078DD0 File Offset: 0x00076FD0
		private static TCPProcessCmdResults ProcessUpdateInputPointsUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = Convert.ToString(fields[0]);
				int addOrSubUserIPoints = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace("$", ":");
				string toDate = fields[3].Replace("$", ":");
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				string strcmd;
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", userID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 64, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 64, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", userID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", userID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool canNotInputPoints = false;
				int inputPoints = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.InputPoints = 0;
					}
					if (addOrSubUserIPoints < 0 && dbUserInfo.InputPoints < Math.Abs(addOrSubUserIPoints))
					{
						canNotInputPoints = true;
					}
					else
					{
						dbUserInfo.InputPoints = Math.Max(0, dbUserInfo.InputPoints + addOrSubUserIPoints);
						inputPoints = dbUserInfo.InputPoints;
					}
				}
				if (canNotInputPoints)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", userID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserIPoints != 0)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, inputPoints))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", userID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", userID, inputPoints);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00079208 File Offset: 0x00077408
		private static TCPProcessCmdResults ProcessUpdateInputPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserIPoints = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace("$", ":");
				string toDate = fields[3].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 64, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 64, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool canNotInputPoints = false;
				int inputPoints = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.InputPoints = 0;
					}
					if (addOrSubUserIPoints < 0 && dbUserInfo.InputPoints < Math.Abs(addOrSubUserIPoints))
					{
						canNotInputPoints = true;
					}
					else
					{
						dbUserInfo.InputPoints = Math.Max(0, dbUserInfo.InputPoints + addOrSubUserIPoints);
						inputPoints = dbUserInfo.InputPoints;
					}
				}
				if (canNotInputPoints)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserIPoints != 0)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, inputPoints))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, inputPoints);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x000796D4 File Offset: 0x000778D4
		private static TCPProcessCmdResults ProcessUpdateSpecJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserSpecJiFen = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace("$", ":");
				string toDate = fields[3].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 44, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 44, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool canNotSpecJiFen = false;
				int SpecJiFen = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.SpecJiFen = 0;
					}
					if (addOrSubUserSpecJiFen < 0 && dbUserInfo.SpecJiFen < Math.Abs(addOrSubUserSpecJiFen))
					{
						canNotSpecJiFen = true;
					}
					else
					{
						dbUserInfo.SpecJiFen = Math.Max(0, dbUserInfo.SpecJiFen + addOrSubUserSpecJiFen);
						SpecJiFen = dbUserInfo.SpecJiFen;
					}
				}
				if (canNotSpecJiFen)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserSpecJiFen != 0)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, SpecJiFen))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, SpecJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x00079BA0 File Offset: 0x00077DA0
		private static TCPProcessCmdResults ProcessUpdateEveryJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserEveryJiFen = Convert.ToInt32(fields[1]);
				string fromDate = fields[2].Replace("$", ":");
				string toDate = fields[3].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 47, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 47, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool canNotEveryJiFen = false;
				int EveryJiFen = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.EveryJiFen = 0;
					}
					if (addOrSubUserEveryJiFen < 0 && dbUserInfo.EveryJiFen < Math.Abs(addOrSubUserEveryJiFen))
					{
						canNotEveryJiFen = true;
					}
					else
					{
						dbUserInfo.EveryJiFen = Math.Max(0, dbUserInfo.EveryJiFen + addOrSubUserEveryJiFen);
						EveryJiFen = dbUserInfo.EveryJiFen;
					}
				}
				if (canNotEveryJiFen)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserEveryJiFen != 0)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, EveryJiFen))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, EveryJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0007A06C File Offset: 0x0007826C
		private static TCPProcessCmdResults ProcessSubChongZhiJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int subGiftJiFen = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool canNotSubJiFen = false;
				int giftJiFen = 0;
				lock (dbUserInfo)
				{
					if (dbUserInfo.GiftJiFen >= Math.Abs(subGiftJiFen))
					{
						dbUserInfo.GiftJiFen = Math.Max(0, dbUserInfo.GiftJiFen - Math.Abs(subGiftJiFen));
						giftJiFen = dbUserInfo.GiftJiFen;
					}
					else
					{
						canNotSubJiFen = true;
					}
				}
				if (canNotSubJiFen)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (subGiftJiFen != 0)
				{
					if (!DBWriter.UpdateUserGiftJiFen(dbMgr, userID, giftJiFen))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, giftJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0007A3E8 File Offset: 0x000785E8
		private static TCPProcessCmdResults ProcessUseLiPinMaCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int songLiID = Convert.ToInt32(fields[1]);
				string liPinMa = fields[2];
				liPinMa = liPinMa.ToUpper();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret;
				if (liPinMa.Substring(0, 2) == "NZ")
				{
					ret = LiPinMaManager.UseLiPinMa2(dbMgr, roleID, songLiID, liPinMa, dbRoleInfo.ZoneID);
				}
				else if (liPinMa.Substring(0, 2) == "NX")
				{
					ret = LiPinMaManager.UseLiPinMaNX(dbMgr, roleID, songLiID, liPinMa, dbRoleInfo.ZoneID);
				}
				else
				{
					ret = LiPinMaManager.UseLiPinMa(dbMgr, roleID, songLiID, liPinMa, false);
				}
				string strcmd = string.Format("{0}:{1}", roleID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0007A62C File Offset: 0x0007882C
		private static TCPProcessCmdResults ProcessDeleteSpecActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int groupID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.SpecActInfoDict)
					{
						if (groupID == 0)
						{
							dbRoleInfo.SpecActInfoDict.Clear();
						}
						else
						{
							List<int> SpecActInfoDelete = new List<int>();
							foreach (KeyValuePair<int, SpecActInfoDB> kvp in dbRoleInfo.SpecActInfoDict)
							{
								if (kvp.Value.GroupID == groupID)
								{
									SpecActInfoDelete.Add(kvp.Key);
								}
							}
							foreach (int key in SpecActInfoDelete)
							{
								dbRoleInfo.SpecActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteSpecialActivityData(dbMgr, roleID, groupID);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0007A94C File Offset: 0x00078B4C
		private static TCPProcessCmdResults ProcessDeleteEveryActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length < 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int groupID = Convert.ToInt32(fields[1]);
				int actID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.EverydayActInfoDict)
					{
						if (groupID == 0)
						{
							dbRoleInfo.EverydayActInfoDict.Clear();
						}
						else
						{
							List<int> EveryActInfoDelete = new List<int>();
							foreach (KeyValuePair<int, EverydayActInfoDB> kvp in dbRoleInfo.EverydayActInfoDict)
							{
								if (kvp.Value.GroupID == groupID && (actID == 0 || kvp.Key == actID))
								{
									EveryActInfoDelete.Add(kvp.Key);
								}
							}
							foreach (int key in EveryActInfoDelete)
							{
								dbRoleInfo.EverydayActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteEverydayActivityData(dbMgr, roleID, groupID, actID);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0007AC8C File Offset: 0x00078E8C
		private static TCPProcessCmdResults ProcessGetSpecActInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, SpecActInfoDB>>(dbRoleInfo.SpecActInfoDict, pool, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0007AE3C File Offset: 0x0007903C
		private static TCPProcessCmdResults ProcessUpdateSpecActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				SpecActInfoDB SpecAct = new SpecActInfoDB();
				SpecAct.GroupID = Convert.ToInt32(fields[1]);
				SpecAct.ActID = Convert.ToInt32(fields[2]);
				SpecAct.PurNum = Convert.ToInt32(fields[3]);
				SpecAct.CountNum = Convert.ToInt32(fields[4]);
				SpecAct.Active = Convert.ToInt16(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.SpecActInfoDict)
					{
						dbRoleInfo.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
					}
					dbRoleInfo.SpecActInfoDict[SpecAct.ActID] = SpecAct;
				}
				DBWriter.UpdateSpecialActivityData(dbMgr, roleID, SpecAct);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0007B0BC File Offset: 0x000792BC
		private static TCPProcessCmdResults ProcessUpdateSpecPriorityActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				SpecPriorityActInfoDB SpecAct = new SpecPriorityActInfoDB();
				SpecAct.TeQuanID = Convert.ToInt32(fields[1]);
				SpecAct.ActID = Convert.ToInt32(fields[2]);
				SpecAct.PurNum = Convert.ToInt32(fields[3]);
				SpecAct.CountNum = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(SpecAct.TeQuanID, SpecAct.ActID);
					if (null == dbRoleInfo.SpecPriorityActInfoDict)
					{
						dbRoleInfo.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
					}
					dbRoleInfo.SpecPriorityActInfoDict[kvpKey] = SpecAct;
				}
				DBWriter.UpdateSpecialPriorityActivityData(dbMgr, roleID, SpecAct);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0007B33C File Offset: 0x0007953C
		private static TCPProcessCmdResults ProcessDeleteSpecPriorityActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int tequanID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.SpecPriorityActInfoDict)
					{
						if (tequanID == 0)
						{
							dbRoleInfo.SpecPriorityActInfoDict.Clear();
						}
						else
						{
							List<KeyValuePair<int, int>> SpecActInfoDelete = new List<KeyValuePair<int, int>>();
							foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> kvp in dbRoleInfo.SpecPriorityActInfoDict)
							{
								if (kvp.Value.TeQuanID == tequanID)
								{
									SpecActInfoDelete.Add(new KeyValuePair<int, int>(tequanID, kvp.Value.ActID));
								}
							}
							foreach (KeyValuePair<int, int> key in SpecActInfoDelete)
							{
								dbRoleInfo.SpecPriorityActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteSpecialPriorityActivityData(dbMgr, roleID, tequanID);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0007B664 File Offset: 0x00079864
		private static TCPProcessCmdResults ProcessUpdateEveryActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				EverydayActInfoDB EveryAct = new EverydayActInfoDB();
				EveryAct.GroupID = Convert.ToInt32(fields[1]);
				EveryAct.ActID = Convert.ToInt32(fields[2]);
				EveryAct.PurNum = Convert.ToInt32(fields[3]);
				EveryAct.CountNum = Convert.ToInt32(fields[4]);
				EveryAct.ActiveDay = Convert.ToInt32(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.EverydayActInfoDict)
					{
						dbRoleInfo.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
					}
					dbRoleInfo.EverydayActInfoDict[EveryAct.ActID] = EveryAct;
				}
				DBWriter.UpdateEverydayActivityData(dbMgr, roleID, EveryAct);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0007B8E4 File Offset: 0x00079AE4
		private static TCPProcessCmdResults ProcessUpdateFuBenDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int fuBenID = Convert.ToInt32(fields[1]);
				int dayID = Convert.ToInt32(fields[2]);
				int enterNum = Convert.ToInt32(fields[3]);
				int nQuickPassTimeSec = Convert.ToInt32(fields[4]);
				int nFinishNum = Convert.ToInt32(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.FuBenDataList)
					{
						dbRoleInfo.FuBenDataList = new List<FuBenData>();
					}
					bool found = false;
					for (int i = 0; i < dbRoleInfo.FuBenDataList.Count; i++)
					{
						if (dbRoleInfo.FuBenDataList[i].FuBenID == fuBenID)
						{
							dbRoleInfo.FuBenDataList[i].FuBenID = fuBenID;
							dbRoleInfo.FuBenDataList[i].DayID = dayID;
							dbRoleInfo.FuBenDataList[i].EnterNum = enterNum;
							dbRoleInfo.FuBenDataList[i].QuickPassTimer = nQuickPassTimeSec;
							dbRoleInfo.FuBenDataList[i].FinishNum = nFinishNum;
							found = true;
							break;
						}
					}
					if (!found)
					{
						dbRoleInfo.FuBenDataList.Add(new FuBenData
						{
							FuBenID = fuBenID,
							DayID = dayID,
							EnterNum = enterNum,
							QuickPassTimer = nQuickPassTimeSec,
							FinishNum = nFinishNum
						});
					}
				}
				DBWriter.UpdateFuBenData(dbMgr, roleID, fuBenID, dayID, enterNum, nQuickPassTimeSec, nFinishNum);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0007BC40 File Offset: 0x00079E40
		private static TCPProcessCmdResults ProcessGetFuBenSeqIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int fuBenSeqID = FuBenSeqIDMgr.GetFuBenSeqID();
				string strcmd = string.Format("{0}:{1}", roleID, fuBenSeqID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0007BD84 File Offset: 0x00079F84
		private static TCPProcessCmdResults ProcessGetFuBenHistDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int fuBenID = Convert.ToInt32(fields[1]);
				FuBenHistData fuBenHistData = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenHistData>(fuBenHistData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0007BEBC File Offset: 0x0007A0BC
		private static TCPProcessCmdResults ProcessAddFuBenHistDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string roleName = fields[1];
				int fuBenID = Convert.ToInt32(fields[2]);
				int usedSecs = Convert.ToInt32(fields[3]);
				FuBenHistData fuBenHistData = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				string strcmd;
				if (fuBenHistData == null || usedSecs < fuBenHistData.UsedSecs)
				{
					int ret = DBWriter.InsertNewFuBenHist(dbMgr, fuBenID, roleID, roleName, usedSecs);
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, ret);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					FuBenHistManager.AddFuBenHistData(fuBenID, roleID, roleName, usedSecs);
				}
				strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0007C0AC File Offset: 0x0007A2AC
		private static TCPProcessCmdResults ProcessUpdateLianZhanCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int lianzhan = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateLianZhan(dbMgr, roleID, lianzhan))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, lianzhan, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色连斩值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.LianZhan = lianzhan;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, lianzhan, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x0007C30C File Offset: 0x0007A50C
		private static TCPProcessCmdResults ProcessUpdateRoleDailyDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 14)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int expDayID = Convert.ToInt32(fields[1]);
				int todayExp = Convert.ToInt32(fields[2]);
				int lingLiDayID = Convert.ToInt32(fields[3]);
				int todayLingLi = Convert.ToInt32(fields[4]);
				int killBossDayID = Convert.ToInt32(fields[5]);
				int todayKillBoss = Convert.ToInt32(fields[6]);
				int fuBenDayID = Convert.ToInt32(fields[7]);
				int todayFuBenNum = Convert.ToInt32(fields[8]);
				int wuXingDayID = Convert.ToInt32(fields[9]);
				int wuXingNum = Convert.ToInt32(fields[10]);
				int rebornExpDayID = Convert.ToInt32(fields[11]);
				int rebornExpMonster = Convert.ToInt32(fields[12]);
				int rebornExpSale = Convert.ToInt32(fields[13]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleDailyData(dbMgr, roleID, expDayID, todayExp, lingLiDayID, todayLingLi, killBossDayID, todayKillBoss, fuBenDayID, todayFuBenNum, wuXingDayID, wuXingNum, rebornExpDayID, rebornExpMonster, rebornExpSale);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色日常数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.MyRoleDailyData)
						{
							dbRoleInfo.MyRoleDailyData = new RoleDailyData();
						}
						dbRoleInfo.MyRoleDailyData.ExpDayID = expDayID;
						dbRoleInfo.MyRoleDailyData.TodayExp = todayExp;
						dbRoleInfo.MyRoleDailyData.LingLiDayID = lingLiDayID;
						dbRoleInfo.MyRoleDailyData.TodayLingLi = todayLingLi;
						dbRoleInfo.MyRoleDailyData.KillBossDayID = killBossDayID;
						dbRoleInfo.MyRoleDailyData.TodayKillBoss = todayKillBoss;
						dbRoleInfo.MyRoleDailyData.FuBenDayID = fuBenDayID;
						dbRoleInfo.MyRoleDailyData.TodayFuBenNum = todayFuBenNum;
						dbRoleInfo.MyRoleDailyData.WuXingDayID = wuXingDayID;
						dbRoleInfo.MyRoleDailyData.WuXingNum = wuXingNum;
						dbRoleInfo.MyRoleDailyData.RebornExpDayID = rebornExpDayID;
						dbRoleInfo.MyRoleDailyData.RebornExpMonster = rebornExpMonster;
						dbRoleInfo.MyRoleDailyData.RebornExpSale = rebornExpSale;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0007C6D0 File Offset: 0x0007A8D0
		private static TCPProcessCmdResults ProcessUpdateKillBossCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int killBoss = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateKillBoss(dbMgr, roleID, killBoss))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, killBoss, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色杀BOSS总数量时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.KillBoss = killBoss;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, killBoss, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x0007C930 File Offset: 0x0007AB30
		private static TCPProcessCmdResults ProcessUpdateRoleStatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int equipJiFen = Convert.ToInt32(fields[1]);
				int xueWeiNum = Convert.ToInt32(fields[2]);
				int skillLearnedNum = Convert.ToInt32(fields[3]);
				int horseJiFen = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleStat(dbMgr, roleID, equipJiFen, xueWeiNum, skillLearnedNum, horseJiFen))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色的统计数据时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x0007CB60 File Offset: 0x0007AD60
		private static TCPProcessCmdResults ProcessGetPaiHangListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2 && fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int paiHangType = Convert.ToInt32(fields[1]);
				int pageShowNum = -1;
				if (fields.Length == 3)
				{
					pageShowNum = Convert.ToInt32(fields[2]);
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (roleID != 0 && null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				PaiHangData paiHangData = PaiHangManager.GetPaiHangData(paiHangType, pageShowNum);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PaiHangData>(paiHangData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0007CD30 File Offset: 0x0007AF30
		private static TCPProcessCmdResults ProcessUpdateYaBiaoDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int yaBiaoID = Convert.ToInt32(fields[1]);
				long startTime = Convert.ToInt64(fields[2]);
				int state = Convert.ToInt32(fields[3]);
				int lineID = Convert.ToInt32(fields[4]);
				int touBao = Convert.ToInt32(fields[5]);
				int yaBiaoDayID = Convert.ToInt32(fields[6]);
				int yaBiaoNum = Convert.ToInt32(fields[7]);
				int takeGoods = Convert.ToInt32(fields[8]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int oldState = 0;
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.MyYaBiaoData)
					{
						oldState = (dbRoleInfo.MyYaBiaoData.State = state);
					}
				}
				if (oldState > 0)
				{
					state = oldState;
				}
				int ret = DBWriter.UpdateYaBiaoData(dbMgr, roleID, yaBiaoID, startTime, state, lineID, touBao, yaBiaoDayID, yaBiaoNum, takeGoods);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.MyYaBiaoData)
						{
							dbRoleInfo.MyYaBiaoData = new YaBiaoData();
						}
						dbRoleInfo.MyYaBiaoData.YaBiaoID = yaBiaoID;
						dbRoleInfo.MyYaBiaoData.StartTime = startTime;
						dbRoleInfo.MyYaBiaoData.State = state;
						dbRoleInfo.MyYaBiaoData.LineID = lineID;
						dbRoleInfo.MyYaBiaoData.TouBao = touBao;
						dbRoleInfo.MyYaBiaoData.YaBiaoDayID = yaBiaoDayID;
						dbRoleInfo.MyYaBiaoData.YaBiaoNum = yaBiaoNum;
						dbRoleInfo.MyYaBiaoData.TakeGoods = takeGoods;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0007D0EC File Offset: 0x0007B2EC
		private static TCPProcessCmdResults ProcessUpdateYaBiaoDataStateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int state = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateYaBiaoDataState(dbMgr, roleID, state);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null == dbRoleInfo.MyYaBiaoData)
						{
							dbRoleInfo.MyYaBiaoData = new YaBiaoData();
						}
						dbRoleInfo.MyYaBiaoData.State = state;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0007D36C File Offset: 0x0007B56C
		private static TCPProcessCmdResults ProcessGetOtherAttrib2DataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int otherRoleID = Convert.ToInt32(fields[1]);
				RoleDataEx roleDataEx = new RoleDataEx();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBAllRoleInfo(otherRoleID);
				if (null == dbRoleInfo)
				{
					roleDataEx.RoleID = -1;
				}
				else
				{
					Global.DBRoleInfo2RoleDataEx(dbRoleInfo, roleDataEx);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0007D4C4 File Offset: 0x0007B6C4
		private static TCPProcessCmdResults ProcessUpdateBattleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				long startTicks = Convert.ToInt64(fields[1]);
				int nameIndex = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleBattleNameInfo(dbMgr, roleID, startTicks, nameIndex))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.BattleNameStart = startTicks;
						dbRoleInfo.BattleNameIndex = nameIndex;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0007D730 File Offset: 0x0007B930
		private static TCPProcessCmdResults ProcessAddMallBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftMoney = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewMallBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的商城购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x0007D964 File Offset: 0x0007BB64
		private static TCPProcessCmdResults ProcessAddQiZhenGeBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftMoney = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewQiZhenGeBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的奇珍阁购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x0007DB98 File Offset: 0x0007BD98
		private static TCPProcessCmdResults ProcessGetLiPinMaInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int songLiID = Convert.ToInt32(fields[1]);
				string liPinMa = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret;
				if (liPinMa.Length < 3)
				{
					ret = -1020;
				}
				else if (liPinMa.Substring(0, 2) == "NZ")
				{
					ret = LiPinMaManager.GetLiPinMaPingTaiID2(dbMgr, songLiID, liPinMa, dbRoleInfo.ZoneID);
				}
				else if (liPinMa.Substring(0, 2) == "NX")
				{
					ret = LiPinMaManager.GetLiPinMaPingTaiIDNX(dbMgr, songLiID, liPinMa, dbRoleInfo.ZoneID);
				}
				else
				{
					ret = LiPinMaManager.GetLiPinMaPingTaiID(dbMgr, songLiID, liPinMa);
				}
				string strcmd = string.Format("{0}:{1}", roleID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0007DDEC File Offset: 0x0007BFEC
		private static TCPProcessCmdResults ProcessGetInputPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace("$", ":");
				string toDate = fields[2].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 64, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 64, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int inputPoints = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.InputPoints = 0;
					}
					else
					{
						inputPoints = dbUserInfo.InputPoints;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, inputPoints);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0007E1B0 File Offset: 0x0007C3B0
		private static TCPProcessCmdResults ProcessGetSpecJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace("$", ":");
				string toDate = fields[2].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 44, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 44, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的专享活动充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int SpecJiFen = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.SpecJiFen = 0;
					}
					else
					{
						SpecJiFen = dbUserInfo.SpecJiFen;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, SpecJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0007E574 File Offset: 0x0007C774
		private static TCPProcessCmdResults ProcessGetEveryJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace("$", ":");
				string toDate = fields[2].Replace("$", ":");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				bool needCleanFirst = true;
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 47, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes > 0)
				{
					needCleanFirst = false;
				}
				else
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 47, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (needCleanFirst)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的每日活动充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int EveryJiFen = 0;
				lock (dbUserInfo)
				{
					if (needCleanFirst)
					{
						dbUserInfo.EveryJiFen = 0;
					}
					else
					{
						EveryJiFen = dbUserInfo.EveryJiFen;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, EveryJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0007E938 File Offset: 0x0007CB38
		private static TCPProcessCmdResults ProcessGetChongZhiJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbRoleInfo.UserID;
				DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int giftJiFen = 0;
				lock (dbUserInfo)
				{
					giftJiFen = dbUserInfo.GiftJiFen;
				}
				strcmd = string.Format("{0}:{1}", roleID, giftJiFen);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0007EBAC File Offset: 0x0007CDAC
		private static TCPProcessCmdResults ProcessUpdateCZTaskIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int czTaskID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleCZTaskID(dbMgr, roleID, czTaskID))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色充值任务ID时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.CZTaskID = czTaskID;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0007EE00 File Offset: 0x0007D000
		private static TCPProcessCmdResults ProcessGetTotalOnlineNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalOnlineNum = LineManager.GetTotalOnlineNum();
				string strcmd = string.Format("{0}", totalOnlineNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0007EF34 File Offset: 0x0007D134
		private static TCPProcessCmdResults ProcessGetFuBenHistListDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = FuBenHistManager.GetFuBenHistListData(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0007F048 File Offset: 0x0007D248
		private static TCPProcessCmdResults ProcessUpdateBattleNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int battleNum = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateBattleNum(dbMgr, roleID, battleNum))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, battleNum, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色角斗场称号次数时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.BattleNum = battleNum;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, battleNum, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0007F2A8 File Offset: 0x0007D4A8
		private static TCPProcessCmdResults ProcessUpdateHeroIndexCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int heroIndex = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateHeroIndex(dbMgr, roleID, heroIndex))
				{
					strcmd = string.Format("{0}:{1}:{2}", roleID, heroIndex, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色英雄逐擂到达层数时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.HeroIndex = heroIndex;
					}
					strcmd = string.Format("{0}:{1}:{2}", roleID, heroIndex, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0007F508 File Offset: 0x0007D708
		private static TCPProcessCmdResults ProcessForceReloadPaiHangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				PaiHangManager.ProcessPaiHang(dbMgr, true);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0007F64C File Offset: 0x0007D84C
		private static TCPProcessCmdResults ProcessGetOtherHorseDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int otherRoleID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				HorseData horsesData = null;
				DBRoleInfo otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
				if (null == otherDbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horsesData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (otherDbRoleInfo)
				{
					if (null != otherDbRoleInfo.HorsesDataList)
					{
						for (int i = 0; i < otherDbRoleInfo.HorsesDataList.Count; i++)
						{
							if (otherDbRoleInfo.HorsesDataList[i].DbID == otherDbRoleInfo.HorseDbID)
							{
								horsesData = otherDbRoleInfo.HorsesDataList[i];
								break;
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horsesData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0007F8C8 File Offset: 0x0007DAC8
		private static TCPProcessCmdResults ProcessAddYinPiaoBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftYinPiaoNum = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewYinPiaoBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftYinPiaoNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的银票购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0007FAFC File Offset: 0x0007DCFC
		private static TCPProcessCmdResults ProcessGetBangHuiListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int isVerify = Convert.ToInt32(fields[1]);
				int startIndex = Convert.ToInt32(fields[2]);
				int endIndex = Convert.ToInt32(fields[3]);
				BangHuiListData bangHuiListData = GameDBManager.BangHuiListMgr.GetBangHuiListData(dbMgr, isVerify, startIndex, endIndex);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiListData>(bangHuiListData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0007FC50 File Offset: 0x0007DE50
		private static TCPProcessCmdResults ProcessGetBangHuiFuBenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int guildid = Convert.ToInt32(fields[0]);
				string result = DBQuery.QueryBangFuBenByID(dbMgr, guildid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, result, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x0007FD7C File Offset: 0x0007DF7C
		private static TCPProcessCmdResults ProcessUpdateBangHuiFuBenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int guildid = Convert.ToInt32(fields[0]);
				int fubenid = Convert.ToInt32(fields[1]);
				int state = Convert.ToInt32(fields[2]);
				int openday = Convert.ToInt32(fields[3]);
				string killers = fields[4];
				DBWriter.UpdateBangHuiFuBen(dbMgr, guildid, fubenid, state, openday, killers);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x0007FED4 File Offset: 0x0007E0D4
		private static TCPProcessCmdResults ProcessCreateBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string bhName = fields[1].Split(new char[]
				{
					'$'
				})[0];
				string bhBulletin = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int retCode = 0;
				lock (dbRoleInfo)
				{
					if (dbRoleInfo.Faction > 0 || !string.IsNullOrEmpty(dbRoleInfo.BHName))
					{
						retCode = -1001;
					}
				}
				if (retCode >= 0 && !SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, bhName))
				{
					retCode = -1031;
				}
				if (retCode >= 0 && !SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(bhName))
				{
					retCode = -1031;
				}
				string strcmd;
				if (retCode < 0)
				{
					strcmd = string.Format("{0}:{1}:{2}", retCode, roleID, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = 0;
				int zoneID = GameDBManager.ZoneID;
				lock (Global.BangHuiMutex)
				{
					bhid = DBQuery.FindBangHuiByRoleID(dbMgr, roleID);
					if (bhid <= 0)
					{
						bhid = DBQuery.FindJoinBangHuiByRoleID(dbMgr, roleID);
						if (bhid <= 0)
						{
							bhid = DBWriter.CreateBangHui(dbMgr, roleID, zoneID, dbRoleInfo.Level, bhName, bhBulletin, Convert.ToInt32(fields[3]));
							if (bhid < 0)
							{
								retCode = -1031;
							}
						}
						else
						{
							retCode = -1021;
						}
					}
					else
					{
						retCode = -1011;
					}
				}
				if (retCode >= 0)
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.Faction = bhid;
						dbRoleInfo.BHName = Global.FormatBangHuiName(zoneID, bhName);
						dbRoleInfo.BHZhiWu = 1;
					}
					DBWriter.UpdateRoleBangHuiInfo(dbMgr, roleID, bhid, Global.FormatBangHuiName(zoneID, bhName), 1);
					GameDBManager.BangHuiJunQiMgr.AddBangHuiJunQi(bhid, bhName, 1);
				}
				strcmd = string.Format("{0}:{1}:{2}", retCode, roleID, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x000802FC File Offset: 0x0007E4FC
		private static TCPProcessCmdResults ProcessGetBangHuiMiniDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(fields[0]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiMiniData>(null, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMiniData bangHuiMiniData = new BangHuiMiniData
				{
					BHID = bangHuiDetailData.BHID,
					BHName = bangHuiDetailData.BHName,
					ZoneID = bangHuiDetailData.ZoneID
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiMiniData>(bangHuiMiniData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0008047C File Offset: 0x0007E67C
		private static TCPProcessCmdResults ProcessQueryBangHuiDetailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2 && fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int nChangeDayFlag = 0;
				int dayid = 0;
				if (4 == fields.Length)
				{
					nChangeDayFlag = Convert.ToInt32(fields[2]);
					dayid = Convert.ToInt32(fields[3]);
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (bangHuiDetailData == null || roleID <= 0)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData.MgrItemList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, bhid);
				if (nChangeDayFlag == 1)
				{
					dbRoleInfo.BGDayID1 = dayid;
					dbRoleInfo.BGMoney = 0;
					dbRoleInfo.BGDayID2 = dayid;
					dbRoleInfo.BGGoods = 0;
					DBWriter.UpdateRoleBangGong(dbMgr, roleID, dbRoleInfo.BGDayID1, dbRoleInfo.BGMoney, dbRoleInfo.BGDayID2, dbRoleInfo.BGGoods, dbRoleInfo.BangGong);
				}
				bangHuiDetailData.TodayZhanGongForGold = dbRoleInfo.BGMoney;
				bangHuiDetailData.TodayZhanGongForDiamond = dbRoleInfo.BGGoods;
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x00080708 File Offset: 0x0007E908
		private static TCPProcessCmdResults ProcessUpdateBangHuiBulletinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				string bulletinMsg = fields[2];
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				if (bangHuiMgrItemData != null && bangHuiMgrItemData.BHZhiwu > 0)
				{
					DBWriter.UpdateBangHuiBulletin(dbMgr, bhid, bulletinMsg);
				}
				string strcmd = string.Format("{0}:{1}:{2}", 0, roleID, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0008088C File Offset: 0x0007EA8C
		private static TCPProcessCmdResults ProcessGVoiceSetPrioritysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				string prioritys = fields[2];
				BangHuiCacheData bhData = GameDBManager.BangHuiListMgr.GetBangHuiCacheData(bhid);
				if (bhData != null && (bhData.LeaderId == (long)roleID || (bhData.Query(bhid) && bhData.LeaderId == (long)roleID)))
				{
					bhData.UpdateGVoicePrioritys(prioritys);
				}
				string strcmd = string.Format("{0}:{1}:{2}", 0, roleID, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x00080A34 File Offset: 0x0007EC34
		private static TCPProcessCmdResults ProcessGVoiceGetPrioritysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				int bhid = DataHelper.BytesToObject<int>(data, 0, count);
				string prioritys = "";
				BangHuiCacheData bhData = GameDBManager.BangHuiListMgr.GetBangHuiCacheData(bhid);
				if (bhData != null)
				{
					prioritys = bhData.GVoicePrioritys;
				}
				string strcmd = string.Format("{0}", prioritys);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00080AD4 File Offset: 0x0007ECD4
		private static TCPProcessCmdResults ProcessUpdateZhengDuoUsedTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int[] cmdData = null;
			try
			{
				cmdData = DataHelper.BytesToObject<int[]>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				int bhid = Convert.ToInt32(cmdData[0]);
				int weekDay = Convert.ToInt32(cmdData[1]);
				int usedTime = Convert.ToInt32(cmdData[2]);
				DBWriter.UpdateZhengDuoUsedTime(dbMgr, bhid, weekDay, usedTime);
				byte[] bytes = DataHelper.ObjectToBytes<int>(usedTime);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x00080BBC File Offset: 0x0007EDBC
		private static TCPProcessCmdResults ProcessGetZhengDuoUsedTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int cmdData;
			try
			{
				cmdData = DataHelper.BytesToObject<int>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				int bhid = Convert.ToInt32(cmdData);
				int[] args = DBQuery.QueryZhengDuoUsedTime(dbMgr, bhid);
				byte[] bytes = DataHelper.ObjectToBytes<int[]>(args);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x00080C88 File Offset: 0x0007EE88
		private static TCPProcessCmdResults ProcessGetBHMemberDataListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				List<BangHuiMemberData> bangHuiMemberDataList = DBQuery.GetBangHuiMemberDataList(dbMgr, bhid);
				if (null != bangHuiMemberDataList)
				{
					int i = 0;
					while (i < bangHuiMemberDataList.Count)
					{
						DBRoleInfo otherDbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != otherDbRoleInfo)
						{
							goto IL_137;
						}
						otherDbRoleInfo = dbMgr.GetDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != otherDbRoleInfo)
						{
							goto IL_137;
						}
						IL_162:
						i++;
						continue;
						IL_137:
						bangHuiMemberDataList[i].LogOffTime = otherDbRoleInfo.LogOffTime;
						bangHuiMemberDataList[i].OnlineState = Global.GetRoleOnlineState(otherDbRoleInfo);
						goto IL_162;
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<BangHuiMemberData>>(bangHuiMemberDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00080E88 File Offset: 0x0007F088
		private static TCPProcessCmdResults ProcessUpdateBHVerifyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int isVerify = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1,
						roleID,
						bhid,
						isVerify
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				if (null == bangHuiMgrItemData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-10,
						roleID,
						bhid,
						isVerify
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemData.BHZhiwu != 1)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-10,
						roleID,
						bhid,
						isVerify
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiVerify(dbMgr, roleID, bhid, isVerify);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					bhid,
					isVerify
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0008116C File Offset: 0x0007F36C
		private static TCPProcessCmdResults ProcessQueryBHMGRListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						bhid,
						-1,
						""
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string bhMgrList = DBQuery.GetBangHuiMgrItemItemStringList(dbMgr, bhid);
				bhMgrList = bhMgrList.TrimEnd(new char[]
				{
					','
				});
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					bhid,
					bangHuiDetailData.IsVerify,
					bhMgrList
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x00081388 File Offset: 0x0007F588
		private static TCPProcessCmdResults ProcessBangHuiVerifyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int toVerify = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				dbRoleInfo.BHVerify = toVerify;
				DBWriter.UpdateRoleBangHuiVerify(dbMgr, roleID, toVerify);
				string strcmd = string.Format("{0}:{1}", roleID, toVerify);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x00081534 File Offset: 0x0007F734
		private static TCPProcessCmdResults ProcessAddBHMemberCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int otherRoleID = Convert.ToInt32(fields[2]);
				string otherRoleName = fields[3];
				int toVerify = Convert.ToInt32(fields[4]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbOtherRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
				if (null == dbOtherRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (toVerify > 0)
				{
					if (dbOtherRoleInfo.BHVerify > 0)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1050,
							roleID,
							bhid,
							otherRoleID
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int bhMaxMembers = 50;
				if (DBQuery.QueryBHMemberNum(dbMgr, bhid) >= bhMaxMembers)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1060,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbOtherRoleInfo)
				{
					if (dbOtherRoleInfo.Faction > 0)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1020,
							roleID,
							bhid,
							otherRoleID
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					dbOtherRoleInfo.Faction = bhid;
					dbOtherRoleInfo.BHName = Global.FormatBangHuiName(bangHuiDetailData.ZoneID, bangHuiDetailData.BHName);
					dbOtherRoleInfo.BHZhiWu = 0;
					dbOtherRoleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbOtherRoleInfo.RoleID, dbOtherRoleInfo.Faction, dbOtherRoleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbOtherRoleInfo.RoleID, 0);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					bhid,
					otherRoleID
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x000819B0 File Offset: 0x0007FBB0
		private static TCPProcessCmdResults ProcessRemoveBHMemberCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int otherRoleID = Convert.ToInt32(fields[2]);
				string otherRoleName = fields[3];
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				string strcmd;
				if (null == bangHuiMgrItemData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemData.BHZhiwu <= 0)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1002,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbOtherRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
				if (null == dbOtherRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbOtherRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbOtherRoleInfo.BHZhiWu > 0 && bangHuiMgrItemData.BHZhiwu >= dbOtherRoleInfo.BHZhiWu)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1002,
						roleID,
						bhid,
						otherRoleID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbOtherRoleInfo)
				{
					dbOtherRoleInfo.Faction = 0;
					dbOtherRoleInfo.BHName = "";
					dbOtherRoleInfo.BHZhiWu = 0;
					dbOtherRoleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbOtherRoleInfo.RoleID, dbOtherRoleInfo.Faction, dbOtherRoleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbOtherRoleInfo.RoleID, 0);
				DBWriter.ClearLastBangHuiInfoByRoleID(dbMgr, dbOtherRoleInfo.RoleID);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					bhid,
					otherRoleID
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x00081E1C File Offset: 0x0008001C
		private static TCPProcessCmdResults ProcessQuitFromBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1000, roleID, bhid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1001, roleID, bhid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (dbRoleInfo.Faction != bhid)
					{
						strcmd = string.Format("{0}:{1}:{2}", -1020, roleID, bhid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (1 == dbRoleInfo.BHZhiWu)
					{
						strcmd = string.Format("{0}:{1}:{2}", -1030, roleID, bhid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					dbRoleInfo.Faction = 0;
					dbRoleInfo.BHName = "";
					dbRoleInfo.BHZhiWu = 0;
					dbRoleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbRoleInfo.RoleID, dbRoleInfo.Faction, dbRoleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbRoleInfo.RoleID, 0);
				strcmd = string.Format("{0}:{1}:{2}", 0, roleID, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0008214C File Offset: 0x0008034C
		private static TCPProcessCmdResults ProcessDestroyBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1000, roleID, bhid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1010, roleID, bhid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}", -1020, roleID, bhid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDestroyMgr.DoDestroyBangHui(dbMgr, bhid);
				strcmd = string.Format("{0}:{1}:{2}", 0, roleID, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x000823A4 File Offset: 0x000805A4
		private static TCPProcessCmdResults ProcessChgBHMemberZhiWuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int otherRoleID = Convert.ToInt32(fields[2]);
				int zhiWu = Convert.ToInt32(fields[3]);
				string strcmd;
				if (roleID == otherRoleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1002,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1001,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1010,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				if (null == bangHuiMgrItemData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1020,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemData.BHZhiwu != 1 || dbRoleInfo.BHZhiWu != 1)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1030,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo newDBRoleInfo = dbMgr.GetDBRoleInfo(ref otherRoleID);
				if (null == newDBRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1040,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (newDBRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1060,
						roleID,
						bhid,
						otherRoleID,
						zhiWu,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<BangHuiMgrItemData> bangHuiMgrItemDataList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, bhid);
				int oldZhiWuRoleID = 0;
				lock (Global.BangHuiMutex)
				{
					if (zhiWu > 0)
					{
						DBWriter.ClearBangHuiMemberZhiWu(dbMgr, bhid, zhiWu);
						oldZhiWuRoleID = Global.GetDBRoleInfoByZhiWu(bangHuiMgrItemDataList, zhiWu);
						if (oldZhiWuRoleID > 0)
						{
							DBRoleInfo oldDBRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref oldZhiWuRoleID);
							if (null != oldDBRoleInfo)
							{
								lock (oldDBRoleInfo)
								{
									oldDBRoleInfo.BHZhiWu = 0;
								}
							}
						}
					}
					bool canUpdateDB = false;
					lock (newDBRoleInfo)
					{
						if (newDBRoleInfo.Faction == bhid)
						{
							canUpdateDB = true;
							newDBRoleInfo.BHZhiWu = zhiWu;
						}
					}
					if (canUpdateDB)
					{
						DBWriter.UpdateBangHuiMemberZhiWu(dbMgr, bhid, otherRoleID, zhiWu);
						if (1 == zhiWu)
						{
							DBWriter.UpdateBangHuiRoleID(dbMgr, otherRoleID, bhid);
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					0,
					roleID,
					bhid,
					otherRoleID,
					zhiWu,
					oldZhiWuRoleID
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x00082A70 File Offset: 0x00080C70
		private static TCPProcessCmdResults ProcessChgJunTuanMemberZhiWuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			try
			{
				tcpOutPacket = null;
				List<int> list = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (list.Count < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}", (TCPGameServerCmds)nID, list.Count), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(list[0]);
				int zhiWu = Convert.ToInt32(list[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}", -1001);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = dbRoleInfo.Faction;
				if (dbRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				if (null == bangHuiMgrItemData)
				{
					strcmd = string.Format("{0}", -1020);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemData.BHZhiwu != 1 || dbRoleInfo.BHZhiWu != 1)
				{
					strcmd = string.Format("{0}", -1030);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> ridList = list.GetRange(2, list.Count - 2);
				DBWriter.ClearBangHuiZhiWuNotInList(dbMgr, bhid, ridList);
				DBWriter.ChangeJunTuanZhiWuList(dbMgr, bhid, zhiWu, ridList);
				List<BangHuiMemberData> bangHuiMemberDataList = DBQuery.GetBangHuiMemberDataList(dbMgr, bhid);
				if (null != bangHuiMemberDataList)
				{
					for (int i = 0; i < bangHuiMemberDataList.Count; i++)
					{
						DBRoleInfo otherDbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != otherDbRoleInfo)
						{
							otherDbRoleInfo.JunTuanZhiWu = (ridList.Contains(bangHuiMemberDataList[i].RoleID) ? zhiWu : 0);
						}
					}
				}
				strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x00082D28 File Offset: 0x00080F28
		private static TCPProcessCmdResults ProcessChgBHMemberChengHaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int otherRoleID = Convert.ToInt32(fields[2]);
				string chengHao = fields[3];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1001,
						roleID,
						bhid,
						otherRoleID,
						chengHao
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						roleID,
						bhid,
						otherRoleID,
						chengHao
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemData = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, bhid, roleID);
				if (null == bangHuiMgrItemData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1020,
						roleID,
						bhid,
						otherRoleID,
						chengHao
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemData.BHZhiwu <= 0)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1030,
						roleID,
						bhid,
						otherRoleID,
						chengHao
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiMemberChengHao(dbMgr, bhid, otherRoleID, chengHao);
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					roleID,
					bhid,
					otherRoleID,
					chengHao
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x000830A0 File Offset: 0x000812A0
		private static TCPProcessCmdResults ProcessSearchRolesFromDBCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string searchText = fields[1];
				int startIndex = Convert.ToInt32(fields[2]);
				List<SearchRoleData> searchRoleDataList = null;
				int otherID = -1;
				if (searchText.Length > 0)
				{
					otherID = dbMgr.DBRoleMgr.FindDBRoleID(searchText);
					if (-1 != otherID)
					{
						DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref otherID);
						if (null != dbRoleInfo)
						{
							searchRoleDataList = new List<SearchRoleData>();
							SearchRoleData searchRoleData = new SearchRoleData
							{
								RoleID = dbRoleInfo.RoleID,
								RoleName = Global.FormatRoleName(dbRoleInfo.ZoneID, dbRoleInfo.RoleName),
								RoleSex = dbRoleInfo.RoleSex,
								Level = dbRoleInfo.Level,
								Occupation = dbRoleInfo.Occupation,
								Faction = dbRoleInfo.Faction,
								BHName = dbRoleInfo.BHName
							};
							searchRoleDataList.Add(searchRoleData);
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(searchRoleDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x000832CC File Offset: 0x000814CC
		private static TCPProcessCmdResults ProcessGetBangGongHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiBagData>(bangHuiBagData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiBagData.BbangGongHistList = DBQuery.GetBangHuiBagHistList(dbMgr, bhid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiBagData>(bangHuiBagData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x00083434 File Offset: 0x00081634
		private static TCPProcessCmdResults ProcessDonateBGMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int money = Convert.ToInt32(fields[2]);
				int bangGong = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						roleID,
						bhid,
						money,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						roleID,
						bhid,
						money,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1001,
						roleID,
						bhid,
						money,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiBangGong(dbMgr, bhid, 0, 0, 0, 0, 0, money);
				DBWriter.AddBangGongHistItem(dbMgr, bhid, roleID, 0, 0, 0, 0, 0, money, bangGong);
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					roleID,
					bhid,
					money,
					bangGong
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0008376C File Offset: 0x0008196C
		private static TCPProcessCmdResults ProcessDonateBGGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int goods1Num = Convert.ToInt32(fields[2]);
				int goods2Num = Convert.ToInt32(fields[3]);
				int goods3Num = Convert.ToInt32(fields[4]);
				int goods4Num = Convert.ToInt32(fields[5]);
				int goods5Num = Convert.ToInt32(fields[6]);
				int bangGong = Convert.ToInt32(fields[7]);
				int nAddMoney = Convert.ToInt32(fields[8]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						roleID,
						bhid,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						bhid,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbRoleInfo.Faction != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						roleID,
						bhid,
						bangGong
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiBangGong(dbMgr, bhid, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, nAddMoney);
				DBWriter.AddBangGongHistItem(dbMgr, bhid, roleID, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, nAddMoney, bangGong);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					bhid,
					bangGong
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x00083AB4 File Offset: 0x00081CB4
		private static TCPProcessCmdResults ProcessUpdateBangGongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bgDayid = Convert.ToInt32(fields[1]);
				int bgMoney = Convert.ToInt32(fields[2]);
				int bgDayid2 = Convert.ToInt32(fields[3]);
				int bgGoods = Convert.ToInt32(fields[4]);
				int addOrSubBangGong = Convert.ToInt32(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				int bangGong = 0;
				lock (dbRoleInfo)
				{
					if (addOrSubBangGong < 0 && dbRoleInfo.BangGong < Math.Abs(addOrSubBangGong))
					{
						failed = true;
					}
					else
					{
						dbRoleInfo.BangGong = Math.Max(0, dbRoleInfo.BangGong + addOrSubBangGong);
						bangGong = dbRoleInfo.BangGong;
					}
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubBangGong != 0)
				{
					if (!DBWriter.UpdateRoleBangGong(dbMgr, roleID, bgDayid, bgMoney, bgDayid2, bgGoods, bangGong))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色帮贡失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					lock (dbRoleInfo)
					{
						dbRoleInfo.BGDayID1 = bgDayid;
						dbRoleInfo.BGMoney = bgMoney;
						dbRoleInfo.BGDayID2 = bgDayid2;
						dbRoleInfo.BGGoods = bgGoods;
					}
				}
				strcmd = string.Format("{0}:{1}", roleID, bangGong);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00083E44 File Offset: 0x00082044
		private static TCPProcessCmdResults ProcessUpdateBHTongQianCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int subMoney = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}", -1000, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					strcmd = string.Format("{0}:{1}", -1010, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < subMoney + GameDBManager.GameConfigMgr.GetGameConfigItemInt("ZhanMengZiJinInitialValue", 20000))
				{
					strcmd = string.Format("{0}:{1}", -1110, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.SubBangHuiTongQian(dbMgr, bhid, subMoney);
				strcmd = string.Format("{0}:{1}", 0, bangHuiDetailData.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x00084100 File Offset: 0x00082300
		private static TCPProcessCmdResults ProcessAddBHTongQianCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int addMoney = Convert.ToInt32(fields[2]);
				if (roleID > 0)
				{
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null == dbRoleInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}", -1000, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddBangHuiTongQian(dbMgr, bhid, addMoney);
				strcmd = string.Format("{0}:{1}", 0, bangHuiDetailData.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00084328 File Offset: 0x00082528
		private static TCPProcessCmdResults ProcessGetBangQiInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				BangQiInfoData bangQiInfoData = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangQiInfoData>(bangQiInfoData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangQiInfoData = DBQuery.QueryBangQiInfoByID(dbMgr, bhid);
				bangQiInfoData.BHLingDiOwnDict = DBQuery.GetBHLingDiOwnDataDict(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangQiInfoData>(bangQiInfoData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0008449C File Offset: 0x0008269C
		private static TCPProcessCmdResults ProcessRenameBangQiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				string bhQiName = fields[2];
				int needMoney = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiBagData)
				{
					strcmd = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < needMoney)
				{
					strcmd = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiName(dbMgr, bhid, bhQiName, needMoney);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiName(bhid, bhQiName);
				strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x000846F8 File Offset: 0x000828F8
		private static TCPProcessCmdResults ProcessUpLevelBangQiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int goods1Num = Convert.ToInt32(fields[2]);
				int goods2Num = Convert.ToInt32(fields[3]);
				int goods3Num = Convert.ToInt32(fields[4]);
				int goods4Num = Convert.ToInt32(fields[5]);
				int goods5Num = Convert.ToInt32(fields[6]);
				int needMoney = Convert.ToInt32(fields[7]);
				int toLevel = Convert.ToInt32(fields[8]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (roleID != bangHuiDetailData.BZRoleID)
				{
					strcmd = string.Format("{0}", -9368);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.QiLevel + 1 != toLevel)
				{
					strcmd = string.Format("{0}", -1005);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					strcmd = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < needMoney)
				{
					strcmd = string.Format("{0}", -1110);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods1Num < goods1Num)
				{
					strcmd = string.Format("{0}", -1111);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods2Num < goods2Num)
				{
					strcmd = string.Format("{0}", -1112);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods3Num < goods3Num)
				{
					strcmd = string.Format("{0}", -1113);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods4Num < goods4Num)
				{
					strcmd = string.Format("{0}", -1114);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods5Num < goods5Num)
				{
					strcmd = string.Format("{0}", -1115);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiLevel(dbMgr, bhid, toLevel, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, needMoney);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, toLevel);
				strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x00084B8C File Offset: 0x00082D8C
		private static TCPProcessCmdResults ProcessGMUpdateBangLevel(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(fields[0]);
				int toLevel = Convert.ToInt32(fields[1]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiLevel(dbMgr, bhid, toLevel, 0, 0, 0, 0, 0, 0);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, toLevel);
				strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x00084D30 File Offset: 0x00082F30
		private static TCPProcessCmdResults ProcessGetBHJunQiListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiJunQiMgr.GetBangHuiJunQiItemsDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x00084E48 File Offset: 0x00083048
		private static TCPProcessCmdResults ProcessGetBHLingDiDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiLingDiMgr.GetBangHuiLingDiItemsDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x00084F60 File Offset: 0x00083160
		private static TCPProcessCmdResults ProcessUpdateLingDiForBHCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int lingDiID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				string strcmd;
				if (bhid <= 0)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.ClearLingDiBangHuiInfo(lingDiID);
					if (null != bangHuiLingDiInfoData)
					{
						DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
					}
					if (lingDiID == 2)
					{
						bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.ClearLingDiBangHuiInfo(1);
						if (null != bangHuiLingDiInfoData)
						{
							DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
						}
					}
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
					if (null == bangHuiDetailData)
					{
						strcmd = string.Format("{0}", -1000);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddBangHuiLingDi(bhid, bangHuiDetailData.ZoneID, bangHuiDetailData.BHName, lingDiID);
					if (null != bangHuiLingDiInfoData)
					{
						DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
					}
					if (lingDiID == 2)
					{
						bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddBangHuiLingDi(bhid, bangHuiDetailData.ZoneID, bangHuiDetailData.BHName, 1);
						if (null != bangHuiLingDiInfoData)
						{
							DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
						}
					}
				}
				strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x000851E4 File Offset: 0x000833E4
		private static TCPProcessCmdResults ProcessGetLeaderRoleIDByBHIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(fields[0]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}", 0, "", "");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}", bangHuiDetailData.BZRoleID, Global.FormatRoleName(bangHuiDetailData.ZoneID, bangHuiDetailData.BZRoleName), Global.FormatBangHuiName(bangHuiDetailData.ZoneID, bangHuiDetailData.BHName));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000853A8 File Offset: 0x000835A8
		private static TCPProcessCmdResults ProcessGetBHLingDiInfoDictByBHIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfoDataDict = null;
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(bangHuiLingDiInfoDataDict, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BHID != bhid || bangHuiDetailData.BZRoleID != roleID)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(bangHuiLingDiInfoDataDict, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiLingDiMgr.GetBangHuiLingDiInfosDictTCPOutPacket(pool, bhid, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00085538 File Offset: 0x00083738
		private static TCPProcessCmdResults ProcessSetLingDiTaxCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int lingDiID = Convert.ToInt32(fields[2]);
				int newLingDiTax = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						roleID,
						bhid,
						lingDiID,
						newLingDiTax
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1005,
						roleID,
						bhid,
						lingDiID,
						newLingDiTax
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.UpdateBangHuiLingDiTax(bhid, lingDiID, newLingDiTax);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						roleID,
						bhid,
						lingDiID,
						newLingDiTax
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					roleID,
					bhid,
					lingDiID,
					newLingDiTax
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x000858B0 File Offset: 0x00083AB0
		private static TCPProcessCmdResults ProcessSetLingDiWarRequestCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int lingDiID = Convert.ToInt32(fields[0]);
				string newWarRequest = fields[1];
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.UpdateBangHuiLingDiWarRequest(lingDiID, newWarRequest);
				string strcmd;
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						-1,
						-1,
						lingDiID,
						newWarRequest
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					-1,
					lingDiID,
					newWarRequest
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00085AC0 File Offset: 0x00083CC0
		private static TCPProcessCmdResults ProcessTakeLingDiDailyAwardCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int lingDiID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						roleID,
						bhid,
						lingDiID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						roleID,
						bhid,
						lingDiID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(lingDiID);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						bhid,
						lingDiID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						roleID,
						bhid,
						lingDiID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayID = DateTime.Now.DayOfYear;
				if (dayID == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1040,
							roleID,
							bhid,
							lingDiID
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.TakeLingDiDailyAward(bhid, lingDiID);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1050,
						roleID,
						bhid,
						lingDiID
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					bhid,
					lingDiID
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00085F6C File Offset: 0x0008416C
		private static TCPProcessCmdResults ProcessTakeLingDiTaxMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int lingDiID = Convert.ToInt32(fields[2]);
				int takeTaxMoney = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1005,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(lingDiID);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1020,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if ((double)takeTaxMoney > (double)bangHuiLingDiInfoData.TotalTax * 0.25)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1030,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayID = DateTime.Now.DayOfYear;
				if (dayID == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1040,
							roleID,
							bhid,
							lingDiID,
							takeTaxMoney
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.TakeLingDiTaxMoney(bhid, lingDiID, takeTaxMoney);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1050,
						roleID,
						bhid,
						lingDiID,
						takeTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				DBWriter.AddBangHuiTongQian(dbMgr, bhid, takeTaxMoney);
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					roleID,
					bhid,
					lingDiID,
					takeTaxMoney
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x000864FC File Offset: 0x000846FC
		private static TCPProcessCmdResults ProcessAddLingDiTaxMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(fields[0]);
				int lingDiID = Convert.ToInt32(fields[1]);
				int addTaxMoney = Convert.ToInt32(fields[2]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						bhid,
						lingDiID,
						addTaxMoney
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddLingDiTaxMoney(bhid, lingDiID, addTaxMoney);
				if (null != bangHuiLingDiInfoData)
				{
					DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					bhid,
					lingDiID,
					addTaxMoney
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0008672C File Offset: 0x0008492C
		private static TCPProcessCmdResults ProcessGetHuangDiBHInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				BangHuiDetailData bangHuiDetailData = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(6);
				if (bangHuiLingDiInfoData == null || bangHuiLingDiInfoData.BHID <= 0)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bangHuiLingDiInfoData.BHID);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData.MgrItemList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, bangHuiDetailData.BHID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0008690C File Offset: 0x00084B0C
		private static TCPProcessCmdResults ProcessQueryQiZhenGeBuyHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<QizhenGeBuItemData> list = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<QizhenGeBuItemData>>(list, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				list = QiZhenGeBuManager.GetQizhenGeBuItemDataList(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<QizhenGeBuItemData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x00086A64 File Offset: 0x00084C64
		private static TCPProcessCmdResults ProcessGetHuangDiRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int huangDiRoleID = Convert.ToInt32(fields[1]);
				RoleDataEx roleDataEx = new RoleDataEx();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref huangDiRoleID);
				if (null == dbRoleInfo)
				{
					roleDataEx.RoleID = -1;
				}
				else
				{
					Global.DBRoleInfo2RoleDataEx(dbRoleInfo, roleDataEx);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B69 RID: 2921 RVA: 0x00086BCC File Offset: 0x00084DCC
		private static TCPProcessCmdResults ProcessAddHuangFeiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int otherRoleID = Convert.ToInt32(fields[1]);
				string otherRoleName = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbRoleInfo.Faction);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbRoleInfo.Faction)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBQuery.QueryHuangFeiCount(dbMgr) >= 3)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1030,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleToHuangFei(dbMgr, otherRoleID, 1);
				DBRoleInfo otherDbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref otherRoleID);
				if (null != otherDbRoleInfo)
				{
					lock (otherDbRoleInfo)
					{
						otherDbRoleInfo.HuangHou = 1;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					otherRoleID,
					otherRoleName
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6A RID: 2922 RVA: 0x00087028 File Offset: 0x00085228
		private static TCPProcessCmdResults ProcessRemoveHuangFeiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int otherRoleID = Convert.ToInt32(fields[1]);
				string otherRoleName = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbRoleInfo.Faction);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbRoleInfo.Faction)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleToHuangFei(dbMgr, otherRoleID, 0);
				DBRoleInfo otherDbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref otherRoleID);
				if (null != otherDbRoleInfo)
				{
					lock (otherDbRoleInfo)
					{
						otherDbRoleInfo.HuangHou = 0;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					otherRoleID,
					otherRoleName
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00087424 File Offset: 0x00085624
		private static TCPProcessCmdResults ProcessGetHuangFeiDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<SearchRoleData> huangFeiDataList = DBQuery.QueryHuangFeiDataList(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(huangFeiDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6C RID: 2924 RVA: 0x00087550 File Offset: 0x00085750
		private static TCPProcessCmdResults ProcessSendToLaoFangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int otherRoleID = Convert.ToInt32(fields[1]);
				string otherRoleName = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbRoleInfo.Faction);
				string strcmd;
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbRoleInfo.Faction)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (HuangDiTeQuanMgr.FindHuangDiToOtherRoleDict(nID, otherRoleID))
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1025,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!HuangDiTeQuanMgr.CanExecuteHuangDiTeQuanNow(nID))
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1026,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!HuangDiTeQuanMgr.AddHuanDiTeQuan(nID, otherRoleID))
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1030,
						roleID,
						otherRoleID,
						otherRoleName
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateHuangDiTeQuan(dbMgr, HuangDiTeQuanMgr.GetHuangDiTeQuanItem());
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					roleID,
					otherRoleID,
					otherRoleName
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x00087A0C File Offset: 0x00085C0C
		private static TCPProcessCmdResults ProcessAddRefreshQiZhenRecCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int oldUserMoney = Convert.ToInt32(fields[1]);
				int leftUserMoney = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddRefreshQiZhenGeRec(dbMgr, roleID, oldUserMoney, leftUserMoney);
				string strcmd = string.Format("{0}:{1}", 0, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x00087BD8 File Offset: 0x00085DD8
		private static TCPProcessCmdResults ProcessClrCachingRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int otherRoleID = Convert.ToInt32(fields[0]);
				string otherRoleName = fields[1];
				if (otherRoleID == 0)
				{
					otherRoleID = dbMgr.DBRoleMgr.FindDBRoleID(otherRoleName);
					if (otherRoleID > 0)
					{
						dbMgr.DBRoleMgr.ReleaseDBRoleInfoByID(otherRoleID);
					}
				}
				else if (otherRoleID == 1)
				{
					dbMgr.dbUserMgr.RemoveDBUserInfo(otherRoleName);
				}
				else if (otherRoleID > 100)
				{
					dbMgr.DBRoleMgr.ReleaseDBRoleInfoByID(otherRoleID);
				}
				string strcmd = string.Format("{0}:{1}", 0, otherRoleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x00087DB0 File Offset: 0x00085FB0
		private static TCPProcessCmdResults ProcessClrAllCachingRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int randNum = Convert.ToInt32(fields[0]);
				dbMgr.DBRoleMgr.ClearAllDBroleInfo();
				string strcmd = string.Format("{0}:{1}", 0, randNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x00087EFC File Offset: 0x000860FC
		private static TCPProcessCmdResults ProcessAddMoneyWarningCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int usedMoney = Convert.ToInt32(fields[1]);
				int goodsMoney = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddMoneyWarning(dbMgr, roleID, usedMoney, goodsMoney);
				string strcmd = string.Format("{0}:{1}", 0, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000880C8 File Offset: 0x000862C8
		private static TCPProcessCmdResults ProcessGetGoodsByDbIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsDbID = Convert.ToInt32(fields[1]);
				GoodsData goodsData = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				goodsData = Global.GetGoodsDataByDbID(dbRoleInfo, goodsDbID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x00088230 File Offset: 0x00086430
		private static TCPProcessCmdResults ProcessQueryChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int zoneID = Convert.ToInt32(fields[1]);
				int userMoney = 0;
				int realMoney = 0;
				DBQuery.QueryUserMoneyByUserID(dbMgr, userID, out userMoney, out realMoney);
				string strcmd = string.Format("{0}", realMoney);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x00088380 File Offset: 0x00086580
		private static TCPProcessCmdResults ProcessQueryUserIdValueCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int[] arr = new int[2];
				DBQuery.QueryUserUserIdValue(dbMgr, userID, out arr[0], out arr[1]);
				arr[0] *= GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 1);
				byte[] bytes = DataHelper.ObjectToBytes<int[]>(arr);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x000884F0 File Offset: 0x000866F0
		private static TCPProcessCmdResults ProcessQueryDayChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				int zoneID = Convert.ToInt32(fields[1]);
				int userMoney = 0;
				int realMoney = 0;
				DBQuery.QueryTodayUserMoneyByUserID(dbMgr, userID, zoneID, out userMoney, out realMoney);
				int userMoney2 = 0;
				int realMoney2 = 0;
				DBQuery.QueryTodayUserMoneyByUserID2(dbMgr, userID, zoneID, out userMoney2, out realMoney2);
				string strcmd = string.Format("{0}", realMoney + realMoney2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x00088658 File Offset: 0x00086858
		private static TCPProcessCmdResults ProcessQueryPeriodChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime startTime;
				DateTime.TryParse(fromDate, out startTime);
				DateTime endTime;
				DateTime.TryParse(toDate, out endTime);
				RankDataKey key = new RankDataKey(RankType.Charge, startTime, endTime, null);
				int money = roleInfo.RankValue.GetRankValue(key);
				string strcmd = string.Format("{0}", money);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x00088848 File Offset: 0x00086A48
		private static TCPProcessCmdResults ProcessAddBuyItemFromNpcCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftMoney = Convert.ToInt32(fields[4]);
				int moneyType = Convert.ToInt32(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewBuyItemFromNpc(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney, moneyType);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x00088A88 File Offset: 0x00086C88
		private static TCPProcessCmdResults ProcessAddYinLiangBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftYinLiang = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewYinLiangBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftYinLiang);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的银两购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x00088CBC File Offset: 0x00086EBC
		private static TCPProcessCmdResults ProcessAddBangGongBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftBangGong = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewBangGongBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftBangGong);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的帮贡购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x00088EF0 File Offset: 0x000870F0
		private static TCPProcessCmdResults ProcessGetUserMailListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<MailData> mailItemDataList = Global.LoadUserMailItemDataList(dbMgr, roleID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<MailData>>(mailItemDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0008901C File Offset: 0x0008721C
		private static TCPProcessCmdResults ProcessGetUserMailCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int excludeReadState = Convert.ToInt32(fields[1]);
				int limitCount = Convert.ToInt32(fields[2]);
				int emailCount = Global.LoadUserMailItemDataCount(dbMgr, roleID, excludeReadState, limitCount);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(emailCount, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x00089160 File Offset: 0x00087360
		private static TCPProcessCmdResults ProcessGetUserMailDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int mailID = Convert.ToInt32(fields[1]);
				MailData mailItemData = Global.LoadMailItemData(dbMgr, roleID, mailID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MailData>(mailItemData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x00089298 File Offset: 0x00087498
		private static TCPProcessCmdResults ProcessSendUserMailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 10 && fields.Length != 11)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int receiverrid = Convert.ToInt32(fields[2]);
				if (fields.Length == 11)
				{
					int checkReceiverExist = Convert.ToInt32(fields[10]);
					if (checkReceiverExist != 0 && DBManager.getInstance().GetDBRoleInfo(ref receiverrid) == null)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int addGoodsCount = 0;
				int mailID = Global.AddMail(dbMgr, fields, out addGoodsCount);
				string strcmd = string.Format("{0}:{1}:{2}", roleID, mailID, addGoodsCount);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00089470 File Offset: 0x00087670
		private static TCPProcessCmdResults ProcessFetchMailGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int mailID = Convert.ToInt32(fields[1]);
				bool ret = Global.UpdateHasFetchMailGoodsStat(dbMgr, roleID, mailID);
				string strcmd = string.Format("{0}:{1}:{2}", roleID, mailID, ret ? 1 : -1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x000895D0 File Offset: 0x000877D0
		private static TCPProcessCmdResults ProcessDeleteUserMailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string mailIDs = fields[1];
				bool ret = false;
				string[] strID = mailIDs.Split(new char[]
				{
					','
				});
				string strRet = null;
				if (strID != null)
				{
					for (int i = 0; i < strID.Length; i++)
					{
						ret = Global.DeleteMail(dbMgr, roleID, strID[i]);
						if (ret)
						{
							string strTmp = strID[i] + ",";
							strRet += strTmp;
						}
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}", roleID, strRet, ret ? 1 : -1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x000897B8 File Offset: 0x000879B8
		private static TCPProcessCmdResults ProcessGetRoleIDByRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = fields[0].Replace('$', ':');
				int roleID = Global.FindDBRoleID(dbMgr, roleName);
				string strcmd = string.Format("{0}:{1}", roleID, roleID, roleName);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x00089904 File Offset: 0x00087B04
		private static TCPProcessCmdResults ProcessSprQueryInputFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				double addPercent = Convert.ToDouble(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				int fanliYuanBao = (int)((double)roleYuanBaoInPeriod * addPercent);
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, fanliYuanBao);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x00089B14 File Offset: 0x00087D14
		private static TCPProcessCmdResults ProcessSprQueryInputJiaSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int gateYuanBao = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (gateYuanBao <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1002, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1004, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				if (roleYuanBaoInPeriod < gateYuanBao)
				{
					strcmd = string.Format("{0}:{1}:0", -1005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, roleYuanBaoInPeriod);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x00089DBC File Offset: 0x00087FBC
		private static TCPProcessCmdResults ProcessSprQueryInputKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, 3);
				foreach (InputKingPaiHangData item2 in listPaiHang)
				{
					Global.GetUserMaxLevelRole(dbMgr, item2.UserID, out item2.MaxLevelRoleName, out item2.MaxLevelRoleZoneID);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<InputKingPaiHangData>>(listPaiHang, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x00089FE8 File Offset: 0x000881E8
		private static TCPProcessCmdResults ProcessSprQueryLevelKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, fields, 5, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0008A0FC File Offset: 0x000882FC
		private static TCPProcessCmdResults ProcessSprQueryEquipKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, fields, 6, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0008A210 File Offset: 0x00088410
		private static TCPProcessCmdResults ProcessSprQueryHorseKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, fields, 7, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0008A324 File Offset: 0x00088524
		private static TCPProcessCmdResults ProcessSprQueryJingMaiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, fields, 8, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0008A438 File Offset: 0x00088638
		private static TCPProcessCmdResults ProcessExcuteInputFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				double addPercent = Convert.ToDouble(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 2, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 2, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1006, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, roleYuanBaoInPeriod);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0008A784 File Offset: 0x00088984
		private static TCPProcessCmdResults ProcessExcuteInputJiaSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int gateYuanBao = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (gateYuanBao <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1002, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				if (roleYuanBaoInPeriod < gateYuanBao)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 3, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 3, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1007, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, roleYuanBaoInPeriod);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0008AB4C File Offset: 0x00088D4C
		private static TCPProcessCmdResults ProcessExcuteInputKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, 3);
				int paiHang = -1;
				int inputMoneyInPeriod = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.UserID == listPaiHang[i].UserID)
					{
						paiHang = listPaiHang[i].PaiHang;
						inputMoneyInPeriod = listPaiHang[i].PaiHangValue;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 4, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 4, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, paiHang);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x0008AFB8 File Offset: 0x000891B8
		private static TCPProcessCmdResults ProcessExcuteLevelKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, fields, 5, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x0008B0CC File Offset: 0x000892CC
		private static TCPProcessCmdResults ProcessExcuteEquipKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, fields, 6, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0008B1E0 File Offset: 0x000893E0
		private static TCPProcessCmdResults ProcessExcuteHorseKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, fields, 7, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0008B2F4 File Offset: 0x000894F4
		private static TCPProcessCmdResults ProcessExcuteJingMaiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, fields, 8, out tcpOutPacket);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0008B408 File Offset: 0x00089608
		private static TCPProcessCmdResults ProcessSprQueryAwardHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int activityType = Global.SafeConvertToInt32(fields[3], 10);
				int exTag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				if (2 == activityType || 3 == activityType || 4 == activityType || 69 == activityType)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, activityType, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (69 == activityType)
					{
						if (hasgettimes > 0)
						{
							hasgettimes = (int)Global.GetBitRangeValue((long)hasgettimes, (exTag - 1) / 2 * 7, 7);
						}
					}
				}
				else
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, activityType, huoDongKeyStr, out hasgettimes, out lastgettime);
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					activityType,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0008B6AC File Offset: 0x000898AC
		private static TCPProcessCmdResults ProcessSprQueryUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string huoDongKeyStr = fields[1];
				int activityType = Global.SafeConvertToInt32(fields[2], 10);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long hasgettimes = 0L;
				string lastgettime = "";
				int ret = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, activityType, huoDongKeyStr, out hasgettimes, out lastgettime);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					ret,
					roleID,
					activityType,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0008B8B0 File Offset: 0x00089AB0
		private static TCPProcessCmdResults ProcessSprUpdateUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string huoDongKeyStr = fields[1];
				int activityType = Global.SafeConvertToInt32(fields[2], 10);
				long hasgettimes = Global.SafeConvertToInt64(fields[3], 10);
				string lastgettime = fields[4];
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = 0;
				lock (roleInfo)
				{
					ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activityType, huoDongKeyStr, hasgettimes, lastgettime);
					if (ret < 0)
					{
						ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, activityType, huoDongKeyStr, hasgettimes, lastgettime);
					}
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					ret,
					roleID,
					activityType,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0008BB24 File Offset: 0x00089D24
		private static TCPProcessCmdResults ProcessDBQueryLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int dayID = 0;
				int usedNum = 0;
				int ret = DBQuery.QueryLimitGoodsUsedNumByRoleID(dbMgr, roleID, goodsID, out dayID, out usedNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						ret,
						dayID,
						usedNum
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("通过角色ID和物品ID查询物品每日的已经购买数量失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						0,
						dayID,
						usedNum
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0008BD3C File Offset: 0x00089F3C
		private static TCPProcessCmdResults ProcessDBUpdateLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int dayID = Convert.ToInt32(fields[2]);
				int usedNum = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddLimitGoodsBuyItem(dbMgr, roleID, goodsID, dayID, usedNum);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加限购物品的历史记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0008BF64 File Offset: 0x0008A164
		private static TCPProcessCmdResults ProcessUpdateDailyVipDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dayID = Convert.ToInt32(fields[1]);
				int priorityType = Convert.ToInt32(fields[2]);
				int usedTimes = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					if (null == dbRoleInfo.VipDailyDataList)
					{
						dbRoleInfo.VipDailyDataList = new List<VipDailyData>();
					}
					bool found = false;
					VipDailyData dailyVipData = null;
					for (int i = 0; i < dbRoleInfo.VipDailyDataList.Count; i++)
					{
						if (dbRoleInfo.VipDailyDataList[i].PriorityType == priorityType)
						{
							found = true;
							dailyVipData = dbRoleInfo.VipDailyDataList[i];
							break;
						}
					}
					if (!found)
					{
						dailyVipData = new VipDailyData();
						dbRoleInfo.VipDailyDataList.Add(dailyVipData);
					}
					dailyVipData.DayID = dayID;
					dailyVipData.PriorityType = priorityType;
					dailyVipData.UsedTimes = usedTimes;
				}
				string strcmd;
				if (DBWriter.AddVipDailyData(dbMgr, roleID, priorityType, dayID, usedTimes) >= 0)
				{
					strcmd = string.Format("1:{0}:{1}", roleID, priorityType);
				}
				else
				{
					strcmd = string.Format("-1:{0}:{1}", roleID, priorityType);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x0008C264 File Offset: 0x0008A464
		private static TCPProcessCmdResults ProcessUpdateYangGongBKDailyJiFenDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dayID = Convert.ToInt32(fields[1]);
				int jifen = Convert.ToInt32(fields[2]);
				int awardhistory = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (DBWriter.AddYangGongBKDailyJiFenData(dbMgr, roleID, jifen, dayID, (long)awardhistory) >= 0)
				{
					strcmd = string.Format("1:{0}:{1}", roleID, jifen);
					if (null == dbRoleInfo.YangGongBKDailyJiFen)
					{
						dbRoleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData();
					}
					dbRoleInfo.YangGongBKDailyJiFen.DayID = dayID;
					dbRoleInfo.YangGongBKDailyJiFen.JiFen = jifen;
					dbRoleInfo.YangGongBKDailyJiFen.AwardHistory = (long)awardhistory;
				}
				else
				{
					strcmd = string.Format("-1:{0}:{1}", roleID, jifen);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x0008C4B4 File Offset: 0x0008A6B4
		private static TCPProcessCmdResults ProcessUpdateSingleTimeAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				long onceAwardFlag = Convert.ToInt64(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleOnceAwardFlag(dbMgr, roleID, onceAwardFlag))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色充值任务ID时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.OnceAwardFlag = onceAwardFlag;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x0008C708 File Offset: 0x0008A908
		private static TCPProcessCmdResults ProcessAddShengXiaoGuessHisotryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] sGuessResults = fields[0].Split(new char[]
				{
					';'
				});
				for (int i = 0; i < sGuessResults.Length; i++)
				{
					string[] singleRoleResults = sGuessResults[0].Split(new char[]
					{
						','
					});
					if (singleRoleResults.Length == 2)
					{
						int roleID = Convert.ToInt32(singleRoleResults[0]);
						DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
						if (null == dbRoleInfo)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("记录竞猜历史是需要处理的的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						}
						else
						{
							string roleName = dbRoleInfo.RoleName;
							int zoneID = dbRoleInfo.ZoneID;
							string[] guessItems = singleRoleResults[1].Split(new char[]
							{
								'|'
							});
							for (int j = 0; j < guessItems.Length; j++)
							{
								string[] itemFields = guessItems[j].Split(new char[]
								{
									'_'
								});
								if (itemFields.Length == 5)
								{
									int guessKey = Convert.ToInt32(itemFields[0]);
									int mortgage = Convert.ToInt32(itemFields[1]);
									int resultKey = Convert.ToInt32(itemFields[2]);
									int gainNum = Convert.ToInt32(itemFields[3]);
									int leftMortgage = Convert.ToInt32(itemFields[4]);
									int ret = DBWriter.AddNewShengXiaoGuessHistory(dbMgr, roleID, roleName, zoneID, guessKey, mortgage, resultKey, gainNum, leftMortgage);
									if (ret < 0)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("添加新的生肖竞猜记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
									}
								}
							}
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x0008CA00 File Offset: 0x0008AC00
		private static TCPProcessCmdResults ProcessQueryShengXiaoGuessHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<ShengXiaoGuessHistory> list = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ShengXiaoGuessHistory>>(list, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				list = DBQuery.QueryShengXiaoGuessHistoryDataList(dbMgr, Convert.ToInt32(fields[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ShengXiaoGuessHistory>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x0008CB60 File Offset: 0x0008AD60
		private static TCPProcessCmdResults ProcessUpdateUserGoldCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int addOrSubUserGold = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				int userGold = 0;
				lock (dbRoleInfo)
				{
					dbRoleInfo.Gold += addOrSubUserGold;
					userGold = dbRoleInfo.Gold;
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (addOrSubUserGold != 0)
				{
					if (!DBWriter.UpdateRoleGold(dbMgr, roleID, userGold))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", roleID, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				strcmd = string.Format("{0}:{1}", roleID, userGold);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0008CE28 File Offset: 0x0008B028
		private static TCPProcessCmdResults ProcessAddGoldBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftGold = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.AddNewGoldBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftGold);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的金币购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0008D05C File Offset: 0x0008B25C
		private static TCPProcessCmdResults ProcessUpdateGoodsLimitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int dayID = Convert.ToInt32(fields[2]);
				int usedNum = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateGoodsLimit(dbMgr, roleID, goodsID, dayID, usedNum))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色物品限制时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					Global.UpdateGoodsLimitByID(dbRoleInfo, goodsID, dayID, usedNum);
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0008D28C File Offset: 0x0008B48C
		private static TCPProcessCmdResults ProcessUpdateRoleParamCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string name = fields[1];
				string value = fields[2];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Global.UpdateRoleParamByName(dbMgr, dbRoleInfo, name, value, null);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0008D450 File Offset: 0x0008B650
		private static TCPProcessCmdResults ProcessGetRoleParamCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string name = fields[1];
				string value = DBWriter.GetRoleParams(roleID, name);
				string strcmd = string.Format("{0}", value);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0008D598 File Offset: 0x0008B798
		private static TCPProcessCmdResults ProcessUpdateWebOldPlayerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleID = fields[0];
				string chouJiangType = "v" + fields[1];
				string addTime = fields[2].Replace('$', ':');
				bool ret = DBWriter.UpdateWebOldPlayer(roleID, chouJiangType, addTime);
				string strcmd = string.Format("{0}", 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0008D6FC File Offset: 0x0008B8FC
		private static TCPProcessCmdResults ProcessAddQiangGouBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int goodsNum = Convert.ToInt32(fields[2]);
				int totalPrice = Convert.ToInt32(fields[3]);
				int leftMoney = Convert.ToInt32(fields[4]);
				int qiangGouID = Convert.ToInt32(fields[5]);
				int actStartDay = Convert.ToInt32(fields[6]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = Global.AddNewQiangGouBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney, qiangGouID, actStartDay);
				string strcmd;
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的限时抢购购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, ret);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0008D94C File Offset: 0x0008BB4C
		private static TCPProcessCmdResults ProcessQueryQiangGouBuyItemInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int goodsID = Convert.ToInt32(fields[1]);
				int qiangGouID = Convert.ToInt32(fields[2]);
				int random = Convert.ToInt32(fields[3]);
				int actStartDay = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleBuyNum = 0;
				int totalBuyNum = 0;
				Global.QueryQiangGouBuyItemInfo(dbMgr, roleID, goodsID, qiangGouID, random, actStartDay, out roleBuyNum, out totalBuyNum);
				string strcmd = string.Format("{0}:{1}:{2}", roleID, roleBuyNum, totalBuyNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0008DB3C File Offset: 0x0008BD3C
		private static TCPProcessCmdResults ProcessAddZaJinDanHisotryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.DisableSomeLog)
				{
					string[] lines = fields[0].Split(new char[]
					{
						';'
					});
					for (int i = 0; i < lines.Length; i++)
					{
						string[] lineFields = lines[i].Split(new char[]
						{
							'_'
						});
						if (lineFields.Length >= 12)
						{
							int rid = Convert.ToInt32(lineFields[0]);
							string rname;
							string uid;
							Global.GetRoleNameAndUserID(dbMgr, rid, out rname, out uid);
							int zoneid = Convert.ToInt32(lineFields[2]);
							int timesselected = Convert.ToInt32(lineFields[3]);
							int usedyuanbao = Convert.ToInt32(lineFields[4]);
							int usedjindan = Convert.ToInt32(lineFields[5]);
							int gaingoodsid = Convert.ToInt32(lineFields[6]);
							int gaingoodsnum = Convert.ToInt32(lineFields[7]);
							int gaingold = Convert.ToInt32(lineFields[8]);
							int gainyinliang = Convert.ToInt32(lineFields[9]);
							int gainexp = Convert.ToInt32(lineFields[10]);
							string strPorp = lineFields[11];
							int ret = DBWriter.AddNewZaJinDanHistory(dbMgr, rid, rname, zoneid, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, strPorp);
							if (ret < 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("添加新的砸金蛋记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, rid), null, true);
							}
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0008DDD8 File Offset: 0x0008BFD8
		private static TCPProcessCmdResults ProcessQueryZaJinDanHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<ZaJinDanHistory> list = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ZaJinDanHistory>>(list, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				list = DBQuery.QueryZaJinDanHistoryDataList(dbMgr, Convert.ToInt32(fields[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ZaJinDanHistory>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0008DF38 File Offset: 0x0008C138
		private static TCPProcessCmdResults ProcessQueryFirstChongZhiDaLiByUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalNum = DBQuery.GetFirstChongZhiDaLiNum(dbMgr, dbRoleInfo.UserID);
				strcmd = string.Format("{0}", totalNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0008E0C0 File Offset: 0x0008C2C0
		private static TCPProcessCmdResults ProcessQueryDayChongZhiDaLiByUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dayID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalNum = 0;
				strcmd = string.Format("{0}", totalNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0008E24C File Offset: 0x0008C44C
		private static TCPProcessCmdResults ProcessQueryKaiFuOnlineAwardRoleIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayID = Convert.ToInt32(fields[0]);
				int totalRoleNum = 0;
				int roleID = DBQuery.GetKaiFuOnlineAwardRoleID(dbMgr, dayID, out totalRoleNum);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1,
						0,
						"",
						0
					});
				}
				else
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						roleID,
						dbRoleInfo.ZoneID,
						dbRoleInfo.RoleName,
						totalRoleNum
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0008E440 File Offset: 0x0008C640
		private static TCPProcessCmdResults ProcessAddKaiFuOnlineAwardCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dayID = Convert.ToInt32(fields[1]);
				int yuanBao = Convert.ToInt32(fields[2]);
				int totalRoleNum = Convert.ToInt32(fields[3]);
				int zoneID = Convert.ToInt32(fields[4]);
				int ret = DBWriter.AddKaiFuOnlineAward(dbMgr, roleID, dayID, yuanBao, totalRoleNum, zoneID);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0008E5B8 File Offset: 0x0008C7B8
		private static TCPProcessCmdResults ProcessQueryKaiFuOnlineAwardListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(fields[1]);
				List<KaiFuOnlineAwardData> list = DBQuery.GetKaiFuOnlineAwardDataList(dbMgr, zoneID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<KaiFuOnlineAwardData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0008E6E4 File Offset: 0x0008C8E4
		private static TCPProcessCmdResults ProcessAddGiveUserMoneyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int yuanBao = Convert.ToInt32(fields[1]);
				string giveType = fields[2];
				int ret = DBWriter.AddSystemGiveUserMoney(dbMgr, roleID, yuanBao, giveType);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0008E83C File Offset: 0x0008CA3C
		private static TCPProcessCmdResults ProcessAddExchange1ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(fields[0]);
				int goodsid = Convert.ToInt32(fields[1]);
				int goodsnum = Convert.ToInt32(fields[2]);
				int leftgoodsnum = Convert.ToInt32(fields[3]);
				int otherroleid = Convert.ToInt32(fields[4]);
				string result = fields[5];
				int ret = DBWriter.AddExchange1Item(dbMgr, rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0008E9BC File Offset: 0x0008CBBC
		private static TCPProcessCmdResults ProcessAddExchange2ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(fields[0]);
				int yinliang = Convert.ToInt32(fields[1]);
				int leftyinliang = Convert.ToInt32(fields[2]);
				int otherroleid = Convert.ToInt32(fields[3]);
				int ret = DBWriter.AddExchange2Item(dbMgr, rid, yinliang, leftyinliang, otherroleid);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0008EB28 File Offset: 0x0008CD28
		private static TCPProcessCmdResults ProcessAddExchange3ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(fields[0]);
				int yuanbao = Convert.ToInt32(fields[1]);
				int leftyuanbao = Convert.ToInt32(fields[2]);
				int otherroleid = Convert.ToInt32(fields[3]);
				int ret = DBWriter.AddExchange3Item(dbMgr, rid, yuanbao, leftyuanbao, otherroleid);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0008EC94 File Offset: 0x0008CE94
		private static TCPProcessCmdResults ProcessAddFallGoodsItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 12)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(fields[0]);
				int autoid = Convert.ToInt32(fields[1]);
				int goodsdbid = Convert.ToInt32(fields[2]);
				int goodsid = Convert.ToInt32(fields[3]);
				int goodsnum = Convert.ToInt32(fields[4]);
				int binding = Convert.ToInt32(fields[5]);
				int quality = Convert.ToInt32(fields[6]);
				int forgelevel = Convert.ToInt32(fields[7]);
				string jewellist = fields[8];
				string mapname = fields[9];
				string goodsgrid = fields[10];
				string fromname = fields[11];
				int ret = DBWriter.AddFallGoodsItem(dbMgr, rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname);
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0008EE68 File Offset: 0x0008D068
		private static TCPProcessCmdResults ProcessUpdateRolePropsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int rolePropIindex = Convert.ToInt32(fields[1]);
				long propValue = Convert.ToInt64(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool ret = false;
				switch (rolePropIindex)
				{
				case 1:
					ret = DBWriter.UpdateRoleBanProps(dbMgr, roleID, "banchat", propValue);
					break;
				case 2:
					ret = DBWriter.UpdateRoleBanProps(dbMgr, roleID, "banlogin", propValue);
					break;
				case 3:
					ret = DBWriter.UpdateRoleBanProps(dbMgr, roleID, "ban_trade_to_ticks", propValue);
					break;
				}
				string strcmd;
				if (!ret)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色属性值时失败，CMD={0}, RoleID={1}, PropIndex={2}", (TCPGameServerCmds)nID, roleID, rolePropIindex), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						switch (rolePropIindex)
						{
						case 1:
							dbRoleInfo.BanChat = (int)propValue;
							break;
						case 2:
							dbRoleInfo.BanLogin = (int)propValue;
							break;
						case 3:
							dbRoleInfo.BanTradeToTicks = propValue;
							break;
						}
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0008F154 File Offset: 0x0008D354
		private static TCPProcessCmdResults ProcessQueryThemeDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 151, huoDongKeyStr, out hasgettimes, out lastgettime);
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x0008F34C File Offset: 0x0008D54C
		private static TCPProcessCmdResults ProcessQueryJieriDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 9, huoDongKeyStr, out hasgettimes, out lastgettime);
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0008F540 File Offset: 0x0008D740
		private static TCPProcessCmdResults ProcessQueryJieriDengLuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int dengLuTimes = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 10, huoDongKeyStr, out hasgettimes, out lastgettime);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					hasgettimes,
					dengLuTimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0008F760 File Offset: 0x0008D960
		private static TCPProcessCmdResults ProcessQueryJieriVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int isVip = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 11, huoDongKeyStr, out hasgettimes, out lastgettime);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					hasgettimes,
					isVip
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0008F980 File Offset: 0x0008DB80
		private static TCPProcessCmdResults ProcessQueryJieriCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int minYuanBao = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 12, huoDongKeyStr, out hasgettimes, out lastgettime);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					roleID,
					minYuanBao,
					roleYuanBaoInPeriod,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0008FBDC File Offset: 0x0008DDDC
		private static TCPProcessCmdResults ProcessQueryJieriCZLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 13, huoDongKeyStr, out hasgettimes, out lastgettime);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					roleYuanBaoInPeriod,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x0008FE40 File Offset: 0x0008E040
		private static TCPProcessCmdResults ProcessQueryJieRiMeiRiLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int minYuanBao = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 70, huoDongKeyStr, out hasgettimes, out lastgettime);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					roleYuanBaoInPeriod,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x00090094 File Offset: 0x0008E294
		private static TCPProcessCmdResults ProcessQueryJieriTotalConsumeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 40, huoDongKeyStr, out hasgettimes, out lastgettime);
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					roleID,
					roleYuanBaoInPeriod,
					hasgettimes
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000902F8 File Offset: 0x0008E4F8
		private static TCPProcessCmdResults ProcessQueryJieriXiaoFeiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				int extTag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, minGateValueList.Count);
				foreach (InputKingPaiHangData item2 in listPaiHang)
				{
					string strUserID;
					Global.GetRoleNameAndUserID(dbMgr, Global.SafeConvertToInt32(item2.UserID, 10), out item2.MaxLevelRoleName, out strUserID);
				}
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 15, huoDongKeyStr, out hasgettimes, out lastgettime);
				int paiHang = -1;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.RoleID.ToString() == listPaiHang[i].UserID)
					{
						paiHang = listPaiHang[i].PaiHang;
					}
				}
				if (inputMoneyInPeriod <= 0)
				{
					paiHang = -1;
				}
				if (0 == extTag)
				{
					JieriCZKingData jieriCZKingData = new JieriCZKingData
					{
						YuanBao = roleYuanBaoInPeriod,
						ListPaiHang = listPaiHang,
						State = hasgettimes
					};
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(jieriCZKingData, pool, nID);
				}
				else if (1 == extTag)
				{
					string strCmd = string.Format("{0}:{1}:{2}", 1, roleID, (paiHang > 0 && hasgettimes == 0) ? "1" : "0");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x000906E0 File Offset: 0x0008E8E0
		private static TCPProcessCmdResults ProcessInputPointsExchangeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int nActType = Convert.ToInt32(fields[3]);
				int extTag = Convert.ToInt32(fields[4]);
				extTag = Math.Max(0, extTag);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 64, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (hasgettimes <= 0)
				{
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 64, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}", roleID, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", hasgettimes, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x000909BC File Offset: 0x0008EBBC
		private static TCPProcessCmdResults ProcessExecuteDanBiChongZhiJiangLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int acType = Convert.ToInt32(fields[3]);
				int JiangLiLv = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (acType != 69)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long hasgettimes = 0L;
				string lastgettime = "";
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int ret = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 69, huoDongKeyStr, out hasgettimes, out lastgettime);
				hasgettimes = ((hasgettimes > 0L) ? hasgettimes : 0L);
				long mask = hasgettimes;
				long awardCount = Global.GetBitRangeValue(hasgettimes, 0, (JiangLiLv - 1) * 7);
				mask >>= (JiangLiLv - 1) * 7;
				mask += 1L;
				mask <<= (JiangLiLv - 1) * 7;
				hasgettimes = mask + awardCount;
				ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 69, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				if (ret < 0)
				{
					ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 69, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				}
				if (ret < 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1008, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, JiangLiLv);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00090CEC File Offset: 0x0008EEEC
		private static TCPProcessCmdResults ProcessQueryDanBiChongZhiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] YuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, int> chargeDic = roleInfo.RankValue.GetUserInputMoneyCount(dbMgr, roleInfo.UserID, roleInfo.ZoneID, fromDate, toDate);
				Dictionary<string, string> chargeInfo = new Dictionary<string, string>();
				long hasgettimes = 0L;
				string lastgettime = "";
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 69, huoDongKeyStr, out hasgettimes, out lastgettime);
				for (int i = 0; i < YuanBaoArr.Length; i += 2)
				{
					string key = string.Format("{0}_{1}", YuanBaoArr[i], YuanBaoArr[i + 1]);
					int minValue = Convert.ToInt32(YuanBaoArr[i]);
					int maxValue = Convert.ToInt32(YuanBaoArr[i + 1]);
					if (maxValue == -1)
					{
						maxValue = int.MaxValue;
					}
					int chargeCount = 0;
					foreach (KeyValuePair<int, int> item in chargeDic)
					{
						int chargeVlue = item.Key;
						if (chargeVlue <= maxValue && chargeVlue >= minValue)
						{
							chargeCount += item.Value;
						}
					}
					int awardCount = 0;
					if (hasgettimes > 0L)
					{
						awardCount = (int)Global.GetBitRangeValue(hasgettimes, i / 2 * 7, 7);
					}
					chargeInfo[key] = chargeCount.ToString() + "_" + awardCount;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(chargeInfo, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x00091034 File Offset: 0x0008F234
		private static TCPProcessCmdResults ProcessQueryJieriCZKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				int extTag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, minGateValueList.Count);
				foreach (InputKingPaiHangData item2 in listPaiHang)
				{
					Global.GetUserMaxLevelRole(dbMgr, item2.UserID, out item2.MaxLevelRoleName, out item2.MaxLevelRoleZoneID);
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 16, huoDongKeyStr, out hasgettimes, out lastgettime);
				int paiHang = -1;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.UserID == listPaiHang[i].UserID)
					{
						paiHang = listPaiHang[i].PaiHang;
					}
				}
				if (inputMoneyInPeriod <= 0)
				{
					paiHang = -1;
				}
				if (0 == extTag)
				{
					JieriCZKingData jieriCZKingData = new JieriCZKingData
					{
						YuanBao = roleYuanBaoInPeriod,
						ListPaiHang = listPaiHang,
						State = hasgettimes
					};
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(jieriCZKingData, pool, nID);
				}
				else if (1 == extTag)
				{
					string strCmd = string.Format("{0}:{1}:{2}", 1, roleID, (paiHang > 0 && hasgettimes == 0) ? "1" : "0");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x00091408 File Offset: 0x0008F608
		private static TCPProcessCmdResults ProcessExecuteThemeDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 151, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					hasgettimes = 1;
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 151, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x000916F8 File Offset: 0x0008F8F8
		private static TCPProcessCmdResults ProcessExecuteJieriDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 9, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					hasgettimes = 1;
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 9, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x000919E0 File Offset: 0x0008FBE0
		private static TCPProcessCmdResults ProcessExecuteJieriDengLuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int dengLuTimes = Convert.ToInt32(fields[3]);
				int extTag = Convert.ToInt32(fields[4]);
				extTag = Math.Max(0, extTag);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 10, huoDongKeyStr, out hasgettimes, out lastgettime);
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dengLuTimes < extTag)
				{
					strcmd = string.Format("{0}:{1}:0", -10077, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (roleInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 10, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 10, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", hasgettimes, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00091DBC File Offset: 0x0008FFBC
		private static TCPProcessCmdResults ProcessExecuteJieriVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int isVip = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (isVip <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10099, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 11, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					hasgettimes = 1;
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 11, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x000920EC File Offset: 0x000902EC
		private static TCPProcessCmdResults ProcessExecuteJieriCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int minYuanBao = Convert.ToInt32(fields[3]);
				int extTag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 12, huoDongKeyStr, out hasgettimes, out lastgettime);
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				if (roleYuanBaoInPeriod < minYuanBao)
				{
					strcmd = string.Format("{0}:{1}:0", -10088, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (userInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 12, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 12, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00092534 File Offset: 0x00090734
		private static TCPProcessCmdResults ProcessExecuteJieriMeiRiLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int minYuanBao = Convert.ToInt32(fields[3]);
				int extTag = Convert.ToInt32(fields[4]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 70, huoDongKeyStr, out hasgettimes, out lastgettime);
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				if (roleYuanBaoInPeriod < minYuanBao)
				{
					strcmd = string.Format("{0}:{1}:0", -10088, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (userInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 70, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 70, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x0009297C File Offset: 0x00090B7C
		private static TCPProcessCmdResults ProcessExecuteJieriCZLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				int extTag = Convert.ToInt32(fields[4]);
				extTag = Math.Max(0, extTag);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 13, huoDongKeyStr, out hasgettimes, out lastgettime);
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				int findIndex = -1;
				for (int i = 0; i < minGateValueList.Count; i++)
				{
					if (roleYuanBaoInPeriod < minGateValueList[i])
					{
						break;
					}
					findIndex = i;
				}
				if (findIndex < extTag - 1)
				{
					strcmd = string.Format("{0}:{1}:0", -10088, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (userInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 13, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 13, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", hasgettimes, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x00092E5C File Offset: 0x0009105C
		private static TCPProcessCmdResults ProcessExecuteJieriTotalConsumeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				int extTag = Convert.ToInt32(fields[4]);
				extTag = Math.Max(0, extTag);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 40, huoDongKeyStr, out hasgettimes, out lastgettime);
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				int roleYuanBaoInPeriod = inputMoneyInPeriod;
				int findIndex = -1;
				for (int i = 0; i < minGateValueList.Count; i++)
				{
					if (roleYuanBaoInPeriod < minGateValueList[i])
					{
						break;
					}
					findIndex = i;
				}
				if (findIndex < extTag - 1)
				{
					strcmd = string.Format("{0}:{1}:0", -10088, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (userInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 40, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 40, huoDongKeyStr, (long)hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", hasgettimes, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0009333C File Offset: 0x0009153C
		private static TCPProcessCmdResults ProcessExecuteJieriXiaoFeiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, minGateValueList.Count);
				int paiHang = -1;
				int inputMoneyInPeriod = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.RoleID.ToString() == listPaiHang[i].UserID)
					{
						paiHang = listPaiHang[i].PaiHang;
						inputMoneyInPeriod = listPaiHang[i].PaiHangValue;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 15, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 15, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, paiHang);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x000937C8 File Offset: 0x000919C8
		private static TCPProcessCmdResults ProcessExecuteJieriCZKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, minGateValueList, minGateValueList.Count);
				int paiHang = -1;
				int inputMoneyInPeriod = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (roleInfo.UserID == listPaiHang[i].UserID)
					{
						paiHang = listPaiHang[i].PaiHang;
						inputMoneyInPeriod = listPaiHang[i].PaiHangValue;
					}
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (inputMoneyInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (paiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 16, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 16, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, paiHang);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00093C3C File Offset: 0x00091E3C
		private static TCPProcessCmdResults ProcessQueryHeFuDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x00093C54 File Offset: 0x00091E54
		private static TCPProcessCmdResults ProcessQueryHeFuVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00093C6C File Offset: 0x00091E6C
		private static TCPProcessCmdResults ProcessQueryHeFuCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x00093C84 File Offset: 0x00091E84
		private static TCPProcessCmdResults ProcessQueryHeFuPKKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int kingID = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					string strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, roleID, roleInfo.ZoneID, 24, huoDongKeyStr, out hasgettimes, out lastgettime);
				DBRoleInfo kingDBRoleInfo = dbMgr.GetDBRoleInfo(ref kingID);
				HeFuPKKingData heFuPKKingData = new HeFuPKKingData
				{
					RoleID = ((kingDBRoleInfo != null) ? kingDBRoleInfo.RoleID : 0),
					RoleName = ((kingDBRoleInfo != null) ? kingDBRoleInfo.RoleName : ""),
					ZoneID = ((kingDBRoleInfo != null) ? kingDBRoleInfo.ZoneID : 0),
					State = hasgettimes
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HeFuPKKingData>(heFuPKKingData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x00093EC8 File Offset: 0x000920C8
		private static TCPProcessCmdResults ProcessQueryHeFuWCKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int kingID = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					string strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int hefuwckingaward = GameDBManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingaward", 0);
				int hasgettimes = hefuwckingaward;
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, kingID);
				HeFuPKKingData heFuPKKingData = new HeFuPKKingData
				{
					RoleID = ((bangHuiDetailData != null) ? bangHuiDetailData.BHID : 0),
					RoleName = ((bangHuiDetailData != null) ? bangHuiDetailData.BHName : ""),
					ZoneID = ((bangHuiDetailData != null) ? bangHuiDetailData.ZoneID : 0),
					State = hasgettimes
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HeFuPKKingData>(heFuPKKingData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x000940F8 File Offset: 0x000922F8
		private static TCPProcessCmdResults ProcessQueryHeFuCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00094110 File Offset: 0x00092310
		private static TCPProcessCmdResults ProcessQueryXinCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				DateTime now = DateTime.Now;
				string startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, startTime, endTime, minGateValueList, 5);
				foreach (InputKingPaiHangData item2 in listPaiHang)
				{
					Global.GetUserMaxLevelRole(dbMgr, item2.UserID, out item2.MaxLevelRoleName, out item2.MaxLevelRoleZoneID);
				}
				int hasgettimes = 0;
				string lastgettime = "";
				DateTime huodongStartTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(fromDate, out huodongStartTime);
				int roleYuanBaoInPeriod = 0;
				if (now.Ticks > huodongStartTime.Ticks + 864000000000L)
				{
					DateTime sub1DayDateTime = Global.GetAddDaysDataTime(now, -1, true);
					startTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
					endTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
					List<InputKingPaiHangData> listPaiHang2 = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, startTime, endTime, minGateValueList, 5);
					RankDataKey key = new RankDataKey(RankType.Charge, startTime, endTime, null);
					int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
					if (inputMoneyInPeriod < 0)
					{
						inputMoneyInPeriod = 0;
					}
					roleYuanBaoInPeriod = inputMoneyInPeriod;
					int selfPaiHang = 0;
					for (int i = 0; i < listPaiHang2.Count; i++)
					{
						if (listPaiHang2[i].UserID == roleInfo.UserID)
						{
							selfPaiHang = listPaiHang2[i].PaiHang;
							break;
						}
					}
					if (selfPaiHang > 0)
					{
						double fanLiPercent = (double)minGateValueList[selfPaiHang - 1] / 100.0;
						roleYuanBaoInPeriod = (int)(fanLiPercent * (double)roleYuanBaoInPeriod);
					}
					else
					{
						roleYuanBaoInPeriod = 0;
					}
					string huoDongKeyStr = Global.GetHuoDongKeyString(startTime, endTime);
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 30, huoDongKeyStr, out hasgettimes, out lastgettime);
				}
				JieriCZKingData jieriCZKingData = new JieriCZKingData
				{
					YuanBao = roleYuanBaoInPeriod,
					ListPaiHang = listPaiHang,
					State = hasgettimes
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(jieriCZKingData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x000945C8 File Offset: 0x000927C8
		private static TCPProcessCmdResults ProcessExecuteHeFuDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x000945E0 File Offset: 0x000927E0
		private static TCPProcessCmdResults ProcessExecuteHeFuVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x000945F8 File Offset: 0x000927F8
		private static TCPProcessCmdResults ProcessExecuteHeFuCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00094610 File Offset: 0x00092810
		private static TCPProcessCmdResults ProcessExecuteHeFuPKKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int kingID = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (kingID != roleInfo.RoleID)
				{
					strcmd = string.Format("{0}:{1}:0", -10089, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				lock (roleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 24, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					hasgettimes = 1;
					int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, 24, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x00094944 File Offset: 0x00092B44
		private static TCPProcessCmdResults ProcessExecuteHeFuWCKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int kingID = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (roleInfo.Faction != kingID)
				{
					strcmd = string.Format("{0}:{1}:0", -10065, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, kingID);
				if (null == bangHuiDetailData)
				{
					strcmd = string.Format("{0}:{1}:0", -10066, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != roleID)
				{
					strcmd = string.Format("{0}:{1}:0", -10067, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int hefuwckingaward = GameDBManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingaward", 0);
				int hasgettimes = hefuwckingaward;
				lock (roleInfo)
				{
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					hasgettimes = 1;
					GameDBManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingaward", hasgettimes.ToString());
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x00094C94 File Offset: 0x00092E94
		private static TCPProcessCmdResults ProcessExecuteHeFuCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00094CAC File Offset: 0x00092EAC
		private static TCPProcessCmdResults ProcessExecuteXinCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				string[] minYuanBaoArr = fields[3].Split(new char[]
				{
					'_'
				});
				List<int> minGateValueList = new List<int>();
				foreach (string item in minYuanBaoArr)
				{
					minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
				}
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int hasgettimes = 0;
				string lastgettime = "";
				DateTime now = DateTime.Now;
				DateTime huodongStartTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(fromDate, out huodongStartTime);
				int roleYuanBaoInPeriod = 0;
				if (now.Ticks <= huodongStartTime.Ticks + 864000000000L)
				{
					strcmd = string.Format("{0}:{1}:0", -1002, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime sub1DayDateTime = Global.GetAddDaysDataTime(now, -1, true);
				string startTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string endTime = new DateTime(sub1DayDateTime.Year, sub1DayDateTime.Month, sub1DayDateTime.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				List<InputKingPaiHangData> listPaiHang = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, startTime, endTime, minGateValueList, 5);
				RankDataKey key = new RankDataKey(RankType.Charge, startTime, endTime, null);
				int inputMoneyInPeriod = roleInfo.RankValue.GetRankValue(key);
				if (inputMoneyInPeriod < 0)
				{
					inputMoneyInPeriod = 0;
				}
				roleYuanBaoInPeriod = inputMoneyInPeriod;
				int selfPaiHang = 0;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (listPaiHang[i].UserID == roleInfo.UserID)
					{
						selfPaiHang = listPaiHang[i].PaiHang;
						break;
					}
				}
				if (selfPaiHang <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				double fanLiPercent = (double)minGateValueList[selfPaiHang - 1] / 100.0;
				roleYuanBaoInPeriod = (int)(fanLiPercent * (double)roleYuanBaoInPeriod);
				string huoDongKeyStr = Global.GetHuoDongKeyString(startTime, endTime);
				if (roleYuanBaoInPeriod <= 0)
				{
					strcmd = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				lock (userInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 30, huoDongKeyStr, out hasgettimes, out lastgettime);
					if (hasgettimes > 0)
					{
						strcmd = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, roleInfo.UserID, 30, huoDongKeyStr, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (ret < 0)
					{
						strcmd = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", 1, roleID, roleYuanBaoInPeriod);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00095204 File Offset: 0x00093404
		private static TCPProcessCmdResults ProcessQueryActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string sFromTime = fields[1].Replace("$", ":");
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (null == sFromTime)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime tStartTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(sFromTime, out tStartTime);
				DateTime dEndTime = Global.GetAddDaysDataTime(tStartTime, 3, true);
				string sEnd = dEndTime.ToString("yyyy-MM-dd HH:mm:ss");
				string sKeyStr = Global.GetHuoDongKeyString(sFromTime, sEnd);
				int nInputMoney = DBQuery.GetUserInputMoney(dbMgr, roleInfo.UserID, roleInfo.ZoneID, sFromTime, sEnd);
				int nIputYuanBao = Global.TransMoneyToYuanBao(nInputMoney);
				int nhasPlaytimes = 0;
				string slastgettimes = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, 31, sKeyStr, out nhasPlaytimes, out slastgettimes);
				DateTime now = DateTime.Now;
				tStartTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
				string sFrom = tStartTime.ToString("yyyy-MM-dd HH:mm:ss");
				dEndTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
				sEnd = dEndTime.ToString("yyyy-MM-dd HH:mm:ss");
				nInputMoney = DBQuery.GetUserInputMoney(dbMgr, roleInfo.UserID, roleInfo.ZoneID, sFrom, sEnd);
				int nIputYuanBao2 = Global.TransMoneyToYuanBao(nInputMoney);
				strcmd = string.Format("{0}:{1}:{2}", nIputYuanBao, nhasPlaytimes, nIputYuanBao2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x00095514 File Offset: 0x00093714
		private static TCPProcessCmdResults ProcessQueryXingYunChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nType = Convert.ToInt32(fields[1]);
				int nXingYunChouJiangYB = Convert.ToInt32(fields[2]);
				string sFromTime = fields[3].Replace("$", ":");
				string sToTime = fields[4].Replace("$", ":");
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}::", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nInputMoney = DBQuery.GetUserInputMoney(dbMgr, roleInfo.UserID, roleInfo.ZoneID, sFromTime, sToTime);
				int nIputYuanBao = Global.TransMoneyToYuanBao(nInputMoney);
				int nCanPlayCount = nIputYuanBao / nXingYunChouJiangYB;
				int nTimes = 0;
				string sLastgettime = "";
				string sKeyStr = Global.GetHuoDongKeyString(sFromTime, sToTime);
				int nActive = 0;
				if (nType == 1)
				{
					nActive = 31;
				}
				else if (nType == 2)
				{
					nActive = 32;
				}
				DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, nActive, sKeyStr, out nTimes, out sLastgettime);
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					nCanPlayCount,
					nTimes,
					nIputYuanBao
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0009579C File Offset: 0x0009399C
		private static TCPProcessCmdResults ProcessExcuteXingYunChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nTpye = Convert.ToInt32(fields[1]);
				int nHasPlayTime = Convert.ToInt32(fields[2]);
				string sFromTime = fields[3].Replace("$", ":");
				string sToTime = fields[4].Replace("$", ":");
				int nActTpye = 0;
				if (nTpye == 1)
				{
					nActTpye = 31;
				}
				else if (nTpye == 2)
				{
					nActTpye = 32;
				}
				string sKeyStr = Global.GetHuoDongKeyString(sFromTime, sToTime);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(dbRoleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (userInfo)
				{
					if (nHasPlayTime == 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userInfo.UserID, nActTpye, sKeyStr, (long)(nHasPlayTime + 1), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}", -1006, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, userInfo.UserID, nActTpye, sKeyStr, (long)(nHasPlayTime + 1), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}", 1, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x00095B1C File Offset: 0x00093D1C
		private static TCPProcessCmdResults ProcessBHMatchLoadSupportFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int minSeasonID = Convert.ToInt32(fields[1]);
				int minRound = Convert.ToInt32(fields[2]);
				List<BHMatchSupportData> bhMatchSupportData = DBQuery.LoadBHMatchSupportFlagData(dbMgr, roleID, minSeasonID, minRound);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<BHMatchSupportData>>(bhMatchSupportData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00095C60 File Offset: 0x00093E60
		private static TCPProcessCmdResults ProcessBHMatchUpdateSupportFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			BHMatchSupportData cmdData = null;
			try
			{
				cmdData = DataHelper.BytesToObject<BHMatchSupportData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string strcmd;
				if (!DBWriter.UpdateBHMatchSupportFlagData(dbMgr, cmdData))
				{
					strcmd = string.Format("{0}:{1}", -1008, cmdData.rid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}", 1, cmdData.rid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x00095D70 File Offset: 0x00093F70
		private static TCPProcessCmdResults ProcessQueryYueDuChouJiangHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				List<YueDuChouJiangData> list = null;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<YueDuChouJiangData>>(list, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				list = DBQuery.QueryYueDuChouJiangHistoryDataList(dbMgr, Convert.ToInt32(fields[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<YueDuChouJiangData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00095ED0 File Offset: 0x000940D0
		private static TCPProcessCmdResults ProcessExcuteAddYueDuChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] lines = fields[0].Split(new char[]
				{
					';'
				});
				for (int i = 0; i < lines.Length; i++)
				{
					string[] lineFields = lines[i].Split(new char[]
					{
						'_'
					});
					if (lineFields.Length >= 8)
					{
						int rid = Convert.ToInt32(lineFields[0]);
						string rname = lineFields[1];
						int zoneid = Convert.ToInt32(lineFields[2]);
						int gaingoodsid = Convert.ToInt32(lineFields[3]);
						int gaingoodsnum = Convert.ToInt32(lineFields[4]);
						int gaingold = Convert.ToInt32(lineFields[5]);
						int gainyinliang = Convert.ToInt32(lineFields[6]);
						int gainexp = Convert.ToInt32(lineFields[7]);
						int ret = DBWriter.AddNewYueDuChouJiangHistory(dbMgr, rid, rname, zoneid, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp);
						if (ret < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("添加新的月度抽奖记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, rid), null, true);
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x00096120 File Offset: 0x00094320
		private static TCPProcessCmdResults ProcessExecuteChangeOccupationCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nOccu = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateRoleOccupation(dbMgr, roleID, nOccu))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色经验和级别失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					string userID = "";
					lock (dbRoleInfo)
					{
						dbRoleInfo.Occupation = nOccu;
						userID = dbRoleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					if (userID != "")
					{
						DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
						if (null != dbUserInfo)
						{
							lock (dbUserInfo)
							{
								for (int i = 0; i < dbUserInfo.ListRoleOccups.Count; i++)
								{
									if (dbUserInfo.ListRoleIDs[i] == roleID)
									{
										dbUserInfo.ListRoleOccups[i] = nOccu;
									}
								}
							}
						}
					}
					strcmd = string.Format("{0}:{1}", roleID, nOccu);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x00096450 File Offset: 0x00094650
		private static TCPProcessCmdResults ProcessGetUsingGoodsDataListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				RoleData4Selector roleData4Selector = SingletonTemplate<RoleManager>.Instance().GetRoleData4Selector(roleID, false);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData4Selector>(roleData4Selector, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x00096580 File Offset: 0x00094780
		private static TCPProcessCmdResults ProcessQueryBloodCastleEnterCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nDate = Convert.ToInt32(fields[1]);
				int nType = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, roleID, nDate, nType);
				strcmd = string.Format("{0}:{1}", 1, nCount);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x00096728 File Offset: 0x00094928
		private static TCPProcessCmdResults ProcessUpdateBloodCastleEnterCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nDate = Convert.ToInt32(fields[1]);
				int nType = Convert.ToInt32(fields[2]);
				int nCount = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool bRet = DBWriter.UpdateBloodCastleEnterCount(dbMgr, roleID, nDate, nType, nCount, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				strcmd = string.Format("{0}:{1}", 1, bRet);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00096908 File Offset: 0x00094B08
		private static TCPProcessCmdResults ProcessQueryFuBenHisInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int fuBenID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				FuBenHistData fuBenHistData = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				if (fuBenHistData != null)
				{
					strcmd = string.Format("{0}:{1}:{2}", 1, fuBenHistData.RoleName, fuBenHistData.UsedSecs);
				}
				else
				{
					strcmd = string.Format("{0}:{1}:{2}", -1, "", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x00096AF8 File Offset: 0x00094CF8
		private static TCPProcessCmdResults ProcessCompleteFlashSceneCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool bRet = DBWriter.UpdateRoleInfoForFlashPlayerFlag(dbMgr, roleID, 0);
				strcmd = string.Format("{0}:{1}", roleID, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00096C84 File Offset: 0x00094E84
		private static TCPProcessCmdResults ProcessFinishFreshPlayerStatusCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(dbRoleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nIndex = -1;
				for (int i = 0; i < userInfo.ListRoleIDs.Count; i++)
				{
					if (userInfo.ListRoleIDs[i] == dbRoleInfo.RoleID)
					{
						nIndex = i;
						break;
					}
				}
				dbRoleInfo.CombatForce = 0;
				DBWriter.UpdateRoleCombatForce(dbMgr, roleID, 0);
				dbRoleInfo.IsFlashPlayer = 0;
				DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.Experience = 0L;
				dbRoleInfo.MainTaskID = 0;
				dbRoleInfo.MainQuickBarKeys = "";
				DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.Level = 1;
				userInfo.ListRoleLevels[nIndex] = 1;
				if (DBWriter.UpdateRoleInfoForFlashPlayerFlag(dbMgr, roleID, 0))
				{
					strcmd = string.Format("{0}:{1}", roleID, 1);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00096F4C File Offset: 0x0009514C
		private static TCPProcessCmdResults ProcessCleanDataWhenFreshPlayerLogOutCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(dbRoleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nIndex = -1;
				for (int i = 0; i < userInfo.ListRoleIDs.Count; i++)
				{
					if (userInfo.ListRoleIDs[i] == dbRoleInfo.RoleID)
					{
						nIndex = i;
						break;
					}
				}
				DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.Experience = 0L;
				dbRoleInfo.MainTaskID = 0;
				dbRoleInfo.MainQuickBarKeys = "";
				DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.Level = 1;
				userInfo.ListRoleLevels[nIndex] = 1;
				DBWriter.UpdateRoleGoodsForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.GoodsDataList = new List<GoodsData>();
				DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, roleID);
				dbRoleInfo.DoingTaskList = new List<TaskData>();
				dbRoleInfo.OldTasks = new List<OldTaskData>();
				strcmd = string.Format("{0}:{1}", roleID, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00097204 File Offset: 0x00095404
		private static TCPProcessCmdResults ProcessChangeTaskStarLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int TaskID = Convert.ToInt32(fields[1]);
				int StarLevel = Convert.ToInt32(fields[2]);
				string strcmd = "";
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool bRet = false;
				if (DBWriter.UpdateRoleTasksStarLevel(dbMgr, roleID, TaskID, StarLevel))
				{
					for (int i = 0; i < dbRoleInfo.DoingTaskList.Count; i++)
					{
						if (dbRoleInfo.DoingTaskList[i].DbID == TaskID)
						{
							dbRoleInfo.DoingTaskList[i].StarLevel = StarLevel;
							strcmd = string.Format("{0}:{1}", roleID, 1);
							bRet = true;
							break;
						}
					}
				}
				if (!bRet)
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00097450 File Offset: 0x00095650
		private static TCPProcessCmdResults ProcessChangeLifeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nChangeLifeCount = Convert.ToInt32(fields[1]);
				string strcmd = "";
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.UpdateRoleChangeLifeInfo(dbMgr, roleID, nChangeLifeCount))
				{
					string userID = "";
					lock (dbRoleInfo)
					{
						dbRoleInfo.ChangeLifeCount = nChangeLifeCount;
						userID = dbRoleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
					strcmd = string.Format("{0}:{1}", roleID, 1);
					dbRoleInfo.ChangeLifeCount = nChangeLifeCount;
					if (userID != "")
					{
						DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
						if (null != dbUserInfo)
						{
							lock (dbUserInfo)
							{
								for (int i = 0; i < dbUserInfo.ListRoleChangeLifeCount.Count; i++)
								{
									if (dbUserInfo.ListRoleIDs[i] == roleID)
									{
										dbUserInfo.ListRoleChangeLifeCount[i] = nChangeLifeCount;
									}
								}
							}
						}
					}
				}
				else
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x0009775C File Offset: 0x0009595C
		private static TCPProcessCmdResults ProcessAdmiredPlayerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleAID = Convert.ToInt32(fields[0]);
				int roleBID = Convert.ToInt32(fields[1]);
				int nDate = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleAID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					strcmd = string.Format("{0}:{1}:{2}", roleAID, -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo2 = dbMgr.GetDBRoleInfo(ref roleBID);
				if (null == dbRoleInfo2)
				{
					strcmd = string.Format("{0}:{1}:{2}", roleAID, -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nId = DBQuery.QueryPlayerAdmiredAnother(dbMgr, roleAID, roleBID, nDate);
				if (nId == roleBID)
				{
					strcmd = string.Format("{0}:{1}:{2}", roleAID, -2, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nCount = dbRoleInfo2.AdmiredCount + 1;
				if (DBWriter.UpdateRoleAdmiredInfo1(dbMgr, roleBID, nCount) && DBWriter.UpdateRoleAdmiredInfo2(dbMgr, roleAID, roleBID, nDate))
				{
					dbRoleInfo2.AdmiredCount = nCount;
					strcmd = string.Format("{0}:{1}:{2}", roleAID, 1, dbRoleInfo2.AdmiredCount);
				}
				else
				{
					strcmd = string.Format("{0}:{1}:{2}", roleAID, -1, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x00097A1C File Offset: 0x00095C1C
		private static TCPProcessCmdResults ProcessQueryRoleMiniInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RoleMiniInfo roleMiniInfo = null;
			try
			{
				long rid = DataHelper.BytesToObject<long>(data, 0, count);
				if (rid > 0L)
				{
					roleMiniInfo = CacheManager.GetRoleMiniInfo(rid);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleMiniInfo>(roleMiniInfo, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleMiniInfo>(roleMiniInfo, pool, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00097AA0 File Offset: 0x00095CA0
		private static TCPProcessCmdResults ProcessUpdateRoleSomeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nCombatForce = Convert.ToInt32(fields[1]);
				int nLevel = Convert.ToInt32(fields[2]);
				int nChangeLifeCount = Convert.ToInt32(fields[3]);
				int nYinLiang = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbRoleInfo)
				{
					dbRoleInfo.CombatForce = nCombatForce;
				}
				DBWriter.UpdateRoleYinLiang(dbMgr, roleID, nYinLiang);
				DBWriter.UpdateRoleCombatForce(dbMgr, roleID, nCombatForce);
				DBWriter.UpdateRoleLevel(dbMgr, roleID, nLevel);
				DBWriter.UpdateRoleChangeLifeInfo(dbMgr, roleID, nChangeLifeCount);
				string strcmd = string.Format("{0}:{1}", roleID, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x00097CEC File Offset: 0x00095EEC
		private static TCPProcessCmdResults ProcessQueryDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nTpye = Convert.ToInt32(fields[0]);
				bool nRet = true;
				List<int> lData = DBQuery.GetDayActivityTotlePoint(dbMgr, nTpye);
				DBRoleInfo dbRoleInfo = null;
				if (lData != null && lData.Count == 2)
				{
					int roleID = lData[0];
					for (dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID); dbRoleInfo == null; dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID))
					{
						DBWriter.DeleteRoleDayActivityInfo(dbMgr, lData[0], nTpye);
						lData = DBQuery.GetDayActivityTotlePoint(dbMgr, nTpye);
						if (lData == null || lData.Count != 2)
						{
							nRet = false;
							break;
						}
					}
				}
				string strcmd;
				if (nRet && dbRoleInfo != null)
				{
					string strName = Global.FormatRoleName(dbRoleInfo);
					strcmd = string.Format("{0}:{1}", lData[1], strName);
				}
				else
				{
					strcmd = string.Format("{0}:{1}", -1, null);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00097F18 File Offset: 0x00096118
		private static TCPProcessCmdResults ProcessQueryRoleDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleid = Convert.ToInt32(fields[0]);
				int nType = Convert.ToInt32(fields[1]);
				string strcmd = "";
				int nVlue = -1;
				if (nType < 0 || nType > 5)
				{
					strcmd = string.Format("{0}:{1}:{2}", nVlue, -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (nType == 0)
				{
					int nBloodValue = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, 1);
					int nDaimonValue = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, 2);
					int nCampValue = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, 3);
					int nKingOfPK = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, 4);
					int nAngelTemple = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, 5);
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						nBloodValue,
						nDaimonValue,
						nCampValue,
						nKingOfPK,
						nAngelTemple
					});
				}
				else
				{
					nVlue = DBQuery.GetRoleDayActivityPoint(dbMgr, nRoleid, nType);
					if (nVlue == 1)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							nVlue,
							-1,
							-1,
							-1,
							-1
						});
					}
					else if (nVlue == 2)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							nVlue,
							-1,
							-1,
							-1
						});
					}
					else if (nVlue == 3)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							nVlue,
							-1,
							-1
						});
					}
					else if (nVlue == 4)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							-1,
							nVlue,
							-1
						});
					}
					else if (nVlue == 5)
					{
						strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							-1,
							-1,
							nVlue
						});
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00098388 File Offset: 0x00096588
		private static TCPProcessCmdResults ProcessUpdateRoleDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleid = Convert.ToInt32(fields[0]);
				int nType = Convert.ToInt32(fields[1]);
				int nDate = Convert.ToInt32(fields[2]);
				int nBloodValue = Convert.ToInt32(fields[3]);
				int nDaimonValue = Convert.ToInt32(fields[4]);
				int nCampValue = Convert.ToInt32(fields[5]);
				int nKingOfPk = Convert.ToInt32(fields[6]);
				long nAngelTemple = Convert.ToInt64(fields[7]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd = "";
				if (nType == 0)
				{
					int nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, 1);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, 1, nCount, (long)nBloodValue);
					nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, 2);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, 2, nCount, (long)nDaimonValue);
					nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, 3);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, 3, nCount, (long)nCampValue);
					nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, 4);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, 4, nCount, (long)nKingOfPk);
					nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, 5);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, 5, nCount, nAngelTemple);
				}
				else
				{
					int nCount = DBQuery.GetBloodCastleEnterCount(dbMgr, nRoleid, nDate, nType);
					if (nType == 1)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, nType, nCount + 1, (long)nBloodValue);
					}
					else if (nType == 2)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, nType, nCount + 1, (long)nDaimonValue);
					}
					else if (nType == 3)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, nType, nCount + 1, (long)nCampValue);
					}
					else if (nType == 4)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, nType, nCount + 1, (long)nKingOfPk);
					}
					else if (nType == 5)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, nRoleid, nDate, nType, nCount + 1, nAngelTemple);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x000986C8 File Offset: 0x000968C8
		private static TCPProcessCmdResults ProcessQueryEveryDayOnLineAwardGiftInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleid = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> lListData = DBQuery.QueryPlayerEveryDayOnLineAwardGiftInfo(dbMgr, nRoleid);
				string strcmd;
				if (lListData != null)
				{
					strcmd = string.Format("{0}:{1}:{2}", 1, lListData[0], lListData[1]);
				}
				else
				{
					strcmd = string.Format("{0}:{1}:{2}", -1, -1, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x000988C4 File Offset: 0x00096AC4
		private static TCPProcessCmdResults ProcessSetAutoAssignPropertyPointCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleid = Convert.ToInt32(fields[0]);
				int nFlag = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRet = DBWriter.SetRoleAutoAssignPropertyPoint(dbMgr, nRoleid, nFlag);
				if (nRet == 1)
				{
					dbRoleInfo.AutoAssignPropertyPoint = nFlag;
				}
				string strcmd = string.Format("{0}:{1}", nRoleid, nRet);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x00098AA4 File Offset: 0x00096CA4
		private static TCPProcessCmdResults ProcessUpdatePushMessageInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleid = Convert.ToInt32(fields[0]);
				string strPushMsgID = fields[1];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRet = DBWriter.SetUserPushMessageID(dbMgr, dbRoleInfo.UserID, strPushMsgID);
				if (nRet == 1)
				{
					dbRoleInfo.PushMsgID = strPushMsgID;
				}
				string strcmd = string.Format("{0}:{1}", nRoleid, nRet);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00098C84 File Offset: 0x00096E84
		private static TCPProcessCmdResults ProcessQueryPushMsgUerListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nCondition = Convert.ToInt32(fields[0]);
				List<PushMessageData> list = new List<PushMessageData>();
				list = DBQuery.QueryPushMsgUerList(dbMgr, nCondition);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<PushMessageData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x00098DB8 File Offset: 0x00096FB8
		private static TCPProcessCmdResults ProcessAddWingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int wingID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string today = now.ToString("yyyy-MM-dd HH:mm:ss");
				long ticks = now.Ticks / 10000L;
				WingData wingData = null;
				int ret = DBWriter.NewWing(dbMgr, roleID, wingID, 0, today, dbRoleInfo.RoleName, dbRoleInfo.Occupation);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的翅膀失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.MyWingData = new WingData
						{
							DbID = ret,
							WingID = wingID,
							ForgeLevel = 0,
							AddDateTime = ticks,
							JinJieFailedNum = 0,
							StarExp = 0,
							ZhuLingNum = 0,
							ZhuHunNum = 0
						};
						wingData = dbRoleInfo.MyWingData;
					}
					WingPaiHangManager.getInstance().createWingData(roleID);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<WingData>(wingData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00099078 File Offset: 0x00097278
		private static TCPProcessCmdResults ProcessModWingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateWing(dbMgr, dbID, fields, 2);
				if (ret < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新时翅膀失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						if (null != dbRoleInfo.MyWingData)
						{
							dbRoleInfo.MyWingData.Using = DataHelper.ConvertToInt32(fields[2], dbRoleInfo.MyWingData.Using);
							dbRoleInfo.MyWingData.WingID = DataHelper.ConvertToInt32(fields[3], dbRoleInfo.MyWingData.WingID);
							dbRoleInfo.MyWingData.ForgeLevel = DataHelper.ConvertToInt32(fields[4], dbRoleInfo.MyWingData.ForgeLevel);
							dbRoleInfo.MyWingData.JinJieFailedNum = DataHelper.ConvertToInt32(fields[5], dbRoleInfo.MyWingData.JinJieFailedNum);
							dbRoleInfo.MyWingData.StarExp = DataHelper.ConvertToInt32(fields[6], dbRoleInfo.MyWingData.StarExp);
							dbRoleInfo.MyWingData.ZhuLingNum = DataHelper.ConvertToInt32(fields[7], dbRoleInfo.MyWingData.ZhuLingNum);
							dbRoleInfo.MyWingData.ZhuHunNum = DataHelper.ConvertToInt32(fields[8], dbRoleInfo.MyWingData.ZhuHunNum);
						}
					}
					WingRankingInfo wingInfo = WingPaiHangManager.getInstance().getWingData(roleID);
					if (null != wingInfo)
					{
						if (wingInfo.nWingID != dbRoleInfo.MyWingData.WingID || wingInfo.nStarNum != dbRoleInfo.MyWingData.ForgeLevel)
						{
							wingInfo.nWingID = dbRoleInfo.MyWingData.WingID;
							wingInfo.nStarNum = dbRoleInfo.MyWingData.ForgeLevel;
							WingPaiHangManager.getInstance().ModifyWingPaihangData(wingInfo, false);
						}
					}
				}
				string strcmd = string.Format("{0}:{1}", roleID, ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x00099428 File Offset: 0x00097628
		private static TCPProcessCmdResults ProcessReferPictureJudgeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nPictureJudgeID = Convert.ToInt32(fields[1]);
				int nReferNum = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleReferPictureJudgeInfo(dbMgr, roleID, nPictureJudgeID, nReferNum);
				if (ret <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						int nReferCount = 0;
						if (null != dbRoleInfo.PictureJudgeReferInfo)
						{
							if (!dbRoleInfo.PictureJudgeReferInfo.TryGetValue(nPictureJudgeID, out nReferCount))
							{
								dbRoleInfo.PictureJudgeReferInfo.Add(nPictureJudgeID, nReferNum);
							}
							else
							{
								dbRoleInfo.PictureJudgeReferInfo[nPictureJudgeID] = nReferNum;
							}
						}
						else
						{
							dbRoleInfo.PictureJudgeReferInfo = new Dictionary<int, int>();
							dbRoleInfo.PictureJudgeReferInfo.Add(nPictureJudgeID, nReferNum);
						}
					}
				}
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x000996D0 File Offset: 0x000978D0
		private static TCPProcessCmdResults ProcessQueryMoJingExchangeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nDayID = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, int> TmpDict = DBQuery.QueryMoJingExchangeDict(dbMgr, roleID, nDayID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, int>>(TmpDict, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0009985C File Offset: 0x00097A5C
		private static TCPProcessCmdResults ProcessUpdateMoJingExchangeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nExchangeID = Convert.ToInt32(fields[1]);
				int nDayid = Convert.ToInt32(fields[2]);
				int nNum = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateMoJingExchangeDict(dbMgr, roleID, nExchangeID, nDayid, nNum);
				if (ret <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x00099A58 File Offset: 0x00097C58
		private static TCPProcessCmdResults ProcessUpdateGoodsCmd2(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			UpdateGoodsArgs args = null;
			try
			{
				args = DataHelper.BytesToObject<UpdateGoodsArgs>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<int>(nID, -1);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				if (null == args)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int roleID = args.RoleID;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GoodsData gd = Global.GetGoodsDataByDbID(dbRoleInfo, args.DbID);
				if (null == gd)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int ret = DBWriter.UpdateGoods2(dbMgr, roleID, gd, args);
				if (ret <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新物品属性信息失败，CMD={0}, RoleID={1}, dbid={2}", (TCPGameServerCmds)nID, roleID, args.DbID), null, true);
				}
				client.sendCmd<int>(nID, ret);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<int>(nID, -1);
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x00099C24 File Offset: 0x00097E24
		private static TCPProcessCmdResults ProcessUpdateStarConstellationCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nStarSiteID = Convert.ToInt32(fields[1]);
				int nStarSlotID = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateRoleStarConstellationInfo(dbMgr, roleID, nStarSiteID, nStarSlotID);
				if (ret <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						int nReferCount = 0;
						if (null != dbRoleInfo.StarConstellationInfo)
						{
							if (!dbRoleInfo.StarConstellationInfo.TryGetValue(nStarSiteID, out nReferCount))
							{
								dbRoleInfo.StarConstellationInfo.Add(nStarSiteID, nStarSlotID);
							}
							else
							{
								dbRoleInfo.StarConstellationInfo[nStarSiteID] = nStarSlotID;
							}
						}
						else
						{
							dbRoleInfo.StarConstellationInfo = new Dictionary<int, int>();
							dbRoleInfo.StarConstellationInfo.Add(nStarSiteID, nStarSlotID);
						}
					}
				}
				string strcmd = string.Format("{0}", ret);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x00099ECC File Offset: 0x000980CC
		private static TCPProcessCmdResults ProcessQueryVipLevelAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nFlag = DBQuery.QueryVipLevelAwardFlagInfo(dbMgr, dbRoleInfo.RoleID, dbRoleInfo.ZoneID);
				string strcmd = string.Format("{0}", nFlag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0009A06C File Offset: 0x0009826C
		private static TCPProcessCmdResults ProcessUpdateVipLevelAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int nFlag = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strcmd;
				if (!DBWriter.UpdateVipLevelAwardFlagInfo(dbMgr, dbRoleInfo.UserID, nFlag, dbRoleInfo.ZoneID))
				{
					strcmd = string.Format("{0}:{1}", roleID, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色VIP等级奖励标记事时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
				}
				else
				{
					lock (dbRoleInfo)
					{
						dbRoleInfo.VipAwardFlag = nFlag;
					}
					strcmd = string.Format("{0}:{1}", roleID, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0009A2CC File Offset: 0x000984CC
		private static TCPProcessCmdResults ProcessUpdateOnePieceTreasureLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string TimeLog = fields[0];
				int LogType = Convert.ToInt32(fields[1]);
				int addValue = Convert.ToInt32(fields[2]);
				DBWriter.UpdateOnePieceTreasureLog(dbMgr, TimeLog, LogType, addValue);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13400);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0009A40C File Offset: 0x0009860C
		private static TCPProcessCmdResults ProcessAddItemLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 10)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.insertItemLog(dbMgr, fields);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x0009A52C File Offset: 0x0009872C
		private static TCPProcessCmdResults ProcessUpdateRoleKuaFuDayLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RoleKuaFuDayLogData cmdData = null;
			try
			{
				cmdData = DataHelper.BytesToObject<RoleKuaFuDayLogData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				if (null == cmdData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析数据结果RoleKuaFuDayLogData失败, CMD={0}, Recv={1}", (TCPGameServerCmds)nID, data.Length), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleKuaFuDayLog(dbMgr, cmdData);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0009A630 File Offset: 0x00098830
		private static TCPProcessCmdResults ProcessAddRoleStoreYinliang(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				long value = Convert.ToInt64(fields[1]);
				int isGM = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				long userYinLiang = 0L;
				lock (dbRoleInfo)
				{
					if (value < 0L && isGM == 0 && dbRoleInfo.store_yinliang < Math.Abs(value))
					{
						failed = true;
					}
					else
					{
						dbRoleInfo.store_yinliang += value;
						userYinLiang = dbRoleInfo.store_yinliang;
					}
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (value != 0L)
				{
					if (!DBWriter.UpdateRoleStoreYinLiang(dbMgr, roleID, userYinLiang))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色仓库金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", -2, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				strcmd = string.Format("{0}:{1}", roleID, userYinLiang);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0009A934 File Offset: 0x00098B34
		private static TCPProcessCmdResults ProcessAddRoleStoreMoney(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				long value = Convert.ToInt64(fields[1]);
				int isGM = Convert.ToInt32(fields[2]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool failed = false;
				long userMoney = 0L;
				lock (dbRoleInfo)
				{
					if (value < 0L && isGM == 0 && dbRoleInfo.store_money < Math.Abs(value))
					{
						failed = true;
					}
					else
					{
						dbRoleInfo.store_money += value;
						userMoney = dbRoleInfo.store_money;
					}
				}
				string strcmd;
				if (failed)
				{
					strcmd = string.Format("{0}:{1}", -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (value != 0L)
				{
					if (!DBWriter.UpdateRoleStoreMoney(dbMgr, roleID, userMoney))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色仓库绑定金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						strcmd = string.Format("{0}:{1}", -2, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbRoleInfo);
				strcmd = string.Format("{0}:{1}", roleID, userMoney);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x0009AC38 File Offset: 0x00098E38
		private static TCPProcessCmdResults ProcessUpdateLingYu(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int type = Convert.ToInt32(fields[1]);
				int level = Convert.ToInt32(fields[2]);
				int suit = Convert.ToInt32(fields[3]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int ret = DBWriter.UpdateLingYu(dbMgr, roleID, type, level, suit);
				if (ret >= 0)
				{
					lock (dbRoleInfo)
					{
						LingYuData lyData = new LingYuData();
						lyData.Type = type;
						lyData.Level = level;
						lyData.Suit = suit;
						dbRoleInfo.LingYuDict[type] = lyData;
					}
					string strcmd = string.Format("{0}:{1}", roleID, ret);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("更新翎羽失败，CMD={0}, RoleID={1}, type={2}", (TCPGameServerCmds)nID, roleID, type), null, true);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x0009AEC0 File Offset: 0x000990C0
		private static TCPProcessCmdResults ProcessAddFuMoMoneyGiveMail(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int sendid = Convert.ToInt32(fields[0]);
				string sendname = fields[1];
				int recid = Convert.ToInt32(fields[2]);
				int num = Convert.ToInt32(fields[3]);
				string content = fields[4];
				int sendjob = Convert.ToInt32(fields[5]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref sendid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, sendid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (DBWriter.InsertFoMoMailData(dbMgr, sendid, sendname, sendjob, recid, num, content, today))
				{
					if (FuMoMailManager.getInstance().InsertFuMoMailCached(dbMgr, sendid, sendname, sendjob, recid, num, content, today))
					{
						int Over = FuMoMailManager.getInstance().MaxLimitContorl(recid);
						if (Over > 0)
						{
							if (-1 == FuMoMailManager.getInstance().DelFuMoMailFromLimitContorl(dbMgr, recid, Over))
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
								return TCPProcessCmdResults.RESULT_DATA;
							}
						}
						cmdData = string.Format("{0}:{1}", sendid, recid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0009B13C File Offset: 0x0009933C
		private static TCPProcessCmdResults ProcessGetFuMoMoneyMapAcceptNum(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int sendid = Convert.ToInt32(fields[0]);
				int nDate = Convert.ToInt32(fields[1]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref sendid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, sendid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				cmdData = string.Format("{0}:{1}", sendid, FuMoMailManager.getInstance().GetFuMoTempDataAcceptFromCached(nDate, sendid));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0009B2E0 File Offset: 0x000994E0
		private static TCPProcessCmdResults ProcessAddFuMoMoneyGiveMailTemp(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int sendid = Convert.ToInt32(fields[0]);
				string recrid_list = fields[1];
				int nDate = Convert.ToInt32(fields[2]);
				int accept = Convert.ToInt32(fields[3]);
				int give = Convert.ToInt32(fields[4]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref sendid);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, sendid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.InsertFoMoMailDataTemp(dbMgr, sendid, recrid_list, nDate, accept, give))
				{
					if (FuMoMailManager.getInstance().InsertAcceptMapCached(sendid, recrid_list, nDate, accept, give))
					{
						cmdData = string.Format("{0}:{1}:{2}", nDate, sendid, recrid_list);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("插入缓存失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, sendid), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0009B51C File Offset: 0x0009971C
		private static TCPProcessCmdResults ProcessGetFuMoMoneyMailList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, List<FuMoMailData>> mailItemDataList = FuMoMailManager.getInstance().GetFuMoMailItemDataListFromCached(roleID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, List<FuMoMailData>>>(mailItemDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0009B67C File Offset: 0x0009987C
		private static TCPProcessCmdResults ProcessGetFuMoMoneyMailMapList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(fields[1]);
				Dictionary<int, FuMoMailTemp> mailItemDataList = FuMoMailManager.getInstance().GetFuMoTempDataListFromCached(nDate, roleID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, FuMoMailTemp>>(mailItemDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0009B7E8 File Offset: 0x000999E8
		private static TCPProcessCmdResults ProcessUpdateFuMoMoneyMailMap(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(fields[1]);
				int give = Convert.ToInt32(fields[2]);
				string recid_list = fields[3];
				if (DBWriter.UpdateRoleStoreFuMoMoneyGiveNum(dbMgr, roleid, give, nDate, recid_list))
				{
					if (FuMoMailManager.getInstance().UpdateGiveAndListCached(roleid, give, nDate, recid_list))
					{
						cmdData = string.Format("{0}:{1}", roleid, give);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0009B9C0 File Offset: 0x00099BC0
		private static TCPProcessCmdResults ProcessUpdateFuMoAcceptMap(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(fields[1]);
				int accept = Convert.ToInt32(fields[2]);
				if (DBWriter.UpdateRoleStoreFuMoMoneyAcceptNum(dbMgr, roleid, nDate, accept) && FuMoMailManager.getInstance().UpdateAcceptCached(roleid, accept, nDate))
				{
					cmdData = string.Format("{0}:{1}", roleid, accept);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0009BB88 File Offset: 0x00099D88
		private static TCPProcessCmdResults ProcessDeleteFuMoMail(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int mailid = Convert.ToInt32(fields[1]);
				if (DBWriter.DeleteMailFuMoByMailID(dbMgr, mailid) && FuMoMailManager.getInstance().UpdataRemoveMailListCached(mailid, roleid))
				{
					cmdData = string.Format("{0}:{1}", roleid, mailid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0009BD28 File Offset: 0x00099F28
		private static TCPProcessCmdResults ProcessDeleteFuMoMailList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string mailid = fields[1];
				string[] mailidList = mailid.Split(new char[]
				{
					'_'
				});
				string parem = FuMoMailManager.getInstance().MakeDelListSQL(mailidList);
				if (DBWriter.DeleteMailFuMoByMailIDList(dbMgr, roleid, parem))
				{
					if (FuMoMailManager.getInstance().UpdataRemoveMailListCached(mailidList, roleid))
					{
						cmdData = string.Format("{0}:{1}", roleid, parem);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0009BF04 File Offset: 0x0009A104
		private static TCPProcessCmdResults ProcessUpdateFuMoMailReadState(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int mailid = Convert.ToInt32(fields[1]);
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (DBWriter.UpdateIsReadFoMoMailData(dbMgr, mailid, today))
				{
					if (FuMoMailManager.getInstance().UpdataReadStateCached(roleid, mailid, today))
					{
						cmdData = string.Format("{0}:{1}", roleid, mailid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0009C0D8 File Offset: 0x0009A2D8
		private static TCPProcessCmdResults ProcessFuMoMailIndexCount(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(DBQuery.GetMailMaxConutFromTable(dbMgr, roleid), pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0009C22C File Offset: 0x0009A42C
		private static TCPProcessCmdResults ProcessQueryRoleMoneyInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbUserInfo = dbMgr.dbUserMgr.FindDBUserInfo(dbRoleInfo.UserID);
				if (null == dbUserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的账号不存在，CMD={0}, dbRoleInfo.UserID={1}", (TCPGameServerCmds)nID, dbRoleInfo.UserID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int costMoney = 0;
				foreach (int tmpID in dbUserInfo.ListRoleIDs)
				{
					string strCostMoney = DBQuery.GetRoleParamByName(dbMgr, tmpID, "TotalCostMoney");
					string[] strFields = strCostMoney.Split(new char[]
					{
						','
					});
					if (strFields == null || strFields.Length != 2)
					{
						int temp = DBQuery.GetUserUsedMoney(dbMgr, tmpID, "2014-01-01 00:00:00", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						string result = string.Format("{0},{1}", 1, temp);
						DBWriter.UpdateRoleParams(dbMgr, tmpID, "TotalCostMoney", result, null);
						costMoney += temp;
					}
					else
					{
						costMoney += Convert.ToInt32(strFields[1]);
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					dbRoleInfo.UserID,
					roleID,
					dbRoleInfo.RoleName,
					dbUserInfo.RealMoney,
					costMoney,
					dbUserInfo.Money,
					dbRoleInfo.TotalOnlineSecs,
					dbRoleInfo.ChangeLifeCount * 100 + dbRoleInfo.Level,
					DataHelper.ConvertToTicks(dbRoleInfo.RegTime)
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0009C5C4 File Offset: 0x0009A7C4
		private static TCPProcessCmdResults ProcessAutoCompletionTaskByTaskID(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				List<int> taskList = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (taskList == null || taskList.Count < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("任务列表异常, CMD={0}", (TCPGameServerCmds)nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = taskList[0];
				int taskID = taskList[taskList.Count - 1];
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				lock (dbRoleInfo)
				{
					if (!DBWriter.WirterAutoCompletionTaskByTaskID(dbMgr, roleID, taskList))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入历史任务标记失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					DBWriter.UpdateRoleMainTaskID(dbMgr, roleID, taskID);
					dbRoleInfo.MainTaskID = taskID;
					if (null == dbRoleInfo.OldTasks)
					{
						dbRoleInfo.OldTasks = new List<OldTaskData>();
					}
					for (int i = 1; i < taskList.Count; i++)
					{
						dbRoleInfo.OldTasks.Add(new OldTaskData
						{
							TaskID = taskList[i],
							DoCount = 1
						});
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 0), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0009C7FC File Offset: 0x0009A9FC
		private static TCPProcessCmdResults ProcessUpdateMarriageDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MarriageData updateMarriageData = null;
			int nRoleID = -1;
			bool bRet = false;
			try
			{
				nRoleID = BitConverter.ToInt32(data, 0);
				updateMarriageData = DataHelper.BytesToObject<MarriageData>(data, 4, count - 4);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				bool bSortPaiHang = false;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbRoleInfo)
				{
					lock (dbRoleInfo)
					{
						if (dbRoleInfo.MyMarriageData.ChangTime != updateMarriageData.ChangTime && updateMarriageData.byGoodwilllevel != 0)
						{
							bSortPaiHang = true;
						}
						dbRoleInfo.MyMarriageData.nSpouseID = updateMarriageData.nSpouseID;
						dbRoleInfo.MyMarriageData.byMarrytype = updateMarriageData.byMarrytype;
						dbRoleInfo.MyMarriageData.nRingID = updateMarriageData.nRingID;
						dbRoleInfo.MyMarriageData.nGoodwillexp = updateMarriageData.nGoodwillexp;
						dbRoleInfo.MyMarriageData.byGoodwillstar = updateMarriageData.byGoodwillstar;
						dbRoleInfo.MyMarriageData.byGoodwilllevel = updateMarriageData.byGoodwilllevel;
						dbRoleInfo.MyMarriageData.nGivenrose = updateMarriageData.nGivenrose;
						dbRoleInfo.MyMarriageData.strLovemessage = updateMarriageData.strLovemessage;
						dbRoleInfo.MyMarriageData.byAutoReject = updateMarriageData.byAutoReject;
						dbRoleInfo.MyMarriageData.ChangTime = updateMarriageData.ChangTime;
					}
				}
				else if (updateMarriageData.byGoodwilllevel != 0)
				{
					bSortPaiHang = true;
				}
				bRet = DBWriter.UpdateMarriageData(dbMgr, nRoleID, updateMarriageData.nSpouseID, updateMarriageData.byMarrytype, updateMarriageData.nRingID, updateMarriageData.nGoodwillexp, updateMarriageData.byGoodwillstar, updateMarriageData.byGoodwilllevel, updateMarriageData.nGivenrose, updateMarriageData.strLovemessage, updateMarriageData.byAutoReject, updateMarriageData.ChangTime);
				RingRankingInfo RingRankData = RingPaiHangManager.getInstance().getRingData(nRoleID);
				if (null != RingRankData)
				{
					RingRankData.nRingID = updateMarriageData.nRingID;
					RingRankData.byGoodwillstar = (int)updateMarriageData.byGoodwillstar;
					RingRankData.byGoodwilllevel = (int)updateMarriageData.byGoodwilllevel;
					RingRankData.strAddTime = updateMarriageData.ChangTime;
				}
				if (bRet && bSortPaiHang)
				{
					RingPaiHangManager.getInstance().createRingData(nRoleID, RingRankData);
				}
				client.sendCmd<bool>(nID, bRet);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			client.sendCmd<bool>(nID, bRet);
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0009CAC8 File Offset: 0x0009ACC8
		private static TCPProcessCmdResults ProcessGetMarriageDataCmd(DBManager dbMgr, TCPOutPacketPool pool, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MarriageData updateMarriageData = null;
			int nRoleID = -1;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				nRoleID = Convert.ToInt32(cmdData);
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbRoleInfo)
				{
					lock (dbRoleInfo)
					{
						updateMarriageData = dbRoleInfo.MyMarriageData;
					}
				}
				else
				{
					updateMarriageData = DBQuery.GetMarriageData(dbMgr, nRoleID);
				}
				if (null != updateMarriageData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarriageData>(updateMarriageData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				updateMarriageData = new MarriageData();
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarriageData>(updateMarriageData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0009CC10 File Offset: 0x0009AE10
		private static TCPProcessCmdResults ProcessExecuteJieriFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string fromDate = fields[1].Replace('$', ':');
				string toDate = fields[2].Replace('$', ':');
				int nActType = Convert.ToInt32(fields[3]);
				int extTag = Convert.ToInt32(fields[4]);
				extTag = Math.Max(0, extTag);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string strcmd;
				if (null == roleInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
				int hasgettimes = 0;
				string lastgettime = "";
				int histForRole = DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, nActType, huoDongKeyStr, out hasgettimes, out lastgettime);
				if (extTag == 0)
				{
					strcmd = string.Format("{0}:{1}", nActType, hasgettimes);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bitVal = Global.GetBitValue(extTag);
				if ((hasgettimes & bitVal) == bitVal)
				{
					strcmd = string.Format("{0}:{1}:0", -10005, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo userInfo = dbMgr.GetDBUserInfo(roleInfo.UserID);
				if (null == userInfo)
				{
					strcmd = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (roleInfo)
				{
					hasgettimes |= 1 << extTag - 1;
					if (histForRole < 0)
					{
						int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, nActType, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int ret = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, nActType, huoDongKeyStr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				strcmd = string.Format("{0}:{1}:{2}", hasgettimes, roleID, extTag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0009D040 File Offset: 0x0009B240
		private static TCPProcessCmdResults ProcessQueryMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int serverLineID = Convert.ToInt32(fields[0]);
				tcpOutPacket = GameDBManager.MarryPartyDataC.GetPartyList(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0009D168 File Offset: 0x0009B368
		private static TCPProcessCmdResults ProcessAddMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				int partyType = Convert.ToInt32(fields[1]);
				long startTime = Convert.ToInt64(fields[2]);
				int husbandRoleID = Convert.ToInt32(fields[3]);
				int wifeRoleID = Convert.ToInt32(fields[4]);
				string husbandName = Convert.ToString(fields[5]);
				string wifeName = Convert.ToString(fields[6]);
				MarryPartyData partyData = GameDBManager.MarryPartyDataC.AddParty(roleid, partyType, startTime, husbandRoleID, wifeRoleID, husbandName, wifeName);
				if (partyData != null)
				{
					DateTime startDateTime = new DateTime(startTime * 10000L);
					string today = startDateTime.ToString("yyyy-MM-dd HH:mm:ss");
					DBWriter.AddMarryParty(dbMgr, roleid, partyType, today, husbandRoleID, wifeRoleID);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarryPartyData>(partyData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0009D338 File Offset: 0x0009B538
		private static TCPProcessCmdResults ProcessRemoveMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				if (GameDBManager.MarryPartyDataC.RemoveParty(roleID))
				{
					DBWriter.RemoveMarryParty(dbMgr, roleID);
					string strcmd = string.Format("{0}", 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0009D48C File Offset: 0x0009B68C
		private static TCPProcessCmdResults ProcessIncMarryPartyJoin(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				int joinerID = Convert.ToInt32(fields[1]);
				int joinCount = Convert.ToInt32(fields[2]);
				if (GameDBManager.MarryPartyDataC.IncPartyJoin(roleid))
				{
					DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref joinerID);
					if (null != roleInfo)
					{
						lock (roleInfo)
						{
							roleInfo.MyMarryPartyJoinList[roleid] = joinCount;
						}
					}
					DBWriter.IncMarryPartyJoin(dbMgr, roleid, joinerID, joinCount);
					string strcmd = string.Format("{0}", 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0009D67C File Offset: 0x0009B87C
		private static TCPProcessCmdResults ProcessClearMarryPartyJoin(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				int writeDB = Convert.ToInt32(fields[1]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null != roleInfo)
				{
					lock (roleInfo)
					{
						roleInfo.MyMarryPartyJoinList.Clear();
					}
				}
				if (writeDB > 0)
				{
					DBWriter.ClearMarryPartyJoin(dbMgr, (writeDB == 1) ? roleid : 0);
				}
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0009D85C File Offset: 0x0009BA5C
		private static TCPProcessCmdResults ProcessUpdateHolyItemDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				sbyte sShengwu_type = Convert.ToSByte(fields[1]);
				sbyte sPart_slot = Convert.ToSByte(fields[2]);
				sbyte sPart_suit = Convert.ToSByte(fields[3]);
				int nPart_slice = Convert.ToInt32(fields[4]);
				int nFail_count = Convert.ToInt32(fields[5]);
				if (sPart_slot < 1 || sPart_slot > 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("sPart_slot错误, cmd={0} sPart_slot={1}, roleid={2}", (TCPGameServerCmds)nID, sPart_slot, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null != dbRoleInfo)
				{
					lock (dbRoleInfo)
					{
						HolyItemData GetHolyData = null;
						bool bFind = dbRoleInfo.MyHolyItemDataDic.TryGetValue(sShengwu_type, out GetHolyData);
						if (!bFind)
						{
							GetHolyData = new HolyItemData();
						}
						GetHolyData.m_sType = sShengwu_type;
						HolyItemPartData partdata = null;
						if (!GetHolyData.m_PartArray.TryGetValue(sPart_slot, out partdata))
						{
							partdata = new HolyItemPartData();
							GetHolyData.m_PartArray.Add(sPart_slot, partdata);
						}
						partdata.m_sSuit = sPart_suit;
						partdata.m_nSlice = nPart_slice;
						partdata.m_nFailCount = nFail_count;
						if (!bFind)
						{
							dbRoleInfo.MyHolyItemDataDic.Add(sShengwu_type, GetHolyData);
						}
					}
				}
				string strcmd = DBWriter.UpdateHolyItemData(dbMgr, roleID, sShengwu_type, sPart_slot, sPart_suit, nPart_slice, nFail_count) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0009DB38 File Offset: 0x0009BD38
		private static TCPProcessCmdResults ProcessGmBanCheck(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 2;
				char span = '#';
				if (!CheckHelper.CheckTCPCmdFields2(nID, data, count, out fields, length, span))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string banIDs = fields[1];
				string strcmd = BanManager.GmBanCheckAdd(dbMgr, roleID, banIDs).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0009DBF4 File Offset: 0x0009BDF4
		private static TCPProcessCmdResults ProcessGmBanLog(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 7;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(fields[0]);
				string userID = fields[1];
				int roleID = Convert.ToInt32(fields[2]);
				int banType = Convert.ToInt32(fields[3]);
				string banID = fields[4];
				int banCount = Convert.ToInt32(fields[5]);
				string deviceID = fields[6];
				string strcmd = BanManager.GmBanLogAdd(dbMgr, zoneID, userID, roleID, banType, banID, banCount, deviceID).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0009DCE0 File Offset: 0x0009BEE0
		private static TCPProcessCmdResults ProcessTenInitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					'#'
				});
				if (fields.Length <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				TenManager.initTen(fields);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0009DE00 File Offset: 0x0009C000
		private static TCPProcessCmdResults ProcessSpreadAwardGetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(fields[0]);
				int roleID = Convert.ToInt32(fields[1]);
				string strcmd = SpreadManager.GetAward(dbMgr, zoneID, roleID).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0009DEB8 File Offset: 0x0009C0B8
		private static TCPProcessCmdResults ProcessSpreadAwardUpdateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 4;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(fields[0]);
				int roleID = Convert.ToInt32(fields[1]);
				int type = Convert.ToInt32(fields[2]);
				string award = fields[3];
				string strcmd = SpreadManager.UpdateAward(dbMgr, zoneID, roleID, type, award).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0009DF84 File Offset: 0x0009C184
		private static TCPProcessCmdResults ProcessActivateStateGetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 1;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = fields[0];
				string strcmd = DBQuery.ActivateStateGet(dbMgr, userID) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0009E034 File Offset: 0x0009C234
		private static TCPProcessCmdResults ProcessActivateStateSetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] fields = null;
			try
			{
				int length = 3;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out fields, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(fields[0]);
				string userID = fields[1];
				int roleID = Convert.ToInt32(fields[2]);
				string strcmd = DBWriter.ActivateStateSet(dbMgr, zoneID, userID, roleID) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0009E0FC File Offset: 0x0009C2FC
		private static TCPProcessCmdResults ProcessFacebookInitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					'#'
				});
				if (fields.Length <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				FacebookManager.initFacebook(fields);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0009E21C File Offset: 0x0009C41C
		private static TCPProcessCmdResults ProcessGetZoneIdByRid(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			string roleId = cmdData;
			int roleID = Convert.ToInt32(roleId);
			try
			{
				DBRoleInfo dbRoleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
				string strcmd;
				if (null == dbRoleInfo)
				{
					List<string> lData = new List<string>();
					lData = DBWriter.GetUserZoneID(dbMgr, roleID);
					strcmd = string.Format("{0}:{1}", lData[0], lData[1]);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}", dbRoleInfo.UserID, dbRoleInfo.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0009E35C File Offset: 0x0009C55C
		private static TCPProcessCmdResults ProcessRebornYinJiUpdateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string UpdateInfo = fields[1];
				int UsePoint = Convert.ToInt32(fields[2]);
				int ResetNum = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.UpdateRebornYinJiInfo(roleID, UpdateInfo, ResetNum, UsePoint))
				{
					if (RebornStampManager.UpdateUserRebornInfo(roleID, UpdateInfo, ResetNum, UsePoint))
					{
						cmdData = string.Format("{0}", roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x0009E524 File Offset: 0x0009C724
		private static TCPProcessCmdResults ProcessGetRebornYinJiInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(fields[0]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RebornStampData userRebornInfo = RebornStampManager.GetUserRebornInfoFromCached(roleid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RebornStampData>(userRebornInfo, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0009E67C File Offset: 0x0009C87C
		private static TCPProcessCmdResults ProcessRebornYinJiInsertInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string UpdateInfo = fields[1];
				int UsePoint = Convert.ToInt32(fields[2]);
				int ResetNum = Convert.ToInt32(fields[3]);
				DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == roleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.InsertRebornYinJiInfo(roleID, UpdateInfo, ResetNum, UsePoint))
				{
					if (RebornStampManager.InsertUserRebornInfo(roleID, UpdateInfo, ResetNum, UsePoint))
					{
						cmdData = string.Format("{0}", roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
