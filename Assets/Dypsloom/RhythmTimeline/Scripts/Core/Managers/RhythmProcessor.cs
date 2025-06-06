﻿/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Managers
{
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Notes;
    using Dypsloom.RhythmTimeline.Core.Playables;
    using Dypsloom.Shared.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Gets information from the RhythmDirector and from the input to processes notes.
    /// </summary>
    public class RhythmProcessor : MonoBehaviour
    {
        public event Action<InputEventData> OnTriggerInputEvent;
        public event Action<NoteTriggerEventData> OnNoteTriggerEvent;
        public event Action<Note> OnNoteInitializeEvent;
        public event Action<Note> OnNoteResetEvent;
        public event Action<Note> OnNoteActivateEvent;
        public event Action<Note> OnNoteDeactivateEvent;
        
        [Tooltip("The Rhythm Director.")]
        [SerializeField] protected RhythmDirector m_RhythmDirector;

        protected List<Note> m_AllPooledNotes = new List<Note>();

        public RhythmDirector RhythmDirector => m_RhythmDirector;
        public IReadOnlyList<Note> AllPooledNotes => m_AllPooledNotes;
        
        private void Awake()
        {
            if (m_RhythmDirector == null) {
                m_RhythmDirector = GetComponent<RhythmDirector>();
            }
            
            m_AllPooledNotes = new List<Note>();
        }

        public virtual void TriggerInput(InputEventData inputEventData)
        {
            InvokeNoteTriggerInputEventInternal(inputEventData);
            
            
            if (inputEventData.Note == null) { return; }

            inputEventData.Note.OnTriggerInput(inputEventData);
        }
        
        protected void InvokeNoteTriggerInputEventInternal(InputEventData inputEventData)
        {
            OnTriggerInputEvent?.Invoke(inputEventData);
        }

        public Note CreateNewNote(NoteDefinition noteDefinition, RhythmClip rhythmClip)
        {
            var noteGameObject = 
                Application.isPlaying
                    ? PoolManager.Instantiate(noteDefinition.NotePrefab)
                    : GameObject.Instantiate<GameObject>(noteDefinition.NotePrefab);

            var note = noteGameObject.GetComponent<Note>();
            var rhythmClipData =  rhythmClip.RhythmClipData;
            rhythmClipData.NoteInstance = note;
            rhythmClip.RhythmClipData = rhythmClipData;
            
            // We register before we initialize. Because the initialize function invokes an event.
            RegisterToNoteEvents(note);
            
            note.Initialize(rhythmClip.RhythmClipData);

            noteGameObject.SetActive(true);

            return note;
        }

        private void RegisterToNoteEvents(Note note)
        {
            // Ensure the last spawned not is last in the list.
            if (m_AllPooledNotes.Contains(note)) {
                m_AllPooledNotes.Remove(note);
                m_AllPooledNotes.Add(note);
                return;
            }

            m_AllPooledNotes.Add(note);

            if (Application.isPlaying) {

                note.OnNoteTriggerEvent += HandleNoteTriggerEvent;
                note.OnInitialize += HandleNoteInitializeEvent;
                note.OnReset += HandleNoteResetEvent;
                note.OnActivate += HandleNoteActivateEvent;
                note.OnDeactivate += HandleNoteDeactivateEvent;
            } else {
                for (int i = m_AllPooledNotes.Count - 1; i >= 0; i--) {
                    if (m_AllPooledNotes[i] == null) {
                        m_AllPooledNotes.RemoveAt(i);
                    }
                }
            }
        }

        protected virtual void HandleNoteInitializeEvent(Note note)
        {
            InvokeNoteInitializeEventInternal(note);
        }

        protected void InvokeNoteInitializeEventInternal(Note note)
        {
            OnNoteInitializeEvent?.Invoke(note);
        }
        
        protected virtual void HandleNoteResetEvent(Note note)
        {
            InvokeNoteResetEventInternal(note);
        }

        protected void InvokeNoteResetEventInternal(Note note)
        {
            OnNoteResetEvent?.Invoke(note);
        }
        
        protected virtual void HandleNoteActivateEvent(Note note)
        {
            InvokeNotActivateEventInternal(note);
        }

        protected void InvokeNotActivateEventInternal(Note note)
        {
            OnNoteActivateEvent?.Invoke(note);
        }
        
        protected virtual void HandleNoteDeactivateEvent(Note note)
        {
            InvokeNoteDeactivateEventInternal(note);
        }

        protected void InvokeNoteDeactivateEventInternal(Note note)
        {
            OnNoteDeactivateEvent?.Invoke(note);
        }

        protected virtual void HandleNoteTriggerEvent(NoteTriggerEventData noteTriggerEventData)
        {
            InvokeNoteTriggerEventInternal(noteTriggerEventData);
        }

        protected void InvokeNoteTriggerEventInternal(NoteTriggerEventData noteTriggerEventData)
        {
            OnNoteTriggerEvent?.Invoke(noteTriggerEventData);
        }

        public void DestroyNote(Note note)
        {
            if (note == null){ return; }
            
            if(Application.isPlaying) {
                note.DestroyNote();
            }
            else
            {
                GameObject.DestroyImmediate(note.gameObject);
            }
        }
    }
}