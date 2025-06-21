using Godot;

public partial class MainScript : Node
{
    private Canvas TargetCanvas;
    public CodeEdit codeEdit; 

    public Button runButton;
    public Button saveButton;
    public Button loadButton;

    SpinBox spinBox;

    public FileDialog fileDialogSave;
	public FileDialog fileDialogLoad;

    public override void _Ready()
    { 
        codeEdit = GetNode<CodeEdit>("CodeEdit");

        runButton = GetNode<Button>("RunButton");
		saveButton = GetNode<Button>("SaveButton");
		loadButton = GetNode<Button>("LoadButton");

        TargetCanvas = GetNode<Canvas>("TextureRect");

        spinBox = GetNode<SpinBox>("SpinBox");

        runButton.Pressed += OnRunPressed;
		saveButton.Pressed += OnSavePressed;
		loadButton.Pressed += OnLoadPressed;


        fileDialogSave = new FileDialog();
        fileDialogSave.Access = FileDialog.AccessEnum.Filesystem;
        fileDialogSave.FileMode = FileDialog.FileModeEnum.SaveFile;
        fileDialogSave.Filters = new string[] { "*.pw" };
		AddChild(fileDialogSave);
		fileDialogSave.FileSelected += _OnFileDialogSaveFileSelected;

        fileDialogLoad = new FileDialog();
		fileDialogLoad.Access = FileDialog.AccessEnum.Filesystem;
		fileDialogLoad.FileMode = FileDialog.FileModeEnum.OpenAny;
		fileDialogLoad.Filters = new string[] { "*.pw" };
        AddChild(fileDialogLoad);
		fileDialogLoad.FileSelected += _OnFileDialogLoadFileSelected;
	

        Program.Initialize();
    }
    private void _on_spin_box_value_changed(float value)
    {
        Canvas.GridSize = (int)value; // actualizo el tamaño del canvas
        Program.UpdateCanvasSize();  // actualiza el interprete en el cual se usa tbn el tamaño del canvas
        TargetCanvas.UpdateGridSize((int)value); // actauliza el canvas
    }

    private void OnRunPressed()
    {
        string codigo = codeEdit.Text;
        string resultado = Program.Execute(codigo);
        GD.Print("Resultado de la ejecución:\n" + resultado);
    }
    public void OnSavePressed()
	{
        fileDialogSave.PopupCentered();
        fileDialogSave.Size = new Vector2I(600, 400); 
	}
    public void OnLoadPressed()
	{
        fileDialogLoad.PopupCentered();
        fileDialogLoad.Size = new Vector2I(600, 400);

	}
    private void _OnFileDialogSaveFileSelected(string ruta)
    {
        GuardarArchivo(ruta);
        GD.Print("Archivo guardado en: " + ruta);
    }
    private void _OnFileDialogLoadFileSelected(string ruta)
    {
        CargarArchivo(ruta);
        GD.Print("Archivo cargado desde: " + ruta);
    }
    private void GuardarArchivo(string ruta)
	{
		using var archivo = FileAccess.Open(ruta, FileAccess.ModeFlags.Write);
        if (archivo != null)
        {
            archivo.StoreString(codeEdit.Text);
            GD.Print("Guardado exitoso");
        }
        else
        {
            GD.PrintErr("Error al guardar: " + FileAccess.GetOpenError());
        }
	}
    private void CargarArchivo(string ruta)
    {
         using var archivo = FileAccess.Open(ruta, FileAccess.ModeFlags.Read);
        if (archivo != null)
        {
            codeEdit.Text = archivo.GetAsText();
            GD.Print("Carga exitosa");
        }
        else
        {
            GD.PrintErr("Error al cargar: " + FileAccess.GetOpenError());
        }
    }
    
}