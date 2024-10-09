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
            painter.fillColor = Color.white;
            painter.Fill(FillRule.NonZero);
            painter.Stroke();
        }

        private void RadialVisualElement(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            var painter = context.painter2D;

            float angle = 360.0f / 5.0f;

            float innerRadius = width / 3;
            float outerRadius = width / 2 - 10;
            
            for (uint i = 0; i < 5; i++)
            {
                painter.BeginPath();
                painter.lineWidth = 0;
                painter.MoveTo(new Vector2(Mathf.Cos(angle * i) * innerRadius, Mathf.Sin(angle * i) * innerRadius));
                painter.LineTo(new Vector2(Mathf.Cos(angle * i) * innerRadius, Mathf.Sin(angle * i) * outerRadius));
                // painter.ArcTo(new Vector2(Mathf.Cos(angle * (i + 1)) * outerRadius, Mathf.Sin(angle * (i + 1)) * outerRadius), 5.0f);
                painter.LineTo(new Vector2(Mathf.Cos(angle * (i + 1)) * innerRadius, Mathf.Sin(angle * (i + 1)) * innerRadius));
                painter.ClosePath();
                painter.fillColor = Color.white;
                painter.Fill(FillRule.NonZero);
                painter.Stroke();
            }
        }
    }
}