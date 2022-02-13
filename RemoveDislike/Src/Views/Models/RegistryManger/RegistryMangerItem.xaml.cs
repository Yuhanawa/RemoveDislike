namespace RemoveDislike.Views.Models.RegistryManger;

public partial class RegistryMangerItem
{
    public delegate bool GetRegValueDelegate();

    private readonly GetRegValueDelegate GetRegValue;


    public RegistryMangerItem(GetRegValueDelegate getRegValue, Action<bool> setRegValue)
    {
        GetRegValue = getRegValue;
        SetRegValue = setRegValue;
        DataContext = this;
        InitializeComponent();
    }

    public RegistryMangerItem(string name, string description, GetRegValueDelegate getRegValue,
        Action<bool> setRegValue)
    {
        RegName = name;
        Description = description;
        GetRegValue = getRegValue;
        SetRegValue = setRegValue;
        DataContext = this;

        InitializeComponent();
    }

    public string RegName { get; set; } = "you shouldn't see it";

    public string Description { get; set; }

    public bool RegValue
    {
        get => GetRegValue();
        set => SetRegValue(value);
    }

    private Action<bool> SetRegValue { get; }
}