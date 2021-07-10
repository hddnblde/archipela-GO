using archipelaGO.UI.Windows;

namespace archipelaGO.VisualNovel.StorySystem
{
    public sealed class VisualNovelChoiceWindow : ChoiceWindow
    {
        protected override void SelectChoice(int choice)
        {
            base.SelectChoice(choice);
            Hide();
        }
    }
}