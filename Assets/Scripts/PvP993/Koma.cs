using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PvP993
{
    public class Koma : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;
        private float komaScale2D;
        public enum Kind { Fu, Kyo, Kei, Gin, Kin, Kak, Hi, Ou, Gyo};

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
        private static readonly Dictionary<string, string> nari =
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

        private int owner; //駒の所有者. 自分 -> 1, 相手 -> -1
        public GameObject[] pieces = new GameObject[9*3];

        private Color[] komaColor = new Color[]
        {
            new Color(238, 130, 238),
            new Color(255, 0, 0),
            new Color(255, 182, 193),
        };

        private void Start()
        {

        }

        public void PutKoma(int o, int kind, int x, int y, int z, GameObject[,,] koma3D, GameObject[,,] koma2D) //owner, 駒の種類, x, y, z, 拡大縮小
        {
            koma3D[x, y, z] = Instantiate(pieces[kind * 3], this.transform);
            koma3D[x, y, z].transform.position = new Vector3(x, y - 0.5f, z);
            if (o == -1) koma3D[x, y, z].transform.eulerAngles = new Vector3(0, 180, 0);
            koma3D[x, y, z].GetComponent<Renderer>().material.color = komaColor[y];

            koma2D[x, y, z] = Instantiate(pieces[kind * 3], this.transform);
            koma2D[x, y, z].transform.position = new Vector3(x * komaScale2D - (1 - y) * 10 * komaScale2D, 0, z * komaScale2D);
            if (o == -1) koma2D[x, y, z].transform.eulerAngles = new Vector3(0, 180, 0);
            koma2D[x, y, z].GetComponent<Renderer>().material.color = komaColor[y];
            koma2D[x, y, z].SetActive(false);

            koma2D[x, y, z].transform.localScale = new Vector3(komaScale2D, komaScale2D, komaScale2D);

        }


        public float KomaScale2D { set { this.komaScale2D = value; } }
    }
}
