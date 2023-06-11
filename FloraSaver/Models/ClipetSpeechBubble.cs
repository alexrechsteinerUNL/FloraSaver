﻿namespace FloraSaver.Models
{
    public class ClipetSpeechBubble
    {
        public ClipetSpeechBubble(string dialogString, string displayImage, string clipetEmotion) 
        { 
            DialogString = dialogString;
            DisplayImage = displayImage;
            ClipetEmotion = clipetEmotion;
        }
        public string DialogString { get; set; }

        public string DisplayImage { get; set; }
        public string ClipetEmotion { get; set; }

    }

}
