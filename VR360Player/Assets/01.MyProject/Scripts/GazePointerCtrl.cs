using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GazePointerCtrl : MonoBehaviour
{
    public Video360Play video360 = default;

    public Transform uiCanvas = default;
    public Image gazeImg = default;

    Vector3 defaultScale = Vector3.one;
    public float uiScale = 1.0f;

    private bool isHitObj = false;
    private GameObject prevHitObj = default;
    private GameObject currentHitObj = default;
    float currentGazeTime = 0f;
    public float gazeChargeTime = 3.0f;


    void Start()
    {
        defaultScale = uiCanvas.localScale;
        currentGazeTime = 0f;
    }

    void Update()
    {
        // 캔버스 오브젝트의 스케일을 거리에 따라 조절
        // 1. 카메라를 기준으로 전방 방향의 좌표를 구한다
        Vector3 direction = transform.TransformPoint(Vector3.forward);

        // 2. 카메라를 기준으로 전방의 레이를 설정한다
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hitInfo = default;

        if (Physics.Raycast(ray, out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScale * hitInfo.distance;
            uiCanvas.position = transform.forward * hitInfo.distance;

            if (hitInfo.transform.tag == "GazeObj")
            {
                isHitObj = true;
            }

            currentHitObj = hitInfo.transform.gameObject;
        }       // if: 3. 레이에 부딪힌 경우에는 거리 값을 이용해서 uiCanvas의 크기를 조절
        else
        {
            uiCanvas.localScale = defaultScale * uiScale;
            uiCanvas.position = transform.position + direction;
        }       // else: 4. 아무것도 부딪히지 않으면 기본 스케일 값으로 uiCanvas의 크기를 조절

        // 5. uiCanvas가 항상 카메라 오브젝트를 바라보게 한다
        uiCanvas.forward = transform.forward * -(1.0f);

        // GazeObj 에 레이가 닿았을 때 실행
        if (isHitObj)
        {
            if (currentHitObj == prevHitObj)
            {
                // 인터렉션이 발생해야 하는 오브젝트에서 시선이 고정돼 있다면 시간을 증가시킨다
                currentGazeTime += Time.deltaTime;
            }       // if: 현재 프레임의 오브젝트가 이전 프레임의 오브젝트에서 머물러 있는 경우
            else
            {
                // 이전 프레임의 영상 정보를 업데이트한다
                prevHitObj = currentHitObj;
            }       // else: 현재 프레임의 오브젝트가 이전 프레임의 오브젝트에서 벗어난 경우

            HitObjectChecker(currentHitObj, true);
        }
        else
        {
            if (prevHitObj != null && prevHitObj != default)
            {
                HitObjectChecker(prevHitObj, false);
                prevHitObj = default;
            }

            // 시선이 벗어났거나 GazeObj가 아닌 오브젝트를 바라보는 경우
            currentGazeTime = 0f;
        }

        // 시선이 머문 시간을 0과 최댓값 사이로 한다
        currentGazeTime = Mathf.Clamp(currentGazeTime, 0f, gazeChargeTime);

        // UI Image의 fillAmount를 업데이트
        gazeImg.fillAmount = currentGazeTime / gazeChargeTime;

        // GazePointer의 게이지를 한 프레임 만큼 올린 다음에 현재 프레임에 사용된 변수들을 초기화
        isHitObj = false;
        currentHitObj = default;
    }       // Update()

    //! 히트된 오브젝트 타입별로 작동 방식을 구분한다
    private void HitObjectChecker(GameObject hitObj, bool isActive)
    {
        // hit가 비디오 플레이어 컴포넌트를 갖고 있는지 확인한다
        if (hitObj.GetComponent<VideoPlayer>())
        {
            if (isActive)
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(true);
            }
            else
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(false);
            }

            // 정해진 시간이 되면 360 스피어에 특정 클립 번호를 전달해 플레이한다
            if (currentGazeTime / gazeChargeTime >= 1.0f)
            {
                // 비디오 플레이어가 없는 Mesh Collider 오브젝트의 이름에 따라 이전/다음 영상 재생
                if (hitObj.name.Contains("Right"))
                {
                    video360.SwapVideoClip(true);
                }
                else if (hitObj.name.Contains("Left"))
                {
                    video360.SwapVideoClip(false);
                }
                else
                {
                    video360.SetVideoPlay(currentHitObj.transform.GetSiblingIndex());
                }

                currentGazeTime = 0f;
            }
            // LEGACY:
            //if (currentHitObj == null || currentHitObj == default) { return; }
            //if (gazeImg.fillAmount >= 1.0f)
            //{
            //    video360.SetVideoPlay(currentHitObj.transform.GetSiblingIndex());
            //}
        }
    }
}
