using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.Dynamic;
using System.Threading;

public class FixedSplineTrack : MonoBehaviour
{

    public Spline spline;
    public GameObject trackPart;
    public GameObject partentObject;

    public float Speed;

    public float lenght;

    private int count;

    public float fireDelta = 0.5F;

    private float nextFire = 0.5F;
    private float myTime = 0.0f;


    private List<GameObject> track = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        spline.GetPosition(100);
        createPlates();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        myTime = myTime + Time.deltaTime;
        if (/*Input.GetButton("Fire1") &&*/ myTime > nextFire)
        {
            nextFire = myTime + fireDelta;
            moveTrack();
            nextFire = nextFire - myTime;
            myTime = 0.0F;
        }
       
    }


    private void moveTrack()
    {
        for (int i = 0; i < track.Count; i++)
        {
            float perc = getPercentage(track[i]);
            perc = perc + Speed;
            if (perc > 100) {
                perc = perc - 100;
            }

            track[i].GetComponent<Rigidbody>().MovePosition(spline.GetPosition(perc));
        }
    }

    private void createPlates()
    {
        count = calcPieceAmount(spline);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = calcPosition(i);
            Quaternion dir = Quaternion.LookRotation(calcDirection(i), Vector3.up);
            GameObject activePart = Instantiate(trackPart, pos, dir);
            track.Add(activePart);
            activePart.transform.SetParent(partentObject.transform);
            /*if (isflipped(activePart)) {
                Vector3 rot = activePart.transform.localScale;
                //rot.x = rot.x + 180;
                rot.y = -rot.y;
                activePart.transform.localScale = rot;
            }*/
            //if (i > 0) setUpPiece(activePart, i - 1);
            //if (i == calcPieceAmount(spline) - 1) setUpPiece(track[0], i - 1);
        }
    }

    private Vector3 calcPosition(int count)
    {
        float perc = (float)(1f / (spline.Length / lenght)) * count;
        return spline.GetPosition(perc);
    }

    private Vector3 calcDirection(int count)
    {
        float perc = (float)(1f / (spline.Length / lenght)) * count;
        return spline.GetDirection(perc);
    }

    private Vector3 calcFacing(int count)
    {
        float perc = (float)(1f / (spline.Length / lenght)) * count;
        return spline.Right(perc);
    }

    private int calcPieceAmount(Spline actSpline)
    {
        //Debug.Log(spline.Length);
        return (int)(Mathf.Round((actSpline.Length / lenght)));
    }

    private float getPercentage(GameObject actObj)
    {
        return spline.ClosestPoint(actObj.transform.position);
    }
}
