using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ImageData
{
    public string Name;
    public Image ImageReference;
    public int DivisionCount;
    public bool DivisionCompleted;
}

[System.Serializable]
public class Container
{
    public Transform ContainerTransform;
    public List<ImageData> m_ImageData = new List<ImageData>();
    public bool ContainerCompleted;
}

public class UIHandler : MonoBehaviour
{
    public Container m_LeftContainer = new Container();
    public Container m_RightContainer = new Container();
    public Container m_TopContainer = new Container();
    public Container m_BottomContainer = new Container();
    public GameObject flyingIMG;
    public Sprite[] DivisionSprites;

    public float speed;
    bool CanMove = false;
    int Counter = -1;
    int completionCounter = 0;
    Vector3 TargetPos = new Vector3(0, 0, 0);
    private void Start()
    {
        PopulateData(m_LeftContainer.m_ImageData, m_LeftContainer.ContainerTransform, "LeftVector");
        PopulateData(m_RightContainer.m_ImageData, m_RightContainer.ContainerTransform, "RightVector");
        PopulateData(m_TopContainer.m_ImageData, m_TopContainer.ContainerTransform, "TopVector");
        PopulateData(m_BottomContainer.m_ImageData, m_BottomContainer.ContainerTransform, "BottomVector");


        flyingIMG.transform.localPosition = new Vector3(-1000, -1000, 0);
        Counter = -1;
        CanMove = false;
    }

    public void PopulateData(List<ImageData> _refList,Transform _container,string _name)
    {
        _refList.Clear();
        ImageData _data;
        for (int i = 0; i < _container.childCount; i++)
        {
            _data = new ImageData();
            _data.ImageReference = _container.GetChild(i).GetComponent<Image>();
            _data.DivisionCount = 0;
            _data.Name = _name + (i + 1);
            _data.DivisionCompleted = false;
            _refList.Add(_data);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !CanMove)
        {
            RandomSide();
        }

        if (CanMove)
            MoveObject();
    }

    public void RandomSide()
    {
        if(completionCounter>=4)
        {
            Debug.Log("All completeedddd");
            return;
        }
        int _randomPos = Random.Range(0, 4);

        switch (_randomPos)
        {
            case 0:
                if (m_LeftContainer.ContainerCompleted == false)
                    StartCoroutine(RandomDivison(m_LeftContainer, m_LeftContainer.m_ImageData));
                else
                    RandomSide();
                break;
            case 1:
                if (m_RightContainer.ContainerCompleted == false)
                    StartCoroutine(RandomDivison(m_RightContainer,m_RightContainer.m_ImageData));
                else
                    RandomSide();
                break;
            case 2:
                if (m_TopContainer.ContainerCompleted == false)
                    StartCoroutine(RandomDivison(m_TopContainer,m_TopContainer.m_ImageData));
                else
                    RandomSide();
                break;
            case 3:
                if (m_BottomContainer.ContainerCompleted == false)
                    StartCoroutine(RandomDivison(m_BottomContainer,m_BottomContainer.m_ImageData));
                else
                    RandomSide();
                break;
            default:
                break;
        }
    }

    public IEnumerator RandomDivison(Container m_container, List<ImageData> _refList)
    {
        int _randomTriangle = Random.Range(0, _refList.Count);

        while(_refList[_randomTriangle].DivisionCompleted)
        {
            _randomTriangle = Random.Range(0, _refList.Count);
        }

        flyingIMG.gameObject.transform.SetParent(_refList[_randomTriangle].ImageReference.transform);
        flyingIMG.transform.localPosition = new Vector3(-1200, -1200, 0);
        flyingIMG.transform.localEulerAngles = new Vector3(0, 0, 0);
        flyingIMG.transform.localScale = new Vector3(2, 2, 2);
        //TargetPos = _refList[_randomTriangle].ImageReference.transform.position;
        CanMove = true;

        yield return new WaitUntil(() => CanMove == false);
        _refList[_randomTriangle].ImageReference.sprite = DivisionSprites[_refList[_randomTriangle].DivisionCount];

        if (_refList[_randomTriangle].DivisionCount < DivisionSprites.Length - 1)
        {
            _refList[_randomTriangle].DivisionCount++;
        }
        else
        {
            _refList[_randomTriangle].DivisionCompleted = true;

            bool allCompleted = true;
            for (int i = 0; i < _refList.Count; i++)
            {
                if(_refList[i].DivisionCompleted==false)
                {
                    allCompleted = false;
                    break;
                }
            }

            m_container.ContainerCompleted = allCompleted;
            if (allCompleted)
            {
                Debug.Log("completed division for: " + m_container.ContainerTransform.name);
                completionCounter++;
            }
        }
    }

    public void ChangeImage()
    {
        //RefImage.sprite = DivisionSprites[Counter];
    }

    public void MoveObject()
    {
        flyingIMG.transform.localPosition = Vector3.MoveTowards(flyingIMG.transform.localPosition, TargetPos, Time.deltaTime * speed);
        if(flyingIMG.transform.localPosition.x >= TargetPos.x)
        {
            CanMove = false;
            Invoke(nameof(resetObject), 0.1f);
        }

    }

    public void resetObject()
    {
        flyingIMG.transform.localPosition = new Vector3(-1000, -1000, 0);
        //if (Counter < DivisionSprites.Length - 1)
            //Counter++;

        //ChangeImage();
    }

}
