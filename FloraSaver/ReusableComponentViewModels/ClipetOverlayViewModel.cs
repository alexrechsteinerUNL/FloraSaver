using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using System.ComponentModel;

namespace FloraSaver.ViewModels
{
    [QueryProperty(nameof(List<ClipetSpeechBubble>), "ClipetSpeechBubble")]
    public partial class ClipetOverlayViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        private readonly string DEFAULT_CLIPET_EMOTION_SOURCE = "up_close_base_clipet.png";
        private int TEXTBOXINDEX = 0;

        [ObservableProperty]
        bool isClipetVisible = false;
        [ObservableProperty]
        bool isRefImageVisable = false;
        [ObservableProperty]
        bool isSpeechBubbleVisible = false;



        [ObservableProperty]
        List<ClipetSpeechBubble> clipetDialog;
        [ObservableProperty]
        ClipetSpeechBubble currentClipetDialog;

        public ClipetOverlayViewModel() 
        {

        }

        [RelayCommand]
        async Task AppearingAsync()
        {
            if (clipetDialog == null)
            {
                await Shell.Current.GoToAsync("..");
            } else
            {
                await MoveToNextTextBoxAsync();
            }
        }

        [RelayCommand]
        async Task MoveToNextTextBoxAsync()
        {
            if (TEXTBOXINDEX >=  ClipetDialog.Count)
            {
                ClipetDialog = null;
                CurrentClipetDialog = null;
                OnPropertyChanged("ClipetSpeechBubble");
                await Shell.Current.GoToAsync("..");
            } else
            {
                IsClipetVisible = true;
                IsRefImageVisable = ClipetDialog[TEXTBOXINDEX].DisplayImage != null ? true : false;
                IsSpeechBubbleVisible = ClipetDialog[TEXTBOXINDEX].DialogString != null ? true : false;

                CurrentClipetDialog = new ClipetSpeechBubble(
                    ClipetDialog[TEXTBOXINDEX].DialogString,
                    ClipetDialog[TEXTBOXINDEX].DisplayImage,
                    ClipetDialog[TEXTBOXINDEX].ClipetEmotion != null ? ClipetDialog[TEXTBOXINDEX].ClipetEmotion : DEFAULT_CLIPET_EMOTION_SOURCE);
                OnPropertyChanged("ClipetSpeechBubble");
                TEXTBOXINDEX++;
            }
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null)
            {
                ClipetDialog = query["ClipetSpeechBubbles"] as List<ClipetSpeechBubble>;
            } else
            {
                ClipetDialog = null;
            }
        }
    }
}