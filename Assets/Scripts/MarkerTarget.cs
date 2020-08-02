using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTarget : MonoBehaviour
{
    public int pointId = 0;
    public void OnTrackBegin ()
    {
        CatmullRom.instance.AddPoint(this);
        //Debug.LogError("asdded");
    }

    public void OnTrackEnd ()
    {
        CatmullRom.instance.RemovePoint(this);
        //Debug.LogError("removed");
    }
}
