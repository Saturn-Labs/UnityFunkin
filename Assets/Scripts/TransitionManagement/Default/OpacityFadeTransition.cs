using System;
using System.Globalization;
using DG.Tweening;
using Extensions;
using TMPro;
using TransitionManagement.Attributes;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TransitionManagement.Default
{
    [TransitionTypeDeclaration(
        Name = "OpacityFadeTransition"
    )]
    // ReSharper disable once ClassNeverInstantiated.Global
    // Instantiated via reflection
    public class OpacityFadeTransition : AbstractTransition
    {
        public CanvasGroup? ProgressCanvasGroup { get; private set; }
        public Image? TargetImage { get; private set; }
        public Slider? TargetSlider { get; private set; }
        public TMP_Text? TargetText { get; private set; }
        
        public const float DeactivatedOffsetY = 1480f;
        public const float ActivatedOffsetY = 0f;

        public OpacityFadeTransition()
        {
            ActivationDuration = 1f;
            DeactivationDuration = 1f;
        }
        
        public override GameObject Construct()
        {
            var gameObject = Resources.Load<GameObject>("Prefabs/TransitionManagement/Basic/GradientTransition");
            var instance = Object.Instantiate(gameObject);
            instance.SetActive(false);
            instance.name = Name;
            instance.layer = LayerMask.NameToLayer("UI");
            TargetImage = instance.GetComponentInChildren<Image>(obj => obj.name == "BlackImage");
            TargetSlider = instance.GetComponentInChildren<Slider>(obj => obj.name == "Progress", true);
            ProgressCanvasGroup = instance.GetComponentInChildren<CanvasGroup>(obj => obj.name == "Progress", true);
            TargetText = instance.GetComponentInChildren<TMP_Text>(obj => obj.name == "Text" && obj.transform.parent.gameObject.name == "Progress", true);
            var rectTransform = instance.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, DeactivatedOffsetY);
            }
            
            return instance;
        }

        public override bool Activate()
        {
            if (!IsConstructed || !TargetImage || !TargetSlider || !ProgressCanvasGroup || IsActive) 
                return false;
            TargetSlider.value = 0;
            ProgressCanvasGroup.alpha = 0;
            ConstructedTarget!.anchoredPosition = new Vector2(ConstructedTarget!.anchoredPosition.x, DeactivatedOffsetY);
            base.Activate();
            DOTween.To(() => ConstructedTarget!.anchoredPosition.y, y => ConstructedTarget!.anchoredPosition = new Vector2(ConstructedTarget!.anchoredPosition.x, y), ActivatedOffsetY, ActivationDuration)
                .OnComplete(() => ProgressCanvasGroup.alpha = 1);
            return true;
        }
        
        public override bool Deactivate()
        {
            if (!IsConstructed || !TargetImage || !TargetSlider || !ProgressCanvasGroup || !IsActive) 
                return false;
            TargetSlider.value = 0;
            ProgressCanvasGroup.alpha = 0;
            DOTween
                .To(() => ConstructedTarget!.anchoredPosition.y, y => ConstructedTarget!.anchoredPosition = new Vector2(ConstructedTarget!.anchoredPosition.x, y), -DeactivatedOffsetY, ActivationDuration)
                .OnComplete(() =>
                {
                    base.Deactivate();
                });
            return true;
        }

        public override void OnSceneLoadingProgress(AsyncOperation operation)
        {
            if (TargetSlider)
            {
                TargetSlider.value = operation.progress;
            }

            if (TargetText)
            {
                TargetText.text = $"Loading Scene ({Math.Clamp(operation.progress * 100.0f, 0, 100).ToString("F1", CultureInfo.InvariantCulture)}%)";
            }
        }
    }
}