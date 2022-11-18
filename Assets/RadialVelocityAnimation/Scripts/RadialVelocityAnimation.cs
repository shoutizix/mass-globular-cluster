using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialVelocityAnimation : MonoBehaviour
{
    [Header("Orientation")]
    [SerializeField] private Vector3 position = new Vector3(0, 0, -100);
    [SerializeField] private Vector3 lookAt = new Vector3(0, 0, 0);
    [SerializeField] private float moveTime = 1f;

    [Header("Controls")]
    [SerializeField] private float fieldOfView = 20;
    public float timeToWait = 1f;

    [Header("Image Eye")]
    [SerializeField] private GameObject eyeGameObject;
    private bool hasMoved = false;
    private CanvasGroup canvasGroup;
    private CameraController cameraController;
    private Coroutine waitAndMove;
    private void Awake() {
        if (!TryGetComponent(out canvasGroup))
        {
            Debug.LogWarning("No CanvasGroup component found");
            return;
        }

        if (!TryGetComponent(out cameraController))
        {
            Debug.LogWarning("No CameraController component found");
            return;
        }
        
        if (eyeGameObject != null)
        {
            eyeGameObject.SetActive(false);
        }
    }
    void Update()
    {
        // If the Slide is inactive the alpha is equal to 0
        if (canvasGroup.alpha == 0) {
            // If the Slide is not showed anymore reset the animation
            if (hasMoved) Reset();
            return;
        } else 
        {
            if (eyeGameObject != null)
            {
                eyeGameObject.SetActive(true);
            }
        }

        if (cameraController.cameraMoving == null && hasMoved == false)
        {
            waitAndMove = StartCoroutine(WaitSecondsAndMoveCamera(timeToWait));
            hasMoved = true;
        }
    }

    private IEnumerator WaitSecondsAndMoveCamera(float timeToWait)
    {
        // Waits for timeToWait seconds before moving the camera
        yield return new WaitForSeconds(timeToWait);

        Camera mainCamera = cameraController.GetMainCamera();

        // Always put the camera in perspective mode when moving
        mainCamera.orthographic = false;
        Quaternion targetRotation = Quaternion.LookRotation(lookAt - position);

        cameraController.cameraMoving = StartCoroutine(cameraController.MoveToPosition(mainCamera.transform.position, position, mainCamera.transform.rotation,
                targetRotation, mainCamera.fieldOfView, fieldOfView, moveTime, lookAt));

        waitAndMove = null;
    }

    public void Reset() {
        hasMoved = false;
        if (eyeGameObject != null)
        {
            eyeGameObject.SetActive(false);
        }
    }
}
