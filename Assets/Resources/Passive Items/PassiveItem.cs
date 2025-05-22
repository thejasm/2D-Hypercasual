using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    public PassiveItemScriptableObject stats;

    public virtual void ApplyModifier() { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            if (player != null) {
                player.AddItem(this.gameObject);
                //Destroy(this.gameObject);
            }
        }
    }
}
