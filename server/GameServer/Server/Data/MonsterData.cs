using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MonsterData
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int RoleSex = 0;

		
		[ProtoMember(4)]
		public int Level = 1;

		
		[ProtoMember(5)]
		public int Experience = 0;

		
		[ProtoMember(6)]
		public int PosX = 0;

		
		[ProtoMember(7)]
		public int PosY = 0;

		
		[ProtoMember(8)]
		public int RoleDirection = 0;

		
		[ProtoMember(9)]
		public double LifeV = 0.0;

		
		[ProtoMember(10)]
		public double MaxLifeV = 0.0;

		
		[ProtoMember(11)]
		public double MagicV = 0.0;

		
		[ProtoMember(12)]
		public double MaxMagicV = 0.0;

		
		[ProtoMember(13)]
		public int EquipmentBody = 0;

		
		[ProtoMember(14)]
		public int ExtensionID = 0;

		
		[ProtoMember(15)]
		public int MonsterType = 0;

		
		[ProtoMember(16)]
		public int MasterRoleID = 0;

		
		[ProtoMember(17)]
		public ushort AiControlType = 1;

		
		[ProtoMember(18)]
		public string AnimalSound = "";

		
		[ProtoMember(19)]
		public int MonsterLevel = 0;

		
		[ProtoMember(20)]
		public long ZhongDuStart = 0L;

		
		[ProtoMember(21)]
		public int ZhongDuSeconds = 0;

		
		[ProtoMember(22)]
		public long FaintStart = 0L;

		
		[ProtoMember(23)]
		public int FaintSeconds = 0;

		
		[ProtoMember(24)]
		public int BattleWitchSide;

		
		[ProtoMember(25)]
		public long BirthTicks;
	}
}
