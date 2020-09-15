using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using Microsoft.CSharp.RuntimeBinder;
using Neo.IronLua;

namespace GameServer.Logic
{
    // Token: 0x0200051A RID: 1306
    public class LuaExeManager
    {
        // Token: 0x06001876 RID: 6262 RVA: 0x0017EB58 File Offset: 0x0017CD58
        public static LuaExeManager getInstance()
        {
            return LuaExeManager.instance;
        }

        // Token: 0x06001877 RID: 6263 RVA: 0x0017EB70 File Offset: 0x0017CD70
        public void InitLuaEnv()
        {
            this.gEnv = this.lua.CreateEnvironment();
            LuaExeManager.timerCheckDict = new Timer(100000.0);
            LuaExeManager.timerCheckDict.Elapsed += this.CheckDictLuaInfo;
            LuaExeManager.timerCheckDict.Interval = 100000.0;
            LuaExeManager.timerCheckDict.Enabled = true;
        }

        // Token: 0x06001878 RID: 6264 RVA: 0x0017EC08 File Offset: 0x0017CE08
        private void CheckDictLuaInfo(object source, ElapsedEventArgs e)
        {
            lock (this.dictLuaCache)
            {
                using (Dictionary<string, LuaExeInfo>.Enumerator enumerator = this.dictLuaCache.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, LuaExeInfo> kvLuaInfo = enumerator.Current;
                        KeyValuePair<string, LuaExeInfo> kvLuaInfo3 = kvLuaInfo;
                        DateTime dateNew = File.GetLastWriteTime(kvLuaInfo3.Key);
                        DateTime t = dateNew;
                        kvLuaInfo3 = kvLuaInfo;
                        if (t > kvLuaInfo3.Value.dateLastWrite)
                        {
                            Func<string> code = delegate ()
                            {
                                KeyValuePair<string, LuaExeInfo> kvLuaInfo2 = kvLuaInfo;
                                return File.ReadAllText(kvLuaInfo2.Key);
                            };
                            kvLuaInfo3 = kvLuaInfo;
                            string chunkName = Path.GetFileName(kvLuaInfo3.Key);
                            LuaChunk c = this.lua.CompileChunk(code(), chunkName, false, new KeyValuePair<string, Type>[0]);
                            kvLuaInfo3 = kvLuaInfo;
                            kvLuaInfo3.Value.dateLastWrite = dateNew;
                            kvLuaInfo3 = kvLuaInfo;
                            kvLuaInfo3.Value.luaChunk = c;
                        }
                    }
                }
            }
        }

        // Token: 0x06001879 RID: 6265 RVA: 0x0017ED9C File Offset: 0x0017CF9C
        public LuaGlobal ExeLua(string strLuaPath)
        {
            LuaExeInfo exeInfo = null;
            string strFullPath = Path.GetFullPath(strLuaPath);
            lock (this.dictLuaCache)
            {
                if (!this.dictLuaCache.TryGetValue(strFullPath, out exeInfo))
                {
                    Func<string> code = () => File.ReadAllText(strFullPath);
                    string chunkName = Path.GetFileName(strFullPath);
                    LuaChunk c = this.lua.CompileChunk(code(), chunkName, false, new KeyValuePair<string, Type>[0]);
                    exeInfo = new LuaExeInfo();
                    exeInfo.dateLastWrite = File.GetLastWriteTime(strFullPath);
                    exeInfo.luaChunk = c;
                    this.dictLuaCache.Add(strFullPath, exeInfo);
                }
                this.gEnv.DoChunk(exeInfo.luaChunk, new object[0]);
            }
            return this.gEnv;
        }

        // Token: 0x0600187A RID: 6266 RVA: 0x0017EEB4 File Offset: 0x0017D0B4
        public LuaResult ExecLuaFunction(LuaManager luaManager, dynamic g, string strLuaFunction, GameClient client)
        {
            lock (this.dictLuaCache)
            {
                return (LuaResult)g[strLuaFunction](GameManager.LuaMgr, client, null);
            }
        }

        // Token: 0x040022B5 RID: 8885
        private Dictionary<string, LuaExeInfo> dictLuaCache = new Dictionary<string, LuaExeInfo>();

        // Token: 0x040022B6 RID: 8886
        private Lua lua = new Lua();

        // Token: 0x040022B7 RID: 8887
        private LuaGlobal gEnv = null;

        // Token: 0x040022B8 RID: 8888
        private static Timer timerCheckDict;

        // Token: 0x040022B9 RID: 8889
        private static LuaExeManager instance = new LuaExeManager();
    }
}
