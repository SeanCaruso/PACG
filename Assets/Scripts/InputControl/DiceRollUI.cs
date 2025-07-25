using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollUI : MonoBehaviour
{
    [Header("Animation Settings")]
    public float rollDuration = 1f;
    public int rollSteps = 20;

    [Header("UI Elements")]
    public TextMeshProUGUI diceDisplayText;
    public TextMeshProUGUI resultText;
    public Button rollButton;
    public GameObject rollPanel;

    public IEnumerator AnimateRoll(DicePool dicePool)
    {
        if (!rollPanel) yield break;

        rollPanel.SetActive(true);

        if (rollButton) rollButton.interactable = false;

        // Show random numbers during the roll.
        float stepDuration = rollDuration / rollSteps;
        for (int i = 0; i < rollSteps; i++)
        {
            if (diceDisplayText)
            {
                int fakeRoll = dicePool.Roll();
                diceDisplayText.text = $"Rolling {dicePool}...\n{fakeRoll}";
            }
            yield return new WaitForSeconds(stepDuration);
        }

        // Get actual result.
        int actualResult = dicePool.Roll();

        if (resultText) resultText.text = $"Final result: {actualResult}";
        if (diceDisplayText) diceDisplayText.text = $"Rolled {dicePool}";

        yield return new WaitForSeconds(1f);

        if (rollButton) rollButton.interactable = true;

        yield return new WaitForSeconds(1f);
        rollPanel.SetActive(false);
    }
}
