using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.SchulteGrid
{
    class GridItem : MonoBehaviour
    {
        /// <summary>
        /// 当前grid的id
        /// </summary>
        private int m_id;
        /// <summary>
        /// 是否可以点击
        /// </summary>
        private bool m_enable = true;
        /// <summary>
        /// 等待的时间
        /// </summary>
        private float m_waitSecond = 0f;

        TextMeshProUGUI txt;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            m_waitSecond = SchulteGridControl.Instance.leftTime;
            gameObject.AddComponent<Button>().onClick.AddListener(_OnButtonClick);
        }

        /// <summary>
        /// 重置组件
        /// </summary>
        /// <param name="id">地块编号</param>
        /// <param name="vector">位置</param>
        public void ResetGameObject(int id,Vector2 vector)
        {
            gameObject.transform.localPosition = new Vector2(vector.x, vector.y);
            txt = transform.Find("number").GetComponent<TextMeshProUGUI>();
            txt.text = id + "";
            m_id = id;
        }

        public void SetSelfActive(bool active)
        {
            txt.gameObject.SetActive(active);
            m_enable = !active;
            if (!active)
            {
                gameObject.GetComponent<Image>().color = Color.white;
            }
        }

        private void _OnTimer()
        {
            if (SchulteGridControl.Instance.mode != MODE.HIGHER)
                return;

            SetSelfActive(true);
            StartCoroutine(StartTimeLoop());
        }

        //倒计时
        private IEnumerator StartTimeLoop()
        {
            while (m_waitSecond>0)
            {
                yield return new WaitForSeconds(1);
                m_waitSecond--;
                txt.text = m_id + "";
            }

            if (m_waitSecond <= 0)
            {
                m_enable = true;
                SetSelfActive(false);
            }
        }

        private void _OnButtonClick()
        {
            if (!m_enable) return;

            //如果选中过，点击无效果
            bool exist = SchulteGridControl.Instance.CheckExist(m_id);
            if (exist) return;

            bool correct = SchulteGridControl.Instance.CheckInput(m_id);
            if (!correct)
            {
                _OnTimer();
            }
            else
            {
                SetSelfActive(true);
            }
            gameObject.GetComponent<Image>().color = correct ? Color.green : Color.red;
        }

        /// <summary>
        /// 自销毁
        /// </summary>
        public void RemoveSelf()
        {
            Destroy(gameObject);
        }
    }
}
