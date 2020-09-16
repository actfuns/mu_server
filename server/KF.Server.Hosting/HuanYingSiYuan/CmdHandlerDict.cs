using System;
using System.Collections.Generic;
using System.Threading;

namespace KF.Hosting.HuanYingSiYuan
{
	
	public class CmdHandlerDict
	{
		
		public void AddCmdHelp(string msg, params string[] cmds)
		{
			lock (this.CmdDict)
			{
				this.HelpDict.Add(new KeyValuePair<string[], string>(cmds, msg));
			}
		}

		
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

		
		public void AddCmdHandler(string cmd, ParameterizedThreadStart handler)
		{
			this.CmdDict.Add(cmd, handler);
		}

		
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

		
		private Dictionary<string, ParameterizedThreadStart> CmdDict = new Dictionary<string, ParameterizedThreadStart>();

		
		private List<KeyValuePair<string[], string>> HelpDict = new List<KeyValuePair<string[], string>>();
	}
}
