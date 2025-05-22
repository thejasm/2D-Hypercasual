using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops 
    {
        public GameObject prefab;
        public float dropRate;
    }

    public List<Drops> drops;

    public void DropLoot() {
        float rand = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops d in drops) {
            if (rand <= d.dropRate) {
                possibleDrops.Add(d);
                //break;
            }
        }

        if (possibleDrops.Count > 0) {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            GameObject drop = Instantiate(drops.prefab, transform.position, Quaternion.identity);
            if (drop.CompareTag("ExpGem")) {
                FindObjectOfType<PlayerCore>().expGems.Add(drop);
            }
        }
    }

}
