using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class DeControl : IManager
	{
		
		public static DeControl getInstance()
		{
			return DeControl.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
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

		
		private void OnReload()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				this.OnInitGame(client);
			}
		}

		
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

		
		private DeControlRuntimeData RuntimeData = new DeControlRuntimeData();

		
		private static DeControl instance = new DeControl();
	}
}
