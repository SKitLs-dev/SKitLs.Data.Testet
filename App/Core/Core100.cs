using SKitLs.Data.Core;
using SKitLs.Data.Core.Banks;
using SKitLs.Data.Core.IdGenerator;
using SKitLs.Data.IO;
using SKitLs.Data.IO.Json;
using SKitLs.Utils.Extensions.Strings;

namespace Tester.App.Core
{
    public static class Core100
    {
        public static IDataManager DataManager { get; private set; }

        public static long DefaultLongId { get; private set; } = -1;

        public static void Run()
        {
            // v1.0
            // DataManager = new DataManager();

            var path = "core/data/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var subjectsBank = new DataBank<long, SubjectModelDso>("Subjects", "Taught Subjects", DropStrategy.Disable)
            {
                Reader = new JsonReader<SubjectModelDso>(path + "subjects.json") { CreateNewFile = true },
                Writer = new JsonWriter<SubjectModelDso, long>(path + "subjects.json"),
                IdGenerator = new SolidLongIdGen(DefaultLongId),
                NewInstanceGenerator = () => new SubjectModelDso(),
            };

            var classesBank = new DataBank<long, ClassModelDso>("Classes", "Classes, Groups, and Subgroups", DropStrategy.Disable)
            {
                Reader = new JsonReader<ClassModelDso>(path + "classes.json") { CreateNewFile = true },
                Writer = new JsonWriter<ClassModelDso, long>(path + "classes.json"),
                IdGenerator = new SolidLongIdGen(DefaultLongId),
                NewInstanceGenerator = () => new ClassModelDso(),
            };

            var teachersBank = new DataBank<Guid, TeacherModelDso>("Teachers", "Teachers and Their Schedules", DropStrategy.Delete)
            {
                Reader = new JsonReader<TeacherModelDso>(path + "teachers.json") { CreateNewFile = true },
                Writer = new JsonWriter<TeacherModelDso, Guid>(path + "teachers.json"),
                IdGenerator = new GuidIdGenerator(),
                NewInstanceGenerator = () => new TeacherModelDso(),
            };

            DataManager.Declare(subjectsBank);
            subjectsBank.OnBankDataUpdated += (count) =>
            {
                Console.WriteLine($"{count} subject(s) updated.");
            };

            DataManager.Declare(classesBank);
            DataManager.Declare(teachersBank);

            DataManager.Initialize();


            var resolvedSubjectsBank = DataManager.ResolveBank<long, SubjectModelDso>();
            var newSubject = resolvedSubjectsBank.BuildNewData();
            newSubject.Name = "Mathematics";
            newSubject.Description = "Basic Mathematics course";
            resolvedSubjectsBank.UpdateSave(newSubject);
            Console.WriteLine(resolvedSubjectsBank.Count);

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