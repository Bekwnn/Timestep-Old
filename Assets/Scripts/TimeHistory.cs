using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeInfo<T> {
	public T data;
	public float time { get; set; }
	
	public TimeInfo(T value, float time)
	{
		data = value;
		this.time = time;
	}

	//TODO: fix comparison?
	public static bool operator == (TimeInfo<T> lhs, TimeInfo<T> rhs)
	{
		return lhs.data.Equals(rhs.data);
	}
	
	public static bool operator != (TimeInfo<T> lhs, TimeInfo<T> rhs)
	{
		return !(lhs.data.Equals(rhs.data));
	}

	public override bool Equals (object obj)
	{
		return base.Equals (obj);
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}

/// <summary>
/// Interpolates data between frames. Good for movement.
/// Do not use with values which should not transition smoothly.
/// </summary>
public class TimeInfoInterp<T> : TimeInfo<T>
{
	public readonly float valueStartTime; //when this info was first created

	public TimeInfoInterp(T value, float time) : base(value, time)
	{
		valueStartTime = time;
	}

	//TODO: write function that returns interped value
}

public class TimeHistory<T>
{
	//seperate lists because C# generics are a headache
	private List<TimeInfo<T>> history;
	private List<TimeInfoInterp<T>> historyInterp;
	public readonly bool bInterpolate;

	public readonly float snapshotInterval;	//measured in time relative to the object!
	
	public TimeHistory(T value, int snapshotsPerSecond, bool bInterpolate = false)
	{
		if (bInterpolate)
		{
			historyInterp = new List<TimeInfoInterp<T>>();
			historyInterp.Add(new TimeInfoInterp<T>(value, 0f));
		}
		else
		{
			history = new List<TimeInfo<T>>();
			history.Add(new TimeInfo<T>(value, 0f));
		}

		Mathf.Clamp (snapshotsPerSecond, 1, 120);
		snapshotInterval = 1f / snapshotsPerSecond;
	}

	public static void ForwardUpdate<T>(TimeHistory<T> timeHistory, float time, T value) 
	{
		if (timeHistory.bInterpolate)
		{
			List<TimeInfoInterp<T>> hist = timeHistory.historyInterp;
			if (time - hist[hist.Count - 1].time > timeHistory.snapshotInterval)
			{
				if (!value.Equals(hist[hist.Count - 1].data))
				{
					hist.Add(new TimeInfoInterp<T>(value, time));
				}
				else
				{
					hist[hist.Count - 1].time = time;
				}
			}
		}
		else
		{
			List<TimeInfo<T>>hist = timeHistory.history;
			if (time - hist[hist.Count - 1].time > timeHistory.snapshotInterval)
			{
				if (!value.Equals(hist[hist.Count - 1].data))
				{
					hist.Add(new TimeInfo<T>(value, time));
				}
				else
				{
					hist[hist.Count - 1].time = time;
				}
			}
		}
	}

	//TODO: return interpolate struct since C# can't into generics arithmatic
	public static TimeInfo<T> RewindTo<T>(TimeHistory<T> timeHistory, float time)
	{
		float clampedTime = (time >= 0f)? time : 0f;

		if (timeHistory.bInterpolate)
		{
			List<TimeInfoInterp<T>> hist = timeHistory.historyInterp;
			while (hist.Count > 1 && hist[hist.Count - 2].time >= clampedTime)
			{
				hist.RemoveAt(hist.Count - 1);
			}
			hist[hist.Count-1].time = clampedTime;
			TimeInfo<T> valueAt = new TimeInfo<T>(hist[hist.Count - 1].data, hist[hist.Count - 1].time);
			return valueAt;
		}
		else
		{
			List<TimeInfo<T>> hist = timeHistory.history;
			while (hist.Count > 1 && hist[hist.Count - 2].time >= clampedTime)
			{
				hist.RemoveAt(hist.Count - 1);
			}
			hist[hist.Count-1].time = clampedTime;
			TimeInfo<T> valueAt = new TimeInfo<T>(hist[hist.Count - 1].data, hist[hist.Count - 1].time);
			return valueAt;
		}
	}
}