using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ARAVRInput
{
#if BUILD_PLATFORM_PC
    public enum ButtonTarget
    {
        Fire1, Fire2, Fire3, Jump
    }       // ButtonTarget
#endif      // BUILD_PLATFORM_PC

    public enum Button
    {
#if BUILD_PLATFORM_PC
        One = ButtonTarget.Fire1,
        Two = ButtonTarget.Jump,
        Thumbstick = ButtonTarget.Fire1,
        IndexTrigger = ButtonTarget.Fire3,
        HandTrigger = ButtonTarget.Fire2,
#elif TARGET_DEVICE_OCULUS
        One = OVRInput.Button.One,
        Two = OVRInput.Button.Two,
        Thumbstick = OVRInput.Button.PrimaryThumbstick,
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger,
        HandTrigger = OVRInput.Button.PrimaryHandTrigger,
#endif      // BUILD_PLATFORM_PC
    }       // Button

    public enum Controller
    {
#if BUILD_PLATFORM_PC
        LTouch,
        RTouch
#endif      // BUILD_PLATFORM_PC
    }       // Controller

#if BUILD_PLATFORM_PC
    private static Vector3 originScale = Vector3.one * 0.02f;
#endif      // BUILD_PLATFORM_PC

#if TARGET_DEVICE_OCULUS
    static Transform rootTransform;
#endif      // TARGET_DEVICE_OCULUS

    // 오른쪽 컨트롤러의 위치 얻어오기
    public static Vector3 RH_Position
    {
        get
        {
#if BUILD_PLATFORM_PC
            // 마우스 스크린 좌표
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7
            pos.z = 0.7f;
            // 스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            RHand.position = pos;
#elif TARGET_DEVICE_OCULUS
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            pos = GetTransform().TransformPoint(pos);
#else
            Vector3 pos = default;
#endif

            return pos;
        }
    }       // RH_Position {;}

    /**
     * @brief 오른쪽 컨트롤러의 방향을 Get하는 프로퍼티
     * @return 카메라를 기준으로 컨트롤러의 정면 방향을 대입받고 리턴
     */
    public static Vector3 RH_Direction
    {
        get
        {
#if BUILD_PLATFORM_PC
            Vector3 direction = RH_Position - Camera.main.transform.position;
            RHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif      // BUILD_PLATFORM_PC
            return direction;
        }
    }       // RH_Direction {;}

    // 왼쪽 컨트롤러의 위치 얻어오기
    public static Vector3 LH_Position
    {
        get
        {
#if BUILD_PLATFORM_PC
            // 마우스 스크린 좌표
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7
            pos.z = 0.7f;
            // 스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
#elif TARGET_DEVICE_OCULUS
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos);
#else
            Vector3 pos = default;
#endif      // BUILD_PLATFORM_PC

            return pos;
        }
    }       // LH_Position {;}
    // 왼쪽 컨트롤러의 방향 얻어오기
    public static Vector3 LH_Direction
    {
        get
        {
#if BUILD_PLATFORM_PC
            Vector3 direction = LH_Position - Camera.main.transform.position;
            LHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif      // BUILD_PLATFORM_PC
            return direction;
        }
    }       // LH_Direction {;}

    // 씬에 등록된 오른쪽 컨트롤러 찾아서 반환하기
    private static Transform rHand;
    public static Transform RHand
    {
        get
        {
            if (rHand == null)
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE
                // RHand라는 이름으로 게임 오브젝트를 만든다
                GameObject handObj = new GameObject("RHand");
                // 만들어진 객체의 트랜스폼을 rHand 변수에 할당
                rHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록
                rHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                rHand = GameObject.Find("RightControllerAnchor").transform;
#endif      //BUILD_PLATFORM_PC
            }

            return rHand;
        }
    }       // RHand {;}

    // 씬에 등록된 왼쪽 컨트롤러 찾아서 반환하기
    private static Transform lHand;
    public static Transform LHand
    {
        get
        {
            if (lHand == null)
            {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE
                // LHand라는 이름으로 게임 오브젝트를 만든다
                GameObject handObj = new GameObject("LHand");
                // 만들어진 객체의 트랜스폼을 lHand 변수에 할당
                lHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록
                lHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif      //BUILD_PLATFORM_PC
            }

            return lHand;
        }
    }       // LHand {;}

    // 컨트롤러의 특정 버튼을 누르고 있는 동안 true를 반환
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC
        // virtaualMask에 들어온 값을 ButtonTarget 타입으로 변환해서 전달
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif      // BUILD_PLATFORM_PC
    }       // Get()

    // 컨트롤러의 특정 버튼을 눌렀을 때, true를 반환
    public static bool GetDown(Button virtualMask, Controller controller = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC
        // virtaualMask에 들어온 값을 ButtonTarget 타입으로 변환해서 전달
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif      // BUILD_PLATFORM_PC
    }       // GetDown()

    // 컨트롤러의 특정 버튼을 눌렀다 떼었을 때, true를 반환
    public static bool GetUp(Button virtualMask, Controller controller = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC
        // virtaualMask에 들어온 값을 ButtonTarget 타입으로 변환해서 전달
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#else
        return false;
#endif      // BUILD_PLATFORM_PC
    }       // GetUp()

    // 컨트롤러의 Axis 입력을 반환
    // axis: Horizontal, Vertical 값을 갖는다
    public static float GetAxis(string axis, Controller hand = Controller.LTouch)
    {
#if BUILD_PLATFORM_PC
        return Input.GetAxis(axis);
#else
        return default;
#endif      // BUILD_PLATFORM_PC
    }       // GetAxis()

    // 컨트롤러에 진동 호출
    public static void PlayVibration(Controller hand)
    {
        // TODO : 진동
    }       // PlayVibration()

    // 카메라가 바라보는 방향을 기준으로 센터를 잡는다
    public static void Recenter()
    {
        // TODO : 오버로딩
    }       // Recenter()

    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
    }       // Recenter()

    // 레이가 닿는 곳에 크로스헤어를 위치시키고 싶다
    public static void DrawCrossHair(Transform crosshair, bool isHand = true, Controller hand = Controller.RTouch)
    {
        Ray ray;
        // 컨트롤러의 위치와 방향을 이용해 레이를 제작한다
        if (isHand)
        {
#if BUILD_PLATFORM_PC
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif      // BUILD_PLATFORM_PC
        }
        else
        {
            // 카메라를 기준으로 화면 정중앙에 레이 발사
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }

        // 눈에 안 보이는 Plane 생성
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;

        // plane을 이용해서 ray를 쏜다
        if (plane.Raycast(ray, out distance))
        {
            // 레이의 GetPoint 함수를 이용해 충돌 지점의 위치를 가져온다
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;
        }       // if: 레이가 충돌함
        else
        {
            // 레이의 발사 위치에서 발사 방향으로 일정거리 떨어진 위치에 크로스헤어 이동
            crosshair.position = ray.origin + ray.direction * 100;
            crosshair.forward = -Camera.main.transform.forward;
            // 크로스헤어와 레이 발사 위치 사이의 거리 대입
            distance = (crosshair.position - ray.origin).magnitude;
        }       // else: 레이가 충돌 안함

        // 크로스헤어의 크기를 최소 기본 크기에서 거리에 따라 더 커지도록 한다
        crosshair.localScale = originScale * Mathf.Max(1.0f, distance);
    }       // DrawCrossHair()

#if TARGET_DEVICE_OCULUS
    static Transform GetTransform()
    {
        if (rootTransform == null)
        {
            rootTransform = GameObject.Find("TrackingSpace").transform;
        }

        return rootTransform;
    }       // GetTransform()
#endif      // TARGET_DEVICE_OCULUS

}       // class ARAVRInput
