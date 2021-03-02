using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    public string MainMenu;
    public string CureentScene;
    public string SoccerScene;
    public string GolfScene;
    public string RaceScene;
    public bool Intancied;
    public GameObject CarPrefab;
    public GameObject ButtonPrefab;
    public Transform RoomList;
    private string ListRoomName;
    private string ListRoomPlayers;
    private string ListRoomMaxPlayers;
    private string [][] ListRoom = new string [0][];
    public Button Connect; 
    public Button Disconnect;
    public InputField RoomName;


    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            return _instance;
        }
    }


    void Awake() {
        DontDestroyOnLoad(this);
    }

    void Start(){
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect.onClick.AddListener(() => {CreateRoom(RoomName.text , 4);});
        Disconnect.onClick.AddListener(() => { JoinRoom(RoomName.text);
        PhotonNetwork.AutomaticallySyncScene = true;});
    }

    public void CreateRoom(string roomName, byte PlayersMax){  
        if(!PhotonNetwork.InRoom)      
            PhotonNetwork.CreateRoom(roomName,  new RoomOptions { MaxPlayers = PlayersMax, IsVisible = true }, TypedLobby.Default);}//si la room exist deja et Mode de jeu a rajouter
        
    public void JoinRoom(string roomName)
    {
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        if(PhotonNetwork.InRoom){ 
        PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        foreach (Transform Button in RoomList)
        {
            Destroy(Button.gameObject);
        }
            foreach (RoomInfo room in roomList)
        {
            GameObject newRoomButton = Instantiate(ButtonPrefab, RoomList);
            newRoomButton.transform.Find("Text").GetComponent<Text>().text = room.Name +"           "+ room.PlayerCount + "/" + room.MaxPlayers;
            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(room.Name); });
        }
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.G)){
                Debug.Log(PhotonNetwork.InLobby);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LeaveRoom();
        }
        if (Input.GetKeyDown(KeyCode.J)){Debug.Log(ListRoom[0][0]);}}

    public override void OnConnectedToMaster (){
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreatedRoom (){
        Debug.Log("Created Room : " + PhotonNetwork.CurrentRoom.Name);}  
    
    public override void OnJoinedRoom (){
        Debug.Log("Joined Room : " + PhotonNetwork.CurrentRoom.Name);
        ChangeScene(SoccerScene);}

    public override void OnLeftRoom(){
        Debug.Log("Left Room");
        ChangeScene(MainMenu);
        Destroy(this.gameObject);
    }  

    public void ChangeScene(string sceneName){
        PhotonNetwork.LoadLevel(sceneName);
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            StartCoroutine("waitForSceneLoad", sceneName);
        }
        else
            SpawnPlayer();
    }

    IEnumerator waitForSceneLoad(string sceneName)
    {
        while (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            CureentScene = sceneName;
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        GameObject playerGameObject = PhotonNetwork.Instantiate(CarPrefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
        playerGameObject.transform.Find("Camera").gameObject.SetActive(true);
        playerGameObject.transform.Find("Voiture").gameObject.transform.GetComponent<CarControllers>().enabled = true;
        //modifier les coloures en fonctions des parametres
        DontDestroyOnLoad(playerGameObject);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("okokokokokok");
    }
}