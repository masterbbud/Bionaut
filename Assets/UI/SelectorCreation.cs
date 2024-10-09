using System;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



namespace radial
{
    public partial class RadialMenu : Button
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<RadialMenu> { }

        [SerializeField]
        float outerRadius = 50.0f;

        [SerializeField]
        float innerRadius = 20.0f;

        static int numItems = 10;

        Vector2[] pointListOuter = new Vector2[numItems];
        Vector2[] pointListInner = new Vector2[numItems];

        public RadialMenu()
        {
            generateVisualContent += GenerateVisualContent;
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            var painter = context.painter2D;
            painter.strokeColor = Color.black;
            painter.fillColor = Color.white;
            painter.lineWidth = 1;
            Vector2 origin = new Vector2(width / 2, height / 2);
            GeneratePoints(height / 2, width / 2);

            for (int i = 0; i < numItems; i++)
            {
                painter.MoveTo(origin);
                painter.BeginPath();
                painter.LineTo(pointListOuter[i]);
                painter.ArcTo(pointListOuter[i], pointListOuter[(i + 1) % numItems], 500);
                painter.LineTo(pointListInner[(i + 1) % numItems]);
                painter.ArcTo(pointListInner[(i + 1) % numItems], pointListInner[i], 2);
                painter.Fill(FillRule.OddEven);
                painter.Stroke();
                painter.ClosePath();
            }
        }

        private void GeneratePoints(float height, float width)
        {
            float angle = (2.0f * Mathf.PI) / numItems;
            for (int i = 0; i < numItems; i++)
            {
                pointListOuter[i] = new Vector2((float)Math.Cos(angle * i) * outerRadius + width, (float)Math.Sin(angle * i) * outerRadius + height);
                pointListInner[i] = new Vector2((float)Math.Cos(angle * i) * innerRadius + width, (float)Math.Sin(angle * i) * innerRadius + height);
            }
        }

        private void GenerateButton(float height, float width)
        {
            var button = new Button();
        }
    }
}