using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallPhysic : MonoBehaviour
{
    private ScriptHandler handler;

    public Transform startPoint;   // điểm bắt đầu
    public Vector3 endPoint;     // điểm kết thúc
    public float height = 3f;      // độ cao cực đại của quỹ đạo
    public float width = 3f;      // độ ngang cực đại của quỹ đạo
    public float duration = 2f;    // thời gian bay (tốc độ)

    private float time = 0f;
    private Vector3 lastPos;
    Vector3 moveDir;

    bool endPath;
    bool step2;
    Rigidbody rb;

    public bool canKick;

    public Transform goalTopLeft;
    public Transform goalBottomCenter;
    public Transform goalTopRight;

    void Start()
    {
        // Gán vị trí bắt đầu ban đầu
        transform.position = startPoint.position;
        lastPos = transform.position;
        rb = GetComponent<Rigidbody>();

        handler = FindObjectOfType<ScriptHandler>();
        handler?.Nextkickk();
    }

    void Update()
    {
        //if (canKick) return;
        //if (time < duration)
        //{
        //    time += Time.deltaTime;
        //    float t = Mathf.Clamp01(time / duration);

        //    // Di chuyển tuyến tính từ start -> end (trên XZ)
        //    Vector3 pos = Vector3.Lerp(startPoint.position, endPoint - new Vector3(0, 0, 1.3f), t);

        //    // Thêm độ cong trên trục Y
        //    pos.y += height * (1 - 4 * (t - 0.5f) * (t - 0.5f));
        //    pos.x += -width * (1 - 4 * (t - 0.5f) * (t - 0.5f));

        //    // Cập nhật vị trí
        //    transform.position = pos;

        //    // --- Xoay theo hướng bay ---
        //    moveDir = pos - lastPos;
        //    if (moveDir.sqrMagnitude > 0.0001f) // tránh chia 0
        //    {
        //        transform.forward = moveDir.normalized;
        //    }

        //    lastPos = pos;
        //}
        //else
        //{
        //    endPath = true;
        //}

        //if (endPath && step2 == false)
        //{
        //    endPath = false;
        //    step2 = true;
        //    rb.AddForce(Vector3.forward * 35f, ForceMode.Impulse);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    ReKick(true);
        //}
        ////if (Input.GetKeyDown(KeyCode.Space))
        ////{
        ////    handler?.Nextkickk();
        ////    canKick = true;
        ////}
    }

    public void ReKick(bool leftKick)
    {
        height = Random.Range(1, 5);
        width = Random.Range(1, 5);
        if (leftKick)
        {
            width = -width;
        }

        if (!canKick) return;
        endPoint = GetRandomGoalPoint(leftKick);
        canKick = false;
        time = 0;
        step2 = false;

        //handler.STCR();
    }

    Vector3 GetRandomGoalPoint(bool leftGoal)
    {
        float randomX=0;
        float randomY=3;
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
