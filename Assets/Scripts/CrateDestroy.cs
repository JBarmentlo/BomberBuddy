using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDestroy : MonoBehaviour
{
    [Range(0,1)]
    public float dropRate;

    public List<GameObject> drops;
    // Start is called before the first frame update
    public void ExplodeCrate()
    {
        int dropIdx = Mathf.FloorToInt(Random.Range(0, drops.Count + 0.99f));
        Instantiate(drops[dropIdx], transform.position, Quaternion.identity);
        Destroy(gameObject);

        // Instantiate()
    }
}