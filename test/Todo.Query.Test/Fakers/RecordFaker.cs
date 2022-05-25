using Bogus;
using System.Runtime.Serialization;

namespace Todo.Query.Test.Fakers
{
    public class RecordFaker<T> : Faker<T> where T : class
    {
        public RecordFaker()
        {
            var type = FormatterServices.GetUninitializedObject(typeof(T)) as T ?? throw new TypeLoadException();
            CustomInstantiator(_ => type);
        }
    }
}
