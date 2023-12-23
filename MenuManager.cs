using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _openDirBtn;
    [SerializeField] private Button _readCSVFileBtn;
    [SerializeField] private InputField _CSVFileName;
    [SerializeField] private Text _hintText;

    private void Awake()
    {
        CheckDir();
    }

    private void CheckDir()
    {
        var path = $"{Application.dataPath}/Resources/CSV Datas";

        if (!Directory.Exists(path))
        {
            CreateDir(path);
        }

        RegisterBtns();
    }

    private void CreateDir(string path)
    {
        Directory.CreateDirectory(path);
    }

    private void RegisterBtns()
    {
        _openDirBtn.onClick.AddListener(() => OpenDir());
        _readCSVFileBtn.onClick.AddListener(() => ReadCSVFile());
    }

    private void OpenDir()
    {
        var path = $"{Application.dataPath}/Resources/CSV Datas";
        Application.OpenURL(path);

        _hintText.text = $"";
    }

    private void ReadCSVFile()
    {
        var fileName = $"{_CSVFileName.text}";

        var filePath = Path.Combine($"{Application.dataPath}/Resources/CSV Datas/", $"{fileName}.csv");

        _hintText.text = $"";

        if (File.Exists(filePath))
        {
            PlayerPrefs.SetString("CSVFileName", fileName);
            Debug.Log($"儲存字串：{fileName}");
            SceneManager.LoadScene("main");
            Debug.Log($"載入場景");
        }
        else
        {
            _hintText.text = $"查無檔案{filePath}";
            _CSVFileName.text = "";
        }
    }

    private void Update()
    {
        if (_CSVFileName.text != "") _readCSVFileBtn.gameObject.SetActive(true);
        else _readCSVFileBtn.gameObject.SetActive(false);
    }
}
