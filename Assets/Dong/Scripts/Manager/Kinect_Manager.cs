using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class Kinect_Manager : Code_Singleton<Kinect_Manager>
{


    public KinectSensor _Sensor;
    private BodyFrameReader _BodyReader;
    private Body[] _BodyData = null;


    [Header("UI")]
    public BodyIndexFrameReader _BodyIndexReader;
    public byte[] BodyIndexData;
    public Texture2D BodyTexture;
    public Texture2D BodyTexture1;

    private Color32[] pixelsPerson1;
    private Color32[] pixelsPerson2;

    public int Width_Dec;
    public int Height_Dec;

    private ulong[] trackedIDs = new ulong[2];

    public Body[] GetData()
    {
        return _BodyData;
    }

    void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            // Mở body tracking
            _BodyReader = _Sensor.BodyFrameSource.OpenReader();

            // Mở body index
            _BodyIndexReader = _Sensor.BodyIndexFrameSource.OpenReader();
            var desc = _Sensor.BodyIndexFrameSource.FrameDescription;
            BodyIndexData = new byte[desc.LengthInPixels];
            Width_Dec = desc.Width;
            Height_Dec = desc.Height;
            BodyTexture = new Texture2D(desc.Width, desc.Height, TextureFormat.RGBA32, false);
            BodyTexture1 = new Texture2D(desc.Width, desc.Height, TextureFormat.RGBA32, false);

            pixelsPerson1 = new Color32[desc.LengthInPixels];
            pixelsPerson2 = new Color32[desc.LengthInPixels];

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
                Debug.Log("Đã mở máy");
            }
        }
        else
        {
            Debug.LogError("Không có Kinect sensor nào");
        }
    }

    void Update()
    {
        UpdateBodyData();
        UpdateBodyIndex();
        //for(int i=0; i<_BodyData.Length; i++)
        //{
        //    if (_BodyData[i] != null && _BodyData[i].IsTracked)
        //        Debug.Log("llll :: " + i);
        //}
    }

    private void UpdateBodyData()
    {
        if (_BodyReader == null) return;

        var frame = _BodyReader.AcquireLatestFrame();
        if (frame != null)
        {
            if (_BodyData == null)
                _BodyData = new Body[_Sensor.BodyFrameSource.BodyCount];

            frame.GetAndRefreshBodyData(_BodyData);

            int count = 0;
            for (int i = 0; i < _BodyData.Length; i++)
            {
                if (_BodyData[i] != null && _BodyData[i].IsTracked)
                {
                    trackedIDs[count] = _BodyData[i].TrackingId;
                    count++;
                    if (count >= 2) break; // chỉ lấy 2 người
                }
            }

            // Nếu ít hơn 2 người, gán slot còn lại = 0
            for (int i = count; i < 2; i++)
                trackedIDs[i] = 0;

            frame.Dispose();
        }
    }

    private void UpdateBodyIndex()
    {
        if (_BodyIndexReader == null) return;

        var frame = _BodyIndexReader.AcquireLatestFrame();
        if (frame != null)
        {
            frame.CopyFrameDataToArray(BodyIndexData);

            // Clear pixels
            for (int i = 0; i < BodyIndexData.Length; i++)
            {
                pixelsPerson1[i] = Color.clear;
                pixelsPerson2[i] = Color.clear;
            }

            // Vẽ pixel cho từng người
            for (int i = 0; i < BodyIndexData.Length; i++)
            {
                byte index = BodyIndexData[i];
                if (index != 255 && _BodyData != null && _BodyData[index] != null && _BodyData[index].IsTracked)
                {
                    ulong id = _BodyData[index].TrackingId;

                    if (id == trackedIDs[0])      // Người 1
                        pixelsPerson1[i] = new Color32(255, 0, 0, 255);
                    else if (id == trackedIDs[1]) // Người 2
                        pixelsPerson2[i] = new Color32(0, 255, 0, 255);
                }
            }

            // Apply lên texture
            BodyTexture.SetPixels32(pixelsPerson1);
            BodyTexture.Apply();

            BodyTexture1.SetPixels32(pixelsPerson2);
            BodyTexture1.Apply();

            frame.Dispose();
        }
    }

    private Color32 BodyIndexToColor(byte index)
    {
        switch (index % 6)
        {
            case 0: return new Color32(255, 0, 0, 255);
            case 1: return new Color32(0, 255, 0, 255);
            case 2: return new Color32(0, 0, 255, 255);
            case 3: return new Color32(255, 255, 0, 255);
            case 4: return new Color32(255, 0, 255, 255);
            case 5: return new Color32(0, 255, 255, 255);
            default: return new Color32(0, 0, 0, 255);
        }
    }

    void OnApplicationQuit()
    {
        if (_BodyReader != null)
        {
            _BodyReader.Dispose();
            _BodyReader = null;
        }

        if (_BodyIndexReader != null)
        {
            _BodyIndexReader.Dispose();
            _BodyIndexReader = null;
        }

        if (_Sensor != null && _Sensor.IsOpen)
        {
            _Sensor.Close();
            _Sensor = null;
        }
    }
}