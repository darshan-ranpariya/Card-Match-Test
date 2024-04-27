using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemGraphics graphic;
    public Animator animator;
    public int index;

    public bool isFlipped;

    private void OnEnable()
    {
        name = index + "";
    }

    public void BtnClick()
    {
        //if (!ItemsHandler.inst.canClick) return;
        if (isFlipped) return;
        animator.SetTrigger("Flip");
        isFlipped = true;
        AudioPlayer.PlaySFX("Flip");
        ItemsHandler.inst.OnItemBtnClick(this);
    }

    internal void ResetItem()
    {
        //print("reset called: " + name) ;
        animator.SetTrigger("Reset");
        isFlipped = false;
    }

    internal void DestroyItem()
    {
        foreach (var item in transform.GetAllChildren())
        {
            iTween.ScaleTo(item.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easetype", iTween.EaseType.easeInElastic));
            new Delayed.Action(() =>
            {
                if(item) item.gameObject.SetActive(false);
            }, 0.7f);
        }
    }
}
