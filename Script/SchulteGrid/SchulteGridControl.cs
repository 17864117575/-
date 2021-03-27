using Assets.Script.SchulteGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public enum MODE
{
    NORMOL = 0,//普通
    HIGHER = 1 //高阶
}

public enum GIRD
{
    GIRD3X3 = 3, //格子 3*3
    GIRD4X4 = 4, //格子 4*4
    GIRD5X5 = 5  //格子 5*5
}

public class SchulteGridControl : Singleton<SchulteGridControl>
{
    //默认格子数量
    public GIRD gird = GIRD.GIRD3X3;
    //模式
    public MODE mode = MODE.NORMOL;
    //预制体
    public GameObject go;
    //倒计时
    public int leftTime = 15;
    private int timeCount;

    private GameObject[,] gridItems;
    private int chooseNum;
    private List<int> randomRange;

    private List<GridItem> pool;

    //选中的数字集合
    private static List<int> chooseNumList;

    private Transform parent;
    private TextMeshProUGUI text;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //获取控件
        parent = GameObject.Find("Canvas/Panel").transform;
        Button btnReset = GameObject.Find("Canvas/ButtonReset").GetComponent<Button>();
        btnReset.onClick.AddListener(_ResetGrids);
        Button btnStart = GameObject.Find("Canvas/ButtonStart").GetComponent<Button>();
        btnStart.onClick.AddListener(_OnBtnStartClick);
        text = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();

        chooseNum = Convert.ToInt32(gird);
        chooseNumList = new List<int>();
        pool = new List<GridItem>();
        timeCount = leftTime;
        //DrawGrid();
    }

    /// <summary>
    /// 绘制格子，核心逻辑，随机生成格子
    /// </summary>
    private void DrawGrid()
    {
        gridItems = new GameObject[chooseNum,chooseNum];

        randomRange = new List<int>();
        for (int k = 1; k <= chooseNum * chooseNum; k++)
        {
            randomRange.Add(k);
        }

        Random r = new Random();

        int space = 30;
        for (int i = 0; i < chooseNum; i++)
        {
            for (int j = 0; j < chooseNum; j++)
            {
                //随机下标
                int num = r.Next(0, randomRange.Count);
                GameObject newGo = Instantiate(go, parent);
                gridItems[i, j] = newGo;

                float x = (go.GetComponent<RectTransform>().rect.width + space) * j;
                float y = (go.GetComponent<RectTransform>().rect.height + space) * i;
                GridItem gridItem = newGo.GetComponent<GridItem>();
                gridItem.ResetGameObject(randomRange[num], new Vector2(x, y));

                pool.Add(gridItem);

                randomRange.Remove(randomRange[num]);
            }
        }

        if (randomRange.Count > 0)
            return;

        Console.WriteLine("Draw Complete！！！");
    }

    private void _OnBtnStartClick()
    {
        _ResetGrids();

        if(mode == MODE.HIGHER)
        {
            text.gameObject.SetActive(true);
            StartCoroutine(_onTimer());
        }
    }

    private IEnumerator _onTimer()
    {
        while (timeCount > 0)
        {
            text.text = timeCount + "";
            yield return new WaitForSeconds(1);
            timeCount--;
        }

        if (timeCount <= 0)
        {
            HideNumbers();
            text.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 重置数据，其实可以不用清理预制体，直接重绘Text组件即可，这里偷懒了：）
    /// </summary>
    private void _ResetGrids()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            //Transform child = parent.GetChild(i);
            GridItem gridItem = pool[i];
            gridItem.RemoveSelf();
        }
        pool.Clear();
        DrawGrid();
    }

    private void HideNumbers()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            GridItem go = pool[i];
            go.SetSelfActive(false);
        }
    }

    public bool CheckInput(int p_num)
    {
        if(chooseNumList.Count + 1 == p_num)
        {
            chooseNumList.Add(p_num);
            return true;
        }
        return false;
    }

    public  bool CheckExist(int p_num)
    {
        if (chooseNumList.IndexOf(p_num) != -1)
        {
            return true;
        }
        return false;
    }

    private void Dispose()
    {
    }
}