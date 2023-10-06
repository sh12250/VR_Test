using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video360Play : MonoBehaviour
{
    // 비디오 플레이어 컴포넌트
    VideoPlayer videoPlayer = default;
    // 재생해야 할 VR 360 영상을 위한 설정
    public VideoClip[] vcList = default;
    int currentVcIdx = default;

    void Start()
    {
        // 비디오 플레이어 컴포넌트의 정보를 받아온다
        videoPlayer = GetComponent<VideoPlayer>();
        currentVcIdx = 0;
        videoPlayer.clip = vcList[currentVcIdx];

        videoPlayer.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SwapVideoClip(false);

            //videoPlayer.clip = vcList[0];
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SwapVideoClip(true);

            //videoPlayer.clip = vcList[1];
        }
    }
    /**
     * 인터랙션을 위해 함수를 퍼블릭으로 선언
     * @brief 배열의 인덱스 번호를 기준으로 영상을 교체, 재생하기 위한 함수
     * @param isNext true 라면 다음 영상, false 라면 이전 영상 재생
     */
    public void SwapVideoClip(bool isNext)
    {
        /*
         * 현재 재생중인 영상의 넘버를 기준으로 체크
         * 이전 영상 번호는 현재 영상보다 배열에서 인덱스 번호가 1 작다
         * 다음 영상 번호는 현재 영상보다 배열에서 인덱스 번호가 1 크다
         */
        int setVcIdx = currentVcIdx;
        videoPlayer.Stop();

        // 재생할 영상 설정
        if (isNext)
        {
            // 배열의 다음 영상 재생
            // 리스트 전체 길이보다 크거나 같으면 리스트의 클립을 첫 번째 영상으로 지정
            setVcIdx = (setVcIdx + 1) % vcList.Length;

            //setVcIdx++;
            //if (setVcIdx >= vcList.Length)
            //{
            //    // 리스트 전체 길이보다 크거나 같으면 리스트의 클립을 첫 번째 영상으로 지정
            //    videoPlayer.clip = vcList[0];
            //}
            //else
            //{
            //    // 리스트 전체 길이보다 작으면 해당 번호의 영상 재생
            //    videoPlayer.clip = vcList[setVcIdx];
            //}
        }
        else
        {
            // 배열의 이전 영상 재생
            setVcIdx = ((setVcIdx - 1) + vcList.Length) % vcList.Length;
        }

        videoPlayer.clip = vcList[setVcIdx];
        videoPlayer.Play();
        currentVcIdx = setVcIdx;
    }
}
