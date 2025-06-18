using System;
using System.Collections.Generic;
using Godot;

public class Walle
{
    public static void FloodFill(int x, int y, string newColor, int width, int height)
    {
        width = Canvas.GridSize();
        height = Canvas

    // Verificar límites del canvas
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

    string targetColor = Canvas.GetPixel(x, y);
    
    // Si ya tiene el nuevo color, no hacer nada
        if (targetColor == newColor)
            return;

    Queue<(int, int)> pixels = new Queue<(int, int)>();
    bool[,] visited = new bool[width, height]; // Matriz de visitados

    pixels.Enqueue((x, y));
    visited[x, y] = true;

    // Direcciones: derecha, izquierda, abajo, arriba
    int[] dx = {1, -1, 0, 0};
    int[] dy = {0, 0, 1, -1};

    while (pixels.Count > 0)
    {
        var (currentX, currentY) = pixels.Dequeue();
        SetPixel(currentX, currentY, newColor);

        // Explorar vecinos en 4 direcciones
        for (int i = 0; i < 4; i++)
        {
            int nx = currentX + dx[i];
            int ny = currentY + dy[i];

            // Verificar límites
            if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                continue;
            
            // Verificar si es del color objetivo y no visitado
            if (!visited[nx, ny] && (Canvas.GetPixel(nx, ny) == targetColor))
            {
                visited[nx, ny] = true;
                pixels.Enqueue((nx, ny));
            }
        }
    }
}

    public static void DrawCircleOutline(int centerX, int centerY, int radius, string color, int size)
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
    public static void DrawCirclePoint(int centerX, int centerY, int offsetX, int offsetY, string color, int size)
    {
        int x = centerX + offsetX;
        int y = centerY + offsetY;
        DrawPoint(x, y, color, size);
    }

    public static void DrawRectangleLines(int left, int top, int right, int bottom, string color, int size)
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

    public static void DrawLine(int startX, int startY, int endX, int endY, int dirX, int dirY, int distance, string color, int size)
    {
        for (int i = 0; i <= distance; i++)
        {
            int x = startX + dirX * i;
            int y = startY + dirY * i;
            DrawPoint(x, y, color, size);
        }
    }

    public static void DrawLineInternal(int startX, int startY, int endX, int endY, string color, int size)
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
    public static void DrawPoint(int x, int y, string color, int size)
    {
        int radius = (size - 1) / 2;
        int intX = x;
        int intY = y;
        
        ///*
        // dibujar todos los píxeles en el área del pincel
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                // dibujo un pixel
                SetPixel(intX + dx, intY + dy, color);
            }
        }
        //*/
    }
    public static void SetPixel(int x, int y, string color)
    {   
        var colorMap = new Dictionary<string, Color>
        {
            { "Blue", Colors.Blue },
            { "Red", Colors.Red },
            { "Green", Colors.Green },
            { "Yellow", Colors.Yellow},
            { "Orange", Colors.Orange },
            { "Purple", Colors.Purple },
            { "Black", Colors.Black },
            { "White", Colors.White },
            { "Transparent", Colors.Transparent },
        };
        if (colorMap.TryGetValue(color, out Color colors))
        {
            Canvas.SetPixel(x, y, colors);
        }        
        
    }
}
