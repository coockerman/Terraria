using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TerrainGeneration : MonoBehaviour
{
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
    public int dirtLayerHeight = 5;
    public float surfaceValue = 0.25f;
    public float heightMultiplier = 2f;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public Texture2D caveNoiseTexture;

    [Header("Trees")]
    public int treeChance = 10;
    public int minTreeHeight = 3;
    public int maxTreeHeight = 6;

    [Header("Addons")]
    public int tallGrassChange = 10;

    [Header("Ore settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    public Color[] biomeColors;
    private BiomeClass curBiome;

    private void OnValidate()
    {
        
        DrawTexture();
    }
    private void Start()
    {
        seed = Random.Range(-10000, 10000);
        DrawTexture();

        CreateChunks();
        GenerateTerrain();
    }
    private void DrawTexture()
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeTexture();

        for(int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for(int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);

            }
            
            GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);

            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                GenerateNoiseTexture(biomes[i].ores[j].rarity, biomes[i].ores[j].size, biomes[i].ores[j].spreadTexture);
            }
        }
    }

    private void DrawBiomeTexture()
    {
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                Color col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }
        biomeMap.Apply();

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
    public BiomeClass GetCurrentBiome( int x, int y )
    {
        for(int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeCol == biomeMap.GetPixel(x, y))
            {
                return biomes[i];
            }
        }
        return curBiome;
    }
    public void GenerateTerrain()
    {
        Sprite[] tileSprite;

        for (int i = 0; i < worldSize; i++)
        {
            float height = Mathf.PerlinNoise((i + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            for (int j = 0; j < height; j++)
            {
                if (j < height - dirtLayerHeight)
                {
                    tileSprite = GetCurrentBiome(i, j).tileAtlas.stone.tileSprites;
                    if (ores[0].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[0].maxSpawnHeight)
                        tileSprite = tileAtlas.coal.tileSprites;
                    if (ores[1].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[1].maxSpawnHeight)
                        tileSprite = tileAtlas.iron.tileSprites;
                    if (ores[2].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[2].maxSpawnHeight)
                        tileSprite = tileAtlas.gold.tileSprites;
                    if (ores[3].spreadTexture.GetPixel(i, j).r > 0.5f && height - j > ores[3].maxSpawnHeight)
                        tileSprite = tileAtlas.diamond.tileSprites;
                }
                else if (j < height - 1)
                {
                    tileSprite = tileAtlas.dirt.tileSprites;
                }
                else
                {
                    tileSprite = tileAtlas.grass.tileSprites;

                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(i, j).r > 0.5f)
                    {
                        PlaceTile(tileSprite, i, j);
                    }
                }
                else
                {
                    PlaceTile(tileSprite, i, j);
                }

                if (j >= height - 1)
                {
                    int t = Random.Range(0, treeChance);
                    if (t == 1)
                    {
                        if (worldTiles.Contains(new Vector2(i, j)))
                            GenerateTree(i, j + 1);
                    }
                    else
                    {
                        int k = Random.Range(0, tallGrassChange);
                        if (k == 1)
                        {
                            if (worldTiles.Contains(new Vector2(i, j)))
                            {
                                PlaceTile(tileAtlas.tallGrass.tileSprites, i, j + 1);
                            }
                        }

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
        for (int t = 0; t < treeHeight; t++)
        {
            PlaceTile(tileAtlas.log.tileSprites, i, j + t);
        }
        PlaceTile(tileAtlas.leaf.tileSprites, i, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, i, j + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, i, j + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprites, i - 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, i - 1, j + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprites, i + 1, j + treeHeight);
        PlaceTile(tileAtlas.leaf.tileSprites, i + 1, j + treeHeight + 1);
    }
    public void PlaceTile(Sprite[] tileSprite, int i, int j)
    {
        if (!worldTiles.Contains(new Vector2Int(i, j)))
        {
            GameObject newTile = new GameObject();

            float chunkCoord = Mathf.RoundToInt(i / chunkSize) * chunkSize;
            chunkCoord /= chunkSize;
            newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();

            int spriteIndex = Random.Range(0, tileSprite.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprite[spriteIndex];

            newTile.name = tileSprite[0].name;
            newTile.transform.position = new Vector2(i + 0.5f, j + 0.5f);

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
        }

    }
}
