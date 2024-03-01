using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TerrainGeneration : MonoBehaviour
{
    public PlayerController playerController;
    public CameraController cameraController;
    public GameObject tileDrop;

    [Header("Tile Sprites")]
    public TileAtlas tileAtlas;
    public float seed;

    public BiomeClass[] biomes;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Texture2D biomeMap;

    [Header("Generation Settings")]
    public int chunkSize = 16;
    public int worldSize = 100;
    public int heightAddition = 25;
    public bool generateCaves = true;
    public float cameraSize;

    [Header("Noise Settings")]
    public Texture2D caveNoiseTexture;
    public float terrainFreq = 0.05f;
    public float caveFreq = 0.05f;

    //[Header("Ore settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    private List<GameObject> worldTileObjects = new List<GameObject>();
    private List<TileClass> worldTileClass = new List<TileClass>();

    private BiomeClass curBiome;
    private Color[] biomeCols;

    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].spreadTexture = new Texture2D(worldSize, worldSize);
        }

        biomeCols = new Color[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].biomeCol;
        }
        DrawTexture();
        DrawCavesAndOres();
        CreateChunks();
        GenerateTerrain();


        playerController.Spawn();
        cameraController.Spawn(new Vector3(playerController.transform.position.x, playerController.transform.position.y, -10));
        RefreshChunks();
    }
    private void Update()
    {
        RefreshChunks();
    }
    void RefreshChunks()
    {
        for (int i = 0; i < worldChunks.Length; i++)
        {
            if (Vector2.Distance(new Vector2((i) * chunkSize, 0), new Vector2(playerController.transform.position.x - (chunkSize / 2), 0)) > (Camera.main.orthographicSize * cameraSize))
                worldChunks[i].SetActive(false);
            else
                worldChunks[i].SetActive(true);
        }
    }
    public void DrawCavesAndOres()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        float v;
        float o;
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                if (v > curBiome.surfaceValue)
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                else
                    caveNoiseTexture.SetPixel(x, y, Color.clear);

                for (int i = 0; i < ores.Length; i++)
                {
                    ores[i].spreadTexture.SetPixel(x, y, Color.clear);
                    if (curBiome.ores.Length >= i + 1)
                    {
                        o = Mathf.PerlinNoise((x + seed) * curBiome.ores[i].frequency, (y + seed) * curBiome.ores[i].frequency);
                        if (o > curBiome.ores[i].size)
                            ores[i].spreadTexture.SetPixel(x, y, Color.white);

                        ores[i].spreadTexture.Apply();

                    }
                }
            }
        }
        caveNoiseTexture.Apply();
    }
    private void DrawTexture()
    {
        biomeMap = new Texture2D(worldSize, worldSize);

        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);
                GenerateNoiseTexture(biomes[i].ores[j].frequency, biomes[i].ores[j].size, biomes[i].ores[j].spreadTexture);

            }
        }
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public BiomeClass GetCurrentBiome(int x, int y)
    {
        if (System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
        {
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];
        }
        return curBiome;
    }
    public void GenerateTerrain()
    {
        TileClass tileClass;

        for (int i = 0; i < worldSize; i++)
        {
            float height;
            for (int j = 0; j < worldSize; j++)
            {
                curBiome = GetCurrentBiome(i, j);
                height = Mathf.PerlinNoise((i + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAddition;
                if (i == worldSize / 2)
                    playerController.spawnPos = new Vector2(i, height + 5);
                if (j >= height)
                    break;

                if (j < height - curBiome.dirtLayerHeight)
                {
                    tileClass = curBiome.tileAtlas.stone;
                    if (ores[0].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[0].maxSpawnHeight)
                        tileClass = tileAtlas.coal;
                    if (ores[1].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[1].maxSpawnHeight)
                        tileClass = tileAtlas.iron;
                    if (ores[2].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[2].maxSpawnHeight)
                        tileClass = tileAtlas.gold;
                    if (ores[3].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[3].maxSpawnHeight)
                        tileClass = tileAtlas.diamond;
                }
                else if (j < height - 1)
                {
                    tileClass = curBiome.tileAtlas.dirt;
                }
                else
                {
                    tileClass = curBiome.tileAtlas.grass;

                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(i, j).r > 0.5f)
                    {
                        PlaceTile(tileClass, i, j);
                    }
                    else if (tileClass.wallVariant != null)
                    {
                        PlaceTile(tileClass.wallVariant, i, j);
                    }
                }
                else
                {
                    PlaceTile(tileClass, i, j);
                }

                if (j >= height - 1)
                {
                    int t = Random.Range(0, curBiome.treeChance);
                    if (t == 1)
                    {
                        //Generate tree
                        if (worldTiles.Contains(new Vector2(i, j)))
                        {
                            if (curBiome.biomeName == "Desert")
                            {
                                GenerateCactus(curBiome.tileAtlas, Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), i, j + 1);
                            }
                            else
                            {
                                GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), i, j + 1);
                            }
                        }
                    }
                    else
                    {
                        int k = Random.Range(0, curBiome.tallGrassChange);
                        if (k == 1)
                        {
                            if (worldTiles.Contains(new Vector2(i, j)))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass, i, j + 1);
                            }
                        }

                    }
                }
            }
        }
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        float v, b;
        Color col;
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                b = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);

                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.clear);
            }
        }
        noiseTexture.Apply();
        biomeMap.Apply();
    }
    void GenerateCactus(TileAtlas atlas, int treeHeight, int i, int j)
    {
        for (int t = 0; t < treeHeight; t++)
        {
            PlaceTile(atlas.log, i, j + t);
        }
    }
    void GenerateTree(int treeHeight, int i, int j)
    {
        for (int t = 0; t < treeHeight; t++)
        {
            PlaceTile(tileAtlas.log, i, j + t);
        }
        PlaceTile(tileAtlas.leaf, i, j + treeHeight);
        PlaceTile(tileAtlas.leaf, i, j + treeHeight + 1);
        PlaceTile(tileAtlas.leaf, i, j + treeHeight + 2);

        PlaceTile(tileAtlas.leaf, i - 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf, i - 1, j + treeHeight + 1);

        PlaceTile(tileAtlas.leaf, i + 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf, i + 1, j + treeHeight + 1);
    }
    public void RemoveTile(int i, int j)
    {
        if (worldTiles.Contains(new Vector2Int(i, j)) && i >= 0 && i <= worldSize && j >= 0 && j <= worldSize)
        {
            int tileIndex = worldTiles.IndexOf(new Vector2(i, j));
            Destroy(worldTileObjects[tileIndex]);

            //Drop tile
            if (worldTileClass[tileIndex].isDrop)
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(i, j + 0.5f), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = worldTileClass[tileIndex].tileSprites[0];
            }
            if (worldTileClass[tileIndex].wallVariant != null)
            {
                PlaceTile(worldTileClass[tileIndex].wallVariant, i, j);
            }
            worldTileObjects.RemoveAt(tileIndex);
            worldTileClass.RemoveAt(tileIndex);
            worldTiles.RemoveAt(tileIndex);

        }
    }
    public void CheckTile(TileClass tile, int i, int j)
    {
        if (i >= 0 && i < worldSize && j >= 0 && j < worldSize)
        {
            if (!worldTiles.Contains(new Vector2Int(i, j)))
            {
                PlaceTile(tile, i, j);
            }
            else if (!worldTileClass[worldTiles.IndexOf(new Vector2Int(i, j))].isImpact)
            {
                RemoveTile(i, j);
                PlaceTile(tile, i, j);
            }
        }

    }
    void PlaceTile(TileClass tile, int i, int j)
    {
        if (i >= 0 && i < worldSize && j >= 0 && j < worldSize)
        {
            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(i / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;
            newTile.transform.parent = worldChunks[chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();
            if (tile.isImpact)
            {
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
            }

            int spriteIndex = Random.Range(0, tile.tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[spriteIndex];

            if (tile.isImpact)
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
            else
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;

            if (tile.name.ToUpper().Contains("WALL"))
                newTile.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);

            newTile.name = tile.tileSprites[0].name;
            newTile.transform.position = new Vector2(i + 0.5f, j + 0.5f);

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
            worldTileObjects.Add(newTile);
            worldTileClass.Add(tile);
        }

    }
}
