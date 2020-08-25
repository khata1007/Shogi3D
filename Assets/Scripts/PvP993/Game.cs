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

        private Vector3 prevCameraPos;

        private bool mouseDetectable = true;

        [SerializeField, Range(0.7f, 1.3f)] private float scale2D = 1.0f;

        public Koma koma;
        public Board board;
        public CameraMover camera;

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
        private Button[,,] gridButton2D;
        

        private void Awake()
        {
            board.BoardScale2D = scale2D;
            koma.KomaScale2D = scale2D;

            gridButton2D = new Button[xLength, yLength, zLength];
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

            
            //領域確保した段階で配列の参照をKomaクラスの方にも渡す. 確保前に渡すとPutKomaでぬるぽ吐く. どうして...
            koma.Boardstate = boardstate;
            koma.Koma3D = koma_on_board3D;
            koma.Koma2D = koma_on_board2D;

            board.CreateBoard(orangeBox3D, greenBox3D, orangeBox2D, greenBox2D);
            board.CreateFrame(frameX3D, frameZ3D, frameX2D, frameZ2D);

            //koma_on_Board3D/2Dをnullで初期化
            for (int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        koma_on_board3D[x, y, z] = null;
                        koma_on_board2D[x, y, z] = null;
                        boardstate[x, y, z] = 0;
                    }
                }
            }

            koma.InitialSet(); //初期配置

            boardstate[0, 0, 0] = koma.Nari(koma_on_board3D[0, 0, 0]);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ChangeDimension()
        {
            camera.CameraMovable = !camera.CameraMovable;
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

            if (frameZ2D[0, 0].activeSelf) //2Dモードに切り替えた場合はカメラを固定
            {
                float c = 4.5f;
                prevCameraPos = camera.MainCameraTransformPosition;
                camera.MainCamera2DSetting(new Vector3(c, 12, c), new Vector3(c, 0, c));
            }
            else camera.MainCameraTransformPosition = prevCameraPos;

            //3Dと2DのorangeBox, greenBoxのactiveを（必要なら）入れ替える
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        bool active3D, active2D;
                        active3D = orangeBox3D[x, y, z].activeSelf;
                        active2D = orangeBox2D[x, y, z].activeSelf;
                        if(active3D & !active2D)
                        {
                            orangeBox2D[x, y, z].SetActive(true);
                            orangeBox3D[x, y, z].SetActive(false);
                        }
                        else if (!active3D & active2D)
                        {
                            orangeBox2D[x, y, z].SetActive(false);
                            orangeBox3D[x, y, z].SetActive(true);
                        }

                        active3D = greenBox3D[x, y, z].activeSelf;
                        active2D = greenBox2D[x, y, z].activeSelf;
                        if (active3D & !active2D)
                        {
                            greenBox2D[x, y, z].SetActive(true);
                            greenBox3D[x, y, z].SetActive(false);
                        }
                        else if (!active3D & active2D)
                        {
                            greenBox2D[x, y, z].SetActive(false);
                            greenBox3D[x, y, z].SetActive(true);
                        }
                    }
                }
            }

            //gridButton の enabled を入れ替える
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        gridButton2D[x, y, z].enabled = !gridButton2D[x, y, z].enabled;
                    }
                }
            }
        }

        private bool CheckBound(int x, int y, int z)
        {
            return (0 <= x && x < xLength) && (0 <= y && y < yLength) && (0 <= z && z < zLength);
        }

        public void ChooseGrid(int idx)
        {
            int nowz = idx % 10;
            int nowy = (idx / 10) % 10;
            int nowx = (idx / 100) % 10;
            //選択マスの orangeBoard による色付け
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        orangeBox2D[x, y, z].SetActive(false);
                        greenBox2D[x, y, z].SetActive(false);
                    }
                }
            }
            if (boardstate[nowx, nowy, nowz] != 0) orangeBox2D[nowx, nowy, nowz].SetActive(true);

            //移動可能マスの greenBox による色付け
            List<Vector3Int> move = Koma.getKomaMove(boardstate[nowx, nowy, nowz]);
            foreach(Vector3Int vec in move)
            {
                int next_x = nowx + vec.x, next_y = nowy + vec.y, next_z = nowz + vec.z;
                if (CheckBound(next_x, next_y, next_z)) greenBox2D[next_x, next_y, next_z].SetActive(true);
            }
        }

        public void SetGridButton2D(Button b, int x, int y, int z) { gridButton2D[x, y, z] = b; }
        public bool MouseDetectable { get { return mouseDetectable; } set { mouseDetectable = value; } }
    }
}
