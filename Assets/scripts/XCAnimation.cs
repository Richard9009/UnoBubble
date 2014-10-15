using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XCAnimation : MonoBehaviour {

	public enum AnimationAttribute {
		POSITION = 1, SCALE, ALPHA, COLOR
	};

	private AnimationAttribute attribute;
	private Vector3 end_value;
	private Vector3 speed_per_frame;
	private int delay_frame = 0;
	private int frame_count = 0;

	public static XCAnimation Create(GameObject obj)
	{
		return obj.AddComponent(typeof(XCAnimation)) as XCAnimation;
	}

	//how much time left from now until animation finishes in seconds
	public float TotalTime {
		get { return (float)(delay_frame + frame_count) / Util.FRAME_RATE; }
	}

	//wait time until the animation starts
	public XCAnimation Delay(float seconds)
	{
		delay_frame += Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		return this;
	}

	//runs [anim] immediately after this animation finished. This function will return [anim] back
	public XCAnimation Link(XCAnimation anim)
	{
		anim.Delay (TotalTime);
		return anim;
	}

	public XCAnimation MoveTo(Vector3 endPos, float seconds)
	{
		attribute = AnimationAttribute.POSITION;
		end_value = endPos;
		frame_count = Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		if (frame_count == 0) return this;
		Vector3 diff = endPos - gameObject.transform.position;
		speed_per_frame = new Vector3(diff.x/frame_count, diff.y/frame_count, diff.z/frame_count);
		return this;
	}

	public XCAnimation MoveBy(Vector3 movement, float seconds)
	{
		return MoveTo (transform.position + movement, seconds);
	}

	//Fade can only be applied to objects with SpriteRenderer
	public XCAnimation FadeTo(float alpha, float seconds)
	{
		TextMesh t_mesh = gameObject.GetComponent (typeof(TextMesh)) as TextMesh;
		attribute = AnimationAttribute.ALPHA;
		end_value = new Vector3(alpha, 0, 0);
		frame_count = Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		if (frame_count == 0) return this;
		float color_a = t_mesh == null ? (renderer as SpriteRenderer).color.a : t_mesh.color.a;
		speed_per_frame = new Vector3 (end_value.x - color_a/ frame_count, 0, 0);
		return this;
	}

	//Make object more and more transparent until it disappeared
	public XCAnimation FadeOut(float seconds)
	{
		return FadeTo (0.0f, seconds);
	}

	public XCAnimation FadeIn(float seconds)
	{
		return FadeTo (1.0f, seconds);
	}

	public XCAnimation ScaleTo(Vector3 scale, float seconds)
	{
		attribute = AnimationAttribute.SCALE;
		end_value = scale;
		frame_count = Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		if (frame_count == 0) return this;
		Vector3 diff = scale - gameObject.transform.localScale;
		speed_per_frame = new Vector3(diff.x/frame_count, diff.y/frame_count, diff.z/frame_count);
		return this;
	}

	public XCAnimation ScaleBy(Vector3 scale, float seconds)
	{
		return ScaleTo (transform.localScale + scale, seconds);
	}

	//scale down until object disappear
	public XCAnimation Shrink(float seconds)
	{
		return ScaleTo (Vector3.zero, seconds);
	}


//=============================================================================================================

	private void SetValue(Vector3 val)
	{
		switch(attribute) {
		case AnimationAttribute.POSITION :
			transform.position = val; break;	
		case AnimationAttribute.ALPHA :
			TextMesh t_mesh = gameObject.GetComponent (typeof(TextMesh)) as TextMesh;
			if(t_mesh == null) {
				Color col = (renderer as SpriteRenderer).color;
				(renderer as SpriteRenderer).color = new Color(col.r, col.g, col.b, val.x);
			}
			else {
				Color col = t_mesh.color;
				t_mesh.color = new Color(col.r, col.g, col.b, val.x);
			}
			break;
		case AnimationAttribute.SCALE :
			transform.localScale = val; break;	
		}
	}

	private Vector3 GetCurrentValue()
	{
		switch(attribute) {
			case AnimationAttribute.POSITION :
				return transform.position;	
			case AnimationAttribute.ALPHA :
				TextMesh t_mesh = gameObject.GetComponent (typeof(TextMesh)) as TextMesh;
				if(t_mesh == null) return new Vector3((renderer as SpriteRenderer).color.a, 0, 0);
				else return new Vector3(t_mesh.color.a, 0, 0);
			case AnimationAttribute.SCALE :
				return transform.localScale;
			default : return Vector3.zero;
		} 
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (delay_frame > 0) {
			delay_frame--;
			Vector3 diff = end_value - GetCurrentValue();
			speed_per_frame = new Vector3(diff.x/frame_count, diff.y/frame_count, diff.z/frame_count);
		}
		else if(frame_count > 0) {
			SetValue(GetCurrentValue() + speed_per_frame);
			frame_count--;
		}
		else {
			SetValue(end_value);
			Destroy(this);
		}
	}
}
