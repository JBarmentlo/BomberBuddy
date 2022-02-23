using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum powerUpEnum // your custom enumeration
 {
    MoveSpeed, 
    BombCount, 
    BombRange
 };
public class ApplyPowerUp : MonoBehaviour
{
    public powerUpEnum type;

    [Range(0.5f, 2)]
    public float        moveSpeedIncrease = 1;
    private int         bombRangeIncrease = 1;
    private int         bombCountIncrease = 1;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("POWER COLL");
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("IN PLAYER");
            // Debug.Log(type);
            if (type == powerUpEnum.MoveSpeed)
            {
                other.gameObject.GetComponent<Player>().moveSpeed += moveSpeedIncrease;
            }
            if (type == powerUpEnum.BombCount)
            {
                other.gameObject.GetComponent<Player>().bombs += bombCountIncrease;
            }
            if (type == powerUpEnum.BombRange)
            {
                other.gameObject.GetComponent<Player>().bombRange += bombRangeIncrease;
            }
            Destroy(gameObject);
        }
    }
}
