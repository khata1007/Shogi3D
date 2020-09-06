using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace PvP993
{
    public class Game : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        private Vector3 prevCameraPos;
        private bool is3D = true;

        private bool mouseDetectable = true;

        [SerializeField, Range(0.7f, 1.3f)] private float scale2D = 1.0f;

        
        public Koma koma;
        public Board board;
        public CameraMover camera;
        public GameObject nariConfirmCanvas;
        public GameObject opponentMochigomaObj;
        public GameObject myMochigomaObj;
        public GameObject mochigomaButtonPrefab;
        public GameObject mochigomaBoxPrefab;

        public Camera mainCamera;
        public Camera upperCamera;
        public Camera underCamera;


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

        //持ち駒
        private List<GameObject> myMochigomaInstance;
        private List<int> myMochigomaIdx; //e.g. 持ち駒が歩と桂馬 -> {1, 3}
        private List<Button> myMochigomaButton;
        private List<GameObject> myMochigomaBox;
        private List<GameObject> opMochigomaInstance;
        private List<int> opMochigomaIdx;
        private List<Button> opMochigomaButton;
        private List<GameObject> opMochigomaBox;

        List<Vector3Int> record; //その対局の棋譜 

        RuleManager rule; //ルール管理

        private int turn = 1; //手番


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

            int piecenum = 40; //持ち駒になり得る駒は高々40枚
            myMochigomaInstance = new List<GameObject>(piecenum);
            myMochigomaIdx = new List<int>(piecenum);
            myMochigomaButton = new List<Button>(piecenum);
            myMochigomaBox = new List<GameObject>(piecenum);
            opMochigomaInstance = new List<GameObject>(piecenum);
            opMochigomaIdx = new List<int>(piecenum);
            opMochigomaButton = new List<Button>(piecenum);
            opMochigomaBox = new List<GameObject>(piecenum);

            record = new List<Vector3Int>();

            rule = new RuleManager(boardstate);

            
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

            nariConfirmCanvas.SetActive(false);

            /*
            Debug.Log(rule.getVal(0, 0, 0));
            boardstate[0, 0, 0] = koma.Nari(koma_on_board3D[0, 0, 0]);
            Debug.Log(rule.getVal(0, 0, 0));
            */
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ChangeDimension()
        {
            is3D = !is3D;
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

            //3Dと2DのorangeBox, greenBox, movableGridのactiveを（必要なら）入れ替える
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

        private void ChangeTurn() //手番を終えたプレイヤーの持ち駒および盤上の駒を選択できなくする -> 駒が取れなくなるので盤上の駒でそれやっちゃダメ
        {
            /*
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        if (turn * boardstate[x, y, z] > 0) gridButton2D[x, y, z].enabled = false;
                        else gridButton2D[x, y, z].enabled = true;
                    }
                }
            }
            */

            //持ち駒に関しては一考の余地あり
            if (turn == 1)
            {
                foreach (Button b in myMochigomaButton) b.enabled = false;
                foreach (Button b in opMochigomaButton) b.enabled = true;
            }
            else
            {
                foreach (Button b in myMochigomaButton) b.enabled = true;
                foreach (Button b in opMochigomaButton) b.enabled = false;
            }

            turn *= -1;
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

        public async void ChooseGrid(int idx)
        {
            int pushz = idx % 10;
            int pushy = (idx / 10) % 10;
            int pushx = (idx / 100) % 10;

            //選択マスが移動可能マス（赤丸の表示されているマス）の場合は駒を移動させる
            if (movableGrid2D[pushx, pushy, pushz].activeSelf)
            {
                int fromx = -1, fromy = -1, fromz = -1;
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            if (orangeBox2D[x, y, z].activeSelf) { fromx = x; fromy = y; fromz = z; }
                        }
                    }
                }
                if (fromx == -1 || fromy == -1 || fromz == -1)
                {
                    Debug.Log("Choosed from Mochigoma");

                    int choose = -1;
                    if (turn == 1) for (int i = 0; i < myMochigomaBox.Count; i++) { if (myMochigomaBox[i].activeSelf) choose = i; }
                    else for (int i = 0; i < opMochigomaBox.Count; i++) { if (opMochigomaBox[i].activeSelf) choose = i; }
                    Debug.Log(choose);
                    if (turn == 1) koma.PutKoma(turn, (Koma.Kind)Enum.ToObject(typeof(Koma.Kind), Math.Abs(myMochigomaIdx[choose])), pushx, pushy, pushz);
                    else koma.PutKoma(turn, (Koma.Kind)Enum.ToObject(typeof(Koma.Kind), Math.Abs(opMochigomaIdx[choose])), pushx, pushy, pushz);
                    koma_on_board2D[pushx, pushy, pushz].SetActive(true);
                    koma_on_board3D[pushx, pushy, pushz].SetActive(false); //PutKomaは標準で3Dを有効にするので
                    MochigomaRemove(choose);
                    UnActivateChoosingGrid();
                }
                else
                {
                    if ((pushz >= 7 && boardstate[fromx, fromy, fromz] == 3) || (pushz <= 1 && boardstate[fromx, fromy, fromz] == -3)) boardstate[fromx, fromy, fromz] = koma.Nari(koma_on_board3D[fromx, fromy, fromz], koma_on_board2D[fromx, fromy, fromz]) * turn;
                    else if ((pushz >= 6 && 1 <= boardstate[fromx, fromy, fromz] && boardstate[fromx, fromy, fromz] <= 6) ||
                             (pushz <= 2 && -6 <= boardstate[fromx, fromy, fromz] && boardstate[fromx, fromy, fromz] <= -1))
                    {
                        //成るかどうか聞く
                        mouseDetectable = false;
                        nariConfirmCanvas.SetActive(true);
                        bool res = await koma.CheckNari();
                        if (res) boardstate[fromx, fromy, fromz] = koma.Nari(koma_on_board3D[fromx, fromy, fromz], koma_on_board2D[fromx, fromy, fromz]) * turn;
                        nariConfirmCanvas.SetActive(false);
                        mouseDetectable = true;
                    }
                    if (boardstate[pushx, pushy, pushz] != 0) //相手の駒が置かれている場所に移動する場合
                    {
                        MochigomaGenerate(Math.Abs(boardstate[pushx, pushy, pushz]));
                        Destroy(koma_on_board3D[pushx, pushy, pushz]);
                        Destroy(koma_on_board2D[pushx, pushy, pushz]);

                    }

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
                if (record.Count >= 1)
                {
                    Vector3Int lastMove = record[record.Count - 1];
                    greenBox2D[lastMove.x, lastMove.y, lastMove.z].SetActive(false);
                }
                greenBox2D[pushx, pushy, pushz].SetActive(true); //今打った手を緑に光らせておく
                record.Add(new Vector3Int(pushx, pushy, pushz));

                Debug.Log(new Vector3Int(pushx, pushy, pushz) + ", " + boardstate[pushx, pushy, pushz]);
                ChangeTurn();
            }
            else if (boardstate[pushx, pushy, pushz] * turn > 0) //手番のプレイヤーが自分の駒を選択した時
            {
                if (orangeBox2D[pushx, pushy, pushz].activeSelf) //二回同じマスをクリック -> キャンセル
                {
                    UnActivateChoosingGrid();
                    return;
                }
                //選択マスの orangeBoard による色付け 軌道上の駒を無視したら最大Nマス移動可能として計算量O(N^2) (必要に応じて改善)
                UnActivateChoosingGrid();
                if (boardstate[pushx, pushy, pushz] != 0) orangeBox2D[pushx, pushy, pushz].SetActive(true);

                //移動可能マスの movableGrid による色付け
                List<Vector3Int> move = Koma.getKomaMove(Math.Abs(boardstate[pushx, pushy, pushz]));
                foreach (Vector3Int vec in move)
                {
                    int next_x = pushx + vec.x * turn, next_y = pushy + vec.y, next_z = pushz + vec.z * turn;

                    if (CheckBound(next_x, next_y, next_z))
                    {
                        if ((Math.Abs(boardstate[pushx, pushy, pushz]) == 3 && CheckMovable(next_x, next_y, next_z, pushx, pushy, pushz, true)) ||
                            (Math.Abs(boardstate[pushx, pushy, pushz]) != 3 && CheckMovable(next_x, next_y, next_z, pushx, pushy, pushz)))
                            movableGrid2D[next_x, next_y, next_z].SetActive(true);
                    }
                }
            }
            else UnActivateChoosingGrid();
        }

        void ChooseMochigoma(GameObject komainst, int idx) //idx: 相手の桂馬なら-3, 自分の歩なら1
        {
            if (komainst.transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf){
                UnActivateChoosingGrid();
                return;
            }
            UnActivateChoosingGrid();
            for(int x = 0; x < xLength; x++)
            {
                for(int y = 0; y < yLength; y++)
                {
                    for(int z = 0; z < zLength; z++)
                    {
                        if ((idx == -3 && z <= 1) || (idx == 3 && z >= 7)) continue; //桂馬
                        else if(boardstate[x, y, z] == 0)
                        {
                            if (is3D) movableGrid3D[x, y, z].SetActive(true);
                            else movableGrid2D[x, y, z].SetActive(true);
                        }
                    }
                }
            }
            
            komainst.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            Debug.Log(komainst);
        }

        //持ち駒を生成して整列するメソッド
        public void MochigomaGenerate(int kind)
        {
            if (kind >= 10) kind -= 9;
            GameObject p = koma.GetKoma2D(kind);
            GameObject g;
            if (turn == 1) g = Instantiate(p, myMochigomaObj.transform);
            else g = Instantiate(p, opponentMochigomaObj.transform);
            g.transform.localPosition = new Vector3(-10 * turn, 0, 0);

            Transform canv = g.transform.GetChild(0).transform;
            canv.GetComponent<Canvas>().worldCamera = (turn == 1) ? underCamera : upperCamera;

            GameObject but = Instantiate(mochigomaButtonPrefab, canv);
            but.transform.localPosition = new Vector3(0, 0, 0);

            Button b = but.transform.GetComponent<Button>();
            b.onClick.AddListener(() => ChooseMochigoma(g, turn * kind));

            GameObject box = Instantiate(mochigomaBoxPrefab, canv);
            box.transform.localPosition = new Vector3(0, 0, 0);
            box.transform.localScale = new Vector3(1, 1, 0.01f);
            box.SetActive(false);

            if (turn == 1) g.SetLayerRecursively(8);
            else g.SetLayerRecursively(9);

            int nowcnt = (turn == 1) ? myMochigomaInstance.Count : opMochigomaInstance.Count;
            if (nowcnt != 0)
            {
                if(turn == 1) g.transform.localPosition = myMochigomaInstance[nowcnt - 1].transform.localPosition + new Vector3(turn, 0, 0);
                else g.transform.localPosition = opMochigomaInstance[nowcnt - 1].transform.localPosition + new Vector3(turn, 0, 0);
            }
            if(turn == 1)
            {
                myMochigomaInstance.Add(g);
                myMochigomaIdx.Add(turn * kind);
                myMochigomaBox.Add(box);
                myMochigomaButton.Add(b);
            }
            else
            {
                opMochigomaInstance.Add(g);
                opMochigomaIdx.Add(turn * kind);
                opMochigomaBox.Add(box);
                opMochigomaButton.Add(b);
            }

            if(turn == 1)
            {
                int target = turn * kind;
                int ins = -1;
                for(int i = 0; i < myMochigomaIdx.Count-1; i++)
                {
                    if(target <= myMochigomaIdx[i]) { ins = i; break; }
                }
                if(ins != -1)
                {
                    for(int i = myMochigomaIdx.Count - 2; i >= ins; i--)
                    {
                        Debug.Log(i);
                        Vector3 pos = myMochigomaInstance[i + 1].transform.localPosition; //例えば銀がi, 桂馬がi+1のとき: まずは桂馬の位置を記録
                        GameObject inst = myMochigomaInstance[i + 1]; //桂馬のインスタンスも記憶
                        int idx = myMochigomaIdx[i + 1];
                        Button bt = myMochigomaButton[i + 1];
                        GameObject bx = myMochigomaBox[i + 1];

                        myMochigomaInstance[i + 1].transform.localPosition = myMochigomaInstance[i].transform.localPosition; //桂馬の位置を銀の位置と変える
                        myMochigomaInstance[i + 1] = myMochigomaInstance[i]; //i+1の参照先を銀に変更
                        myMochigomaIdx[i + 1] = myMochigomaIdx[i];
                        myMochigomaButton[i + 1] = myMochigomaButton[i];
                        myMochigomaBox[i + 1] = myMochigomaBox[i];
                        myMochigomaInstance[i].transform.localPosition = pos; //iはまだ銀を参照してる. 銀の位置を変更
                        myMochigomaInstance[i] = inst; //iの参照先を桂馬に変更
                        myMochigomaIdx[i] = idx;
                        myMochigomaButton[i] = bt;
                        myMochigomaBox[i] = bx;
                    }
                }
            }
            else
            {
                int target = turn * kind;
                int ins = -1;
                for (int i = 0; i < opMochigomaIdx.Count - 1; i++)
                {
                    if (target >= opMochigomaIdx[i]) { ins = i; break; }
                }
                if (ins != -1)
                {
                    for (int i = opMochigomaIdx.Count - 2; i >= ins; i--)
                    {
                        Vector3 pos = opMochigomaInstance[i + 1].transform.localPosition;
                        GameObject inst = opMochigomaInstance[i + 1];
                        int idx = opMochigomaIdx[i + 1];
                        Button bt = opMochigomaButton[i + 1];
                        GameObject bx = opMochigomaBox[i + 1];

                        opMochigomaInstance[i + 1].transform.localPosition = opMochigomaInstance[i].transform.localPosition;
                        opMochigomaInstance[i + 1] = opMochigomaInstance[i];
                        opMochigomaIdx[i + 1] = opMochigomaIdx[i];
                        opMochigomaButton[i + 1] = opMochigomaButton[i];
                        opMochigomaBox[i + 1] = opMochigomaBox[i];
                        opMochigomaInstance[i].transform.localPosition = pos;
                        opMochigomaInstance[i] = inst;
                        opMochigomaIdx[i] = idx;
                        opMochigomaButton[i] = bt;
                        opMochigomaBox[i] = bx;
                    }
                }
                for(int i = 0; i < opMochigomaIdx.Count; i++) { Debug.Log(opMochigomaIdx[i]); }
            }
        }

        //持ち駒の一つを盤に置いたときに再整列するメソッド
        public void MochigomaRemove(int idx)
        {
            Vector3 pos;
            if(turn == 1)
            {
                pos = myMochigomaInstance[idx].transform.localPosition;
                myMochigomaBox.RemoveAt(idx);
                myMochigomaButton.RemoveAt(idx);
                myMochigomaIdx.RemoveAt(idx);
                Destroy(myMochigomaInstance[idx]);
                myMochigomaInstance.RemoveAt(idx);
                for(int i = myMochigomaInstance.Count - 2; i >= idx; i--)
                {
                    myMochigomaInstance[i + 1].transform.localPosition = myMochigomaInstance[i].transform.localPosition;
                }
                if(myMochigomaInstance.Count > idx) myMochigomaInstance[idx].transform.localPosition = pos;
            }
            else
            {
                pos = opMochigomaInstance[idx].transform.localPosition;
                opMochigomaBox.RemoveAt(idx);
                opMochigomaButton.RemoveAt(idx);
                opMochigomaIdx.RemoveAt(idx);

                Destroy(opMochigomaInstance[idx]);
                opMochigomaInstance.RemoveAt(idx);
                for(int i = opMochigomaInstance.Count - 2; i >= idx; i--)
                {
                    opMochigomaInstance[i + 1].transform.localPosition = opMochigomaInstance[i].transform.localPosition;
                }
                if(opMochigomaInstance.Count > idx) opMochigomaInstance[idx].transform.localPosition = pos;
            }
        }

        public void ReverseView()
        {
            //camera.transform.eulerAngles += new Vector3(0, 180, 0);
        }

        public void UnActivateChoosingGrid()
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
            foreach (GameObject box in opMochigomaBox) box.SetActive(false);
            foreach (GameObject box in myMochigomaBox) box.SetActive(false);

        }
        
        public void SetGridButton2D(Button b, int x, int y, int z) { gridButton2D[x, y, z] = b; }
        public Button ChooseResetButton2D { set { chooseResetButton2D = value; } }
        public bool MouseDetectable { get { return mouseDetectable; } set { mouseDetectable = value; } }
    }
}
