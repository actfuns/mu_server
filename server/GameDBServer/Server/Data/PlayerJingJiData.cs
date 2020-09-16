using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using ProtoBuf;
using Server.Tools;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiData
	{
		
		public PlayerJingJiData()
		{
			this.rankingData = new PlayerJingJiRankingData(this);
		}

		
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

		
		public PlayerJingJiMiniData getPlayerJingJiMiniData()
		{
			this.miniData.roleId = this.roleId;
			this.miniData.roleName = this.roleName;
			this.miniData.ranking = this.ranking;
			this.miniData.occupationId = this.occupationId;
			this.miniData.combatForce = this.combatForce;
			return this.miniData;
		}

		
		public PlayerJingJiRankingData getPlayerJingJiRankingData()
		{
			this.rankingData.ranking = this.ranking;
			return this.rankingData;
		}

		
		public bool isOnline = false;

		
		[DBMapping(ColumnName = "roleId")]
		[ProtoMember(1)]
		public int roleId;

		
		[DBMapping(ColumnName = "roleName")]
		[ProtoMember(2)]
		public string roleName;

		
		[ProtoMember(3)]
		[DBMapping(ColumnName = "level")]
		public int level;

		
		[DBMapping(ColumnName = "changeLiveCount")]
		[ProtoMember(4)]
		public int changeLiveCount;

		
		[DBMapping(ColumnName = "occupationId")]
		[ProtoMember(5)]
		public int occupationId;

		
		[ProtoMember(6)]
		[DBMapping(ColumnName = "winCount")]
		public int winCount = 0;

		
		[ProtoMember(7)]
		[DBMapping(ColumnName = "ranking")]
		public int ranking = -1;

		
		[DBMapping(ColumnName = "nextRewardTime")]
		[ProtoMember(8)]
		public long nextRewardTime;

		
		[DBMapping(ColumnName = "nextChallengeTime")]
		[ProtoMember(9)]
		public long nextChallengeTime;

		
		[ProtoMember(10)]
		public double[] baseProps;

		
		[DBMapping(ColumnName = "baseProps")]
		public string stringBaseProps;

		
		[ProtoMember(11)]
		public double[] extProps;

		
		[DBMapping(ColumnName = "extProps")]
		public string stringExtProps;

		
		[ProtoMember(12)]
		public List<PlayerJingJiEquipData> equipDatas;

		
		[DBMapping(ColumnName = "equipDatas")]
		public string stringEquipDatas;

		
		[ProtoMember(13)]
		public List<PlayerJingJiSkillData> skillDatas;

		
		[DBMapping(ColumnName = "skillDatas")]
		public string stringSkillDatas;

		
		[DBMapping(ColumnName = "CombatForce")]
		[ProtoMember(14)]
		public int combatForce = 0;

		
		[DBMapping(ColumnName = "version")]
		public int version;

		
		[DBMapping(ColumnName = "sex")]
		[ProtoMember(15)]
		public int sex;

		
		[DBMapping(ColumnName = "name")]
		[ProtoMember(16)]
		public string name;

		
		[DBMapping(ColumnName = "zoneId")]
		[ProtoMember(17)]
		public int zoneId;

		
		[DBMapping(ColumnName = "maxwincnt")]
		[ProtoMember(18)]
		public int MaxWinCnt = 0;

		
		[DBMapping(ColumnName = "wingData")]
		public string stringWingData;

		
		[ProtoMember(19)]
		public WingData wingData;

		
		[DBMapping(ColumnName = "settingFlags")]
		[ProtoMember(20)]
		public long settingFlags;

		
		[ProtoMember(21)]
		public int AdmiredCount;

		
		[ProtoMember(28)]
		public SkillEquipData shenShiEquipData;

		
		[DBMapping(ColumnName = "shenshiequip")]
		public string stringShenShiEuipSkill;

		
		[ProtoMember(29)]
		public List<int> PassiveEffectList;

		
		[DBMapping(ColumnName = "passiveEffect")]
		public string stringPassiveEffect;

		
		[ProtoMember(32)]
		[DBMapping(ColumnName = "suboccupation")]
		public int SubOccupation;

		
		private PlayerJingJiMiniData miniData = new PlayerJingJiMiniData();

		
		private PlayerJingJiRankingData rankingData;
	}
}
