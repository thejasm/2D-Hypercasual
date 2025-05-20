using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBarTemp: MonoBehaviour
{
    public PlayerCore playerCore;

    void Update()
    {
        Vector3 scale = new Vector3(
            (float)playerCore.exp / playerCore.expCap,
            this.transform.localScale.y,
            this.transform.localScale.z
        );
        this.transform.localScale = scale;
    }
}
