using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions { Up, Down, Left, Right }

public class GameWorld : MonoBehaviour
{
    public Tile GrassTile;
    public Tile WaterTile;

    private Tilemap _tileMap;
    private const int X_BOUNDARY = 50;
    private const int Y_BOUNDARY = 50;

    // Start is called before the first frame update
    void Start()
    {
        _tileMap = GetComponent<Tilemap>();

        for (int x = -X_BOUNDARY; x <= X_BOUNDARY; x++)
        {
            for (int y = -Y_BOUNDARY; y <= Y_BOUNDARY; y++)
            {
                _tileMap.SetTile(new Vector3Int(x, y), GrassTile);
            }
        }

        GenerateWorld();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateWorld()
    {
        var totalLakes = Random.Range(5, 15);

        for (int lakeNum = 0; lakeNum < totalLakes; lakeNum++)
        {
            var location = new Vector3Int(
                    Random.Range(-X_BOUNDARY, X_BOUNDARY),
                    Random.Range(-Y_BOUNDARY, Y_BOUNDARY));

            int waterTop = Random.Range(0, 20);
            int waterBottom = Random.Range(-20, 0);
            int waterLeft = Random.Range(-20, 0);
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
        var originalDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));

        
    }

    private void AddLake(Vector3Int center, float top, float bottom, float left, float right)
    {
        int circleResolution = 500;
        while (right > 0 || top > 0 || bottom < 0 || left < 0)
        {
            SetTilesInCircle(circleResolution, center, top, right, -bottom, -left);

            right = Mathf.Max(0, right - 0.5f);
            top = Mathf.Max(0, top - 0.5f);
            bottom = Mathf.Min(0, bottom + 0.5f);
            left = Mathf.Min(0, left + 0.5f);
        }
    }

    private void SetTilesInCircle(float numberOfTilesInCircle, Vector3 center, float top, float right, float bottom, float left)
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
            
            _tileMap.SetTile(pos, WaterTile);
        }
    }
}
