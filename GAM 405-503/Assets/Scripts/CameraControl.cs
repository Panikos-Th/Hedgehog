using UnityEngine;
using UnityEngine.InputSystem;
public class CameraControl : MonoBehaviour

{
    [SerializeField] private float sensetivity = 5f;
    [SerializeField] private Vector2 clampValue = new Vector2(-90f, 90f);
    public bool isActive = false;
    

    public InputActionReference cameraAction;

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        Vector2 cameraInput = new Vector2(-cameraAction.action.ReadValue<Vector2>().y, 0f);
        cameraInput *= sensetivity;
        transform.Rotate(cameraInput); 
        float cameraEulerX = transform.rotation.eulerAngles.x > 180f ? transform.rotation.eulerAngles.x - 360f : transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(Mathf.Clamp(cameraEulerX,clampValue.x,clampValue.y), transform.rotation.eulerAngles.y, 0f);
        //Debug.Log(transform.rotation.eulerAngles.x);
    }
}
