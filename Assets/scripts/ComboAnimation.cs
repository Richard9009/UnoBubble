using UnityEngine;
using System.Collections;

public enum ComboName {
	NONE = 1, COMBO, BRAVO, AWESOME, SUPER, PERFECT
};

public class ComboAnimation : MonoBehaviour {

	public static ComboAnimation create(ComboName combo)
	{
		GameObject obj = new GameObject ();
		ComboAnimation com = obj.AddComponent (typeof(ComboAnimation)) as ComboAnimation;
		com.Init (combo);
		return com;
	}

	public void Init(ComboName combo)
	{
		Sprite image = Resources.Load<Sprite> (combo.ToString ());
		(gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer).sprite = image;
		transform.localScale = Vector3.zero;
		transform.position = new Vector3 (0, 0, -5);

		XCAnimation.Create (gameObject).ScaleTo (Vector3.one, 0.7f);
		Destroy (gameObject, 1.5f);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
