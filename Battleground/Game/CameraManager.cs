using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour {

    public Button   ToggleCameraButton;
    public Camera   MainCamera, TacticalCamera;
    public Canvas   Background;

    public Unit     FollowedUnit { private get; set; }
    private Vector3 LastFollowedUnitPos;
    private Vector3 MainCameraDefaultPos = new Vector3(8, 17, -8);

    public List<UIAnchorTransform>      HealthSliders;
    private static int                  MainCameraHealthSliderOffset = 90;
    private static int                  TacticalCameraHealthSliderOffset = 40;


    void Start () {

        HealthSliders = new List<UIAnchorTransform>();
        TacticalCamera.enabled = false;
        ToggleCameraButton.onClick.AddListener(ToggleCamera);
    }

    private void Update() {
        
        if (MainCamera.enabled && FollowedUnit != null && LastFollowedUnitPos != FollowedUnit.transform.position) {
            CenterMainCamera(LastFollowedUnitPos = FollowedUnit.transform.position);
        }

        //if (TacticalCamera.enabled) TODO: Pan/Zoom with 2 fingers
    }

    public void InitBackground(Sprite image) {
        Background.worldCamera = MainCamera;
        Background.GetComponentInChildren<Image>().sprite = image;
    }

    public void ToggleCamera() {

        MainCamera.enabled = !MainCamera.enabled;
        TacticalCamera.enabled = !TacticalCamera.enabled;

        Background.worldCamera = (MainCamera.enabled) ? MainCamera : TacticalCamera;
        AdjustHealthSliders(MainCamera.enabled ? MainCamera : TacticalCamera);
    }

    private void AdjustHealthSliders(Camera camera) {

        for (int i = 0; i < HealthSliders.Count; i++) {

            if (HealthSliders[i].gameObject.activeSelf) {
                if (camera == MainCamera)
                    HealthSliders[i].screenOffset = new Vector3(0, MainCameraHealthSliderOffset, 0);
                else if (camera == TacticalCamera)
                    HealthSliders[i].screenOffset = new Vector3(0, TacticalCameraHealthSliderOffset, 0);
            }
        }
    }

    public void CenterMainCamera(Vector3 position) {
        MainCamera.transform.position = MainCameraDefaultPos + position;
    }

    public void CenterTacticalCamera(Vector3 position) {
        TacticalCamera.transform.position = new Vector3(-0.5f, 10, 0) + position;
        TacticalCamera.orthographicSize = 14f;
    }
}
