using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{

    [SerializeField] float Speed = 0.002f;
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, Speed * Time.realtimeSinceStartup);
        Debug.Log(Speed * Time.realtimeSinceStartup);
    }
}
