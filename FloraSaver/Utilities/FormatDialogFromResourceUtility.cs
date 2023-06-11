using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Utilities
{
    public class FormatDialogFromResourceUtility
    {
        private Tuple<string, Dictionary<int, string>> SpecialLocations(char start, char end, string unformattedResource, Dictionary<int, string> elementDict = null)
        {
            if (elementDict == null)
            {
                elementDict = new Dictionary<int, string>();
            }

            if (unformattedResource.IndexOf(start) == -1)
            {
                return new Tuple<string, Dictionary<int, string>>(unformattedResource, elementDict) ?? new Tuple<string, Dictionary<int, string>>(unformattedResource, null);
            } 
            else
            {
                int elementStart = unformattedResource.IndexOf(start);
                var startingData = unformattedResource.Substring(elementStart);
                var elementAsString = startingData.Substring(1, startingData.IndexOf(end)-1);
                elementDict.Add(elementStart + 1, elementAsString);
                var unformattedResourceWithoutCurrentImage = unformattedResource.Remove(elementStart, startingData.IndexOf(end) + 1);
                return SpecialLocations(start, end, unformattedResourceWithoutCurrentImage, elementDict);
            }
        }

        //Images denoted by {}
        public Dictionary<int, string> ImageChangeLocations(string unformattedResource)
        {
            var noEmotionOrNewResource = SpecialLocations('#', '$', SpecialLocations('[',']', unformattedResource).Item1).Item1;
            return SpecialLocations('{', '}', noEmotionOrNewResource).Item2;
        }
        //Emotion changes denoted by []
        public Dictionary<int, string> ClipetEmotionChangeLocations(string unformattedResource)
        {
            var noImageOrNewResource = SpecialLocations('#', '$', SpecialLocations('{', '}', unformattedResource).Item1).Item1;
            return SpecialLocations('[', ']', noImageOrNewResource).Item2;
        }

        //new text boxes denoted by #$
        public Dictionary<int, string> GetTextBoxes(string unformattedResource)
        {
            var textBoxSections = new Dictionary<int, string>();
            var noImageOrEmotionResource = SpecialLocations('[', ']', SpecialLocations('{', '}', unformattedResource).Item1).Item1;
            var sections = noImageOrEmotionResource.Split("#$").ToList();
            var end = 0;
            foreach (var section in sections)
            {
                if (section.Length > 0)
                {
                    end = end + section.Length;
                    textBoxSections.Add(end, section);
                }
                
                
            }
            return textBoxSections;

        }

        public List<ClipetSpeechBubble> SortTextBoxes(string unformattedResource) 
        {
            var images = ImageChangeLocations(unformattedResource);
            var emotions = ClipetEmotionChangeLocations(unformattedResource);
            var boxLocations = GetTextBoxes(unformattedResource);
            var boxes = new List<ClipetSpeechBubble>();
            
            foreach (var box in boxLocations)
            {
                var image = images.FirstOrDefault(_ => _.Key <= box.Key);
                var emotion = emotions.FirstOrDefault(_ => _.Key <= box.Key);
                boxes.Add(new ClipetSpeechBubble(box.Value, image.Value, emotion.Value));
                images.Remove(image.Key);
                emotions.Remove(emotion.Key);
            }

            return boxes;
        }
            

    }
}
