using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GrassTile : TileBase
{
    public Sprite GrassNormal;
    public Sprite TopEdgeGrass;
    public Sprite TopLeftCornerGrass;
    public Sprite TopLeftRightGrass;
    public Sprite TopBottomGrass;
    public Sprite RightEdgeGrass;
    public Sprite TopRightCornerGrass;
    public Sprite BottomEdgeGrass;
    public Sprite LeftEdgeGrass;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        int xPos = position.x;
        int yPos = position.y;

        for (int x = xPos - 1; x <= xPos + 1; x++)
        {
            for (int y = yPos - 1; y <= yPos + 1; y++)
            {
                tilemap.RefreshTile(new Vector3Int(x, y));
            }
        }

        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var topTile = tilemap.GetTile(position + Vector3Int.up);
        var rightTile = tilemap.GetTile(position + Vector3Int.right);
        var bottomTIle = tilemap.GetTile(position + Vector3Int.down);
        var leftTile = tilemap.GetTile(position + Vector3Int.left);

        if (topTile != this)
        {
            if (leftTile != this)
            {
                if (rightTile != this)
                {
                    tileData.sprite = TopLeftRightGrass;
                }
                else
                {
                    tileData.sprite = TopLeftCornerGrass;
                }
            }
            else if (rightTile != this)
            {
                tileData.sprite = TopRightCornerGrass;
            }
            else if (bottomTIle != this)
            {
                tileData.sprite = TopBottomGrass;
            }
            else // Only the top
            {
                tileData.sprite = TopEdgeGrass;
            }
        }
        else if (rightTile != this)
        {
            tileData.sprite = RightEdgeGrass;
        }
        else if (bottomTIle != this)
        {
            tileData.sprite = BottomEdgeGrass;
        }
        else if (leftTile != this)
        {
            tileData.sprite = LeftEdgeGrass;
        }
        else
        {
            tileData.sprite = GrassNormal;
        }
    }
}
