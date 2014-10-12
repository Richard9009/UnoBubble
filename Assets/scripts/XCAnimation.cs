using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class XCAnimation : MonoBehaviour {
	
	public string attribute;
	public Vector3 end_value;
	public Vector3 speed_per_frame;
	public int delay_frame = 0;
	public int frame_count = 0;

	public XCAnimation Delay(float seconds)
	{
		delay_frame = Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		return this;
	}

	public XCAnimation MoveTo(Vector3 endPos, float seconds)
	{
		attribute = "position";
		end_value = endPos;
		frame_count = Mathf.CeilToInt (seconds * Util.FRAME_RATE);
		if (frame_count == 0) return this;
		Vector3 diff = endPos - gameObject.transform.position;
		speed_per_frame = new Vector3(diff.x/frame_count, diff.y/frame_count, diff.z/frame_count);
		return this;
	}

	private void SetValue(Vector3 val)
	{
		if (attribute == "position")
			transform.position = val;
		
	}

	private Vector3 GetCurrentValue()
	{
		if (attribute == "position") 
			return transform.position;

		return new Vector3 ();
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
