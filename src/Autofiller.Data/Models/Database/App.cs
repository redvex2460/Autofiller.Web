namespace Autofiller.Data.Models.Database
{
    public class App : IDatabaseObject
    {
        public App()
        {

        }
        public App(long appId, string name)
        {
            AppId = appId;
            Name = name;
        }

        public long AppId { get; set; }
        public string Name { get; set; }

        public string Table => "Steam_Apps";

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}