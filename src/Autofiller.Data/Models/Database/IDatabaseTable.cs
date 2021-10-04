using System;

namespace Autofiller.Data.Models.Database
{
    public interface IDatabaseTable
    {
        public string Table { get; }

        public void SaveToDatabase();
        public void LoadFromDatabase();
    }
    public interface IDatabaseTable<T>
    {
        public string Table { get; }
        public T Data { get; set; }
        public void SaveToDatabase();
        public void LoadFromDatabase();
    }
}
