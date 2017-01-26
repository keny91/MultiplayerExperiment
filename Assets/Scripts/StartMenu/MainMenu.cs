using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;



public class MainMenu : MonoBehaviour {

    public string buildVersion = "1.0";
    private Text BuildText;
    public CanvasGroup StartMenu, MultiplayerMenu;

    public KoskiNetworkManager NetManager;
    public string AvailableHOST = "";
    public bool Hosting = false;

    // Use this for initialization
    void Start () {

        NetManager = GameObject.Find("NetworkController").GetComponent<KoskiNetworkManager>();



        StartMenu = (CanvasGroup)GameObject.Find("MainMenu").GetComponent<CanvasGroup>();
        MultiplayerMenu = (CanvasGroup)GameObject.Find("MultiplayerMenu").GetComponent<CanvasGroup>();
        BuildText = (Text)GameObject.Find("BuildID").GetComponent<Text>();
        BuildText.text = buildVersion;

        Hide(MultiplayerMenu);
        Show(StartMenu);

    }

    public void Hide(CanvasGroup menu)
    {
        menu.alpha = 0;
        menu.interactable = false;
        menu.blocksRaycasts = false;
    }
    /// <summary>
    /// Display interface and return interactivity
    /// </summary>
    public void Show(CanvasGroup menu)
    {

        menu.alpha = 1;
        menu.interactable = true;
        menu.blocksRaycasts = true;
    }


    /// <summary>
    /// Start 
    /// </summary>
    public void OnSinglePlayerButton()
    {
        SceneManager.LoadScene("PreScene1");
    }


    /// <summary>
    /// Enable Multiplayer menu
    /// </summary>
    public void OnMultiPlayerButton()
    {
        Show(MultiplayerMenu);
        Hide(StartMenu);
        NetManager.PROV = GameObject.Find("IPDISPLAY").GetComponent<Text>();

    }

    public void OnHostButton()
    {
        NetManager.StartLocalServer();
        
        
        
    }
        
    public void OnJoinButton()
    {
        NetManager.StartClientGame();
            
    }

    // Update is called once per frame
    void Update () {
	
	}
}
