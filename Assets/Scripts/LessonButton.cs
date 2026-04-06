using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LessonButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public LessonData lessonData;

    public void OnClick()
    {
        print("idk");

        ProgressManager.Instance.lessons.Clear();
        ProgressManager.Instance.lessons.Add(lessonData);

        SceneManager.LoadScene("LessonScene");
    }

        public void OnPointerDown(PointerEventData e) => Debug.Log("DOWN");
    public void OnPointerUp(PointerEventData e) => Debug.Log("UP");
    public void OnPointerClick(PointerEventData e) => Debug.Log("CLICK");
}
