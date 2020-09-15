using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Logic.Damon;
using GameServer.Logic.Goods;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000298 RID: 664
	public class JingLingQiYuanManager : IManager
	{
		// Token: 0x060009D4 RID: 2516 RVA: 0x0009C474 File Offset: 0x0009A674
		public static JingLingQiYuanManager getInstance()
		{
			return JingLingQiYuanManager.instance;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0009C48C File Offset: 0x0009A68C
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0009C4B0 File Offset: 0x0009A6B0
		public bool startup()
		{
			return true;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0009C4C4 File Offset: 0x0009A6C4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0009C4D8 File Offset: 0x0009A6D8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0009C54C File Offset: 0x0009A74C
		public bool InitConfig()
		{
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.PetGroupPropertyList.Clear();
					fileName = "Config/PetGroupProperty.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						PetGroupPropertyItem item = new PetGroupPropertyItem();
						item.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item.Name = Global.GetSafeAttributeStr(node, "Name");
						string petGoods = Global.GetSafeAttributeStr(node, "PetGoods");
						item.PetGoodsList = ConfigParser.ParserIntArrayList(petGoods, true, '|', ',');
						string groupProperty = Global.GetSafeAttributeStr(node, "GroupProperty");
						item.PropItem = ConfigParser.ParseEquipPropItem(groupProperty, true, '|', ',', '-');
						this.RuntimeData.PetGroupPropertyList.Add(item);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetLevelAwardList.Clear();
					fileName = "Config/PetLevelAward.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						PetLevelAwardItem item2 = new PetLevelAwardItem();
						item2.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.Level = (int)Global.GetSafeAttributeLong(node, "Level");
						string shuXing = Global.GetSafeAttributeStr(node, "ShuXing");
						item2.PropItem = ConfigParser.ParseEquipPropItem(shuXing, true, '|', ',', '-');
						this.RuntimeData.PetLevelAwardList.Add(item2);
					}
					this.RuntimeData.PetLevelAwardList.Sort((PetLevelAwardItem x, PetLevelAwardItem y) => x.Level - y.Level);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetSkillLevelAwardList.Clear();
					fileName = "Config/PetSkillLevelAward.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						PetSkillLevelAwardItem item3 = new PetSkillLevelAwardItem();
						item3.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.Level = (int)Global.GetSafeAttributeLong(node, "Level");
						string shuXing = Global.GetSafeAttributeStr(node, "ShuXing");
						item3.PropItem = ConfigParser.ParseEquipPropItem(shuXing, true, '|', ',', '-');
						this.RuntimeData.PetSkillLevelAwardList.Add(item3);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetTianFuAwardList.Clear();
					fileName = "Config/PetTianFuAward.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						PetTianFuAwardItem item4 = new PetTianFuAwardItem();
						item4.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item4.TianFuNum = (int)Global.GetSafeAttributeLong(node, "TianFuNum");
						string shuXing = Global.GetSafeAttributeStr(node, "ShuXing");
						item4.PropItem = ConfigParser.ParseEquipPropItem(shuXing, true, '|', ',', '-');
						this.RuntimeData.PetTianFuAwardList.Add(item4);
					}
					this.RuntimeData.PetTianFuAwardList.Sort((PetTianFuAwardItem x, PetTianFuAwardItem y) => x.TianFuNum - y.TianFuNum);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
			}
			try
			{
				this.RuntimeData.PetSkillAwardList.Clear();
				fileName = "Config/PetSkillGroupProperty.xml";
				string fullPathFileName = Global.GameResPath(fileName);
				XElement xml = XElement.Load(fullPathFileName);
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						PetSkillGroupInfo config = new PetSkillGroupInfo();
						config.GroupID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
						config.SkillList = new List<int>();
						string skills = Global.GetDefAttributeStr(xmlItem, "SkillList", "");
						if (!string.IsNullOrEmpty(skills))
						{
							string[] arr = skills.Split(new char[]
							{
								'|'
							});
							foreach (string s in arr)
							{
								config.SkillList.Add(int.Parse(s));
							}
						}
						config.SkillNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SkillNum", "0"));
						string prop = Global.GetDefAttributeStr(xmlItem, "Property", "0");
						config.GroupProp = this.GetGroupProp(prop);
						this.RuntimeData.PetSkillAwardList.Add(config);
					}
				}
				this.RuntimeData.PetSkillAwardList.Sort((PetSkillGroupInfo x, PetSkillGroupInfo y) => x.GroupID - y.GroupID);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
			}
			return true;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0009CCD0 File Offset: 0x0009AED0
		private EquipPropItem GetGroupProp(string strEffect)
		{
			EquipPropItem result;
			if (string.IsNullOrEmpty(strEffect))
			{
				result = null;
			}
			else
			{
				EquipPropItem item = new EquipPropItem();
				string[] arrEffect = strEffect.Split(new char[]
				{
					'|'
				});
				foreach (string effect in arrEffect)
				{
					string[] arr = effect.Split(new char[]
					{
						','
					});
					int id = (int)Enum.Parse(typeof(ExtPropIndexes), arr[0]);
					double value = double.Parse(arr[1]);
					item.ExtProps[id] += value;
				}
				result = item;
			}
			return result;
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0009CDD0 File Offset: 0x0009AFD0
		public void RefreshProps(GameClient client, bool notifyPorpsChangeInfo = true)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot5))
			{
				int sumPetLevel = 0;
				int findPetLevel = 0;
				int sumPetTianFuNum = 0;
				int findPetTianFuNum = 0;
				int sumPetSkillLevel = 0;
				int findPetSkillLevel = 0;
				List<PetSkillInfo> petSkillList = new List<PetSkillInfo>();
				EquipPropItem petLevelAwardItem = null;
				EquipPropItem petTianFuAwardItem = null;
				EquipPropItem petSkillAwardItem = null;
				EquipPropItem petSkillLevelAwardItem = null;
				Dictionary<int, GoodsData> havingPetDict = new Dictionary<int, GoodsData>();
				Dictionary<int, EquipPropItem> groupPropItemDict = new Dictionary<int, EquipPropItem>();
				List<GoodsData> demonGoodsList = DamonMgr.GetDemonGoodsDataList(client);
				foreach (GoodsData goodsData in demonGoodsList)
				{
					GoodsData existGoodsData;
					if (!havingPetDict.TryGetValue(goodsData.GoodsID, out existGoodsData))
					{
						existGoodsData = new GoodsData();
						existGoodsData.GoodsID = goodsData.GoodsID;
						existGoodsData.GCount = 0;
						havingPetDict[existGoodsData.GoodsID] = existGoodsData;
					}
					existGoodsData.GCount++;
					sumPetLevel += goodsData.Forge_level + 1;
					sumPetTianFuNum += Global.GetEquipExcellencePropNum(goodsData);
					petSkillList.AddRange(PetSkillManager.GetPetSkillInfo(goodsData));
				}
				foreach (PetSkillInfo item in petSkillList)
				{
					sumPetSkillLevel += (item.PitIsOpen ? item.Level : 0);
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (PetLevelAwardItem item2 in this.RuntimeData.PetLevelAwardList)
					{
						if (sumPetLevel >= item2.Level && item2.Level > findPetLevel)
						{
							findPetLevel = item2.Level;
							petLevelAwardItem = item2.PropItem;
						}
					}
					foreach (PetTianFuAwardItem item3 in this.RuntimeData.PetTianFuAwardList)
					{
						if (sumPetTianFuNum >= item3.TianFuNum && item3.TianFuNum > findPetTianFuNum)
						{
							findPetTianFuNum = item3.TianFuNum;
							petTianFuAwardItem = item3.PropItem;
						}
					}
					foreach (PetGroupPropertyItem item4 in this.RuntimeData.PetGroupPropertyList)
					{
						groupPropItemDict[item4.Id] = null;
						bool avalid = true;
						foreach (List<int> list in item4.PetGoodsList)
						{
							GoodsData existGoodsData;
							if (!havingPetDict.TryGetValue(list[0], out existGoodsData) || existGoodsData.GCount < list[1])
							{
								avalid = false;
								break;
							}
						}
						if (avalid)
						{
							groupPropItemDict[item4.Id] = item4.PropItem;
						}
					}
					foreach (PetSkillGroupInfo item5 in this.RuntimeData.PetSkillAwardList)
					{
						int sum = 0;
						using (List<int>.Enumerator enumerator8 = item5.SkillList.GetEnumerator())
						{
							while (enumerator8.MoveNext())
							{
								int p = enumerator8.Current;
								IEnumerable<PetSkillInfo> temp = from info in petSkillList
								where info.PitIsOpen && info.SkillID > 0 && info.SkillID == p
								select info;
								if (temp.Any<PetSkillInfo>())
								{
									sum += temp.Count<PetSkillInfo>();
								}
							}
						}
						if (sum < item5.SkillNum)
						{
							break;
						}
						petSkillAwardItem = item5.GroupProp;
					}
					foreach (PetSkillLevelAwardItem item6 in this.RuntimeData.PetSkillLevelAwardList)
					{
						if (sumPetSkillLevel >= item6.Level && item6.Level > findPetSkillLevel)
						{
							findPetSkillLevel = item6.Level;
							petSkillLevelAwardItem = item6.PropItem;
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					0,
					petLevelAwardItem
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					1,
					petTianFuAwardItem
				});
				foreach (KeyValuePair<int, EquipPropItem> groupPropItem in groupPropItemDict)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JingLingQiYuan,
						2,
						groupPropItem.Key,
						groupPropItem.Value
					});
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					3,
					petSkillAwardItem
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					4,
					petSkillLevelAwardItem
				});
				if (notifyPorpsChangeInfo)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		// Token: 0x04001064 RID: 4196
		private static JingLingQiYuanManager instance = new JingLingQiYuanManager();

		// Token: 0x04001065 RID: 4197
		public JingLingQiYuanData RuntimeData = new JingLingQiYuanData();

		// Token: 0x02000299 RID: 665
		private static class SubPropsTypes
		{
			// Token: 0x04001069 RID: 4201
			public const int Level = 0;

			// Token: 0x0400106A RID: 4202
			public const int TianFuNum = 1;

			// Token: 0x0400106B RID: 4203
			public const int PetGroup = 2;

			// Token: 0x0400106C RID: 4204
			public const int PetSkill = 3;

			// Token: 0x0400106D RID: 4205
			public const int PetSkillLev = 4;
		}
	}
}
