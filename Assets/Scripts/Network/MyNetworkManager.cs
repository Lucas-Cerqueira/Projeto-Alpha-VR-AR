using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager {

    [HideInInspector] public string playerRoleSelected = "Shooter";
    [HideInInspector] public bool   useVuforia = false;
    
    private Dropdown roleSelectionDropdown;
    private static bool updatedDropdownListener = false;
    [HideInInspector] public long networkId;
    [HideInInspector] public long nodeId;

    void OnDestroy()
    {
        updatedDropdownListener = false;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        roleSelectionDropdown = GameObject.Find("RoleSelectionDropdown").GetComponent<Dropdown>();
        roleSelectionDropdown.onValueChanged.RemoveAllListeners();
        UpdatePlayerRole(roleSelectionDropdown.value);
    }

    void Update()
    {
        if (updatedDropdownListener == false)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                roleSelectionDropdown = GameObject.Find("RoleSelectionDropdown").GetComponent<Dropdown>();
                roleSelectionDropdown.onValueChanged.RemoveAllListeners();
                roleSelectionDropdown.onValueChanged.AddListener(UpdatePlayerRole);
                updatedDropdownListener = true;
                UpdatePlayerRole(roleSelectionDropdown.value);
            }
        }
    }


    public void UpdatePlayerRole(int value)
    {
        playerRoleSelected = roleSelectionDropdown.options[value].text;
        if (playerRoleSelected.Equals("Shooter"))
        {
            GetComponent<Matchmaking>().SetActiveToggleUseVuforia(false);
            useVuforia = false;
        }
        else
            GetComponent<Matchmaking>().SetActiveToggleUseVuforia(true);
    }

    public void ToggleUseVuforiaChanged (bool newValue)
    {
        useVuforia = newValue;
    }

    public override void OnMatchCreate(UnityEngine.Networking.Match.CreateMatchResponse matchInfo)
    {
        base.OnMatchCreate(matchInfo);
        networkId = (long)matchInfo.networkId;
        nodeId = (long)matchInfo.nodeId;
    }

    public override void OnMatchList(UnityEngine.Networking.Match.ListMatchResponse matchList)
    {
        base.OnMatchList(matchList);
    }

    public void OnMatchJoin(UnityEngine.Networking.Match.JoinMatchResponse matchInfo)
    {
        if (matchInfo.success)
        {
            try
            {
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, new UnityEngine.Networking.Types.NetworkAccessToken(matchInfo.accessTokenString));
            }
            catch (System.Exception ex)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(ex);
                }
            }
            this.StartClient(new UnityEngine.Networking.Match.MatchInfo(matchInfo));
        }
        else if (LogFilter.logError)
        {
            Debug.LogError(string.Concat("Join Failed:", matchInfo));
        }

        networkId = (long)matchInfo.networkId;
        nodeId = (long)matchInfo.nodeId;
    }

    public void DisconnectHost()
    {
        matchMaker.DestroyMatch((UnityEngine.Networking.Types.NetworkID)networkId, OnMatchDestroyed);
    }

    public void DisconnectClient()
    {
        UnityEngine.Networking.Match.DropConnectionRequest dropRequest = new UnityEngine.Networking.Match.DropConnectionRequest();
        dropRequest.networkId = (UnityEngine.Networking.Types.NetworkID)networkId;
        dropRequest.nodeId = (UnityEngine.Networking.Types.NodeID)nodeId;
        matchMaker.DropConnection(dropRequest, OnConnectionDrop);
    }

    public void OnMatchDestroyed(UnityEngine.Networking.Match.BasicResponse response)
    {
        if (response.success)
        {
            StopMatchMaker();
            StopHost();
        }
    }

    public void OnConnectionDrop(UnityEngine.Networking.Match.BasicResponse response)
    {
        if (response.success)
        {
            StopMatchMaker();
            StopHost();
        }
    }
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    /* Handling the player joining according to the options selected on the main menu:
     * playerRoleSelected
     * useVuforia
    */
    public class MsgTypes
    {
        public const short PlayerPrefab = MsgType.Highest + 1;

        public class PlayerPrefabMsg : MessageBase
        {
            public short controllerID;
            public string roleSelected;
            public bool useVuforia;
        }
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgTypes.PlayerPrefab, OnResponseRole);
        base.OnStartServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgTypes.PlayerPrefab, OnRequestRole);
        base.OnClientConnect(conn);
    }

    private void OnRequestRole(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>().controllerID;
        msg.roleSelected = playerRoleSelected;
        msg.useVuforia   = useVuforia;     
        client.Send(MsgTypes.PlayerPrefab, msg);
    }

    private void OnResponseRole(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        playerPrefab = (GameObject)Resources.Load(msg.roleSelected);
        playerPrefab.GetComponent<ControlLocalPlayer>().useVuforia = msg.useVuforia;
        //if (msg.roleSelected.Equals("General"))
        //{
        //    if (msg.useVuforia == false)
        //    {
        //        playerPrefab.transform.GetChild(0).tag = "MainCamera";
        //        playerPrefab.transform.GetChild(0).gameObject.SetActive(true);
        //        playerPrefab.transform.GetChild(1).gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        playerPrefab.transform.GetChild(1).tag = "MainCamera";
        //        playerPrefab.transform.GetChild(0).gameObject.SetActive(false);
        //        playerPrefab.transform.GetChild(1).gameObject.SetActive(true);
        //    }
        //}
        //else
        //{
        //    playerPrefab.transform.GetChild(0).tag = "MainCamera";
        //    playerPrefab.transform.GetChild(0).gameObject.SetActive(true);
        //}

        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefab, msg);
    }

    //public void SpawnPlayer(NetworkConnection connection, short playerControllerId, string roleSelected)
    //{
    //    print("Player ID: " + playerControllerId.ToString());

    //    int numberOfPlayers = 10;

    //    matchMaker.ListMatches(0, 20, matchName, OnMatchList);

    //    if (matches.Count != 0)
    //    {
    //        numberOfPlayers = matches[0].currentSize;
    //        //foreach (UnityEngine.Networking.Match.MatchDesc match in matches)
    //        //{
    //        //    numberOfPlayers = match.currentSize;
    //        //}
    //    }
    //    else
    //        numberOfPlayers = 1;

    //    switch (roleSelected)
    //    {
    //        case "Shooter":
    //            {
    //                // Subtracted 1 from numberOfPlayer because indexing of children starts at 0
    //                Transform spawnPoint = GameObject.Find("ShooterSpawnPoints").transform.GetChild(numberOfPlayers - 1);
    //                GameObject shooter = (GameObject)Instantiate(Resources.Load("Shooter", typeof(GameObject)), spawnPoint.position, spawnPoint.rotation);
    //                NetworkServer.AddPlayerForConnection(connection, shooter, playerControllerId);
    //                break;
    //            }
    //        case "General":
    //            {
    //                Transform spawnPoint = GameObject.Find("GeneralSpawnPoint").transform;
    //                GameObject general = (GameObject)Instantiate(Resources.Load("General", typeof(GameObject)), spawnPoint.position, spawnPoint.rotation);
    //                NetworkServer.AddPlayerForConnection(connection, general, playerControllerId);
    //                break;
    //            }
    //    }
}


