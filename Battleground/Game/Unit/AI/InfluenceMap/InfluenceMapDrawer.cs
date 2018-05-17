using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapDrawer : MonoBehaviour {

    public float[,] influenceMap { private get; set; }

    private void OnDrawGizmos() {
        
        if (influenceMap != null) {
            for (int x = 0; x < influenceMap.GetLength(0); x++)
                for (int z = 0; z < influenceMap.GetLength(1); z++) {

                    if (influenceMap[x, z] != 0) {
                        float color = 1 - influenceMap[x, z] / 100;
                        Gizmos.color = new Color(color, 1, 1, 1f);
                        Gizmos.DrawCube(new Vector3(x, 4, z), new Vector3(1, 1, 1));
                    }
                }
        }
    }
}
