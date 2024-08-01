using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Reflectis.SDK.Utilities;

namespace Reflectis.SDK.CreatorKit
{
    public enum EQuizLayout
    {
        Horizontal = 0,
        Vertical = 1,
        Grid = 2
    }
    public enum EQuizElementLayout
    {
        Line = 0,
        Box = 1
    }

    [Serializable]
    public class QuizAnswer
    {
        [SerializeField, TextArea]
        private string titleLabel = string.Empty;

        [SerializeField]
        private Sprite image = null;

        [SerializeField]
        private float score = 0;

        [SerializeField, TextArea]
        private string feedbackLabel = string.Empty;

        public string TitleLabel => titleLabel.Trim();  // Removes white spaces at start and end of the string.
        public Sprite Image => image;
        public float Score => score;
        public string FeedbackLabel => feedbackLabel.Trim();  // Removes white spaces at start and end of the string.

        public bool IsSelected { get; private set; }

        public void Select()
        {
            IsSelected = true;
        }
        public void Deselect()
        {
            IsSelected = false;
        }
    }

    public class QuizPlaceholder : SceneComponentPlaceholderNetwork
    {
        [HelpBox("Do not change the value of \"IsNetworked\" field", HelpBoxMessageType.Warning)]

        #region Quiz Placeholder References

        [Header("Quiz Placeholder references (internal). \n=> Do not change unless making a custom prefab.")]

        [SerializeField]
        private bool showReferences = false;

        [DrawIf(nameof(showReferences), true)]
        [SerializeField, Tooltip("The transform that contains the body of the media player (the panel, optional graphics, and so on). " +
       "It's recommended to put custom graphics, like a background, a logo, etc. as children of this transform, " +
       "but keep in mind that, when a media is sent to this panel, the GameObject associated with this transform will be deactivated.")]
        private Transform contentTransform;

        [DrawIf(nameof(showReferences), true)]
        [SerializeField, Tooltip("The transform that represents the panel where the media is being reproduced. " +
            "Do not change its size, it will be automatically updated by using the panel settings.")]
        private Transform panelTransform;

        [DrawIf(nameof(showReferences), true)]
        [SerializeField, Tooltip("The transform used by the camera in case of a pan towards the panel. " +
            "Do not change its local position, it will be automatically updated by using the panel settings.")]
        private Transform cameraPanTransform;

        #endregion

        #region Quiz Panel Size

        [HelpBox("To resize the panel, don't modify the scale of the transforms, but use the parameters \"Panel Width\" and \"Panel Height\" " +
            "and they will adjust automatically its dimensions. The same applies to the distance of the camera pan transform.", HelpBoxMessageType.Info)]

        [Header("Panel settings")]

        [SerializeField, /*Range(0.5f, 10),*/ Tooltip("The width of the panel.")]
        [OnChangedCall(nameof(OnWidthChanged))]
        private float panelWidth = 1.5f;

        [SerializeField, /*Range(0.5f, 10),*/ Tooltip("The height of the panel.")]
        [OnChangedCall(nameof(OnHeightChanged))]
        private float panelHeight = 1f;

        [SerializeField/*, Range(0.5f, 10)*/, Tooltip("The distance of the transform to which the camera pans (WebGL only).")]
        [OnChangedCall(nameof(OnPanTransformChanged))]
        private float cameraPanDistance = 1f;

        #endregion

        [Header("Quiz details")]

        [SerializeField, TextArea]
        private string titleLabel = "Quiz";

        [SerializeField, TextArea]
        private string descriptionLabel = string.Empty;

        [Space]

        [SerializeField]
        private bool allowMultipleSelection = true;

        [DrawIf(nameof(allowMultipleSelection), true)]
        [Min(0)]
        [SerializeField]
        private int maxSelectableAnswers = 100;

        [Space]

        [SerializeField]
        private bool shuffleAnswers = false;

        [DrawIf(nameof(shuffleAnswers), true)]
        [SerializeField]
        private bool pickSubset = false;

        [DrawIf(nameof(shuffleAnswers), true)]
        [DrawIf(nameof(pickSubset), true)]
        [Min(1)]
        [SerializeField]
        private int answersSubsetQuantity = 100;

        [Space]

        [SerializeField]
        private EQuizLayout quizLayout = EQuizLayout.Horizontal;

        [SerializeField]
        private EQuizElementLayout quizElementLayout = EQuizElementLayout.Line;

        [SerializeField]
        private ScriptMachine quizEventsScriptMachine;

        [Space]

        [SerializeField]
        private List<QuizAnswer> quizAnswers;


        public Transform ContentTransform => contentTransform;
        public Transform PanelTransform => panelTransform;
        public Transform CameraPanTransform => cameraPanTransform;
        public string TitleLabel => titleLabel.Trim(); // Removes white spaces at start and end of the string.
        public string DescriptionLabel => descriptionLabel.Trim(); // Removes white spaces at start and end of the string.
        public bool AllowMultipleSelection => allowMultipleSelection;
        public bool ShuffleAnswers => shuffleAnswers;
        public bool PickSubset => pickSubset;
        public int AnswersSubsetQuantity => answersSubsetQuantity;
        public EQuizLayout QuizLayout => quizLayout;
        public EQuizElementLayout QuizElementLayout => quizElementLayout;
        public ScriptMachine QuizEventsScriptMachine => quizEventsScriptMachine;
        public List<QuizAnswer> QuizAnswers => quizAnswers;

        // MaxAnswers: if multiple answers are not allowed, automatically reduce to 1.
        // In every case, MaxAnswers can't be negative.
        public int MaxSelectableAnswers => AllowMultipleSelection ? Mathf.Max(maxSelectableAnswers, 0) : 1;

        public void OnWidthChanged()
        {
            panelTransform.localScale = new Vector3(panelWidth, panelTransform.localScale.y, panelTransform.localScale.z);
        }

        public void OnHeightChanged()
        {
            panelTransform.localScale = new Vector3(panelTransform.localScale.x, panelHeight, panelTransform.localScale.z);
        }

        public void OnPanTransformChanged()
        {
            cameraPanTransform.localPosition = new Vector3(cameraPanTransform.localPosition.x, cameraPanTransform.localPosition.y, -cameraPanDistance);
        }

#if UNITY_EDITOR
        TextMesh placeholderLabelEditor;
        private void OnValidate()
        {
            if (placeholderLabelEditor == null)
            {
                placeholderLabelEditor = GetComponentInChildren<TextMesh>();
            }

            // Setting quiz info on each quiz placeholder text.
            if (placeholderLabelEditor != null)
            {
                placeholderLabelEditor.text = TitleLabel;
            }
        }
#endif
    }
}
