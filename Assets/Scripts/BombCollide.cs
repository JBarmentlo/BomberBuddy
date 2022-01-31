using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollide : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetCollisionIgnore(Collider ignoreCollider)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), ignoreCollider);
    }
}
