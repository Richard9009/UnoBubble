using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GameManager {

	// Use this for initialization
	public static int stage_num = 1;
	private static GameManager instance = null;

	private GameObject current_bubble = null;
	private int current_num = -1;
	private Color current_col = Color.white;
	private int combo_count = 0;

	private int total_score = 0;
	public int TotalScore { get { return total_score; } }

	private int remaining_bubbles = 50;
	public int RemainingBubbles { get { return remaining_bubbles; } }

	private Vector3 active_pos = new Vector3();
	public Vector3 ActiveBubblePos { get { return active_pos; } }

	private StageData stage_data = new StageData();
	public StageData CurrentStageData { get { return stage_data; } }

	public struct MissionData
	{
		string target;
		int amount;
	};

	public struct StageData
	{
		public float size;
		public float gravity;
		public int colors;
		public int max_num;
		public int goal;
		public int interval;
		public MissionData[] missions;
		public int total_bubbles;
	};

	public GameManager()
	{
		float diameter = Util.FullscreenSize ().y * Util.PANEL_HEIGHT;
		active_pos = new Vector3 (-Util.GameAreaSize ().x / 2 + diameter/2,
		                               -Util.GameAreaSize ().y / 2 - diameter / 2, -5);
		loadStage ();
		remaining_bubbles = stage_data.total_bubbles;
	}

	public static GameManager getInstance()
	{
		if (instance == null)
			instance = new GameManager ();
		return instance;
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	void loadStage()
	{
		string json_data = (Resources.Load("stages/stage_" + stage_num.ToString ()) as TextAsset).text;
		JSONNode json = JSONNode.Parse (json_data);
		stage_data.size = json ["size"].AsInt * 0.11f + 0.5f;
		stage_data.colors = json ["colors"].AsInt;
		stage_data.max_num = json ["max_num"].AsInt;
		stage_data.goal = json ["goal"].AsInt;
		stage_data.gravity = json ["speed"].AsInt * 0.01f;
		stage_data.interval = Mathf.FloorToInt(json ["interval"].AsFloat * Util.FRAME_RATE);
		stage_data.total_bubbles = json ["bubbles"].AsInt;
	}

	public void setAsActiveBubble(GameObject bubble)
	{
		if (current_bubble != null)
			(current_bubble.GetComponent("BubbleBase") as BubbleBase).ReleaseAnimation();

		current_bubble = bubble;
	}

	public Vector3 GetActiveBubblePos()
	{
		return active_pos;
	}

	public void ReleaseBubble()
	{
		current_col = Color.white;
		current_num = -1;
		combo_count = 0;

		if (current_bubble != null)
			(current_bubble.GetComponent("BubbleBase") as BubbleBase).ReleaseAnimation();
	}

	public bool IsValidBubble(Color col, int num)
	{
		bool match = current_col == Color.white || current_num == -1;
		if(!match) match = current_col ==  col || current_num == num;

		if (match) {
			if(current_col == col && current_num == num && combo_count > 0) 
				combo_count *= 2; //super match, double score
			else combo_count++;

			current_col = col;
			current_num = num;
		} else
			combo_count = 0;

		return match;
	}

	public int Score
	{
		get {
			int score = combo_count * 100;
			total_score += score;
			return score;
		}
	}

	public GameObject NewBubble
	{
		get{
			GameObject bubble = new GameObject();
			(bubble.AddComponent(typeof(BubbleBase)) as BubbleBase).Init(Util.BubbleColor.getRandom(stage_data.colors), 
			                                 Random.Range(0, stage_data.max_num), stage_data.size, stage_data.gravity);
			remaining_bubbles--;
			return bubble;
		}
	}
		           
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
