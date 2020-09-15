using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020001DD RID: 477
	public class DeControl : IManager
	{
		// Token: 0x06000603 RID: 1539 RVA: 0x00054BA0 File Offset: 0x00052DA0
		public static DeControl getInstance()
		{
			return DeControl.instance;
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00054BB8 File Offset: 0x00052DB8
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00054BDC File Offset: 0x00052DDC
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00054BF0 File Offset: 0x00052DF0
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00054C04 File Offset: 0x00052E04
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00054C18 File Offset: 0x00052E18
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					for (int i = 0; i < this.RuntimeData.DeControlItemListArray.Length; i++)
					{
						List<DeControlItem> list = this.RuntimeData.DeControlItemListArray[i];
						if (null != list)
						{
							foreach (DeControlItem item in list)
							{
								item.Next = null;
								item.Head = null;
							}
							this.RuntimeData.DeControlItemListArray[i] = null;
						}
					}
					this.RuntimeData.IsGongNengOpend = false;
					int platformId = (int)GameCoreInterface.getinstance().GetPlatformType();
					List<string> emblemOpenStrs = GameManager.systemParamsList.GetParamValueStringListByName("DeControlOpen", '|');
					foreach (string str in emblemOpenStrs)
					{
						List<int> args = Global.StringToIntList(str, ',');
						if (args != null && args[0] == platformId && args[1] > 0)
						{
							this.RuntimeData.IsGongNengOpend = true;
						}
					}
					this.RuntimeData.IsGongNengOpend &= !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot5);
					if (this.RuntimeData.IsGongNengOpend)
					{
						fileName = "Config\\DeControl.xml";
						string fullPathFileName = Global.GameResPath(fileName);
						XElement xml = XElement.Load(fullPathFileName);
						IEnumerable<XElement> nodes = xml.Elements();
						foreach (XElement node in nodes)
						{
							int extPropID = (int)Global.GetSafeAttributeLong(node, "ExtPropID");
							int count = (int)Global.GetSafeAttributeLong(node, "MaxFlood");
							double[] deControlPercent = Global.GetSafeAttributeDoubleArray(node, "DeControlPercent", -1, ',');
							double[] deControlTime = Global.GetSafeAttributeDoubleArray(node, "DeControlTime", -1, ',');
							double[] durationTime = Global.GetSafeAttributeDoubleArray(node, "DurationTime", -1, ',');
							if (deControlPercent.Length < count || deControlTime.Length < count || durationTime.Length < count)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("解析文件{0}的BaoMingTime出错", fileName), null, true);
							}
							List<DeControlItem> list = this.RuntimeData.DeControlItemListArray[extPropID];
							if (list == null)
							{
								list = (this.RuntimeData.DeControlItemListArray[extPropID] = new List<DeControlItem>());
							}
							DeControlItem headItem = null;
							DeControlItem pre = null;
							for (int i = 0; i < count; i++)
							{
								DeControlItem item = new DeControlItem();
								item.ExtPropIndex = extPropID;
								item.DeControlPercent = deControlPercent[i];
								item.DeControlTime = deControlTime[i];
								item.DurationTime = durationTime[i];
								if (pre == null)
								{
									headItem = item;
								}
								else
								{
									pre.Next = item;
								}
								item.Head = headItem;
								pre = item;
								list.Add(item);
							}
						}
					}
					this.OnReload();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00055038 File Offset: 0x00053238
		private void OnReload()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				this.OnInitGame(client);
			}
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00055098 File Offset: 0x00053298
		public void OnInitGame(GameClient client)
		{
			for (int i = 0; i < client.ClientData.DeControlItemArray.Length; i++)
			{
				client.ClientData.DeControlItemArray[i] = null;
			}
			foreach (List<DeControlItem> list in this.RuntimeData.DeControlItemListArray)
			{
				if (null != list)
				{
					int idx = list[0].ExtPropIndex;
					client.ClientData.DeControlItemArray[idx] = new DeControlItemData
					{
						Item = list[0]
					};
				}
			}
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00055140 File Offset: 0x00053340
		public double OnControl(GameClient client, int propIndex)
		{
			double result;
			if (null == client)
			{
				result = 1.0;
			}
			else
			{
				DeControlItemData data = client.ClientData.DeControlItemArray[propIndex];
				if (null == data)
				{
					result = 1.0;
				}
				else
				{
					DeControlItem item = data.Item;
					if (null == item)
					{
						client.ClientData.DeControlItemArray[propIndex] = null;
						result = 1.0;
					}
					else
					{
						long nowTicks = TimeUtil.NOW();
						if (nowTicks > data.EndTicks)
						{
							DeControlItem next = item.Head;
							if (null != next)
							{
								data.Item = next;
								data.EndTicks = nowTicks + (long)(next.DurationTime * 1000.0);
								LogManager.WriteLog(LogTypes.Info, string.Format("控制效果递减#{0}#触发", propIndex), null, true);
							}
							result = 1.0;
						}
						else
						{
							double rnd = Global.GetRandom();
							if (rnd < item.DeControlPercent)
							{
								LogManager.WriteLog(LogTypes.Info, string.Format("控制效果#{0}#未触发,rnd={1},percent={2}", propIndex, rnd, item.DeControlPercent), null, true);
								result = 0.0;
							}
							else
							{
								DeControlItem next = item.Next;
								if (null != next)
								{
									data.Item = next;
									data.EndTicks = nowTicks + (long)(next.DurationTime * 1000.0);
								}
								LogManager.WriteLog(LogTypes.Info, string.Format("控制效果#{0}#触发,rnd={1},percent={2},endtime={3},DeControlTime={4}", new object[]
								{
									propIndex,
									rnd,
									item.DeControlPercent,
									TimeUtil.NowDateTime().AddSeconds(next.DurationTime),
									item.DeControlTime
								}), null, true);
								result = item.DeControlTime;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04000A81 RID: 2689
		private DeControlRuntimeData RuntimeData = new DeControlRuntimeData();

		// Token: 0x04000A82 RID: 2690
		private static DeControl instance = new DeControl();
	}
}
