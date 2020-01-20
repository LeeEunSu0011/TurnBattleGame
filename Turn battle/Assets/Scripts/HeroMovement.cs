using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    float moveSpeed = 10f;

    void Start()
    {
        transform.position = GameManager.instance.nextHeroPosition;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ);
        GetComponent<Rigidbody>().velocity = movement * moveSpeed;
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EnterTown")
        {
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
            GameManager.instance.nextHeroPosition = col.spawnPoint.transform.position;
            GameManager.instance.sceneToLoad = col.sceneToLoad;
            GameManager.instance.LoadNextScene();
        }

        if (other.tag == "LeaveTown")
        {
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
            GameManager.instance.nextHeroPosition = col.spawnPoint.transform.position;
            GameManager.instance.sceneToLoad = col.sceneToLoad;
            GameManager.instance.LoadNextScene();
        }
    }
}
