using System;
using System.Globalization;
using DG.Tweening;
using Extensions;
using TMPro;
using TransitionManager.Attributes;
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
        public CanvasGroup? TargetCanvasGroup { get; private set; }
        public Image? TargetImage { get; private set; }
        public Slider? TargetSlider { get; private set; }
        public TMP_Text? TargetText { get; private set; }
        
        public const float DeactivatedOffsetY = 1080f;
        public const float ActivatedOffsetY = -359.50f;
        public const float DeactivationOffset = -1800f;

        public OpacityFadeTransition()
        {
            ActivationDuration = 0.45f;
            DeactivationDuration = 0.45f;
        }
        
        public override GameObject Construct()
        {
            var gameObject = Resources.Load<GameObject>("Prefabs/Transitions/Default/OpacityFadeTransition");
            var instance = Object.Instantiate(gameObject);
            instance.SetActive(false);
            instance.name = Name;
            instance.layer = LayerMask.NameToLayer("UI");
            TargetCanvasGroup = instance.GetComponent<CanvasGroup>();
            TargetImage = instance.GetComponentInChildren<Image>(obj => obj.name == "BlackImage");
            TargetSlider = instance.GetComponentInChildren<Slider>(obj => obj.name == "Progress", true);
            TargetText = instance.GetComponentInChildren<TMP_Text>(obj => obj.name == "Text" && obj.transform.parent.gameObject.name == "Progress", true);
            // var rectTransform = instance.GetComponent<RectTransform>();
            // if (rectTransform)
            // {
            //     rectTransform.anchorMin = Vector2.zero;
            //     rectTransform.anchorMax = Vector2.one;
            //     rectTransform.offsetMin = Vector2.zero;
            //     rectTransform.offsetMax = Vector2.zero;
            // }
            return instance;
        }

        public override bool Activate()
        {
            if (!IsConstructed || !TargetImage || !TargetSlider || !TargetCanvasGroup) 
                return false;
            //TargetCanvasGroup.alpha = 0;
            TargetSlider.value = 0;
            base.Activate();
            var lastY = ConstructedTarget!.position.y;
            DOTween.To(() => ConstructedTarget!.position.y, y => ConstructedTarget!.position = new Vector3(ConstructedTarget!.position.x, y, ConstructedTarget!.position.z), ActivatedOffsetY, ActivationDuration);
            return true;
        }
        
        public override bool Deactivate()
        {
            if (!IsConstructed || !TargetImage || !TargetSlider || !TargetCanvasGroup) 
                return false;
            //TargetSlider.value = 0;
            if (!IsActive)
            {
                //TargetCanvasGroup.alpha = 1;
                base.Activate();
            }
            //DOTween
                //.To(() => TargetCanvasGroup.alpha, x => TargetCanvasGroup.alpha = x, 0, ActivationDuration / 2f)
                //.OnComplete(() => base.Deactivate());
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