using UnityEditor;
using UnityEngine;

namespace BeatThat.NetworkStatus
{
    [CustomEditor(typeof(NetworkStatusStore))]
    public class UserGoalServiceEditor : UnityEditor.Editor
    {
        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                return;
            }

            EditorGUI.indentLevel++;

            var store = (target as NetworkStatusStore);

            var defaultColor = GUI.color;

            var state = store.stateData;

            GUI.color = state.hasNetworkError? ERROR: defaultColor;
            EditorGUILayout.LabelField("Has Network Error", state.hasNetworkError ? "TRUE" : "FALSE");

            GUI.color = state.networkReachability == NetworkReachability.NotReachable ? ERROR : defaultColor;
            EditorGUILayout.LabelField("Network Reachability", state.networkReachability.ToString());

            GUI.color = state.HasLastNetworkSuccess() ? PENDING : defaultColor;
            EditorGUILayout.LabelField("Last Network Success",
                                       state.HasLastNetworkSuccess() ?
                                       state.lastNetworkSuccess.ToString() : "none");


            GUI.color = defaultColor;
            EditorGUILayout.LabelField("Last Network Error",
                                       state.HasLastNetworkError() ?
                                       state.lastNetworkError.ToString() : "none");


            GUI.color = defaultColor;

            EditorGUI.indentLevel--;
        }

#pragma warning disable 414
        private static readonly Color IN_PROGRESS = Color.cyan;
        private static readonly Color ERROR = Color.red;
        private static readonly Color PENDING = Color.yellow;
#pragma warning restore 414


    }
}