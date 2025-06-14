using System;
using System.Collections.Generic;
using System.Drawing;
public class Walle
{

   
    public void FloodFill(int x, int y, string color, int width, int height)
    {
        Queue<(int, int)> pixels = new Queue<(int, int)>();
        pixels.Enqueue((x, y));

        while (pixels.Count > 0)
        {
            var (currentX, currentY) = pixels.Dequeue();

            if (currentX < 0 || currentX >= width ||
                currentY < 0 || currentY >= height)
                continue;

            // Cambiar el color del píxel
            SetPixel(currentX, currentY, color);

            // Añadir píxeles vecinos (4-direcciones)
            pixels.Enqueue((currentX + 1, currentY)); // derecha
            pixels.Enqueue((currentX - 1, currentY)); // izquierda
            pixels.Enqueue((currentX, currentY + 1)); // abajo
            pixels.Enqueue((currentX, currentY - 1)); // arriba
        }
    }

    public void DrawCircleOutline(int centerX, int centerY, int radius, string color, int size)
    {
        // Implementación del algoritmo del punto medio para círculos
        int x = radius;
        int y = 0;
        int decision = 1 - radius;

        while (x >= y)
        {
            DrawCirclePoint(centerX, centerY, x, y, color, size);
            DrawCirclePoint(centerX, centerY, y, x, color, size);
            DrawCirclePoint(centerX, centerY, -x, y, color, size);
            DrawCirclePoint(centerX, centerY, -y, x, color, size);
            DrawCirclePoint(centerX, centerY, -x, -y, color, size);
            DrawCirclePoint(centerX, centerY, -y, -x, color, size);
            DrawCirclePoint(centerX, centerY, x, -y, color, size);
            DrawCirclePoint(centerX, centerY, y, -x, color, size);

            y++;
            if (decision <= 0)
            {
                decision += 2 * y + 1;
            }
            else
            {
                x--;
                decision += 2 * (y - x) + 1;
            }
        }
    }
    public void DrawCirclePoint(int centerX, int centerY, int offsetX, int offsetY, string color, int size)
    {
        int x = centerX + offsetX;
        int y = centerY + offsetY;
        DrawPoint(x, y, color, size);
    }
    
    public void DrawRectangleLines(int left, int top, int right, int bottom, string color, int size)
    {
        // RECORDAR CAMBIAR DRAWLINEINTERNAL A DRAWLINE

        // Dibujar línea superior
        DrawLineInternal(left, top, right, top, color, size);
        // Dibujar línea derecha
        DrawLineInternal(right, top, right, bottom, color, size);
        // Dibujar línea inferior
        DrawLineInternal(left, bottom, right, bottom, color, size);
        // Dibujar línea izquierda
        DrawLineInternal(left, top, left, bottom, color, size);

    }

    public void DrawLine(int startX, int startY, int endX, int endY, int dirX, int dirY, int distance, string color, int size)
    {

        for (int i = 0; i <= distance; i++)
        {
            int x = startX + dirX * i;
            int y = startY + dirY * i;

            // Dibujar un punto en esta posición con el tamaño actual y el color actual
            DrawPoint(x, y, color, size);
        }
    }
    
    public void DrawLineInternal(int startX, int startY, int endX, int endY, string color, int size)
    {
        // Calcular dirección y distancia
        int dx = endX - startX;
        int dy = endY - startY;
        int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));

        // Normalizar incrementos
        int xIncrement = dx / steps;
        int yIncrement = dy / steps;

        // Dibujar cada punto en la línea
        int x = startX;
        int y = startY;
        for (int i = 0; i <= steps; i++)
        {
            DrawPoint(x, y, color, size);
            x += xIncrement;
            y += yIncrement;
        }
    }
    public void DrawPoint(int x, int y, string color, int size)
    {
        int radius = (size - 1) / 2;
        int intX = x;
        int intY = y;

        // dibujar todos los píxeles en el área del pincel
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                // dibujo un pixel
                SetPixel(intX + dx, intY + dy, color);
            }
        }
    }
    public void SetPixel(int x, int y, string color)
    {
        // implementar como voy a pintar 
    }    
}
