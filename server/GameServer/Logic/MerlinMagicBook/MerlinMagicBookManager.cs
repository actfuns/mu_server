using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.MerlinMagicBook
{
	
	public class MerlinMagicBookManager
	{
		
		private void LoadMerlinLevelUpConfigData()
		{
			try
			{
				lock (this.MerlinLevelUpConfigDict)
				{
					string fileName = "Config/Merlin/MagicBook.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
					}
					else
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						this.MerlinLevelUpConfigDict.Clear();
						foreach (XElement xmlItem in xmlItems)
						{
							if ((int)Global.GetSafeAttributeLong(xmlItem, "Level") > 1)
							{
								MerlinLevelUpConfigData tmpData = new MerlinLevelUpConfigData();
								tmpData._Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
								tmpData._LuckyOne = (int)Global.GetSafeAttributeLong(xmlItem, "LuckyOne");
								tmpData._LuckyTwo = (int)Global.GetSafeAttributeLong(xmlItem, "LuckyTwo");
								tmpData._Rate = Global.GetSafeAttributeDouble(xmlItem, "LuckyTwoRate");
								long[] NeedGoods = Global.GetSafeAttributeLongArray(xmlItem, "NeedGoods", -1);
								if (NeedGoods.Length != 2)
								{
									LogManager.WriteLog(LogTypes.Error, "梅林魔法书升阶数据有误，无法读取", null, true);
									break;
								}
								tmpData._NeedGoodsID = (int)NeedGoods[0];
								tmpData._NeedGoodsCount = (int)NeedGoods[1];
								tmpData._NeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "NeedZuanShi");
								this.MerlinLevelUpConfigDict[tmpData._Level] = tmpData;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinLevelUpConfigData", new object[0])));
			}
		}

		
		private void LoadMerlinStarUpConfigData()
		{
			try
			{
				lock (this.MerlinStarUpConfigDict)
				{
					string fileName = "Config/Merlin/MagicBookStar.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
					}
					else
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						this.MerlinStarUpConfigDict.Clear();
						foreach (XElement xmlItem in xmlItems)
						{
							MerlinStarUpConfigData tmpData = new MerlinStarUpConfigData();
							tmpData._Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
							tmpData._StarNum = (int)Global.GetSafeAttributeLong(xmlItem, "Star");
							tmpData._MinAttackV = (int)Global.GetSafeAttributeLong(xmlItem, "MinAttackV");
							tmpData._MaxAttackV = (int)Global.GetSafeAttributeLong(xmlItem, "MaxAttackV");
							tmpData._MinMAttackV = (int)Global.GetSafeAttributeLong(xmlItem, "MinMAttackV");
							tmpData._MaxMAttackV = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMAttackV");
							tmpData._MinDefenseV = (int)Global.GetSafeAttributeLong(xmlItem, "MinDefenseV");
							tmpData._MaxDefenseV = (int)Global.GetSafeAttributeLong(xmlItem, "MaxDefenseV");
							tmpData._MinMDefenseV = (int)Global.GetSafeAttributeLong(xmlItem, "MinMDefenseV");
							tmpData._MaxMDefenseV = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMDefenseV");
							tmpData._HitV = (int)Global.GetSafeAttributeLong(xmlItem, "HitV");
							tmpData._DodgeV = (int)Global.GetSafeAttributeLong(xmlItem, "Dodge");
							tmpData._MaxHpV = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLifeV");
							tmpData._ReviveP = Global.GetSafeAttributeDouble(xmlItem, "Revive");
							tmpData._MpRecoverP = Global.GetSafeAttributeDouble(xmlItem, "MagicRecover");
							long[] NeedGoods = Global.GetSafeAttributeLongArray(xmlItem, "NeedGoods", -1);
							if (NeedGoods.Length == 2)
							{
								tmpData._NeedGoodsID = (int)NeedGoods[0];
								tmpData._NeedGoodsCount = (int)NeedGoods[1];
							}
							tmpData._NeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "NeedZuanShi");
							tmpData._NeedExp = (int)Global.GetSafeAttributeLong(xmlItem, "StarExp");
							string tmpStr = Global.GetSafeAttributeStr(xmlItem, "GrowUp");
							if (string.IsNullOrEmpty(tmpStr))
							{
								LogManager.WriteLog(LogTypes.Error, "梅林魔法书升星成长经验与暴击率有误，无法读取", null, true);
								break;
							}
							string[] tmpStrArr = tmpStr.Split(new char[]
							{
								'|'
							});
							tmpData._AddExp = new int[2];
							tmpData._CritPercent = new double[2];
							if (tmpStrArr.Length == 2)
							{
								for (int i = 0; i < tmpStrArr.Length; i++)
								{
									string[] tmpStrArr2 = tmpStrArr[i].Split(new char[]
									{
										','
									});
									if (tmpStrArr2.Length == 2)
									{
										tmpData._AddExp[i] = Convert.ToInt32(tmpStrArr2[0]);
										tmpData._CritPercent[i] = Convert.ToDouble(tmpStrArr2[1]);
									}
								}
							}
							int nKey = this.GetMerlinStarUpKey(tmpData._Level, tmpData._StarNum);
							this.MerlinStarUpConfigDict[nKey] = tmpData;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinStarUpConfigData", new object[0])));
			}
		}

		
		private void LoadMerlinSecretConfigData()
		{
			try
			{
				lock (this.MerlinSecretConfigDict)
				{
					string fileName = "Config/Merlin/MagicWord.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件异常", fileName), null, true);
					}
					else
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						this.MerlinSecretConfigDict.Clear();
						foreach (XElement xmlItem in xmlItems)
						{
							MerlinSecretConfigData tmpData = new MerlinSecretConfigData();
							tmpData._Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
							long[] NeedGoods = Global.GetSafeAttributeLongArray(xmlItem, "NeedGoods", -1);
							if (NeedGoods.Length != 2)
							{
								LogManager.WriteLog(LogTypes.Error, "梅林魔法书秘语数据有误，无法读取", null, true);
								break;
							}
							tmpData._NeedGoodsID = (int)NeedGoods[0];
							tmpData._NeedGoodsCount = (int)NeedGoods[1];
							long[] lNums = Global.GetSafeAttributeLongArray(xmlItem, "Num", -1);
							tmpData._Num = new int[lNums.Length];
							for (int i = 0; i < lNums.Length; i++)
							{
								tmpData._Num[i] = Convert.ToInt32(lNums[i]);
							}
							this.MerlinSecretConfigDict[tmpData._Level] = tmpData;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinSecretConfigData", new object[0])));
			}
		}

		
		private int GetMerlinStarUpKey(int nLevel, int nStarNum)
		{
			return nLevel * 1000 + nStarNum;
		}

		
		private bool IsMerlinSecretTime(GameClient client)
		{
			long lNowTicks = TimeUtil.NOW();
			return lNowTicks - client.ClientData.MerlinData._ToTicks < 0L;
		}

		
		private void RefreshMerlinSecondAttr(GameClient client, int nLevel, int nStarNum)
		{
			int nKey = this.GetMerlinStarUpKey(nLevel, nStarNum);
			MerlinStarUpConfigData starData = null;
			lock (this.MerlinStarUpConfigDict)
			{
				if (!this.MerlinStarUpConfigDict.TryGetValue(nKey, out starData) || null == starData)
				{
					return;
				}
			}
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				7,
				starData._MinAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				8,
				starData._MaxAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				9,
				starData._MinMAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				10,
				starData._MaxMAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				3,
				starData._MinDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				4,
				starData._MaxDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				5,
				starData._MinMDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				6,
				starData._MaxMDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				18,
				starData._HitV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				19,
				starData._DodgeV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				13,
				starData._MaxHpV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				60,
				starData._ReviveP
			});
		}

		
		private void RefreshMerlinSecretSecondAttr(GameClient client)
		{
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(0))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					56,
					client.ClientData.MerlinData._ActiveAttr[0] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(1))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					57,
					client.ClientData.MerlinData._ActiveAttr[1] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(2))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					58,
					client.ClientData.MerlinData._ActiveAttr[2] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(3))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					59,
					client.ClientData.MerlinData._ActiveAttr[3] / 100.0
				});
			}
		}

		
		private void ResetActiveSecretAttr(GameClient client)
		{
			for (int i = 0; i < client.ClientData.MerlinData._ActiveAttr.Count; i++)
			{
				client.ClientData.MerlinData._ActiveAttr[i] = 0.0;
			}
		}

		
		private void ResetUnActiveSecretAttr(GameClient client)
		{
			for (int i = 0; i < client.ClientData.MerlinData._UnActiveAttr.Count; i++)
			{
				client.ClientData.MerlinData._UnActiveAttr[i] = 0.0;
			}
		}

		
		private EMerlinStarUpErrorCode MerlinStarUp(GameClient client, bool bIsDiamond, out int nIsCrit, out int nOutAddExp)
		{
			nIsCrit = 0;
			nOutAddExp = 0;
			int nRoleID = client.ClientData.RoleID;
			int nCurStarNum = client.ClientData.MerlinData._StarNum;
			int nCurLevel = client.ClientData.MerlinData._Level;
			string strCostList = "";
			int nUseType = 0;
			try
			{
				if (nCurLevel <= 0 || nCurLevel > MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinStarUpErrorCode.LevelError;
				}
				if (nCurStarNum < 0)
				{
					return EMerlinStarUpErrorCode.StarError;
				}
				if (nCurStarNum >= MerlinSystemParamsConfigData._MaxStarNum)
				{
					return EMerlinStarUpErrorCode.MaxStarNum;
				}
				MerlinStarUpConfigData starData = null;
				int nKey = this.GetMerlinStarUpKey(nCurLevel, nCurStarNum + 1);
				lock (this.MerlinStarUpConfigDict)
				{
					if (!this.MerlinStarUpConfigDict.TryGetValue(nKey, out starData) || null == starData)
					{
						return EMerlinStarUpErrorCode.StarDataError;
					}
				}
				SystemXmlItem needGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(starData._NeedGoodsID, out needGoods))
				{
					return EMerlinStarUpErrorCode.NeedGoodsIDError;
				}
				if (starData._NeedGoodsCount <= 0)
				{
					return EMerlinStarUpErrorCode.NeedGoodsCountError;
				}
				int nTotalGoodsCount = Global.GetTotalGoodsCountByID(client, starData._NeedGoodsID);
				if (nTotalGoodsCount < starData._NeedGoodsCount)
				{
					if (!bIsDiamond)
					{
						return EMerlinStarUpErrorCode.GoodsNotEnough;
					}
					nUseType = 1;
				}
				switch (nUseType)
				{
				case 0:
				{
					bool bUsedBinding = false;
					bool bUsedTimeLimited = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, starData._NeedGoodsID, starData._NeedGoodsCount, false, out bUsedBinding, out bUsedTimeLimited, false))
					{
						return EMerlinStarUpErrorCode.GoodsNotEnough;
					}
					GoodsData goodsDataCost = new GoodsData
					{
						GoodsID = starData._NeedGoodsID,
						GCount = starData._NeedGoodsCount
					};
					strCostList += EventLogManager.NewGoodsDataPropString(goodsDataCost);
					break;
				}
				case 1:
				{
					int oldUserMoney = client.ClientData.UserMoney;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, starData._NeedDiamond, "梅林魔法书升星", true, true, false, DaiBiSySType.MeiLingZhiShu))
					{
						return EMerlinStarUpErrorCode.DiamondNotEnough;
					}
					strCostList += EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
					{
						-starData._NeedDiamond,
						oldUserMoney,
						client.ClientData.UserMoney
					});
					break;
				}
				}
				int nRandom = Global.GetRandomNumber(0, 10001);
				int nRate = (int)(starData._CritPercent[1] * 10000.0);
				int nAddExp = 0;
				if (nRandom < nRate)
				{
					nAddExp = starData._AddExp[1];
					nIsCrit = 1;
				}
				else
				{
					nAddExp = starData._AddExp[0];
				}
				nOutAddExp = nAddExp;
				while (nAddExp > 0)
				{
					int nMaxNeedExp = starData._NeedExp;
					int nNeedExp = nMaxNeedExp - client.ClientData.MerlinData._StarExp;
					if (nAddExp < nNeedExp)
					{
						client.ClientData.MerlinData._StarExp += nAddExp;
						break;
					}
					client.ClientData.MerlinData._StarNum++;
					client.ClientData.MerlinData._StarExp = 0;
					nAddExp -= nNeedExp;
					if (client.ClientData.MerlinData._StarNum >= MerlinSystemParamsConfigData._MaxStarNum)
					{
						if (nCurLevel < MerlinSystemParamsConfigData._MaxLevelNum)
						{
							client.ClientData.MerlinData._StarExp += nAddExp;
						}
						break;
					}
					if (nAddExp <= 0)
					{
						break;
					}
					nKey = this.GetMerlinStarUpKey(nCurLevel, client.ClientData.MerlinData._StarNum);
					lock (this.MerlinStarUpConfigDict)
					{
						if (!this.MerlinStarUpConfigDict.TryGetValue(nKey, out starData))
						{
							break;
						}
					}
				}
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					nRoleID,
					"*",
					"*",
					client.ClientData.MerlinData._StarNum,
					client.ClientData.MerlinData._StarExp,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				if (nCurStarNum != client.ClientData.MerlinData._StarNum)
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, nCurLevel, nCurStarNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				EventLogManager.AddMerlinBookStarEvent(client, nUseType, nAddExp, nCurStarNum, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, client.ClientData.MerlinData._StarExp, strCostList);
				return EMerlinStarUpErrorCode.Success;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinStarUpErrorCode.Error;
		}

		
		private EMerlinLevelUpErrorCode MerlinLevelUp(GameClient client, bool bIsDiamond)
		{
			string strCostList = "";
			int nRoleID = client.ClientData.RoleID;
			int nCurLevel = client.ClientData.MerlinData._Level;
			int nCurStarNum = client.ClientData.MerlinData._StarNum;
			int nOldFailedNum = client.ClientData.MerlinData._LevelUpFailNum;
			int nOldExp = client.ClientData.MerlinData._StarExp;
			int nUseType = 0;
			try
			{
				if (nCurLevel <= 0)
				{
					return EMerlinLevelUpErrorCode.LevelError;
				}
				if (nCurLevel >= MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinLevelUpErrorCode.MaxLevelNum;
				}
				if (nCurStarNum < MerlinSystemParamsConfigData._MaxStarNum)
				{
					return EMerlinLevelUpErrorCode.NotMaxStarNum;
				}
				MerlinLevelUpConfigData levelData = null;
				lock (this.MerlinLevelUpConfigDict)
				{
					if (!this.MerlinLevelUpConfigDict.TryGetValue(nCurLevel + 1, out levelData) || null == levelData)
					{
						return EMerlinLevelUpErrorCode.LevelDataError;
					}
				}
				SystemXmlItem needGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(levelData._NeedGoodsID, out needGoods))
				{
					return EMerlinLevelUpErrorCode.NeedGoodsIDError;
				}
				if (levelData._NeedGoodsCount <= 0)
				{
					return EMerlinLevelUpErrorCode.NeedGoodsCountError;
				}
				int nTotalGoodsCount = Global.GetTotalGoodsCountByID(client, levelData._NeedGoodsID);
				if (nTotalGoodsCount < levelData._NeedGoodsCount)
				{
					if (!bIsDiamond)
					{
						return EMerlinLevelUpErrorCode.GoodsNotEnough;
					}
					nUseType = 1;
				}
				switch (nUseType)
				{
				case 0:
				{
					bool bUsedBinding = false;
					bool bUsedTimeLimited = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, levelData._NeedGoodsID, levelData._NeedGoodsCount, false, out bUsedBinding, out bUsedTimeLimited, false))
					{
						return EMerlinLevelUpErrorCode.GoodsNotEnough;
					}
					GoodsData goodsDataCost = new GoodsData
					{
						GoodsID = levelData._NeedGoodsID,
						GCount = levelData._NeedGoodsCount
					};
					strCostList += EventLogManager.NewGoodsDataPropString(goodsDataCost);
					break;
				}
				case 1:
				{
					int oldUserMoney = client.ClientData.UserMoney;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, levelData._NeedDiamond, "梅林魔法书升阶", true, true, false, DaiBiSySType.MeiLingZhiShu))
					{
						return EMerlinLevelUpErrorCode.DiamondNotEnough;
					}
					strCostList += EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
					{
						-levelData._NeedDiamond,
						oldUserMoney,
						client.ClientData.UserMoney
					});
					break;
				}
				}
				if (client.ClientData.MerlinData._LuckyPoint <= 0)
				{
					client.ClientData.MerlinData._LuckyPoint = levelData._LuckyOne;
				}
				client.ClientData.MerlinData._LuckyPoint++;
				string strCmd;
				if (client.ClientData.MerlinData._LuckyPoint < levelData._LuckyTwo)
				{
					client.ClientData.MerlinData._LevelUpFailNum++;
					strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
					{
						nRoleID,
						"*",
						client.ClientData.MerlinData._LevelUpFailNum,
						"*",
						"*",
						client.ClientData.MerlinData._LuckyPoint,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*"
					});
					this.UpdateMerlinMagicBookData2DB(client, strCmd);
					return EMerlinLevelUpErrorCode.Fail;
				}
				int nRandom = Global.GetRandomNumber(0, 10001);
				int nRate = (int)(levelData._Rate * 10000.0);
				if (nRandom < nRate)
				{
					client.ClientData.MerlinData._Level++;
					client.ClientData.MerlinData._LevelUpFailNum = 0;
					client.ClientData.MerlinData._StarNum = 0;
					client.ClientData.MerlinData._LuckyPoint = 0;
					if (client.ClientData.MerlinData._Level >= MerlinSystemParamsConfigData._MaxLevelNum)
					{
						client.ClientData.MerlinData._StarExp = 0;
					}
					strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
					{
						nRoleID,
						client.ClientData.MerlinData._Level,
						client.ClientData.MerlinData._LevelUpFailNum,
						client.ClientData.MerlinData._StarNum,
						client.ClientData.MerlinData._StarExp,
						client.ClientData.MerlinData._LuckyPoint,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*"
					});
					this.UpdateMerlinMagicBookData2DB(client, strCmd);
					EventLogManager.AddMerlinBookStarEvent(client, 2, 0, nCurStarNum, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, client.ClientData.MerlinData._StarExp, strCostList);
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, nCurLevel, nCurStarNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					EventLogManager.AddMerlinBookLevEvent(client, nUseType, nOldFailedNum, client.ClientData.MerlinData._LevelUpFailNum, nCurLevel, client.ClientData.MerlinData._Level, nCurStarNum, client.ClientData.MerlinData._StarNum, nOldExp, client.ClientData.MerlinData._StarExp, strCostList);
					return EMerlinLevelUpErrorCode.Success;
				}
				client.ClientData.MerlinData._LevelUpFailNum++;
				strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					nRoleID,
					"*",
					client.ClientData.MerlinData._LevelUpFailNum,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				return EMerlinLevelUpErrorCode.Fail;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinLevelUpErrorCode.Error;
		}

		
		private EMerlinSecretAttrUpdateErrorCode MerlinSecretAttrUpdate(GameClient client)
		{
			int nRoleID = client.ClientData.RoleID;
			int nCurLevel = client.ClientData.MerlinData._Level;
			try
			{
				if (nCurLevel <= 0 || nCurLevel > MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinSecretAttrUpdateErrorCode.LevelError;
				}
				MerlinSecretConfigData secretData = null;
				lock (this.MerlinSecretConfigDict)
				{
					if (!this.MerlinSecretConfigDict.TryGetValue(nCurLevel, out secretData) || null == secretData)
					{
						return EMerlinSecretAttrUpdateErrorCode.SecretDataError;
					}
				}
				SystemXmlItem needGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(secretData._NeedGoodsID, out needGoods))
				{
					return EMerlinSecretAttrUpdateErrorCode.NeedGoodsIDError;
				}
				if (secretData._NeedGoodsCount <= 0)
				{
					return EMerlinSecretAttrUpdateErrorCode.NeedGoodsCountError;
				}
				int nTotalGoodsCount = Global.GetTotalGoodsCountByID(client, secretData._NeedGoodsID);
				if (nTotalGoodsCount < secretData._NeedGoodsCount)
				{
					return EMerlinSecretAttrUpdateErrorCode.GoodsNotEnough;
				}
				bool bUsedBinding = false;
				bool bUsedTimeLimited = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, secretData._NeedGoodsID, secretData._NeedGoodsCount, false, out bUsedBinding, out bUsedTimeLimited, false))
				{
					return EMerlinSecretAttrUpdateErrorCode.GoodsNotEnough;
				}
				int nAddTotalPointIndex = Global.GetRandomNumber(0, secretData._Num.Length);
				int nAddTotalPoint = secretData._Num[nAddTotalPointIndex];
				this.ResetUnActiveSecretAttr(client);
				while (nAddTotalPoint > 0)
				{
					int nTotalPoint = (int)(client.ClientData.MerlinData._UnActiveAttr[0] + client.ClientData.MerlinData._UnActiveAttr[1] + client.ClientData.MerlinData._UnActiveAttr[2] + client.ClientData.MerlinData._UnActiveAttr[3]);
					int nMaxPoint = MerlinSystemParamsConfigData._MaxSecretAttrNum * 4;
					if (nTotalPoint >= nMaxPoint)
					{
						break;
					}
					int nAttrIndex = Global.GetRandomNumber(0, 4);
					if (client.ClientData.MerlinData._UnActiveAttr[nAttrIndex] < (double)MerlinSystemParamsConfigData._MaxSecretAttrNum)
					{
						Dictionary<int, double> unActiveAttr;
						int key;
						(unActiveAttr = client.ClientData.MerlinData._UnActiveAttr)[key = nAttrIndex] = unActiveAttr[key] + 1.0;
						nAddTotalPoint--;
					}
				}
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					nRoleID,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				return EMerlinSecretAttrUpdateErrorCode.Success;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinSecretAttrUpdateErrorCode.Error;
		}

		
		private EMerlinSecretAttrReplaceErrorCode MerlinSecretAttrReplace(GameClient client)
		{
			bool bIsReplace = false;
			try
			{
				for (int i = 0; i < client.ClientData.MerlinData._UnActiveAttr.Count; i++)
				{
					if (client.ClientData.MerlinData._UnActiveAttr[i] > 0.0)
					{
						bIsReplace = true;
						break;
					}
				}
				if (!bIsReplace)
				{
					return EMerlinSecretAttrReplaceErrorCode.NotUpdate;
				}
				for (int i = 0; i < client.ClientData.MerlinData._ActiveAttr.Count; i++)
				{
					client.ClientData.MerlinData._ActiveAttr[i] = client.ClientData.MerlinData._UnActiveAttr[i];
				}
				this.ResetUnActiveSecretAttr(client);
				this.RefreshMerlinSecretSecondAttr(client);
				client.ClientData.MerlinData._ToTicks = TimeUtil.NOW() + (long)(MerlinSystemParamsConfigData._MaxSecretTime * 60 * 1000);
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					"*",
					"*",
					"*",
					client.ClientData.MerlinData._ToTicks,
					client.ClientData.MerlinData._ActiveAttr[0],
					client.ClientData.MerlinData._ActiveAttr[1],
					client.ClientData.MerlinData._ActiveAttr[2],
					client.ClientData.MerlinData._ActiveAttr[3],
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				return EMerlinSecretAttrReplaceErrorCode.Success;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinSecretAttrReplaceErrorCode.Error;
		}

		
		private void MerlinSecretAttrNotReplace(GameClient client)
		{
			try
			{
				this.ResetUnActiveSecretAttr(client);
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		private bool CreateMerlinMagicBookData2DB(GameClient client)
		{
			try
			{
				byte[] dataBytes = DataHelper.ObjectToBytes<MerlinGrowthSaveDBData>(client.ClientData.MerlinData);
				byte[] byRoleID = BitConverter.GetBytes(client.ClientData.RoleID);
				byte[] sendBytes = new byte[dataBytes.Length + 4];
				Array.Copy(byRoleID, sendBytes, 4);
				Array.Copy(dataBytes, 0, sendBytes, 4, dataBytes.Length);
				return Global.sendToDB<bool, byte[]>(10203, sendBytes, client.ServerId);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool UpdateMerlinMagicBookData2DB(GameClient client, string strCmd)
		{
			byte[] bytesCmd = new UTF8Encoding().GetBytes(strCmd);
			return Global.sendToDB<bool, byte[]>(10204, bytesCmd, client.ServerId);
		}

		
		private static string FormatUpdateDBMerlinStr(params object[] args)
		{
			string result;
			if (args.Length != 15)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("FormatUpdateDBMerlinStr, 参数个数不对{0}", args.Length), null, true);
				result = null;
			}
			else
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}:{14}", args);
			}
			return result;
		}

		
		public void RefreshMerlinExcellenceAttr(GameClient client, int nLevel, int nStarNum, bool bToAdd)
		{
			int nKey = this.GetMerlinStarUpKey(nLevel, nStarNum);
			MerlinStarUpConfigData starData = null;
			lock (this.MerlinStarUpConfigDict)
			{
				if (!this.MerlinStarUpConfigDict.TryGetValue(nKey, out starData) || null == starData)
				{
					return;
				}
			}
			if (starData._MpRecoverP > 0.0)
			{
				if (bToAdd)
				{
					client.ClientData.ExcellenceProp[16] += starData._MpRecoverP;
				}
				else
				{
					client.ClientData.ExcellenceProp[16] -= starData._MpRecoverP;
				}
			}
		}

		
		public void OnLoginInitMerlinMagicBook(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					if (null == client.ClientData.MerlinData)
					{
						client.ClientData.MerlinData = new MerlinGrowthSaveDBData();
					}
					if (client.ClientData.MerlinData._Level < 1)
					{
						client.ClientData.MerlinData._RoleID = client.ClientData.RoleID;
						client.ClientData.MerlinData._Level = 1;
						client.ClientData.MerlinData._Occupation = Global.CalcOriginalOccupationID(client);
						this.ResetActiveSecretAttr(client);
						this.ResetUnActiveSecretAttr(client);
						this.CreateMerlinMagicBookData2DB(client);
						this.CheckMerlinSecretAttr(client);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public void OnLoginAddAttr(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinSecretSecondAttr(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public void LoadMerlinConfigData()
		{
			this.LoadMerlinLevelUpConfigData();
			this.LoadMerlinStarUpConfigData();
			this.LoadMerlinSecretConfigData();
		}

		
		public void LoadMerlinSystemParamsConfigData()
		{
			try
			{
				MerlinSystemParamsConfigData._ReviveCDTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ChongShengCD"));
				MerlinSystemParamsConfigData._MaxSecretAttrNum = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MagicWordMax"));
				MerlinSystemParamsConfigData._MaxSecretTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MagicWordTime"));
				int[] ArrayMaxLevelAndStar = GameManager.systemParamsList.GetParamValueIntArrayByName("MagicBookLevel", ',');
				if (ArrayMaxLevelAndStar.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, "梅林魔法书最大阶数星数有误，无法读取", null, true);
				}
				else
				{
					MerlinSystemParamsConfigData._MaxLevelNum = ArrayMaxLevelAndStar[0];
					MerlinSystemParamsConfigData._MaxStarNum = ArrayMaxLevelAndStar[1];
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinSystemParamsConfigData", new object[0])));
			}
		}

		
		public bool IsOpenMerlin(GameClient client)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = false;
			}
			else if (null == client)
			{
				result = false;
			}
			else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MerlinMagicBook"))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("版本控制未开启梅林魔法书功能, RoleID={0}", client.ClientData.RoleID), null, true);
				result = false;
			}
			else
			{
				result = GlobalNew.IsGongNengOpened(client, GongNengIDs.MerlinMagicBook, false);
			}
			return result;
		}

		
		public void CheckMerlinSecretAttr(GameClient client)
		{
			if (this.IsOpenMerlin(client))
			{
				if (this.IsMerlinSecretTime(client))
				{
					client._IconStateMgr.AddFlushIconState(14201, false);
				}
				else
				{
					client._IconStateMgr.AddFlushIconState(14201, true);
				}
			}
		}

		
		public void DoMerlinSecretTime(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					long lNowTicks = TimeUtil.NOW();
					if (lNowTicks - this.nextCheckTime >= 5000L)
					{
						this.nextCheckTime = lNowTicks;
						if (!this.IsMerlinSecretTime(client))
						{
							if (client.ClientData.MerlinData._ToTicks > 0L)
							{
								client.ClientData.MerlinData._ToTicks = 0L;
								this.ResetActiveSecretAttr(client);
								this.RefreshMerlinSecretSecondAttr(client);
								string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
								{
									client.ClientData.RoleID,
									"*",
									"*",
									"*",
									"*",
									"*",
									client.ClientData.MerlinData._ToTicks,
									client.ClientData.MerlinData._ActiveAttr[0],
									client.ClientData.MerlinData._ActiveAttr[1],
									client.ClientData.MerlinData._ActiveAttr[2],
									client.ClientData.MerlinData._ActiveAttr[3],
									"*",
									"*",
									"*",
									"*"
								});
								this.UpdateMerlinMagicBookData2DB(client, strCmd);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
								this.CheckMerlinSecretAttr(client);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public void InitMerlinMagicBook(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					if (null == client.ClientData.MerlinData)
					{
						client.ClientData.MerlinData = new MerlinGrowthSaveDBData();
					}
					if (client.ClientData.MerlinData._Level < 1)
					{
						client.ClientData.MerlinData._RoleID = client.ClientData.RoleID;
						client.ClientData.MerlinData._Level = 1;
						client.ClientData.MerlinData._Occupation = Global.CalcOriginalOccupationID(client);
						this.ResetActiveSecretAttr(client);
						this.ResetUnActiveSecretAttr(client);
						this.CreateMerlinMagicBookData2DB(client);
						this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
						this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						this.CheckMerlinSecretAttr(client);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		
		public TCPProcessCmdResults ProcQueryMerlinMagicBookData(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (client.ClientData.MerlinData == null || client.ClientData.MerlinData._RoleID <= 0)
				{
					client.ClientData.MerlinData = Global.sendToDB<MerlinGrowthSaveDBData, string>(10205, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(client.ClientData.MerlinData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcMerlinMagicBookStarUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				int nIsDiamond = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				bool bIsDiamond = nIsDiamond != 0;
				int nIsCrit = 0;
				int nAddExp = 0;
				EMerlinStarUpErrorCode err = this.MerlinStarUp(client, bIsDiamond, out nIsCrit, out nAddExp);
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)err,
					client.ClientData.MerlinData._StarNum,
					client.ClientData.MerlinData._StarExp,
					nIsCrit,
					nAddExp
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcMerlinMagicBookLevelUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				int nIsDiamond = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				bool bIsDiamond = nIsDiamond != 0;
				EMerlinLevelUpErrorCode err = this.MerlinLevelUp(client, bIsDiamond);
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)err,
					client.ClientData.MerlinData._StarNum,
					client.ClientData.MerlinData._StarExp,
					client.ClientData.MerlinData._Level,
					client.ClientData.MerlinData._LevelUpFailNum
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcMerlinSecretAttrUpdate(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EMerlinSecretAttrUpdateErrorCode err = this.MerlinSecretAttrUpdate(client);
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)err,
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcMerlinSecretAttrReplace(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EMerlinSecretAttrReplaceErrorCode err = this.MerlinSecretAttrReplace(client);
				string strcmd = string.Format("{0}", (int)err);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcMerlinSecretAttrNotReplace(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
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
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int nRoleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client == null || client.ClientData.RoleID != nRoleID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), nRoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(client))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				this.MerlinSecretAttrNotReplace(client);
				string strcmd = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public void GMMerlinStarUp1(GameClient client)
		{
			int nCrit = 0;
			int nAddExp = 0;
			this.MerlinStarUp(client, false, out nCrit, out nAddExp);
		}

		
		public void GMMerlinStarUp2(GameClient client)
		{
			int nCrit = 0;
			int nAddExp = 0;
			this.MerlinStarUp(client, true, out nCrit, out nAddExp);
		}

		
		public void GMMerlinLevelUp1(GameClient client)
		{
			this.MerlinLevelUp(client, false);
		}

		
		public void GMMerlinLevelUp2(GameClient client)
		{
			this.MerlinLevelUp(client, true);
		}

		
		public void GMMerlinSecretUpdate(GameClient client)
		{
			this.MerlinSecretAttrUpdate(client);
		}

		
		public void GMMerlinSecretReplace(GameClient client)
		{
			this.MerlinSecretAttrReplace(client);
		}

		
		public void GMMerlinSecretNotReplace(GameClient client)
		{
			this.MerlinSecretAttrNotReplace(client);
		}

		
		public void GMMerlinInit(GameClient client)
		{
			this.InitMerlinMagicBook(client);
		}

		
		public string GMMerlinLevelUpToN(GameClient client, int nLevel)
		{
			string lang;
			if (client == null || !this.IsOpenMerlin(client))
			{
				lang = GLang.GetLang(495, new object[0]);
			}
			else if (nLevel < 1)
			{
				lang = GLang.GetLang(496, new object[0]);
			}
			else
			{
				nLevel = Math.Min(nLevel, MerlinSystemParamsConfigData._MaxLevelNum);
				int nCurLevel = client.ClientData.MerlinData._Level;
				int nCurStarNum = client.ClientData.MerlinData._StarNum;
				client.ClientData.MerlinData._Level = nLevel;
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					nLevel,
					0,
					"*",
					"*",
					0,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				if (!this.UpdateMerlinMagicBookData2DB(client, strCmd))
				{
					lang = GLang.GetLang(497, new object[0]);
				}
				else
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, nCurLevel, nCurStarNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					lang = GLang.GetLang(498, new object[0]);
				}
			}
			return lang;
		}

		
		public string GMMerlinStarUpToN(GameClient client, int nStarNum)
		{
			string lang;
			if (client == null || !this.IsOpenMerlin(client))
			{
				lang = GLang.GetLang(495, new object[0]);
			}
			else if (nStarNum < 0)
			{
				lang = GLang.GetLang(499, new object[0]);
			}
			else
			{
				nStarNum = Math.Min(nStarNum, MerlinSystemParamsConfigData._MaxStarNum);
				int nCurLevel = client.ClientData.MerlinData._Level;
				int nCurStarNum = client.ClientData.MerlinData._StarNum;
				client.ClientData.MerlinData._StarNum = nStarNum;
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					nStarNum,
					0,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				if (!this.UpdateMerlinMagicBookData2DB(client, strCmd))
				{
					lang = GLang.GetLang(500, new object[0]);
				}
				else
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, nCurLevel, nCurStarNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					lang = GLang.GetLang(501, new object[0]);
				}
			}
			return lang;
		}

		
		private Dictionary<int, MerlinLevelUpConfigData> MerlinLevelUpConfigDict = new Dictionary<int, MerlinLevelUpConfigData>();

		
		private Dictionary<int, MerlinStarUpConfigData> MerlinStarUpConfigDict = new Dictionary<int, MerlinStarUpConfigData>();

		
		private Dictionary<int, MerlinSecretConfigData> MerlinSecretConfigDict = new Dictionary<int, MerlinSecretConfigData>();

		
		private long nextCheckTime = 0L;
	}
}
