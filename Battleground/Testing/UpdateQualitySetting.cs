using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateQualitySetting : MonoBehaviour {

    public Button UpQuality;
    public Button DownQuality;
    public Text qualityText;

    void Start() {
        qualityText.text = QualityString;
    }

    public void ClickQualityUp() {
        QualitySettings.IncreaseLevel();
        qualityText.text = QualityString;
    }

    public void ClickQualityDown() {
        QualitySettings.DecreaseLevel();
        qualityText.text = QualityString;
    }

    string QualityString {
        get {
            return QualitySettings.names[QualitySettings.GetQualityLevel()];
        }
    }
}
