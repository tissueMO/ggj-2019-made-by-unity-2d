using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    const int MAX_ITEM_NAMBER = 5;

    ItemClass[] mItemList =new ItemClass[MAX_ITEM_NAMBER];

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// アイテムを入手できるか。
    /// </summary>
    /// <param name="item">入手アイテム</param>
    /// <returns>入手成功でtrue</returns>
    public bool TryGetItem(ItemClass item)
    {
        //ItemClass.OnUse+=UseItem;
        for(int i=0;i<=mItemList.Length;i++)
        {
            if (mItemList[i] == null)
            {
                mItemList[i] = item;
                item.ID = mItemList.Length;
                item.OnUse += UseItem;
                return true;
            }
        }
        return false;
    }

    void UseItem(int id)
    {
        mItemList[id]=null;
    }
}
