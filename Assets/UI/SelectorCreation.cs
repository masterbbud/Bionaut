using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;


namespace radial
{
    public partial class RadialMenu : Button
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<RadialMenu> { }

        public RadialMenu()
        {
            generateVisualContent += GenerateVisualContent;
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            var painter = context.painter2D;
            painter.BeginPath();
            painter.lineWidth = 0f;
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.25f, 360f, 0f);
            painter.ClosePath();
            painter.fillColor = Color.white; ;
            painter.Fill(FillRule.NonZero);
            painter.Stroke();


        }
    }
}