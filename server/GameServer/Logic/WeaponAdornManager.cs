using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic
{
	
	public class WeaponAdornManager
	{
		
		public static int GetWeaponAdornOrder(int nOccupation, int nHandType, int nActionType)
		{
			int result;
			if (nOccupation < 0 || nHandType < 0 || nActionType < 0)
			{
				result = -1;
			}
			else
			{
				result = 1000 * nOccupation + 100 * nHandType + nActionType;
			}
			return result;
		}

		
		public static WeaponAdornInfo GetWeaponAdornInfo(int nOccupation, int nHandType, int nActionType)
		{
			int nOrder = WeaponAdornManager.GetWeaponAdornOrder(nOccupation, nHandType, nActionType);
			WeaponAdornInfo result;
			if (nOrder < 0)
			{
				result = null;
			}
			else
			{
				WeaponAdornInfo weaponInfo = null;
				if (!WeaponAdornManager.dictWeaponAdornInfo.TryGetValue(nOrder, out weaponInfo))
				{
					result = null;
				}
				else
				{
					result = weaponInfo;
				}
			}
			return result;
		}

		
		public static int VerifyWeaponCanEquip(int nOccupation, int nHandType, int nActionType, Dictionary<int, List<GoodsData>> EquipDict)
		{
			WeaponAdornInfo weaponInfo = WeaponAdornManager.GetWeaponAdornInfo(nOccupation, nHandType, nActionType);
			int result;
			if (null == weaponInfo)
			{
				result = -1;
			}
			else
			{
				int nWeaponCount = 0;
				List<GoodsData> listGood = null;
				int i = 11;
				while (i < 22)
				{
					listGood = null;
					lock (EquipDict)
					{
						if (EquipDict.TryGetValue(i, out listGood))
						{
							if (listGood != null && listGood.Count > 0)
							{
								if (weaponInfo.listCoexistType.Count < 1)
								{
									return -5;
								}
								nWeaponCount += listGood.Count;
								bool bCanEquip = false;
								for (int nCount = 0; nCount < listGood.Count; nCount++)
								{
									bCanEquip = false;
									SystemXmlItem systemGoods = null;
									if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(listGood[nCount].GoodsID, out systemGoods))
									{
										return -1;
									}
									int nExistHandType = systemGoods.GetIntValue("HandType", -1);
									int nExistActionType = systemGoods.GetIntValue("ActionType", -1);
									for (int nCoexistCount = 0; nCoexistCount < weaponInfo.listCoexistType.Count; nCoexistCount++)
									{
										if (weaponInfo.listCoexistType[nCoexistCount].nHandType == nExistHandType && weaponInfo.listCoexistType[nCoexistCount].nActionType == nExistActionType)
										{
											bCanEquip = true;
											break;
										}
									}
								}
								if (!bCanEquip)
								{
									return -5;
								}
							}
						}
					}
					IL_19C:
					i++;
					continue;
					goto IL_19C;
				}
				if (nWeaponCount > 1)
				{
					result = -3;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		
		public static void LoadWeaponAdornInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/WeaponAdorn.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("Weapons").Elements<XElement>();
					foreach (XElement xmlItem in ChgOccpXEle)
					{
						if (null != xmlItem)
						{
							WeaponAdornInfo tmpInfo = new WeaponAdornInfo();
							int nOccupation = (int)Global.GetSafeAttributeLong(xmlItem, "Occupation");
							tmpInfo.nOccupationLimit = nOccupation;
							string strWeaponType = Global.GetSafeAttributeStr(xmlItem, "Type");
							if (!string.IsNullOrEmpty(strWeaponType.Trim()))
							{
								string[] strFields = strWeaponType.Split(new char[]
								{
									','
								});
								if (strFields != null && strFields.Length == 2)
								{
									tmpInfo.tagWeaponTypeInfo.nHandType = Convert.ToInt32(strFields[0]);
									tmpInfo.tagWeaponTypeInfo.nActionType = Convert.ToInt32(strFields[1]);
								}
							}
							string strCoexistType = Global.GetSafeAttributeStr(xmlItem, "CoexistType");
							if (!string.IsNullOrEmpty(strCoexistType.Trim()))
							{
								string[] strFields = strCoexistType.Split(new char[]
								{
									'|'
								});
								if (strFields != null && strFields.Length > 0)
								{
									for (int i = 0; i < strFields.Length; i++)
									{
										string[] strWeaponTypes = strFields[i].Split(new char[]
										{
											','
										});
										if (strWeaponTypes != null && strWeaponTypes.Length == 2)
										{
											WeaponTypeAndACTInfo tmpCoexistType = new WeaponTypeAndACTInfo();
											tmpCoexistType.nHandType = Convert.ToInt32(strWeaponTypes[0]);
											tmpCoexistType.nActionType = Convert.ToInt32(strWeaponTypes[1]);
											tmpInfo.listCoexistType.Add(tmpCoexistType);
										}
									}
								}
							}
							int nOrder = WeaponAdornManager.GetWeaponAdornOrder(nOccupation, tmpInfo.tagWeaponTypeInfo.nHandType, tmpInfo.tagWeaponTypeInfo.nActionType);
							if (nOrder > 0)
							{
								WeaponAdornManager.dictWeaponAdornInfo.Add(nOrder, tmpInfo);
							}
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/WeaponAdorn.xml", new object[0])));
			}
		}

		
		public static Dictionary<int, WeaponAdornInfo> dictWeaponAdornInfo = new Dictionary<int, WeaponAdornInfo>();
	}
}
