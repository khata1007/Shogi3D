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

        //3Dの箱とフレーム、2Dの箱とフレーム、移動可能場所の赤い円
        private GameObject[,,] orangeBox3D;
        private GameObject[,,] greenBox3D;

        private GameObject[,] frameX3D;
        private GameObject[,] frameZ3D;

        private GameObject[,,] orangeBox2D;
        private GameObject[,,] greenBox2D;

        private GameObject[,] frameX2D;
        private GameObject[,] frameZ2D;

        private GameObject[,,] movableGrid3D;
        private GameObject[,,] movableGrid2D;

        //盤上の駒
        private GameObject[,,] koma_on_board3D; //実際の駒への参照配列
        private GameObject[,,] koma_on_board2D;
        private int[,,] boardstate;
        private Button[,,] gridButton2D;
        private Button chooseResetButton2D;
        

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

            movableGrid3D = new GameObject[xLength, yLength, zLength];
            movableGrid2D = new GameObject[xLength, yLength, zLength];
            boardstate = new int[xLength, yLength, zLength];

            
            //領域確保した段階で配列の参照をKomaクラスの方にも渡す. 確保前に渡すとPutKomaでぬるぽ吐く. どうして...
            koma.Boardstate = boardstate;
            koma.Koma3D = koma_on_board3D;
            koma.Koma2D = koma_on_board2D;

            board.CreateBoard(orangeBox3D, greenBox3D, orangeBox2D, greenBox2D, movableGrid3D, movableGrid2D);
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

                        active3D = movableGrid3D[x, y, z].activeSelf;
                        active2D = movableGrid2D[x, y, z].activeSelf;
                        if (active3D & !active2D)
                        {
                            movableGrid2D[x, y, z].SetActive(true);
                            movableGrid3D[x, y, z].SetActive(false);
                        }
                        else if (!active3D & active2D)
                        {
                            movableGrid2D[x, y, z].SetActive(false);
                            movableGrid3D[x, y, z].SetActive(true);
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

        private bool CheckMovable(int gx, int gy, int gz, int sx, int sy, int sz, bool keima = false)
        {
            if (!keima)
            {
                int max = Math.Max(Math.Abs(gx - sx), Math.Max(Math.Abs(gy - sy), Math.Abs(gz - sz)));
                int dx = (gx - sx) / max, dy = (gy - sy) / max, dz = (gz - sz) / max;
                int posx = sx + dx, posy = sy + dy, posz = sz + dz;
                while (posx != gx || posy != gy || posz != gz)
                {
                    if (boardstate[posx, posy, posz] != 0) return false;
                    posx += dx; posy += dy; posz += dz;
                }
            }
            //ここまでで「駒が重なってもok」とした. 
            return boardstate[sx, sy, sz] * boardstate[gx, gy, gz] <= 0; //自分の駒と重なるのはNG
        }

        private void UnActivateChoosingGrid()
        {
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        //greenBox3D[x, y, z].SetActive(false);
                        //greenBox2D[x, y, z].SetActive(false);
                        movableGrid3D[x, y, z].SetActive(false);
                        movableGrid2D[x, y, z].SetActive(false);
                        orangeBox3D[x, y, z].SetActive(false);
                        orangeBox2D[x, y, z].SetActive(false);
                    }
                }
            }
        }

        public void ChooseGrid(int idx)
        {
            int pushz = idx % 10;
            int pushy = (idx / 10) % 10;
            int pushx = (idx / 100) % 10;
            //選択マスが移動可能マス（赤丸の表示されているマス）の場合は駒を移動させる
            if (movableGrid2D[pushx, pushy, pushz].activeSelf)
            {
                int fromx = -1, fromy = -1, fromz = -1;
                for(int x = 0; x < xLength; x++)
                {
                    for(int y = 0; y < yLength; y++)
                    {
                        for(int z = 0; z < zLength; z++)
                        {
                            if(orangeBox2D[x, y, z].activeSelf) { fromx = x; fromy = y; fromz = z; }
                        }
                    }
                }
                if (fromx == -1 || fromy == -1 || fromz == -1) Debug.Log("gridChooseError");
                else
                {
                    int dx = pushx - fromx, dy = pushy - fromy, dz = pushz - fromz;
                    koma_on_board3D[fromx, fromy, fromz].transform.position += new Vector3(dx, dy, dz);
                    koma_on_board2D[fromx, fromy, fromz].transform.position += new Vector3(dx * scale2D - (dy * 10) * scale2D, 0, dz * scale2D);

                    koma_on_board3D[pushx, pushy, pushz] = koma_on_board3D[fromx, fromy, fromz];
                    koma_on_board3D[fromx, fromy, fromz] = null;
                    koma_on_board2D[pushx, pushy, pushz] = koma_on_board2D[fromx, fromy, fromz];
                    koma_on_board2D[fromx, fromy, fromz] = null;
                    
                    boardstate[pushx, pushy, pushz] = boardstate[fromx, fromy, fromz];
                    boardstate[fromx, fromy, fromz] = 0;
                    UnActivateChoosingGrid();
                }
            }
            else
            {
                //選択マスの orangeBoard による色付け 軌道上の駒を無視したら最大Nマス移動可能として計算量O(N^2) (必要に応じて改善)
                UnActivateChoosingGrid();
                if (boardstate[pushx, pushy, pushz] != 0) orangeBox2D[pushx, pushy, pushz].SetActive(true);

                //移動可能マスの movableGrid による色付け
                List<Vector3Int> move = Koma.getKomaMove(boardstate[pushx, pushy, pushz]);
                foreach (Vector3Int vec in move)
                {
                    int next_x = pushx + vec.x, next_y = pushy + vec.y, next_z = pushz + vec.z;
                    
                    if(CheckBound(next_x, next_y, next_z))
                    {
                        if ((boardstate[pushx, pushy, pushz] == 3 && CheckMovable(next_x, next_y, next_z, pushx, pushy, pushz, true)) ||
                            boardstate[pushx, pushy, pushz] != 3 && CheckMovable(next_x, next_y, next_z, pushx, pushy, pushz))
                            movableGrid2D[next_x, next_y, next_z].SetActive(true);
                    }
                }
            }
        }

        public void SetGridButton2D(Button b, int x, int y, int z) { gridButton2D[x, y, z] = b; }
        public Button ChooseResetButton2D { set { chooseResetButton2D = value; } }
        public bool MouseDetectable { get { return mouseDetectable; } set { mouseDetectable = value; } }
    }
}
