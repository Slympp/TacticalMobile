using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTeamManager : MonoBehaviour {

    public GameObject TeamManagerUI;

    public void Toggle(bool b) {
        TeamManagerUI.SetActive(b);
    }
}

