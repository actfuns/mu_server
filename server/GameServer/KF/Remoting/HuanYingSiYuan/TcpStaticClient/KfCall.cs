using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AutoCSer;
using AutoCSer.BinarySerialize;
using AutoCSer.IOS;
using AutoCSer.Json;
using AutoCSer.Log;
using AutoCSer.Metadata;
using AutoCSer.Net;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Contract;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.HuanYingSiYuan.TcpStaticClient
{
	
	public class KfCall
	{
		
		public static AutoCSer.Net.TcpStaticServer.Client NewTcpClient(AutoCSer.Net.TcpInternalServer.ServerAttribute attribute, Action<SubArray<byte>> onCustomData, ILog log, Func<AutoCSer.Net.TcpInternalServer.ClientSocketSender, bool> verifyMethod)
		{
			KfCall.TcpClient = new AutoCSer.Net.TcpStaticServer.Client(attribute, onCustomData, log, verifyMethod);
			CommandBase tcpClient = KfCall.TcpClient;
			Type[] array = new Type[7];
			array[0] = typeof(KfCall._p12);
			array[1] = typeof(KfCall._p15);
			array[2] = typeof(KfCall._p18);
			array[3] = typeof(KfCall._p27);
			array[4] = typeof(KfCall._p29);
			array[5] = typeof(KfCall._p30);
			Type[] simpleSerializeTypes = array;
			array = new Type[4];
			array[0] = typeof(KfCall._p6);
			array[1] = typeof(KfCall._p10);
			array[2] = typeof(KfCall._p31);
			Type[] simpleDeSerializeTypes = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p1);
			array[1] = typeof(KfCall._p3);
			array[2] = typeof(KfCall._p5);
			array[3] = typeof(KfCall._p7);
			array[4] = typeof(KfCall._p9);
			array[5] = typeof(KfCall._p11);
			array[6] = typeof(KfCall._p19);
			array[7] = typeof(KfCall._p20);
			array[8] = typeof(KfCall._p22);
			array[9] = typeof(KfCall._p23);
			array[10] = typeof(KfCall._p25);
			Type[] serializeTypes = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p2);
			array[1] = typeof(KfCall._p4);
			array[2] = typeof(KfCall._p8);
			array[3] = typeof(KfCall._p13);
			array[4] = typeof(KfCall._p14);
			array[5] = typeof(KfCall._p16);
			array[6] = typeof(KfCall._p17);
			array[7] = typeof(KfCall._p21);
			array[8] = typeof(KfCall._p24);
			array[9] = typeof(KfCall._p26);
			array[10] = typeof(KfCall._p28);
			Type[] deSerializeTypes = array;
			array = new Type[1];
			Type[] jsonSerializeTypes = array;
			array = new Type[1];
			tcpClient.ClientCompileSerialize(simpleSerializeTypes, simpleDeSerializeTypes, serializeTypes, deSerializeTypes, jsonSerializeTypes, array);
			return KfCall.TcpClient;
		}

		
		public static AutoCSer.Net.TcpStaticServer.Client TcpClient;

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p1
		{
			
			public EscapeBattleSyncData p0;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p2 : IReturnParameter<EscapeBattleSyncData>
		{
			
			
			
			[Preserve(Conditional = true)]
			public EscapeBattleSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public EscapeBattleSyncData Ret;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p3
		{
			
			public EscapeBattleFuBenData p0;

			
			public int p1;

			
			public int p2;

			
			public int p3;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p4 : IReturnParameter<int>
		{
			
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			public EscapeBattleFuBenData p0;

			
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p5
		{
			
			public List<int> p0;

			
			public int p1;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p6 : IReturnParameter<int>
		{
			
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p7
		{
			
			public int p0;

			
			public int p1;

			
			public int p2;

			
			public int[] p3;

			
			public string[] p4;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p8 : IReturnParameter<int>
		{
			
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			public int p0;

			
			public int p1;

			
			public int[] p2;

			
			public string[] p3;

			
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p9
		{
			
			public KFBoCaiShopDB p0;

			
			public int p1;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p10 : IReturnParameter<bool>
		{
			
			
			
			[Preserve(Conditional = true)]
			public bool Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public bool Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p11
		{
			
			public KFBuyBocaiData p0;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p12
		{
			
			public int p0;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p13 : IReturnParameter<KFStageData>
		{
			
			
			
			[Preserve(Conditional = true)]
			public KFStageData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public KFStageData Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p14 : IReturnParameter<OpenLottery>
		{
			
			
			
			[Preserve(Conditional = true)]
			public OpenLottery Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public OpenLottery Ret;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p15
		{
			
			public bool p0;

			
			public int p1;

			
			public long p2;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p16 : IReturnParameter<List<OpenLottery>>
		{
			
			
			
			[Preserve(Conditional = true)]
			public List<OpenLottery> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public List<OpenLottery> Ret;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p17 : IReturnParameter<List<KFBoCaoHistoryData>>
		{
			
			
			
			[Preserve(Conditional = true)]
			public List<KFBoCaoHistoryData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public List<KFBoCaoHistoryData> Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p18
		{
			
			public int p0;

			
			public int p1;

			
			public long p2;

			
			public string p3;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p19
		{
			
			public string[] p0;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p20
		{
			
			public KuaFuClientContext p0;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p21 : IReturnParameter<KFCallMsg>
		{
			
			
			
			[Preserve(Conditional = true)]
			public KFCallMsg Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public KFCallMsg Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p22
		{
			
			public Dictionary<int, int> p0;

			
			public int p1;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p23
		{
			
			public ZhanDuiZhengBaSyncData p0;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p24 : IReturnParameter<ZhanDuiZhengBaSyncData>
		{
			
			
			
			[Preserve(Conditional = true)]
			public ZhanDuiZhengBaSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public ZhanDuiZhengBaSyncData Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p25
		{
			
			public ZhanDuiZhengBaFuBenData p0;

			
			public int p1;

			
			public int p2;

			
			public int p3;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p26 : IReturnParameter<int>
		{
			
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			public ZhanDuiZhengBaFuBenData p0;

			
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p27
		{
			
			public int p0;

			
			public int p1;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p28 : IReturnParameter<List<ZhanDuiZhengBaNtfPkResultData>>
		{
			
			
			
			[Preserve(Conditional = true)]
			public List<ZhanDuiZhengBaNtfPkResultData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public List<ZhanDuiZhengBaNtfPkResultData> Ret;
		}

		
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p29
		{
			
			public int p0;

			
			public int p1;

			
			public int p2;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p30
		{
			
			public bool p0;

			
			public int p1;
		}

		
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p31 : IReturnParameter<string>
		{
			
			
			
			[Preserve(Conditional = true)]
			public string Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			
			[AutoCSer.Json.IgnoreMember]
			public string Ret;
		}

		
		public sealed class ClientConfig
		{
			
			public AutoCSer.Net.TcpInternalServer.ServerAttribute ServerAttribute;

			
			public Action<SubArray<byte>> OnCustomData;

			
			public ILog Log;

			
			public Func<AutoCSer.Net.TcpInternalServer.ClientSocketSender, bool> VerifyMethod;
		}
	}
}
