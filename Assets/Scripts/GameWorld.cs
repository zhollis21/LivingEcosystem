using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions { Up, Down, Left, Right }

public class GameWorld : MonoBehaviour
{
    public List<TileBase> GrassTiles;
    public List<TileBase> WaterTiles;
    public Tile SandTile;
    public Tile TempRiverTile;

    private Tilemap _tileMap;
    private const int X_BOUNDARY = 50;
    private const int Y_BOUNDARY = 50;

    // Start is called before the first frame update
    void Start()
    {
        _tileMap = GetComponent<Tilemap>();

        GenerateWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateWorld();
        }
    }

    void GenerateWorld()
    {
        for (int x = -X_BOUNDARY; x <= X_BOUNDARY; x++)
        {
            for (int y = -Y_BOUNDARY; y <= Y_BOUNDARY; y++)
            {
                _tileMap.SetTile(new Vector3Int(x, y), PickRandomTile(GrassTiles, true, 0, 0.9f));
            }
        }

        var totalLakes = Random.Range(5, 15);

        for (int lakeNum = 0; lakeNum < totalLakes; lakeNum++)
        {
            var location = new Vector3Int(
                    Random.Range(-X_BOUNDARY, X_BOUNDARY),
                    Random.Range(-Y_BOUNDARY, Y_BOUNDARY));

            int waterTop = Random.Range(0, 20);
            int waterBottom = Random.Range(0, 20);
            int waterLeft = Random.Range(0, 20);
            int waterRight = Random.Range(0, 20);

            AddLake(location, waterTop, waterBottom, waterLeft, waterRight);

            var riversFromLakes = Random.Range(0, 3);
            for (int riverNum = 0; riverNum < riversFromLakes; riverNum++)
            {
                AddRiver(location);
            }
        }
    }

    private void AddRiver(Vector3Int center)
    {
        int circleResolution = 25;
        var currentPosition = center;
        var originalDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        bool hasHitLand = false;
        var doNotReplaceTiles = new List<TileBase>(WaterTiles);
        doNotReplaceTiles.Add(TempRiverTile);

        while (IsWithinMapBounds(currentPosition))
        {
            var directionVariation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            currentPosition += Vector3Int.RoundToInt(originalDirection + directionVariation);

            if (!WaterTiles.Contains(_tileMap.GetTile(currentPosition)))
            {
                hasHitLand = true;
            }
            else if (hasHitLand && WaterTiles.Contains(_tileMap.GetTile(currentPosition)))
            {
                FillInNewRiverTiles();
                return;
            }


            float radius = 1;

            // First add some circles of sand on the outside
            int sandPerimeters = 2;
            for (int i = 1; i <= sandPerimeters; i++)
            {
                SetTilesInCircle(circleResolution, currentPosition, radius + i, radius + i, radius + i, radius + i, SandTile, doNotReplaceTiles);
            }

            // Then fill in the water from outer most circle to inner most circle
            while (radius >= 0)
            {
                SetTilesInCircle(circleResolution, currentPosition, radius, radius, radius, radius, TempRiverTile, doNotReplaceTiles);

                radius -= 0.5f;
            }
        }

        FillInNewRiverTiles();
    }

    private void FillInNewRiverTiles()
    {
        for (int x = -X_BOUNDARY; x <= X_BOUNDARY; x++)
        {
            for (int y = -Y_BOUNDARY; y <= Y_BOUNDARY; y++)
            {
                if (_tileMap.GetTile(new Vector3Int(x, y)) == TempRiverTile)
                {
                    _tileMap.SetTile(new Vector3Int(x, y), PickRandomTile(WaterTiles));
                }
            }
        }
    }

    private TileBase PickRandomTile(List<TileBase> tiles, bool hasNormalTile = false, int normalTilePos = 0, float percentToPickNormal = 0.75f)
    {
        if (hasNormalTile && Random.Range(0f, 1f) <= percentToPickNormal)
        {
            return tiles[normalTilePos];
        }

        if (tiles == null || tiles.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, tiles.Count - 1);

        // If there is a normal tile it's already had it's shot, we will reroll once to try to pick another 
        if (hasNormalTile && index == normalTilePos)
        {
            index = Random.Range(0, tiles.Count - 1);
        }

        return tiles[index];
    }

    private bool IsWithinMapBounds(Vector3 pos)
    {
        return pos.x > -X_BOUNDARY && pos.x < X_BOUNDARY &&
               pos.y > -Y_BOUNDARY && pos.y < Y_BOUNDARY;
    }

    private void AddLake(Vector3Int center, float top, float bottom, float left, float right)
    {
        int circleResolution = 200;

        // First add some circles of sand on the outside
        int sandPerimeters = 2;
        for (int i = 1; i <= sandPerimeters; i++)
        {
            SetTilesInCircle(circleResolution, center, top + i, right + i, bottom + i, left + i, SandTile, WaterTiles);
        }

        // Then fill in the water from outer most circle to inner most circle
        while (right > 0 || top > 0 || bottom > 0 || left > 0)
        {
            SetTilesInCircle(circleResolution, center, top, right, bottom, left, PickRandomTile(WaterTiles), WaterTiles);

            right = Mathf.Max(0, right - 0.5f);
            top = Mathf.Max(0, top - 0.5f);
            bottom = Mathf.Max(0, bottom - 0.5f);
            left = Mathf.Max(0, left - 0.5f);
        }
    }

    private void SetTilesInCircle(float numberOfTilesInCircle, Vector3 center, float top, float right, float bottom, float left, TileBase tile, List<TileBase> doNotReplaceTiles)
    {
        for (int i = 0; i < numberOfTilesInCircle; i++)
        {
            var percent = i / numberOfTilesInCircle;
            float xRadius = percent < 0.25 || percent > 0.75 ? right : left;
            float yRadius = percent < 0.5 ? top : bottom;

            float angle = percent * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * xRadius;
            float y = Mathf.Sin(angle) * yRadius;

            Vector3Int pos = Vector3Int.RoundToInt(
                new Vector2(
                    Mathf.Clamp(center.x + x, -X_BOUNDARY, X_BOUNDARY),
                    Mathf.Clamp(center.y + y, -Y_BOUNDARY, Y_BOUNDARY)));

            if (!doNotReplaceTiles.Contains(_tileMap.GetTile(pos)))
            {
                _tileMap.SetTile(pos, tile);
            }
        }
    }
}
