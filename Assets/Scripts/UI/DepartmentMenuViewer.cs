using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepartmentMenuViewer : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private DepartmentInfoPanelViewer departmentInfoPanelViewer;
    [SerializeField] private CrewAssignmentPanelController crewAssignmentPanelController;
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void Show(Department department)
    {
        content.SetActive(true);
        Initialize(department);
    }

    public void Hide()
    {
        content.SetActive(false);
    }
    
    private void Initialize(Department department)
    {
        departmentInfoPanelViewer.Initialization(department);
    }
}
