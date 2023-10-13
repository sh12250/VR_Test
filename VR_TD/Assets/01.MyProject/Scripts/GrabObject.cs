using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // 물체를 잡고 있는지 여부
    bool isGrabbing = false;
    // 잡고 있는 물체
    private GameObject grabbedObject = default;
    // 잡을 물체의 종류
    public LayerMask grabbedLayer = default;
    // 잡을 수 있는 거리
    public float grabRange = 0.2f;

    // { 물체를 던지기 위한 변수
    // 이전 위치
    private Vector3 prevPos = default;
    // 던지는 힘
    private float throwPower = 10f;

    // 이전 회전
    Quaternion prevRot = default;
    // 회전력
    public float rotPower = 5f;

    // 원거리에서 물체를 잡는 기능 활성화
    public bool isRemoteGrab = true;
    // 원거리에서 물체를 잡을 수 있는 거리
    public float remoteGrabDistance = 20f;
    // } 물체를 던지기 위한 변수


    private void Update()
    {
        // 물체 잡기
        // 물체를 잡고 있지 않을 경우
        if (isGrabbing == false)
        {
            // 잡기 시도
            TryGrab();
        }
        else
        {
            // 물체 놓기
            TryUngrab();
        }
    }

    //! Grab 버튼을 누르면 일정 영역 안에 폭탄을 잡는 함수
    private void TryGrab()
    {
        // Grab 버튼을 눌렀다면
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 원거리 물체 잡기를 사용한다면
            if (isRemoteGrab)
            {
                // 손 방향으로 Ray 발사
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo = default;
                // SphereCast를 이용해서 물체 충돌을 체크한다
                if (Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDistance, grabbedLayer))
                {
                    // 잡은 상태로 전환
                    isGrabbing = true;
                    // 잡은 물체로 캐싱
                    grabbedObject = hitInfo.transform.gameObject;
                    // 물체가 끌려오는 기능 실행
                    StartCoroutine(GrabbingAnimation());
                }

            }

            // 영역 안에 있는 모든 폭탄 검출
            Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);

            // 가장 가까운 폭탄 인덱스
            int closest = 0;
            // 손과 가장 가까운 물체 선택
            for (int i = 0; i < hitObjects.Length; i++)
            {
                // 손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
                // 다음 물체와 손의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
                // 다음 물체와의 거리가 더 가깝다면
                if (nextDistance < closestDistance)
                {
                    // 가장 까가운 물체 인덱스 교체
                    closest = i;
                }
            }       // loop: 가장 가까운 물체를 찾는 루프

            // 검출된 물체가 있을 경우
            if (0 < hitObjects.Length)
            {
                // 잡은 상태로 전환
                isGrabbing = true;
                // 잡은 물체를 캐싱
                grabbedObject = hitObjects[closest].gameObject;
                // 잡은 물체를 손의 자식으로 등록
                grabbedObject.transform.SetParent(ARAVRInput.RHand, true);

                // 물리 기능 정지
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

                // 초기 위치 값 지정
                prevPos = ARAVRInput.RHandPosition;
                // 초기 회전 값 지정
                prevRot = ARAVRInput.RHand.rotation;
            }
        }
    }       // TryGrab()

    //! 물체를 내려 놓는 함수
    private void TryUngrab()
    {
        // 던지는 방향
        Vector3 throwDirection = (ARAVRInput.RHandPosition - prevPos);
        // 이전 위치 갱신
        prevPos = ARAVRInput.RHandPosition;

        /* 쿼터니온 공식
         * angle1 = Q1, angle2 = Q2
         * angle1 + angle2 = Q1 * Q2
         * -angle2 = Quaternion.Inverse(Q2)
         * angle2 - angle1 =Quaternion.FromToRotation(Q1, Q2) = Q2 * Quaternion.Inverse(Q1)
         */

        // 회전 방향 = current - previous의 차이로 구함. -previous는 inverse로 구함
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        // 이전 회전 저장
        prevRot = ARAVRInput.RHand.rotation;

        // 버튼을 놓았다면
        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 잡지 않은 상태로 전환
            isGrabbing = false;
            // 물리 기능 활성화
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            // 손에서 폭탄 떼어내기
            grabbedObject.transform.SetParent(default, true);
            // 던지기
            grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;

            // 각속도 = (1/dt) * dθ(특정 축 기준 변위 각도)
            float angle = default;
            Vector3 axis = default;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            // 잡은 물체가 없도록 설정
            grabbedObject = default;
        }
    }       // TryUngrab()

    private IEnumerator GrabbingAnimation()
    {
        // 물리 기능 정지 
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        // 초기 위치 값 지정
        prevPos = ARAVRInput.RHandPosition;
        // 초기 회전 값 지정
        prevRot = ARAVRInput.RHand.rotation;
        Vector3 startLocation = grabbedObject.transform.position;
        Vector3 targetLocation = ARAVRInput.RHandPosition + (ARAVRInput.RHandDirection * 0.1f);

        float currentTime = 0f;
        float finishTime = 0.2f;
        // 경과율
        float elapsedRate = currentTime / finishTime;
        while (elapsedRate < 1f)
        {
            currentTime += Time.deltaTime;
            elapsedRate = currentTime / finishTime;
            grabbedObject.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);

            yield return null;
        }

        // 잡은 물체를 손의 자식으로 등록
        grabbedObject.transform.position = targetLocation;
        grabbedObject.transform.SetParent(ARAVRInput.RHand);
    }
}
