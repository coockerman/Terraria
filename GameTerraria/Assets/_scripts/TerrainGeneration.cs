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

    [Header("Noise Settings")]
    public Texture2D caveNoiseTexture;

    [Header("Ore settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    private BiomeClass curBiome;
    public Color[] biomeCols;


    //private void OnValidate()
    //{

    //    DrawTexture();
    //    DrawCavesAndOres();

    //}
    private void Start()
    {
        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].spreadTexture = new Texture2D(worldSize, worldSize);
        }

        biomeCols = new Color[biomes.Length];  
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].biomeCol;
        }
        seed = Random.Range(-10000, 10000);
        DrawTexture();
        DrawCavesAndOres();
        CreateChunks();
        GenerateTerrain();
    }
    public void DrawCavesAndOres()
    {
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                float v = Mathf.PerlinNoise((x + seed) * curBiome.caveFreq, (y + seed) * curBiome.caveFreq);
                if (v > curBiome.surfaceValue)
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                else
                    caveNoiseTexture.SetPixel(x, y, Color.clear);
            }
        }
        caveNoiseTexture.Apply();

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                for (int i = 0; i < ores.Length; i++)
                {
                    ores[i].spreadTexture.SetPixel(x, y, Color.clear);
                    if (curBiome.ores.Length >= i + 1)
                    {
                        float v = Mathf.PerlinNoise((x + seed) * curBiome.ores[i].frequency, (y + seed) * curBiome.ores[i].frequency);
                        if (v > curBiome.ores[i].size)
                            ores[i].spreadTexture.SetPixel(x, y, Color.white);

                        ores[i].spreadTexture.Apply();

                    }
                }
            }
        }
    }
    private void DrawTexture()
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeTexture();

        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);

            }

            GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);

            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                GenerateNoiseTexture(biomes[i].ores[j].frequency, biomes[i].ores[j].size, biomes[i].ores[j].spreadTexture);
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
    public BiomeClass GetCurrentBiome(int x, int y)
    {
        //for (int i = 0; i < biomes.Length; i++)
        //{
        //    if (biomes[i].biomeCol == biomeMap.GetPixel(x, y))
        //    {
        //        return biomes[i];
        //    }
        //}
        if(System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) > 0)
        {
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];
        }
        return curBiome;
    }
    public void GenerateTerrain()
    {
        Sprite[] tileSprite;

        for (int i = 0; i < worldSize; i++)
        {
            float height;
            for (int j = 0; j < worldSize; j++)
            {
                curBiome = GetCurrentBiome(i, j);
                height = Mathf.PerlinNoise((i + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;
                if (j >= height)
                    break;

                if (j < height - curBiome.dirtLayerHeight)
                {
                    tileSprite = curBiome.tileAtlas.stone.tileSprites;
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
                    tileSprite = curBiome.tileAtlas.dirt.tileSprites;
                }
                else
                {
                    tileSprite = curBiome.tileAtlas.grass.tileSprites;

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
                    int t = Random.Range(0, curBiome.treeChance);
                    if (t == 1)
                    {
                        if (worldTiles.Contains(new Vector2(i, j)))
                            GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), i, j + 1);
                    }
                    else
                    {
                        int k = Random.Range(0, curBiome.tallGrassChange);
                        if (k == 1)
                        {
                            if (worldTiles.Contains(new Vector2(i, j)))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass.tileSprites, i, j + 1);
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
    void GenerateTree(int treeHeight, int i, int j)
    {
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
