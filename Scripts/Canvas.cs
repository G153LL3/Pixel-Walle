using Godot;
using System;

public partial class Canvas : TextureRect
{
    private Color[,] pixels;
    [Export] private int gridSize = 20; // Tamaño de la cuadrícula (en bloques)
    [Export] private int pixelSize = 20; // Tamaño de cada bloque en píxeles reales
    [Export] private Color gridColor = new Color(0.8f, 0.8f, 0.8f);

    private Image _image;
    private ImageTexture _texture;
    public static Canvas Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this; // Asigna la instancia al crear
        InitializeGrid();
        GenerateTexture();
    }

    private void InitializeGrid()
    {
        pixels = new Color[gridSize, gridSize];
        // Rellenar con blanco
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                pixels[x, y] = Colors.White;
            }
        }
    }

    private void GenerateTexture()
    {
        // Calcular tamaño total de la textura
        int width = gridSize * pixelSize;
        int height = gridSize * pixelSize;

        _image = Image.Create(width, height, false, Image.Format.Rgba8);
        _texture = ImageTexture.CreateFromImage(_image);
        Texture = _texture;

        UpdateTexture();
    }

    private void UpdateTexture()
    {
        // Limpiar imagen
        _image.Fill(Colors.Transparent);

        // Dibujar cada bloque
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Dibujar el bloque de color
                DrawBlock(x, y, pixels[x, y]);
            }
        }

        // Dibujar líneas de la cuadrícula
        DrawGridLines();

        // Actualizar la textura
        _texture.Update(_image);
    }

    private void DrawBlock(int gridX, int gridY, Color color)
    {
        int startX = gridX * pixelSize;
        int startY = gridY * pixelSize;

        for (int x = startX; x < startX + pixelSize; x++)
        {
            for (int y = startY; y < startY + pixelSize; y++)
            {
                _image.SetPixel(x, y, color);
            }
        }

    }

    private void DrawGridLines()
    {
        int width = gridSize * pixelSize;
        int height = gridSize * pixelSize;

        // Dibujar líneas verticales
        for (int x = 0; x <= width; x += pixelSize)
        {
            for (int y = 0; y < height; y++)
            {
                _image.SetPixel(x, y, gridColor);
            }
        }

        // Dibujar líneas horizontales
        for (int y = 0; y <= height; y += pixelSize)
        {
            for (int x = 0; x < width; x++)
            {
                _image.SetPixel(x, y, gridColor);
            }
        }
        //SetPixel(5, 5, Colors.Green);
    }

    public void SetGridSize(int newSize)
    {
        if (newSize < 1) return;
        gridSize = newSize;
        InitializeGrid();
        GenerateTexture();
    }
    public static void SetPixel(int x, int y, Color color)
    {
        if (Instance == null) return;

        if (x >= 0 && x < Instance.gridSize && y >= 0 && y < Instance.gridSize)
        {
            Instance.pixels[x, y] = color;
            //Instance.DrawBlock(x, y, color);
            //Instance._texture.Update(Instance._image);
            Instance.UpdateTexture();
        }

    }

    public void PaintPixel(Vector2 position)
    {
        // Calcular posición en la cuadrícula
        int gridX = (int)(position.X / pixelSize);
        int gridY = (int)(position.Y / pixelSize);

        // Asegurarse de que está dentro de los límites
        if (gridX >= 0 && gridX < gridSize && gridY >= 0 && gridY < gridSize)
        {
            SetPixel(gridX, gridY, Colors.Black);
        }
    }
    public static string GetPixel(int x, int y)
    {
        string def = "Transparent";
        if (Instance == null)
            return def;

        if (x >= 0 && x < Instance.gridSize && y >= 0 && y < Instance.gridSize)
        {
            Color pixelcolor = Instance.pixels[x, y];
            string test = pixelcolor.ToString();

            return test;
        }
        else
        {
            return def;
        }


    }
}
