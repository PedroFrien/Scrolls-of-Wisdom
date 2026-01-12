using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoopArea : MonoBehaviour
{
    [SerializeField] float LoopDuration;
    [SerializeField] float fadeToWhiteStep = 2f;
    [SerializeField] float fadeFromWhiteStep = 2f;
    [SerializeField] float fadeToWhiteDuration = 1f;

    [SerializeField] private List<Collider> loopingObjects = new List<Collider>();

    private bool loopActive = true;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private float currentLoopTime = 0f;

    [SerializeField] private GameObject player;

    [SerializeField] private CanvasGroup fadeToWhiteGroup;

    private bool restarting;

    [SerializeField] Color insideColor;
    [SerializeField] Color outsideColor;

    private bool playerInZone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLoopTime = 0f;
    }

    private void Update()
    {
        if (loopActive)
        {
            currentLoopTime += Time.deltaTime;

            if (currentLoopTime >= LoopDuration)
            {
                StartCoroutine(RestartLoop());
            }


            UpdateProgressBar();
        }
    }

    private void UpdateProgressBar()
    {
        float newValue =  1 - currentLoopTime / LoopDuration;

        progressBar.ChangeValue(newValue);
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (!loopingObjects.Contains(other) && other.GetComponent<BaseLooping>() != null)
        {
            loopingObjects.Add(other);

            if (other.CompareTag("Player"))
            {
                progressBar.ChangeColor(insideColor);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (loopingObjects.Contains(other) && other.GetComponent<BaseLooping>() != null)
        {
            loopingObjects.Remove(other);

            if (other.CompareTag("Player"))
            {
                progressBar.ChangeColor(outsideColor);
            }
        }
    }


    public IEnumerator RestartLoop()
    {
        if (restarting) yield break;

        restarting = true;

        bool playerInZone = false;

        if (loopingObjects.Contains(player.GetComponent<Collider>()))
        {
            playerInZone = true;
        }

        if (playerInZone)
        {
            player.GetComponent<FPController>().MovementEnabled = false;
            StartCoroutine(FadePlayer(1));
        }

        yield return new WaitForSeconds(fadeToWhiteDuration);

        ResetObjects();
        yield return new WaitForSeconds(1);
        currentLoopTime = 0f;

        // Fade back to transparent
        player.GetComponent<FPController>().MovementEnabled = true;
        

        if (playerInZone)
        {
            StartCoroutine(FadePlayer(0));
        }
        


        
        restarting = false;
    }

    private IEnumerator FadePlayer(int targetAlpha)
    {
        float elapsed = 0f;
        float startingAlpha = fadeToWhiteGroup.alpha;

        float step = 0;
        if (targetAlpha == 1)
        {
            step = fadeToWhiteStep;
        }
        if (targetAlpha == 0)
        {
            step = fadeFromWhiteStep;
        }
        while (elapsed < fadeToWhiteDuration)
        {
            elapsed += Time.deltaTime;
            fadeToWhiteGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, elapsed / step);
            yield return null;
        }
        fadeToWhiteGroup.alpha = targetAlpha; // Ensure it's exactly 1
    }

    //private IEnumerator FadeLevel(int targetAlpha)
    //{

    //}

    public void ResetObjects()
    {
        foreach (var obj in loopingObjects)
        {
            if (obj.GetComponent<BaseLooping>() != null)
            {
                obj.GetComponent<BaseLooping>().Reset();
            }
        }
    }

    
}
