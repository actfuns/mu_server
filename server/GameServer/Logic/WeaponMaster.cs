using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class WeaponMaster
	{
		
		public static void UpdateRoleAttr(GameClient client, int weaponType, bool needBrocast = false)
		{
			try
			{
				List<WeaponMaster.WeaponMasterItem> weaponMasterList;
				if (WeaponMaster.WeaponMasterXml.TryGetValue(weaponType, out weaponMasterList))
				{
					int begin = 11;
					int end = 21;
					List<int> equipHandleList = new List<int>();
					foreach (GoodsData _g in client.ClientData.GoodsDataList)
					{
						if (_g.Using == 1)
						{
							int cateGoriy = -1;
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(_g.GoodsID, out systemGoods))
							{
								cateGoriy = systemGoods.GetIntValue("Categoriy", -1);
							}
							if (cateGoriy >= begin && cateGoriy <= end)
							{
								equipHandleList.Add(cateGoriy);
							}
						}
					}
					if (equipHandleList.Count >= 1 && equipHandleList.Count <= 2)
					{
						WeaponMaster.WeaponMasterItem weaponItem = null;
						foreach (WeaponMaster.WeaponMasterItem _w in weaponMasterList)
						{
							if (WeaponMaster.WeaponIsMatch(_w.WeaponType1, _w.WeaponType2, equipHandleList) || WeaponMaster.WeaponIsMatch(_w.WeaponType2, _w.WeaponType1, equipHandleList))
							{
								weaponItem = _w;
								break;
							}
						}
						double[] ExtProps = (weaponItem == null) ? new double[177] : weaponItem.ExtProps;
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.WeaponMaster,
							ExtProps
						});
						if (needBrocast)
						{
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("WeaponMaster :: 更新角色武器大师属性加成:{0}, 失败。", new object[0]), ex, true);
			}
		}

		
		public static bool WeaponIsMatch(List<int> leftList, List<int> rightList, List<int> equipList)
		{
			int checkIndex = 1;
			if (leftList == null || leftList.Count < 1 || leftList[0] < 0)
			{
				if (equipList.Count > 1)
				{
					return false;
				}
				checkIndex = 0;
			}
			else if (!leftList.Contains(equipList[0]))
			{
				return false;
			}
			bool result;
			if (rightList == null || rightList.Count < 1 || rightList[0] < 0)
			{
				result = (equipList.Count < checkIndex + 1);
			}
			else
			{
				result = (equipList.Count >= checkIndex + 1 && rightList.Contains(equipList[checkIndex]));
			}
			return result;
		}

		
		public static void LoadWeaponMaster()
		{
			string fileName = "Config\\WeaponMaster.xml";
			try
			{
				fileName = Global.GameResPath(fileName);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0"));
							List<WeaponMaster.WeaponMasterItem> weaponMasterList;
							if (!WeaponMaster.WeaponMasterXml.TryGetValue(type, out weaponMasterList))
							{
								weaponMasterList = new List<WeaponMaster.WeaponMasterItem>();
								WeaponMaster.WeaponMasterXml[type] = weaponMasterList;
							}
							string weaponType = Global.GetDefAttributeStr(xmlItem, "WeaponType1", "");
							string weaponType2 = Global.GetDefAttributeStr(xmlItem, "WeaponType2", "");
							string tempValueString = Global.GetSafeAttributeStr(xmlItem, "WeaponMasterProps");
							string[] valueFileds = tempValueString.Split(new char[]
							{
								'|'
							});
							double[] extProps = new double[177];
							foreach (string value in valueFileds)
							{
								string[] KvpFileds = value.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
									if (index < ExtPropIndexes.Max)
									{
										extProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
									}
								}
							}
							List<WeaponMaster.WeaponMasterItem> list = weaponMasterList;
							WeaponMaster.WeaponMasterItem weaponMasterItem = new WeaponMaster.WeaponMasterItem();
							WeaponMaster.WeaponMasterItem weaponMasterItem2 = weaponMasterItem;
							List<int> weaponType3;
							if (!("" == weaponType))
							{
								weaponType3 = Array.ConvertAll<string, int>(weaponType.Split(new char[]
								{
									','
								}), (string x) => Convert.ToInt32(x)).ToList<int>();
							}
							else
							{
								weaponType3 = new List<int>();
							}
							weaponMasterItem2.WeaponType1 = weaponType3;
							WeaponMaster.WeaponMasterItem weaponMasterItem3 = weaponMasterItem;
							List<int> weaponType4;
							if (!("" == weaponType2))
							{
								weaponType4 = Array.ConvertAll<string, int>(weaponType2.Split(new char[]
								{
									','
								}), (string x) => Convert.ToInt32(x)).ToList<int>();
							}
							else
							{
								weaponType4 = new List<int>();
							}
							weaponMasterItem3.WeaponType2 = weaponType4;
							weaponMasterItem.ExtProps = extProps;
							list.Add(weaponMasterItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public static Dictionary<int, List<WeaponMaster.WeaponMasterItem>> WeaponMasterXml = new Dictionary<int, List<WeaponMaster.WeaponMasterItem>>();

		
		public class WeaponMasterItem
		{
			
			public int Type;

			
			public List<int> WeaponType1;

			
			public List<int> WeaponType2;

			
			public double[] ExtProps = new double[177];
		}
	}
}
