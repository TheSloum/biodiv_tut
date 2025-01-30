using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class J_DebugProjectAnalyzer : MonoBehaviour
{
    [Tooltip("Touche pour déclencher l'analyse")]
    public KeyCode debugKey = KeyCode.H;

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            AnalyzeScene();
        }
    }

    void AnalyzeScene()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== SCENE ANALYSIS REPORT ===");
        report.AppendLine($"Active scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        report.AppendLine("\n// HIERARCHY STRUCTURE //");

        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            BuildHierarchyReport(root, ref report, 0);
        }

        report.AppendLine("\n// ADDITIONAL INFO //");
        report.AppendLine($"Total GameObjects: {CountAllObjects(rootObjects)}");
        report.AppendLine($"Active Objects: {CountActiveObjects(rootObjects)}");
        report.AppendLine($"Components Found: {CountComponents(rootObjects)}");

        Debug.Log(report.ToString());
    }

    void BuildHierarchyReport(GameObject obj, ref StringBuilder report, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        
        // Nom et état d'activation
        string activeStatus = obj.activeInHierarchy ? "" : " (INACTIVE)";
        report.AppendLine($"{indent}├─ {obj.name}{activeStatus}");

        // Liste des composants
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            report.AppendLine($"{indent}│  ● {comp.GetType().Name}");
        }

        // Enfants
        Transform t = obj.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            BuildHierarchyReport(t.GetChild(i).gameObject, ref report, indentLevel + 1);
        }
    }

    int CountAllObjects(GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            count += root.GetComponentsInChildren<Transform>(true).Length;
        }
        return count;
    }

    int CountActiveObjects(GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.gameObject.activeInHierarchy) count++;
            }
        }
        return count;
    }

    int CountComponents(GameObject[] roots)
    {
        int count = 0;
        foreach (GameObject root in roots)
        {
            count += root.GetComponentsInChildren<Component>(true).Length;
        }
        return count;
    }
}