using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

using Random = UnityEngine.Random;

public class InputSystem : MonoBehaviour
{
    public static InputSystem instance;

    [Range(0, 10)] public int minKeys = 1;
    [Range(0, 10)] public int maxKeys = 10;
    public List<KeyCode> possibleKeys;

    [Space]
    public Transform buttonsToPress;
    public Transform buttonPrefab;

    [Space]
    [FMODUnity.EventRef]
    string soundCorrectButton;

    [FMODUnity.EventRef]
    string soundWrongButton;

    bool lost = false;
    bool isPlaying = false;
    Queue<KeyObject> keysToPress;
    KeyObject currentKey;

    public delegate void SuccessOrFailureDelegate();
    public event SuccessOrFailureDelegate Succes = delegate { };
    public event SuccessOrFailureDelegate Failure = delegate { };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnValidate()
    {
        if (minKeys > maxKeys)
        {
            minKeys = maxKeys;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)DateTime.Now.Ticks);
    }

    // Update is called once per frame
    void Update()
    {
        if (lost && Input.GetKeyDown(KeyCode.Return))
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isPlaying == false)
        {
            return;
        }

        if (currentKey == null && keysToPress.Count > 0)
        {
            currentKey = keysToPress.Dequeue();
            currentKey.text.gameObject.GetComponentInParent<Image>().color = Color.green;
        }
        else if (currentKey == null && keysToPress.Count == 0)
        {
            Succes.Invoke();
            isPlaying = false;
            return;
        }

        var correctKeyPressed = Input.GetKeyDown(currentKey.key);
        var wrongKeyPressed = Input.anyKeyDown && !Input.GetKeyDown(currentKey.key);

        if (correctKeyPressed)
        {
            Destroy(currentKey.trans.gameObject);
            currentKey = null;
        }
        else if (wrongKeyPressed)
        {
            StopInput();
            Failure.Invoke();
        }
    }

    public void StartInput(int difficulty)
    {
        keysToPress = new Queue<KeyObject>();

        var keyCodes = GetRandomKeyCodes(difficulty);
        foreach (var key in keyCodes)
        {
            var newButton = InsertNewButton();
            var text = newButton.GetComponentInChildren<Text>();
            text.text = key.ToString();

            keysToPress.Enqueue(new KeyObject(newButton, text, key));
        }

        isPlaying = true;
    }

    public void StopInput()
    {
        isPlaying = false;
        while (keysToPress.Count > 0)
        {
            var key = keysToPress.Dequeue();
            Destroy(key.trans.gameObject);
        }

        if (currentKey != null)
        {
            Destroy(currentKey.trans.gameObject);
            currentKey = null;
        }
    }

    List<KeyCode> GetRandomKeyCodes(int difficulty)
    {
        var keys = new List<KeyCode>();
        var numbKeys = Random.Range(minKeys, difficulty + 1);

        for (int i = 0; i < numbKeys; i++)
        {
            var keyID = Random.Range(0, possibleKeys.Count);
            var newKey = possibleKeys[keyID];
            keys.Add(newKey);
        }

        return keys;
    }

    Transform InsertNewButton()
    {
        var newButton = Instantiate(buttonPrefab);
        newButton.SetParent(buttonsToPress);
        newButton.transform.localScale = Vector3.one;
        return newButton;
    }

    public void SetLose()
    {
        lost = true;
    }

    class KeyObject
    {
        public Transform trans;
        public Text text;
        public KeyCode key;

        public KeyObject(Transform trans, Text text, KeyCode keyCode)
        {
            this.trans = trans;
            this.text = text;
            this.key = keyCode;
        }
    }
}
