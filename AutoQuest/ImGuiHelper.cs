using ImGuiNET;
using System.Numerics;

namespace AutoQuest
{
    internal static class ImGuiHelper
    {
        public static void ClickText(string text,string tooltip,Action clickAction)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Dalamud.Interface.Colors.ImGuiColors.TankBlue);
            try
            {
                ImGui.Text(text);
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(tooltip);
                if (ImGui.IsItemClicked())
                    clickAction.Invoke();
            }
            finally { ImGui.PopStyleColor(); }
        }
    }
}
