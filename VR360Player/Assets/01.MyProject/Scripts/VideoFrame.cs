using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFrame : MonoBehaviour
{
    VideoPlayer videoPlayer = default;

    void Start()
    {
        // 현재 오브젝트의 비디오 플레이어 컴포넌트 정보를 가지고 온다
        videoPlayer = GetComponent<VideoPlayer>();
        // 자동 재생 방지
        videoPlayer.Stop();
    }

    void Update()
    {
        // s를 누르면 정지
        if (Input.GetKeyDown(KeyCode.S))
        {
            videoPlayer.Stop();
        }

        // 스페이스 바를 누르면 재생 또는 일시정지   
        if (Input.GetKeyDown("space"))
        {
            // 현재 비디오 플레이어가 플레이 상태인지 확인
            if (videoPlayer.isPlaying)
            {
                // 플레이 중이면 일시정지
                videoPlayer.Pause();
            }       // if: 플레이중
            else
            {
                videoPlayer.Play();
            }       // else: 정지중
        }


    }
}
