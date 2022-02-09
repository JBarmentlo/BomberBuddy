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
        Debug.Log("Booooon");
        Vector3    rayStart = transform.position; //  + new Vector3(0,.5f,0) 
        for (int i = 1; i < bombRange; i++) 
        {
            Debug.Log(rayStart);
            RaycastHit hit;
            Physics.Raycast(rayStart, direction, out hit, 1, levelMask);
            rayStart += direction;
            if (!hit.collider) 
            {
                // Debug.Log((transform.position + (i * direction)));
                Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation); 
            }
            else if (hit.collider.gameObject.CompareTag("Crate"))
            {
                hit.collider.gameObject.GetComponent<CrateDestroy>().ExplodeCrate();
                Debug.Log("Fik da crat");

                yield break;
            }
            else 
            {
                Debug.Log("Stop " + hit.collider.name);
                break; 
            }
            yield return new WaitForSeconds(.05f); 
        }
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
