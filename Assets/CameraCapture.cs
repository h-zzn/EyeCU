using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebSocketSharp;

public class CameraCapture : MonoBehaviour 
{
    private WebSocket ws;
    [SerializeField] private Camera mainCamera;
    private HashSet<string> receivedMessages = new HashSet<string>();

    private void Start()
    {
        List<string> positive_emotion = new List<string>(new string[] {
            "Concentration",
            "Anxiety",
            "Amusement",
            "Contentment"
        });

        List<string> negative_emotion = new List<string>(new string[] {
            "Boredom"
        });


        // WebSocket 연결을 Start에서 한 번만 설정
        ws = new WebSocket("ws://localhost:8764");
        ws.OnOpen += (sender, e) => Debug.Log("WebSocket connected");
        ws.Connect();

        // CaptureAndSendFrame을 코루틴으로 실행
        StartCoroutine(CaptureAndSendFrame());
        
        // 웹소켓 메시지 핸들러를 한 번만 추가
        ws.OnMessage += (sender, e) =>
        {
            int get = 0;
            if (!receivedMessages.Contains(e.Data))
            {
                receivedMessages.Add(e.Data);
                for(int i = 0; i < positive_emotion.Count; i++) {
                    if (positive_emotion.Contains(e.Data) == true) {
                        Debug.Log(e.Data);
                        break;
                    }
                }
                for(int i = 0; i < negative_emotion.Count; i++) {
                    if (negative_emotion.Contains(e.Data) == true) {
                        Debug.Log(e.Data);
                        break;
                    }
                }
            }
        };
    }

    private IEnumerator CaptureAndSendFrame()
    {
        while (true)
        {
            // 비동기로 프레임 캡처
            yield return new WaitForEndOfFrame();

            // 프레임 캡처 및 전송
            CaptureAndSend();

            receivedMessages.Clear();

            // 2초 대기 (Thread.Sleep 대신)
            yield return new WaitForSeconds(2);
        }
    }

    private void CaptureAndSend()
    {
        // 캡처 및 전송 로직
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        mainCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] imageBytes = screenShot.EncodeToJPG(40);

        ws.Send(imageBytes);
    }
}