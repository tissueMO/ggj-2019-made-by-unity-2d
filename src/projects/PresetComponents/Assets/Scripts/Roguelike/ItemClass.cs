using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnUse(int id);

public class ItemClass : MonoBehaviour
{
    public int ID { get; set; }

    public OnUse OnUse;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        OnUse(ID);
    }
}

