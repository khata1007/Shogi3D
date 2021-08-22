using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PvP553
{

    public class Board : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        private Color[] frameColor = new Color[]
        {
            new Color(75/255.0f, 63/255.0f, 48/255.0f),
            new Color(95/255.0f, 84/255.0f, 60/255.0f),
            new Color(125/255.0f, 109/255.0f, 90/255.0f),
            //new Color(142/255.0f, 129/255.0f, 106/255.0f),
        };
        //amezaki


        [SerializeField, Range(0.0f, 0.03f)] private float frameWidth3D = 0.01f;
        [SerializeField, Range(0.0f, 0.1f)] private float frameWidth2D = 0.05f;

        private float boardScale2D;
        private float boardScale3D;
        public GameObject orangeBoardPrefab3D;
        public GameObject greenBoardPrefab3D;
        public GameObject framePrefab;

        public GameObject orangeBoardPrefab2D;
        public GameObject greenBoardPrefab2D;

        public GameObject gridButtonPrefab;
        public GameObject debugButtonPrefab;

        public GameObject movableGridPrefab3D;
        public GameObject movableGridPrefab2D;

        public GameObject boardImage;
        public GameObject boardMatPrefab;

        public Game game;
        // Start is called before the first frame update
        void Start()
        {
            Transform list = this.transform.GetChild(6).transform;
            for (int y = 0; y < yLength; y++)
            {
                float offset = (1 - y) * (zLength + 1) * boardScale2D;
                for (int z = 0; z < zLength; z++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        //プレハブからボタンを生成
                        //GameObject listButton = Instantiate(gridButtonPrefab) as GameObject;
                        //Vertical Layout Group の子にする
                        //listButton.transform.SetParent(list, false);
                        GameObject listButton = Instantiate(gridButtonPrefab, list);
                        //position 設定
                        listButton.transform.localPosition = new Vector3((x - (xLength - 1) / 2) * boardScale2D, 0, (z - (zLength - 1) / 2) * boardScale2D - offset);
                        listButton.transform.localScale = new Vector3(boardScale2D, boardScale2D, 1);
                        listButton.transform.Rotate(new Vector3(-90, 0, 0));
                        //以下、追加---------
                        int n = x * 100 + y * 10 + z;
                        //引数に何番目のボタンかを渡す
                        Button b = listButton.GetComponent<Button>();
                        b.onClick.AddListener(() => game.ChooseGrid(n));
                        b.enabled = true; //初期状態はtrue
                        game.SetGridButton2D(b, x, y, z);
                    }
                }
            }
            //MeshRenderer mr = this.transform.GetChild(7).gameObject.transform.GetComponent<MeshRenderer>();
            //mr.material.color = new Color(0, 0, 0, 0.0f);
        }

        public void CreateBoard(GameObject[,,] ob3D, GameObject[,,] gb3D, GameObject[,,] ob2D, GameObject[,,] gb2D, GameObject[,,] mg3D, GameObject[,,] mg2D)
        {
            Transform orangeBoardTransform3D = this.transform.GetChild(0).gameObject.transform;
            Transform greenBoardTransform3D = this.transform.GetChild(1).gameObject.transform;
            Transform movableGridTransform3D = this.transform.GetChild(7).gameObject.transform;
            orangeBoardTransform3D.localScale = new Vector3(boardScale3D, boardScale3D, boardScale3D);
            greenBoardTransform3D.localScale = new Vector3(boardScale3D, boardScale3D, boardScale3D);
            movableGridTransform3D.localScale = new Vector3(boardScale3D, boardScale3D, boardScale3D);

            float mgZoom = 0.8f;
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        ob3D[x, y, z] = Instantiate(orangeBoardPrefab3D, orangeBoardTransform3D);
                        ob3D[x, y, z].transform.localPosition = new Vector3(x, y - 0.5f, z);
                        ob3D[x, y, z].layer = ob3D[x, y, z].transform.parent.gameObject.layer;
                        ob3D[x, y, z].transform.localScale = new Vector3(1.0f, 0.3f, 1.0f);
                        ob3D[x, y, z].SetActive(false);
                        gb3D[x, y, z] = Instantiate(greenBoardPrefab3D, greenBoardTransform3D);
                        gb3D[x, y, z].transform.localPosition = new Vector3(x, y - 0.5f, z);
                        gb3D[x, y, z].layer = gb3D[x, y, z].transform.parent.gameObject.layer;
                        gb3D[x, y, z].transform.localScale = new Vector3(1.0f, 0.3f, 1.0f);
                        gb3D[x, y, z].SetActive(false);
                        mg3D[x, y, z] = Instantiate(movableGridPrefab3D, movableGridTransform3D);
                        mg3D[x, y, z].transform.localPosition = new Vector3(x, y - 0.5f, z);
                        mg3D[x, y, z].layer = mg3D[x, y, z].transform.parent.gameObject.layer;
                        mg3D[x, y, z].transform.localScale = new Vector3(0.8f, 0.001f, 0.8f); //赤丸は少し小さめに
                        mg3D[x, y, z].SetActive(false);

                    }
                }
            }

            Transform orangeBoardTransform2D = this.transform.GetChild(2).gameObject.transform;
            Transform greenBoardTransform2D = this.transform.GetChild(3).gameObject.transform;
            Transform movableGridTransform2D = this.transform.GetChild(8).gameObject.transform;
            //orangeBoardTransform2D.localScale = new Vector3(1, 0.f, 1);
            //greenBoardTransform2D.localScale = new Vector3(1, 0.01f, 1);
            //movableGridTransform2D.localScale = new Vector3(1, 0.01f, 1);
            for (int y = 0; y < yLength; y++)
            {
                float offset = (1 - y) * (zLength + 1) * boardScale2D;
                for (int z = 0; z < zLength; z++)
                {
                    for (int x = 0; x < xLength; x++)
                    {
                        ob2D[x, y, z] = Instantiate(orangeBoardPrefab2D, orangeBoardTransform2D);
                        ob2D[x, y, z].transform.localPosition = new Vector3((x - (xLength - 1) / 2) * boardScale2D, 0, (z - (zLength - 1) / 2) * boardScale2D - offset);
                        ob2D[x, y, z].transform.localScale = new Vector3(boardScale2D, 0.01f, boardScale2D);
                        ob2D[x, y, z].layer = ob2D[x, y, z].transform.parent.gameObject.layer;
                        ob2D[x, y, z].SetActive(false);
                        gb2D[x, y, z] = Instantiate(greenBoardPrefab2D, greenBoardTransform2D);
                        gb2D[x, y, z].transform.localPosition = new Vector3((x - (xLength - 1) / 2) * boardScale2D, 0, (z - (zLength - 1) / 2) * boardScale2D - offset);
                        gb2D[x, y, z].transform.localScale = new Vector3(boardScale2D, 0.01f, boardScale2D);
                        gb2D[x, y, z].layer = gb2D[x, y, z].transform.parent.gameObject.layer;
                        gb2D[x, y, z].SetActive(false);
                        mg2D[x, y, z] = Instantiate(movableGridPrefab2D, movableGridTransform2D);
                        mg2D[x, y, z].transform.localPosition = new Vector3((x - (xLength - 1) / 2) * boardScale2D, -0.005f, (z - (zLength - 1) / 2) * boardScale2D - offset);
                        mg2D[x, y, z].transform.localScale = new Vector3(boardScale2D * mgZoom, 0.001f, boardScale2D * mgZoom);
                        mg2D[x, y, z].layer = mg2D[x, y, z].transform.parent.gameObject.layer;
                        mg2D[x, y, z].SetActive(false);
                    }
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                GameObject g = Instantiate(boardMatPrefab, boardImage.transform);
                g.transform.localPosition = new Vector3(0, -0.01f, i * (zLength + 1) * boardScale2D);
                g.transform.localScale = new Vector3(boardScale2D * xLength, 0.001f, boardScale2D * zLength);
                g.layer = g.transform.parent.gameObject.layer;
            }
            Debug.Log(ob2D[0, 0, 0].transform.position);
        }

        public void CreateFrame(GameObject[,] fx3D, GameObject[,] fz3D, GameObject[,] fx2D, GameObject[,] fz2D)
        {
            float xCenter = (xLength - 1.0f) / 2.0f;
            float zCenter = (zLength - 1.0f) / 2.0f;
            Transform frameX3DTransform = this.transform.GetChild(4).gameObject.transform;
            Transform frameZ3DTransform = this.transform.GetChild(5).gameObject.transform;
            Transform frameX2DTransform = this.transform.GetChild(9).gameObject.transform;
            Transform frameZ2DTransform = this.transform.GetChild(10).gameObject.transform;
            frameX3DTransform.localScale = new Vector3(boardScale3D, boardScale3D, boardScale3D);
            frameZ3DTransform.localScale = new Vector3(boardScale3D, boardScale3D, boardScale3D);

            // ---------------- 3D盤面のフレーム作成 ------------------ //

            framePrefab.transform.localScale = new Vector3(xLength, frameWidth3D, frameWidth3D);
            for (int y = 0; y < yLength; y++)
            {
                for (int z = 0; z <= zLength; z++)
                {
                    fx3D[y, z] = Instantiate(framePrefab, frameX3DTransform);
                    fx3D[y, z].GetComponent<Renderer>().material.color = frameColor[y];
                    fx3D[y, z].transform.localPosition = new Vector3(xCenter, y - 0.5f, z - 0.5f);
                    fx3D[y, z].layer = fx3D[y, z].transform.parent.gameObject.layer;
                }
            }

            framePrefab.transform.localScale = new Vector3(frameWidth3D, frameWidth3D, zLength);
            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x <= xLength; x++)
                {
                    fz3D[y, x] = Instantiate(framePrefab, frameZ3DTransform);
                    fz3D[y, x].GetComponent<Renderer>().material.color = frameColor[y];
                    fz3D[y, x].transform.localPosition = new Vector3(x - 0.5f, y - 0.5f, zCenter);
                    fz3D[y, x].layer = fz3D[y, x].transform.parent.gameObject.layer;
                }
            }

            // ---------------- 2D盤面のフレーム作成 ------------------ //
            xCenter *= boardScale2D;
            zCenter *= boardScale2D;
            framePrefab.transform.localScale = new Vector3(boardScale2D * xLength, frameWidth2D, frameWidth2D);
            for (int y = 0; y < yLength; y++)
            {
                float offset = (1 - y) * (xLength + 1) * boardScale2D;
                for (int z = 0; z <= zLength; z++)
                {
                    fx2D[y, z] = Instantiate(framePrefab, frameX2DTransform);
                    fx2D[y, z].GetComponent<Renderer>().material.color = frameColor[2];
                    fx2D[y, z].transform.localPosition = new Vector3(0, 0, (z - (zLength - 1) / 2) * boardScale2D - boardScale2D / 2 + offset);
                    fx2D[y, z].layer = fx2D[y, z].transform.parent.gameObject.layer;
                    fx2D[y, z].SetActive(true);
                }
            }

            framePrefab.transform.localScale = new Vector3(frameWidth2D, frameWidth2D, boardScale2D * zLength);
            for (int y = 0; y < yLength; y++)
            {
                float offset = (1 - y) * (xLength + 1) * boardScale2D;
                for (int x = 0; x <= xLength; x++)
                {
                    fz2D[y, x] = Instantiate(framePrefab, frameZ2DTransform);
                    fz2D[y, x].GetComponent<Renderer>().material.color = frameColor[2];
                    fz2D[y, x].transform.localPosition = new Vector3((x - (xLength + 1) / 2) * boardScale2D + boardScale2D / 2, 0, offset);
                    fz2D[y, x].layer = fz2D[y, x].transform.parent.gameObject.layer;
                    fz2D[y, x].SetActive(true);
                }
            }
            Debug.Log("2DFrame Created.");
        }

        public float BoardScale2D { set { boardScale2D = value; } }
        public float BoardScale3D { set { boardScale3D = value; } }
    }

}
