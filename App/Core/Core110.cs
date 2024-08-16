using SKitLs.Data.Core;
using SKitLs.Data.Core.Banks;
using SKitLs.Data.Core.IdGenerator;
using SKitLs.Data.IO;
using SKitLs.Utils.Extensions.Strings;

namespace Tester.App.Core
{
    public class Core110
    {
        public static IDataManager DataManager { get; private set; }

        public static void Run()
        {
            DataManager = new MyDataManager("core/data/");
            DataManager.Initialize();

            var resolvedSubjectsBank = DataManager.ResolveBank<long, SubjectModelDso>();
            var newSubject = resolvedSubjectsBank.BuildNewData();
            newSubject.Name = "Mathematics";
            newSubject.Description = "Basic Mathematics course";
            resolvedSubjectsBank.UpdateSave(newSubject);

            //var resolvedSubjectsBank = DataManager.ResolveBank<SubjectModelDso>();
            var subject = resolvedSubjectsBank.GetValue(1);
            subject.Description = "Advanced Mathematics course";
            resolvedSubjectsBank.UpdateSave(subject);
            Console.WriteLine(resolvedSubjectsBank.Count);

            resolvedSubjectsBank.DropSave(subject);
            Console.WriteLine(resolvedSubjectsBank.Count);

            var banks = DataManager.GetBanks();
            var notations = DataManager.GetNotations().Select(x => x.Name + " " + x.Count);
            Console.WriteLine("Declared banks: " + notations.JoinAll(", "));
        }

        public class MyDataManager(string dataFolderPath) : DataManager(dataFolderPath)
        {
            public static long DefaultLongId { get; private set; } = -1;

            public override void InitializeDeclarations()
            {
                base.InitializeDeclarations();

                var solid = new SolidLongIdGen(DefaultLongId);
                var guid = new GuidIdGenerator();

                // -> dataFolderPath/Subjects.json
                var subjectsBank = JsonBank<long, SubjectModelDso>(solid, "Subjects", "Taught Subjects");
                subjectsBank.OnBankDataUpdated += (count) =>
                {
                    Console.WriteLine($"{count} subject(s) updated.");
                };
                JsonBank<long, ClassModelDso>(solid, "Classes", "Classes, Groups, and Subgroups", DropStrategy.Disable);
                JsonBank<Guid, TeacherModelDso>(guid, "Teachers", "Teachers and Their Schedules", DropStrategy.Delete);

                // Declarations: v1.0 /vs/ v1.1

                // >> Quick setup for JSON files
                //var subjectsBank = JsonBank<long, SubjectModelDso>(solid, "Subjects", "Taught Subjects");

                // >> Instead of full declaration
                //var subjectsBank = new DataBank<long, SubjectModelDso>("Subjects", "Taught Subjects", DropStrategy.Disable)
                //{
                //    Reader = new JsonReader<SubjectModelDso>(DataFolderPath + "subjects.json") { CreateNewFile = true },
                //    Writer = new JsonWriter<SubjectModelDso, long>(DataFolderPath + "subjects.json"),
                //    IdGenerator = new SolidLongIdGen(DefaultLongId),
                //    NewInstanceGenerator = () => new SubjectModelDso(),
                //};
                
                //subjectsBank.OnBankDataUpdated += (count) =>
                //{
                //    Console.WriteLine($"{count} subject(s) updated.");
                //};

                // >> Same for the ef
                // EfBank<TId, TData>(context: , idGenerator: , dbName: , dbDescription: , dropStrategy: );

                // >> Not required: manual Declare()
                //DataManager.Declare(subjectsBank);
                //DataManager.Declare(classesBank);
                //DataManager.Declare(teachersBank);
            }
        }

        public class SubjectModelDso : ModelDso<long>
        {
            public long Id { get; set; }
            public override long GetId() => Id;
            public override void SetId(long id) => Id = id;

            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class ClassModelDso : ModelDso<long>
        {
            public long Id { get; set; }
            public override long GetId() => Id;
            public override void SetId(long id) => Id = id;

            public string ClassName { get; set; }
            public int Grade { get; set; }
        }

        public class TeacherModelDso : ModelDso<Guid>
        {
            public Guid Id { get; set; }
            public override Guid GetId() => Id;
            public override void SetId(Guid id) => Id = id;

            public string FullName { get; set; }
            public string Subject { get; set; }
        }
    }
}