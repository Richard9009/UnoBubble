using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	// Use this for initialization
	static public Block create(Vector3 pos)
	{
		GameObject obj = new GameObject ();
		obj.transform.position = pos;
		obj.name = "Block";
		return obj.AddComponent (typeof(Block)) as Block;
	}

	void Start () {
		SpriteRenderer sr = gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer;
		sr.sprite = Resources.Load<Sprite> ("blank_layer");
		sr.color = Color.red;

		gameObject.AddComponent (typeof(Rigidbody2D));
		rigidbody2D.isKinematic = true;
		gameObject.AddComponent (typeof(BoxCollider2D)); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Spike : MonoBehaviour {

	static public Spike create(Vector3 pos)
	{
		GameObject obj = new GameObject ();
		obj.transform.position = pos;
		obj.name = "Spike";
		return obj.AddComponent (typeof(Spike)) as Spike;
	}

	void Start()
	{
		SpriteRenderer sr = gameObject.AddComponent (typeof(SpriteRenderer)) as SpriteRenderer;
		sr.sprite = Resources.Load<Sprite> ("blank_layer");
		sr.color = Color.blue;

		gameObject.AddComponent (typeof(BoxCollider2D));
		collider2D.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		(other.gameObject.GetComponent (typeof(BubbleBase)) as BubbleBase).Pop ();
	}
}