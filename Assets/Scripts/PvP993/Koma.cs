using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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
        private static int nariflg = 0;
        public GameObject[] pieces = new GameObject[28];
        public GameObject[] pieces2D = new GameObject[10];
        public enum Kind { Emp, Fu, Kyo, Kei, Gin, Kak, Hi, Kin, Ou, Gyo, To, NariKyo, NariKei, NariGin, Uma, Ryu};

        //-----------------------駒の設計図-----------------------//
        //備考: sunpou と deg は ナンバリングが fu, kyo, kei, gin, kin, kak, hi, ou, gyo になっているので Kind との混同に注意
        //      本当は直すべきだけど sunpou と deg に触れるのは prefab 生成時だけなのでこのまま放置
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
                {Kind.Fu, 1 },
                {Kind.Kyo,2 },
                {Kind.Kei, 3 },
                {Kind.Gin, 4 },
                {Kind.Kin, 7 },
                {Kind.Kak, 5 },
                {Kind.Hi, 6 },
                {Kind.Ou, 8 },
                {Kind.Gyo, 9 },
        };

        private Color[] komaColor = new Color[]
        {
            new Color(0, 0, 0),
            new Color(255, 0, 0),
            new Color(0, 0, 0),
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

        private static List<Vector3Int>[] komaMove;

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
            komaMove = new List<Vector3Int>[16];
            for (int i = 0; i < 16; i++) komaMove[i] = new List<Vector3Int>();
            //歩の動き
            komaMove[1].Add(new Vector3Int(0, 1, 0));
            komaMove[1].Add(new Vector3Int(0, -1, 0));
            komaMove[1].Add(new Vector3Int(0, 0, 1));

            //香の動き
            for(int dy = -2; dy <= 2; dy++)
            {
                if(dy != 0) komaMove[2].Add(new Vector3Int(0, dy, 0));
            }
            for(int dz = 1; dz <= 8; dz++)
            {
                komaMove[2].Add(new Vector3Int(0, 0, dz));
            }

            //桂の動き
            komaMove[3].Add(new Vector3Int(1, 0, 2));
            komaMove[3].Add(new Vector3Int(-1, 0, 2));
            komaMove[3].Add(new Vector3Int(0, 1, 2));
            komaMove[3].Add(new Vector3Int(0, -1, 2));

            //銀
            for(int dy = -1; dy <= 1; dy++) komaMove[4].Add(new Vector3Int(1, dy, 1));
            for(int dy = -1; dy <= 1; dy++) komaMove[4].Add(new Vector3Int(-1, dy, 1));
            for(int dy = -1; dy <= 1; dy++) komaMove[4].Add(new Vector3Int(0, dy, 1));
            for(int dy = -1; dy <= 1; dy++) komaMove[4].Add(new Vector3Int(1, dy, -1));
            for(int dy = -1; dy <= 1; dy++) komaMove[4].Add(new Vector3Int(-1, dy, -1));
            //komaMove[4].Add(new Vector3Int(1, 1, 0));
            //komaMove[4].Add(new Vector3Int(1, -1, 0));

            //角
            for(int dy = -2, dz = -2; dy <= 2; dy++, dz++)
            {
                if(dy != 0) komaMove[5].Add(new Vector3Int(0, dy, dz));
            }
            for(int dy = -2, dz = 2; dy <= 2; dy++, dz--)
            {
                if (dy != 0) komaMove[5].Add(new Vector3Int(0, dy, dz));
            }
            for (int dy = -2, dx = -2; dy <= 2; dy++, dx++)
            {
                if(dy != 0) komaMove[5].Add(new Vector3Int(dx, dy, 0));
            }
            for (int dy = -2, dx = 2; dy <= 2; dy++, dx--)
            {
                if (dy != 0) komaMove[5].Add(new Vector3Int(dx, dy, 0));
            }
            for (int dx = -8, dz = -8; dx <= 8; dx++, dz++)
            {
                if(dx != 0) komaMove[5].Add(new Vector3Int(dx, 0, dz));
            }
            for(int dx = -8, dz = 8; dx <= 8; dx++, dz--)
            {
                if(dx != 0) komaMove[5].Add(new Vector3Int(dx, 0, dz));
            }
            //飛
            for(int dy = -2; dy <= 2; dy++)
            {
                if (dy != 0) komaMove[6].Add(new Vector3Int(0, dy, 0));
            }
            for(int dx = -8; dx <= 8; dx++)
            {
                if (dx != 0) komaMove[6].Add(new Vector3Int(dx, 0, 0));
            }
            for(int dz = -8; dz <= 8; dz++)
            {
                if (dz != 0) komaMove[6].Add(new Vector3Int(0, 0, dz));
            }
            //金
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[7].Add(new Vector3Int(-1, dy, 0));
            //komaMove[7].Add(new Vector3Int(0, 1, 0));
            //komaMove[7].Add(new Vector3Int(0, 1, 1));
            //komaMove[7].Add(new Vector3Int(0, -1, 0));
            //komaMove[7].Add(new Vector3Int(0, -1, 1));

            //王,玉
            for(int dx = -1; dx <= 1; dx++)
            {
                for(int dy = -1; dy <= 1; dy++)
                {
                    for(int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0) continue;
                        komaMove[8].Add(new Vector3Int(dx, dy, dz));
                        komaMove[9].Add(new Vector3Int(dx, dy, dz));
                    }
                }
            }
            //と
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[10].Add(new Vector3Int(-1, dy, 0));
            //成香
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[11].Add(new Vector3Int(-1, dy, 0));
            //成桂
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[12].Add(new Vector3Int(-1, dy, 0));
            //成銀
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[13].Add(new Vector3Int(-1, dy, 0));
            //馬
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[14].Add(new Vector3Int(-1, dy, 0));
            for (int i = 0; i < komaMove[5].Count; i++) komaMove[14].Add(komaMove[5][i]);
            //竜
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(-1, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(0, dy, 1));
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(1, dy, 0));
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(0, dy, -1));
            for (int dy = -1; dy <= 1; dy++) komaMove[15].Add(new Vector3Int(-1, dy, 0));
            for (int i = 0; i < komaMove[6].Count; i++) komaMove[15].Add(komaMove[6][i]);
        }

        public async UniTask<bool> CheckNari()
        {
            int ret;
            await UniTask.WaitWhile(() => nariflg == 0);
            ret = nariflg;
            nariflg = 0;
            return ret == 1;
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

            koma2D[x, y, z] = Instantiate(pieces2D[k], this.transform);
            koma2D[x, y, z].transform.position = new Vector3(x * komaScale2D - (y - 1) * 10 * komaScale2D, 0, z * komaScale2D);
            if (o == -1) koma2D[x, y, z].transform.eulerAngles = new Vector3(0, 180, 0);
            koma2D[x, y, z].GetComponent<Renderer>().material.color = komaColor[y];
            koma2D[x, y, z].SetActive(false);

            koma2D[x, y, z].transform.localScale = new Vector3(komaScale2D, komaScale2D, komaScale2D);
            boardstate[x, y, z] = (int)kind * o;
        }

        public int Nari(GameObject koma3D, GameObject koma2D)
        { 
            Text targetText3D = koma3D.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();

            string kind = targetText3D.text;
            targetText3D.text = nari[kind];

            targetText3D = koma3D.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
            targetText3D.text = nari[kind];

            Text targetText2D = koma2D.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
            targetText2D.text = nari[kind];

            return komaName_to_kind[nari[kind]]; //正値を返すので手番に応じて呼び出し元で-1倍してね
        }

       
        public static List<Vector3Int> getKomaMove(int k) { return komaMove[k]; }
        public GameObject GetKoma3D(int k) { return pieces[k]; }
        public GameObject GetKoma2D(int k) { return pieces2D[k]; }
        public float KomaScale2D { set { this.komaScale2D = value; } }
        public GameObject[,,] Koma3D { set { this.koma3D = value; } }
        public GameObject[,,] Koma2D { set { this.koma2D = value; } }
        public int[,,] Boardstate { set { this.boardstate = value; } }
        public static int Nariflg { set { if (value == 1 || value == -1) nariflg = value; } }
    }
}
