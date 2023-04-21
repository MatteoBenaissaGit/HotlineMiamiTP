using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MatteoBenaissaLibrary.SingletonClassBase;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<Enemy> _enemiesList = new List<Enemy>();
    [SerializeField] private List<TMP_Text> _uiTextList = new List<TMP_Text>();
    [SerializeField] private TMP_Text _uiText;
    [SerializeField] private List<Image> _uiImagetList = new List<Image>();
    [SerializeField] private Image _background;
    [SerializeField] private List<GameObject> _uiObjectList = new List<GameObject>();

    private void Start()
    {
        _uiText.text = "";
        FadeUI(0);
    }

    private void Update()
    {

        bool enemyIsAlive = false;
        foreach (Enemy enemy in _enemiesList)
        {
            if (enemy.IsDead == false)
            {
                enemyIsAlive = true;
            }
        }
        if (enemyIsAlive == false)
        {
            Win();
        }
    }

    private void Win()
    {
        _uiText.text = "You won !";
        PlayerManager.Instance.CharacterController.CanMove = false;
        FadeUI(1);
    }

    public void Lose()
    {
        _uiText.text = "You lost !";
        PlayerManager.Instance.CharacterController.CanMove = false;
        FadeUI(1);
    }

    private void FadeUI(float value)
    {
        _uiImagetList.ForEach(x => x.DOFade(value,0.5f));
        _uiTextList.ForEach(x => x.DOFade(value,0.5f));
        _uiObjectList.ForEach(x => x.SetActive(value == 1));
        _background.DOFade(value / 2, 0.5f);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
