using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Input;
using Dypsloom.RhythmTimeline.Core.Managers;
using Dypsloom.RhythmTimeline.Core.Notes;
using TMPro;
using UnityEngine;

public class HandNoteTrigger : MonoBehaviour
{
    [SerializeField] protected RhythmProcessor rhythmProcessor;
    public TextMeshProUGUI debugText;

    private void Start()
    {
        if (rhythmProcessor != null)
        {
            Debug.Log("✅ RhythmProcessor 연결됨");
        }
        else
        {
            Debug.LogWarning("⚠️ RhythmProcessor를 찾지 못했습니다.");
        }
    }

    public void Log(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;
        }
        Debug.Log(message);
    }

    private void OnTriggerEnter(Collider other)
    {
        Log("▶️ OnTriggerEnter");

        var track = other.GetComponent<TrackObject>();
        if (track == null || rhythmProcessor == null) return;
        Log("Track On");

        var note = track.CurrentNote;
        if (note == null) return;

        Log("🎯 Note 감지됨");

        var trackID = note.RhythmClipData.TrackID;
        var inputData = new InputEventData(trackID, 0); // 0 = Tap (InputDown)
        inputData.Note = note;

        rhythmProcessor.TriggerInput(inputData);
    }

    private void OnTriggerExit(Collider other)
    {
        Log("⏹️ OnTriggerExit");

        var track = other.GetComponent<TrackObject>();
        if (track == null || rhythmProcessor == null) return;

        var note = track.CurrentNote;
        if (note == null) return;

        var trackID = note.RhythmClipData.TrackID;
        var inputData = new InputEventData(trackID, 1); // 1 = Release (InputUp)
        inputData.Note = note;

        rhythmProcessor.TriggerInput(inputData);
    }
}