using LLMUnity;
using TinyShrine.VRoid.Chats.Utils;
using TMPro;
using UnityEngine;

namespace TinyShrine.VRoid.Chats.Controllers
{
    public class LLMChatController : MonoBehaviour
    {
        [SerializeField]
        private LLMCharacter llmCharacter;

        [SerializeField]
        private TMP_InputField userText;

        [SerializeField]
        private TMP_Text avatarText;

        private bool onValidateWarning = true;

        public void SetAvatarText(string text)
        {
            avatarText.text = TextUtil.ConvertMarkdownToTMPRichText(text);
        }

        public void AIReplyComplete()
        {
            userText.interactable = true;
            userText.Select();
            userText.text = string.Empty;
        }

        public void CancelRequests()
        {
            llmCharacter.CancelRequests();
            AIReplyComplete();
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }

        private void Start()
        {
            userText.onSubmit.AddListener(OnInputFieldSubmit);
            userText.Select();
        }

        private void OnInputFieldSubmit(string message)
        {
            userText.interactable = false;
            avatarText.text = "...";
            _ = llmCharacter.Chat(message, SetAvatarText, AIReplyComplete);
        }

        private void OnValidate()
        {
            if (
                onValidateWarning
                && !llmCharacter.remote
                && llmCharacter.llm != null
                && string.IsNullOrEmpty(llmCharacter.llm.model)
            )
            {
                Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
                onValidateWarning = false;
            }
        }
    }
}
