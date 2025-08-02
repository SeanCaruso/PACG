using UnityEngine;

namespace PACG.SharedAPI.PresentationContexts
{
    /// <summary>
    /// A data context for presentation actions involving a card that
    /// may need to be returned to its original position, such as when
    /// staging a card from preview.
    /// </summary>
    public class CardPresentationContext
    {
        // Pre-preview information
        public readonly CardZone OriginalZone;
        public readonly int OriginalSiblingIndex;

        public CardPresentationContext(CardZone originalZone, int originalSiblingIndex)
        {
            OriginalZone = originalZone;
            OriginalSiblingIndex = originalSiblingIndex;
        }
    }
}
