using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {

	private static TimeManager _global;
	public static TimeManager global
	{
		get
		{
			if (_global == null)
				_global = GameObject.FindObjectOfType<TimeManager> ();
			return _global;
		}
	}
	
	public float timeScale { get; set; }
	public float time { get; private set; }


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime * timeScale;
		time = (time <= 0f) ? 0 : time;
	}
}
