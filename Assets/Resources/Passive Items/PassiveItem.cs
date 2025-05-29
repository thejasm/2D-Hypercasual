using UnityEngine;
using UnityEngine.UI; // Keep for UI components

public class PassiveItem: MonoBehaviour {
    public PassiveItemScriptableObject stats;
    private bool isPickedUp = false;

    public virtual void ApplyModifier() { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isPickedUp) return;
        if (collision.gameObject.CompareTag("Player")) {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            if (player != null) {
                isPickedUp = true;
                player.AddItem(this.gameObject);
                this.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}