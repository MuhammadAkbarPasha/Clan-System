using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGamesCore.Matchmaker.Utils;
namespace BGGamesCore.Matchmaker
{
    public class UIRevengePopup : MonoBehaviour
    {
        private RevengeData revengeData;

        /// <summary>
        /// Area to spawn the prefabs
        /// </summary>
        [SerializeField]
        private Transform spawnTransform;
        /// <summary>
        /// Reference prefab for cards
        /// </summary>
        [SerializeField]
        private UIPlayerInfoCard cardPrefab;
        /// <summary>
        /// Pointer for info cards
        /// </summary>
        private List<UIPlayerInfoCard> infoCards;

        private Coroutine populateRoutine;
        // Start is called before the first frame update
        void Start()
        {
            infoCards = new List<UIPlayerInfoCard>();
            RevengeUtils.GetRevengeData((revengeData) =>
            {
                this.revengeData = revengeData;
                Populate();
            }, () => { Debug.LogError("Error occured when fetching revenge data!"); });
        }

        private void Populate()
        {
            populateRoutine = StartCoroutine(PopulateRoutine());

        }

        /// <summary>
        /// Remove the element when tapped
        /// </summary>
        /// <param name="playerId"></param>
        private void OnTapped(string playerId)
        {
            foreach (UIPlayerInfoCard card in infoCards)
            {
                if (card.GetPlayerId == playerId)
                {
                    infoCards.Remove(card);
                    return;
                }
            }
        }

        private IEnumerator PopulateRoutine()
        {
            yield return null;
            bool isLoading = false;
            foreach (string id in revengeData.revengeIDs)
            {
                while (isLoading)
                {
                    yield return null;
                }

                isLoading = true;
                MatchmakingSystem.Instance.GetPlayerData(id, (data) => {
                    isLoading = false;
                    if (data != null)
                    {
                        UIPlayerInfoCard card = Instantiate(cardPrefab, spawnTransform);
                        card.Initialize(new PlayerMatchmakerData(data));
                        card.AddListener(OnTapped);
                        infoCards.Add(card);
                    }
                    else
                    {
                        Debug.LogError("Player Revenge Data is null!");
                    }
                });
            }
        }
    }
}
