/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Scoring;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class HighScoreUI : MonoBehaviour
    {
        [Tooltip("The score text.")]
        [SerializeField] protected TextMeshProUGUI m_ScoreTmp;
        [Tooltip("The rank image.")]
        [SerializeField] protected Image m_RankImage;
        [Tooltip("The max chain text.")]
        [SerializeField] protected TextMeshProUGUI m_MaxChainTmp;
        [Tooltip("The accuracy UI.")]
        [SerializeField] protected AccuracyCountUI[] m_AccuracyUi;

        public virtual void SetScoreData(ScoreData scoreData)
        {
            m_ScoreTmp.text = scoreData.FullScore.ToString("n2");
            m_RankImage.sprite = scoreData.Rank?.icon;
            m_MaxChainTmp.text = scoreData.MaxChain.ToString();

            //We show all good, bad and miss accuracy
            var count = 0;
            var allAccuracyList = ScoreManager.Instance.ScoreSettings.OrderedAllAccuracyList;
            foreach (var noteAccuracy in allAccuracyList) {
                if (m_AccuracyUi.Length <= count) {
                    Debug.LogWarning("Note Accuracy will not be displayed because there are not enough 'AccuracyCountUI' gameobject in the array.", gameObject);
                    break;
                }
                m_AccuracyUi[count].gameObject.SetActive(true);
                m_AccuracyUi[count].SetAccuracyCount(noteAccuracy.icon, scoreData.NoteAccuracyIDCounts[count]);
                count++;
            }

            for (int i = count; i < m_AccuracyUi.Length; i++) {
                m_AccuracyUi[i].gameObject.SetActive(false);
            }
        }
    }
}
