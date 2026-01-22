using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _comingSpeed = 6.0f;
    [SerializeField] private float _magnetDistance = 6f;

    private Player _player;

    void Start()
    {
        _player = GameObject.Find("Player")?.GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4.5f)
        {
            Destroy(gameObject);
        }

        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if (_player == null) return;

        if (Input.GetKey(KeyCode.C))
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance <= _magnetDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _player.transform.position,
                    _comingSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            switch (_powerupID)
            {
                case 0: _player.TripleShotActive(); break;
                case 1: _player.SpeedBoostActive(); break;
                case 2: _player.ShieldActive(); break;
                case 3: _player.HealthActive(); break;
                case 4: _player.NegativePowerUp(); break;
            }

            Destroy(gameObject);
        }
    }

  
    public void Damage()
    {
        Destroy(gameObject);
    }
}