using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Sprites")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public int chunkSize = 16;
    public bool generateCaves = true;
    public int dirtLayerHeight = 5;
    public float surfaceValue = 0.25f;
    public int worldSize = 100;
    public float heightMultiplier = 2f;
    public int heightAddition = 25;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    [Header("Trees")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 6;

    [Header("Ore settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    private void OnValidate()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);
    }
    private void Start()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        ores[0].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[1].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[2].spreadTexture = new Texture2D(worldSize, worldSize);
        ores[3].spreadTexture = new Texture2D(worldSize, worldSize);

        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture(caveFreq, surfaceValue, caveNoiseTexture);
        GenerateNoiseTexture(ores[0].rarity, ores[0].size, ores[0].spreadTexture);
        GenerateNoiseTexture(ores[1].rarity, ores[1].size, ores[1].spreadTexture);
        GenerateNoiseTexture(ores[2].rarity, ores[2].size, ores[2].spreadTexture);
        GenerateNoiseTexture(ores[3].rarity, ores[3].size, ores[3].spreadTexture);

        CreateChunks();
        GenerateTerrain();
    }
    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for(int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public void GenerateTerrain()
    {
        for (int i = 0; i < worldSize; i++)
        {
            float height = Mathf.PerlinNoise((i + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for (int j = 0; j < height; j++)
            {
                Sprite tileSprite;
                if (j < height - dirtLayerHeight)
                {
                    tileSprite = tileAtlas.stone.tileSprite;
                    if (ores[0].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[0].maxSpawnHeight)
                        tileSprite = tileAtlas.coal.tileSprite;
                    if (ores[1].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[1].maxSpawnHeight)
                        tileSprite = tileAtlas.iron.tileSprite;
                    if (ores[2].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[2].maxSpawnHeight)
                        tileSprite = tileAtlas.gold.tileSprite;
                    if (ores[3].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[3].maxSpawnHeight)
                        tileSprite = tileAtlas.diamond.tileSprite;
                }
                else if(j< height - 1 )
                {
                    tileSprite = tileAtlas.dirt.tileSprite;
                }
                else
                {
                    tileSprite = tileAtlas.grass.tileSprite;
                    
                }

                if(generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(i, j).r > 0.5f)
                    {
                        PlaceTile(tileSprite, i, j);
                    }
                }else
                {
                    PlaceTile(tileSprite, i, j);
                }
                
                if(j >= height - 1)
                {
                    int t = Random.Range(0, treeChance);
                    if (t == 1)
                    {
                        if (worldTiles.Contains(new Vector2(i, j)))
                            GenerateTree(i, j + 1);
                    }
                }
            }
        }
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.clear);
            }
        }
        noiseTexture.Apply();
    }
    void GenerateTree(int i, int j)
    {
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for(int t=0; t<treeHeight; t++)
        {
            PlaceTile(tileAtlas.log.tileSprite, i, j + t);
        }
        PlaceTile(tileAtlas.leaf.tileSprite, i, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, i, j + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprite, i, j + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprite, i - 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, i - 1, j + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprite, i + 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprite, i + 1, j + treeHeight + 1);
    }
    public void PlaceTile(Sprite tileSprite, int i, int j)
    {
        GameObject newTile = new GameObject();

        float chunkCoord = Mathf.RoundToInt(i / chunkSize) * chunkSize;
        chunkCoord /= chunkSize;
        newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2(i + 0.5f, j + 0.5f);

        worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
    }
}
