using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
public class ScrollCircle : ScrollRect 
{
    protected float mRadius=0f;
 
    protected override void Start()
    {
        base.Start();
        //计算摇杆块的半径
        mRadius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }
 
    public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
    {
		//base.OnDrag (eventData);
		RectTransform rectTransform = transform as RectTransform;
		Vector2 center = rectTransform.anchoredPosition + new Vector2(rectTransform.rect.width/2, rectTransform.rect.height/2);

		print("center:" + center);

		if ((eventData.position - center).magnitude < mRadius) {
			SetContentAnchoredPosition((eventData.position - center));
		}
		else {
			SetContentAnchoredPosition((eventData.position - center).normalized * mRadius);
		}
		//var contentPostion = this.content.anchoredPosition;
		//if (contentPostion.magnitude > mRadius){
		//	contentPostion = contentPostion.normalized * mRadius ;
		//	SetContentAnchoredPosition(contentPostion);
		//}
    }
}