using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatabasePreset", menuName = "DatabasePreset")]
public class DatabasePreset : ScriptableObject {

    public string           Name;
    public string           InputFilesType;
    public string           InputFilesPath;
    public List<string>     Infos;
}
