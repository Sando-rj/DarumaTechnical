
using System.Linq;
using NaughtyAttributes;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FPSController : NetworkBehaviour
{
    [BoxGroup("PlayerParts")]
    public Transform Head;
    [BoxGroup("PlayerParts")]
    public Vector3 RightHandPosition;
    [BoxGroup("PlayerParts")]   
    public Vector3 LeftHandPosition;

    [BoxGroup("Stats")]
    public float movementSpeed = 1;
    [BoxGroup("Stats")]
    public float gazeSpeed = 1;
    [BoxGroup("Stats")]
    public float gazeDistance = 2;

    [BoxGroup("UI")]
    public TMP_Text UIText;

#region PrivateFields
    private float horizontalRotation = 0;
    private float verticalRotation = 0;
    private GameObject currentFocus = null;
    private bool isHandFull = false;
    private GameObject heldItem = null;

    NetworkVariable<ulong> playerId = new NetworkVariable<ulong>();
#endregion

    public override void OnNetworkSpawn(){
        this.playerId.Value = NetworkObject.OwnerClientId;

        if(!IsOwner) {
            this.enabled = false;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update(){
        MoveOnNetwork();
        UpdateFocus();
        Interaction();
    }

    void OnEnable(){
        EventManager.instance.Pick += Grab;
        EventManager.instance.Drop += Drop;
    }
    void OnDisable(){
        EventManager.instance.Pick -= Grab;
        EventManager.instance.Drop += Drop;
    }

#region NetworkFunctions
    void MoveOnNetwork(){
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalMouseAxis = Input.GetAxis("Mouse X");
        float verticalMouseAxis = Input.GetAxis("Mouse Y");

        Vector3 movement = new Vector3(horizontalAxis, 0, verticalAxis);
        Vector3 gaze = new Vector3(horizontalMouseAxis, verticalMouseAxis, 0);

        MoveServerRpc(movement, gaze);
        if(IsOwner) MovePositionAndGaze(movement, gaze);
    }

    [Rpc(SendTo.Server)]
    void MoveServerRpc(Vector3 movement, Vector3 gaze){
        MoveClientRpc(movement, gaze);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void MoveClientRpc(Vector3 movement, Vector3 gaze){
        if(!IsOwner) MovePositionAndGaze(movement, gaze);
    }
#endregion

#region UpdateFunctions
    void MovePositionAndGaze(Vector3 movement, Vector3 gaze){
        UpdatePosition(movement);
        UpdateGaze(gaze);
    }

    void UpdatePosition(Vector3 movement){
        Vector3 direction = movement;
        transform.Translate(movementSpeed * Time.deltaTime * direction);
    }

    void UpdateGaze(Vector3 gaze){
        horizontalRotation += gaze.x * gazeSpeed;
        verticalRotation -= gaze.y * gazeSpeed;

        Quaternion bodyRotation = Quaternion.Euler(0, horizontalRotation, 0);
        Quaternion headRotation = Quaternion.Euler(verticalRotation, 0, 0);

        transform.rotation = bodyRotation;
        Head.transform.localRotation = headRotation;
    }

    void UpdateFocus(){
        bool hasHit = Physics.Raycast(Head.position, Head.forward, out RaycastHit hitInfo, gazeDistance);

        if(hasHit){
            IFocusable newFocus = hitInfo.transform.GetComponent<IFocusable>();

            // If we hit a NEW focusable Item, we switch focus to it 
            if(newFocus != null && currentFocus != hitInfo.transform.gameObject){
                ChangeFocus(hitInfo.transform.gameObject);
            }
            // If we did not and an item is currently focused, we need to reset the focus
            else if(newFocus == null && currentFocus) {
                ResetFocus();
            }
        }
        // No item hit, we need to reset the focus
        else if(currentFocus){
            ResetFocus();
        } 
    }

    void Interaction()
    {
        if(currentFocus && Input.GetKeyDown(KeyCode.E)){
            EventManager.instance.OnInteract(heldItem);
        }
        else if(currentFocus && CanPickItem(currentFocus) && Input.GetKeyDown(KeyCode.G)){
            EventManager.instance.OnPick(this.GetPlayerId(), RightHandPosition, currentFocus);
        }
        else if(isHandFull && Input.GetKeyDown(KeyCode.R)){
            EventManager.instance.OnDrop(this.GetPlayerId(), heldItem);
        }
    }
#endregion

#region Actions
    void Grab(ulong playerId, Vector3 handPosition, GameObject item){
        if(this.GetPlayerId() == playerId){
            heldItem = currentFocus;
            isHandFull = true;
        }
    }

    void Drop(ulong playerId, GameObject item){
        if(this.GetPlayerId() == playerId)
            isHandFull = false;
    }
#endregion

#region Helpers
    public ulong GetPlayerId(){
        return this.playerId.Value;
    }
    bool CanPickItem(GameObject item){
        bool pickable = item?.GetComponent<IPickable>() != null;
        return !isHandFull && pickable;
    } 
    void ChangeFocus(GameObject item){
        EventManager.instance.OnFocus(item);
        currentFocus = item;
    }
    void ResetFocus(){
        EventManager.instance.OnFocus(null);
        currentFocus = null;
    }
#endregion
}
