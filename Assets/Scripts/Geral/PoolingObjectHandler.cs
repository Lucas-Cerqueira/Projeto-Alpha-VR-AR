using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PoolingObjectHandler : NetworkBehaviour
{
    [SerializeField] GameObject pooledPrefab;
    [SerializeField] int pooledObjectsAmount = 20;
    [SerializeField] float objectSpawnDelay = 1f;

    private float elapsedTime;
    private bool gameStarted = false;
    private bool isInitialized = false;

    private int currentActiveCount = 0;

    NetworkHash128 spawnAssetId;

    //[SyncVar] SyncListBool objectStateList = new SyncListBool();
    Stack<GameObject> availableObjectsPool = new Stack<GameObject>();
    HashSet<GameObject> activeObjectsPool = new HashSet<GameObject>();

    Dictionary<GameObject, IEnumerator> killRequests = new Dictionary<GameObject, IEnumerator>();

    //public override void OnStartClient() 
    //{
    //    objectStateList.Callback = OnStateListChanged;
    //    if (!isServer)
    //        for (int i = 0; i < objectStateList.Count; i++)
    //        {
    //            print("Pedindo update");
    //            CmdRequestStateUpdate(i);
    //        }
    //} 

    //private void OnStateListChanged(SyncListBool.Operation op, int index)
    //{
    //    print("Mudou o estado de algum inimigo");
    //    CmdRequestStateUpdate(index);
    //}

    public override void OnStartServer()
    {
        gameStarted = true;
    }

    //GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId)
    //{
    //    return InstantiatePrefab();
    //}

    //void ClientUnSpawnHandler(GameObject spawned)
    //{
    //    DeactivatePrefab(spawned);
    //}

    void Start ()
    {
        elapsedTime = objectSpawnDelay;
        if (isServer)
            //objectStateList = new SyncListBool();

        BuildPool(pooledObjectsAmount);  
    }

    //[Command]
    //void CmdRequestStateUpdate (int index)
    //{
    //    if (objectStateList[index] == false)
    //        RpcDeactivatePrefab(objectPool[index]);
    //    if (objectStateList[index] == true)
    //        RpcActivatePrefab(objectPool[index], transform.position, transform.rotation);
    //}

    GameObject ClientSpawnHandler(Vector3 position, NetworkHash128 assetId)
    {
        GameObject go = CreateFromPool(position, Quaternion.identity);
        Debug.LogWarning("ClientSpawn:  " + go.GetInstanceID());
        return go;
    }

    void ClientUnSpawnHandler(GameObject spawned)
    {
        Debug.LogWarning("ClientUnSpawn:" + spawned.GetInstanceID());
        ReturnToPool(spawned);
    }

    void BuildPool(int poolSize)
    {
        if (isInitialized)
            return;

        isInitialized = true;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newObject = (GameObject)Instantiate(pooledPrefab, transform.position, transform.rotation);
            newObject.SetActive(false);
            newObject.transform.SetParent(this.transform);
            availableObjectsPool.Push(newObject);
            //NetworkServer.Spawn(newObject);
            //if (isServer)
            //{
            //    objectStateList.Add(false);
            //    RpcDeactivatePrefab(newObject);
            //}
            //CmdDeactivatePrefab(newObject);
            
            //Destroy(enemy, 10);
        }
        spawnAssetId = pooledPrefab.GetComponent<NetworkIdentity>().assetId;
        ClientScene.RegisterSpawnHandler(spawnAssetId, ClientSpawnHandler, ClientUnSpawnHandler);
    }

    protected virtual GameObject CreateFromPool(Vector3 pos, Quaternion rot)
    {
        // Commented this section because we can use it later, but not for now

        //if (availableObjectsPool.Count == 0)
        //{
        //    if (currentPoolSize < maxPoolSize)
        //    {
        //        int newPoolSize = (int)(currentPoolSize * growthRate);
        //        if (newPoolSize > maxPoolSize)
        //        {
        //            newPoolSize = maxPoolSize;
        //        }
        //        Debug.LogWarning("Growing pool " + gameObject + " to " + newPoolSize);

        //        for (int n = currentPoolSize; n < newPoolSize; n++)
        //        {
        //            var newGo = InstantiatePrefab(Vector3.zero, Quaternion.identity);
        //            newGo.SetActive(false);
        //            freeObjects.Push(newGo);

        //            if (autoParent)
        //            {
        //                newGo.transform.parent = transform;
        //            }
        //        }
        //        currentPoolSize = newPoolSize;
        //    }
        //    else
        //    {
        //        Debug.LogError("Pool empty for " + prefab);
        //        return null;
        //    }
        //}

        if (availableObjectsPool.Count == 0)
            return null;

        GameObject go = availableObjectsPool.Pop();

        go.transform.position = pos;
        go.transform.rotation = rot;
        go.SetActive(true);

        activeObjectsPool.Add(go);
        currentActiveCount += 1;

        return go;
    }

    public virtual void ReturnToPool(GameObject go)
    {
        if (!activeObjectsPool.Contains(go))
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
        activeObjectsPool.Remove(go);
        currentActiveCount -= 1;
        availableObjectsPool.Push(go);
    }

    public GameObject ServerCreateFromPool(Vector3 pos, Quaternion rot)
    {
        if (!isServer)
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
        if (!isServer)
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
        if (!isServer)
        {
            Debug.LogError("ServerReturnToPool called on client!");
            return;
        }

        if (!activeObjectsPool.Contains(go))
        {
            Debug.LogError("Pool doesnt contain " + go);
            return;
        }

        // prevents double-destroys
        var coroutine = ServerDelayDestroy(go, delay);
        if (!killRequests.ContainsKey(go))
        {
            killRequests.Add(go, coroutine);
            StartCoroutine(coroutine);
        }
    }

    void FixedUpdate()
    {
        if (gameStarted && isServer)
        {
            //RpcSyncClientServerPooledObjects();
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= objectSpawnDelay)
            {
                ServerCreateFromPool(transform.position, transform.rotation);
                elapsedTime = 0;
            }
        }
    }

    //[Command]
    //public void CmdActivatePrefab()
    //{
    //    if (availableObjectsPool.Count > 0)
    //    {
    //        ActivatePrefab(availableObjectsPool[0], transform.position, transform.rotation);

    //        availableObjectsPool.RemoveAt(0);
    //        objectStateList[objectPool.FindIndex(x => x == availableObjectsPool[0])] = true;
    //        RpcActivatePrefab(availableObjectsPool[0], transform.position, transform.rotation);
    //    }
    //}

    //[Command]
    //public void CmdDeactivatePrefab(GameObject target)
    //{
    //    DeactivatePrefab(target);
    //    availableObjectsPool.Add(target);
    //    objectStateList[objectPool.FindIndex(x => x == target)] = false;
    //    RpcDeactivatePrefab(target);
    //}

    //[ClientRpc]
    //void RpcActivatePrefab(GameObject target, Vector3 position, Quaternion rotation)
    //{
    //    print("Ativando no cliente");
    //    target.transform.position = position;
    //    target.transform.rotation = rotation;
    //    if (!isServer)
    //        ActivatePrefab(target, position, rotation);
    //}

    //[ClientRpc]
    //void RpcDeactivatePrefab(GameObject target)
    //{
    //    print("Destivando no cliente");
    //    if (!isServer)
    //        DeactivatePrefab(target);
    //}

    //public void ActivatePrefab(GameObject target, Vector3 position, Quaternion rotation)
    //{
        
    //    Component[] comps = target.GetComponents<Component>();

    //    for (int i = 0; i < comps.Length; i++)
    //    {
    //        if (comps[i] != this && comps[i].GetType() != typeof(NetworkIdentity) && comps[i].GetType() != typeof(NetworkAnimator) && comps[i].GetType() != typeof(NetworkTransform))
    //        {
    //            if (comps[i].GetType() == typeof(NavMeshAgent))
    //                ((NavMeshAgent)comps[i]).enabled = true;

    //            if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
    //                ((MonoBehaviour)comps[i]).enabled = true;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Collider)))
    //                ((Collider)comps[i]).enabled = true;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Collider2D)))
    //                ((Collider2D)comps[i]).enabled = true;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Renderer)))
    //                ((Renderer)comps[i]).enabled = true;

    //        }
    //    }

    //    for (int i = 0; i < target.transform.childCount; i++)
    //    {
    //        target.transform.GetChild(i).gameObject.SetActive(true);
    //    }

    //    if (isServer)
    //    {
    //        target.transform.position = position;
    //        target.transform.rotation = rotation;
    //    }
    //}

    

    //public void DeactivatePrefab(GameObject target)
    //{
    //    //target.SetActive(false);
    //    Component[] comps = target.GetComponents<Component>();

    //    for (int i = 0; i < comps.Length; i++)
    //    {
    //        if (comps[i] != this && comps[i].GetType() != typeof(NetworkIdentity) && comps[i].GetType() != typeof(NetworkAnimator) && comps[i].GetType() != typeof(NetworkTransform))
    //        {
    //            if (comps[i].GetType() == typeof(NavMeshAgent))
    //                ((NavMeshAgent)comps[i]).enabled = false;

    //            if (comps[i].GetType().IsSubclassOf(typeof(MonoBehaviour)))
    //                ((MonoBehaviour)comps[i]).enabled = false;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Collider)))
    //                ((Collider)comps[i]).enabled = false;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Collider2D)))
    //                ((Collider2D)comps[i]).enabled = false;

    //            if (comps[i].GetType().IsSubclassOf(typeof(Renderer)))
    //                ((Renderer)comps[i]).enabled = false;

                
    //        }
    //    }

    //    for (int i = 0; i < target.transform.childCount; i++)
    //    {
    //        target.transform.GetChild(i).gameObject.SetActive(false);
    //    }
    //}

    

    //public void ActivateObject()
    //{
    //    for (int i = 0; i < pooledObjectsAmount; i++)
    //    {
    //        if (!objectPool[i].activeInHierarchy)
    //        {
    //            objectPool[i].transform.position = transform.position;
    //            objectPool[i].transform.rotation = transform.rotation;
    //            objectPool[i].SetActive(true);
    //            if (isServer)
    //            {
    //                RpcActivateObject();
    //            }
    //            return;
    //        }
    //    }
    //}

    //public void DeactivateObject(GameObject target)
    //{
    //    objectPool.Find(target).SetActive(false);
    //    if (isServer)
    //    {
    //        objectState[i] = false;
    //        RpcDeactivateObject(target);
    //    }
    //}

    //[ClientRpc]
    //void RpcActivateObject()
    //{
    //    if (!isServer)
    //        ActivateObject();
    //}

    //[ClientRpc]
    //void RpcDeactivateObject(GameObject target)
    //{
    //    if (!isServer)
    //        DeactivateObject(target);
    //}

    //[ClientRpc]
    //public void RpcSyncClientServerPooledObjects()
    //{
    //    if (!isServer)
    //        for (int i = 0; i < pooledObjectsAmount; i++)
    //        {
    //            if (objectPool[i].activeInHierarchy && objectState[i] == false)
    //                objectPool[i].SetActive(false);

    //            if (!objectPool[i].activeInHierarchy && objectState[i] == true)
    //                ActivateObject();
    //        }
    //}

    
}
