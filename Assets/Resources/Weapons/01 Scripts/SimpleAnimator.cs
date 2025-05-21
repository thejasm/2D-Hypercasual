using UnityEngine;
using System.Linq;

public class SimpleAnimator : MonoBehaviour {
    public string spriteFolderPath;
    public float fps = 12f;

    new SpriteRenderer renderer;
    Sprite[] frames;
    int currentFrameIndex;
    float timer;

    void Start() {
        renderer = GetComponent<SpriteRenderer>();
        LoadSprites();
        if (frames.Length > 0) {
            renderer.sprite = frames[0];
        }
    }

    void LoadSprites() {
        frames = Resources.LoadAll<Sprite>(spriteFolderPath).OrderBy(s => s.name).ToArray();
        if (frames.Length == 0) {
            Debug.LogError("No sprites found in folder: " + spriteFolderPath);
        }
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer >= 1f / fps) {
            timer = 0f;
            currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
            renderer.sprite = frames[currentFrameIndex];
        }
    }
}
