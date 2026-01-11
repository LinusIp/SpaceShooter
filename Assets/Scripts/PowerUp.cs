using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;
    private Player _player;
    private float _comingSpeed = 6.0f;
    [SerializeField]
    


    private void Start()
    {
         Player _player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            transform.position = Vector3.Lerp(this.transform.position, _player.transform.position, 3f *  Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();                   
                        break;
                    case 3:
                        player.HealthActive();
                        break;
                    case 4:
                        player.NegativePowerUp();
                        break;
                    default:
                        Debug.Log("Defaul value");
                        break;
                }
            }
            Destroy(this.gameObject);
        }

        if(other.tag == "Laser")
        {
            Destroy(this.gameObject);
        }

    }

    public void Damage()
    {
        Destroy(this.gameObject);
    }
}
