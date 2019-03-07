using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour {

    public GameObject[] buildings;
    public GameObject xStreets;
    public GameObject zStreets;
    public GameObject crossRoad;
    private int _mapWidth = 50;
    private int _mapHeight = 50;
    private float _distanceBetweenBuildings = 3.2f;
    private int[,] _mapGrid;

	void Start () {
        CreateMapGrid();
        GenerateStreets();
        GenerateBuildings();	
	}
	

    private void CreateMapGrid()
    {
        _mapGrid = new int[_mapWidth, _mapHeight];

        for (int h = 0; h < _mapHeight; h++)
        {
            for (int w = 0; w < _mapWidth; w++)
            {
                /// Is better than random because generate a smooth transiction
                _mapGrid[w,h] = (int)(Mathf.PerlinNoise(w / 10.0f , h / 10.0f) * 10);
            }
        }
    }

    private void GenerateStreets()
    {
        int x = 0;
        /// Create 50 streets
        for ( int n = 0; n < 50; ++n)
        {
            for (int h = 0; h < _mapHeight; ++h)
            {
                _mapGrid[x, h] = -1;
            }
            x += Random.Range(2, 10);
            if (x >= _mapWidth) break;
        }

        int z = 0;
        for (int n = 0; n < 10; ++n)
        {         
            for (int w = 0; w < _mapWidth; ++w)
            {
                if (_mapGrid[w, z] == -1)
                    _mapGrid[w, z] = -3;
                else
                    _mapGrid[w, z] = -2;
            }
            z += Random.Range(2, 20);
            if (z >= _mapHeight) break;
        }
    }
    private void GenerateBuildings()
    {
        for ( int  h = 0; h < _mapHeight; h++)
        {
            for ( int w = 0; w < _mapWidth; w++)
            {
                int result = _mapGrid[w, h];
                Vector3 pos = new Vector3(_distanceBetweenBuildings * w, 0, _distanceBetweenBuildings * h );
                if (result < -2)
                {
                    pos.y = 0.01f;
                    Instantiate(crossRoad, pos, crossRoad.transform.rotation);
                }
                                 
                else if (result < -1)
                {
                    pos.y = 0.01f;
                    Instantiate(xStreets, pos, xStreets.transform.rotation);
                }                   
                else if (result < 0)
                {
                    pos.y = 0.01f;
                    Instantiate(zStreets, pos, zStreets.transform.rotation);
                }                        
                else if (result < 1)
                    Instantiate(buildings[0], pos, Quaternion.identity);
                else if (result < 2)
                    Instantiate(buildings[1], pos, Quaternion.identity);
                else if (result < 4)
                    Instantiate(buildings[2], pos, Quaternion.identity);
                else if (result < 6)
                    Instantiate(buildings[3], pos, Quaternion.identity);
                else if (result < 7)
                    Instantiate(buildings[4], pos, Quaternion.identity);
                else if (result < 10)
                    Instantiate(buildings[5], pos, Quaternion.identity);
            }
        }
    }
}
