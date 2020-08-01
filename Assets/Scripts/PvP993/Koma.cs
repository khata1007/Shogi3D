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
        private GameObject[,,] koma3D;
        private GameObject[,,] koma2D;
        private int[,,] boardstate;
        public GameObject[] pieces = new GameObject[9 * 3];
        public enum Kind { Emp, Fu, Kyo, Kei, Gin, Kak, Hi, Kin, Ou, Gyo, To, NariKyo, NariKei, NariGin, Uma, Ryu};

        //-----------------------駒の設計図-----------------------//
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
        //---------------ここまで駒の設計図-----------------//

        private static readonly Dictionary<Kind, int> kind_to_piece = //PutKomaでkindを目的のpiecesに変換するためのdictionary
            new Dictionary<Kind, int>()
        {
                {Kind.Fu, 0 },
                {Kind.Kyo, 3 },
                {Kind.Kei, 6 },
                {Kind.Gin, 9 },
                {Kind.Kin, 12 },
                {Kind.Kak, 15 },
                {Kind.Hi, 18 },
                {Kind.Ou, 21 },
                {Kind.Gyo, 24 },
        };

        private Color[] komaColor = new Color[]
        {
            new Color(238, 130, 238),
            new Color(255, 0, 0),
            new Color(255, 182, 193),
        };

        private static readonly Dictionary<int, int> komaGet =
            new Dictionary<int, int>()
        {
                {1, -1 }, {-1, 1 },
                {2, -2 }, {-2, 2 },
                {3, -3 }, {-3, 3 },
                {4, -4 }, {-4, 4 },
                {5, -5 }, {-5, 5 },
                {6, -6 }, {-6, 6 },
                {7, -7 }, {-7, 7 },
                {8, -8 }, {-8, 8 },
                {9, -9 }, {-9, 9 },
                {10, -1 }, {-10, 1 },
                {11, -2 }, {-11, 2 },
                {12, -3 }, {-12, 3 },
                {13, -4 }, {-13, 4 },
                {14, -5 }, {-14, 5 },
                {15, -6 }, {-15, 6 },
        };

        private static readonly Dictionary<string, int> komaName_to_kind =
            new Dictionary<string, int>()
        {
                {"歩", 1 },
                {"香\n車", 2 },
                {"桂\n馬", 3 },
                {"銀\n将", 4 },
                {"角\n行", 5 },
                {"飛\n車", 6 },
                {"金\n将", 7 },
                {"王\n将", 8 },
                {"玉\n将", 9 },
                {"と", 10 },
                {"成\n香", 11 },
                {"成\n桂", 12 },
                {"成\n銀", 13 },
                {"馬", 14 },
                {"竜", 15 },
        };

        private static readonly Dictionary<string, string> nari =
            new Dictionary<string, string>()
        {
                {"歩", "と" },
                {"香\n車", "成\n香" },
                {"桂\n馬", "成\n桂" },
                {"銀\n将", "成\n銀" },
                {"金\n将", "金\n将" },
                {"角\n行", "馬" },
                {"飛\n車", "竜" },
                {"王\n将", "王\n将" },
                {"玉\n将", "玉\n将" },
        };


        private void Start()
        {

        }

        public void InitialSet()
        {
            for (int x = 0; x < xLength; x++)
            {
                PutKoma(1, Kind.Fu, x, 0, 2);
                PutKoma(-1, Kind.Fu, x, 2, zLength - 3);
            }
            //銀桂香の配置
            for (int x = 0; x <= 2; x++)
            {
                Kind k = (Kind)Enum.ToObject(typeof(Kind), x + 2);
                PutKoma(1, k, x, 0, 0);
                PutKoma(1, k, 8 - x, 0, 0);
                PutKoma(-1, k, x, 2, 8);
                PutKoma(-1, k, 8 - x, 2, 8);
            }
            //その他の駒の配置
            PutKoma(1, Kind.Kin, 3, 0, 0);
            PutKoma(1, Kind.Kin, 5, 0, 0);
            PutKoma(-1,Kind.Kin, 3, 2, 8);
            PutKoma(-1,Kind.Kin, 5, 2, 8);
            PutKoma(1, Kind.Ou, 4, 0, 0);
            PutKoma(-1,Kind.Gyo, 4, 2, 8);
            PutKoma(1, Kind.Kak, 1, 0, 1);
            PutKoma(-1,Kind.Kak, 7, 2, 7);
            PutKoma(1, Kind.Hi, 7, 0, 1);
            PutKoma(-1,Kind.Hi, 1, 2, 7);
        }

        public void PutKoma(int o, Kind kind, int x, int y, int z) //owner, 駒の種類, x, y, z, 拡大縮小
        {
            int k = kind_to_piece[kind]; //kindにあたる駒のpiecesにおける添え字番号を計算
            koma3D[x, y, z] = Instantiate(pieces[k], this.transform);
            koma3D[x, y, z].transform.position = new Vector3(x, y - 0.5f, z);
            if (o == -1) koma3D[x, y, z].transform.eulerAngles = new Vector3(0, 180, 0);
            koma3D[x, y, z].GetComponent<Renderer>().material.color = komaColor[y];

            koma2D[x, y, z] = Instantiate(pieces[k], this.transform);
            koma2D[x, y, z].transform.position = new Vector3(x * komaScale2D - (y - 1) * 10 * komaScale2D, 0, z * komaScale2D);
            if (o == -1) koma2D[x, y, z].transform.eulerAngles = new Vector3(0, 180, 0);
            koma2D[x, y, z].GetComponent<Renderer>().material.color = komaColor[y];
            koma2D[x, y, z].SetActive(false);

            koma2D[x, y, z].transform.localScale = new Vector3(komaScale2D, komaScale2D, komaScale2D);
            boardstate[x, y, z] = (int)kind * o;
        }

        public int Nari(GameObject koma)
        {
            int x = (int)koma.transform.position.x, y = (int)(koma.transform.position.y + 0.5f), z = (int)koma.transform.position.z; 
            Text targetText = koma.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
            string kind = targetText.text;

            //もし成るならtargetTextを書き換える
            bool nari_flag = true;
            targetText.text = nari[kind];

            return nari_flag ? komaName_to_kind[nari[kind]] : komaName_to_kind[kind];
        }


        public float KomaScale2D { set { this.komaScale2D = value; } }
        public GameObject[,,] Koma3D { set { this.koma3D = value; } }
        public GameObject[,,] Koma2D { set { this.koma2D = value; } }
        public int[,,] Boardstate { set { this.boardstate = value; } }
    }
}
