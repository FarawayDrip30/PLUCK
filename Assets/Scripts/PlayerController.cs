using UnityEngine;
using UnityEngine.InputSystem.XR;

enum PlayerState
{
    Sun,
    Sun_Zoom,
    Plucking_Pull,
    Plucking_Fall
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform sunCamTransform;
    [SerializeField] Transform camLookAtTransform;
    [SerializeField] float fallCamDistance;
    [SerializeField] float fromWallForce;
    [SerializeField] float blockMoveSpeed;
    [SerializeField] float camPosLerpSpeed = 1;
    [SerializeField] float camRotLerpSpeed = 1;
    Quaternion camRotTarget;
    Vector3 camPosTarget;

    [SerializeField] float startFOV = 60;
    [SerializeField] float endFOV = 30;
    [SerializeField] float FOVLerpSpeed = 1;
    [SerializeField] float FOVPluckSpeed = 1;
    float targetFOV = 0;

    [SerializeField] float maxFreezeTime = 4;
    [SerializeField] float minFreezeTime = 2;
    float freezeTimer = 0;

    [SerializeField] float pullSpeed = 1.0f;

    [SerializeField] SunController sunController;
    [SerializeField] Sprite sunLookIcon;
    [SerializeField] Sprite sunHandIcon;

    Block currPlucking = null;
    Rigidbody currPluckingRB = null;

    PlayerState playerState = PlayerState.Sun;

    int pluckCameraRayLayerMask;

    void Start()
    {
        targetFOV = startFOV;

        pluckCameraRayLayerMask = ~LayerMask.GetMask("Wall");
    }

    void Update()
    {
        if (Input.GetButtonDown("Mouse Click"))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetButtonDown("Escape"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }



        if (playerState == PlayerState.Sun || playerState == PlayerState.Sun_Zoom)
        {
            sunController.SunUpdate();

            // We wanna keep them the same position but with independent rotation
            camLookAtTransform.position = sunCamTransform.position;

            // Set these before raycast so if it doesn't hit a pluckable object it goes back to normal
            camRotTarget = sunCamTransform.rotation;
            camPosTarget = sunCamTransform.position;
            UI.singleton.crosshair.sprite = sunLookIcon;

            RaycastHit hit;
            if (Physics.Raycast(sunCamTransform.position, sunCamTransform.forward, out hit, 1000))
            {
                targetFOV = startFOV + Vector3.Distance(sunCamTransform.position, hit.point);

                if (hit.collider.tag == "Pluckable")
                {
                    Block block = hit.collider.gameObject.GetComponent<Block>();
                    if (!block.placed)
                    {
                        UI.singleton.crosshair.sprite = sunHandIcon;

                        playerState = PlayerState.Sun_Zoom;
                        targetFOV = endFOV;
                        camLookAtTransform.LookAt(hit.collider.transform.position, Vector3.up);
                        camRotTarget = camLookAtTransform.rotation;

                        if (Input.GetButtonDown("Mouse Click"))
                        {
                            StartPlucking(block);
                        }
                    }
                }
            }
        }

        if (playerState == PlayerState.Plucking_Pull)
        {
            if (Input.GetButtonUp("Mouse Click"))
            {
                CancelPlucking();
            }

            targetFOV += FOVPluckSpeed * Time.deltaTime;
            currPlucking.transform.position -= camLookAtTransform.transform.forward * pullSpeed * Time.deltaTime;

            if (currPlucking.getCurrCollisions() <= 0)
            {
                StartFalling();
            }
        }

        if(playerState == PlayerState.Plucking_Fall)
        {
            if (freezeTimer > 0)
            {
                camLookAtTransform.position = new Vector3(currPlucking.transform.position.x, currPlucking.transform.position.y + fallCamDistance, currPlucking.transform.position.z);
                camLookAtTransform.LookAt(currPlucking.transform.position);

                SetCameraBetweenCurrObjectAndThingsInWay(fallCamDistance);

                camPosTarget = camLookAtTransform.position;
                camRotTarget = camLookAtTransform.rotation;
                targetFOV = startFOV;

                float moveX = Input.GetAxis("Horizontal");
                float moveZ = Input.GetAxis("Vertical");

                Vector3 moveVector = (camLookAtTransform.right * moveX) + (camLookAtTransform.up * moveZ);
                currPluckingRB.linearVelocity += (moveVector.normalized * blockMoveSpeed * Time.deltaTime);

                freezeTimer -= Time.deltaTime;
                if (freezeTimer <= 0)
                {
                    currPluckingRB.isKinematic = true;
                    currPlucking.placed = true;
                    freezeTimer = 0;
                }
                UI.singleton.fallTimerText.text = Mathf.Ceil(freezeTimer).ToString();
            }
            else
            {
                if(Input.GetButtonDown("Mouse Click"))
                {
                    UI.singleton.fallUI.SetActive(false);
                    UI.singleton.sunUI.SetActive(true);
                    playerState = PlayerState.Sun;
                }
            }
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, camPosTarget, camPosLerpSpeed * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, camRotTarget, camRotLerpSpeed * Time.deltaTime);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, FOVLerpSpeed * Time.deltaTime);
    }

    void StartPlucking(Block go)
    {
        currPlucking = go;
        currPluckingRB = currPlucking.GetComponent<Rigidbody>();

        playerState = PlayerState.Plucking_Pull;

        camLookAtTransform.position = new Vector3(0, currPlucking.transform.position.y, 0);
        camLookAtTransform.LookAt(currPlucking.transform.position);

        SetCameraBetweenCurrObjectAndThingsInWay();

        camPosTarget = camLookAtTransform.position;
        camRotTarget = camLookAtTransform.rotation;

        targetFOV = startFOV;
    }

    void CancelPlucking()
    { 
        currPlucking = null;
        currPluckingRB = null;

        // Player would have to be zoomed in on object to pluck it
        playerState = PlayerState.Sun_Zoom;
    }

    void StartFalling()
    {
        playerState = PlayerState.Plucking_Fall;
        currPlucking.StartFalling();
        currPluckingRB.linearVelocity += -camLookAtTransform.transform.forward * fromWallForce;
        currPluckingRB.angularVelocity = new Vector3(Random.Range(100, 300), Random.Range(100, 300), Random.Range(100, 300));
        freezeTimer = Random.Range(minFreezeTime, maxFreezeTime);

        UI.singleton.fallUI.SetActive(true);
        UI.singleton.sunUI.SetActive(false);
        UI.singleton.fallTimerText.text = Mathf.Ceil(freezeTimer).ToString();
    }

    void SetCameraBetweenCurrObjectAndThingsInWay(float maxDistance = 100)
    {
        // Send ray out to towards centre to see if anything is blocking the camera and if so, just the camera to there
        RaycastHit hit;
        if (Physics.Raycast(currPlucking.transform.position, -camLookAtTransform.forward, out hit, maxDistance, pluckCameraRayLayerMask))
        {
            // Add forward so it's not in the object
            camLookAtTransform.position = hit.point + camLookAtTransform.forward;
        }
    }
}
