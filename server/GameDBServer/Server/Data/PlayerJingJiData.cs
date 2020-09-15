using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using ProtoBuf;
using Server.Tools;

namespace Server.Data
{
	// Token: 0x0200008C RID: 140
	[ProtoContract]
	public class PlayerJingJiData
	{
		// Token: 0x06000130 RID: 304 RVA: 0x00006E10 File Offset: 0x00005010
		public PlayerJingJiData()
		{
			this.rankingData = new PlayerJingJiRankingData(this);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006E60 File Offset: 0x00005060
		public void convertString()
		{
			this.stringBaseProps = this.convertBasePropsToString(this.baseProps);
			this.stringExtProps = this.convertExtPropsToString(this.extProps);
			this.stringEquipDatas = this.convertEquipDatasToString(this.equipDatas);
			this.stringSkillDatas = this.convertSkillDatasToString(this.skillDatas);
			this.stringWingData = this.convertWingDataToString(this.wingData);
			this.stringShenShiEuipSkill = this.convertSkillEquipDataToString(this.shenShiEquipData);
			this.stringPassiveEffect = this.convertPassiveEffectDataToString(this.PassiveEffectList);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00006EEC File Offset: 0x000050EC
		public void convertObject()
		{
			if (this.version == JingJiChangConstants.Current_Data_Version)
			{
				this.baseProps = this.convertStringToBaseProps(this.stringBaseProps);
				this.extProps = this.convertStringToExtProps(this.stringExtProps);
				this.equipDatas = this.convertStringToEquipDatas(this.stringEquipDatas);
				this.skillDatas = this.convertStringToSkillDatas(this.stringSkillDatas);
				this.wingData = this.convertStringToWingData(this.stringWingData);
				this.shenShiEquipData = this.convertStringToSkillEquipData(this.stringShenShiEuipSkill);
				this.PassiveEffectList = this.convertStringToPassiveEffectData(this.stringPassiveEffect);
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00006F98 File Offset: 0x00005198
		private string convertBasePropsToString(double[] baseProps)
		{
			StringBuilder _baseProps = new StringBuilder();
			for (int i = 0; i < baseProps.Length; i++)
			{
				if (i == baseProps.Length - 1)
				{
					_baseProps.Append(Convert.ToString(baseProps[i]));
				}
				else
				{
					_baseProps.Append(Convert.ToString(baseProps[i]));
					_baseProps.Append(',');
				}
			}
			return (_baseProps.Length != 0) ? _baseProps.ToString() : "";
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00007018 File Offset: 0x00005218
		private string convertExtPropsToString(double[] extProps)
		{
			StringBuilder _extProps = new StringBuilder();
			for (int i = 0; i < extProps.Length; i++)
			{
				if (i == extProps.Length - 1)
				{
					_extProps.Append(Convert.ToString(extProps[i]));
				}
				else
				{
					_extProps.Append(Convert.ToString(extProps[i]));
					_extProps.Append(',');
				}
			}
			return (_extProps.Length != 0) ? _extProps.ToString() : "";
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007098 File Offset: 0x00005298
		private string convertEquipDatasToString(List<PlayerJingJiEquipData> equipDatas)
		{
			string result;
			if (equipDatas == null)
			{
				result = "";
			}
			else
			{
				StringBuilder _equipDatas = new StringBuilder();
				for (int i = 0; i < equipDatas.Count; i++)
				{
					if (i == equipDatas.Count - 1)
					{
						_equipDatas.Append(equipDatas[i].getStringValue());
					}
					else
					{
						_equipDatas.Append(equipDatas[i].getStringValue());
						_equipDatas.Append('|');
					}
				}
				result = ((_equipDatas.Length != 0) ? _equipDatas.ToString() : "");
			}
			return result;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007138 File Offset: 0x00005338
		private string convertSkillDatasToString(List<PlayerJingJiSkillData> skillDatas)
		{
			StringBuilder _skillDatas = new StringBuilder();
			for (int i = 0; i < skillDatas.Count; i++)
			{
				if (i == skillDatas.Count - 1)
				{
					_skillDatas.Append(skillDatas[i].getStringValue());
				}
				else
				{
					_skillDatas.Append(skillDatas[i].getStringValue());
					_skillDatas.Append('|');
				}
			}
			return (_skillDatas.Length != 0) ? _skillDatas.ToString() : "";
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000071C4 File Offset: 0x000053C4
		private string convertWingDataToString(WingData wingData)
		{
			string result;
			if (wingData != null)
			{
				byte[] data = DataHelper.ObjectToBytes<WingData>(wingData);
				result = Convert.ToBase64String(data);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000071F8 File Offset: 0x000053F8
		private string convertSkillEquipDataToString(SkillEquipData shenShiEquipData)
		{
			string result;
			if (shenShiEquipData != null)
			{
				byte[] data = DataHelper.ObjectToBytes<SkillEquipData>(shenShiEquipData);
				result = Convert.ToBase64String(data);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000722C File Offset: 0x0000542C
		private string convertPassiveEffectDataToString(List<int> passiveEffectList)
		{
			string result;
			if (this.shenShiEquipData != null)
			{
				byte[] data = DataHelper.ObjectToBytes<List<int>>(passiveEffectList);
				result = Convert.ToBase64String(data);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007264 File Offset: 0x00005464
		private double[] convertStringToBaseProps(string value)
		{
			string[] _value = value.Split(new char[]
			{
				','
			});
			double[] baseProps = new double[_value.Length];
			for (int i = 0; i < _value.Length; i++)
			{
				baseProps[i] = Convert.ToDouble(_value[i]);
			}
			return baseProps;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000072B8 File Offset: 0x000054B8
		private double[] convertStringToExtProps(string value)
		{
			string[] _value = value.Split(new char[]
			{
				','
			});
			double[] extProps = new double[_value.Length];
			for (int i = 0; i < _value.Length; i++)
			{
				extProps[i] = Convert.ToDouble(_value[i]);
			}
			return extProps;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000730C File Offset: 0x0000550C
		private List<PlayerJingJiEquipData> convertStringToEquipDatas(string value)
		{
			List<PlayerJingJiEquipData> equipDatas = new List<PlayerJingJiEquipData>();
			List<PlayerJingJiEquipData> result;
			if (value == null || value.Equals(""))
			{
				result = equipDatas;
			}
			else
			{
				string[] _value = value.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < _value.Length; i++)
				{
					PlayerJingJiEquipData data = PlayerJingJiEquipData.createPlayerJingJiEquipData(_value[i]);
					if (null != data)
					{
						equipDatas.Add(data);
					}
				}
				result = equipDatas;
			}
			return result;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00007390 File Offset: 0x00005590
		private List<PlayerJingJiSkillData> convertStringToSkillDatas(string value)
		{
			List<PlayerJingJiSkillData> skillDatas = new List<PlayerJingJiSkillData>();
			List<PlayerJingJiSkillData> result;
			if (value == null || value.Equals(""))
			{
				result = skillDatas;
			}
			else
			{
				string[] _value = value.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < _value.Length; i++)
				{
					PlayerJingJiSkillData data = PlayerJingJiSkillData.createPlayerJingJiSkillData(_value[i]);
					if (null != data)
					{
						skillDatas.Add(data);
					}
				}
				result = skillDatas;
			}
			return result;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00007414 File Offset: 0x00005614
		private WingData convertStringToWingData(string value)
		{
			WingData result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] data = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<WingData>(data, 0, data.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007448 File Offset: 0x00005648
		private SkillEquipData convertStringToSkillEquipData(string value)
		{
			SkillEquipData result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] data = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<SkillEquipData>(data, 0, data.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000747C File Offset: 0x0000567C
		private List<int> convertStringToPassiveEffectData(string value)
		{
			List<int> result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] data = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<List<int>>(data, 0, data.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000074B0 File Offset: 0x000056B0
		public PlayerJingJiMiniData getPlayerJingJiMiniData()
		{
			this.miniData.roleId = this.roleId;
			this.miniData.roleName = this.roleName;
			this.miniData.ranking = this.ranking;
			this.miniData.occupationId = this.occupationId;
			this.miniData.combatForce = this.combatForce;
			return this.miniData;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007520 File Offset: 0x00005720
		public PlayerJingJiRankingData getPlayerJingJiRankingData()
		{
			this.rankingData.ranking = this.ranking;
			return this.rankingData;
		}

		// Token: 0x04000304 RID: 772
		public bool isOnline = false;

		// Token: 0x04000305 RID: 773
		[DBMapping(ColumnName = "roleId")]
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x04000306 RID: 774
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(2)]
		public string roleName;

		// Token: 0x04000307 RID: 775
		[ProtoMember(3)]
		[DBMapping(ColumnName = "level")]
		public int level;

		// Token: 0x04000308 RID: 776
		[DBMapping(ColumnName = "changeLiveCount")]
		[ProtoMember(4)]
		public int changeLiveCount;

		// Token: 0x04000309 RID: 777
		[DBMapping(ColumnName = "occupationId")]
		[ProtoMember(5)]
		public int occupationId;

		// Token: 0x0400030A RID: 778
		[ProtoMember(6)]
		[DBMapping(ColumnName = "winCount")]
		public int winCount = 0;

		// Token: 0x0400030B RID: 779
		[ProtoMember(7)]
		[DBMapping(ColumnName = "ranking")]
		public int ranking = -1;

		// Token: 0x0400030C RID: 780
		[DBMapping(ColumnName = "nextRewardTime")]
		[ProtoMember(8)]
		public long nextRewardTime;

		// Token: 0x0400030D RID: 781
		[DBMapping(ColumnName = "nextChallengeTime")]
		[ProtoMember(9)]
		public long nextChallengeTime;

		// Token: 0x0400030E RID: 782
		[ProtoMember(10)]
		public double[] baseProps;

		// Token: 0x0400030F RID: 783
		[DBMapping(ColumnName = "baseProps")]
		public string stringBaseProps;

		// Token: 0x04000310 RID: 784
		[ProtoMember(11)]
		public double[] extProps;

		// Token: 0x04000311 RID: 785
		[DBMapping(ColumnName = "extProps")]
		public string stringExtProps;

		// Token: 0x04000312 RID: 786
		[ProtoMember(12)]
		public List<PlayerJingJiEquipData> equipDatas;

		// Token: 0x04000313 RID: 787
		[DBMapping(ColumnName = "equipDatas")]
		public string stringEquipDatas;

		// Token: 0x04000314 RID: 788
		[ProtoMember(13)]
		public List<PlayerJingJiSkillData> skillDatas;

		// Token: 0x04000315 RID: 789
		[DBMapping(ColumnName = "skillDatas")]
		public string stringSkillDatas;

		// Token: 0x04000316 RID: 790
		[DBMapping(ColumnName = "CombatForce")]
		[ProtoMember(14)]
		public int combatForce = 0;

		// Token: 0x04000317 RID: 791
		[DBMapping(ColumnName = "version")]
		public int version;

		// Token: 0x04000318 RID: 792
		[DBMapping(ColumnName = "sex")]
		[ProtoMember(15)]
		public int sex;

		// Token: 0x04000319 RID: 793
		[DBMapping(ColumnName = "name")]
		[ProtoMember(16)]
		public string name;

		// Token: 0x0400031A RID: 794
		[DBMapping(ColumnName = "zoneId")]
		[ProtoMember(17)]
		public int zoneId;

		// Token: 0x0400031B RID: 795
		[DBMapping(ColumnName = "maxwincnt")]
		[ProtoMember(18)]
		public int MaxWinCnt = 0;

		// Token: 0x0400031C RID: 796
		[DBMapping(ColumnName = "wingData")]
		public string stringWingData;

		// Token: 0x0400031D RID: 797
		[ProtoMember(19)]
		public WingData wingData;

		// Token: 0x0400031E RID: 798
		[DBMapping(ColumnName = "settingFlags")]
		[ProtoMember(20)]
		public long settingFlags;

		// Token: 0x0400031F RID: 799
		[ProtoMember(21)]
		public int AdmiredCount;

		// Token: 0x04000320 RID: 800
		[ProtoMember(28)]
		public SkillEquipData shenShiEquipData;

		// Token: 0x04000321 RID: 801
		[DBMapping(ColumnName = "shenshiequip")]
		public string stringShenShiEuipSkill;

		// Token: 0x04000322 RID: 802
		[ProtoMember(29)]
		public List<int> PassiveEffectList;

		// Token: 0x04000323 RID: 803
		[DBMapping(ColumnName = "passiveEffect")]
		public string stringPassiveEffect;

		// Token: 0x04000324 RID: 804
		[ProtoMember(32)]
		[DBMapping(ColumnName = "suboccupation")]
		public int SubOccupation;

		// Token: 0x04000325 RID: 805
		private PlayerJingJiMiniData miniData = new PlayerJingJiMiniData();

		// Token: 0x04000326 RID: 806
		private PlayerJingJiRankingData rankingData;
	}
}
