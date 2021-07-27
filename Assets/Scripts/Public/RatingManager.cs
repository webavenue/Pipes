using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class RatingManager : MonoBehaviour
{
    public static RatingManager instance;
    #if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo = null;
    #endif

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        _reviewManager = new ReviewManager();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //It prepares the rating in advance, call it some seconds before actual rating request
    public void PrepareStoreRating()
    {
#if UNITY_ANDROID
       StartCoroutine(PrepareStoreReviewAndroid());
#elif UNITY_IOS
        
#endif
    }

    public void LaunchStoreRating()
    {
#if UNITY_ANDROID
        if (_playReviewInfo != null)
            StartCoroutine(LaunchStoreReviewAndroid());
#elif UNITY_IOS
        LaunchStoreReviewiOS();
#endif
    }

#if UNITY_ANDROID
    private IEnumerator PrepareStoreReviewAndroid()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log("ERROROR : " + requestFlowOperation.Error.ToString());
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
    }

    private IEnumerator LaunchStoreReviewAndroid()
    {
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using launchFlowOperation.Error.ToString().
            Debug.Log("ERROROR : " + launchFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.

    }
#elif UNITY_IOS
    private static void LaunchStoreReviewiOS()
    {
        UnityEngine.iOS.Device.RequestStoreReview();
    }
#endif

}
