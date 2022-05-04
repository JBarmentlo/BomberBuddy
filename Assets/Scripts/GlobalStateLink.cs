using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public enum StateLinkType
{
    Player1,
    Player2,
    Bomb,
    Explosion,
    Wall,
    Crate,
    ExtraRange,
    ExtraBomb,
    ExtraSpeed
}

public class GlobalStateLink : MonoBehaviour
{
    public StateLinkType 	type;
    public bool 			is_player = false;

    public string           localDate;

    [SerializeField]
    public Vector3          position = new Vector3();


    public virtual void Start()
    {
        localDate = DateTime.Now.ToString();
        GlobalStateManager.Instance.AddGameObject(this);
    }
    public virtual void OnDestroy()
    {
        GlobalStateManager.Instance.RemoveGameObject(this);
    }

    public virtual string JsonRep()
    {
        position = this.gameObject.transform.position;
        return JsonUtility.ToJson(this);
    }

    // public virtual Vector3 GetPos()
    // {
    //     return ;
    // }
}
