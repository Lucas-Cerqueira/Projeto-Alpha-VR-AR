using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetDisconnectFunctionOnButton : MonoBehaviour {

    bool isInitialized = false;

	void OnEnable()
    {
        isInitialized = false;
        
    }

    void Update()
    {
        if (!isInitialized)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject manager = GameObject.Find("NetworkManager");
            GetComponent<Button>().onClick.AddListener(() => { manager.GetComponent<MyNetworkManager>().DisconnectClient(); });

            isInitialized = true;
        }
    }
}
