using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script makes sure that a bomb can be laid down at the player's feet without causing buggy movement when the player walks away.
/// It disables the trigger on the collider, essentially making the object solid.
/// </summary>
public class DisableTriggerOnPlayerExit : MonoBehaviour
{
    private Collider    collid;
	private int			playersInBomb;
    // private bool        firstFrame = true;
    // private List<Collider>  ignoredColliders;

    // IEnumerator EnableCollision()
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     collid.isTrigger = false; // Disable the trigger
    // }
    // void Start()
    // {
    //     Collider[] G = GetComponents<Collider>();
    //     collid = G[0];
    //     StartCoroutine(EnableCollision());
    // }

    public void OnTriggerExit (Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        { // When the player exits the trigger area
            GetComponent<Collider> ().isTrigger = false; // Disable the trigger
        }
    }


    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     { // When the player exits the trigger area
    //         // Debug.Log("Trigger Enter");
    //         Physics.IgnoreCollision(collid, other, true);
    //         // GetComponent<Collider>().isTrigger = false; // Disable the trigger
    //     }
    // }  

    // void OnCollisionEnter(Collision other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         // Debug.Log("Colision ENter");
    //         Physics.IgnoreCollision(collid, other.collider, true);
    //     }
    // }

    // void OnCollision(Collision other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         // Debug.Log("Colision");
    //         // Physics.IgnoreCollision(collid, other.collider);
    //     }
    // }

    // void OnCollisionExit(Collision other)
    // {
    //     if(other.gameObject.CompareTag("Player"))
    //     {
    //         // Debug.Log("Colision EXIT");
    //         Physics.IgnoreCollision(collid, other.collider, false);
    //     }
    // }
}
