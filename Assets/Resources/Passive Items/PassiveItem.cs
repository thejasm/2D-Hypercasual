using UnityEngine;
using UnityEngine.UI; // Keep for UI components

public class PassiveItem: MonoBehaviour {
    public PassiveItemScriptableObject stats;

    [Header("Pointer Behavior")]
    public GameObject pointerHolderSource;
    public Canvas targetCanvas;
    public Transform arrowTr;
    public Transform iconTr;
    public float maxDist = 20f;
    public float screenBorder = 30f;
    public float arrowOrbitRadius = 30f;
    private RectTransform pointerRect;
    private RectTransform iconRect;
    private RectTransform arrowRect;
    private Transform playerT;
    private Camera cam;
    private bool isPickedUp = false;

    private bool InitializePointerAndReferences() {
        pointerHolderSource.transform.SetParent(targetCanvas.transform, false);
        pointerRect = pointerHolderSource.GetComponent<RectTransform>();
        if (pointerRect == null) pointerRect = pointerHolderSource.AddComponent<RectTransform>();

        if (iconTr == null || (iconRect = iconTr.GetComponent<RectTransform>()) == null) {
            Debug.LogError("Icon (iconTr) missing or not a RectTransform!", this); return false;
        }
        iconRect.localPosition = Vector3.zero;
        iconRect.localRotation = Quaternion.identity;

        if (arrowTr == null || (arrowRect = arrowTr.GetComponent<RectTransform>()) == null) {
            Debug.LogError("Arrow (arrowTr) missing or not a RectTransform!", this); return false;
        }
        arrowRect.localRotation = Quaternion.identity;
        return true;
    }

    void Start() {
        cam = Camera.main;
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) playerT = pObj.transform;

        if (pointerHolderSource == null || targetCanvas == null) {
            Debug.LogError("PointerHolderSource or TargetCanvas not assigned!", this);
            enabled = false; return;
        }

        if (!InitializePointerAndReferences()) {
            enabled = false; return;
        }
        pointerRect.gameObject.SetActive(false);
    }

    void LateUpdate() {
        if (isPickedUp || playerT == null || cam == null || pointerRect == null) {
            if (pointerRect != null && pointerRect.gameObject.activeSelf && isPickedUp) {
                pointerRect.gameObject.SetActive(false);
            }
            return;
        }

        Vector3 itemPos = transform.position;
        Vector3 playerPos = playerT.position;
        float distToItem = Vector3.Distance(itemPos, playerPos);
        Vector3 itemViewportPos = cam.WorldToViewportPoint(itemPos);
        bool isOnScreen = itemViewportPos.z > 0 &&
                          itemViewportPos.x > 0 && itemViewportPos.x < 1 &&
                          itemViewportPos.y > 0 && itemViewportPos.y < 1;

        if (!isOnScreen || distToItem > maxDist) {
            pointerRect.gameObject.SetActive(true);

            Vector3 screenPos = cam.WorldToScreenPoint(itemPos);
            if (itemViewportPos.z < 0) {
                screenPos.x = Screen.width - screenPos.x;
                screenPos.y = Screen.height - screenPos.y;
            }
            float cX = Mathf.Clamp(screenPos.x, screenBorder, Screen.width - screenBorder);
            float cY = Mathf.Clamp(screenPos.y, screenBorder, Screen.height - screenBorder);
            pointerRect.position = new Vector3(cX, cY, 0);

            if (arrowRect != null) {
                Vector3 offsetDir = (itemPos - pointerRect.position).normalized;
                Vector3 rotDir = (itemPos - playerPos).normalized;

                arrowRect.localPosition = (offsetDir != Vector3.zero) ?
                    (Vector2)offsetDir * arrowOrbitRadius :
                    new Vector2(arrowOrbitRadius, 0);

                if (rotDir != Vector3.zero) {
                    float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg;
                    arrowRect.localEulerAngles = new Vector3(0, 0, angle);
                }
                else {
                    arrowRect.localEulerAngles = Vector3.zero;
                }
            }
        }
        else {
            pointerRect.gameObject.SetActive(false);
        }
    }

    public virtual void ApplyModifier() { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isPickedUp) return;
        if (collision.gameObject.CompareTag("Player")) {
            PlayerCore player = collision.gameObject.GetComponent<PlayerCore>();
            if (player != null) {
                isPickedUp = true;
                player.AddItem(this.gameObject);
                this.GetComponent<Collider2D>().enabled = false;
                if (pointerRect != null) {
                    pointerRect.gameObject.SetActive(false);
                }
            }
        }
    }
}