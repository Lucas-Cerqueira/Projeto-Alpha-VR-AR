using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Matchmaking : MonoBehaviour {

    [SerializeField] uint maxPlayersPerRoom = 4;

    private GameObject matchmakingUIPanel;
    private Toggle toggleUseVuforia;
    private InputField inputCreateMatchName;
    private InputField inputCreateMatchPwd;
	private InputField inputJoinMatchPwd;
    private Transform matchListRef;

    [HideInInspector] public NetworkManager manager;
    private MyNetworkManager myManager;
    private bool isMatchVisible;
    private string defaultRoomName;
    private float elapsedTime = 2;
    string roleSelected;

    private static bool reloadedMenu = false;

    void OnDestroy()
    {
        reloadedMenu = false;
    }

	// Use this for initialization
	void Start ()
    {   
	}
	
    public void SetupMatchmakingUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        manager = GetComponent<NetworkManager>();
        myManager = GetComponent<MyNetworkManager>();
        manager.matchSize = maxPlayersPerRoom;
        manager.StartMatchMaker();

        matchmakingUIPanel = GameObject.Find("Matchmaking UI");
        toggleUseVuforia = GameObject.Find("Toggle-UseVuforia").GetComponent<Toggle>();
        inputCreateMatchName = GameObject.Find("InputField-MatchName").GetComponent<InputField>();
        inputCreateMatchPwd = GameObject.Find("InputField-Password").GetComponent<InputField>();
        inputJoinMatchPwd = GameObject.Find("InputField-JoinMatchPassword").GetComponent<InputField>();
        matchListRef = GameObject.Find("RoomListRef").transform;

        toggleUseVuforia.onValueChanged.RemoveAllListeners();
        GameObject.Find("Button-CreateMatch").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Button-ListMatches").GetComponent<Button>().onClick.RemoveAllListeners();

        //print("Resetando Listeners dos botoes");

        toggleUseVuforia.onValueChanged.AddListener((value) => { GetComponent<MyNetworkManager>().ToggleUseVuforiaChanged(value); });
        GameObject.Find("Button-CreateMatch").GetComponent<Button>().onClick.AddListener(() => { this.CreateMatch(); });
        GameObject.Find("Button-ListMatches").GetComponent<Button>().onClick.AddListener(() => { this.ListMatches(); });
    }

	// Update is called once per frame
	void Update ()
    {
        if (reloadedMenu == false)
        {
            SetupMatchmakingUI();
            reloadedMenu = true;
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            matchmakingUIPanel.SetActive(!NetworkClient.active);

            elapsedTime += Time.deltaTime;

            if (!NetworkClient.active && !NetworkServer.active && elapsedTime >= 2)
            {
                manager.matchMaker.ListMatches(0, 20, "", manager.OnMatchList);

                /*if (manager.matches == null)
                    defaultRoomName = "Room 1";
                else
                    defaultRoomName = "Room " + (manager.matches.Count + 1).ToString();*/
                //inputCreateRoomName.text = defaultRoomName;
                elapsedTime = 0;
            }
        }
        
	}

    public void CreateMatch()
    {
        manager.matchMaker.CreateMatch(inputCreateMatchName.text, manager.matchSize, true, inputCreateMatchPwd.text, manager.OnMatchCreate);
        //roleSelected = roleSelectionDropdown.options[roleSelectionDropdown.value].text;
    }

    public void ListMatches()
    {
        /* Argumentos:
         * Numero da primeira pagina de resultados
         * Numero de resultados por página
         * String pra filtrar a busca pelo nome
         * Callback
         */
        manager.matchMaker.ListMatches(0, 20, "", manager.OnMatchList);

        if (manager.matches != null)
        {
            int roomNumber = 0;

            for (int i = 0; i < matchListRef.childCount; i++)
                Destroy(matchListRef.GetChild(i).gameObject);

            foreach (UnityEngine.Networking.Match.MatchDesc match in manager.matches)
            {

                GameObject button = Instantiate(Resources.Load("Button-Room") as GameObject);
                button.transform.SetParent(matchListRef);
                button.name = "Room " + roomNumber;
                button.transform.position = matchListRef.position + new Vector3(0, -40 * roomNumber, 0);
                button.transform.GetChild(0).GetComponent<Text>().text = match.name + ": " + match.currentSize + "/" + match.maxSize;
                button.GetComponent<Button>().onClick.AddListener(() => JoinMatch());
                roomNumber++;
            }
        }
    }

    public void JoinMatch()
    {
        Transform buttonPressed = GameObject.Find(EventSystem.current.currentSelectedGameObject.name).transform;
        string[] buttonText = buttonPressed.GetChild(0).GetComponent<Text>().text.Split(':');
        string joinRoomName = buttonText[0];

        string password = inputJoinMatchPwd.text;


        foreach (UnityEngine.Networking.Match.MatchDesc match in GetComponent<NetworkManager>().matches)
        {
            if (string.Compare(match.name, joinRoomName) == 0)
            {
                manager.matchMaker.JoinMatch(match.networkId, password, myManager.OnMatchJoin);
                return;
            }
        }

        print("Deu merda");
    }

    public void SetActiveToggleUseVuforia (bool active)
    {
        toggleUseVuforia.gameObject.SetActive(active);
    }
}
