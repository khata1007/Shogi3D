using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PvP993
{
    public class Game : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        [SerializeField, Range(0.5f, 0.9f)] private float scale2D = 0.7f;

        public Koma koma;
        public Board board;

        //3Dの箱とフレーム、2Dの箱とフレーム
        private GameObject[,,] orangeBox3D;
        private GameObject[,,] greenBox3D;

        private GameObject[,] frameX3D;
        private GameObject[,] frameZ3D;

        private GameObject[,,] orangeBox2D;
        private GameObject[,,] greenBox2D;

        private GameObject[,] frameX2D;
        private GameObject[,] frameZ2D;

        //盤上の駒
        private GameObject[,,] koma_on_board3D; //実際の駒への参照配列
        private GameObject[,,] koma_on_board2D;
        private int[,,] boardstate;
        

        private void Awake()
        {
            board.BoardScale2D = scale2D;
            koma.KomaScale2D = scale2D;
        }
        // Start is called before the first frame update
        void Start()
        {
            orangeBox3D = new GameObject[xLength, yLength, zLength];
            greenBox3D = new GameObject[xLength, yLength, zLength];

            frameX3D = new GameObject[yLength, zLength + 1]; //+1するのはzLength個のブロックを間に挟むから
            frameZ3D = new GameObject[yLength, xLength + 1];

            orangeBox2D = new GameObject[xLength, yLength, zLength];
            greenBox2D = new GameObject[xLength, yLength, zLength];

            frameX2D = new GameObject[yLength, zLength + 1];
            frameZ2D = new GameObject[yLength, xLength + 1];

            koma_on_board3D = new GameObject[xLength, yLength, zLength];
            koma_on_board2D = new GameObject[xLength, yLength, zLength];
            boardstate = new int[xLength, yLength, zLength];

            board.CreateBoard(orangeBox3D, greenBox3D, orangeBox2D, greenBox2D);
            board.CreateFrame(frameX3D, frameZ3D, frameX2D, frameZ2D);

            //koma_on_Board3D/2Dをnullで初期化
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        koma_on_board3D[x, y, z] = null;
                        koma_on_board2D[x, y, z] = null;
                    }
                }
            }

            //-----------------初期配置--------------------//
            //歩の配置
            for (int x = 0; x < xLength; x++)
            {
                koma.PutKoma(1, Koma.Kind.Fu, x, 0, 2, koma_on_board3D, koma_on_board2D);
                koma.PutKoma(-1, Koma.Kind.Fu, x, 2, zLength - 3, koma_on_board3D, koma_on_board2D);
            }
            //銀桂香の配置
            for(int x = 0; x <= 2; x++)
            {
                Koma.Kind k = (Koma.Kind) Enum.ToObject(typeof(Koma.Kind), x + 2);
                koma.PutKoma(1, k, x, 0, 0, koma_on_board3D, koma_on_board2D);
                koma.PutKoma(1, k, 8 - x, 0, 0, koma_on_board3D, koma_on_board2D);
                koma.PutKoma(-1, k, x, 2, 8, koma_on_board3D, koma_on_board2D);
                koma.PutKoma(-1, k, 8 - x, 2, 8, koma_on_board3D, koma_on_board2D);
            }
            //その他の駒の配置
            koma.PutKoma(1, Koma.Kind.Kin, 3, 0, 0, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(1, Koma.Kind.Kin, 5, 0, 0, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(-1, Koma.Kind.Kin, 3, 2, 8, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(-1, Koma.Kind.Kin, 5, 2, 8, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(1, Koma.Kind.Ou, 4, 0, 0, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(-1,Koma.Kind.Gyo, 4, 2, 8, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(1, Koma.Kind.Kak, 1, 0, 1, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(-1,Koma.Kind.Kak, 7, 2, 7, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(1, Koma.Kind.Hi, 7, 0, 1, koma_on_board3D, koma_on_board2D);
            koma.PutKoma(-1,Koma.Kind.Hi, 1, 2, 7, koma_on_board3D, koma_on_board2D);



            Debug.Log(boardstate[0, 0, 0]);
            boardstate[0, 0, 0] = koma.Nari(koma_on_board3D[0, 0, 0]);
            Debug.Log(boardstate[0, 0, 0]);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeDimension()
        {
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        //orangeBox3D[x, y, z].SetActive(!orangeBox3D[x, y, z].activeSelf);
                        //orangeBox2D[x, y, z].SetActive(!orangeBox2D[x, y, z].activeSelf);
                        //greenBox3D[x, y, z].SetActive(!greenBox3D[x, y, z].activeSelf);
                        //greenBox2D[x, y, z].SetActive(!greenBox2D[x, y, z].activeSelf);

                        if (koma_on_board3D[x, y, z] != null) koma_on_board3D[x, y, z].SetActive(!koma_on_board3D[x, y, z].activeSelf);
                        if (koma_on_board2D[x, y, z] != null) koma_on_board2D[x, y, z].SetActive(!koma_on_board2D[x, y, z].activeSelf);
                    }
                }
            }

            for(int y = 0; y < yLength; y++)
            {
                for(int z = 0; z <= zLength; z++)
                {
                    frameX3D[y, z].SetActive(!frameX3D[y, z].activeSelf);
                    frameX2D[y, z].SetActive(!frameX2D[y, z].activeSelf);
                }
            }
            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x <= xLength; x++)
                {
                    frameZ3D[y, x].SetActive(!frameZ3D[y, x].activeSelf);
                    frameZ2D[y, x].SetActive(!frameZ2D[y, x].activeSelf);
                }
            }
        }

        public void chooseKoma(int kind)
        {
            Debug.Log("Koma selected.");
        }
    }
}
