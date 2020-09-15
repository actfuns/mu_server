using System;
using System.Collections.Generic;
using System.Threading;

namespace KF.Hosting.HuanYingSiYuan
{
	// Token: 0x02000005 RID: 5
	public class CmdHandlerDict
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002E38 File Offset: 0x00001038
		public void AddCmdHelp(string msg, params string[] cmds)
		{
			lock (this.CmdDict)
			{
				this.HelpDict.Add(new KeyValuePair<string[], string>(cmds, msg));
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002E90 File Offset: 0x00001090
		public void ShowHelp(params string[] cmds)
		{
			lock (this.CmdDict)
			{
				foreach (KeyValuePair<string[], string> item in this.HelpDict)
				{
					bool match = true;
					int i = 0;
					while (i < cmds.Length && i < item.Key.Length)
					{
						if (string.Compare(item.Key[i], cmds[i], true) != 0)
						{
							match = false;
						}
						i++;
					}
					if (match)
					{
						Console.WriteLine("{0} {1}", string.Join(" ", item.Key), item.Value);
					}
				}
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002F9C File Offset: 0x0000119C
		public void AddCmdHandler(string cmd, ParameterizedThreadStart handler)
		{
			this.CmdDict.Add(cmd, handler);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002FB0 File Offset: 0x000011B0
		public string[] ParseConsoleCmd(string cmd)
		{
			List<string> argsList = new List<string>();
			string arg = "";
			Stack<char> quoteStack = new Stack<char>();
			int i = 0;
			while (i < cmd.Length)
			{
				char c = cmd[i];
				if (char.IsWhiteSpace(c))
				{
					if (quoteStack.Count != 0)
					{
						goto IL_108;
					}
					if (arg != "")
					{
						argsList.Add(arg);
						arg = "";
					}
				}
				else
				{
					if (c == '"')
					{
						if (quoteStack.Count > 0 && quoteStack.Peek() == '"')
						{
							quoteStack.Pop();
						}
						else
						{
							quoteStack.Push(c);
						}
						goto IL_108;
					}
					if (c == '\'')
					{
						if (quoteStack.Count > 0 && quoteStack.Peek() == '\'')
						{
							quoteStack.Pop();
						}
						else
						{
							quoteStack.Push(c);
						}
						goto IL_108;
					}
					goto IL_108;
				}
				IL_116:
				i++;
				continue;
				IL_108:
				arg += c;
				goto IL_116;
			}
			if (arg != "")
			{
				argsList.Add(arg);
			}
			return argsList.ToArray();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003118 File Offset: 0x00001318
		public void ExcuteCmd(string cmd)
		{
			if (!string.IsNullOrEmpty(cmd))
			{
				string[] args = this.ParseConsoleCmd(cmd);
				if (args != null && args.Length != 0)
				{
					ParameterizedThreadStart proc;
					if (this.CmdDict.TryGetValue(args[0].ToLower(), out proc))
					{
						proc(args);
					}
				}
			}
		}

		// Token: 0x04000016 RID: 22
		private Dictionary<string, ParameterizedThreadStart> CmdDict = new Dictionary<string, ParameterizedThreadStart>();

		// Token: 0x04000017 RID: 23
		private List<KeyValuePair<string[], string>> HelpDict = new List<KeyValuePair<string[], string>>();
	}
}
