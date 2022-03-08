using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDestroy : MonoBehaviour
{
    [Range(0,1)]
    public 	float 			dropRate;
	private System.Random 	rand = new System.Random();

    public 	List<GameObject> drops;

	private bool			hit = false; // to prevent crate from double dropping

    public void ExplodeCrate()
    {
		if (hit)
			return;
		hit = true;
		Debug.Log("Crate destroy " + transform.position);
		if (rand.NextDouble() < dropRate)
		{
    	    int dropIdx = Mathf.FloorToInt(Random.Range(0, drops.Count));
	        Instantiate(drops[dropIdx], transform.position, Quaternion.identity);	
		}
        Destroy(gameObject);

        // Instantiate()
    }
}
