# 🎨 Pixel_Wall-E 🤖🖌️

## 🚀 Introducción

**Pixel_Wall-E** es un proyecto desarrollado en C# con Godot que simula un lenguaje de programación minimalista para crear pixel-art mediante comandos. Inspirado en el robot Wall-E, el programa interpreta instrucciones escritas en un editor de texto y las ejecuta en un canvas interactivo. Soporta funciones como variables, saltos condicionales, expresiones aritméticas/booleanas, y manejo de archivos (.pw). 

---

##  🎯️ Características

💻 Interfaz sencilla en Godot.

✨ Diseña con código ✨

📝 Editor intuitivo: escribe tus comandos en el lenguaje especial de Wall-E y observa cómo cobran vida en el canvas, pixel a pixel.

🔄 Flujo creativo flexible:

    📥 Importa proyectos guardados en .pw.

    📤 Exporta tus creaciones para compartirlas.

    🎨 Visualiza al instante cada cambio.

💡 ¡Experimenta, guarda tus diseños favoritos y crea arte digital con solo líneas de código!

---

##  🖥️ Tecnologías

 - 🎮 Motor: Godot Engine (C#/.NET) 4.4.1
 - 👾 Lenguaje: C# 8.0.405
 - 📦 Gestión: Git + GitHub
 - 🪟 Multiplataforma: ¡El proyecto corre en Windows, Linux y macOS! 

---

## 🛠️ ¿Cómo usarlo? 🔓

1. **Descarga y ejecuta**
  - ⬇️ Clona este repositorio y descomprime el archivo `.zip` que contiene el ejecutable.

2. **Interfaz**

- 🖼️ **Dimensión del canvas**: para adaptar el espacio de dibujo puedes cambiar la dimensión del canvas cuadrado.
- ▶️ **Run**: este botón ejecuta el código escrito en el editor de texto.
- 💾 **Save**: para guardar tus códigos utiliza este botón.
- 🗂️ **Load**: para cargar archivos existentes en formato `.pw`.

---

## ⚙️ Acerca del lenguaje

El lenguaje de programación de Pixel Wall-E es un lenguaje de tipado dinámico, es fácil de escribir 
y visualmente muy intuitivo. Permite controlar al robot Wall-E con comandos simples que se traducen a dibujos
sobre el canvas. El lenguaje soporta instrucciones, asignación de variables, saltos
codicionales y funciones predefinidas. Después de cada comando debe existir un salto de línea.

## 1. Instrucciones 👾

  - **Spawn(int x, int y)**  
  Inicializa a Wall-E en la posición (x, y) del canvas (solo puede aparecer una vez y además debe ser en la primera línea).  
  - **Color(string color)**  
  Cambia el color del pincel.  
  - **Size(int k)**  
  Ajusta el grosor de la brocha a k píxeles.
  - **DrawLine(int dirX, int dirY, int distance)**  
  Traza una línea en la dirección indicada avanzando distance píxeles. El Wall-E acaba al final.
  - **DrawCircle(int dirX, int dirY, int radius)**  
  Dibuja una circunferencia de radio radius en la dirección indicada.  EL Wall-E acaba en el centro.
  - **DrawRectangle(int dirX, int dirY, int distance, int width, int height)**  
  Dibuja los bordes de un rectángulo centrado a distancia distance en la dirección dada. EL Wall-E acaba en el centro.
  - **Fill()**  
  Pinta con el color de brocha actual todos los píxeles del color de la posición actual que son alcanzables
  sin tener que caminar sobre algún otro color.

---

## 2. Asignación de variables ⬅️

- Soporta la asignación de variables aritméticas o booleanas.
- Sintaxis: var <_ Expression
- Una variables es numérica si su Expression es aritmética, y booleana si su Expression es booleana. Además
  var tiene forma de cadena de texto formada por los 27 caracteres del alfabeto español, caracteres numéricos
  y el símbolo _ pero no puede comenzar ni con un números ni con_.

---
## 3. Saltos condicionales 🏷️
- Un salto condicional tiene la forma:
- `GoTo[label](condition)`
- Label: etiqueta declarada en el código (sintaxis igual que el nombre de las variables). Por si
  solas no tienen efecto.
- Condition: es una variable booleana o una comparación. Si condition tiene valor verdadero
  el código continua su ejecución en la línea de la etiqueta correspondiente. Si tiene valor
  falso la línea se ignora.

## 4. Funciones 📈

- **GetActualX()**  
Retorna la fila actual del Wall-E.
- **GetActualY()**   
Retorna la columna actual del Wall-E.
- **GetCanvasSize()**  
Retorna la dimensión del canvas.
- **GetColorCount(string color, int x1, int y1, int x2, int y2)**  
Retorna la cantidad de píxeles (del color dado) que existen en un área específica.
- **IsBrushColor(string color)**  
Retorna 1 si el color de la brocha es el indicado y 0 en caso contrario.
- **IsBrushSize(int size)**  
Retorna 1 si el tamaño de la brocha es el indicado y 0 en caso contrario.
- **IsCanvasColor(string color, int vertical, int horizontal)**  
Retorna 1 si la casilla señalada está pintada del parámetro color y 0 en caso contrario.

---
## 👩‍🎨 ¡Libera tu creatividad creando pixel-art con Wall-E! 🤖
Realizado por **Yisell Velázquez Ochoa** 👩‍💻
[G153LL3](https://github.com/G153LL3)  

---
