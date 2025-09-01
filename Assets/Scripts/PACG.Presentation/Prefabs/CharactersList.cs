using PACG.Gameplay;
using PACG.SharedAPI;
using UnityEngine;
using UnityEngine.UI;

namespace PACG.Presentation
{
    public class CharactersList : MonoBehaviour
    {
        [Header("UI Elements")]
        public Transform LocalCharacters;
        public Transform DistantCharacters;

        [Header("Characters")]
        public GameObject Local1;
        public GameObject Local2;
        public GameObject Local3;
        public GameObject Local4;
        public GameObject Local5;
        public GameObject Local6;
        public GameObject Distant1;
        public GameObject Distant2;
        public GameObject Distant3;
        public GameObject Distant4;
        public GameObject Distant5;
        public GameObject Distant6;

        private void OnEnable()
        {
            GameEvents.PlayerCharacterChanged += OnPlayerCharacterChanged;
            GameEvents.PcLocationChanged += OnPcLocationChanged;
        }

        private void OnDisable()
        {
            GameEvents.PlayerCharacterChanged -= OnPlayerCharacterChanged;
            GameEvents.PcLocationChanged -= OnPcLocationChanged;
        }

        private void OnPlayerCharacterChanged(PlayerCharacter pc)
        {
            var locals = pc.LocalCharacters;
            var distant = pc.DistantCharacters;

            LocalCharacters.gameObject.SetActive(locals.Count > 0);
            DistantCharacters.gameObject.SetActive(distant.Count > 0);

            var localObjs = new[] { Local1, Local2, Local3, Local4, Local5, Local6 };
            var distantObjs = new[] { Distant1, Distant2, Distant3, Distant4, Distant5, Distant6 };
            for (var i = 0; i < localObjs.Length; i++)
            {
                var obj = localObjs[i];
                obj.SetActive(i < locals.Count);

                if (i >= locals.Count) continue;
                
                UpdatePcIcon(pc, locals[i], obj);
            }

            for (var i = 0; i < distantObjs.Length; i++)
            {
                var obj = distantObjs[i];
                obj.SetActive(i < distant.Count);

                if (i >= distant.Count) continue;
                
                UpdatePcIcon(pc, distant[i], obj);
            }
        }

        private static void UpdatePcIcon(PlayerCharacter currentPc, PlayerCharacter iconPc, GameObject iconObj)
        {

            iconObj.GetComponent<Image>().sprite = iconPc == currentPc
                ? iconPc.CharacterData.IconDisabled
                : iconPc.CharacterData.IconEnabled;

            if (iconPc == currentPc) return;

            iconObj.GetComponent<Button>().onClick.RemoveAllListeners();
            iconObj.GetComponent<Button>().onClick.AddListener(iconPc.SetActive);
        }
        
        private void OnPcLocationChanged(PlayerCharacter pc, Location location) => OnPlayerCharacterChanged(pc);
    }
}
