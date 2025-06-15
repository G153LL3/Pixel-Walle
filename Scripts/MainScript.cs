using Godot;

public partial class MainScript : Node
{
    public CodeEdit miEditor;
    public Button miBoton;

    public override void _Ready()
    { 
        miEditor = GetNode<CodeEdit>("CodeEdit");
        miBoton = GetNode<Button>("RunButton");
        miBoton.Pressed += EjecutarCodigo;
        Program.Initialize();
    }

    private void EjecutarCodigo()
    {
        string codigo = miEditor.Text;
        string resultado = Program.Execute(codigo);

        GD.Print("Resultado de la ejecución:\n" + resultado);

    }
    
}