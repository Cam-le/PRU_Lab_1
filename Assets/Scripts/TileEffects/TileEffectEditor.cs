using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
/// <summary>
/// Editor utility to help with setting up tile effects
/// </summary>
[CustomEditor(typeof(TileEffect), true)]
public class TileEffectEditor : Editor
{
    private bool showGeneralSettings = true;
    private bool showVisualSettings = true;
    private bool showAudioSettings = true;
    private bool showEffectSpecificSettings = true;

    public override void OnInspectorGUI()
    {
        TileEffect tileEffect = (TileEffect)target;

        EditorGUILayout.Space();

        // General settings
        showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings", true);
        if (showGeneralSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectDescription"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectWeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showNotification"));

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Visual settings
        showVisualSettings = EditorGUILayout.Foldout(showVisualSettings, "Visual Settings", true);
        if (showVisualSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectIcon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectParticlesPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectDuration"));

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Audio settings
        showAudioSettings = EditorGUILayout.Foldout(showAudioSettings, "Audio Settings", true);
        if (showAudioSettings)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectSound"));

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Effect-specific settings
        showEffectSpecificSettings = EditorGUILayout.Foldout(showEffectSpecificSettings, "Effect-Specific Settings", true);
        if (showEffectSpecificSettings)
        {
            EditorGUI.indentLevel++;

            // Draw default inspector excluding properties we already handled
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                "effectName",
                "effectDescription",
                "effectWeight",
                "showNotification",
                "effectIcon",
                "effectColor",
                "effectParticlesPrefab",
                "effectDuration",
                "effectSound"
            });

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Add test button
        if (GUILayout.Button("Test Effect"))
        {
            TestEffect(tileEffect);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void TestEffect(TileEffect effect)
    {
        if (Application.isPlaying)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                effect.ApplyEffect(player);
                Debug.Log($"Testing effect: {effect.EffectName}");
            }
            else
            {
                Debug.LogWarning("No PlayerController found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning("Effect can only be tested during play mode!");
        }
    }
}
#endif

/// <summary>
/// Helper component for setting up predefined tile effects in the editor
/// </summary>
public class TileEffectPrefab : MonoBehaviour
{
    [System.Serializable]
    public class EffectPreset
    {
        public string presetName;
        public TileEffect effectPrefab;
    }

    [Header("Presets")]
    [SerializeField] private List<EffectPreset> resourceEffectPresets = new List<EffectPreset>();
    [SerializeField] private List<EffectPreset> movementEffectPresets = new List<EffectPreset>();
    [SerializeField] private List<EffectPreset> statusEffectPresets = new List<EffectPreset>();
    [SerializeField] private List<EffectPreset> storyEffectPresets = new List<EffectPreset>();
    [SerializeField] private List<EffectPreset> visualEffectPresets = new List<EffectPreset>();
    [SerializeField] private List<EffectPreset> comboEffectPresets = new List<EffectPreset>();

    [Header("Automatic Setup")]
    [SerializeField] private bool setupOnStart = false;

    private void Start()
    {
        if (setupOnStart)
        {
            SetupEffects();
        }
    }

    /// <summary>
    /// Set up the effects in the TileEffectManager
    /// </summary>
    public void SetupEffects()
    {
        TileEffectManager effectManager = FindObjectOfType<TileEffectManager>();
        if (effectManager == null)
        {
            Debug.LogError("No TileEffectManager found in the scene!");
            return;
        }

        // Clear existing effects
        ClearComponentsField(effectManager, "availableEffects");
        ClearComponentsField(effectManager, "positiveEffects");
        ClearComponentsField(effectManager, "negativeEffects");
        ClearComponentsField(effectManager, "neutralEffects");
        ClearComponentsField(effectManager, "specialEffects");
        ClearComponentsField(effectManager, "storyEffects");

        // Add resource effects
        foreach (EffectPreset preset in resourceEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);

                // Categorize effect
                ResourceEffect resourceEffect = effect as ResourceEffect;
                if (resourceEffect != null)
                {
                    // Check if positive or negative
                    int scoreChange = GetPrivateField<int>(resourceEffect, "scoreChange");
                    int movesChange = GetPrivateField<int>(resourceEffect, "movesChange");

                    if (scoreChange > 0 || movesChange > 0)
                    {
                        AddToComponentsField(effectManager, "positiveEffects", effect);
                    }
                    else if (scoreChange < 0 || movesChange < 0)
                    {
                        AddToComponentsField(effectManager, "negativeEffects", effect);
                    }
                    else
                    {
                        AddToComponentsField(effectManager, "neutralEffects", effect);
                    }
                }
            }
        }

        // Add movement effects
        foreach (EffectPreset preset in movementEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);

                // Categorize effect
                MovementEffect movementEffect = effect as MovementEffect;
                if (movementEffect != null)
                {
                    MovementEffect.MovementType movementType = GetPrivateField<MovementEffect.MovementType>(movementEffect, "movementType");

                    switch (movementType)
                    {
                        case MovementEffect.MovementType.MoveForward:
                        case MovementEffect.MovementType.ExtraTurn:
                            AddToComponentsField(effectManager, "positiveEffects", effect);
                            break;

                        case MovementEffect.MovementType.MoveBackward:
                        case MovementEffect.MovementType.SkipNextTurn:
                            AddToComponentsField(effectManager, "negativeEffects", effect);
                            break;

                        case MovementEffect.MovementType.Teleport:
                        case MovementEffect.MovementType.ReturnToCheckpoint:
                            AddToComponentsField(effectManager, "specialEffects", effect);
                            break;
                    }
                }
            }
        }

        // Add status effects
        foreach (EffectPreset preset in statusEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);

                // Categorize effect
                StatusEffect statusEffect = effect as StatusEffect;
                if (statusEffect != null)
                {
                    bool isPositive = GetPrivateField<bool>(statusEffect, "isPositive");

                    if (isPositive)
                    {
                        AddToComponentsField(effectManager, "positiveEffects", effect);
                    }
                    else
                    {
                        AddToComponentsField(effectManager, "negativeEffects", effect);
                    }
                }
            }
        }

        // Add story effects
        foreach (EffectPreset preset in storyEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);
                AddToComponentsField(effectManager, "storyEffects", effect);
            }
        }

        // Add visual effects
        foreach (EffectPreset preset in visualEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);
                AddToComponentsField(effectManager, "neutralEffects", effect);
            }
        }

        // Add combo effects
        foreach (EffectPreset preset in comboEffectPresets)
        {
            if (preset.effectPrefab != null)
            {
                TileEffect effect = Instantiate(preset.effectPrefab, effectManager.transform);
                effect.name = preset.presetName;
                AddToComponentsField(effectManager, "availableEffects", effect);
                AddToComponentsField(effectManager, "specialEffects", effect);
            }
        }

        Debug.Log("Tile effects set up successfully!");
    }

    private void ClearComponentsField(object obj, string fieldName)
    {
        System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        if (field != null)
        {
            System.Collections.IList list = (System.Collections.IList)field.GetValue(obj);
            if (list != null)
            {
                list.Clear();
            }
        }
    }

    private void AddToComponentsField(object obj, string fieldName, TileEffect effect)
    {
        System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        if (field != null)
        {
            System.Collections.IList list = (System.Collections.IList)field.GetValue(obj);
            if (list != null)
            {
                list.Add(effect);
            }
        }
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic);

        if (field != null)
        {
            return (T)field.GetValue(obj);
        }

        return default(T);
    }
}