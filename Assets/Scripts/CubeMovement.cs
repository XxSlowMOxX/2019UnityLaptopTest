using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviourPun
{

    public Color selectorColor;
    public GameObject camObj;
    public float speed;
    public GameObject debugArrow;
    private float camHeight = 5;
    [SerializeField]
    [Range(0,5)]
    private float cameraCursorFollow;
    [SerializeField]
    [Range(0.0f, 0.5f)]
    private float cameraCursorFollowTrigger;
    [SerializeField]
    [Range(0, 1)]
    private float cameraLag;
    private Entity thisEntity;
    public float grabReach;
    public GameObject playerHUD;
    private bool reloadInit;
    public Vector3 mvVector;
    private GameManager2 myMatch;
    private List<RespawnPoint> spawnPoints;
    private PhotonView myView;

    void Awake()
    {
        //GameObject cameraObject = Instantiate(camObj, this.transform);
        //camObj = cameraObject;
        this.GetComponent<EscMenu>().ShowEscMenu(false);
        thisEntity = this.GetComponent<Entity>();
        myMatch = FindObjectOfType<GameManager2>();
        myView = GetComponent<PhotonView>();
        if (myView.IsMine)
        {
            this.GetComponent<Scoreboard>().Init();
        }
    }

    void Update()
    {
        if (myView.IsMine)
        {
            if(thisEntity.myState != Entity.entityState.Alive)
            {
                return;
            }
            #region menus
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                this.GetComponent<EscMenu>().ShowEscMenu(true);
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                this.GetComponent<Scoreboard>().ShowScoreboard(true);
            }
            else
            {
                this.GetComponent<Scoreboard>().ShowScoreboard(false);
            }
            #endregion
            #region Weapon Switching
            if(Input.mouseScrollDelta.y > 0)
            {
                thisEntity.ChangeWeaponCall();
            }
            #endregion
            Ray myRay = camObj.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); 
            Vector3 mouseWorldPoint = new Vector3(myRay.origin.x, this.transform.position.y, myRay.origin.z);
            //print(mouseWorldPoint);
            debugArrow.transform.LookAt(mouseWorldPoint, Vector3.up);
            thisEntity.forwardVector = debugArrow.transform.forward;
            Quaternion lookAtRot = debugArrow.transform.rotation;            
            //debugArrow.transform.Rotate(new Vector3(90, -90, 0));
            CameraOffsetCursor();
            CameraFovHandling();
            if (Input.GetMouseButton(0))
            {
                if(thisEntity.weaponList.Count > 0)
                {
                    thisEntity.weaponList[thisEntity.currentWeaponIndex].Fire(Time.deltaTime);
                }
                else
                {
                    print("No weapon");
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;                
                if(Physics.Raycast(transform.position, debugArrow.transform.forward, out hit, grabReach))
                {
                    if(hit.transform.gameObject.GetComponent<WeaponBase>() != null)
                    {
                        print("Found a Weapon: " + hit.transform.name);
                        thisEntity.PickUpCall(hit.transform.gameObject.name);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                reloadInit = true;
                thisEntity.weaponList[thisEntity.currentWeaponIndex].ReloadStart();                
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                thisEntity.DropWeaponCall();
            }
            
        }
        else
        {
            camObj.SetActive(false);
            playerHUD.SetActive(false);
            this.GetComponent<EscMenu>().ShowEscMenu(false);
            debugArrow.transform.position = this.transform.position;
        }
    }

    void FixedUpdate()
    {
        if (myView.IsMine && thisEntity.myState == Entity.entityState.Alive)
        {
            handleMovement();
            if (Input.GetKey(KeyCode.Backspace))
            {
                myView.RPC("SpartanDeath", RpcTarget.All);
            }
        }
    }

    void handleMovement()
    {
        Vector3 moveVector = Vector3.zero;
        float moveMult = 1;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            thisEntity.myMovement = Entity.movementType.Run;
            moveMult = 2.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            thisEntity.myMovement = Entity.movementType.Sneak;
            moveMult = 0.5f;
        }
        else
        {
            thisEntity.myMovement = Entity.movementType.Walk;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            moveVector.x = Input.GetAxis("Horizontal");
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            moveVector.z = Input.GetAxis("Vertical");
        }
        if (moveVector.magnitude >= 1)
        {
            moveVector.Normalize();
        }
        mvVector = moveVector;
        moveVector *= speed * moveMult;
        this.transform.position += moveVector;
        camObj.transform.localPosition = new Vector3(0,5,0) - (moveVector / (speed*moveMult) * cameraLag) + (CameraOffsetCursor() * cameraCursorFollow);
    }  
    
    Vector3 CameraOffsetCursor()
    {
        Vector3 Offset = new Vector3(0, 0, 0);
        Vector3 relativeMouse = new Vector3((Input.mousePosition.x / Screen.width) - 0.5f, 0, (Input.mousePosition.y / Screen.height)-0.5f);
        if(relativeMouse.x > cameraCursorFollowTrigger)
        {
            Offset.x = relativeMouse.x - cameraCursorFollowTrigger;
        }
        if(relativeMouse.x < -cameraCursorFollowTrigger)
        {
            Offset.x = relativeMouse.x + cameraCursorFollowTrigger;
        }
        if (relativeMouse.z > cameraCursorFollowTrigger)
        {
            Offset.z = relativeMouse.z - cameraCursorFollowTrigger;
        }
        if (relativeMouse.z < -cameraCursorFollowTrigger)
        {
            Offset.z = relativeMouse.z + cameraCursorFollowTrigger;
        }
        return Offset;
    }

    void CameraFovHandling()
    {
        if (camObj.GetComponent<Camera>().orthographic)
        {
            float camFovMult = 1;
            if(thisEntity.myMovement == Entity.movementType.Aim && (mvVector.magnitude != 0))
            {
                camFovMult = 0.85f;
            }
            else if(thisEntity.myMovement == Entity.movementType.Run && (mvVector.magnitude != 0))
            {
                camFovMult = 0.85f;
            } else if (thisEntity.myMovement == Entity.movementType.Sneak && (mvVector.magnitude != 0))
            {
                camFovMult = 1.5f;
            }
            camObj.GetComponent<Camera>().orthographicSize = 5 * camFovMult;
        }
    }
    #region RPCS and Callers
    public void SpartanRespawnCall(RespawnPoint resPoint)
    {
        playerHUD.GetComponent<HUDScript>().respawnPanel.SetActive(false);
        print("IWILLSURVIVE");        
        this.GetComponent<PhotonView>().RPC("SpartanRespawn", RpcTarget.All,resPoint.transform.position , thisEntity.myTeam.ToString());
    }
    [PunRPC]
    public void SpartanRespawn(Vector3 pos, string teamString)
    {
        //test
        thisEntity.myState = Entity.entityState.Alive;
        transform.position = pos;
        thisEntity.health = thisEntity.maxHealth;
        if (teamString == "blue")
        {
            debugArrow.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            debugArrow.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }

    }
    public void TeamChangeCall(string teamstring)
    {
        this.GetComponent<PhotonView>().RPC("ChangeTeam", RpcTarget.All, teamstring);
    }
    public void NameChangeCall(string newname)
    {
        this.GetComponent<PhotonView>().RPC("ChangeName", RpcTarget.All, newname);
    }    
    public void SpartanDeath()
    {
        debugArrow.transform.Find("RotatingSprite").GetComponent<SpriteRenderer>().color = Color.black;
        if(PhotonNetwork.LocalPlayer == myView.Owner)
        {
            playerHUD.GetComponent<HUDScript>().respawnPanel.SetActive(true);
            print("I am Dead" + this.gameObject.name);
            spawnPoints = myMatch.GetRespawnPoints(thisEntity.myTeam);
            playerHUD.GetComponent<HUDScript>().respawnPanel.SetActive(true);
            playerHUD.GetComponent<HUDScript>().respawnPanel.GetComponent<RespawnUI>().Initialize(spawnPoints.ToArray(), this);
            print(spawnPoints);
        }        
    }
    [PunRPC]
    public void ChangeTeam(string teamstring)
    {
        if(teamstring == "blue")
        {
            thisEntity.myTeam = playerTeam.blue;
            debugArrow.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            thisEntity.myTeam = playerTeam.red;
            debugArrow.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }
    }
    [PunRPC]
    public void ChangeName(string newname)
    {
        this.gameObject.name = newname;
    }
    #endregion
}