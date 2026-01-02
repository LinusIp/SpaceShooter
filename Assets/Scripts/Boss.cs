using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
