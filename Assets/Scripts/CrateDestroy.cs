using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDestroy : MonoBehaviour
{
    [Range(0,1)]
    public float dropRate;

    public List<GameObject> drops;
    public GameObject       ExplosionPrefab;
    // Start is called before the first frame update
    public void ExplodeCrate()
    {
        int dropIdx = Mathf.FloorToInt(Random.Range(0, drops.Count + 1));
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Instantiate(drops[dropIdx], transform.position, Quaternion.identity);
        Destroy(gameObject);

        // Instantiate()
    }
}
