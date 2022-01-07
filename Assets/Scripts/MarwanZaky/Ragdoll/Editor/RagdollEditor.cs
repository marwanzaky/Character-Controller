
using UnityEngine;
using UnityEditor;

namespace MarwanZaky
{
    [CustomEditor(typeof(Ragdoll))]
    [CanEditMultipleObjects]
    public class RagdollEditor : Editor
    {
        Ragdoll ragdoll;
        bool enable = false;

        void OnEnable()
        {
            ragdoll = (Ragdoll)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_collider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rigidbody"));

            if (GUILayout.Button("Remove"))
                Remove();

            if (GUILayout.Button(enable ? "Disable" : "Enable"))
                if (!enable)
                {
                    enable = true;
                    Enable();
                }
                else
                {
                    enable = false;
                    Disable();
                }

            serializedObject.ApplyModifiedProperties();
        }

        void Remove()
        {
            ragdoll.Init();

            foreach (var el in ragdoll.CharacterJoints)
                DestroyImmediate(el);

            foreach (var el in ragdoll.Colliders)
                DestroyImmediate(el);

            foreach (var el in ragdoll.Rigidbodies)
                DestroyImmediate(el);
        }

        void Disable()
        {
            ragdoll.Init();
            ragdoll.Revive();
        }

        void Enable()
        {
            ragdoll.Init();
            ragdoll.Die();
        }
    }
}