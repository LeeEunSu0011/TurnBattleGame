using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    //Hero
    public GameObject heroCharacher;

    //position
    public Vector3 nextHeroPosition;
    public Vector3 lastHeroPosition;//battle

    //scenes
    public string sceneToLoad;
    public string lastScene;//battle

    void Awake()
    {
        //check if instace exist
        if(instance == null)
        {
            //if not set the instace
            instance = this;
        }
        //if it exist but is not this instace
        else if(instance != this)
        {
            //destroy it
            Destroy(gameObject);
        }
        //set this to be not destroyable
        DontDestroyOnLoad(gameObject);

        if (!GameObject.Find("HeroCharacher"))
        {
            GameObject Hero = Instantiate(heroCharacher,Vector3.zero, Quaternion.identity) as GameObject;
            Hero.name = "HeroCharacher";
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
