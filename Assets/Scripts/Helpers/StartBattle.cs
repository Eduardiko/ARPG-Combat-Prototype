using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattle : MonoBehaviour
{

    public GameObject enemy1;
    public GameObject enemy2;

    private Character enemy1Character;
    private Character enemy2Character;

    // Start is called before the first frame update
    void Start()
    {
        enemy1Character = enemy1.GetComponent<Character>();
        enemy2Character = enemy2.GetComponent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            BeginBattle();
    }

    private void BeginBattle()
    {
        enemy1Character.target = enemy2;
        enemy2Character.target = enemy1;
    }
}
