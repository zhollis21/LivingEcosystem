using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GrassTile : TileBase
{
    public Sprite GrassNormal;
    public Sprite TopEdgeGrass;
    public Sprite RightEdgeGrass;
    public Sprite BottomEdgeGrass;
    public Sprite LeftEdgeGrass;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
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

        if (topTile != null && topTile != this)
        {
            tileData.sprite = TopEdgeGrass;
        }
        else if (rightTile != null && rightTile != this)
        {
            tileData.sprite = RightEdgeGrass;
        }
        else if (bottomTIle != null && bottomTIle != this)
        {
            tileData.sprite = BottomEdgeGrass;
        }
        else if (leftTile != null && leftTile != this)
        {
            tileData.sprite = LeftEdgeGrass;
        }
        else
        {
            tileData.sprite = GrassNormal;
        }
    }
}
