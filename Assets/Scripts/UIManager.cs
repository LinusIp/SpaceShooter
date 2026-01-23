using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _ammoText;
    [SerializeField]
    private Image _livesIMG;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private TMP_Text _ammoIsEmptyText;
    [SerializeField]
    private TMP_Text _nextWave;
    private int ammo = 15;



    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if( _gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateAmmo(int ammoCount)
    {
        ammo -= ammoCount;
        if(ammo <= 0)
        {
            ammo = 0;
        }
        _ammoText.text = "Ammo: " + ammo.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0 || currentLives >= _liveSprites.Length)
        {
            return;
        }
        _livesIMG.sprite = _liveSprites[currentLives];

        if(currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true );

        StartCoroutine(GameOverFlickerRoutine());
    }

    public void NextWave()
    {
            StartCoroutine(NextWaveFlickerRoutine());
    }

    IEnumerator NextWaveFlickerRoutine()
    {
        while (true)
        {
            _nextWave.text = "Next Wave";
            yield return new WaitForSeconds(0.5f);
            _nextWave.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5F);
            _gameOverText.text = " "; 
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AmmoFlicker()
    {
        StartCoroutine(AmmoIsEmptyRoutine());
    }
    
    IEnumerator AmmoIsEmptyRoutine()
    {
        while (true)
        {
            _ammoIsEmptyText.text = "AMMO IS EMPTY";
            yield return new WaitForSeconds(0.5f);
            _ammoIsEmptyText.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
