using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TerrainGeneration : MonoBehaviour
{
    //[Header("Light")]
    public Texture2D worldTilesMap;

    [Header("Connect")]
    public PlayerController playerController;
    public CameraController cameraController;
    public GameObject tileDrop;
    //
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

    //private List<GameObject> worldTileObjects = new List<GameObject>();

    private GameObject[,] world_BackgroundObjects;
    private GameObject[,] world_ForegroundObjects;

    private TileClass[,] world_BackgroundTiles;
    private TileClass[,] world_ForegroundTiles;

    private BiomeClass curBiome;
    private Color[] biomeCols;

    private void Start()
    {
        world_ForegroundTiles = new TileClass[worldSize, worldSize];
        world_BackgroundTiles = new TileClass[worldSize, worldSize];
        world_ForegroundObjects = new GameObject[worldSize, worldSize];
        world_BackgroundObjects = new GameObject[worldSize, worldSize];

        //light
        worldTilesMap = new Texture2D(worldSize, worldSize);

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                worldTilesMap.SetPixel(x, y, Color.white);
            }
        }
        worldTilesMap.Apply();

        //terrain
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

        worldTilesMap.Apply();

        playerController.Spawn();
        cameraController.Spawn(new Vector3(playerController.transform.position.x, playerController.transform.position.y, -10));
        RefreshChunks();
    }
    private void Update()
    {
        RefreshChunks();
    }
    void RefreshChunks()//tat mo chunk tung khu vuc
    {
        for (int i = 0; i < worldChunks.Length; i++)
        {
            if (Vector2.Distance(new Vector2((i) * chunkSize, 0), new Vector2(playerController.transform.position.x - (chunkSize / 2), 0)) > (Camera.main.orthographicSize * cameraSize))
                worldChunks[i].SetActive(false);
            else
                worldChunks[i].SetActive(true);
        }
    }
    private void DrawTexture()//Tach biomeMap, khoi tao moi truong
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);
                GenerateNoiseTexture(biomes[i].ores[j]);
            }
        }
    }
    public void GenerateNoiseTexture(OreClass ore)//Khoi tao tai nguyen
    {
        float v, b;
        Color col;
        for (int x = 0; x < ore.spreadTexture.width; x++)
        {
            for (int y = 0; y < ore.spreadTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * ore.frequency, (y + seed) * ore.frequency);
                b = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);

                if (v > ore.size)
                    ore.spreadTexture.SetPixel(x, y, Color.white);
                else
                    ore.spreadTexture.SetPixel(x, y, Color.clear);
            }
        }
        ore.spreadTexture.Apply();
        biomeMap.Apply();
    }
    public void DrawCavesAndOres()//Ve hang dong
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
    public BiomeClass GetCurrentBiome(int x, int y) //Tra lai thong so moi truong theo toa do diem
    {
        if (System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
        {
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];
        }
        return curBiome;
    }
    public void CreateChunks()//Tao chunks
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

    public void GenerateTerrain()
    {
        TileClass tileClass;

        for (int i = 0; i < worldSize; i++)
        {
            float height;
            for (int j = 0; j < worldSize - 1; j++)
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
                        PlaceTile(tileClass, i, j, true);
                    }
                    else if (tileClass.wallVariant != null)
                    {
                        PlaceTile(tileClass.wallVariant, i, j, true);
                    }
                }
                else
                {
                    PlaceTile(tileClass, i, j, true);
                }

                if (j >= height - 1)
                {
                    int t = Random.Range(0, curBiome.treeChance);
                    if (t == 1)
                    {
                        //Generate tree
                        if (GetTileFromWorld(i, j))
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
                            if (GetTileFromWorld(i, j))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass, i, j + 1, true);
                            }
                        }
                    }
                }
            }
        }
        worldTilesMap.Apply();
    }


    void GenerateCactus(TileAtlas atlas, int treeHeight, int i, int j)//Tao xuong rong
    {
        for (int t = 0; t < treeHeight; t++)
        {
            PlaceTile(atlas.log, i, j + t, true);
        }
    }
    void GenerateTree(int treeHeight, int i, int j)//Tao cay
    {
        for (int t = 0; t < treeHeight; t++)
        {
            PlaceTile(tileAtlas.log, i, j + t, true);
        }
        PlaceTile(tileAtlas.leaf, i, j + treeHeight, true);
        PlaceTile(tileAtlas.leaf, i, j + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, i, j + treeHeight + 2, true);

        PlaceTile(tileAtlas.leaf, i - 1, j + treeHeight, true);
        PlaceTile(tileAtlas.leaf, i - 1, j + treeHeight + 1, true);

        PlaceTile(tileAtlas.leaf, i + 1, j + treeHeight, true);
        PlaceTile(tileAtlas.leaf, i + 1, j + treeHeight + 1, true);
    }
    public bool BreakTile(int i, int j, ItemClass item )
    {
        if(GetTileFromWorld(i, j) && i >= 0 && i <= worldSize && j >= 0 && j <= worldSize)
        {
            TileClass tile = GetTileFromWorld(i, j);

            if(tile.toolToBreak == ItemEnum.ToolType.none)
            {
                RemoveTile(i, j);
                return true;
            }
            else
            {
                if (item.itemType == ItemEnum.ItemType.tool)
                {
                    if (tile.toolToBreak == item.toolType)
                    {
                        RemoveTile(i, j);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void RemoveTile(int i, int j)
    {
        if (GetTileFromWorld(i, j) && i >= 0 && i <= worldSize && j >= 0 && j <= worldSize)
        {
            TileClass tile = GetTileFromWorld(i, j);
            if (tile.wallVariant != null)
            {
                Destroy(world_BackgroundObjects[i, j].gameObject);
                if (tile.isNaturallyPlace)
                    PlaceTile(tile.wallVariant, i, j, true);
            }

            if (GetTileFromWorld(i, j).isNaturallyPlace || GetTileFromWorld(i, j) != null)
            {
                if (GetTileFromWorld(i, j).isNaturallyPlace && GetTileFromWorld(i, j).name.ToLower().Contains("wall"))
                {
                    worldTilesMap.SetPixel(i, j, Color.white);
                }
            }

            //Drop tile
            if (tile.tileDrop)
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(i, j + 0.5f), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileDrop.tileSprites[0];
                ItemClass tileDropItem = new ItemClass(tile.tileDrop);
                newTileDrop.GetComponent<TileCropController>().item = tileDropItem;
            }
            RemoveTileToWorld(i, j);

            Destroy(GetObjectFromWorld(i, j));
            RemoveObjectFromWorld(i, j);
            worldTilesMap.Apply();
        }
    }

    public bool CheckTile(TileClass tile, int i, int j, bool isNaturallyPlace)
    {
        if (i >= 0 && i < worldSize && j >= 0 && j < worldSize)
        {
            if (GetTileFromWorld(i, j) != null)
            {
                if (GetTileFromWorld(i, j).isNaturallyPlace)
                {
                    PlaceTile(tile, i, j, isNaturallyPlace);
                    return true;
                }
            }
            else
            {
                if (GetTileFromWorld(i + 1, j)
                    || GetTileFromWorld(i - 1, j)
                    || GetTileFromWorld(i, j + 1)
                    || GetTileFromWorld(i, j - 1))
                {
                    if (GetTileFromWorld(i, j) == null)
                    {
                        PlaceTile(tile, i, j, isNaturallyPlace);
                        return true;
                    }
                    else
                    {
                        if (GetTileFromWorld(i, j).isNaturallyPlace )
                        {
                            RemoveTile(i, j);
                            PlaceTile(tile, i, j, isNaturallyPlace);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    void PlaceTile(TileClass tile, int i, int j, bool isNaturallyPlace)
    {
        if (i >= 0 && i < worldSize && j >= 0 && j < worldSize)//Kiem tra toa do diem
        {
            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(i / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;
            newTile.transform.parent = worldChunks[chunkCoord].transform;//Add vao chunks phu hop
            newTile.AddComponent<SpriteRenderer>();

            int spriteIndex = Random.Range(0, tile.tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[spriteIndex];

            worldTilesMap.SetPixel(i, j, Color.black);

            if (tile.isNaturallyPlace)
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;
                if (tile.name.ToLower().Contains("wall"))
                {
                    newTile.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
                }
                else
                {
                    worldTilesMap.SetPixel(i, j, Color.white);
                }
            }
            else
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
            }

            if (tile.isImpact)
            {
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
            }

            newTile.name = tile.tileSprites[0].name;
            newTile.transform.position = new Vector2(i + 0.5f, j + 0.5f);

            TileClass newTileClass = TileClass.CreateInstance(tile, isNaturallyPlace);

            AddObjectToWorld(i, j, newTile, newTileClass);
            AddTileToWorld(i, j, newTileClass);
        }
    }

    void AddTileToWorld(int x, int y, TileClass tile)
    {
        if (tile.isNaturallyPlace)
        {
            world_BackgroundTiles[x, y] = tile;
        }
        else
        {
            world_ForegroundTiles[x, y] = tile;
        }
    }

    void AddObjectToWorld(int x, int y, GameObject obj, TileClass tile)
    {
        if (tile.isNaturallyPlace)
        {
            world_BackgroundObjects[x, y] = obj;
        }
        else
        {
            world_ForegroundObjects[x, y] = obj;
        }
    }

    void RemoveTileToWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            world_ForegroundTiles[x, y] = null;
        }
        else if (world_BackgroundTiles[x, y] != null)
        {
            world_BackgroundTiles[x, y] = null;
        }
    }

    void RemoveObjectFromWorld(int x, int y)
    {
        if (world_ForegroundObjects[x, y] != null)
        {
            world_ForegroundObjects[x, y] = null;
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            world_BackgroundObjects[x, y] = null;
        }
    }

    GameObject GetObjectFromWorld(int x, int y)
    {
        if (world_ForegroundObjects[x, y] != null)
        {
            return world_ForegroundObjects[x, y];
        }
        else if (world_BackgroundObjects[x, y] != null)
        {
            return world_BackgroundObjects[x, y];
        }
        return null;
    }
    TileClass GetTileFromWorld(int x, int y)
    {
        if (world_ForegroundTiles[x, y] != null)
        {
            return world_ForegroundTiles[x, y];
        }
        else if (world_BackgroundTiles[x, y] != null)
        {
            return world_BackgroundTiles[x, y];
        }
        return null;
    }
}
