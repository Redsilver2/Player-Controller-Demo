using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1f, Time.deltaTime, 0f);
    }
}
