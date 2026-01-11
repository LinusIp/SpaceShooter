using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _multiDirectionPrefab;

    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private float _duration = 5.0f;
    
    private bool _isTripleShotActive = false;
    private bool _isMultiDirectionActive = false;
    private bool _isShieldActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isHealthActive = false;
    private bool _isNegativePowerUpActive = false;
    [SerializeField]
    private GameObject _shieldVizualizer;
    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;
    public Color normal_color = Color.white;
    public Color boost_color = new Color(233f / 255f, 142f / 255f, 25f / 255f);
    [SerializeField]
    private SpriteRenderer _thrustRenderer;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;
    private int _shieldUsage;
    private Color _firstHit = new Color(255f / 255f, 255f / 255f, 255f / 255f, 190f / 255f);
    private Color _secondHit = new Color(255f / 255f, 255f / 255f, 255f / 255f, 150f / 255f);
    private int _ammoAmount = 15;
    private Animator _anim;
    public Animator _animator;



    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if( _uiManager == null)
        {
            Debug.Log("The UIManager is NULL.");
        }
        if(_audioSource == null)
        {
            Debug.Log("The AudioSource on the Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire )
        {
            FireLaser();

        }

    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        SpeedBooster();

        transform.Translate(direction * _speed * Time.deltaTime);


        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    private void SpeedBooster()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = _speed * 2;
            _thrustRenderer.color = boost_color;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _speed / 2;
            _thrustRenderer.color = normal_color;
        }
    }

    void FireLaser()
    {
        if (_ammoAmount > 0)
        {
            Vector3 distance = transform.position + new Vector3(0, 0.8f, 0);
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position , Quaternion.identity);
            }
            if(_isMultiDirectionActive == true)
            {
                Instantiate(_multiDirectionPrefab, transform.position + new Vector3(-1.69f, 1.1f, 0), Quaternion.identity);
            }
            else
            {
                    
                Instantiate(_laserPrefab, distance, Quaternion.identity);

            }
            _audioSource.Play();
            _ammoAmount--;
        }
        if(_isTripleShotActive == true)
        {
            _ammoAmount = 15;
        }
        else
        {
            _uiManager.AmmoFlicker();
        }
        
        
    }
    public void Damage()
    {

        
        if (_isShieldActive == true)
        {
            _shieldUsage++;
            if (_shieldUsage == 1)
            {
                _shieldRenderer.color = _firstHit;
            }
            if( _shieldUsage == 2)
            {
                _shieldRenderer.color = _secondHit;
            }
            if( (_shieldUsage == 3))
            {
                _isShieldActive = false;
                _shieldVizualizer.SetActive(false);
            }
            return;

        }
        _lives--;

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if( _lives == 1)
        {
            _leftEngine.SetActive(true);
        }

            _uiManager.UpdateLives(_lives);

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }


    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());

    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_duration);
        _isTripleShotActive = false;
    }

    public void MultiDirectionShotActive()
    {
        _isMultiDirectionActive = true;
        StartCoroutine(MultiDirectionPowerDownRoutine());

    }

    IEnumerator MultiDirectionPowerDownRoutine()
    {
        yield return new WaitForSeconds(_duration);
        _isMultiDirectionActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void NegativePowerUp()
    {
        _isNegativePowerUpActive = true;
        _speed -= _speedMultiplier;
        StartCoroutine(NegativePowerUpPowerDownRoutine());
    }

    IEnumerator NegativePowerUpPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isNegativePowerUpActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVizualizer.SetActive(true);
    }

    public void HealthActive()
    {
        _isHealthActive = true;
        _lives += 1;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);

        if(_score % 100 == 0)
        {
            MultiDirectionShotActive();
        }
    }

    public void DamageAnim()
    {
        StartCoroutine(DamageAnimRoutine());
    }

    IEnumerator DamageAnimRoutine()
    {
        _animator.SetTrigger("OnPlayerDamage");
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger("AfterPlayerDamage");
    }
}
