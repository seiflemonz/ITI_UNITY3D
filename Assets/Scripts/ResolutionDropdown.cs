using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private List<Resolution> resolutions = new List<Resolution>();
    private const string PREF_KEY = "ResolutionIndex";

    private int appliedIndex = 0;

    // ---------------- AWAKE ----------------
    void Awake()
    {
        BuildResolutionList();

        appliedIndex = PlayerPrefs.GetInt(PREF_KEY, 0);
        appliedIndex = Mathf.Clamp(appliedIndex, 0, resolutions.Count - 1);

        ApplyResolutionInternal(appliedIndex);
    }

    // ---------------- START ----------------
    void Start()
    {
        dropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (Resolution res in resolutions)
        {
            options.Add($"{res.width} x {res.height}");
        }

        dropdown.AddOptions(options);
        dropdown.value = appliedIndex;
        dropdown.RefreshShownValue();

        // 🔥 IMPORTANT: Apply instantly when changed
        dropdown.onValueChanged.AddListener(ApplyResolution);
    }

    // ---------------- BUILD LIST ----------------
    void BuildResolutionList()
    {
        Resolution[] allRes = Screen.resolutions;
        HashSet<string> seen = new HashSet<string>();

        Resolution? res1080p = null;

        foreach (Resolution r in allRes)
        {
            string key = $"{r.width}x{r.height}";
            if (seen.Contains(key)) continue;

            seen.Add(key);

            if (r.width == 1920 && r.height == 1080)
                res1080p = r;
            else
                resolutions.Add(r);
        }

        // 🔥 Force 1920x1080 as FIRST option
        if (res1080p.HasValue)
            resolutions.Insert(0, res1080p.Value);
    }

    // ---------------- DROPDOWN ----------------
    public void ApplyResolution(int index)
    {
        ApplyResolutionInternal(index);

        PlayerPrefs.SetInt(PREF_KEY, index);
        PlayerPrefs.Save();
    }

    // ---------------- INTERNAL ----------------
    void ApplyResolutionInternal(int index)
    {
        Resolution res = resolutions[index];

#if UNITY_EDITOR
        Screen.SetResolution(res.width, res.height, false);
#else
        Screen.SetResolution(res.width, res.height, true);
#endif
    }
}
