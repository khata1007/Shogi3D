using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PvP993
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
            game.UnActivateChoosingGrid();
        }

        public void OnDimensionButtonClick()
        {
            if (game.MouseDetectable)
            {
                game.ChangeDimension();
                Text target = changeDimensionButton.transform.GetChild(0).GetComponent<Text>();
                if (target.text[0] == '3') target.text = "2D";
                else target.text = "3D";
            }
        }

        public void OnNariButtonClick(int n)
        {
            Koma.Nariflg = n;
        }

        public void OnReverseButtonClick()
        {
            game.ReverseView();
        }
        
    }
}
