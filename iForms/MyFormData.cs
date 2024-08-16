using SKitLs.Data.InputForms.InputParts;
using SKitLs.Data.InputForms.Notations;
using SKitLs.Data.InputForms.Notations.Refs;
using System.Diagnostics.SymbolStore;

namespace Tester.iForms
{
    public enum Profession
    {
        Unknown, Doctor, Fireman, Programmer
    }

    public class MyFormData
    {
        [TextInput()]
        [DependentReference(nameof(OnNamePicked), false, [nameof(Age)])]
        public string Name { get; set; } = "None";

        [IntInput(18, 99)]
        public int Age { get; set; } = 18;

        [SelectData(nameof(GetProfessions))]
        public Profession Profession { get; set; } = Profession.Unknown;

        public List<SelectValue> GetProfessions() => [ new("d", Profession.Doctor), new("f", Profession.Fireman), new("p", Profession.Programmer) ];

        public void OnNamePicked(InputPartBase master, InputPartBase slave)
        {
            if (master.InputValue?.ToString()?.Contains('j') ?? false && slave.PropertyInfo.Name == nameof(Age))
                (slave.Meta as IntInputAttribute)!.MinValue = 24;
        }

        public override string ToString() => $"Name: {Name}\nAge: {Age}\nProfession: {Profession}";
    }

    public class MyFormData1
    {
        [TextInput("Name", "Enter your name")]
        public string Name { get; set; } = "None";

        [IntInput("Age", "Enter your age", 18, 99)]
        public int Age { get; set; } = 18;

        [SelectData("Profession", "Select your profession (d - doctor, f - fireman, p - programmer)", nameof(GetProfessions))]
        public Profession Profession { get; set; } = Profession.Unknown;

        public List<SelectValue> GetProfessions() => [ new("d", Profession.Doctor), new("f", Profession.Fireman), new("p", Profession.Programmer) ];

        public override string ToString() => $"Name: {Name}\nAge: {Age}\nProfession: {Profession}";
    }
}