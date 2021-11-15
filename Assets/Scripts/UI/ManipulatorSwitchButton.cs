using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManipulatorSwitchButton : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Button _translateButton;
    [SerializeField]
    private Button _rotateButton;
    [SerializeField]
    private Button _scaleButton;

    public ManipulatorSwitchButton(TextMeshProUGUI text)
    {
        this._text = text;
    }

    void Start()
    {
        GizmoManipulatorManager.onUpdate.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if(_text!=null)
            _text.text = GizmoManipulatorManager.instance.CurrentManipulator.ToString();

        if (_translateButton == null || _rotateButton == null || _scaleButton == null)
            return;

        _translateButton.interactable = GizmoManipulatorManager.instance.CurrentManipulator != GizmoManipulatorManager.Manipulator.Translate;
        _rotateButton.interactable = GizmoManipulatorManager.instance.CurrentManipulator != GizmoManipulatorManager.Manipulator.Rotate;
        _scaleButton.interactable = GizmoManipulatorManager.instance.CurrentManipulator != GizmoManipulatorManager.Manipulator.Scale;
        
    }

    public void SetNextManipulator()
    {
        GizmoManipulatorManager.instance.SwitchToNextManipulator();
    }

    public void SetManipulator(GizmoManipulatorManager.Manipulator manipulator) 
    {
        GizmoManipulatorManager.instance.CurrentManipulator = manipulator;
    }

    public void SetManipulatorTranslate()
    {
        SetManipulator(GizmoManipulatorManager.Manipulator.Translate);
    }
    public void SetManipulatorRotate()
    {
        SetManipulator(GizmoManipulatorManager.Manipulator.Rotate);
    }
    public void SetManipulatorScale()
    {
        SetManipulator(GizmoManipulatorManager.Manipulator.Scale);
    }
}
