using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSetter : MonoBehaviour
{
    // List to store the display names of the available locales
    private List<string> localizationKeys = new();

    // Current index for the localization keys
    private int currentIndex = 0;

    void Start()
    {
        // Populate the list of available locales' display names
        FillAvailableLocales();

        // Initialize with the first language on start
        if (localizationKeys.Count > 0)
        {
            SetLanguage(currentIndex);
        }
        else
        {
            Debug.LogWarning("No available locales found.");
        }
    }

    // Function to go to the previous language
    public void PreviousLanguage()
    {
        if (localizationKeys.Count == 0)
        {
            Debug.LogWarning("No localization keys available.");
            return;
        }

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = localizationKeys.Count - 1;
        }

        SetLanguage(currentIndex);
    }

    // Function to go to the next language
    public void NextLanguage()
    {
        if (localizationKeys.Count == 0)
        {
            Debug.LogWarning("No localization keys available.");
            return;
        }

        currentIndex++;
        if (currentIndex >= localizationKeys.Count)
        {
            currentIndex = 0;
        }

        SetLanguage(currentIndex);
    }

    // Populates the localizationKeys list with available locales' display names
    private void FillAvailableLocales()
    {
        var availableLocales = LocalizationSettings.AvailableLocales.Locales;
        for (int i = 0; i < availableLocales.Count; i++)
        {
            localizationKeys.Add(availableLocales[i].LocaleName);

            var currentLocale = LocalizationSettings.SelectedLocale;

            if (availableLocales[i] == currentLocale)
                currentIndex = i;
        }

        Debug.Log("Available locales populated.");
    }

    // Function to set the language using the current index
    private void SetLanguage(int index)
    {
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLocale;

        Debug.Log($"Language set to: {selectedLocale.LocaleName}");
    }
}
