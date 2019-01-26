using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StageObjectBase : MonoBehaviour
{
    SpriteRenderer mSpriteRenderer;

    private void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseOver()
    {
        mSpriteRenderer.color = new Color(200,200,200,255);
    }

    abstract protected void OnMouseDown();
}
