using Godot;
using System;

public partial class Canvas : TextureRect
{
    private Color[,] pixels; // para almacenar colores de los pixeles
    public int gridSize = 31; // tamaño de la cuadrícula (en bloques)
    private int pixelSize = 1; // tamaño de cada bloque en píxeles reales
    private Color gridColor = new Color(0.8f, 0.8f, 0.8f);
    public static int GridSize; 
   
    private Image _image;
    private ImageTexture _texture;
    public static Canvas Instance { get; private set; }

    public void UpdateGridSize(int newSize)
    {
        gridSize = newSize;
        InitializeGrid(); 
        GenerateTexture();
    }

    public override void _Ready()
    {
        GridSize = gridSize; 
        Instance = this;  // asigna la instancia al crear
        InitializeGrid(); // inicializa matriz
        GenerateTexture(); // crea la textura inicial
    }

    private void InitializeGrid()
    {
        pixels = new Color[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                pixels[x, y] = Colors.White; // blanco x defecto
            }
        }
    }
    private void GenerateTexture()
    {
        // calcular tamaño total de la textura
        int width = gridSize * pixelSize;
        int height = gridSize * pixelSize;

        _image = Image.Create(width, height, false, Image.Format.Rgba8); // crea imagen en blanco
        _texture = ImageTexture.CreateFromImage(_image); // crea textura desde imagen
        Texture = _texture; // asigna la   textura al texture rect
        UpdateTexture(); // actualiza el contenido
    }

    private void UpdateTexture()
    {
        // limpiar imagen
        _image.Fill(Colors.Transparent);

        // dibujar cada bloque
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // dibujar el bloque de color
                DrawBlock(x, y, pixels[x, y]);
            }
        }
        // dibujar líneas de la cuadrícula
        //DrawGridLines();

        // actualizar la textura
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

        // dibujar líneas verticales
        for (int x = 0; x <= width; x += pixelSize)
        {
            for (int y = 0; y < height; y++)
            {
                _image.SetPixel(x, y, gridColor);
            }
        }

        // dibujar líneas horizontales
        for (int y = 0; y <= height; y += pixelSize)
        {
            for (int x = 0; x < width; x++)
            {
                _image.SetPixel(x, y, gridColor);
            }
        }
    }

    public void SetGridSize(int newSize)
    {
        if (newSize < 1) return;
        gridSize = newSize;
        InitializeGrid(); // inicia matriz
        GenerateTexture(); // crea textura
    }
    public static void SetPixel(int x, int y, Color color) //pinta el pixel
    {
        if (Instance == null) return;

        if (x >= 0 && x < Instance.gridSize && y >= 0 && y < Instance.gridSize)
        {
            Instance.pixels[x, y] = color;
            Instance.UpdateTexture(); // actualiza visualizacion (es muy lento)
        }
    }

    public void PaintPixel(Vector2 position)
    {
        // calcular posición en la cuadrícula
        int gridX = (int)(position.X / pixelSize);
        int gridY = (int)(position.Y / pixelSize);

        // asegurarse de que está dentro de los límites
        if (gridX >= 0 && gridX < gridSize && gridY >= 0 && gridY < gridSize)
        {
            SetPixel(gridX, gridY, Colors.Black);
        }
    }
    public static string GetPixel(int x, int y)
    {
        if (Instance == null) return "Transparent";

        if (x >= 0 && x < Instance.gridSize && y >= 0 && y < Instance.gridSize)
        {
            Color pixelColor = Instance.pixels[x, y];
            return ConvertColorToName(pixelColor);
        }
        return "Transparent";
    }
    private static string ConvertColorToName(Color c)
    {
        if (c == Colors.White) return "White";
        if (c == Colors.Black) return "Black";
        if (c == Colors.Blue) return "Blue";
        if (c == Colors.Red) return "Red";
        if (c == Colors.Purple) return "Purple";
        if (c == Colors.Green) return "Green";
        if (c == Colors.Orange) return "Orange";
        if (c == Colors.Yellow) return "Yellow"; 
        return "Transparent";
    }
}
