using UnityEngine;
using System.Collections.Generic;
using Assets.SuperGoalie.Scripts.Entities;
using Assets.SuperGoalie.Scripts.Managers;

public class KickScript : MonoBehaviour
{
    [SerializeField] private GameObject leftFoot;
    [SerializeField] private GameObject rightFoot;
    [SerializeField] private GameObject head;
    //[SerializeField] private BallScript ballScript;
    //[SerializeField] private BallPhysic parabolaPhysics;

    public Ball _ball;
    public delegate void BallLaunch(float power, Vector3 target);   //delegate to launch a ball
    public BallLaunch OnBallLaunch;                                 //on ball launch

    [SerializeField]
    GoalKeeper _goalKeeper;

    private readonly List<float> timeList = new();
    private readonly List<float> footPos = new();
    //private Vector3 angleOfKick;
    private float time;
    private bool kickStarted;

    //vi tri chan dau va cuoi
    float firtFootPos = 0;
    float lastFootPos = 0;

    public Vector3 endPoint;     // điểm kết thúc
    public float height = 3f;      // độ cao cực đại của quỹ đạo
    public float width = 3f;      // độ ngang cực đại của quỹ đạo
    public float duration = 2f;    // thời gian bay (tốc độ)

    public Transform goalTopLeft;
    public Transform goalBottomCenter;
    public Transform goalTopRight;
    private void Start()
    {
        //register entities to local delegates
        OnBallLaunch += _ball.Instance_OnBallLaunch;
        _ball.OnBallLaunched += _goalKeeper.Instance_OnBallLaunched;
    }

    private void Update()
    {
        float posOfFoot = (leftFoot.transform.position.z - head.transform.position.z);
        if (!GameManager.Instance.Run) return;
        // Bắt đầu đá
        if (posOfFoot > 0.1f && !kickStarted)
        {
            kickStarted = true;
            time = 0f;
            timeList.Clear();
            footPos.Clear();
            firtFootPos = rightFoot.transform.position.x;
        }

        if (kickStarted)
            DetectKick(posOfFoot);
    }

    private void DetectKick(float posOfFoot)
    {
        time += Time.deltaTime;
        footPos.Add(posOfFoot);
        timeList.Add(time);

        // Kết thúc cú đá (chân qua hết bóng)
        if (posOfFoot < -0.4f)
        {
            lastFootPos = rightFoot.transform.position.x;
            //angleOfKick = (leftFoot.transform.position - head.transform.position).normalized;
            //GoalieAI.angle = angleOfKick;
            kickStarted = false;
            GameManager.Instance.Run = false;
            //parabolaPhysics.ReKick(firtFootPos - lastFootPos < 0);
            BallLaunch tempBallLaunch = OnBallLaunch;
            if (tempBallLaunch != null)
            {
                endPoint = GetRandomGoalPoint(firtFootPos - lastFootPos < 0);
                tempBallLaunch.Invoke(40, endPoint);
            }

            GameManager.Instance.RiSet();
        }
    }

    Vector3 GetRandomGoalPoint(bool leftGoal)
    {
        float randomX = 0;
        float randomY = 3;
        if (leftGoal)
        {
            randomX = Random.Range(goalTopLeft.position.x, goalBottomCenter.position.x);
            randomY = Random.Range(goalBottomCenter.position.y, goalTopLeft.position.y);
        }
        else
        {
            randomX = Random.Range(goalBottomCenter.position.x, goalTopRight.position.x);
            randomY = Random.Range(goalBottomCenter.position.y, goalTopRight.position.y);
        }
        //Debug.Log("random pos " + randomX + " :: " + randomY);
        return new Vector3(randomX, randomY, goalTopLeft.position.z);
    }
}
