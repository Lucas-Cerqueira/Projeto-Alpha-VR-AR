using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PoolingObjectHandler : MonoBehaviour
{
	static Dictionary<string, PoolingObjectHandler> s_pools = new Dictionary<string, PoolingObjectHandler>();

	bool isInitialized;

	public GameObject prefab;
	public int initialPoolSize = 4;
	public int currentActiveCount = 0;
	public int currentPoolSize;
	public int maxPoolSize = 12;
	public float growthRate = 0f;
	public bool autoParent = true;

	public string dynamicAssetString;
	public NetworkHash128 spawnAssetId;

	HashSet<GameObject> activeObjects = new HashSet<GameObject>();
	Stack<GameObject> freeObjects = new Stack<GameObject>();

	Dictionary<GameObject,IEnumerator> killRequests = new Dictionary<GameObject, IEnumerator>();

	static public PoolingObjectHandler GetPoolByName(string name)
	{
		return s_pools[name];
	}

	public virtual void Awake()
	{
		InitializePool();
	}

	GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId)
	{
		var go = CreateFromPool(position, Quaternion.identity);
		Debug.LogWarning("ClientSpawn:  " + go.GetInstanceID());
		return go;
	}

	void ClientUnSpawnHandler(GameObject spawned)
	{
		print ("unspawn");
		Debug.LogWarning("ClientUnSpawn:" + spawned.GetInstanceID());
		ReturnToPool(spawned);
	}

	void InitializePool()
	{
		if (isInitialized)
		{
			return;
		}

		if (prefab != null)
		{
			if (prefab.GetComponent<NetworkIdentity>() == null)
			{
				Debug.LogError("Network Spawn Pool prefab " + prefab + " does not have a NetworkIdentity");
				return;
			}
		}
		isInitialized = true;
		s_pools.Add(gameObject.name, this);

		for (int i = 0; i < initialPoolSize; i++)
		{
			var go = InstantiatePrefab(Vector3.zero, Quaternion.identity);
			go.SetActive(false);
			freeObjects.Push(go);

			if (autoParent)
			{
				go.transform.parent = transform;
			}
		}
		currentPoolSize = initialPoolSize;

		if (prefab != null)
		{
			spawnAssetId = prefab.GetComponent<NetworkIdentity>().assetId;
		}
		else if (dynamicAssetString != "")
		{
			spawnAssetId = NetworkHash128.Parse(dynamicAssetString);
		}

		ClientScene.RegisterSpawnHandler(spawnAssetId, ClientSpawnHandler, ClientUnSpawnHandler);
	}

	GameObject CreateFromPool(Vector3 pos, Quaternion rot)
	{
		if (freeObjects.Count == 0)
		{
			if (currentPoolSize < maxPoolSize)
			{
				int newPoolSize = (int) (currentPoolSize*growthRate);
				if (newPoolSize > maxPoolSize)
				{
					newPoolSize = maxPoolSize;
				}
				Debug.LogWarning("Growing pool " + gameObject + " to " + newPoolSize);

				for (int n = currentPoolSize; n < newPoolSize; n++)
				{
					var newGo = InstantiatePrefab(Vector3.zero, Quaternion.identity);
					newGo.SetActive(false);
					freeObjects.Push(newGo);

					if (autoParent)
					{
						newGo.transform.parent = transform;
					}
				}
				currentPoolSize = newPoolSize;
			}
			else
			{
				Debug.LogError("Pool empty for " + prefab);
				return null;
			}
		}

		var go = freeObjects.Pop();

		go.transform.position = pos;
		go.transform.rotation = rot;
		go.SetActive(true);

		activeObjects.Add(go);
		currentActiveCount += 1;

		return go;
	}

	void ReturnToPool(GameObject go)
	{
		if (!activeObjects.Contains(go))
		{
			Debug.LogError("Pool doesnt contain " + go);
			return;
		}

		if (killRequests.ContainsKey(go))
		{
			var coroutine = killRequests[go];
			killRequests.Remove(go);

			StopCoroutine(coroutine);
		}

		go.SetActive(false);
		activeObjects.Remove(go);
		currentActiveCount -= 1;
		freeObjects.Push(go);
	}

	protected virtual GameObject InstantiatePrefab(Vector3 pos, Quaternion rot)
	{
		return (GameObject) Instantiate(prefab, pos, rot);
	}


	public GameObject ServerCreateFromPool(Vector3 pos, Quaternion rot)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("ServerCreateFromPool called on client!");
			return null;
		}
		var go = CreateFromPool(pos, rot);

		if (go == null)
			return null;

		NetworkServer.Spawn(go, spawnAssetId);
		return go;
	}

	public void ServerReturnToPool(GameObject go)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("ServerReturnToPool called on client!");
			return;
		}

		ReturnToPool(go);
		NetworkServer.UnSpawn(go);
	}

	IEnumerator ServerDelayDestroy(GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		ServerReturnToPool(go);
	}

	public void ServerReturnToPool(GameObject go, float delay)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("ServerReturnToPool called on client!");
			return;
		}

		if (!activeObjects.Contains(go))
		{
			Debug.LogError("Pool doesnt contain " + go);
			return;
		}

		// prevents double-destroys
		var coroutine = ServerDelayDestroy(go, delay);
		killRequests.Add(go, coroutine);

		StartCoroutine(coroutine);
	}


}
