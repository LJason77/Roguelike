using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;

	public static GameManager Instance
	{
		get { return _instance; }
	}

	[FormerlySerializedAs("Level")]   public int       level = 1;
	[FormerlySerializedAs("Food")]    public int       food  = 100;
	[FormerlySerializedAs("DieClip")] public AudioClip dieClip;

	[HideInInspector] public List<Enemy> enemiesList = new List<Enemy>();

	// 是否到达终点
	[HideInInspector] public bool isEnd;

	private bool       _sleepStep;
	private Text       _foodText;
	private Text       _failText;
	private Image      _dayImage;
	private Text       _dayText;
	private Text       _day;
	private Button     _quit;
	private Button     _restart;
	private Player     _player;
	private MapManager _mapManager;

	private int _show;

	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);
		InitGame();
	}

	private void InitGame()
	{
		// 初始化地图
		_mapManager = GetComponent<MapManager>();
		_mapManager.InitMap();

		// 初始化 UI
		_day      = GameObject.Find("Day").GetComponent<Text>();
		_foodText = GameObject.Find("FoodText").GetComponent<Text>();
		UpdateFoodText(0);
		_failText         = GameObject.Find("FailText").GetComponent<Text>();
		_failText.enabled = false;
		_player           = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		_dayImage         = GameObject.Find("DayImage").GetComponent<Image>();
		_dayText          = GameObject.Find("DayText").GetComponent<Text>();
		_restart          = GameObject.Find("RestartButton").GetComponent<Button>();
		_quit             = GameObject.Find("QuitButton").GetComponent<Button>();

		_day.text     = "Day " + level;
		_dayText.text = "Day " + level;
		_quit.gameObject.SetActive(false);
		_restart.gameObject.SetActive(false);
		Invoke("HideBlack", 1);

		// 初始化参数
		isEnd = false;
		enemiesList.Clear();
	}

	void UpdateFoodText(int foodChange)
	{
		if (foodChange == 0)
		{
			_foodText.text = "Food:" + food;
		}
		else
		{
			string s;
			if (foodChange < 0)
			{
				s = foodChange.ToString();
			}
			else
			{
				s = "+" + foodChange;
			}

			_foodText.text = s + " Food:" + food;
		}
	}

	public void Die()
	{
		if (food <= 0)
		{
			AudioManager.Instance.StopBgMusic();
			AudioManager.Instance.DieAudio(dieClip);
			_failText.enabled = true;
		}
	}

	public void ReduceFood(int count)
	{
		food -= count;
		UpdateFoodText(-count);
	}

	public void AddFood(int count)
	{
		food += count;
		UpdateFoodText(count);
	}

	public void OnPlayerMove()
	{
		if (_sleepStep)
		{
			_sleepStep = false;
		}
		else
		{
			foreach (var enemy in enemiesList)
			{
				enemy.Move();
			}

			_sleepStep = true;
		}

		// 检测是否到达终点
		if (_player.targetPos.x == _mapManager.cols - 2 && _player.targetPos.y == _mapManager.rows - 2)
		{
			isEnd = true;
			// 加载下一个关卡
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.LoadScene("Main");
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mod)
	{
		level++;
		// 初始化游戏
		InitGame();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void HideBlack()
	{
		_dayImage.gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_show % 2 == 0)
			{
				_dayImage.gameObject.SetActive(true);
				AudioManager.Instance.StopBgMusic();
				_quit.gameObject.SetActive(true);
				_restart.gameObject.SetActive(true);
			}
			else
			{
				_dayImage.gameObject.SetActive(false);
				AudioManager.Instance.PlayBgMusic();
				_quit.gameObject.SetActive(true);
				_restart.gameObject.SetActive(true);
			}

			_show++;
		}
	}
}