using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Directions { Up, Down, Left, Right }

public class GameWorld : MonoBehaviour
{
    public Tile MyTile;
    public Tile WaterTile;

    private Tilemap _tileMap;
    private const int X_BOUNDARY = 50;
    private const int Y_BOUNDARY = 50;

    // Start is called before the first frame update
    void Start()
    {
        _tileMap = GetComponent<Tilemap>();

        for (int x = -X_BOUNDARY; x < X_BOUNDARY; x++)
        {
            for (int y = -Y_BOUNDARY; y < Y_BOUNDARY; y++)
            {
                _tileMap.SetTile(new Vector3Int(x, y), MyTile);
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
        var numOfLakes = Random.Range(1, 10);

        for (int i = 0; i < numOfLakes; i++)
        {
            var location = new Vector3Int(
                    Random.Range(-X_BOUNDARY, X_BOUNDARY),
                    Random.Range(-Y_BOUNDARY, Y_BOUNDARY));

            int waterTop = Random.Range(0, 20);
            int waterBottom = Random.Range(-20, 0);
            int waterLeft = Random.Range(-20, 0);
            int waterRight = Random.Range(0, 20);

            AddLake(location, waterTop, waterBottom, waterLeft, waterRight);
        }
    }

    void AddLake(Vector3Int center, float top, float bottom, float left, float right)
    {
        // Fill in center
        _tileMap.SetTile(center, WaterTile);

        // Fill in to top
        for (int i = 0; i <= top; i++)
        {
            _tileMap.SetTile(new Vector3Int(center.x, center.y + i), WaterTile);
        }

        FillInTilesFromTopToDownwardRight(center, top, right);

        FillInTilesFromRightToBottomLeft(center, right, bottom);

        FillInTilesFromBottomToUpperLeft(center, bottom, left);

        FillInTilesFromLeftToTopRight(center, left, top);
    }

    Vector3 GetDirectionStepValue(float xPos, float yPos, Vector2Int direction)
    {
        Vector3 step;
        xPos = Mathf.Abs(xPos);
        yPos = Mathf.Abs(yPos);

        if (yPos > xPos)
        {
            step = new Vector3(direction.x * xPos / yPos, direction.y);
        }
        else
        {
            step = new Vector3(direction.x, direction.y * yPos / xPos);
        }

        return step;
    }

    void FillInTilesFromTopToDownwardRight(Vector3Int center, float startYPos, float endXPos)
    {
        var step = GetDirectionStepValue(endXPos, startYPos, Vector2Int.down + Vector2Int.right);

        Vector3 currentSpot = new Vector3(center.x, center.y + startYPos);
        Vector3Int roundedSpot = Vector3Int.RoundToInt(currentSpot);

        while (roundedSpot.x != center.x + endXPos || roundedSpot.y != center.y)
        {
            currentSpot += step;
            roundedSpot = Vector3Int.RoundToInt(currentSpot);

            for (int y = roundedSpot.y; y >= center.y; y--)
            {
                _tileMap.SetTile(new Vector3Int(roundedSpot.x, y), WaterTile);
            }
        }
    }

    void FillInTilesFromRightToBottomLeft(Vector3Int center, float startXPos, float endYPos)
    {
        var step = GetDirectionStepValue(startXPos, endYPos, Vector2Int.down + Vector2Int.left);

        Vector3 currentSpot = new Vector3(center.x + startXPos, center.y);
        Vector3Int roundedSpot = Vector3Int.RoundToInt(currentSpot);

        // Fill in to up or down
        while (roundedSpot.y != center.y + endYPos || roundedSpot.x != center.x)
        {
            currentSpot += step;
            roundedSpot = Vector3Int.RoundToInt(currentSpot);

            for (int x = roundedSpot.x; x >= center.x; x--)
            {
                _tileMap.SetTile(new Vector3Int(x, roundedSpot.y), WaterTile);
            }
        }
    }

    void FillInTilesFromBottomToUpperLeft(Vector3Int center, float startYPos, float endXPos)
    {
        var step = GetDirectionStepValue(endXPos, startYPos, Vector2Int.up + Vector2Int.left);

        Vector3 currentSpot = new Vector3(center.x, center.y + startYPos);
        Vector3Int roundedSpot = Vector3Int.RoundToInt(currentSpot);

        while (roundedSpot.x != center.x + endXPos || roundedSpot.y != center.y)
        {
            currentSpot += step;
            roundedSpot = Vector3Int.RoundToInt(currentSpot);

            for (int y = roundedSpot.y; y <= center.y; y++)
            {
                _tileMap.SetTile(new Vector3Int(roundedSpot.x, y), WaterTile);
            }
        }
    }

    void FillInTilesFromLeftToTopRight(Vector3Int center, float startXPos, float endYPos)
    {
        var step = GetDirectionStepValue(startXPos, endYPos, Vector2Int.up + Vector2Int.right);

        Vector3 currentSpot = new Vector3(center.x + startXPos, center.y);
        Vector3Int roundedSpot = Vector3Int.RoundToInt(currentSpot);

        // Fill in to up or down
        while (roundedSpot.y != center.y + endYPos || roundedSpot.x != center.x)
        {
            currentSpot += step;
            roundedSpot = Vector3Int.RoundToInt(currentSpot);

            for (int x = roundedSpot.x; x <= center.x; x++)
            {
                _tileMap.SetTile(new Vector3Int(x, roundedSpot.y), WaterTile);
            }
        }
    }
}
