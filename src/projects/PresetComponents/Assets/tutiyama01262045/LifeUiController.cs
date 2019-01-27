using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Roguelike;

public class LifeUiController : MonoBehaviour
{
    private Player1 m_Player1;
    private int m_OldHp = 0;  //前のHP
    [SerializeField]
    private Vector3 m_StartPosition;
    [SerializeField]
    private float m_SpaceAmount;
    [SerializeField]
    private GameObject m_LifeObj;
    [SerializeField]
    private Canvas m_Canvas;
    List<GameObject> m_LifeObjcts = new List<GameObject>();

    private bool m_IniFlag = false;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        m_Player1 = GameObject.FindObjectOfType<Player1>();
        m_OldHp = m_Player1.GetHp();

        for(int i = 0; i < m_OldHp; i++)
        {
            Vector3 pos = m_StartPosition;

            pos.x += (m_SpaceAmount * i);

            //lifeObjの生成
            GameObject obj = Instantiate(m_LifeObj, pos, Quaternion.identity);
            obj.transform.parent = m_Canvas.transform;
            m_LifeObjcts.Add(obj);
        }

        m_IniFlag = true;
    }

    private void Start()
    {
       
    }

    private void Update()
    { 
        if(Input.GetKeyDown(KeyCode.R))
        {
            Initialize();
        }

        if(m_IniFlag)
        {
            int hp = m_Player1.GetHp();

            if (m_OldHp != hp)
            {
                if(m_LifeObjcts.Count < hp)
                {
                    int difference = hp - m_LifeObjcts.Count;

                    for(int i = 0; i < difference; i++)
                    {
                        Vector3 pos = m_LifeObjcts[m_LifeObjcts.Count - 1].gameObject.transform.position;
                        pos.x += m_SpaceAmount;

                        //lifeObjの生成
                        GameObject obj = Instantiate(m_LifeObj, pos, Quaternion.identity);
                        obj.transform.parent = m_Canvas.transform;
                        m_LifeObjcts.Add(obj);
                    }
                }
                else
                {
                    foreach(GameObject obj in m_LifeObjcts)
                    {
                        obj.SetActive(false);
                    }

                    for(int i = 0; i < hp; i++)
                    {
                        m_LifeObjcts[i].SetActive(true);
                    }
                }

                m_OldHp = hp;
            }
        }
        
    }
}
