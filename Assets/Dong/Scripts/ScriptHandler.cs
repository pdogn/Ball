using UnityEngine;
using System.Collections;

public class ScriptHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private Transform ballTransform;
    //[SerializeField] private GoalieAI goalie;
    //[SerializeField] private TMPro.TextMeshProUGUI uiText;

    [SerializeField] private BallPhysic parabolaPhysics;

    [SerializeField] private ParticleSystem GoalEffect;
 
    private Vector3 initialBallPosition;
    //private bool canKick = true;

    private void Start()
    {
        if (ballTransform == null)
            ballTransform = GameObject.Find("Ball")?.transform;

        if (ballRigidbody == null && ballTransform)
            ballRigidbody = ballTransform.GetComponent<Rigidbody>();

        //if (goalie == null)
            //goalie = FindObjectOfType<GoalieAI>();

        initialBallPosition = ballTransform.position;
        UpdateUI(true);
    }

    //public IEnumerator NextKick()
    //{
    //    canKick = false;
    //    UpdateUI(false);

    //    yield return new WaitForSeconds(4f);

    //    // Reset b¨®ng
    //    ballRigidbody.velocity = Vector3.zero;
    //    ballRigidbody.angularVelocity = Vector3.zero;
    //    ballTransform.position = initialBallPosition;

    //    ballRigidbody.constraints = RigidbodyConstraints.FreezeAll;

    //    yield return new WaitForSeconds(1f);

    //    ballRigidbody.constraints = RigidbodyConstraints.None;
    //    BallScript.goal = null;

    //    //goalie?.ResetPosition();

    //    canKick = true;
    //    UpdateUI(true);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Nextkickk();
        }
    }

    public void Nextkickk()
    {
        //yield return new WaitForSeconds(2f);
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballTransform.position = initialBallPosition;
        ballTransform.eulerAngles = Vector3.zero;
        ballTransform.gameObject.SetActive(true);
        parabolaPhysics.canKick = true;
    }
    private IEnumerator Next()
    {
        yield return new WaitForSeconds(3f);
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballTransform.position = initialBallPosition;
        ballTransform.eulerAngles = Vector3.zero;
        ballTransform.gameObject.SetActive(true);
        parabolaPhysics.canKick = true;
    }

    public void STCR()
    {
        StartCoroutine(Next());
    }



    private void UpdateUI(bool showKick)
    {
        //if (uiText)
        //    uiText.text = showKick ? "KICK" : "";
    }

    public void PlayGoalEffect(Transform pos)
    {
        GoalEffect.gameObject.transform.position = pos.position;
        GoalEffect.Play();
    }
}
