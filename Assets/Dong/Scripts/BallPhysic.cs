using UnityEngine;
using Assets.SuperGoalie.Scripts.Entities;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class BallPhysic : MonoBehaviour
{
    private ScriptHandler handler;

    public Transform startPoint;   // điểm bắt đầu
    //public Vector3 endPoint;     // điểm kết thúc
    public float height = 3f;      // độ cao cực đại của quỹ đạo
    public float width = 3f;      // độ ngang cực đại của quỹ đạo
    //public float duration = 2f;    // thời gian bay (tốc độ)

    Transform model;
    public Ball _ball;
    //private float time = 0f;
    //private Vector3 lastPos;
    //Vector3 moveDir;

    //bool endPath;
    //bool step2;
    //Rigidbody rb;

    public bool canKick;

    public Transform goalTopLeft;
    public Transform goalBottomCenter;
    public Transform goalTopRight;

    private Sequence seq;
    Vector3 targetLocalPos;
    Vector3 midLocalPos;
    Vector3 localOffset = new Vector3(0, 2, 0);

    void Start()
    {
        // Gán vị trí bắt đầu ban đầu
        transform.position = startPoint.position;
        //lastPos = transform.position;
        //rb = GetComponent<Rigidbody>();

        model = this.transform.GetChild(0);
        targetLocalPos = model.localPosition;

        handler = FindObjectOfType<ScriptHandler>();
        handler?.Nextkickk();
    }


    public void CurveTheBall(float duration)
    {
        //float duration = _ball.TimeFly;
        height = Random.Range(1, 5);
        width = Random.Range(-5, 5);

        localOffset.x = width;
        localOffset.y = height;
        midLocalPos = targetLocalPos + localOffset;

        seq?.Kill();
        seq = DOTween.Sequence();

        seq.Append(model.DOLocalMove(midLocalPos, duration / 2f).SetEase(Ease.OutQuad));

        seq.Append(model.DOLocalMove(targetLocalPos, duration / 2f).SetEase(Ease.InQuad));

        seq.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            handler.PlayGoalEffect(this.transform);
            transform.gameObject.SetActive(false);
            Debug.Log("VAOOOOOO!!!!");
        }
    }
}
