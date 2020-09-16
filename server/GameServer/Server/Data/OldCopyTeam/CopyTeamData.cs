using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	
	[ProtoContract]
	public class CopyTeamData
	{
		
		public CopyTeamData SimpleClone()
		{
			return new CopyTeamData
			{
				TeamID = this.TeamID,
				LeaderRoleID = this.LeaderRoleID,
				StartTime = this.StartTime,
				GetThingOpt = this.GetThingOpt,
				SceneIndex = this.SceneIndex,
				FuBenSeqID = this.FuBenSeqID,
				MinZhanLi = this.MinZhanLi,
				AutoStart = this.AutoStart,
				TeamRoles = null,
				MemberCount = this.MemberCount,
				TeamName = this.TeamName
			};
		}

		
		[ProtoMember(1)]
		public int TeamID = 0;

		
		[ProtoMember(2)]
		public int LeaderRoleID = 0;

		
		[ProtoMember(3)]
		public List<CopyTeamMemberData> TeamRoles;

		
		[ProtoMember(4)]
		public long StartTime = 0L;

		
		[ProtoMember(5)]
		public int GetThingOpt = 0;

		
		[ProtoMember(6)]
		public int SceneIndex = 0;

		
		[ProtoMember(7)]
		public int FuBenSeqID = 0;

		
		[ProtoMember(8)]
		public int MinZhanLi = 0;

		
		[ProtoMember(9)]
		public bool AutoStart = false;

		
		[ProtoMember(10)]
		public int MemberCount = 0;

		
		[ProtoMember(11)]
		public string TeamName = null;
	}
}
