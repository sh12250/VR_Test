using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! 이것은 Doxygen 이 인식하는 한 줄 주석
//! ARAVR_Input 클래스는 AR/VR 에 대응하는 Input 기능을 구현하는 클래스이다
public static class ARAVR_Input
{
    //! 다양한 기기에서 사용할 버튼의 종류를 미리 정의해둔 것이다
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // #if 는 && || 연산 가능
    public enum ButtonTarget
    {
        Fire1,      /**< 발사 버튼 1번이다. */
        Fire2,      /**< 발사 버튼 2번이다. */
        Fire3,      /**< 발사 버튼 3번이다. */
        Jump        /**< 점프 버튼이다. */
    }
#endif

    //! 미리 정의해놓은 버튼을 기기별로 다르게 매핑해둔 것이다.
    public enum Button
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE // PC BUILD 를 위한 custom 전처리기, 유니티 제공 전처리기
        One = ButtonTarget.Fire1,               /**< 발사 버튼 1 을 매핑 */
        Two = ButtonTarget.Jump,                /**< 점프 버튼을 매핑 */
        Thumbstick = ButtonTarget.Fire1,        /**< 발사 버튼 1을 매핑 */
        IndexTrigger = ButtonTarget.Fire3,      /**< 발사 버튼 3을 매핑 */
        HandTrigger = ButtonTarget.Fire2        /**< 발사 버튼 2을 매핑 */
#elif TARGET_DEVICE_OCULUS
        One = OVRInput.Button.One,                          /**< VR 컨트롤러의 One 버튼을 매핑 */
        Two = OVRInput.Button.Two,                          /**< VR 컨트롤러의 Two 버튼을 매핑 */
        Thumbstick = OVRInput.Button.PrimaryThumbstick,     /**< VR 컨트롤러의 조이스틱 버튼을 매핑 */
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger, /**< VR 컨트롤러의 IndexTrigger 버튼을 매핑 */
        HandTrigger = OVRInput.Button.PrimaryHandTrigger,   /**< VR 컨트롤러의 HandTrigger 버튼을 매핑 */
#endif
    }

    //! 기기별로 다른 컨트롤러를 미리 정의해둔 것이다
    public enum Controller
    {
#if BUILD_PLATFORM_PC || UNITY_STANDALONE 
        LTouch,     /**< 왼쪽 컨트롤러 */
        RTouch      /**< 오른쪽 컨트롤러 */
#elif TARGET_DEVICE_OCULUS
        LTouch = OVRInput.Controller.LTouch,    /**< VR 왼쪽 컨트롤러 */
        RTouch = OVRInput.Controller.RTouch,    /**< VR 오른쪽 컨트롤러 */
#endif
    }


#if BUILD_PLATFORM_PC
    private static Vector3 originalScale = Vector3.one * 0.02f;     /**< 크로스헤어 그릴 때 기존 스케일을 캐싱하는 변수 */
#else
    private static Vector3 originalScale = Vector3.one * 0.005f;    /**< 크로스헤어 그릴 때 기존 스케일을 캐싱하는 변수 */
#endif

#if TARGET_DEVICE_OCULUS
    private static Transform rootTransform;    /**< VR 에서 사용할 카메라를 기준으로 연산한 Tracking Space 의 기준이 되는 변수 */
