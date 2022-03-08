using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBoom : MonoBehaviour
{
    public  GameObject  explosionPrefab;
    // public  GameObject  cc;
    public  LayerMask   levelMask;
    private bool        exploded = false;
    public  int         bombRange;

    public System.Action       bombcountreset;

    // public  int         playerOwner;

    void Start()
    {
        Invoke("Explode", 3f);
    }

    void Explode()
    {
        exploded = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity); //1
        bombcountreset();
        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));  

        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Collider").gameObject.SetActive(false);
        Destroy(gameObject, .3f); // ! SHORTEN THIS
    }

    private IEnumerator CreateExplosions(Vector3 direction) 
    {
        Vector3    rayStart = transform.position; //  + new Vector3(0,.5f,0) 
        for (int i = 1; i < bombRange; i++) 
        {
            RaycastHit hit;
            Physics.Raycast(rayStart, direction, out hit, 1, levelMask);
			// Debug.Log(i);
			// if (!hit.collider)
            // 	Debug.Log("raystart: " + rayStart + " Direction: " + direction + " Hit: nofin");
			// else
            // 	Debug.Log("raystart: " + rayStart + " Direction: " + direction + " Hit: "+ hit.collider.gameObject);
            rayStart += direction;
            if (!hit.collider) 
            {
                // Debug.Log((transform.position + (i * direction)));
                Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
            }
            else if (hit.collider.gameObject.CompareTag("Crate"))
            {
                Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                hit.collider.gameObject.GetComponent<CrateDestroy>().ExplodeCrate();
                break;
            }
            else 
            {
                break; 
            }
            yield return new WaitForSeconds(.03f); 
        }
		yield break; 
    }  

    private void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Explosion"))
        {
            CancelInvoke("Explode");
            Explode();
        }  
    }

}
