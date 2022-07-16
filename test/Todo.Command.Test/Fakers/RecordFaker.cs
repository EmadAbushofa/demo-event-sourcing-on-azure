using Bogus;
using System.Runtime.Serialization;

namespace Todo.Command.Test.Fakers
{
    public class RecordFaker<T> : Faker<T> where T : class
    {
        public RecordFaker()
        {
            CustomInstantiator(_ => Initialize());
        }

        private static T Initialize()
        {
            return FormatterServices.GetUninitializedObject(typeof(T)) as T ?? throw new TypeLoadException();
        }
    }
}
