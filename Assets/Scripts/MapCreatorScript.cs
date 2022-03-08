using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCreatorScript : MonoBehaviour
{
	private static 	MapCreatorScript 		_instance 	= null;
    public static 	MapCreatorScript 		Instance 	{ get { return _instance; } }

	public float		crateDensity;
	public GameObject	wallPrefab;
	public GameObject	floorLightPrefab;
	public GameObject	floorDarkPrefab;
	public GameObject	cratePrefab;

	private GameObject	tmp;
	private GameObject	ground;
	private GameObject	walls;
	private GameObject	crates;
	private System.Random rand = new System.Random();
	private int h;
	private int w;

    // Start is called before the first frame update
    void Start()
    {
		if (_instance != null && _instance != this)
        {
			Debug.LogError("destroyed MapCreatorScript");
            Destroy(_instance.gameObject);
            _instance = this;
        } else {
            _instance = this;
        }
		walls = transform.GetChild(1).gameObject;
		ground = transform.GetChild(2).gameObject;
		crates = transform.GetChild(0).gameObject;
        CreateMap(11, 11);
		// GlobalStateManager.Instance.InstantiatePlayer(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	int DistToClosestPlayerStart(int r, int c)
	{
		return (Math.Min(r + c - 2, (h - 2 - r + w - 2 - c)));
	}

	public void CreateMap(int h, int w)
	{
		this.h = h;
		this.w = w;
		for (int r = 0; r < h; r++)
		{
			for (int c = 0; c < w; c++)
			{
				if (r == 0 || c == 0 || r == (h - 1) || c == (w - 1) || (r % 2 == 0 && c % 2 == 0))
				{
					Instantiate(wallPrefab, new Vector3(transform.position.x + c, transform.position.y + 0.5f, transform.position.z - r), Quaternion.identity).transform.SetParent(walls.transform);
				}
				else if (DistToClosestPlayerStart(r, c) > 2 && rand.NextDouble() < crateDensity)
				{
					Instantiate(cratePrefab, new Vector3(transform.position.x + c, transform.position.y + 0.5f, transform.position.z - r), Quaternion.identity).transform.SetParent(crates.transform);
				}
				if ((r + c) % 2 == 1)
				{
					Instantiate(floorDarkPrefab, new Vector3(transform.position.x + c, transform.position.y - 0.5f, transform.position.z - r), Quaternion.identity).transform.SetParent(ground.transform);
				}
				else
				{
					Instantiate(floorLightPrefab, new Vector3(transform.position.x + c, transform.position.y - 0.5f, transform.position.z - r), Quaternion.identity).transform.SetParent(ground.transform);
				}
			}
		}
	}

	public void ResetMap()
	{
		foreach (Transform child in crates.transform)
		{
			Destroy(child.gameObject);
		}
		for (int r = 0; r < h; r++)
		{
			for (int c = 0; c < w; c++)
			{
				if (r == 0 || c == 0 || r == (h - 1) || c == (w - 1) || (r % 2 == 0 && c % 2 == 0))
				{
					continue;
				}
				else if (DistToClosestPlayerStart(r, c) > 2 && rand.NextDouble() < crateDensity)
				{
					Instantiate(cratePrefab, new Vector3(transform.position.x + c, transform.position.y + 0.5f, transform.position.z - r), Quaternion.identity).transform.SetParent(crates.transform);
				}
			}
		}
	}
	public Vector3 GetPlayerStartCoord(int playerNum)
	{
		if (playerNum == 1)
		{
			return (this.transform.position + new Vector3(1, 0.5f, -1));
		}
		else if (playerNum == 2)
		{
			return (this.transform.position + new Vector3(w - 2, 0.5f, - (h - 2)));
		}
		else
		{
			throw new Exception();
		}
	}
}
