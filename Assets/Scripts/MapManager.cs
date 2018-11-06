using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MapManager : MonoBehaviour
{
	[FormerlySerializedAs("OutWallArray")] public GameObject[] outWallArray;
	[FormerlySerializedAs("FloorArray")]   public GameObject[] floorArray;
	[FormerlySerializedAs("WallArray")]    public GameObject[] wallArray;
	[FormerlySerializedAs("FoodArray")]    public GameObject[] foodArray;
	[FormerlySerializedAs("EnemyArray")]   public GameObject[] enemyArray;
	[FormerlySerializedAs("ExitPrefab")]   public GameObject   exitPrefab;

	[FormerlySerializedAs("Rows")] public int rows = 10;
	[FormerlySerializedAs("Cols")] public int cols = 10;

	private          Transform     _mapHolder;
	private readonly List<Vector2> _positionList = new List<Vector2>();

	private GameManager _gameManager;
	private Camera      _camera;

	// 初始化地图
	public void InitMap()
	{
		_gameManager = GetComponent<GameManager>();

		if (_gameManager.level >= 12)
		{
			rows = cols = 11 + (_gameManager.level - 12) / 4;
		}

		_camera                  = GameObject.Find("Main Camera").GetComponent<Camera>();
		_camera.orthographicSize = cols / 2f;
		_camera.transform.Translate((cols - 1) / 2f, (cols - 1) / 2f, 0);
		_mapHolder = new GameObject("Map").transform;

		// 创建外墙和地板
		for (int x = 0; x < cols; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				if (x == 0 || y == 0 || x == cols - 1 || y == rows - 1)
				{
					int index = Random.Range(0, outWallArray.Length);
					GameObject go =
						Instantiate(outWallArray[index], new Vector3(x, y, 0), Quaternion.identity);
					go.transform.SetParent(_mapHolder);
				}
				else
				{
					int        index = Random.Range(0, floorArray.Length);
					GameObject go    = Instantiate(floorArray[index], new Vector3(x, y, 0), Quaternion.identity);
					go.transform.SetParent(_mapHolder);
				}
			}
		}

		_positionList.Clear();
		for (int x = 2; x < cols - 2; x++)
		{
			for (int y = 2; y < rows - 2; y++)
			{
				_positionList.Add(new Vector2(x, y));
			}
		}

		// 创建敌人 Level / 2 + Level / 4 * 3
		int enemyCount = Random.Range(0, _gameManager.level / 2 + _gameManager.level / 4 * 3);
		InstantiateItems(enemyCount, enemyArray);

		// 创建障碍物 5 ～ Level + 5
		int wallCount = Random.Range(5, _gameManager.level + 5);
		InstantiateItems(wallCount, wallArray);

		// 创建食物：2 ～ Level - Level / 5
		int foodCount = Random.Range(2, _gameManager.level - _gameManager.level / 5);
		InstantiateItems(foodCount, foodArray);

		// 创建出口
		GameObject go1 = Instantiate(exitPrefab, new Vector2(cols - 2, rows - 2), Quaternion.identity);
		go1.transform.SetParent(_mapHolder);

		Debug.Log("敌人：" + enemyCount + " 障碍物：" + wallCount + " 食物：" + foodCount);
	}

	private void InstantiateItems(int count, GameObject[] prefabs)
	{
		for (int i = 0; i < count; i++)
		{
			Vector2    posVector2 = RandomPosition();
			GameObject prefab     = RandomPrefab(prefabs);
			GameObject go         = Instantiate(prefab, posVector2, Quaternion.identity);
			go.transform.SetParent(_mapHolder);
		}
	}

	// 随机位置
	private Vector2 RandomPosition()
	{
		int     positionIndex = Random.Range(0, _positionList.Count);
		Vector2 posVector2    = _positionList[positionIndex];
		_positionList.RemoveAt(positionIndex);
		return posVector2;
	}

	// 随机预设
	private GameObject RandomPrefab(GameObject[] prefabs)
	{
		int index = Random.Range(0, prefabs.Length);
		return prefabs[index];
	}
}