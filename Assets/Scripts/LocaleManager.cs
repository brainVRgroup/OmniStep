using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocaleManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    bool changeInProgres = false;
    const string localePlayerPrefs = "locale-id";

    private void Awake()
    {
        int id = PlayerPrefs.GetInt(localePlayerPrefs, 0);
        ChangeLocale(id);
        StartCoroutine(FillDropdown());
    }

    public void ChangeLocale(int localeId)
    {
        if (changeInProgres == false)
            StartCoroutine(SetLocale(localeId));
    }

    IEnumerator SetLocale(int localeId)
    {
        changeInProgres = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
        PlayerPrefs.SetInt(localePlayerPrefs, localeId);
        changeInProgres = false;
    }

    IEnumerator FillDropdown()
    {
        if (dropdown == null)
        {
            Debug.LogWarning("No localization dropdown element assigned.");
            yield break;
        }

        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }

        dropdown.options = options;

        dropdown.value = selected;
        dropdown.onValueChanged.AddListener(ChangeLocale);
    }
}
