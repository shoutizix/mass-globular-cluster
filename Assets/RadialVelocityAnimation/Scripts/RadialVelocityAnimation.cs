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
    [SerializeField] private float timeToWait = 1f;

    [Header("Objects")]
    [SerializeField] private GameObject eyeGameObject;

    [Header("Arrows")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float animationArrowsTime = 1f;
    [SerializeField] private float arrowLength = 0f;
    [SerializeField] private Vector3 arrowPositionRelatedToScreen = new Vector3(0, 0, 0);
    
    [Header("Fade Out Image")]
    [SerializeField] private FadeInUI textArrow;

    private Arrow arrowLengthLeft, arrowLengthRight;
    private bool hasMoved = false;
    private bool hasEndedWaitMove = false;
    private bool arrowHaveMoved = false;
    private CanvasGroup canvasGroup;
    private CameraController cameraController;
    private Coroutine waitAndMove;
    private Coroutine arrowsAnimation;
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

        if (arrowPrefab)
        {
            arrowLengthLeft = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Arrow>();
            arrowLengthLeft.SetComponents(Vector3.zero);
            arrowLengthLeft.name = "Arrow Left";

            arrowLengthRight = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Arrow>();
            arrowLengthRight.SetComponents(Vector3.zero);
            arrowLengthRight.name = "Arrow Right";
        }
        
        if (eyeGameObject)
        {
            eyeGameObject.SetActive(false);
        }
    }
    void Update()
    {
        // If the Slide is inactive the alpha is equal to 0
        if (canvasGroup.alpha != 1) {
            // If the Slide is not showed anymore reset the animation
            if (hasMoved) Reset();
            return;
        } else 
        {
            if (eyeGameObject)
            {
                eyeGameObject.SetActive(true);
            }
        }

        if (cameraController.cameraMoving == null && hasMoved == false)
        {
            waitAndMove = StartCoroutine(WaitSecondsAndMoveCamera(timeToWait));
            hasMoved = true;
        }

        // Check if the animation of the camera has already been done
        if (cameraController.cameraMoving == null && hasEndedWaitMove && !arrowHaveMoved && arrowsAnimation == null)
        {
            // Animation of the arrows
            arrowsAnimation = StartCoroutine(AnimationArrows(animationArrowsTime));
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
        hasEndedWaitMove = true;
    }

    private IEnumerator AnimationArrows(float animationDuration)
    {
        Camera mainCamera = cameraController.GetMainCamera();
        Vector3 centerScreen = new Vector3(mainCamera.pixelWidth/2, mainCamera.pixelHeight/2, 80);
        Vector3 positionArrows = mainCamera.ScreenToWorldPoint(centerScreen);

        arrowLengthLeft.transform.position = positionArrows;
        arrowLengthRight.transform.position = positionArrows;

        float time = 0;
        Vector3 startPosition = Vector3.zero;

        Vector3 arrowTargetComponent = Vector3.forward * arrowLength;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            t = t * t * (3f - 2f * t);  // Apply some smoothing
            
            arrowLengthRight.SetComponents(Vector3.Lerp(startPosition, arrowTargetComponent, t));
            arrowLengthLeft.SetComponents(Vector3.Lerp(startPosition, -arrowTargetComponent, t));
            yield return null;
        }

        if (textArrow) 
        {
            textArrow.TriggerFadeIn();
        }
        arrowHaveMoved = true;
        arrowsAnimation = null;
    }

    public void Reset() {
        StopAllCoroutines();

        hasMoved = false;
        arrowHaveMoved = false;
        hasEndedWaitMove = false;

        if (eyeGameObject)
        {
            eyeGameObject.SetActive(false);
        }

        if (textArrow) 
        {
            textArrow.TriggerReset();
        }

        // Reset arrows
        arrowLengthRight.SetComponents(Vector3.zero);
        arrowLengthLeft.SetComponents(Vector3.zero);
    }
}
