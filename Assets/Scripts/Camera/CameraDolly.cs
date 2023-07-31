using UnityEngine;
using Utility;

[RequireComponent(typeof(Camera))]
public class CameraDolly : MonoBehaviour
{
    [Header("Camera Target")]
    [Rename("Target Player")] private PlayerController C_targetPlayer = null;

    [Header("Modifier Varaibles")]
    [Rename("Look Strength")] public float f_lookSrength;
    [Rename("Focus Radius")] public float f_focusRadius;
    [Rename("Offset")] public Vector3 S_offsetVector;


    private Camera C_camera;
    private Vector3 S_playerPosition;
    private Vector3 S_playerLookDirection;


    // Start is called before the first frame update
    void Start()
    {
        C_camera = GetComponent<Camera>();
        C_targetPlayer = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        S_playerPosition = C_targetPlayer.transform.position;
        S_playerLookDirection = C_targetPlayer.GetRotationDirection();
        RaycastHit hitInfo;
        Physics.Raycast(C_camera.transform.position, C_camera.transform.forward, out hitInfo, S_offsetVector.y * 1.2f);
        Vector3 cameraCenterPos = new Vector3(hitInfo.point.x, S_playerPosition.y, hitInfo.point.z);

        Vector3 lookOffset = (S_playerPosition + S_playerLookDirection * f_focusRadius) / 2.0f;
        Vector3 nextCameraPos = S_offsetVector + lookOffset;

        float playerDistance = Vector3.Distance(S_playerPosition, cameraCenterPos);

        float t = 1f;
        if (f_focusRadius < playerDistance)
        {
            t = Mathf.Min(t, f_focusRadius / playerDistance);
            nextCameraPos += (S_playerPosition - cameraCenterPos).normalized * (t * (f_focusRadius));
        }

        C_camera.transform.position = Vector3.Lerp(C_camera.transform.position, nextCameraPos, t * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
    }
}
