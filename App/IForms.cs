using SKitLs.Data.InputForms;
using SKitLs.Utils.Localizations.Languages;
using SKitLs.Utils.Localizations.Model;
using Tester.iForms;

namespace Tester.App
{
    internal class IForms
    {
        public static InputForm<MyFormData> MyInputForm { get; set; } = new InputForm<MyFormData>(new());

        public static void Run()
        {
            var localizator = new StoredLocalizator("Resources/Locals");
            InputForm.DefaultLocalizator = localizator;
            InputForm.DefaultLanguage = LanguageCode.EN;
            MyInputForm = new InputForm<MyFormData>(new(), localizator: localizator);

            Console.WriteLine($"The input form fields:\n{string.Join("\n", MyInputForm.GetDefinedParts().Select(x => $"- {x.InputCaption}: {x.GetInputValue()}"))}\n");
            Console.WriteLine($"The form data fields:\n{MyInputForm.FormData}");
            Console.WriteLine();

            foreach (var field in MyInputForm.GetDefinedParts())
            {
                var ok = false;
                while (!ok)
                {
                    Console.WriteLine(field.InputCaption);
                    Console.Write(field.InputDescription + ": ");
                    var value = Console.ReadLine();
                    var errorMessage = field.Preview(value, null);
                    if (errorMessage is not null)
                    {
                        Console.WriteLine(errorMessage);
                    }
                    else
                    {
                        field.InputValue = value;
                        ok = true;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"The input form fields:\n{string.Join("\n", MyInputForm.GetDefinedParts().Select(x => $"- {x.InputCaption}: {x.GetInputValue()}"))}\n");
            Console.WriteLine($"The form data fields:\n{MyInputForm.FormData}");
            Console.WriteLine();

            Console.WriteLine("T ApplyChanges()");
            var updatedData = MyInputForm.ApplyChanges();
            Console.WriteLine($"The form data fields:\n{MyInputForm.FormData}");
            Console.WriteLine($"The form data fields:\n{updatedData}");
            // example: updatedData.Save()
        }
    }
}