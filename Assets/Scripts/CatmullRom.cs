using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Interpolation between points with a Catmull-Rom spline
public class CatmullRom : MonoBehaviour
{
    //public Transform plane;
	public static CatmullRom instance;
	//Has to be at least 4 points
	public List<MarkerTarget> controlPointsList = new List<MarkerTarget>();
	//Are we making a line or a loop?
	public bool isLooping = true;
	LineRenderer lr;
	private void Awake ()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			Debug.LogError("multiple instances of catmull");
		}
		instance = this;
		lr = GetComponent<LineRenderer>();
	}

	private void Update ()
	{
		Vector3[] positions = new Vector3[controlPointsList.Count];
		controlPointsList.Sort((m1, m2) => m1.pointId.CompareTo(m2.pointId));
		for (int i = 0; i < controlPointsList.Count; i++)
		{
			positions[i] = controlPointsList[i].transform.position;
		}

		lr.positionCount = positions.Length;
		lr.SetPositions(positions);
        if(controlPointsList.Count> 0)
        {
			transform.forward = -controlPointsList[0].transform.up; 
            //plane.position = controlPointsList[0].transform.position - controlPointsList[0].transform.up * 0.1f;
            //plane.rotation = controlPointsList[0].transform.rotation;
        }
        
	}

	//Display without having to press play
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.white;

		//Draw the Catmull-Rom spline between the points
		for (int i = 0; i < controlPointsList.Count; i++)
		{
			//Cant draw between the endpoints
			//Neither do we need to draw from the second to the last endpoint
			//...if we are not making a looping line
			if ((i == 0 || i == controlPointsList.Count - 2 || i == controlPointsList.Count - 1) && !isLooping)
			{
				continue;
			}

			DisplayCatmullRomSpline(i);
		}
	}

	public void AddPoint(MarkerTarget t)
	{
		controlPointsList.Add(t);
	}

	public void RemovePoint (MarkerTarget t)
	{
		controlPointsList.Remove(t);
	}

	//Display a spline between 2 points derived with the Catmull-Rom spline algorithm
	void DisplayCatmullRomSpline (int pos)
	{
		//The 4 points we need to form a spline between p1 and p2
		Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].transform.position;
		Vector3 p1 = controlPointsList[pos].transform.position;
		Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].transform.position;
		Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].transform.position;

		//The start position of the line
		Vector3 lastPos = p1;

		//The spline's resolution
		//Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
		float resolution = 0.2f;

		//How many times should we loop?
		int loops = Mathf.FloorToInt(1f / resolution);

		for (int i = 1; i <= loops; i++)
		{
			//Which t position are we at?
			float t = i * resolution;

			//Find the coordinate between the end points with a Catmull-Rom spline
			Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

			//Draw this line segment
			Gizmos.DrawLine(lastPos, newPos);

			//Save this pos so we can draw the next line segment
			lastPos = newPos;
		}
	}

	//Clamp the list positions to allow looping
	int ClampListPos (int pos)
	{
		if (pos < 0)
		{
			pos = controlPointsList.Count - 1;
		}

		if (pos > controlPointsList.Count)
		{
			pos = 1;
		}
		else if (pos > controlPointsList.Count - 1)
		{
			pos = 0;
		}

		return pos;
	}

	//Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
	//http://www.iquilezles.org/www/articles/minispline/minispline.htm
	Vector3 GetCatmullRomPosition (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * t + c * t^2 + d * t^3
		Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}
}