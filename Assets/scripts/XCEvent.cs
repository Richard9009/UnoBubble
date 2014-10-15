using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct EventData
{
	public string name;
	public List<MonoBehaviour> objects;
	public List<string> callback;

	public EventData(string evt_name)
	{
		name = evt_name;
		objects = new List<MonoBehaviour> ();
		callback = new List<string> ();
	}
}

public class XCEvent 
{
	static List<EventData> events = new List<EventData>();

	static public void DispatchEvent(string name, Hashtable parameters)
	{
		for (int i = 0; i < events.Count; i++) {
			EventData evt = events[i];
			for(int j = 0; j < evt.objects.Count;j++) {
				evt.objects[j].SendMessage(evt.callback[j], parameters);
			}
		}
	}

	//Method should be a function with 1 DictionaryBase parameter
	static public void AddListener(string evt_name, MonoBehaviour obj, string method)
	{
		EventData evt = new EventData (string.Empty);
		for (int i = 0; i < events.Count; i++) {
			if(events[i].name == evt_name)
			{
				evt = events[i];
				for (int j = 0; j < evt.objects.Count; j++) {
					if(evt.objects[j] == obj && evt.callback[j] == method)
						return; //identical event listener exists
				}

				break;
			}
		}

		if(evt.name == string.Empty) 
			evt = new EventData (evt_name);

		evt.objects.Add(obj);
		evt.callback.Add(method);
	}

	//remove one listener of a specific method for an object
	static public void RemoveListener(string evt_name, MonoBehaviour obj, string method)
	{
		for (int i = 0; i < events.Count; i++) {
			EventData evt = events [i];
			for (int j = evt.objects.Count - 1; j >= 0; j--) {
				if(evt.objects[j] == obj && evt.callback[j] == method)
				{
					evt.objects.RemoveAt(j);
					evt.callback.RemoveAt(j);
					break;
				}
			}
		}
	}

	//remove all listeners added on an object
	static void RemoveAllListeners(string evt_name, MonoBehaviour obj)
	{
		for (int i = 0; i < events.Count; i++) {
			EventData evt = events [i];
			for (int j = evt.objects.Count - 1; j >= 0; j--) {
				if(evt.objects[j] == obj)
				{
					evt.objects.RemoveAt(j);
					evt.callback.RemoveAt(j);
				}
			}
		}
	}
}

