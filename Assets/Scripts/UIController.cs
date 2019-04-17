using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HexFallDemo
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private RectTransform endPanel;
        private GridManager m_GridManager;
        // Start is called before the first frame update

        private void Start()
        {
            m_GridManager = GameObject.FindGameObjectWithTag(Tags.GridManager).GetComponent<GridManager>();
            m_GridManager.onGameFinishedEvent += onGameFinishedEvent;
        }

        private void onGameFinishedEvent()
        {
            print("here");
            endPanel.DOAnchorPos(new Vector2(0, 0), 0.2f).SetDelay(0.6f);
        }

        public void DisplayScore(int score)
        {
            _scoreText.text = score.ToString();
        }
        public void OnRestartClick()
        {
            SceneManager.LoadScene(0);
        }
        public void OnMenuClick(CanvasGroup panel)
        {
            if (panel.gameObject.activeInHierarchy)
            {
                CustomInputManager.isEnabled = true;

                panel.DOFade(0, 0.5f).OnComplete(() =>
                {
                    panel.gameObject.SetActive(false);
                });
            }
            else
            {

                CustomInputManager.isEnabled = false;
                panel.gameObject.SetActive(true);
                panel.DOFade(1, 0.5f);
            } 

        }
    }
}
