using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionMovement : AIAction {

    public Vector2 Destination;

    public void GetHighestScore(float[,] map, int xPos, int yPos, int maxX, int maxY, int movementPoints) {

        float highest = -1;
        float dist = movementPoints + 1;
        int xHighest = 0;
        int yHighest = 0;

        for (int x = -movementPoints; x <= movementPoints; x++)
            for (int y = -movementPoints; y <= movementPoints; y++) {

                if ((Mathf.Abs(x + y)) <= movementPoints &&
                    (Mathf.Abs(x) + Mathf.Abs(y) >= movementPoints) &&
                    (Mathf.Abs(x) + Mathf.Abs(y) <= movementPoints) &&
                    xPos + x >= 0 && xPos + x < maxX &&
                    yPos + y >= 0 && yPos + y < maxY &&
                    map[xPos + x, yPos + y] != -1) {

                    Debug.Log(map[xPos + x, yPos + y]);

                    int newDist = Mathf.Abs(x) + Mathf.Abs(y);
                    if (map[xPos + x, yPos + y] >= highest) {
                        if (newDist < dist) {
                            highest = map[xPos + x, yPos + y];
                            dist = newDist;
                            xHighest = xPos + x;
                            yHighest = yPos + y;
                        }
                    }
                }
            }

        Destination = new Vector2(xHighest, yHighest);
    }

    public void GetLowestScore(float[,] map, int xPos, int yPos, int maxX, int maxY, int movementPoints) {

        float lowest = 1;
        int xLowest = 0;
        int yLowest = 0;

        for (int x = -movementPoints; x <= movementPoints; x++)
            for (int y = -movementPoints; y <= movementPoints; y++) {

                if ((Mathf.Abs(x + y)) <= movementPoints &&
                    (Mathf.Abs(x) + Mathf.Abs(y) >= movementPoints) &&
                    (Mathf.Abs(x) + Mathf.Abs(y) <= movementPoints) &&
                    xPos + x >= 0 && xPos + x < maxX &&
                    yPos + y >= 0 && yPos + y < maxY &&
                    map[xPos + x, yPos + y] != -1) {

                    if (map[xPos + x, yPos + y] < lowest) {
                        lowest = map[xPos + x, yPos + y];
                        xLowest = xPos + x;
                        yLowest = yPos + y;
                    }
                }
            }

        Destination = new Vector2(xLowest, yLowest);
    }
}
