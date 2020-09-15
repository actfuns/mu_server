using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200024A RID: 586
	internal class RobotTaskSender
	{
		// Token: 0x06000814 RID: 2068 RVA: 0x0007AAB6 File Offset: 0x00078CB6
		private RobotTaskSender()
		{
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0007AACC File Offset: 0x00078CCC
		public static RobotTaskSender getInstance()
		{
			return RobotTaskSender.instance;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0007AAE4 File Offset: 0x00078CE4
		public bool Initialize(int seed, int randomCount, string pubKey)
		{
			lock (this)
			{
				try
				{
					this.m_TaskListVerifySeed = seed;
					this.m_TaskListVerifyRandomCount = randomCount;
					this.m_TaskListRSA = new RSACryptoServiceProvider();
					this.m_TaskListRSA.PersistKeyInCsp = false;
					this.m_TaskListRSA.FromXmlString(pubKey);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0007AB78 File Offset: 0x00078D78
		public byte[] EncryptTaskList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> encryptedData = new List<byte>();
			lock (this)
			{
				try
				{
					int tick = Environment.TickCount;
					int randomCount = tick % this.m_TaskListVerifyRandomCount;
					int randomValue = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(randomCount));
					string output = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tick,
						randomValue,
						jailbreak ? 1 : 0,
						autoStart ? 1 : 0,
						info
					});
					List<byte> outputCompress = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(output)));
					int count = (outputCompress == null) ? output.Length : outputCompress.Count;
					for (int i = 0; i < count; i += 100)
					{
						byte[] section;
						if (outputCompress == null)
						{
							section = new UTF8Encoding().GetBytes(output.Substring(i, (i + 100 <= output.Length) ? 100 : (output.Length % 100)));
						}
						else
						{
							section = new byte[(i + 100 <= outputCompress.Count) ? 100 : (outputCompress.Count % 100)];
							outputCompress.CopyTo(i, section, 0, section.Length);
						}
						byte[] encryptedSection = this.m_TaskListRSA.Encrypt(section, false);
						int len = encryptedSection.Length;
						encryptedData.Add((byte)len);
						for (int j = 0; j < len; j++)
						{
							encryptedData.Add(encryptedSection[j]);
						}
					}
				}
				catch (Exception e)
				{
					return null;
				}
			}
			return encryptedData.ToArray();
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0007AD8C File Offset: 0x00078F8C
		public byte[] EncryptGeniusList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> encryptedData = new List<byte>();
			lock (this)
			{
				try
				{
					int tick = Environment.TickCount;
					int randomCount = tick % this.m_TaskListVerifyRandomCount;
					int randomValue = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(randomCount));
					string output = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tick,
						randomValue,
						jailbreak ? 1 : 0,
						autoStart ? 1 : 0,
						info
					});
					List<byte> outputCompress = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(output)));
					int count = outputCompress.Count;
					int c = 7;
					this.AddMagicBytes(encryptedData, c);
					for (int i = 0; i < count; i += 100)
					{
						byte[] section = new byte[(i + 100 <= outputCompress.Count) ? 100 : (outputCompress.Count % 100)];
						outputCompress.CopyTo(i, section, 0, section.Length);
						byte[] encryptedSection = this.m_TaskListRSA.Encrypt(section, false);
						int len = encryptedSection.Length;
						encryptedData.Add((byte)len);
						encryptedData.AddRange(encryptedSection);
						c = ((int)section[0] + c & 15);
					}
					this.AddMagicBytes(encryptedData, c);
				}
				catch (Exception e)
				{
					return null;
				}
			}
			return encryptedData.ToArray();
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0007AF4C File Offset: 0x0007914C
		private void AddMagicBytes(List<byte> data, int c)
		{
			Random random = new Random(this.m_TaskListVerifySeed);
			int randomValue = random.Next();
			for (int i = 0; i < c; i++)
			{
				randomValue = random.Next();
				data.AddRange(BitConverter.GetBytes(randomValue));
			}
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0007AF94 File Offset: 0x00079194
		private uint GenMagicRandom(uint seed, int loop)
		{
			uint w = seed;
			uint z = 362436069U;
			for (int i = 0; i <= loop; i++)
			{
				z = 36969U * (z & 65535U) + (z >> 16);
				w = 18000U * (w & 65535U) + (w >> 16);
			}
			return (z << 16) + w;
		}

		// Token: 0x04000DED RID: 3565
		private static RobotTaskSender instance = new RobotTaskSender();

		// Token: 0x04000DEE RID: 3566
		private int m_TaskListVerifySeed;

		// Token: 0x04000DEF RID: 3567
		private int m_TaskListVerifyRandomCount = 50;

		// Token: 0x04000DF0 RID: 3568
		private RSACryptoServiceProvider m_TaskListRSA;
	}
}
