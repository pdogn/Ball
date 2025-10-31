using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class PlayerControl : MonoBehaviour
{
    public int maxPlayer = 1;

    public Dictionary<ulong, Body> Body_with_Key = new Dictionary<ulong, Body>();

    public Dictionary<ulong, Texture2D> PLayer_Ui_with_Key = new Dictionary<ulong, Texture2D>();

    private ulong slotPlayer1 = 0;
    //private ulong slotPlayer2 = 0;

    //[SerializeField] RawImage rawImage1;
    //[SerializeField] RawImage rawImage2;

    private JointType[] allJoints;
    public GameObject jointPrefab;

    private GameObject[] jointObjects1;
    public GameObject[] JointObjects1 => jointObjects1;

    //private GameObject[] jointObjects2;
    //public GameObject[] JointObjects2 => jointObjects2;

    [SerializeField] Transform BodyJoints1;
    //[SerializeField] Transform BodyJoints2;

    [SerializeField] private float scale = 3f;          // scale hiển thị trong Unity

    ////Các bộ phận của player 1
    //public PosePath poseOfPlayer1;
    ////Các bộ phận của player 2
    //public PosePath poseOfPlayer2;

    void Start()
    {
        allJoints = (JointType[])System.Enum.GetValues(typeof(JointType));

        jointObjects1 = SpawnJoint(BodyJoints1);
        //poseOfPlayer1 = new PosePath();
        //PoseMatcher.Instance.LoadPath(poseOfPlayer1, jointObjects1);

        //jointObjects2 = SpawnJoint(BodyJoints2);
        //poseOfPlayer2 = new PosePath();
        //PoseMatcher.Instance.LoadPath(poseOfPlayer2, jointObjects2);

        //foreach (var c in poseOfPlayer1.parts["head"])
        //{
        //    Debug.Log("lkj:::::: " + c.name + "::: " + c.transform.parent);
        //}
        //foreach (var c in poseOfPlayer2.parts["head"])
        //{
        //    Debug.Log("lkj:::::: " + c.name + "::: " + c.transform.parent);
        //}
    }

    private void Update()
    {
        For_Kinect();
        //UpdateIndexFrameForTexture2D();
        UpdateBodyJoint();

    }

    private void For_Kinect()
    {
        Body[] kinect_bodies = Kinect_Manager.Instance.GetData();
        byte[] bodyIndexData = Kinect_Manager.Instance.BodyIndexData;
        HashSet<ulong> trackedIDs = new HashSet<ulong>();
        if (kinect_bodies == null || kinect_bodies.Length <= 0 || bodyIndexData == null) return;

        for (int b = 0; b < kinect_bodies.Length; b++)
        {
            Body body = kinect_bodies[b];
            if (body != null && body.IsTracked)
            {
                trackedIDs.Add(body.TrackingId);

                // --- Check xem body này đã có trong slot chưa ---
                if (body.TrackingId != slotPlayer1 /*&& body.TrackingId != slotPlayer2*/)
                {
                    if (slotPlayer1 == 0) slotPlayer1 = body.TrackingId;
                    //else if (slotPlayer2 == 0) slotPlayer2 = body.TrackingId;
                }

                // --- Dictionary quản lý ---
                if (!Body_with_Key.ContainsKey(body.TrackingId))
                {
                    if (Body_with_Key.Count < maxPlayer)
                    {
                        Body_with_Key.Add(body.TrackingId, body);
                        Texture2D maskTex = new Texture2D(Kinect_Manager.Instance.Width_Dec, Kinect_Manager.Instance.Height_Dec, TextureFormat.RGBA32, false);
                        maskTex.wrapMode = TextureWrapMode.Clamp;
                        maskTex.filterMode = FilterMode.Point;
                        PLayer_Ui_with_Key.Add(body.TrackingId, maskTex);

                        //Observer_Pattern_Manager.Instance.DKSK_Player_Add(body.TrackingId);
                        Debug.Log($"➕ Thêm: {body.TrackingId}");
                    }
                }
                else
                {
                    Body_with_Key[body.TrackingId] = body;
                }

                // --- Vẽ texture silhouette ---
                if (PLayer_Ui_with_Key.ContainsKey(body.TrackingId))
                {
                    Texture2D tex = PLayer_Ui_with_Key[body.TrackingId];
                    Color32[] pixels = new Color32[bodyIndexData.Length];

                    for (int p = 0; p < bodyIndexData.Length; p++)
                    {
                        byte bodyIndex = bodyIndexData[p];
                        if (bodyIndex != 255 && kinect_bodies[bodyIndex] != null && kinect_bodies[bodyIndex].IsTracked &&
                            kinect_bodies[bodyIndex].TrackingId == body.TrackingId)
                        {
                            pixels[p] = new Color32(255, 255, 255, 255);
                        }
                        else
                        {
                            pixels[p] = new Color32(0, 0, 0, 0);
                        }
                    }

                    tex.SetPixels32(pixels);
                    tex.Apply(false);
                }
            }
        }

        // Xóa những ID không còn tracking
        var keys = new List<ulong>(Body_with_Key.Keys);
        foreach (ulong id in keys)
        {
            if (!trackedIDs.Contains(id))
            {
                Body_with_Key.Remove(id);

                if (PLayer_Ui_with_Key.ContainsKey(id))
                {
                    //Destroy(PLayer_Ui_with_Key[id]);
                    PLayer_Ui_with_Key.Remove(id);
                }

                // Reset slot nếu player rời
                if (slotPlayer1 == id) slotPlayer1 = 0;
                //if (slotPlayer2 == id) slotPlayer2 = 0;

                Debug.Log($"❌ Xoá: {id}");
            }
        }


    }

    //private void UpdateIndexFrameForTexture2D()
    //{
    //    if (slotPlayer1 != 0 && PLayer_Ui_with_Key.ContainsKey(slotPlayer1))
    //    {
    //        rawImage1.texture = PLayer_Ui_with_Key[slotPlayer1];
    //        rawImage1.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        rawImage1.texture = null;
    //        rawImage1.gameObject.SetActive(false);
    //    }
    //    if (slotPlayer2 != 0 && PLayer_Ui_with_Key.ContainsKey(slotPlayer2))
    //    {
    //        rawImage2.texture = PLayer_Ui_with_Key[slotPlayer2];
    //        rawImage2.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        rawImage2.texture = null;
    //        rawImage2.gameObject.SetActive(false);
    //    }
    //}

    void UpdateBodyJoint()
    {
        if (slotPlayer1 != 0 && Body_with_Key.ContainsKey(slotPlayer1))
        {
            BodyJoints1.gameObject.SetActive(true);
            UpdateJointPositions(Body_with_Key[slotPlayer1], jointObjects1);
            //UpdateJointRotations(Body_with_Key[slotPlayer1], jointObjects1);
        }
        else
        {
            BodyJoints1.gameObject.SetActive(false);
        }
        //if (slotPlayer2 != 0 && Body_with_Key.ContainsKey(slotPlayer2))
        //{
        //    BodyJoints2.gameObject.SetActive(true);
        //    UpdateJointPositions(Body_with_Key[slotPlayer2], jointObjects2);
        //    //UpdateJointRotations(Body_with_Key[slotPlayer2], jointObjects2);
        //}
        //else
        //{
        //    BodyJoints2.gameObject.SetActive(false);
        //}
    }

    void UpdateJointPositions(Body body, GameObject[] joints)
    {
        for (int i = 0; i < allJoints.Length; i++)
        {
            JointType jt = allJoints[i];
            var joint = body.Joints[jt];

            if (joint.TrackingState == TrackingState.NotTracked)
                continue; // bỏ qua

            Vector3 pos = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z) * scale;
            joints[i].transform.localPosition = pos;
        }
    }
    GameObject[] SpawnJoint(Transform parent)
    {
        var arr = new GameObject[25];
        for (int i = 0; i < 25; i++)
        {
            arr[i] = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity, parent);
            arr[i].name = ((JointType)i).ToString();
        }
        parent.gameObject.SetActive(false);
        return arr;
    }

    //lấy góc xoay
    void UpdateJointRotations(Body body, GameObject[] joints)
    {
        for (int i = 0; i < allJoints.Length; i++)
        {
            JointType jt = allJoints[i];
            var joint = body.Joints[jt];

            if (joint.TrackingState == TrackingState.NotTracked)
                continue;

            // Lấy quaternion từ Kinect
            var orientation = body.JointOrientations[jt].Orientation;

            // Kinect trả về Quaternion có dạng (w, x, y, z)
            Quaternion kinectRot = new Quaternion(
                orientation.X,
                orientation.Y,
                orientation.Z,
                orientation.W
            );

            //// ⚠️ Lưu ý: hệ tọa độ Kinect khác Unity → phải đổi
            //// Kinect: X trái, Y lên, Z ra trước
            //// Unity: X phải, Y lên, Z ra trước
            //// Cách nhanh nhất: lật trục X
            //Quaternion unityRot = new Quaternion(
            //    -kinectRot.x,
            //     kinectRot.y,
            //     kinectRot.z,
            //    -kinectRot.w
            //);

            // Apply vào joint object trong Unity
            //joints[i].transform.localRotation = unityRot;
            //Debug.Log(joints[i].name);
            joints[i].transform.rotation = Quaternion.Slerp(joints[i].transform.rotation, kinectRot, 10 * Time.deltaTime);
            // Debug.Log("JJJJJJ :: " + orientation.X + "KKKK :: " + unityRot.x);
        }
    }
}
