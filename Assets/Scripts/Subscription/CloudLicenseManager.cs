using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Globalization;

[Serializable]
public class IpInfo
{
    public string city;
    public string region;
    public string country;
}

public class CloudLicenseManager : MonoBehaviour
{
    [Header("License Info")]
    public string licenseKey = "12345678";
    public string projectName = "ChemSim";
    public string scriptURL;

    [Header("subscription Panel")]
    public GameObject subscriptionPanel;

    private string location = "Unknown";
    private bool isValid = false;

    void Start()
    {
        StartCoroutine(DetectLocationAndCheckLicense());
    }

    IEnumerator DetectLocationAndCheckLicense()
    {
        yield return StartCoroutine(DetectLocation());
        yield return StartCoroutine(CheckLicense());
    }

    IEnumerator DetectLocation()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://ipinfo.io/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            Debug.Log("🌍 Location JSON: " + json);

            IpInfo info = JsonUtility.FromJson<IpInfo>(json);
            location = $"{info.region}, {info.country}".Trim(); // e.g., Maharashtra, IN
        }
        else
        {
            Debug.LogWarning("⚠️ Location fetch failed. Using fallback.");
            try
            {
                location = new RegionInfo(CultureInfo.CurrentCulture.Name).EnglishName;
            }
            catch
            {
                location = "Unknown";
            }
        }

        Debug.Log("📍 Auto-detected Location: " + location);
    }

    IEnumerator CheckLicense()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;

        // Escape ALL query values
        string url = string.Format(
            "{0}?key={1}&device={2}&project={3}&loc={4}",
            scriptURL,
            UnityWebRequest.EscapeURL(licenseKey),
            UnityWebRequest.EscapeURL(deviceID),
            UnityWebRequest.EscapeURL(projectName),
            UnityWebRequest.EscapeURL(location)
        );

        Debug.Log("🔍 Final License Check URL: " + url);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string result = www.downloadHandler.text;
            Debug.Log("📥 License Response: " + result);

            if (result.StartsWith("VALID"))
            {
                string expiryStr = result.Split('|')[1].Trim();

                string[] formats = {
                    "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'",
                    "ddd MMM dd yyyy HH:mm:ss 'GMT+0530 (India Standard Time)'",
                    "yyyy-MM-dd",
                    "yyyy-MM-ddTHH:mm:ss"
                };

                if (DateTime.TryParseExact(expiryStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiryDate))
                {
                    TimeSpan sinceExpirySet = DateTime.Now - expiryDate;

                    if (sinceExpirySet.TotalDays > 1)
                    {
                        if (DateTime.Now <= expiryDate)
                        {
                            Debug.Log("✅ License valid until: " + expiryDate.ToString("yyyy-MM-dd"));
                            isValid = true;
                            yield break;
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ License expired on: " + expiryDate.ToString("yyyy-MM-dd"));
                        }
                    }
                    else
                    {
                        Debug.Log("🆕 First-time device entry. License assumed valid.");
                        isValid = true;
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError("❌ Date parsing failed: " + expiryStr);
                }
            }
            else
            {
                Debug.Log("❌ Invalid License or Key not found.");
            }
        }
        else
        {
            Debug.Log("🌐 License check failed: " + www.error);
        }

        // Invalid license → block game
        Time.timeScale = 0f;
        subscriptionPanel.SetActive(true);
    }
}