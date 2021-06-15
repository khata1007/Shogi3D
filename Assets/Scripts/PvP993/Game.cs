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
        public CameraMover cameraMover;
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
        private int[,,] myReachablePieces;
        private int[,,] opReachablePieces; //敵の駒で[x, y, z]に到達可能な盤上の駒の数

        //持ち駒
        private List<GameObject> myMochigomaInstance;
        private List<int> myMochigomaIdx; //e.g. 持ち駒が歩と桂馬 -> {1, 3}
        private List<Button> myMochigomaButton;
        private List<GameObject> myMochigomaBox;
        private List<GameObject> opMochigomaInstance;
        private List<int> opMochigomaIdx;
        private List<Button> opMochigomaButton;
        private List<GameObject> opMochigomaBox;

        private Vector3Int myOuPos, opOuPos;

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

            myReachablePieces = new int[xLength, yLength, zLength];
            opReachablePieces = new int[xLength, yLength, zLength];

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
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        koma_on_board3D[x, y, z] = null;
                        koma_on_board2D[x, y, z] = null;
                        boardstate[x, y, z] = 0;
                        myReachablePieces[x, y, z] = opReachablePieces[x, y, z] = 0;
                    }
                }
            }

            koma.InitialSet(); //初期配置

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if (boardstate[x, y, z] > 0) CalcReachableRange();
                        else if (boardstate[x, y, z] < 0) CalcReachableRange();
                    }
                }
            }
            myOuPos = new Vector3Int(4, 0, 0);
            opOuPos = new Vector3Int(4, 2, 8);
            Debug.Log(record.Count + ": " + myReachablePieces[0, 0, 1]);
            Debug.Log(record.Count + ": " + opReachablePieces[8, 2, 7]);

            nariConfirmCanvas.SetActive(false);

            /*
            Debug.Log(rule.getVal(0, 0, 0));
            boardstate[0, 0, 0] = koma.Nari(koma_on_board3D[0, 0, 0]);
            Debug.Log(rule.getVal(0, 0, 0));
            */
            Vector3Int temp = myOuPos;


        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void ChooseGrid(int idx) //実際に手番が進むメソッド 作業のしやすさを考慮してUpdate()直下に配置
        {
            if (!mouseDetectable) return;
            int pushz = idx % 10;
            int pushy = (idx / 10) % 10;
            int pushx = (idx / 100) % 10;

            //選択マスが移動可能マス（赤丸の表示されているマス）の場合は駒を移動させる ここで実際に手番が進む
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
                    //Debug.Log("Choosed from Mochigoma");

                    int choose = -1;
                    if (turn == 1) for (int i = 0; i < myMochigomaBox.Count; i++) { if (myMochigomaBox[i].activeSelf) choose = i; }
                    else for (int i = 0; i < opMochigomaBox.Count; i++) { if (opMochigomaBox[i].activeSelf) choose = i; }

                    if ((turn == 1 && !CheckLegal(myMochigomaIdx[choose], new Vector3Int(-1, -1, -1), new Vector3Int(pushx, pushy, pushz)))
                    || (turn == -1 && !CheckLegal(opMochigomaIdx[choose], new Vector3Int(-1, -1, -1), new Vector3Int(pushx, pushy, pushz))))
                        return; //合法手じゃなければなんもせずにreturn

                    if (turn == 1) koma.PutKoma(turn, (Koma.Kind)Enum.ToObject(typeof(Koma.Kind), Math.Abs(myMochigomaIdx[choose])), pushx, pushy, pushz);
                    else koma.PutKoma(turn, (Koma.Kind)Enum.ToObject(typeof(Koma.Kind), Math.Abs(opMochigomaIdx[choose])), pushx, pushy, pushz);
                    koma_on_board2D[pushx, pushy, pushz].SetActive(true);
                    koma_on_board3D[pushx, pushy, pushz].SetActive(false); //PutKomaは標準で3Dを有効にするので
                    MochigomaRemove(choose);
                    UnActivateChoosingGrid();
                }
                else
                {
                    if (!CheckLegal(boardstate[fromx, fromy, fromz], new Vector3Int(fromx, fromy, fromz), new Vector3Int(pushx, pushy, pushz))) return; //合法手でなければreturn

                    if ((pushz >= 7 && boardstate[fromx, fromy, fromz] == 3) || (pushz <= 1 && boardstate[fromx, fromy, fromz] == -3)) boardstate[fromx, fromy, fromz] = koma.Nari(koma_on_board3D[fromx, fromy, fromz], koma_on_board2D[fromx, fromy, fromz]) * turn;
                    else if ((pushz >= 6 && 1 <= boardstate[fromx, fromy, fromz] && boardstate[fromx, fromy, fromz] <= 6) ||
                             (pushz <= 2 && -6 <= boardstate[fromx, fromy, fromz] && boardstate[fromx, fromy, fromz] <= -1))
                    {
                        //成るかどうか聞く
                        mouseDetectable = false;
                        nariConfirmCanvas.SetActive(true);
                        bool res = await koma.CheckNari();
                        if (res)
                        {
                            boardstate[fromx, fromy, fromz] = koma.Nari(koma_on_board3D[fromx, fromy, fromz], koma_on_board2D[fromx, fromy, fromz]) * turn;
                        }
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

                    //王を動かしてるかどうかチェック
                    if (8 <= Math.Abs(boardstate[pushx, pushy, pushz]) && Math.Abs(boardstate[pushx, pushy, pushz]) <= 9)
                    {
                        if (turn == 1) myOuPos = new Vector3Int(pushx, pushy, pushz);
                        else opOuPos = new Vector3Int(pushx, pushy, pushz);
                    }
                }
                if (record.Count >= 1)
                {
                    Vector3Int lastMove = record[record.Count - 1];
                    greenBox2D[lastMove.x, lastMove.y, lastMove.z].SetActive(false);
                }
                greenBox2D[pushx, pushy, pushz].SetActive(true); //今打った手を緑に光らせておく
                record.Add(new Vector3Int(pushx, pushy, pushz));

                CalcReachableRange(); //駒の各マスに対する到達可能性を再計算

                if (GameSet() != 0)
                {
                    string winner = (turn == 1) ? "先手" : "後手";
                    Debug.Log(winner + "の勝ち");
                }
                ChangeTurn();

                //for (int i = 0; i < 10000000; i++) if(i == 1000000-1) Debug.Log("Done"); //どんくらいの計算量を許容できるかのテスト -> 10^7が限界
            }
            else if (boardstate[pushx, pushy, pushz] * turn > 0) //手番のプレイヤーが自分の駒を選択した時
            {
                Vector3Int now = new Vector3Int(pushx, pushy, pushz);
                if (orangeBox2D[pushx, pushy, pushz].activeSelf) //二回同じマスをクリック -> キャンセル
                {
                    UnActivateChoosingGrid();
                    return;
                }
                UnActivateChoosingGrid();
                if (boardstate[pushx, pushy, pushz] != 0) orangeBox2D[pushx, pushy, pushz].SetActive(true);

                //移動可能マスの movableGrid による色付け
                List<Vector3Int> move = Koma.getKomaMove(Math.Abs(boardstate[pushx, pushy, pushz]));
                foreach (Vector3Int vec in move)
                {
                    Vector3Int next = CalcNextPos(now, vec, turn);

                    if (CheckBound(next))
                    {
                        if ((Math.Abs(boardstate[pushx, pushy, pushz]) == 3 && CheckMovable(boardstate, next, now, true)) ||
                            (Math.Abs(boardstate[pushx, pushy, pushz]) != 3 && CheckMovable(boardstate, next, now)))
                            movableGrid2D[next.x, next.y, next.z].SetActive(true);
                    }
                }
            }
            else UnActivateChoosingGrid();
        }

        private Vector3Int CalcNextPos(Vector3Int now, Vector3Int inc, int owner)
        {
            return new Vector3Int(now.x + inc.x * owner, now.y + inc.y, now.z + inc.z * owner);
        }

        private void CalcReachableRange(int[,,] work = null) //workで与えた盤面における盤上の駒の到達範囲を計算 何も与えなければboardstateについて計算
        {
            if (work == null) work = boardstate;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        myReachablePieces[x, y, z] = 0;
                        opReachablePieces[x, y, z] = 0;
                    }
                }
            }
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        Vector3Int now = new Vector3Int(x, y, z);
                        if (work[x, y, z] > 0)
                        {
                            List<Vector3Int> komaMoveList = Koma.getKomaMove(work[x, y, z]);
                            foreach (Vector3Int komaMove in komaMoveList)
                            {
                                Vector3Int next = CalcNextPos(now, komaMove, 1);
                                if (work[x, y, z] == 3 && CheckBound(next) && CheckMovable(work, now, next, true)) myReachablePieces[next.x, next.y, next.z]++;
                                else if (work[x, y, z] != 3 && CheckBound(next) && CheckMovable(work, now, next)) myReachablePieces[next.x, next.y, next.z]++;
                            }
                        }
                        else if (work[x, y, z] < 0)
                        {
                            List<Vector3Int> komaMoveList = Koma.getKomaMove(work[x, y, z]);
                            foreach (Vector3Int komaMove in komaMoveList)
                            {
                                Vector3Int next = CalcNextPos(now, komaMove, -1);
                                if (work[x, y, z] == -3 && CheckBound(next) && CheckMovable(work, now, next, true)) opReachablePieces[next.x, next.y, next.z]++;
                                else if (work[x, y, z] != -3 && CheckBound(next) && CheckMovable(work, now, next)) opReachablePieces[next.x, next.y, next.z]++;
                            }
                        }
                    }
                }
            }
        }

        public void ChangeDimension()
        {
            is3D = !is3D;
            cameraMover.CameraMovable = !cameraMover.CameraMovable;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if (koma_on_board3D[x, y, z] != null) koma_on_board3D[x, y, z].SetActive(!koma_on_board3D[x, y, z].activeSelf);
                        if (koma_on_board2D[x, y, z] != null) koma_on_board2D[x, y, z].SetActive(!koma_on_board2D[x, y, z].activeSelf);
                    }
                }
            }

            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z <= zLength; z++)
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
                prevCameraPos = cameraMover.MainCameraTransformPosition;
                cameraMover.MainCamera2DSetting(new Vector3(c, 12, c), new Vector3(c, 0, c));
            }
            else cameraMover.MainCameraTransformPosition = prevCameraPos;

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
                        if (active3D & !active2D)
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
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        gridButton2D[x, y, z].enabled = !gridButton2D[x, y, z].enabled;
                    }
                }
            }
        }

        private void ChangeTurn() //手番を終えたプレイヤーの持ち駒を選択できなくする 
        {
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

        private bool CheckBound(Vector3Int vec) //座標の境界チェック
        {
            return (0 <= vec.x && vec.x < xLength) && (0 <= vec.y && vec.y < yLength) && (0 <= vec.z && vec.z < zLength);
        }

        private bool CheckLegal(int k, Vector3Int from, Vector3Int to)
        {
            bool ret = true;
            int[,,] work = new int[xLength, yLength, zLength];
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        work[x, y, z] = boardstate[x, y, z];
                    }
                }
            }
            //詰みのチェックをする前に到達可能範囲の再計算を行う必要あり
            int temp = work[to.x, to.y, to.z];
            if (from.x == -1)
            {
                work[to.x, to.y, to.z] = k;
            }
            else
            {
                work[to.x, to.y, to.z] = work[from.x, from.y, from.z];
                work[from.x, from.y, from.z] = 0;
            }
            Debug.Log(k + " from " + new Vector3Int(from.x, from.y, from.z) + " to " + new Vector3Int(to.x, to.y, to.z));
            CalcReachableRange(work);
            if (from.x == -1)
            {
                if (Math.Abs(k) == 1 && CheckTsumi())
                {
                    Debug.Log("打歩詰めです");
                    ret = false;
                }
            }

            Vector3Int nowOu;
            if (turn == 1)
            {
                //自分の王を見つける
                //その座標に相手が到達できるならダメ（自殺手の禁止）
                nowOu = myOuPos;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dz = -1; dz <= 1; dz++)
                        {
                            Vector3Int pos = nowOu + new Vector3Int(dx, dy, dz);
                            if (CheckBound(pos) && work[pos.x, pos.y, pos.z] == 8 && opReachablePieces[pos.x, pos.y, pos.z] > 0)
                            {
                                Debug.Log("自殺手です");
                                for (int x = 0; x < xLength; x++)
                                {
                                    for (int y = 0; y < yLength; y++)
                                    {
                                        for (int z = 0; z < zLength; z++)
                                        {
                                            if (boardstate[x, y, z] < 0 && CheckMovable_Specific(work, boardstate[x, y, z], -1, new Vector3Int(x, y, z), new Vector3Int(pos.x, pos.y, pos.z)))
                                            {
                                                Debug.Log("Piece " + new Vector3Int(x, y, z) + " can reach Ou");
                                            }
                                        }
                                    }
                                }
                                ret = false;
                            }
                        }
                    }
                }
            }
            else
            {
                nowOu = opOuPos;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dz = -1; dz <= 1; dz++)
                        {
                            Vector3Int pos = nowOu + new Vector3Int(dx, dy, dz);
                            if (CheckBound(pos) && work[pos.x, pos.y, pos.z] == -9 && myReachablePieces[pos.x, pos.y, pos.z] > 0)
                            {
                                Debug.Log("自殺手です");
                                ret = false;
                            }
                        }
                    }
                }
            }

            if (from.x != -1) work[from.x, from.y, from.z] = work[to.x, to.y, to.z];
            work[to.x, to.y, to.z] = temp;
            CalcReachableRange(work); //原状復帰

            return ret;
        }

        private bool CheckMovable(int[,,] work, Vector3Int start, Vector3Int goal, bool keima = false) //startからgoalまでに遮蔽する駒はないか
        {
            if (!keima)
            {
                int max = Math.Max(Math.Abs(goal.x - start.x), Math.Max(Math.Abs(goal.y - start.y), Math.Abs(goal.z - start.z)));
                int dx = (goal.x - start.x) / max, dy = (goal.y - start.y) / max, dz = (goal.z - start.z) / max;
                int posx = start.x + dx, posy = start.y + dy, posz = start.z + dz;
                while (posx != goal.x || posy != goal.y || posz != goal.z)
                {
                    if (work[posx, posy, posz] != 0) return false;
                    posx += dx; posy += dy; posz += dz;
                }
            }
            //ここまでで「駒が重なってもok」とした. 
            return work[start.x, start.y, start.z] * work[goal.x, goal.y, goal.z] <= 0; //自分の駒と重なるのはNG
        }

        private bool CheckMovable_Specific(int[,,] work, int k, int owner, Vector3Int start, Vector3Int goal) //startにあるownerが所有する特定の駒kはgoalに移動できるか
        {
            List<Vector3Int> komaMoveList = Koma.getKomaMove(k);
            foreach (Vector3Int komaMove in komaMoveList)
            {
                if (CalcNextPos(start, komaMove, owner) == goal)
                {
                    if (Math.Abs(k) == 3) return CheckMovable(work, start, goal, true);
                    else return CheckMovable(work, start, goal);
                }
            }
            return false;
        }

        private void ChooseMochigoma(GameObject komainst, int idx) //idx: 相手の桂馬なら-3, 自分の歩なら1
        {
            if (!mouseDetectable) return;
            if (komainst.transform.GetChild(0).transform.GetChild(2).gameObject.activeSelf)
            {
                UnActivateChoosingGrid();
                return;
            }
            UnActivateChoosingGrid();
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
                    {
                        if ((idx == -3 && z <= 1) || (idx == 3 && z >= 7)) continue; //桂馬
                        else if (boardstate[x, y, z] == 0)
                        {
                            if (is3D) movableGrid3D[x, y, z].SetActive(true);
                            else movableGrid2D[x, y, z].SetActive(true);
                        }
                    }
                }
            }

            komainst.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
        }

        //持ち駒を生成して整列するメソッド
        private void MochigomaGenerate(int kind)
        {
            //持ち駒のインスタンスやボタン, オレンジのタイル等の設定
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


            if (turn == 1)
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

            //挿入ソートにより正しい順番に並び替える
            if (turn == 1)
            {
                int target = turn * kind;
                int ins = -1;
                for (int i = 0; i < myMochigomaIdx.Count - 1; i++)
                {
                    if (target <= myMochigomaIdx[i]) { ins = i; break; }
                }
                if (ins != -1)
                {
                    for (int i = myMochigomaIdx.Count - 2; i >= ins; i--)
                    {
                        Debug.Log(i);
                        //Vector3 pos = myMochigomaInstance[i + 1].transform.localPosition; //例えば銀がi, 桂馬がi+1のとき: まずは桂馬の位置を記録
                        GameObject inst = myMochigomaInstance[i + 1]; //桂馬のインスタンスも記憶
                        int idx = myMochigomaIdx[i + 1];
                        Button bt = myMochigomaButton[i + 1];
                        GameObject bx = myMochigomaBox[i + 1];

                        //myMochigomaInstance[i + 1].transform.localPosition = myMochigomaInstance[i].transform.localPosition; //桂馬の位置を銀の位置と変える
                        myMochigomaInstance[i + 1] = myMochigomaInstance[i]; //i+1の参照先を銀に変更
                        myMochigomaIdx[i + 1] = myMochigomaIdx[i];
                        myMochigomaButton[i + 1] = myMochigomaButton[i];
                        myMochigomaBox[i + 1] = myMochigomaBox[i];
                        //myMochigomaInstance[i].transform.localPosition = pos; //iはまだ銀を参照してる. 銀の位置を変更
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

                        //opMochigomaInstance[i + 1].transform.localPosition = opMochigomaInstance[i].transform.localPosition;
                        opMochigomaInstance[i + 1] = opMochigomaInstance[i];
                        opMochigomaIdx[i + 1] = opMochigomaIdx[i];
                        opMochigomaButton[i + 1] = opMochigomaButton[i];
                        opMochigomaBox[i + 1] = opMochigomaBox[i];
                        //opMochigomaInstance[i].transform.localPosition = pos;
                        opMochigomaInstance[i] = inst;
                        opMochigomaIdx[i] = idx;
                        opMochigomaButton[i] = bt;
                        opMochigomaBox[i] = bx;
                    }
                }
            }

            //駒の再配置
            MochigomaRelocate();
        }

        //持ち駒の一つを盤に置いたときに再整列するメソッド
        private void MochigomaRemove(int idx)
        {
            Vector3 pos;
            if (turn == 1)
            {
                pos = myMochigomaInstance[idx].transform.localPosition;
                myMochigomaBox.RemoveAt(idx);
                myMochigomaButton.RemoveAt(idx);
                myMochigomaIdx.RemoveAt(idx);
                Destroy(myMochigomaInstance[idx]);
                myMochigomaInstance.RemoveAt(idx);
            }
            else
            {
                pos = opMochigomaInstance[idx].transform.localPosition;
                opMochigomaBox.RemoveAt(idx);
                opMochigomaButton.RemoveAt(idx);
                opMochigomaIdx.RemoveAt(idx);
                Destroy(opMochigomaInstance[idx]);
                opMochigomaInstance.RemoveAt(idx);
            }

            //駒の再配置
            MochigomaRelocate();
        }

        private void MochigomaRelocate() //画面右端を超えないように持ち駒の位置調整
        {
            float newScale = 20.0f;
            if (turn == 1)
            {
                if (myMochigomaInstance.Count >= 20) newScale /= myMochigomaInstance.Count;
                else newScale = 1.0f;
                Vector3 pos = new Vector3(-10, 0, 0);
                Vector3 inc = new Vector3(newScale, 0, 0);
                foreach (GameObject inst in myMochigomaInstance)
                {
                    inst.transform.localScale = new Vector3(newScale, newScale, newScale);
                    inst.transform.localPosition = pos;
                    pos += inc;
                }
            }
            else
            {
                if (opMochigomaInstance.Count >= 20) newScale /= opMochigomaInstance.Count;
                else newScale = 1.0f;
                Vector3 pos = new Vector3(10, 0, 0);
                Vector3 inc = new Vector3(-newScale, 0, 0);
                foreach (GameObject inst in opMochigomaInstance)
                {
                    inst.transform.localScale = new Vector3(newScale, newScale, newScale);
                    inst.transform.localPosition = pos;
                    pos += inc;
                }
            }
        }

        private Vector3Int CheckOute() //相手（turn*-1）の王に王手がかかっているかを判定. かかっているなら王手をかけている駒の座標を返す
        {
            if (turn == 1)
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            if (boardstate[x, y, z] > 0)
                            {
                                Vector3Int now = new Vector3Int(x, y, z);
                                if (CheckMovable_Specific(boardstate, boardstate[x, y, z], turn, now, opOuPos)) return now;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            if (boardstate[x, y, z] < 0)
                            {
                                Vector3Int now = new Vector3Int(x, y, z);
                                if (CheckMovable_Specific(boardstate, boardstate[x, y, z], turn, now, myOuPos)) return now;
                            }
                        }
                    }
                }
            }
            return new Vector3Int(-1, -1, -1);
        }
        private Vector3Int CheckOute(int[,,] work, Vector3Int newOuPos) // 相手（turn*-1）の王に王手がかかっているかを判定. かかっているならその座標を返す
        {
            if (turn == 1)
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            if (work[x, y, z] > 0)
                            {
                                Vector3Int now = new Vector3Int(x, y, z);
                                if (CheckMovable_Specific(work, work[x, y, z], turn, now, newOuPos)) return now;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            if (work[x, y, z] < 0)
                            {
                                Vector3Int now = new Vector3Int(x, y, z);
                                if (CheckMovable_Specific(work, work[x, y, z], turn, now, newOuPos)) return now;
                            }
                        }
                    }
                }
            }
            return new Vector3Int(-1, -1, -1);
        }

        private bool CheckTsumi()
        {
            //詰みの判定
            //王手をかけている駒を取れるなら問題なし
            //効きのない場所に王が移動できればよし
            //上記を満たすなら張り駒ができなきゃ詰み
            List<Vector3Int> ouMoveList = Koma.getKomaMove(8);
            bool tsumi = true;
            Vector3Int oute = CheckOute();
            if (oute.x != -1) Debug.Log("王手");
            if (oute.x != -1)
            {
                //作業用配列workの用意
                int[,,] work = new int[xLength, yLength, zLength];
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        for (int z = 0; z < zLength; z++)
                        {
                            work[x, y, z] = boardstate[x, y, z];
                        }
                    }
                }
                if (turn == 1)
                {
                    Vector3Int diff = oute - opOuPos; //王と王手をかけている駒の座標の差
                    if (opReachablePieces[oute.x, oute.y, oute.z] > 0) //相手の駒で王手をかけてる駒を取れる駒が存在する場合
                    {
                        /*
                        //王で取りに行った場合は取りに行った場所に効いている駒が無ければセーフ
                        if (Math.Abs(diff.x) <= 1 && Math.Abs(diff.y) <= 1 && Math.Abs(diff.z) <= 1)
                        {
                            int temp = work[oute.x, oute.y, oute.z];
                            work[oute.x, oute.y, oute.z] = work[opOuPos.x, opOuPos.y, opOuPos.z];
                            work[opOuPos.x, opOuPos.y, opOuPos.z] = 0;
                            Debug.Log("王手の駒: " + oute);
                            if(CheckOute(work, oute).x == -1)
                            {
                                tsumi = false;
                                Debug.Log("王が取っても生き残れるよ");
                            }
                            work[opOuPos.x, opOuPos.y, opOuPos.z] = work[oute.x, oute.y, oute.z];
                            work[oute.x, oute.y, oute.z] = temp;
                        }
                        */

                        //取った結果、王が取られなければセーフ
                        for (int x = 0; x < xLength; x++)
                        {
                            for (int y = 0; y < yLength; y++)
                            {
                                for (int z = 0; z < zLength; z++)
                                {
                                    if (work[x, y, z] < 0 && CheckMovable_Specific(work, work[x, y, z], turn, new Vector3Int(x, y, z), oute))
                                    {
                                        //自分が王手をかけている駒を取れる相手の駒全てに対し, 実際に王手の駒を取ると王が取られてしまわないかチェック
                                        bool isOu = (Math.Abs(work[x, y, z]) == 8 || Math.Abs(work[x, y, z]) == 9) ? true : false;
                                        int temp = work[oute.x, oute.y, oute.z];
                                        work[oute.x, oute.y, oute.z] = work[x, y, z];
                                        work[x, y, z] = 0;
                                        if (isOu && CheckOute(work, oute).x == -1)
                                        {
                                            Debug.Log("王が取っても生き残れるよ");
                                            tsumi = false;
                                        }
                                        else if (!isOu && CheckOute(work, opOuPos).x == -1)
                                        {
                                            Debug.Log("王以外のもので取れるよ");
                                            tsumi = false;
                                        }
                                        work[x, y, z] = work[oute.x, oute.y, oute.z];
                                        work[oute.x, oute.y, oute.z] = temp;
                                    }
                                }
                            }
                        }
                    }
                    //王が他に効き駒のないマスに移動できる場合
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dz = -1; dz <= 1; dz++)
                            {
                                Vector3Int next = opOuPos + new Vector3Int(dx, dy, dz);
                                if (dx == 0 && dy == 0 && dz == 0) continue;
                                if (next != oute && CheckBound(next) && work[next.x, next.y, next.z] == 0 && myReachablePieces[next.x, next.y, next.z] == 0)
                                {
                                    Debug.Log("逃げ場があるよ");
                                    tsumi = false;
                                }
                            }
                        }
                    }
                    if (work[oute.x, oute.y, oute.z] != 3)
                    {
                        //貼り駒ができるかどうか
                        int mx = Math.Max(Math.Abs(diff.x), Math.Max(Math.Abs(diff.y), Math.Abs(diff.z)));
                        Vector3Int inc = diff / mx;
                        for (Vector3Int loc = opOuPos + inc; loc != oute; loc += inc)
                        {
                            for (int x = 0; x < xLength; x++)
                            {
                                for (int y = 0; y < yLength; y++)
                                {
                                    for (int z = 0; z < zLength; z++)
                                    {
                                        if (work[x, y, z] < 0 && CheckMovable_Specific(work, work[x, y, z], -turn, new Vector3Int(x, y, z), loc))
                                        {
                                            int temp = work[loc.x, loc.y, loc.z];
                                            work[loc.x, loc.y, loc.z] = work[x, y, z];
                                            work[x, y, z] = 0;
                                            if (CheckOute(work, opOuPos).x == -1)
                                            {
                                                Debug.Log("張り駒ができるよ");
                                                tsumi = false;
                                            }
                                            work[x, y, z] = work[loc.x, loc.y, loc.z];
                                            work[loc.x, loc.y, loc.z] = temp;
                                        }
                                    }
                                }
                            }
                            foreach (int opMochigoma in opMochigomaIdx)
                            {
                                //locに置けるかどうかの判定（二歩は本来はダメになると思うので後で実装）
                                //if(puttable(opMochigoma, loc)) tsumi = false
                                Debug.Log("持ち駒で張り駒ができるよ");
                                tsumi = false;
                            }
                        }
                    }
                }
                else
                {
                    Vector3Int diff = oute - myOuPos;
                    if (myReachablePieces[oute.x, oute.y, oute.z] > 0)
                    {
                        /*
                        //王で取りに行った場合は取りに行った場所に効いている駒が無ければセーフ
                        if (Math.Abs(diff.x) <= 1 && Math.Abs(diff.y) <= 1 && Math.Abs(diff.z) <= 1)
                        {
                            int temp = work[oute.x, oute.y, oute.z];
                            work[oute.x, oute.y, oute.z] = work[myOuPos.x, myOuPos.y, myOuPos.z];
                            work[myOuPos.x, myOuPos.y, myOuPos.z] = 0;
                            if (CheckOute(work, oute).x == -1)
                            {
                                Debug.Log("王が取っても生き残れるよ");
                                tsumi = false;
                            }
                            work[myOuPos.x, myOuPos.y, myOuPos.z] = work[oute.x, oute.y, oute.z];
                            work[oute.x, oute.y, oute.z] = temp;
                        }
                        */

                        //王じゃないもので取りに行けて、その結果取られなければセーフ
                        for (int x = 0; x < xLength; x++)
                        {
                            for (int y = 0; y < yLength; y++)
                            {
                                for (int z = 0; z < zLength; z++)
                                {
                                    if (work[x, y, z] > 0 && CheckMovable_Specific(work, work[x, y, z], turn, new Vector3Int(x, y, z), oute))
                                    {
                                        //自分が王手をかけている駒を取れる相手の駒全てに対し, 実際に王手の駒を取ると王が取られてしまわないかチェック
                                        bool isOu = (8 <= Math.Abs(work[x, y, z]) && Math.Abs(work[x, y, z]) <= 9) ? true : false;
                                        int temp = work[oute.x, oute.y, oute.z];
                                        work[oute.x, oute.y, oute.z] = work[x, y, z];
                                        work[x, y, z] = 0;
                                        if (isOu && CheckOute(work, oute).x == -1)
                                        {
                                            Debug.Log("王が取っても生き残れるよ");
                                            tsumi = false;
                                        }
                                        if (CheckOute(work, myOuPos).x == -1)
                                        {
                                            Debug.Log("王以外のもので取れるよ");
                                            tsumi = false;
                                        }
                                        work[x, y, z] = work[oute.x, oute.y, oute.z];
                                        work[oute.x, oute.y, oute.z] = temp;
                                    }
                                }
                            }
                        }
                    }
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dz = -1; dz <= 1; dz++)
                            {
                                Vector3Int next = myOuPos + new Vector3Int(dx, dy, dz);
                                if (dx == 0 && dy == 0 && dz == 0) continue;
                                if (next != oute && CheckBound(next) && work[next.x, next.y, next.z] == 0 && opReachablePieces[next.x, next.y, next.z] == 0)
                                {
                                    Debug.Log("逃げ場があるよ");
                                    tsumi = false;
                                }
                            }
                        }
                    }
                    if (work[oute.x, oute.y, oute.z] != -3)
                    {
                        //貼り駒ができるかどうか
                        int mx = Math.Max(Math.Abs(diff.x), Math.Max(Math.Abs(diff.y), Math.Abs(diff.z)));
                        Vector3Int inc = diff / mx;

                        for (Vector3Int loc = myOuPos + inc; loc != oute; loc += inc)
                        {
                            for (int x = 0; x < xLength; x++)
                            {
                                for (int y = 0; y < yLength; y++)
                                {
                                    for (int z = 0; z < zLength; z++)
                                    {
                                        if (work[x, y, z] > 0 && CheckMovable_Specific(work, work[x, y, z], turn, new Vector3Int(x, y, z), loc))
                                        {
                                            int temp = work[loc.x, loc.y, loc.z];
                                            work[loc.x, loc.y, loc.z] = work[x, y, z];
                                            work[x, y, z] = 0;
                                            if (CheckOute(work, myOuPos).x == -1)
                                            {
                                                Debug.Log("張り駒ができるよ");
                                                tsumi = false;
                                            }
                                            work[x, y, z] = work[loc.x, loc.y, loc.z];
                                            work[loc.x, loc.y, loc.z] = temp;
                                        }
                                        else if (work[x, y, z] == 7) Debug.Log(work[x, y, z] + " at " + new Vector3Int(x, y, z) + "cannot reach " + loc);
                                    }
                                }
                            }
                            foreach (int myMochigoma in myMochigomaIdx)
                            {
                                //locに置けるかどうかの判定（二歩は本来はダメになると思うので後で実装）
                                //if(puttable(myMochigoma, loc)) tsumi = false;
                                Debug.Log("持ち駒で張り駒ができるよ");
                                tsumi = false;
                            }
                        }
                    }
                }
            }
            else tsumi = false; //王手がかかっていない
            return tsumi;
        }

        private int GameSet(bool hansoku = false) //手番終了時に毎回呼び出される
        {
            if (hansoku) return -turn;
            else return (CheckTsumi()) ? turn : 0;
        }

        public void ReverseView()
        {
            //camera.transform.eulerAngles += new Vector3(0, 180, 0);
        }

        public void UnActivateChoosingGrid()
        {
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    for (int z = 0; z < zLength; z++)
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