#endif


    /**
     * @brief 오른쪽 컨트롤러의 위치를 얻어오는 프로퍼티이다
     * @return 스크린 좌표를 얻어서 월드좌표로 변환한 값을 리턴한다
     */
    public static Vector3 RHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC
            // 마우스 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7 m 로 설정
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
    }

    /**
     * @brief 오른쪽 컨트롤러의 방향을 얻어오는 프로퍼티이다
     * @return 카메라를 기준으로 컨트롤러의 정면 방향을 연산해서 리턴한다
     */
    public static Vector3 RHandDirection
    {
        get
        {
#if BUILD_PLATFORM_PC
            Vector3 direction = RHandPosition - Camera.main.transform.position;
            RHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = 
            OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif
            return direction;
        }
    }

    /**
     * @brief 왼쪽 컨트롤러의 위치를 얻어오는 프로퍼티이다
     * @return 스크린 좌표를 얻어서 월드좌표로 변환한 값을 리턴한다
     */
    public static Vector3 LHandPosition
    {
        get
        {
#if BUILD_PLATFORM_PC
            // 마우스 스크린 좌표 얻어오기
            Vector3 pos = Input.mousePosition;
            // z 값은 0.7 m 로 설정
            pos.z = 0.7f;
            // 스크린 좌표를 월드 좌표로 변환
            pos = Camera.main.ScreenToWorldPoint(pos);
            LHand.position = pos;
#elif TARGET_DEVICE_OCULUS
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos);
#else
            Vector3 pos = default;
#endif
            return pos;
        }
    }

    /**
     * @brief 왼쪽 컨트롤러의 방향을 얻어오는 프로퍼티이다
     * @return 카메라를 기준으로 컨트롤러의 정면 방향을 연산해서 리턴한다
     */
    public static Vector3 LHandDirection
    {
        get
        {
#if BUILD_PLATFORM_PC
            Vector3 direction = LHandPosition - Camera.main.transform.position;
            LHand.forward = direction;
#elif TARGET_DEVICE_OCULUS
            Vector3 direction = 
            OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
#else
            Vector3 direction = default;
#endif
            return direction;
        }
    }

    private static Transform rHand; /**< Scene 에 등록된 오른쪽 컨트롤러를 캐싱하는 변수 */
    /**
     * @brief Scene 에 등록된 오른쪽 컨트롤러를 찾아 반환하는 프로퍼티이다
     * @return 오른쪽 컨트롤러의 Transform 을 리턴한다
     */
    public static Transform RHand
    {
        get
        {
            if (lHand == null)
            {
#if BUILD_PLATFORM_PC
                // RHand 라는 이름으로 게임 오브젝틑를 만든다.
                GameObject handObj = new GameObject("RHand");
                // 만들어진 객체의 트랜스폰을 rHand 변수에 할당한다.
                rHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록한다.
                rHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                rHand = GameObject.Find("RightControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return rHand;
        }
    }

    private static Transform lHand; /**< Scene 에 등록된 왼쪽 컨트롤러를 캐싱하는 변수 */
    /**
     * @brief Scene 에 등록된 왼쪽 컨트롤러를 찾아 반환하는 프로퍼티이다
     * @return 왼쪽 컨트롤러의 Transform 을 리턴한다
     */
    public static Transform LHand
    {
        get
        {
            if (lHand == null)
            {
#if BUILD_PLATFORM_PC
                // LHand 라는 이름으로 게임 오브젝틑를 만든다.
                GameObject handObj = new GameObject("LHand");
                // 만들어진 객체의 트랜스폰을 lHand 변수에 할당한다.
                lHand = handObj.transform;
                // 컨트롤러를 카메라의 자식 객체로 등록한다.
                lHand.parent = Camera.main.transform;
#elif TARGET_DEVICE_OCULUS
                lHand = GameObject.Find("LeftControllerAnchor").transform;
#endif // BUILD_PLATFORM_PC
            }
            return lHand;
        }
    }


    /**
     * @brief 컨트롤러의 특정 버튼을 누르고 있는 동안 true 를 반환하는 함수
     * @param virtualMask 미리 정의된 버튼의 종류를 받아온다
     * @param hand 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
     */
    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#elif TARGET_DEVICE_OCULUS
        return OVRInput.Get((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#else
        return false;
#endif
    }

    /**
     * @brief 컨트롤러의 특정 버튼을 눌렀을 때 true 를 반환하는 함수
     * @param virtualMask 미리 정의된 버튼의 종류를 받아온다
     * @param hand 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
     */
    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#elif TARGET_DEVICE_OCULUS
        return OVRInput.GetDown((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#else
        return false;
#endif
    }

    /**
     * @brief 컨트롤러의 특정 버튼을 눌렀다 때었을 때 true 를 반환하는 함수
     * @param virtualMask 미리 정의된 버튼의 종류를 받아온다
     * @param hand 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 리턴한다
      */
    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if BUILD_PLATFORM_PC 
        // virtualMask 에 들어온 값을 ButtonTarget 타입으로 변환해서 전달한다
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#elif TARGET_DEVICE_OCULUS
        return OVRInput.GetUp((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#else
        return false;
#endif
    }

    /**
     * @brief 컨트롤러의 Axis 입력을 반환하는 함수
     * @param axis Horizontal, Vertical 값을 받아온다
     * @param hand 어느 컨트롤러의 버튼을 누를 것인지 받아온다
     * @return 컨트롤러의 Axis 입력을 실수 형태로 리턴한다
     */
    public static float GetAxis(string axis, Controller hand = Controller.LTouch)
    {
#if BUILD_PLATFORM_PC 
        return Input.GetAxis(axis);
#elif TARGET_DEVICE_OCULUS
        if (axis == "Horizontal")
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).x;
        }       // if: axis가 수평일 때
        else
        {
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, (OVRInput.Controller)hand).y;
        }       // else: axis가 수직일 때
#else
        return default;
#endif
    }

    /**
     * @brief VR 카메라가 바라보는 방향을 기준으로 센터를 잡는다
     * @see 
     */
    public static void Recenter()
    {
#if TARGET_DEVICE_OCULUS
        OVRManager.display.RecenterPose();
#endif
    }

    /**
     * @brief 타겟이 바라보는 방향을 기준으로 센터를 잡는다
     * @param target 바라보는 방향을 결정할 Target 의 Transform 을 받아온다
     * @param direction 바라볼 방향을 받아온다
     */
    public static void Recenter(Transform target, Vector3 direction)
    {
        target.forward = target.rotation * direction;
    }

    /**
     * @brief 레이가 닿는 곳에 크로스헤어를 위치시키는 함수
     * @param crosshair 컨트롤러의 정면에 그릴 크로스헤어 오브젝트의 Transform 을 받아온다
     * @param isHand 컨트롤러가 존재하는지 여부를 받아온다
     * @param hand 
     */
    public static void DrawCrosshair(Transform crosshair, bool isHand = true,
        Controller hand = Controller.RTouch)
    {
        Ray ray;
        // 컨트롤러의 위치와 방향을 이용해 레이를 제작한다
        if (isHand)
        {
#if BUILD_PLATFORM_PC 
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif TARGET_DEVICE_OCULUS
            if (hand == Controller.RTouch)
            {
                ray = new Ray(RHandPosition, RHandDirection);
            }       // if: 오른쪽 컨트롤러에서 레이 발사
            else
            {
                ray = new Ray(LHandPosition, LHandDirection);
            }       // else: 왼쪽 컨트롤러에서 레이 발사
#endif
        }
        else
        {
            // 카메라를 기준으로 화면의 정중앙으로 레이를 쏜다
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }

        // 눈에 안보이는 Plane 을 만든다
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;
        // Plane 을 이용해 ray 를 쏜다

        // 레이에 충돌한 지점이 있는 경우
        if (plane.Raycast(ray, out distance))
        {
            // 레이의 GetPoint 함수를 이용해서 충돌 지점의 위치를 가져온다
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;
            // 크로스헤어의 크기를 최소 기본 크기에서 거리에 따라 더 커지도록 한다
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
        // 없는 경우
        else
        {
            crosshair.position = ray.origin + ray.direction * 100.0f;
            crosshair.forward = -Camera.main.transform.forward;
            distance = (crosshair.position - ray.origin).magnitude;
            crosshair.localScale = originalScale * Mathf.Max(1.0f, distance);
        }
    }

    /**
     * @brief 컨트롤러의 진동을 실행하는 함수
     * @see PlayVibration() 
     */
    public static void PlayVibration(Controller hand)
    {
#if TARGET_DEVICE_OCULUS
        PlayVibration(0.06f, 1f, 1f, hand);
#endif
    }

    /**
     * @brief 컨트롤러의 진동을 실행하는 함수
     * @param duration 얼마나 오래 진동할 것인지 받아온다
     * @param frequency 얼마나 빠르게 진동할 것인지 받아온다
     * @param amplitude 얼마나 세게 진동할 것인지 받아온다
     * @param hand 어느쪽 컨트롤러를 진동할 것인지 받아온다
     * @see VibrationCoroutine() 
     */
    public static void PlayVibration(float duration, float frequency, float amplitude, Controller hand)
    {
#if TARGET_DEVICE_OCULUS
        if (CoroutineInstance.coroutineInstance == null)
        {
            GameObject coroutineObj = new GameObject("CoroutineInstance");
            coroutineObj.AddComponent<CoroutineInstance>();
        }       // if: 싱글턴 코루틴 인스턴스의 널 체크 위치 여기 맞음?

        // 이미 플레이중인 진동 코루틴은 정지
        CoroutineInstance.coroutineInstance.StopAllCoroutines();
        CoroutineInstance.coroutineInstance.StartCoroutine(VibrationCoroutine(duration, frequency, amplitude, hand));
#endif
    }

    /**
     * @brief TrackingSpace 의 Transform 을 리턴하는 함수
     * @return TrackingSpace 의 Transform 
     */
#if TARGET_DEVICE_OCULUS
    private static Transform GetTransform()
    {
        if (rootTransform == null)
        {
            rootTransform = GameObject.Find("TrackingSpace").transform;
        }

        return rootTransform;
    }
#endif

    /**
     * @brief 컨트롤러의 진동기능을 구현하기 위한 코루틴
     * @param duration 얼마나 오래 진동할 것인지 받아온다
     * @param frequency 얼마나 빠르게 진동할 것인지 받아온다
     * @param amplitude 얼마나 세게 진동할 것인지 받아온다
     * @param hand 어느쪽 컨트롤러를 진동할 것인지 받아온다
     */
    private static IEnumerator VibrationCoroutine(float duration, float frequency, float amplitude, Controller hand)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            OVRInput.SetControllerVibration(frequency, amplitude, (OVRInput.Controller)hand);

            yield return null;
        }       // loop: duration 만큼 반복

        // 진동 시간이 지나면 진동을 멈추는 로직
        OVRInput.SetControllerVibration(0, 0, (OVRInput.Controller)hand);
    }
}       // class ARAVR_Input

//! ARAVRInput 클래스에서 사용할 코루틴 객체
public class CoroutineInstance : MonoBehaviour
{
    public static CoroutineInstance coroutineInstance = null;   /**< ARAVRInput  기능을 사용하는 동안 메모리에 인스턴스화 할 코루틴 객체 변수  */

    //! 싱글턴 패턴으로 코루틴 객체를 생성
    private void Awake()
    {
        if (coroutineInstance == null)
        {
            coroutineInstance = this;
        }

        DontDestroyOnLoad(gameObject);
    }       // Awake()
}       // class CoroutineInstance
