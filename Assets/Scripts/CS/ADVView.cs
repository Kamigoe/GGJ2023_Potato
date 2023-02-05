using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ADVView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _textBox1;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text1;
    [Space(10)]
    [SerializeField] private CanvasGroup _textBox2;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _text2;
    [SerializeField] private GameObject _waitIcon;
    [Space(10)]
    [SerializeField] private Image _screen;
    [SerializeField] private Image _back;
    [SerializeField] private RectTransform _charParent;

    private CanvasGroup _view = default;
    private int _type = 1;
    private Dictionary<string, Image> _chars;
    private bool _waitHitKey = false;
    private bool _hitKey = false;

    private void Awake()
    {
        _view = this.GetComponent<CanvasGroup>();
        _chars = new Dictionary<string, Image>();
    }

    private void Update()
    {
        if (_waitHitKey && Input.GetKeyDown(KeyCode.Return))
        {
            _hitKey = true;
        }
        else if (!_waitHitKey)
        {
            _hitKey = false;
        }
    }

    public void Init()
    {
        _waitHitKey = false;
        _hitKey = false;
        _textBox1.alpha = 0;
        _textBox2.alpha = 0;
        _screen.color = new Color(0, 0, 0, 0);
        _back.color = new Color(0, 0, 0, 0);
        _view.alpha = 1;
    }

    public void SetType(int type)
    {
        Init();
        _type = type;
        if (type == 1)
        {
            _text1.text = "";
            _textBox1.alpha = 1;
        }
        else
        {
            _text2.text = "";
            _name.text = "";
            _waitIcon.SetActive(false);
            _textBox2.alpha = 2;
        }
    }

    public IEnumerator Wait(float dulation)
    {
        yield return new WaitForSeconds(dulation);
    }

    public IEnumerator View(string color, string type, float dulation)
    {
        var col = color == "black" ? 0f: 1f;
        var start = type == "fadein" ? 1f : 0f;
        var end = 1 - start;

        if (dulation > 0)
        {
            var deltaTime = 0f;

            while (deltaTime < dulation)
            {
                var alpha = start - (start - end) * deltaTime / dulation;
                _screen.color = new Color(col, col, col, alpha);
                deltaTime += Time.deltaTime;
                yield return null;
            }
        }

        _screen.color = new Color(col, col, col, end);
    }

    public void Back(string color, float alpha)
    {
        var col = color == "black" ? 0f : 1f;
        alpha = Mathf.Clamp(0, 1, alpha);
        _back.color = new Color(col, col, col, alpha);
    }

    public IEnumerator Text(string path, string name, float x, float y, string text)
    {
        TextMeshProUGUI tex;
        if (_type == 1)
        {
            tex = _text1;
        }
        else
        {
            tex = _text2;
            _name.text = name;

            Image image;
            if (!_chars.ContainsKey(name))
            {
                var obj = new GameObject(name);
                obj.transform.SetParent(_charParent);
                image = obj.AddComponent<Image>();
                _chars.Add(name, image);
            }
            else
            {
                image = _chars[name];
            }
            image.sprite = Resources.Load<Sprite>($"ADV/Textures/{path}");
            var rect = (RectTransform)image.transform;
            rect.localPosition = new Vector2(x, y);

            var seq = DOTween.Sequence();

            seq.Append(rect.DOScale(Vector3.one * 1.1f, 0.15f).SetEase(Ease.Linear))
            .Append(rect.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear));
        }

        if (text != "")
        {
            _waitHitKey = true;
            yield return DOText(tex, text);
            _waitIcon.SetActive(true);
            yield return WaitHitKey();
            _waitHitKey = false;
            _waitIcon.SetActive(false);
        }
    }

    public void End()
    {
        _view.alpha = 0;
        _textBox1.alpha = 0;
        _textBox2.alpha = 0;
        _textBox2.alpha = 0;
        _screen.color = new Color(0, 0, 0, 0);
        _back.color = new Color(0, 0, 0, 0);

        foreach (var (id, img) in _chars)
        {
            Destroy(img.gameObject);
        }
        _chars.Clear();
    }

    private IEnumerator DOText(TextMeshProUGUI tm, string text)
    {
        tm.text = "";
        var deltaTime = 0f;
        var length = text.Length;
        var dulation = (float)length / 20f;
        var nowLength = 0;

        while (deltaTime < dulation)
        {
            if (_hitKey) break;

            deltaTime += Time.deltaTime;
            var len = (int)(length * deltaTime / dulation);
            if (len > nowLength)
            {
                tm.text += text.Substring(nowLength, len - nowLength);
                nowLength = len;
            }
            yield return null;
        }
        _hitKey = false;
        tm.text = text;
    }

    private IEnumerator WaitHitKey()
    {
        _waitHitKey = true;
        while (true)
        {
            if (_hitKey) break;
            yield return null;
        }
        _hitKey = false;
        _waitHitKey = false;
    }
}
