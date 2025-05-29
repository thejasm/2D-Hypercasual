using UnityEngine;
using System.Collections;
using InteliMapPro;

public class InfiniteChunkLoader: MonoBehaviour {
    public InteliMapGenerator generator;
    public Transform player;

    public int chunkSize = 25;
    public int radius = 1;

    private Grid grid;
    private Vector2Int currentChunk;
    private Coroutine chunkLoadingCoroutine;

    void Start() {
        grid = generator.GetComponentInParent<Grid>();

        currentChunk = GetChunkCoord(player.transform.position);
        LoadChunks(currentChunk);
    }

    void Update() {
        Vector2Int newCurrentChunk = GetChunkCoord(player.transform.position);
        if (newCurrentChunk != currentChunk) {
            currentChunk = newCurrentChunk;
            LoadChunks(currentChunk);
        }
    }

    Vector2Int GetChunkCoord(Vector3 worldPos) {
        Vector3Int cellPos = grid.WorldToCell(worldPos);
        return new Vector2Int(
            Mathf.FloorToInt((float)cellPos.x / chunkSize),
            Mathf.FloorToInt((float)cellPos.y / chunkSize)
        );
    }

    void LoadChunks(Vector2Int centerChunk) {
        if (chunkLoadingCoroutine != null) {
            StopCoroutine(chunkLoadingCoroutine);
        }
        chunkLoadingCoroutine = StartCoroutine(LoadChunksCoroutine(centerChunk));
    }

    IEnumerator LoadChunksCoroutine(Vector2Int centerChunk) {
        for (int yOffset = -radius; yOffset <= radius; yOffset++) {
            for (int xOffset = -radius; xOffset <= radius; xOffset++) {
                Vector2Int chunkToLoad = new Vector2Int(centerChunk.x + xOffset, centerChunk.y + yOffset);

                while (generator.IsAsyncOperationInProgress) yield return null;

                BoundsInt chunkBounds = new BoundsInt(
                    chunkToLoad.x * chunkSize, chunkToLoad.y * chunkSize, 0,
                    chunkSize, chunkSize, 1
                );

                //Debug.Log($"Requesting generation for chunk: {chunkToLoad}");

                generator.boundsToFill = chunkBounds;
                generator.StartGenerationAsync();

                yield return null;
            }
        }
        chunkLoadingCoroutine = null;
    }
}