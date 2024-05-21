using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour {
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] public bool _isWalkable;
    [SerializeField] private Color _Portalcolor;
    [SerializeField] public bool _isPortalSpawned;
    public int G; // a star G value
    public int H;// a star H value
    public int F { get { return G + H; } } // a star F val
    public Tile previousTile; //the previous tile is stored here
    public Vector2Int gridLocation; // the current grid locaiton
    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;
    

    public virtual void Init(int x, int y)
    {
      
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }
    public virtual void ColorPortal()
    {

        if (_isPortalSpawned == true)
        {
            _renderer.color = _Portalcolor;
        }
    }
    public virtual void LightPath()
    {
        _renderer.color = _Portalcolor;
    }
    void OnMouseDown() {
        //UnitManager.Instance.PlayerMove();
    }
    //sets the unit to the tile
    public void SetUnit(BaseUnit unit) {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}