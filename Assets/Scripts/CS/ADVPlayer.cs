using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ADVPlayer : MonoBehaviour
{
    [SerializeField] private ADVView _view;
    [SerializeField] private string _test = "";

    private string _basePath = "ADV/";
    private int _height = 0;
    private List<string[]> _csvDatas = new List<string[]>();
    private bool _loaded = false;

    private void Start()
    {
        if (_test != "")
        {
            Play(_test);
        }
    }

    public void Load(string filename)
    {
        _height = 0;
        var textAssets = Resources.Load($"{_basePath}{filename}") as TextAsset;
        var reader = new StringReader(textAssets.text);

        while(reader.Peek() > -1) {
            var line = reader.ReadLine();
            _csvDatas.Add(line.Split(','));
            _height++;
        }

        _loaded = true;
    }

    public void Play(string filename)
    {
        if (filename != "") {
            Load(filename);
        }
        else if (!_loaded) {
            return;
        }

        StartCoroutine(Player());
    }

    private IEnumerator Player()
    {
        _view.Init();
        foreach (var text in _csvDatas)
        {
            switch (text[0])
            {
                case "start":
                    Debug.Log("ADV Start");
                    _view.SetType(int.Parse(text[1]));
                    break;
                case "wait":
                    Debug.Log("Wait");
                    yield return _view.Wait(float.Parse(text[1]));
                    break;
                case "view":
                    Debug.Log("表示演出");
                    yield return _view.View(text[1], text[2], float.Parse(text[3]));
                    break;
                case "back":
                    Debug.Log("背景変更");
                    _view.Back(text[1], float.Parse(text[2]));
                    break;
                case "text":
                    Debug.Log($"{text[2]}: {text[5]}");
                    yield return _view.Text(text[1], text[2], float.Parse(text[3]), float.Parse(text[4]), text[5]);
                    break;
                default:
                    break;
            }
        }
        _view.End();
    }
}
