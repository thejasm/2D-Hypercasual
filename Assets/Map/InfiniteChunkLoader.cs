using UnityEngine;
using System.Collections; // Required for Coroutines
using InteliMapPro;

public class InfiniteChunkLoader: MonoBehaviour {
    public InteliMapGenerator generator;
    public Camera mainCam;

    public int chunkSize = 25;
    public int loadRadius = 1; // loadRadius = 0 means only current chunk, 1 means 3x3, etc.

    private Grid grid;
    private Vector2Int currentCamChunk; // Renamed to avoid confusion with loop variable
    private Coroutine chunkLoadingCoroutine;

    void Start() {
        if (mainCam == null) mainCam = Camera.main;

        // object generatorData = generator.generatorData; // This line is unused

        grid = generator.GetComponentInParent<Grid>();
        if (grid == null) {
            Debug.LogError("Grid component not found on the parent of the InteliMapGenerator. Disabling InfiniteChunkLoader.");
            enabled = false;
            return;
        }

        currentCamChunk = GetChunkCoordFromWorldPos(mainCam.transform.position);
        StartLoadingChunksFor(currentCamChunk);
    }

    void Update() {
        Vector2Int newCamChunk = GetChunkCoordFromWorldPos(mainCam.transform.position);
        if (newCamChunk != currentCamChunk) {
            currentCamChunk = newCamChunk;
            StartLoadingChunksFor(currentCamChunk);
        }
    }

    Vector2Int GetChunkCoordFromWorldPos(Vector3 worldPos) {
        Vector3Int cellPos = grid.WorldToCell(worldPos);
        return new Vector2Int(
            Mathf.FloorToInt((float)cellPos.x / chunkSize),
            Mathf.FloorToInt((float)cellPos.y / chunkSize)
        );
    }

    void StartLoadingChunksFor(Vector2Int centerChunk) {
        if (chunkLoadingCoroutine != null) {
            StopCoroutine(chunkLoadingCoroutine);
        }
        chunkLoadingCoroutine = StartCoroutine(LoadChunksSequentiallyCoroutine(centerChunk));
    }

    IEnumerator LoadChunksSequentiallyCoroutine(Vector2Int centerChunk) {
        // Iterate from -loadRadius to +loadRadius for both x and y offsets
        // This will cover the center chunk (when xOffset and yOffset are 0) and its neighbors.
        for (int yOffset = -loadRadius; yOffset <= loadRadius; yOffset++) {
            for (int xOffset = -loadRadius; xOffset <= loadRadius; xOffset++) {
                Vector2Int chunkToLoad = new Vector2Int(centerChunk.x + xOffset, centerChunk.y + yOffset);

                // Wait for the generator to finish its current async task
                while (generator.IsAsyncOperationInProgress) {
                    yield return null; // Wait for the next frame
                }

                // Now that the generator is free, proceed to generate the current chunkToLoad
                BoundsInt chunkBounds = new BoundsInt(
                    chunkToLoad.x * chunkSize, chunkToLoad.y * chunkSize, 0,
                    chunkSize, chunkSize, 1
                );

                Debug.Log($"Requesting generation for chunk: {chunkToLoad}");

                generator.boundsToFill = chunkBounds;
                generator.StartGenerationAsync();

                // Optional: brief yield after starting a generation.
                // This can help if generations are extremely fast and you want to ensure
                // IsAsyncOperationInProgress updates, or just to pace the requests slightly.
                // If IsAsyncOperationInProgress is robust, this might not be strictly necessary
                // but doesn't hurt.
                yield return null;
            }
        }
        chunkLoadingCoroutine = null; // Mark coroutine as finished
    }
}