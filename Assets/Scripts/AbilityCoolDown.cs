using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCoolDown : MonoBehaviour
{
    [SerializeField] float coolTime;

    private Image CoolDownRing;

    private bool startCoolDown = false;

    private void Start()
    {
        CoolDownRing = this.gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        if(startCoolDown)
        {
            CoolDownRing.fillAmount += 1.0f / coolTime * Time.deltaTime;
        }
    }

    public void StartCoolDownTime()
    {
        startCoolDown = true;
        CoolDownRing.fillAmount = 0;
    }

    public void EndCoolDownTime()
    {
        CoolDownRing.fillAmount = 1;
        startCoolDown = false;
    }
}
