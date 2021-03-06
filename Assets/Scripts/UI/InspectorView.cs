using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InspectorView : MonoBehaviour
{
    [SerializeField]
    private EntityHeaderUI _entityHeaderUi;

    [SerializeField]
    private GameObject _components;

    [SerializeField]
    private GameObject[] _moreThanOneSelectedObjects;
    [SerializeField]
    private GameObject[] _nothingSelectedObjects;
    [SerializeField]
    private GameObject[] _somethingSelectedObjects;

    private RectTransform _rectTransform;


    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        SceneManager.OnUpdateSelection.AddListener(SetDirty);
    }

    void OnEnable()
    {
        SetDirty();
    }

    private bool _dirty = false;

    public void SetDirty()
    {
        _dirty = true;
    }

    void LateUpdate()
    {
        if (_dirty)
        {
            _dirty = false;
            UpdateVisuals();
        }
    }

    private void ShowObjects(GameObject[] objectsToActivate)
    {
        var allObjects = _moreThanOneSelectedObjects.Concat(_nothingSelectedObjects).Concat(_somethingSelectedObjects).Where((o)=>!objectsToActivate.Contains(o));
        foreach (var go in allObjects)
        {
            go.SetActive(false);
        }

        foreach (var go in objectsToActivate)
        {
            go.SetActive(true);   
        }
    }

    public void UpdateVisuals()
    {
        //EditorApplication.isPaused = true;
        var entity = SceneManager.PrimarySelectedEntity;

        if (entity == null)
        {
            ShowObjects(_nothingSelectedObjects);
        }
        else if (SceneManager.SecondarySelectedEntity.Any(e => e!=null)) // When there are any Secondary selected entities
        {
            ShowObjects(_moreThanOneSelectedObjects);
        }
        else
        {
            ShowObjects(_somethingSelectedObjects);


            // Entity Header
            _entityHeaderUi.entity = entity;
            _entityHeaderUi.UpdateVisuals();


            // Components
            foreach (Transform component in _components.transform)
            {
                Destroy(component.gameObject);
            }


            foreach (var component in entity.Components)
            {
                var newComponentObject = Instantiate(component.UiItemTemplate, Vector3.zero, Quaternion.identity, _components.transform);
                //var newComponentRectTransform = newComponentObject.GetComponent<RectTransform>();
                //componentUiItemTemplate.GetComponent<RectTransform>().position += Vector3.down;
                //if (newComponentObject.TryGetComponent<ComponentUI>(out var newComponentUi))
                //{
                //    newComponentUi.entityComponent = component;
                //    newComponentUi.UpdateVisuals();
                //}

                if (component.GetType() != typeof(TransformComponent)) // Can't remove Transform component
                    newComponentObject.GetComponentInChildren<RemoveComponent>().component = component;

                if (component.GetType() == typeof(GLTFShapeComponent))
                {
                    var assetItem = newComponentObject.GetComponentInChildren<AssetItemUI>();
                    assetItem.asset = ((GLTFShapeComponent)component).asset;
                    assetItem.isInInspector = true;
                }
            }

            Canvas.ForceUpdateCanvases();

            //StartCoroutine(ReEnableAfterFrame(gameObject));
            //RefreshLayoutGroupsImmediateAndRecursive(_components);
            //RefreshLayoutGroupsImmediateAndRecursive(gameObject);
        }
    }


    IEnumerator ReEnableAfterFrame(GameObject theObject)
    {
        foreach (Transform child in theObject.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in theObject.transform)
        {
            child.gameObject.SetActive(true);
        }

        yield return null;
    }

    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject root)
    {
        foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }
}
