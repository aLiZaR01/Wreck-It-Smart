using UnityEngine;
using UnityEditor;

public static class DevTools
{
    [MenuItem("DevTools/Сбросить монеты")]
    private static void ClearCoins()
    {
        PlayerPrefs.SetInt("WIS_TotalCoins", 0);
        PlayerPrefs.Save();
        Debug.Log("[DevTools] Монеты сброшены.");
    }

    [MenuItem("DevTools/Добавить 500 монет")]
    private static void AddCoins()
    {
        int current = PlayerPrefs.GetInt("WIS_TotalCoins", 0);
        PlayerPrefs.SetInt("WIS_TotalCoins", current + 500);
        PlayerPrefs.Save();
        Debug.Log($"[DevTools] Монеты: {current + 500}");
    }

    [MenuItem("DevTools/Сбросить всё (DeleteAll)")]
    private static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[DevTools] Все PlayerPrefs удалены.");
    }

    [MenuItem("DevTools/Показать текущие монеты")]
    private static void ShowCoins()
    {
        int coins = PlayerPrefs.GetInt("WIS_TotalCoins", 0);
        Debug.Log($"[DevTools] Текущие монеты: {coins}");
    }
}