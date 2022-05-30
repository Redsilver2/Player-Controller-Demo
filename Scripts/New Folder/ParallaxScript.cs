using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Player.States;


public class ParallaxScript : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 3f;
    [SerializeField] Transform Pos;

    public float Offset = 10f;
    public float Threshold = 15;

    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        transform.Translate(Vector3.left * Camera.main.velocity.x * MoveSpeed * Time.fixedDeltaTime);

        Pos.position = new Vector3(PlayerController.instance.transform.position.x, Pos.position.y);
        transform.position = new Vector3(transform.position.x, transform.position.y);
        Debug.Log(dist);

        if(dist > Threshold)
        {
            transform.position = new Vector3(Pos.transform.position.x , transform.position.y);
        }

    }
}
