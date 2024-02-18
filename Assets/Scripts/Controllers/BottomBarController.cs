using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI personNameText;
    public Image backgroundImage; // Reference to the background image component

    private int sentenceIndex = -1;
    public List<StoryScene> scenes; // Assuming you have a list of scenes
    private StoryScene currentScene;
    private State state = State.COMPLETED;
    private bool isHidden = false;

    private Coroutine typingCoroutine;
    private float speedFactor = 1f;

    private enum State
    {
        PLAYING, SPEEDED_UP, COMPLETED
    }

    private void Start()
    {
        if (scenes.Count > 0)
            PlayScene(scenes[0]); // Start with the first scene
    }


    private void Update()
    {
        // Wait for spacebar or left mouse button click to proceed to the next sentence or scene
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (state == State.COMPLETED)
            {
                if (IsLastSentence())
                {
                    if (currentScene.nextScene != null)
                    {
                        PlayNextScene();
                    }
                    else
                    {
                        if (IsLastScene())
                        {
                            // If it's the last scene and last sentence, do whatever you need to do.
                            Debug.Log("Last sentence of the last scene.");
                            return;
                        }
                        else
                        {
                            // Proceed to the next scene if there's no next scene defined
                            PlayNextScene();
                        }
                    }
                }
                else
                {
                    PlayNextSentence();
                }
            }
            else if (state == State.PLAYING)
            {
                // If text is still typing, speed up the typing
                SpeedUp();
            }
        }
    }

    public void PlayScene(StoryScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        currentScene = scene;
        this.sentenceIndex = sentenceIndex;
        UpdateBackground(); // Update the background image when a new scene is played
        ClearText();
        PlayNextSentence();
    }

    private void UpdateBackground()
    {
        if (backgroundImage != null && currentScene.background != null)
        {
            backgroundImage.sprite = currentScene.background;
        }
    }

    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    public bool IsLastScene()
    {
        return scenes.IndexOf(currentScene) == scenes.Count - 1;
    }

    public void PlayNextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        sentenceIndex++;
        if (sentenceIndex < currentScene.sentences.Count)
        {
            PlaySentence();
        }
    }

    private void PlaySentence()
    {
        StoryScene.Sentence sentence = currentScene.sentences[sentenceIndex];
        speedFactor = 1f;
        personNameText.text = sentence.speaker.speakerName;
        personNameText.color = sentence.speaker.textColor;
        typingCoroutine = StartCoroutine(TypeText(sentence.text));
    }

    private IEnumerator TypeText(string text)
    {
        state = State.PLAYING;
        barText.text = ""; // Clear the text before typing the new sentence
        int wordIndex = 0;

        while (wordIndex < text.Length)
        {
            barText.text += text[wordIndex];
            yield return new WaitForSeconds(speedFactor * 0.05f);
            wordIndex++;
        }

        state = State.COMPLETED;
    }

    public void ClearText()
    {
        barText.text = "";
        personNameText.text = "";
    }

    public void SpeedUp()
    {
        speedFactor = 0.5f; // Increase the speed factor when speeding up
    }

    public void PlayNextScene()
    {
        if (currentScene.nextScene != null && currentScene.nextScene is StoryScene)
        {
            PlayScene((StoryScene)currentScene.nextScene);
        }
        else
        {
            int nextSceneIndex = scenes.IndexOf(currentScene) + 1;
            if (nextSceneIndex < scenes.Count)
            {
                PlayScene(scenes[nextSceneIndex]);
            }
            else
            {
                Debug.LogWarning("No more scenes available.");
            }
        }
    }
}
