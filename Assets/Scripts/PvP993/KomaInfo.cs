using System;
using System.Collections.Generic;

namespace PvP993
{
    public class KomaInfo
    {
        public enum Koma { Fu, Kyo, Kei, Gin, Kin, Kak, Hi, Ou, Gyo};

        private const float k = 0.9f / 31; //王将のz軸方向が0.90fになるようにする
        public static readonly float[,] sunpou =
        {
            { k*22.0f,k*27.0f,k*7.6f},
            { k*23.0f,k*28.0f,k*7.9f},
            { k*25.0f,k*28.0f,k*7.9f},
            { k*26.0f,k*29.0f,k*8.2f},
            { k*26.0f,k*29.0f,k*8.2f},
            { k*27.0f,k*30.0f,k*8.6f},
            { k*27.0f,k*30.0f,k*8.6f},
            { k*28.0f,k*31.0f,k*9.2f},
            { k*28.0f,k*31.0f,k*9.2f},
        };
        public static readonly float[,] deg =
        {
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
            {146.0f, 80.5f, 85.0f },
        };
        public static readonly Dictionary<string, string> nari =
            new Dictionary<string, string>()
        {
                {"歩兵", "と" },
                {"香車", "成香" },
                {"桂馬", "成桂" },
                {"銀将", "成銀" },
                {"金将", "金将" },
                {"角行", "馬" },
                {"飛車", "竜" },
                {"王将", "王将" },
                {"玉将", "玉将" },
        };
    }
}
