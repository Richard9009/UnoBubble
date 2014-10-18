using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum MatchStatus {
	MATCH = 1, NOT_MATCH, SUPER_MATCH
};

public class GameManager {

	// Use this for initialization
	public static int stage_num = 1;
	private static GameManager instance = null;

	private GameObject current_bubble = null;
	private int current_num = -1;
	private Color current_col = Color.white;
	private int next_combo = 10;

	private int combo_count = 0;
	public int ComboCount { get { return combo_count; } }

	private ComboName combo_level = ComboName.NONE;
	public  ComboName ComboLevel { get { return combo_level; } }

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
		if (current_bubble != null) {
			XCAnimation.StopAllAnimations(current_bubble);
			(current_bubble.GetComponent ("BubbleBase") as BubbleBase).ReleaseAnimation ();
		}
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
		next_combo = 10;
		combo_level = ComboName.NONE;

		if (current_bubble != null) {
			current_bubble.name = "released bubble";
			(current_bubble.GetComponent ("BubbleBase") as BubbleBase).ReleaseAnimation ();
		}
	}

	public MatchStatus IsValidBubble(Color col, int num)
	{
		MatchStatus match = (current_col == Color.white || current_num == -1) ? MatchStatus.MATCH : MatchStatus.NOT_MATCH;
		if(match == MatchStatus.NOT_MATCH){
			if(current_col ==  col && current_num == num)
				match = MatchStatus.SUPER_MATCH;
			else if(current_col ==  col || current_num == num)
				match = MatchStatus.MATCH;
		}

		if(match == MatchStatus.NOT_MATCH)
			combo_count = 0;
		else {
			if(match == MatchStatus.SUPER_MATCH) 
				combo_count += 5; //super match, double score
			else combo_count++;

			current_col = col;
			current_num = num;

			if(combo_count >= next_combo)
				CallCombo();
		}

		return match;
	}

	private void CallCombo()
	{
		if(combo_level != ComboName.PERFECT)
			combo_level++;

		ComboAnimation.create (combo_level);
		XCEvent.DispatchEvent ("COMBO CALLED", null);
		Camera.main.audio.PlayOneShot(Resources.Load<AudioClip>("sounds/combo"));

		switch (combo_level) {
			case ComboName.PERFECT:
			case ComboName.SUPER:
			case ComboName.AWESOME:
					XCEvent.DispatchEvent("FIESTA", null);
					goto case ComboName.BRAVO;
			case ComboName.BRAVO:
					Hashtable param = new Hashtable();
					param.Add("color", current_col);
					XCEvent.DispatchEvent("CHANGE COLOR", param);
					goto case ComboName.COMBO;
			case ComboName.COMBO:
					XCEvent.DispatchEvent("SLOW DOWN", null);
					break;

		}

		switch (combo_level) {
			case ComboName.PERFECT: next_combo += 150; break;
			case ComboName.SUPER: next_combo = 200; break;
			case ComboName.AWESOME: next_combo = 100; break;
			case ComboName.BRAVO: next_combo = 50; break;
			case ComboName.COMBO: next_combo = 25; break;
		}
	}
	
	public int Score
	{
		get {
			int score = combo_count * 100;
			total_score += score;
			return score;
		}
	}

	public BubbleBase NewBubble
	{
		get{
			BubbleBase bubble = BubbleBase.create(Util.BubbleColor.getRandom(stage_data.colors), 
			                                 Random.Range(0, stage_data.max_num), stage_data.size, stage_data.gravity);
			if(bubble == null) return null;
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
