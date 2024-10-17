using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ButtonCreation : Button
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<ButtonCreation> { }

    float radius = 30.0f;

    static int numItems = 5;

    Vector2[] circleOrigins = new Vector2[numItems];

    public ButtonCreation() 
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
            painter.MoveTo(circleOrigins[i]);
            painter.Arc(circleOrigins[i], 10.0f, 0, 360);
            painter.Fill(FillRule.OddEven);
            painter.ClosePath();
        }
    }

    private void GeneratePoints(float height, float width)
    {
        float angle = (2.0f * Mathf.PI) / numItems;
        for (int i = 0; i < numItems; i++)
        {
            circleOrigins[i] = new Vector2((float)Math.Cos(angle * i + 20.0f) * radius + width, (float)Math.Sin(angle * i + 20.0f) * radius + height);
        }
    }
}
