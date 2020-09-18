using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class ChangeLifeManager
	{
		
		public void LoadRoleZhuanShengInfo()
		{
			for (int i = 0; i < 6; i++)
			{
				if (i != 4)
				{
					XElement xmlFile = null;
					try
					{
						xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanSheng_{0}.xml", i));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanSheng_{0}.xml", i)));
					}
					IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ZhuanShengs").Elements<XElement>();
					Dictionary<int, ChangeLifeDataInfo> tmpDic = new Dictionary<int, ChangeLifeDataInfo>();
					foreach (XElement xmlItem in ChgOccpXEle)
					{
						ChangeLifeDataInfo tmpChgLifeInfo = new ChangeLifeDataInfo();
						int nLifeID = 0;
						if (null != xmlItem)
						{
							nLifeID = (int)Global.GetSafeAttributeLong(xmlItem, "ChangeLifeID");
							tmpChgLifeInfo.ChangeLifeID = (int)Global.GetSafeAttributeLong(xmlItem, "ChangeLifeID");
							tmpChgLifeInfo.NeedLevel = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
							tmpChgLifeInfo.NeedMoney = (int)Global.GetSafeAttributeLong(xmlItem, "NeedJinBi");
							tmpChgLifeInfo.NeedMoJing = (int)Global.GetSafeAttributeLong(xmlItem, "NeedMoJing");
							tmpChgLifeInfo.ExpProportion = Global.GetSafeAttributeLong(xmlItem, "ExpProportion");
							string sGoodsID = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
							if (string.IsNullOrEmpty(sGoodsID))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] fields = sGoodsID.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									tmpChgLifeInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID, "转生文件");
								}
							}
							string strShuXingInfos = Global.GetSafeAttributeStr(xmlItem, "AwardShuXing");
							string[] sArrayPropInfo = strShuXingInfos.Split(new char[]
							{
								'|'
							});
							if (sArrayPropInfo != null)
							{
								tmpChgLifeInfo.Propertyinfo = new ChangeLifePropertyInfo();
								for (int j = 0; j < sArrayPropInfo.Length; j++)
								{
									string[] sArryInfo = sArrayPropInfo[j].Split(new char[]
									{
										','
									});
									string strPorpName = sArryInfo[0];
									string strPorpValue = sArryInfo[1];
									string[] strArrayPorpValue = strPorpValue.Split(new char[]
									{
										'-'
									});
									if (strPorpName == "Defense")
									{
										tmpChgLifeInfo.Propertyinfo.PhyDefenseMin = 3;
										tmpChgLifeInfo.Propertyinfo.AddPhyDefenseMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.PhyDefenseMax = 4;
										tmpChgLifeInfo.Propertyinfo.AddPhyDefenseMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mdefense")
									{
										tmpChgLifeInfo.Propertyinfo.MagDefenseMin = 5;
										tmpChgLifeInfo.Propertyinfo.AddMagDefenseMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.MagDefenseMax = 6;
										tmpChgLifeInfo.Propertyinfo.AddMagDefenseMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Attack")
									{
										tmpChgLifeInfo.Propertyinfo.PhyAttackMin = 7;
										tmpChgLifeInfo.Propertyinfo.AddPhyAttackMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.PhyAttackMax = 8;
										tmpChgLifeInfo.Propertyinfo.AddPhyAttackMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mattack")
									{
										tmpChgLifeInfo.Propertyinfo.MagAttackMin = 9;
										tmpChgLifeInfo.Propertyinfo.AddMagAttackMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.MagAttackMax = 10;
										tmpChgLifeInfo.Propertyinfo.AddMagAttackMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "HitV")
									{
										tmpChgLifeInfo.Propertyinfo.HitProp = 18;
										tmpChgLifeInfo.Propertyinfo.AddHitPropValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "Dodge")
									{
										tmpChgLifeInfo.Propertyinfo.DodgeProp = 19;
										tmpChgLifeInfo.Propertyinfo.AddDodgePropValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "MaxLifeV")
									{
										tmpChgLifeInfo.Propertyinfo.MaxLifeProp = 13;
										tmpChgLifeInfo.Propertyinfo.AddMaxLifePropValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "AddAttack")
									{
										tmpChgLifeInfo.Propertyinfo.PhyAttackMin = 7;
										tmpChgLifeInfo.Propertyinfo.AddPhyAttackMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.PhyAttackMax = 8;
										tmpChgLifeInfo.Propertyinfo.AddPhyAttackMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
										tmpChgLifeInfo.Propertyinfo.MagAttackMin = 9;
										tmpChgLifeInfo.Propertyinfo.AddMagAttackMinValue = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpChgLifeInfo.Propertyinfo.MagAttackMax = 10;
										tmpChgLifeInfo.Propertyinfo.AddMagAttackMaxValue = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
								}
							}
							string sGoodsID2 = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
							if (string.IsNullOrEmpty(sGoodsID2))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] fields2 = sGoodsID2.Split(new char[]
								{
									'|'
								});
								if (fields2.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									tmpChgLifeInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID2, "转生文件");
								}
							}
						}
						if (nLifeID > this.m_MaxChangeLifeCount)
						{
							this.m_MaxChangeLifeCount = nLifeID;
						}
						if (nLifeID > 1)
						{
							tmpChgLifeInfo.Propertyinfo.AddFrom(tmpDic[nLifeID - 1].Propertyinfo);
						}
						tmpDic.Add(nLifeID, tmpChgLifeInfo);
					}
					this.m_ChangeLifeInfoList.Add(i, tmpDic);
				}
			}
		}

		
		public ChangeLifeDataInfo GetChangeLifeDataInfo(GameClient Client, int nChangeLife = 0)
		{
			if (nChangeLife == 0)
			{
				nChangeLife = Client.ClientData.ChangeLifeCount;
			}
			Dictionary<int, ChangeLifeDataInfo> dicTmp = new Dictionary<int, ChangeLifeDataInfo>();
			ChangeLifeDataInfo result;
			if (!GameManager.ChangeLifeMgr.m_ChangeLifeInfoList.TryGetValue(Client.ClientData.Occupation, out dicTmp))
			{
				result = null;
			}
			else
			{
				ChangeLifeDataInfo infoTmp = new ChangeLifeDataInfo();
				if (!dicTmp.TryGetValue(nChangeLife, out infoTmp))
				{
					result = null;
				}
				else
				{
					result = infoTmp;
				}
			}
			return result;
		}

		
		public void InitPlayerChangeLifePorperty(GameClient client)
		{
			if (client.ClientData.ChangeLifeCount > 0)
			{
				int nOccupation = client.ClientData.Occupation;
				Dictionary<int, ChangeLifeDataInfo> dicTmp = null;
				if (this.m_ChangeLifeInfoList.TryGetValue(nOccupation, out dicTmp) && dicTmp != null)
				{
					ChangeLifeDataInfo dataTmp = new ChangeLifeDataInfo();
					if (dicTmp.TryGetValue(client.ClientData.ChangeLifeCount, out dataTmp) && dataTmp != null)
					{
						ChangeLifePropertyInfo tmpProp = dataTmp.Propertyinfo;
						if (tmpProp != null)
						{
							this.ActivationChangeLifeProp(client, tmpProp);
						}
					}
				}
			}
		}

		
		public void ProcessRoleChangeLifeProp(GameClient client)
		{
			this.InitPlayerChangeLifePorperty(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
		}

		
		public void ActivationChangeLifeProp(GameClient client, ChangeLifePropertyInfo tmpProp)
		{
			client.ClientData.RoleChangeLifeProp.ResetChangeLifeProps();
			if (tmpProp.PhyAttackMin >= 0 && tmpProp.AddPhyAttackMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyAttackMin] += (double)tmpProp.AddPhyAttackMinValue;
			}
			if (tmpProp.PhyAttackMax >= 0 && tmpProp.AddPhyAttackMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyAttackMax] += (double)tmpProp.AddPhyAttackMaxValue;
			}
			if (tmpProp.MagAttackMin >= 0 && tmpProp.AddMagAttackMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagAttackMin] += (double)tmpProp.AddMagAttackMinValue;
			}
			if (tmpProp.MagAttackMax >= 0 && tmpProp.AddMagAttackMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagAttackMax] += (double)tmpProp.AddMagAttackMaxValue;
			}
			if (tmpProp.PhyDefenseMin >= 0 && tmpProp.AddPhyDefenseMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyDefenseMin] += (double)tmpProp.AddPhyDefenseMinValue;
			}
			if (tmpProp.PhyDefenseMax >= 0 && tmpProp.AddPhyDefenseMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyDefenseMax] += (double)tmpProp.AddPhyDefenseMaxValue;
			}
			if (tmpProp.MagDefenseMin >= 0 && tmpProp.AddMagDefenseMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagDefenseMin] += (double)tmpProp.AddMagDefenseMinValue;
			}
			if (tmpProp.MagDefenseMax >= 0 && tmpProp.AddMagDefenseMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagDefenseMax] += (double)tmpProp.AddMagDefenseMaxValue;
			}
			if (tmpProp.HitProp >= 0 && tmpProp.AddHitPropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.HitProp] += (double)tmpProp.AddHitPropValue;
			}
			if (tmpProp.DodgeProp >= 0 && tmpProp.AddDodgePropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.DodgeProp] += (double)tmpProp.AddDodgePropValue;
			}
			if (tmpProp.MaxLifeProp >= 0 && tmpProp.AddMaxLifePropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MaxLifeProp] += (double)tmpProp.AddMaxLifePropValue;
			}
		}

		
		public Dictionary<int, Dictionary<int, ChangeLifeDataInfo>> m_ChangeLifeInfoList = new Dictionary<int, Dictionary<int, ChangeLifeDataInfo>>();

		
		public int m_MaxChangeLifeCount = 0;
	}
}
