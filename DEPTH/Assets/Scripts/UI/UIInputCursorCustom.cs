/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2023.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity.InputModule;
using UnityEngine.XR;
using Leap;
using Hand = Leap.Hand;
using System.Drawing;
using Color = UnityEngine.Color;

public class UIInputCursorCustom : MonoBehaviour
{
    [SerializeField] private PointerElement element;
    [SerializeField] private float interactionPointerScale = 0.6f;

    //private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;
    
    [SerializeField]
    private LineRenderer lineRenderer;

    public ColorBlock colorBlock;

    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = lineRenderer.gameObject.transform.localScale;
    }

    private void OnEnable()
    {
        if (element != null)
        {
            element.OnPointerStateChanged += OnPointerStateChanged;
        }
    }

    private void OnDisable()
    {
        if (element != null)
        {
            element.OnPointerStateChanged -= OnPointerStateChanged;
        }
    }

    private void OnPointerStateChanged(PointerElement element, Hand hand)
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (element.IsUserInteractingDirectly && !element.ShowDirectPointerCursor)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        lineRenderer.transform.localScale = hand != null
            ? Vector3.Lerp(initialScale, initialScale * interactionPointerScale, hand.PinchStrength)
            : Vector3.one;

        switch (element.AggregatePointerState)
        {
            case PointerStates.OnCanvas:
                lineRenderer.material.color = colorBlock.normalColor;
                break;
            case PointerStates.OffCanvas:
                lineRenderer.material.color = colorBlock.disabledColor;
                break;
            case PointerStates.OnElement:
                lineRenderer.material.color = colorBlock.highlightedColor;
                break;
            case PointerStates.PinchingToCanvas:
                lineRenderer.material.color = colorBlock.pressedColor;
                break;
            case PointerStates.PinchingToElement:
                lineRenderer.material.color = colorBlock.pressedColor;
                break;
            case PointerStates.NearCanvas:
                lineRenderer.material.color = colorBlock.normalColor;
                break;
            case PointerStates.TouchingCanvas:
                lineRenderer.material.color = colorBlock.normalColor;
                break;
            case PointerStates.TouchingElement:
                lineRenderer.material.color = colorBlock.pressedColor;
                break;
            default:
                lineRenderer.material.color = colorBlock.normalColor;
                break;
        }
    }
}