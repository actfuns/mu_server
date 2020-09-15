using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020005B5 RID: 1461
	public class AdvanceBufferPropsMgr
	{
		// Token: 0x06001A74 RID: 6772 RVA: 0x00195138 File Offset: 0x00193338
		private static int[] GetCachingIDsByID(int id)
		{
			int[] ids = null;
			lock (AdvanceBufferPropsMgr.CachingIDsDict)
			{
				if (!AdvanceBufferPropsMgr.CachingIDsDict.TryGetValue(id, out ids))
				{
					string paramName = "";
					if (AdvanceBufferPropsMgr.BufferId2ConfigParamsNameDict.TryGetValue((BufferItemTypes)id, out paramName))
					{
						ids = GameManager.systemParamsList.GetParamValueIntArrayByName(paramName, ',');
					}
					AdvanceBufferPropsMgr.CachingIDsDict[id] = ids;
				}
			}
			return ids;
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x001951D8 File Offset: 0x001933D8
		public static void ResetCache()
		{
			lock (AdvanceBufferPropsMgr.CachingIDsDict)
			{
				AdvanceBufferPropsMgr.CachingIDsDict.Clear();
				foreach (KeyValuePair<BufferItemTypes, string> kv in AdvanceBufferPropsMgr.BufferId2ConfigParamsNameDict)
				{
					int bufferId = (int)kv.Key;
					string paramName = kv.Value;
					int[] ids = GameManager.systemParamsList.GetParamValueIntArrayByName(paramName, ',');
					AdvanceBufferPropsMgr.CachingIDsDict[bufferId] = ids;
				}
			}
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0019529C File Offset: 0x0019349C
		public static int GetGoodsID(BufferItemTypes bufferItemType, int goodsIndex)
		{
			int[] goodsIds = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferItemType);
			int result;
			if (null == goodsIds)
			{
				result = -1;
			}
			else if (goodsIndex < 0 || goodsIndex >= goodsIds.Length)
			{
				result = -1;
			}
			else
			{
				int goodsID = goodsIds[goodsIndex];
				result = goodsID;
			}
			return result;
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x001952E4 File Offset: 0x001934E4
		public static double GetExtProp(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsIndex)
		{
			int[] goodsIds = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferItemType);
			double result;
			if (null == goodsIds)
			{
				result = 0.0;
			}
			else if (goodsIndex < 0 || goodsIndex >= goodsIds.Length)
			{
				result = 0.0;
			}
			else
			{
				int goodsID = goodsIds[goodsIndex];
				EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
				if (null == item)
				{
					result = 0.0;
				}
				else
				{
					result = item.ExtProps[(int)extPropIndexe];
				}
			}
			return result;
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0019536C File Offset: 0x0019356C
		public static double GetExtPropByGoodsID(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsID)
		{
			EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
			double result;
			if (null == item)
			{
				result = 0.0;
			}
			else
			{
				result = item.ExtProps[(int)extPropIndexe];
			}
			return result;
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x001953AC File Offset: 0x001935AC
		public static void AddTempBufferProp(GameClient client, BufferItemTypes bufferID, int type)
		{
			EquipPropItem item = null;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, (int)bufferID))
			{
				BufferData bufferData = Global.GetBufferDataByID(client, (int)bufferID);
				if (null != bufferData)
				{
					if (!Global.IsBufferDataOver(bufferData, 0L))
					{
						int bufferGoodsId = 0;
						if (type == 0)
						{
							int goodsIndex;
							if (bufferID == BufferItemTypes.ZuanHuang)
							{
								goodsIndex = client.ClientData.VipLevel;
							}
							else
							{
								goodsIndex = (int)bufferData.BufferVal;
							}
							int[] goodsIds = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferID);
							if (null == goodsIds)
							{
								goto IL_F8;
							}
							if (goodsIndex < 0 || goodsIndex >= goodsIds.Length)
							{
								goto IL_F8;
							}
							bufferGoodsId = goodsIds[goodsIndex];
						}
						else if (type == 1)
						{
							bufferGoodsId = (int)bufferData.BufferVal;
						}
						if (bufferGoodsId > 0)
						{
							item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsId);
						}
					}
				}
			}
			IL_F8:
			if (null != item)
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.BufferByGoodsProps,
					bufferID,
					item.ExtProps
				});
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.BufferByGoodsProps,
					bufferID,
					PropsCacheManager.ConstExtProps
				});
			}
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x00195534 File Offset: 0x00193734
		public static void DoSpriteBuffers(GameClient client)
		{
			int age = client.ClientData.PropsCacheManager.GetAge();
			foreach (KeyValuePair<BufferItemTypes, int> kv in AdvanceBufferPropsMgr.BufferId2ConfigTypeDict)
			{
				if (kv.Value >= 0)
				{
					AdvanceBufferPropsMgr.AddTempBufferProp(client, kv.Key, kv.Value);
				}
			}
			if (age != client.ClientData.PropsCacheManager.GetAge())
			{
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		// Token: 0x0400290F RID: 10511
		private static readonly Dictionary<BufferItemTypes, string> BufferId2ConfigParamsNameDict = new Dictionary<BufferItemTypes, string>
		{
			{
				BufferItemTypes.ChengJiu,
				"ChengJiuBufferGoodsIDs"
			},
			{
				BufferItemTypes.JingMai,
				"JingMaiBufferGoodsIDs"
			},
			{
				BufferItemTypes.WuXue,
				"WuXueBufferGoodsIDs"
			},
			{
				BufferItemTypes.ZuanHuang,
				"ZhuanhuangBufferGoodsIDs"
			},
			{
				BufferItemTypes.ZhanHun,
				"ZhanhunBufferGoodsIDs"
			},
			{
				BufferItemTypes.RongYu,
				"RongyaoBufferGoodsIDs"
			},
			{
				BufferItemTypes.JunQi,
				"JunQiBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_FRESHPLAYERBUFF,
				"FreshPlayerBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF1,
				"AngelTempleGoldBuffGoodsID"
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF2,
				"AngelTempleGoldBuffGoodsID"
			},
			{
				BufferItemTypes.MU_JINGJICHANG_JUNXIAN,
				"JunXianBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_WORLDLEVEL,
				"WorldLevelGoodsIDs"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI,
				"ZhanMengZhanQiBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JITAN,
				"ZhanMengJiTanBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JUNXIE,
				"ZhanMengJunXieBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_GUANGHUAN,
				"ZhanMengGuangHuanBUFF"
			}
		};

		// Token: 0x04002910 RID: 10512
		private static readonly Dictionary<BufferItemTypes, int> BufferId2ConfigTypeDict = new Dictionary<BufferItemTypes, int>
		{
			{
				BufferItemTypes.ChengJiu,
				0
			},
			{
				BufferItemTypes.ZuanHuang,
				0
			},
			{
				BufferItemTypes.ZhanHun,
				0
			},
			{
				BufferItemTypes.RongYu,
				0
			},
			{
				BufferItemTypes.JunQi,
				0
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF1,
				0
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF2,
				0
			},
			{
				BufferItemTypes.MU_JINGJICHANG_JUNXIAN,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JITAN,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JUNXIE,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_GUANGHUAN,
				0
			},
			{
				BufferItemTypes.JieRiChengHao,
				1
			}
		};

		// Token: 0x04002911 RID: 10513
		private static Dictionary<int, int[]> CachingIDsDict = new Dictionary<int, int[]>();
	}
}
