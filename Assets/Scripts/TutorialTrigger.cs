using UnityEngine;
using System.Collections;

public class TutorialHealthTrigger : MonoBehaviour
{
    [Header("Main Object To Turn On Permanently")]
    [SerializeField] private GameObject permanentObject;

    [Header("Few Second Tip Panel")]
    [SerializeField] private GameObject timedTutorialPanel;

    [Header("Time Settings")]
    [SerializeField] private float Duration = 4f;

    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy to gracz i czy trigger nie był jeszcze użyty
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // Jeśli przypisałeś coś, co ma zostać na stałe (np. pasek zdrowia) – włączamy to
            if (permanentObject != null) 
            {
                permanentObject.SetActive(true);
            }

            if (timedTutorialPanel != null)
            {
                timedTutorialPanel.SetActive(true);
                StartCoroutine(DisableTutorialPanelAfterTime());
            }
        }
    }

    private IEnumerator DisableTutorialPanelAfterTime()
    {
        yield return new WaitForSeconds(Duration);

        if (timedTutorialPanel != null)
        {
            timedTutorialPanel.SetActive(false);
        }

        Destroy(gameObject);
    }
}