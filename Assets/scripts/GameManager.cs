using UnityEngine;
using System.Collections;

public class GameManager {

	// Use this for initialization
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
		int size;
		int gravity;
		int colors;
		MissionData[] missions;
		int total_bubbles;
	};

	public GameManager()
	{
		float diameter = Util.FullscreenSize ().y * Util.PANEL_HEIGHT;
		active_pos = new Vector3 (-Util.GameAreaSize ().x / 2 + diameter/2,
		                               -Util.GameAreaSize ().y / 2 - diameter / 2, -5);
		Debug.Log (active_pos.ToString());
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
			if(current_col == col && current_num == num) 
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
			(bubble.AddComponent(typeof(BubbleBase)) as BubbleBase).Init(Util.BubbleColor.getRandom(), Random.Range(0, 10), 1.5f, 0.01f);
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
