using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CSV_Reader : MonoBehaviour
{
    [SerializeField] private string _fileName;

    [SerializeField] private float _seconds;
    [SerializeField] private List<Vector3> _posDatas;
    [SerializeField] private List<Vector3> _rotDatas;

    [SerializeField] private GameObject _circleObj;

    [SerializeField] private List<Vector3> _targetPosDatas;
    [SerializeField] private List<Vector3> _targetRotDatas;

    [SerializeField] private GameObject _targetObj;

    [SerializeField] private Text _distanceText;
    [SerializeField] private Text _distanceXText;
    [SerializeField] private Text _distanceYText;
    [SerializeField] private Text _distanceZText;

    private void Awake()
    {
        if (_fileName == "" || _fileName == null)
        {
            _fileName = PlayerPrefs.GetString("CSVFileName");
            Debug.Log($"讀取字串：{_fileName}");
        }

        var filePath = Path.Combine($"{Application.dataPath}/Resources/CSV Datas/", $"{_fileName}.csv");

        if (File.Exists(filePath)) StartCoroutine(ReadCSVFile(filePath));
        else Debug.LogError($"{filePath}");
    }

    private IEnumerator ReadCSVFile(string path)
    {
        _posDatas = new List<Vector3>();
        _rotDatas = new List<Vector3>();

        _targetPosDatas = new List<Vector3>();
        _targetRotDatas = new List<Vector3>();

        // 使用StreamReader打開檔案
        StreamReader reader = new StreamReader(path);

        var data = 0;

        while (!reader.EndOfStream)
        {
            // 讀取一行CSV內容
            string line = reader.ReadLine();

            if (data > 0)
            { 
                // 分割CSV內容
                string[] values = line.Split(',');

                // sensor1
                // 第2筆資料為pos x
                // 第3筆資料為pos y
                // 第4筆資料為pos z

                var targetPos = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]));
                //targetPos = new Vector3(float.Parse(values[2]), float.Parse(values[4]), float.Parse(values[3]));
                _targetPosDatas.Add(targetPos);

                // 第5筆資料為rot x
                // 第6筆資料為rot y
                // 第7筆資料為rot z

                var targetRot = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]));
                //targetRot = new Vector3(float.Parse(values[2]), float.Parse(values[4]), float.Parse(values[3]));
                _targetRotDatas.Add(targetRot);

                // sensor2
                // 第13筆資料為pos x
                // 第14筆資料為pos y
                // 第15筆資料為pos z

                // sensor4
                // 第24筆資料為pos x
                // 第25筆資料為pos y
                // 第26筆資料為pos z

                // 取平均
                var pos = new Vector3((float.Parse(values[13]) + float.Parse(values[24])) / 2, (float.Parse(values[14]) + float.Parse(values[25])) / 2, (float.Parse(values[15]) + float.Parse(values[26])) / 2);
                //pos = new Vector3((float.Parse(values[13]) + float.Parse(values[24])) / 2, (float.Parse(values[15]) + float.Parse(values[26])) / 2, (float.Parse(values[14]) + float.Parse(values[25])) / 2);
                _posDatas.Add(pos);

                Debug.LogError($"第{data}筆資料讀取中...");

                // sensor2
                // 第16筆資料為rot x
                // 第17筆資料為rot y
                // 第18筆資料為rot z

                // sensor4
                // 第27筆資料為rot x
                // 第28筆資料為rot y
                // 第29筆資料為rot z

                // 取平均
                var rot = new Vector3((float.Parse(values[16]) + float.Parse(values[27]))/2, (float.Parse(values[17]) + float.Parse(values[28])) / 2, (float.Parse(values[18]) + float.Parse(values[29])) / 2);
                //rot = new Vector3((float.Parse(values[16]) + float.Parse(values[27])) / 2, (float.Parse(values[18]) + float.Parse(values[29])) / 2, (float.Parse(values[17]) + float.Parse(values[28])) / 2);
                _rotDatas.Add(rot);
            }
            data++;
        }

        // 關閉StreamReader
        reader.Close();

        // 80Hz 資料筆數/80即為花費時間(second)
        _seconds = _posDatas.Count / 80.0f;

        yield return null;

        StartCoroutine(RunPosByDatas());
    }

    private IEnumerator RunPosByDatas()
    {
        // 設定手部初始位置及旋轉角度
        _circleObj.transform.position = _posDatas[0];
        _circleObj.transform.rotation = Quaternion.Euler(_rotDatas[0]);

        // 設定目標初始位置及旋轉角度
        _targetObj.transform.position = _targetPosDatas[0];
        _targetObj.transform.rotation = Quaternion.Euler(_targetRotDatas[0]);

        // 取得每個資料消耗時間
        var waitTime = _seconds / _posDatas.Count;

        yield return null;

        // 開始跑資料動態
        for (int i = 0;i< _posDatas.Count; i++)
        {
            Debug.LogError($"第{i + 1}筆資料演算中...");
            StartCoroutine(RunCirclePosByData(_posDatas[i], Quaternion.Euler(_rotDatas[i]), waitTime, i + 1));
            StartCoroutine(RunTargetPosByData(_targetPosDatas[i], Quaternion.Euler(_targetRotDatas[i]), waitTime));

            float elapsedTime = 0f;
            while (elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null; // 讓協程等待一個更新周期
            }
        }
    }

    private IEnumerator RunCirclePosByData(Vector3 targetPos, Quaternion targetRot, float waitTime, int id)
    {
        float elapsedTime = 0f;
        Vector3 startPos = _circleObj.transform.position;
        Quaternion startRot = _circleObj.transform.rotation;

        // 在每筆資料所需的秒數內移動
        while (elapsedTime < waitTime)
        {
            float t = elapsedTime / waitTime; // 計算插值的百分比
            _circleObj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            _circleObj.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsedTime += Time.deltaTime;
            yield return null; // 讓協程等待一個更新周期
        }

        _circleObj.transform.position = targetPos;
        _circleObj.transform.rotation = targetRot;

        Debug.LogError($"第{id}筆資料演算完成");

        if(id == 1)
        {
            // 開啟手部軌跡
            _circleObj.GetComponent<TrailRenderer>().enabled = true;
        }

    }

    private IEnumerator RunTargetPosByData(Vector3 targetPos, Quaternion targetRot, float waitTime)
    {
        float elapsedTime = 0f;
        Vector3 startPos = _targetObj.transform.position;
        Quaternion startRot = _targetObj.transform.rotation;

        // 在每筆資料所需的秒數內移動
        while (elapsedTime < waitTime)
        {
            float t = elapsedTime / waitTime; // 計算插值的百分比
            _targetObj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            _targetObj.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            elapsedTime += Time.deltaTime;
            yield return null; // 讓協程等待一個更新周期
        }

        _targetObj.transform.position = targetPos;
        _targetObj.transform.rotation = targetRot;
    }


    private void Update()
    {
        var distance = Vector3.Distance(_circleObj.transform.position, _targetObj.transform.position);
        _distanceText.text = $"{distance}";

        _distanceXText.text = $"{Mathf.Abs(_circleObj.transform.position.x - _targetObj.transform.position.x)}";
        _distanceYText.text = $"{Mathf.Abs(_circleObj.transform.position.y - _targetObj.transform.position.y)}";
        _distanceZText.text = $"{Mathf.Abs(_circleObj.transform.position.z - _targetObj.transform.position.z)}";
    }
}
