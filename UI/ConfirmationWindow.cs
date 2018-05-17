using LlockhamIndustries.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConfirmationWindow : MonoBehaviour {

    public Button               CancelAction;
    public Button               ValidAction;

    public Vector3              LocalOffset;
    public Vector3              ScreenOffset;
    public RectTransform        MyCanvas;

    public ProjectionRenderer   CellToFollow { private get; set; } = null;
    public ConfirmationStatus   ConfirmationStatus { get; private set; } = ConfirmationStatus.None;

    void Start () {

    }

    void LateUpdate() {

        // Translate our anchored position into world space.
        if (CellToFollow != null) {

            Vector3 worldPoint = CellToFollow.transform.TransformPoint(LocalOffset);

            // Translate the world position into viewport space.
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(worldPoint);

            // Canvas local coordinates are relative to its center, 
            // so we offset by half. We also discard the depth.
            viewportPoint -= 0.5f * Vector3.one;
            viewportPoint.z = 0;

            // Scale our position by the canvas size, 
            // so we line up regardless of resolution & canvas scaling.
            Rect rect = MyCanvas.rect;
            viewportPoint.x *= rect.width;
            viewportPoint.y *= rect.height;

            // Add the canvas space offset and apply the new position.
            transform.localPosition = viewportPoint + ScreenOffset;
        }
    }

    public void SetActive(bool active) {
        transform.GetChild(0).gameObject.SetActive(active);
        ConfirmationStatus = ConfirmationStatus.None;
    }

    public void SetConfirmationStatus(int status) {
        ConfirmationStatus = (ConfirmationStatus)status;
    }
}

public enum ConfirmationStatus {
    None,
    Valid,
    Cancel
}
