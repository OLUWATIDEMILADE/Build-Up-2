using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform[] roomPositions;  // Assign room positions in Inspector
    public Transform uiCanvas;  // Assign the UI Canvas
    public Text storyText;
    public Button nextButton, prevButton;
    public float transitionSpeed = 2f;

    private int currentRoomIndex = 0;

    private string[] storyLines =
    {
        "You step into the abandoned mansion. The air is thick with dust and memories...",
        "The living room feels eerie. A torn diary lies on a table...",
        "In the bedroom, a music box plays on its own. Who turned it on?",
        "The library holds a secret letter. The lonely girl’s story unfolds...",
        "In the attic, the truth awaits. A shadow lurks behind the mirror..."
    };

    void Start()
    {
        if (!storyText || !nextButton || !prevButton || !uiCanvas)
        {
            Debug.LogError("UI elements are not assigned in the Inspector!");
            return;
        }

        storyText.text = storyLines[currentRoomIndex];

        nextButton.onClick.AddListener(NextRoom);
        prevButton.onClick.AddListener(PreviousRoom);

        UpdateButtons();
    }

    void NextRoom()
    {
        if (currentRoomIndex < roomPositions.Length - 1)
        {
            currentRoomIndex++;
            StartCoroutine(MoveCamera(roomPositions[currentRoomIndex]));
            storyText.text = storyLines[currentRoomIndex];
        }
        UpdateButtons();
    }

    void PreviousRoom()
    {
        if (currentRoomIndex > 0)
        {
            currentRoomIndex--;
            StartCoroutine(MoveCamera(roomPositions[currentRoomIndex]));
            storyText.text = storyLines[currentRoomIndex];
        }
        UpdateButtons();
    }

    IEnumerator MoveCamera(Transform targetPosition)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startUIPos = uiCanvas.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            // Move camera and UI together
            transform.position = Vector3.Lerp(startPos, targetPosition.position, t);
            transform.rotation = Quaternion.Slerp(startRot, targetPosition.rotation, t);
            uiCanvas.position = Vector3.Lerp(startUIPos, targetPosition.position + transform.forward * 2f, t);

            yield return null;
        }

        // Ensure final position matches exactly
        transform.position = targetPosition.position;
        transform.rotation = targetPosition.rotation;
        uiCanvas.position = targetPosition.position + transform.forward * 2f;

        // Make the UI face the camera
        FaceUIToCamera();
    }

    void FaceUIToCamera()
    {
        uiCanvas.LookAt(transform);
        uiCanvas.Rotate(0f, 180f, 0f); // Ensure the text is readable
    }

    void UpdateButtons()
    {
        prevButton.interactable = currentRoomIndex > 0;
        nextButton.interactable = currentRoomIndex < roomPositions.Length - 1;
    }
}
