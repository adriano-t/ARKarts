using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Interpolation between points with a Catmull-Rom spline
public class CatmullRom : MonoBehaviour
{
    //public Transform plane;
	public static CatmullRom instance;
	public Transform colliderChild;
	//Has to be at least 4 points
	public List<MarkerTarget> controlPointsList = new List<MarkerTarget>();
	private Vector3[] positions;

	float refreshTime = 0;
	LineRenderer lr;
	MeshCollider meshCollider;

	public Transform goal;
	public Vector3 startDirection;
	Mesh mesh;
	private void Awake ()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			Debug.LogError("multiple instances of catmull");
		}
		instance = this;
		lr = GetComponent<LineRenderer>();
		meshCollider = colliderChild.GetComponent<MeshCollider>();
		mesh = new Mesh();
	}

	private void Update ()
	{
		if(refreshTime <= 0)
			refreshTime = 0.5f; //half second
		else
		{
			refreshTime -= Time.deltaTime;
			return;
		}

		float resolution = 0.1f;
		int segments = Mathf.FloorToInt(1f / resolution);

		//sort points
		controlPointsList.Sort((m1, m2) => m1.pointId.CompareTo(m2.pointId));

		Vector3 trackUp = Vector3.up;
		if(controlPointsList.Count > 0)
			trackUp = controlPointsList[0].transform.up;

		//generate points from the markers
		positions = new Vector3[controlPointsList.Count * segments];

		int elem = 0;
		if (controlPointsList.Count >= 4)  
		for (int i = 0; i < controlPointsList.Count; i++)
		{
			//catmull algorithm: 4 points
			Vector3 p0 = controlPointsList[ClampListPos(i - 1)].transform.position;
			Vector3 p1 = controlPointsList[i].transform.position;
			Vector3 p2 = controlPointsList[ClampListPos(i + 1)].transform.position;
			Vector3 p3 = controlPointsList[ClampListPos(i + 2)].transform.position;

			int startElem = elem;
			for (int j = 1; j <= segments; j++)
			{
				//Which t position are we at?
				float t = j * resolution;

				//Find the coordinate between the end points with a Catmull-Rom spline
				Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
				 
				//align to the first
				if (i > 0)
				{
					newPos = Vector3.ProjectOnPlane(newPos - positions[0], trackUp) + positions[0];

				}
				positions[elem] = newPos;

				elem++;

			}

			//muovi la barra sul punto della pista
			Transform child = controlPointsList[i].transform.GetChild(0);
			child.position = positions[startElem];

		}


		//orienta i target intermedi
		for (int i = 0; i < controlPointsList.Count; i++)
		{
			int startElem = i * segments;
			Transform child;
			if (positions.Length > 1)
			{
				//ottieni il punto precedente
				int idx1 = startElem - 1;
				if (idx1 < 0)
					idx1 = positions.Length - 1;

				//ottieni il punto successivo
				int idx2 = (startElem + 1) % positions.Length;

				//orienta usando il punto precedente e il successivo
				Quaternion rot = Quaternion.LookRotation((positions[idx2] - positions[idx1]).normalized, trackUp);

				child = controlPointsList[i].transform.GetChild(0);
				child.rotation = rot;
			}
		}


		if (positions.Length > 2)
		{
			//sposta la bandiera d'arrivo nel punto corretto e la orienta
			goal.position = positions[0];
			goal.rotation = Quaternion.LookRotation((positions[positions.Length - 1] - positions[1]).normalized, trackUp);

		}

		//align the spline direction to the first marker
		lr.positionCount = positions.Length;
		lr.SetPositions(positions);
        if(controlPointsList.Count> 0)
        {
			transform.forward = -trackUp; 
			
			colliderChild.transform.rotation = Quaternion.identity;
            //plane.position = controlPointsList[0].transform.position - controlPointsList[0].transform.up * 0.1f;
            //plane.rotation = controlPointsList[0].transform.rotation;
        }
		
		//bake a collision mesh
		lr.BakeMesh(mesh, true);
		meshCollider.sharedMesh = mesh;
	}

	 

	public void AddPoint(MarkerTarget t)
	{
		controlPointsList.Add(t);
	}

	public void RemovePoint (MarkerTarget t)
	{
		controlPointsList.Remove(t);
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