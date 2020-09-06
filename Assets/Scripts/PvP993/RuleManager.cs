using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvP993
{
    class RuleManager
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        private int[,,] boardstate;

        public RuleManager(int[,,] bs)
        {
            boardstate = bs;
        }

        public int getVal(int x, int y, int z)
        {
            return boardstate[x, y, z];
        }

        public int[,,] Boardstate { set { boardstate = value; } }
    }
}
