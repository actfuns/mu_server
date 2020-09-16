using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMatchMainInfo
	{
		
		[ProtoMember(1)]
		public int round = 0;

		
		[ProtoMember(2)]
		public List<BangHuiMatchPKInfo> LastRoundPKInfo = new List<BangHuiMatchPKInfo>();

		
		[ProtoMember(3)]
		public List<BangHuiMatchPKInfo> CurRoundPKInfo = new List<BangHuiMatchPKInfo>();

		
		[ProtoMember(4)]
		public int timestate;

		
		[ProtoMember(5)]
		public int seasonid;
	}
}
