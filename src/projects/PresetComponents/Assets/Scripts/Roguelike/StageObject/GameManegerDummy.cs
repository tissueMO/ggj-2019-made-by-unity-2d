using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Roguelike;

public class GameManegerDummy : MonoBehaviour
{
    public Player1 Player { get; private set; }
    private static GameManegerDummy Instance;

    public static GameManegerDummy GetInstance()
    {
        return Instance;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player1").GetComponent<Player1>();
        }
        
    }
}
