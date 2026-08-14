using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal.Internal;

[CustomEditor(typeof(AnimationManager))]
public class AnimationEditor : Editor
{
    SerializedProperty behaviorAnimations;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Animation Folder"))
        {
            behaviorAnimations = serializedObject.FindProperty("_behaviorAnimation");
            behaviorAnimations.ClearArray();

            GetFolders();

            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Behavior Animations"))
        {
            behaviorAnimations = serializedObject.FindProperty("_behaviorAnimation");
            behaviorAnimations.ClearArray();

            serializedObject.ApplyModifiedProperties();
        }
    }

    void GetFolders()
    {
        // defining path where the main folder is
        string parentPathFolder = "Assets/Animations/AnimationSystemFolder";

        // getting head animation folders
        string[] subFolders = AssetDatabase.GetSubFolders(parentPathFolder);

        for (int i = 0; i < subFolders.Length; i++)
        {
            //Debug.Log("Found subfolder :" + subFolders[i]);

            // adding each subfolder name to the list
            behaviorAnimations.InsertArrayElementAtIndex(i);
            SerializedProperty element = behaviorAnimations.GetArrayElementAtIndex(i);

            // getting serialized reference to the enum for animation type
            SerializedProperty animationElement = element.FindPropertyRelative("animationType");

            // setting each enum in the array set to the folder name (WHICH HAS TO MATCH UP!!)
            animationElement.enumValueIndex = i;

            // add list of animation to each array element
            SerializedProperty clipsElement = element.FindPropertyRelative("clips");

            // clearing each clip element in the array before continuing
            clipsElement.ClearArray();

            // finding the assets, "t:" defines the type of assets we are looking for
            string[] clipsFound = AssetDatabase.FindAssets("t:AnimationClip", new[] { subFolders[i] });

            // adding the total amount of clips to clips underneath each animation type
            for (int j = 0; j < clipsFound.Length; j++)
            {
                clipsElement.InsertArrayElementAtIndex(j);
                SerializedProperty clipElement = clipsElement.GetArrayElementAtIndex(j);

                //converting the string of found animation string at folder path to an animation clip asset
                string anim = AssetDatabase.GUIDToAssetPath(clipsFound[j]);
                AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(anim);

                // refering the animation clip to index of the array
                clipElement.objectReferenceValue = animation;

                // vertifying this!
                Debug.Log($"{subFolders[i]} found {clipsFound.Length} animations");
            }
        
        }
    }

}
