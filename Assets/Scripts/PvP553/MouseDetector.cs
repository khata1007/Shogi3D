using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PvP553
{
    public class MouseDetector : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength; //オセロ盤の一辺の長さ
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        public Button cancelgridChooseButton;
        public Button changeDimensionButton;
        public Button gridChooseButton;
        public Button reverseButton; //180°視点を反転
        public Button nariYesButton, nariNoButton;
        public Game game;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (game.MouseDetectable)
            {
                if (Input.GetMouseButtonUp(0)) Debug.Log("yeah");
            }
            */
        }

        public void OnCancelGridChooseButtonClick()
        {
            Debug.Log("cancelled");
            game.UnActivateChoosingGrid();
        }

        public void OnNariButtonClick(int n)
        {
            Koma.Nariflg = n;
        }

        public void OnReverseButtonClick()
        {
            game.ReverseView();
        }

        public void OnMattaButtonClick()
        {
            game.Matta();
        }
        public void OnFinishButtonClick()
        {
            //セーブしてシーン遷移
            Debug.Log("FinishButton Clicked");
        }
    }
}
