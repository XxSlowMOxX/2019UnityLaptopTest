using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CubeMovement : MonoBehaviour
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

    // Start is called before the first frame update
    void Awake()
    {
        //GameObject cameraObject = Instantiate(camObj, this.transform);
        //camObj = cameraObject;
        this.GetComponent<EscMenu>().ShowEscMenu(false);
        thisEntity = this.GetComponent<Entity>();
        myMatch = FindObjectOfType<GameManager2>();
    }

    // Update is called once per frame
    void Update()
    {
        PhotonView myView = this.GetComponent<PhotonView>();
        if (myView.IsMine)
        {
            
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
                    print("Peng");
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
                if(Physics.Raycast(this.transform.position, debugArrow.transform.forward, out hit, grabReach))
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
        PhotonView myView = this.GetComponent<PhotonView>();
        if (myView.IsMine)
        {
            handleMovement();
            if (Input.GetKey(KeyCode.Backspace))
            {
                SpartanDeath();
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

    public void SpartanDeath()
    {
        playerHUD.GetComponent<HUDScript>().respawnPanel.SetActive(true);
        print("I am Dead" + this.gameObject.name);
        spawnPoints = myMatch.GetRespawnPoints(PhotonNetwork.LocalPlayer);
        print(spawnPoints);
    }

    public void SpartanRespawn()
    {
        playerHUD.GetComponent<HUDScript>().respawnPanel.SetActive(false);
        print("IWILLSURVIVE");
        thisEntity.myState = Entity.entityState.Alive;
        
    }
}