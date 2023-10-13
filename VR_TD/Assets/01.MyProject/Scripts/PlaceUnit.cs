using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUnit : MonoBehaviour
{
    // 배치할 유닛 프리팹
    public GameObject PlaceUnitPrefab = default;
    // 유닛 배치 UI
    public Transform placeUnitUI = default;
    // 선을 그릴 라인 렌더러
    private LineRenderer lineRenderer = default;

    // 최초 유닛 배치 UI의 크기
    private Vector3 originScale = Vector3.one * 0.2f;


    private void Awake()
    {
        // 시작할 때 비활성화
        placeUnitUI.gameObject.SetActive(false);

        // 라인 렌더러 컴포넌트 얻어오기
        lineRenderer = GetComponent<LineRenderer>();

        // 라인 렌더러 비활성화
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        // 왼쪽 컨트롤러의 One 버튼이 눌리면 
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 활성화
            lineRenderer.enabled = true;
        }
        // 왼쪽 컨트롤러의 One 버튼에서 손을 떼면
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 비활성화
            lineRenderer.enabled = false;

            if (placeUnitUI.gameObject.activeSelf)
            {
                // 유닛 배치 UI 위치에 유닛 생성
                SpawnUnit(placeUnitUI.transform);
            }

            // 유닛 배치 UI 비활성화
            placeUnitUI.gameObject.SetActive(false);
        }
        // 왼쪽 컨트롤러의 One 버튼을 누르고 있을 때
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 왼쪽 컨트롤러를 기준으로 Ray를 만든다
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo = default;
            int layer = 1 << LayerMask.NameToLayer("Terrain");

            // Terrain만 Ray 충돌 검출
            if (Physics.Raycast(ray, out hitInfo, 200f, layer))
            {
                // Ray가 부딪힌 지점에 라인 그리기
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, hitInfo.point);
                // Ray가 부딪힌 지점에 유닛 배치 UI 표시
                placeUnitUI.gameObject.SetActive(true);
                placeUnitUI.position = hitInfo.point;
                // 유닛 배치 UI의 Head가 위로 향하도록 방향 설정
                placeUnitUI.up = hitInfo.normal;
                // 유닛 배치 UI의 크기가 거리에 따라 보정되도록 설정
                placeUnitUI.localScale = originScale * Mathf.Max(1, hitInfo.distance);
            }
            else
            {
                // Ray 충돌이 발생하지 않으면 선이 Ray 방향으로 그려지도록 처리
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200f);
                // 유닛 배치 UI는 화면에서 비활성화
                placeUnitUI.gameObject.SetActive(false);
            }
        }
    }       // Update()

    private void SpawnUnit(Transform placeUnitUI)
    {
        // 유닛 소환
        Transform unit = Instantiate(PlaceUnitPrefab).transform;

        unit.position = placeUnitUI.position;
        unit.rotation = placeUnitUI.rotation;
        unit.localScale = placeUnitUI.localScale;
    }
}
